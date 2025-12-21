using static SsdeepNET.Constants;

namespace SsdeepNET
{
    /// <summary>
    /// A blockhash contains a signature state for a specific (implicit) blocksize. The blocksize
    /// is equal to <see cref="MinBlocksize"/> &lt;&lt; index. The <see cref="H"/> and
    /// <see cref="HalfH"/> members are the FNV hashes, where <see cref="HalfH"/> stops to be reset
    /// after digest is <see cref="SpamSumLength"/> long. The <see cref="HalfH"/> hash is needed be
    /// able to truncate digest for the second output hash to stay compatible with ssdeep output.
    /// </summary>
    sealed class BlockhashContext
    {
        const int HashPrime = 0x01000193;
        const int HashInit = 0x28021967;

        public uint H;
        public uint HalfH;
        public byte[] Digest = new byte[SpamSumLength];
        public byte HalfDigest;

        public int DigestLen;

        public BlockhashContext()
            : this(HashInit, HashInit)
        {
        }

        public BlockhashContext(uint h, uint halfH)
        {
            H = h;
            HalfH = halfH;
        }

        public void Hash(byte c)
        {
            H = Hash(c, H);
            HalfH = Hash(c, HalfH);
        }

        /// <summary>
        /// A simple non-rolling hash, based on the FNV hash.
        /// </summary>
        private static uint Hash(byte c, uint h)
        {
            return (h * HashPrime) ^ c;
        }

        public void Reset()
        {
            ++DigestLen;
            H = HashInit;
            if (DigestLen < SpamSumLength / 2)
            {
                HalfH = HashInit;
                HalfDigest = 0;
            }
        }
    }
}
