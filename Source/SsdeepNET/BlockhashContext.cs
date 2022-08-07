using System;

namespace SsdeepNET
{
    /// <summary>
    /// A blockhash contains a signature state for a specific (implicit) blocksize.
    /// The blocksize is given by SSDEEP_BS(index). The h and halfh members are the
    /// FNV hashes, where halfh stops to be reset after digest is SPAMSUM_LENGTH/2
    /// long. The halfh hash is needed be able to truncate digest for the second
    /// output hash to stay compatible with ssdeep output.
    /// </summary>
    sealed class BlockhashContext
    {
        const int HashPrime = 0x01000193;
        const int HashInit = 0x28021967;

        public uint H = HashInit;
        public uint HalfH = HashInit;
        public byte[] Digest = new byte[FuzzyConstants.SpamSumLength];
        public byte HalfDigest;

        public uint DLen;

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
            ++DLen;
            H = HashInit;
            if (DLen < FuzzyConstants.SpamSumLength / 2)
            {
                HalfH = HashInit;
                HalfDigest = 0;
            }
        }
    }
}
