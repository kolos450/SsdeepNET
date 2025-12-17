using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace SsdeepNET.Testing
{
    public class HasherTests(ITestOutputHelper Output)
    {
        private static void Test(byte[] input, string expectedHash, FuzzyHashFlags mode = FuzzyHashFlags.None)
        {
            var hash = new FuzzyHash(mode);
            var actualHash = hash.ComputeHash(input);
            Assert.Equal(expectedHash, actualHash);
        }

        private static void Test(string input, string expectedHash, FuzzyHashFlags mode = FuzzyHashFlags.None) =>
            Test(Encoding.UTF8.GetBytes(input), expectedHash, mode);

        private static void TestRandom(int seed, int count, string expectedHash, FuzzyHashFlags mode = FuzzyHashFlags.None)
        {
            var rand = new Random(seed);
            var buffer = new byte[count];

            for (int i = 0; i < count; i++)
            {
                buffer[i] = (byte)rand.Next(255);
            }

            Test(buffer, expectedHash);
        }

        private void TestSpeed(string fileName, int maxDurationMilliseconds, FuzzyHashFlags mode = FuzzyHashFlags.None)
        {
            var hash = new FuzzyHash(mode);
            //FileStream fileStream = new(fileName, FileMode.Open);
            var fileStream = File.ReadAllBytes(fileName);

            var sw = Stopwatch.StartNew();
            var actualHash = hash.ComputeHash(fileStream);
            sw.Stop();

            Output.WriteLine($"Computed hash: {actualHash}");
            Assert.True(sw.ElapsedMilliseconds < maxDurationMilliseconds, $"Fail, Call took {sw.ElapsedMilliseconds} ms");
            Output.WriteLine($"Call took {sw.ElapsedMilliseconds} ms");
             
        }

        [Fact]
        public void Test1()
        {
            Assert.Throws<ArgumentNullException>(() => new FuzzyHash().ComputeHash(null, 0));
            Assert.Throws<ArgumentException>(() => new FuzzyHash().ComputeHash(Array.Empty<byte>(), 10));
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

        [Fact]
        public void TestImage() => 
            Test(File.ReadAllBytes("/home/nils/Documents/me.jpg"), "1536:JyTO1GCxFMptvnmi3gU3hvFXLsbc+NmgAlGU89gE2QOyvTTP:+OhxyCiD3hdsbc3Z589g7KTb");
        
        //Important RUN with Release build (over 10 times faster)
        // [Fact]
        // public void TestDurationLargeFile() => 
        //     TestSpeed("/somefile", 5000);
    }
}
