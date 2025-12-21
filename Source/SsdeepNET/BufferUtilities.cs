using System;

namespace SsdeepNET
{
    static class BufferUtilities
    {
        public static int EliminateSequences<T>(T[] src, int srcOffset, T[]? dst, int dstOffset, int length, int window)
            where T : IEquatable<T>
        {
            return EliminateSequences(src.AsSpan(srcOffset, length), dst.AsSpan(dstOffset), window);
        }

        public static int EliminateSequences<T>(Span<T> src, Span<T> dst, int window)
            where T : IEquatable<T>
        {
            for (int i = 0; i < window; i++)
                dst[i] = src[i];

            int length = src.Length;
            if (length <= window)
                return length;

            int j = window;
            for (int i = j; i < length; i++)
            {
                var current = src[i];
                bool duplicate = true;
                for (int w = 1; w <= window && duplicate; w++)
                {
                    if (!current.Equals(src[i - w]))
                    {
                        duplicate = false;
                        break;
                    }
                }

                if (!duplicate)
                {
                    dst[j] = src[i];
                    j++;
                }
            }

            return j;
        }
        public static int EliminateSequencesDryRun<T>(Span<T> buffer, int window)
            where T : IEquatable<T>
        {
            int length = buffer.Length;
            if (length <= window)
                return length;

            int j = window;
            for (int i = j; i < length; i++)
            {
                var current = buffer[i];
                bool duplicate = true;
                for (int w = 1; w <= window && duplicate; w++)
                {
                    if (!current.Equals(buffer[i - w]))
                    {
                        duplicate = false;
                        break;
                    }
                }

                if (!duplicate)
                    j++;
            }

            return j;
        }
    }
}
