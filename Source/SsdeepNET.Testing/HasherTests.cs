using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace SsdeepNET.Testing
{
    public class HasherTests
    {
        private static void Test(byte[] input, string expectedHash, FuzzyHashMode mode = FuzzyHashMode.None)
        {
            var actualHash = Hasher.HashBuffer(input, input.Length, mode);
            Assert.Equal(expectedHash, actualHash);
        }

        private static void Test(string input, string expectedHash, FuzzyHashMode mode = FuzzyHashMode.None) =>
            Test(Encoding.UTF8.GetBytes(input), expectedHash, mode);

        private static void TestRandom(int seed, int count, string expectedHash, FuzzyHashMode mode = FuzzyHashMode.None)
        {
            var rand = new Random(seed);
            var buffer = new byte[count];

            for (int i = 0; i < count; i++)
            {
                buffer[i] = (byte)rand.Next(255);
            }

            Test(buffer, expectedHash);
        }

        [Fact(Skip = "TODO")]
        public void Test1()
        {
            Assert.Throws<ArgumentNullException>(() => Hasher.HashBuffer(null, 0));
            Assert.Throws<ArgumentException>(() => Hasher.HashBuffer(Array.Empty<byte>(), 10));
        }

        [Fact]
        public void Test2() =>
            Test("", "3::");

        [Fact]
        public void Test3a() =>
            Test("a", "3:E:E");

        [Fact]
        public void Test3b() =>
            Test(
                string.Join("", Enumerable.Repeat("ab", 10)),
                "3:ue:ue");

        [Fact]
        public void Test3c() =>
            Test(
                string.Join("", Enumerable.Repeat("ab", 10000)),
                "3:ui:ui");

        [Fact]
        public void Test4a() =>
            TestRandom(1, 100, "3:S9bQXymJwY25U7uPF3IEgUYpkKgpX:S9QJt2fPF3IJ9GN");

        [Fact]
        public void Test4b() =>
            TestRandom(1, 1000, "24:fugVcFvzcVXAdKdCMghIsHSEimsGmE/JEaIuZtMKDll:mgscV7PUeC/JHIufMgl");

        [Fact]
        public void Test4c() =>
            TestRandom(1, 10000, "192:/rtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:/5+7eVvwUL7gKCpzNSbl");
    }
}
