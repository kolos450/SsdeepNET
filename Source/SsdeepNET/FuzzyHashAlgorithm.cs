using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

using static SsdeepNET.Constants;

namespace SsdeepNET
{
    sealed class FuzzyHashAlgorithm : HashAlgorithm
    {
        private static readonly byte[] Base64;

        static FuzzyHashAlgorithm()
        {
            var b64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            Base64 = new byte[b64Chars.Length];
            for (int i = 0; i < b64Chars.Length; i++)
                Base64[i] = (byte)b64Chars[i];
        }

        private int _bhstart;
        private BlockhashContext[]? _bh;
        private int _bhCount;
        private int _totalSize;
        private Roll _roll;

        public FuzzyHashFlags Flags { get; }

        public FuzzyHashAlgorithm(FuzzyHashFlags flags)
        {
            Flags = flags;
        }

        [MemberNotNull(nameof(_bh))]
        void EnsureInitialized()
        {
            if (_bh is null)
            {
                _bh = new BlockhashContext[NumBlockhashes];
                _bh[0] = new BlockhashContext();
                _bhCount = 1;
                _bhstart = 0;
                _totalSize = 0;
                _roll = new();
            }
        }

        public override void Initialize()
        {
            _bh = null;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            var memory = new ReadOnlySpan<byte>(array, ibStart, cbSize);
            HashCore(memory);
        }

        protected override byte[] HashFinal()
        {
            var segment = HashFinalCore();
            var buffer = new byte[segment.Count];
            Buffer.BlockCopy(segment.Array, segment.Offset, buffer, 0, segment.Count);
            return buffer;
        }

        internal ArraySegment<byte> HashFinalCore()
        {
            EnsureInitialized();

            var result = new byte[MaxResultLength];
            var pos = 0;

            int bi = _bhstart;
            uint h = _roll.Sum;

            // Initial blocksize guess.
            while ((MinBlocksize << bi) * SpamSumLength < _totalSize)
            {
                ++bi;
                if (bi >= NumBlockhashes)
                    throw new OverflowException("Blockhashes number overflow.");
            }

            // Adapt blocksize guess to actual digest length.
            bi = Math.Min(_bhCount - 1, bi);

            while (bi > _bhstart && _bh[bi].DigestLen < SpamSumLength / 2)
                --bi;

            var actualBlockSize = ((uint)MinBlocksize) << bi;
            var blockSizeChars = actualBlockSize.ToString().ToCharArray();
            int i = blockSizeChars.Length;
            for (int j = 0; j < i; j++)
                result[j + pos] = (byte)blockSizeChars[j];
            result[i++] = (byte)':';

            pos += i;

            var eliminateSequences = Flags.HasFlag(FuzzyHashFlags.EliminateSequences);
            var dontTruncate = Flags.HasFlag(FuzzyHashFlags.DoNotTruncate);

            i = (int)_bh[bi].DigestLen;
            if (eliminateSequences)
                i = BufferUtilities.EliminateSequences(_bh[bi].Digest, 0, result, pos, i, SequencesToEliminateLength);
            else
                Buffer.BlockCopy(_bh[bi].Digest, 0, result, pos, i);
            pos += i;

            if (h != 0)
            {
                var base64Val = Base64[_bh[bi].H % 64];
                result[pos] = base64Val;
                if (!eliminateSequences || i < 3 || base64Val != result[pos - 1] || base64Val != result[pos - 2] || base64Val != result[pos - 3])
                    ++pos;
            }
            else if (_bh[bi].Digest[i] != 0)
            {
                var digestVal = _bh[bi].Digest[i];
                result[pos] = digestVal;
                if (!eliminateSequences || i < 3 || digestVal != result[pos - 1] || digestVal != result[pos - 2] || digestVal != result[pos - 3])
                    ++pos;
            }
            result[pos++] = (byte)':';

            if (bi < _bhCount - 1)
            {
                ++bi;
                i = (int)_bh[bi].DigestLen;
                if (!dontTruncate && i > SpamSumLength / 2 - 1)
                    i = SpamSumLength / 2 - 1;

                if (eliminateSequences)
                    i = BufferUtilities.EliminateSequences(_bh[bi].Digest, 0, result, pos, i, SequencesToEliminateLength);
                else
                    Buffer.BlockCopy(_bh[bi].Digest, 0, result, pos, i);

                pos += i;
                if (h != 0)
                {
                    h = dontTruncate ? _bh[bi].H : _bh[bi].HalfH;
                    var base64Val = Base64[h % 64];
                    result[pos] = base64Val;
                    if (!eliminateSequences || i < 3 || base64Val != result[pos - 1] || base64Val != result[pos - 2] || base64Val != result[pos - 3])
                        ++pos;
                }
                else
                {
                    i = dontTruncate ? _bh[bi].Digest[_bh[bi].DigestLen] : _bh[bi].HalfDigest;
                    if (i != 0)
                    {
                        result[pos] = (byte)i;
                        if (!eliminateSequences || i < 3 || i != result[pos - 1] || i != result[pos - 2] || i != result[pos - 3])
                            ++pos;
                    }
                }
            }
            else if (h != 0)
            {
                result[pos++] = Base64[_bh[bi].H % 64];
                // No need to bother with FuzzyHashMode.EliminateSequences, because this digest has length 1.
            }

            return new ArraySegment<byte>(result, 0, pos);
        }

        private void TryForkBlockhash()
        {
            if (_bhCount < NumBlockhashes)
            {
                var last = _bh![_bhCount - 1];
                _bh[_bhCount++] = new BlockhashContext(last.H, last.HalfH);
            }
        }

        private void TryReduceBlockhash()
        {
            if (_bhCount - _bhstart < 2)
                // Need at least two working hashes.
                return;
            if ((((uint)MinBlocksize) << _bhstart) * SpamSumLength >= _totalSize)
                // Initial blocksize estimate would select this or a smaller blocksize.
                return;
            if (_bh![_bhstart + 1].DigestLen < SpamSumLength / 2)
                // Estimate adjustment would select this blocksize.
                return;
            // At this point we are clearly no longer interested in the start_blocksize. Get rid of it.
            ++_bhstart;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EngineStep(byte c)
        {
            // At each character we update the rolling hash and the normal hashes.
            // When the rolling hash hits a reset value then we emit a normal hash
            // as a element of the signature and reset the normal hash.
            _roll.Hash(c);
            uint h = _roll.Sum;

            for (int i = _bhstart; i < _bhCount; ++i)
                _bh![i].Hash(c);

            for (int i = _bhstart; i < _bhCount; ++i)
            {
                // With growing blocksize almost no runs fail the next test.
                if (h % (((uint)MinBlocksize) << i) != (((uint)MinBlocksize) << i) - 1)
                    // Once this condition is false for one bs, it is
                    // automatically false for all further bs. I.e. if
                    // h === -1 (mod 2*bs) then h === -1 (mod bs).
                    break;
                // We have hit a reset point. We now emit hashes which are
                // based on all characters in the piece of the message between
                // the last reset point and this one.
                if (0 == _bh![i].DigestLen)
                {
                    // Can only happen 30 times.
                    // First step for this blocksize. Clone next.
                    TryForkBlockhash();
                }
                _bh[i].Digest[_bh[i].DigestLen] = Base64[_bh[i].H % 64];
                _bh[i].HalfDigest = Base64[_bh[i].HalfH % 64];
                if (_bh[i].DigestLen < SpamSumLength - 1)
                {
                    // We can have a problem with the tail overflowing. The
                    // easiest way to cope with this is to only reset the
                    // normal hash if we have room for more characters in
                    // our signature. This has the effect of combining the
                    // last few pieces of the message into a single piece.
                    _bh[i].Reset();
                }
                else
                {
                    TryReduceBlockhash();
                }
            }
        }

        internal void HashCore(ReadOnlySpan<byte> span)
        {
            EnsureInitialized();

            _totalSize += span.Length;

            for (int i = 0; i < span.Length; i++)
                EngineStep(span[i]);
        }

        public override int HashSize =>
            throw new NotSupportedException("Hash size is variable.");
    }
}
