using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SsdeepNET.Testing
{
    public class ComparerTests
    {
        [Fact]
        public void Test1()
        {
            Assert.Throws<ArgumentNullException>(() => Comparer.Compare(null, ""));
            Assert.Throws<ArgumentNullException>(() => Comparer.Compare("", null));
            Assert.Throws<ArgumentException>(() => Comparer.Compare("", "3:E:E"));
            Assert.Throws<ArgumentException>(() => Comparer.Compare("3:E:E", ""));
            Assert.Throws<ArgumentException>(() => Comparer.Compare("", "3::"));
            Assert.Throws<ArgumentException>(() => Comparer.Compare("3::", ""));
        }

        [Fact]
        public void Test2a() =>
            Assert.Equal(100, Comparer.Compare("3:E:E", "3:E:E"));

        [Fact]
        public void Test2b() =>
            Assert.Equal(0, Comparer.Compare("3:E:E", "3:ue:ue"));

        [Fact]
        public void Test2c() =>
            Assert.Equal(0, Comparer.Compare("3:ue:ue", "3:E:E"));

        [Fact]
        public void Test2d() =>
            Assert.Equal(0, Comparer.Compare("3:E:E", "3:ui:ui"));

        [Fact]
        public void Test2e() =>
            Assert.Equal(0, Comparer.Compare("3:E:E", "192:/rtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:/5+7eVvwUL7gKCpzNSbl"));

        [Fact]
        public void Test2f() =>
            Assert.Equal(99, Comparer.Compare(
                "192:sdrtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:I5+7eVvwUL7gKCpzNSbl",
                "192:/rtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:/5+7eVvwUL7gKCpzNSbl"));

        [Fact]
        public void Test2g() =>
            Assert.Equal(99, Comparer.Compare(
                "192:/rtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:/5+7eVvwUL7gKCpzNSbl",
                "192:sdrtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:I5+7eVvwUL7gKCpzNSbl"));

        [Fact]
        public void Test2h() =>
            Assert.Equal(100, Comparer.Compare(
                "192:/rtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:/5+7eVvwUL7gKCpzNSbl",
                "192:/rtCMPFGVk+1cvtuIyeEUGZnhbvC6VsyYT6k8ylgKCaLizoQSbl:/5+7eVvwUL7gKCpzNSbl"));
    }
}
