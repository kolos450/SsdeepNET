using System.Runtime.CompilerServices;

namespace SsdeepNET
{
    /// <summary>
    /// A blockhash contains a signature state for a specific (implicit) blocksize.
    /// The blocksize is given by SSDEEP_BS(index). The h and halfh members are the
    /// FNV hashes, where halfh stops to be reset after digest is SPAMSUM_LENGTH/2
    /// long. The halfh hash is needed be able to truncate digest for the second
    /// output hash to stay compatible with ssdeep output.
    /// </summary>
    internal struct BlockhashContext
    {
        const int HashPrime = 0x01000193;
        public const int HashInit = 0x28021967;

        public uint H ;
        public uint HalfH;
        public byte[] Digest;
        public byte HalfDigest;

        public uint DigestLen;

        public BlockhashContext(uint h, uint halfH)
        {
            H = h;
            HalfH = halfH;
            HalfDigest = 0;
            DigestLen = 0;
            Digest = new byte[Constants.SpamSumLength];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Hash(byte c)
        {
            H = (H * HashPrime) ^ c;
            HalfH = (HalfH * HashPrime) ^ c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            DigestLen++;
            H = HashInit;
            if (DigestLen < Constants.SpamSumLength / 2)
            {
                HalfH = HashInit;
                HalfDigest = 0;
            }
        }
    }
}
