using System;
using Xunit;

namespace SsdeepNET.Testing
{
    public class ComparerTests
    {
        [Fact]
        public void Test1()
        {
            Assert.Throws<ArgumentNullException>(() => new FuzzyHash().CompareHashes(null, ""));
            Assert.Throws<ArgumentNullException>(() => new FuzzyHash().CompareHashes("", null));
            Assert.Throws<ArgumentException>(() => new FuzzyHash().CompareHashes("", "3:E:E"));
            Assert.Throws<ArgumentException>(() => new FuzzyHash().CompareHashes("3:E:E", ""));
            Assert.Throws<ArgumentException>(() => new FuzzyHash().CompareHashes("", "3::"));
            Assert.Throws<ArgumentException>(() => new FuzzyHash().CompareHashes("3::", ""));
        }

        [Fact]
        public void Test2a() =>
            Assert.Equal(100, new FuzzyHash().CompareHashes("3:E:E", "3:E:E"));

        [Fact]
        public void Test2b() =>
            Assert.Equal(0, new FuzzyHash().CompareHashes("3:E:E", "3:ue:ue"));

        [Fact]
        public void Test2c() =>
            Assert.Equal(0, new FuzzyHash().CompareHashes("3:ue:ue", "3:E:E"));

        [Fact]
        public void Test2d() =>
            Assert.Equal(0, new FuzzyHash().CompareHashes("3:E:E", "3:ui:ui"));

        [Fact]
        public void Test2e() =>
            Assert.Equal(0, new FuzzyHash().CompareHashes("3:E:E", "192:/rtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:/5+7eVvwUL7gKCpzNSbl"));

        [Fact]
        public void Test2f() =>
            Assert.Equal(99, new FuzzyHash().CompareHashes(
                "192:sdrtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:I5+7eVvwUL7gKCpzNSbl",
                "192:/rtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:/5+7eVvwUL7gKCpzNSbl"));

        [Fact]
        public void Test2g() =>
            Assert.Equal(99, new FuzzyHash().CompareHashes(
                "192:/rtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:/5+7eVvwUL7gKCpzNSbl",
                "192:sdrtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:I5+7eVvwUL7gKCpzNSbl"));

        [Fact]
        public void Test2h() =>
            Assert.Equal(100, new FuzzyHash().CompareHashes(
                "192:/rtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:/5+7eVvwUL7gKCpzNSbl",
                "192:/rtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:/5+7eVvwUL7gKCpzNSbl"));
    }
}
