using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SsdeepNET
{
    class Program
    {
        static void Main(string[] args)
        {
            var content1 = File.ReadAllText(@"C:\TEMP\ssdeep\foo.txt");
            var content2 = File.ReadAllText(@"C:\TEMP\ssdeep\bar.txt");
            var arr1 = Encoding.UTF8.GetBytes(content1);
            var arr2 = Encoding.UTF8.GetBytes(content2);

            var hash1 = Hasher.HashBuffer(arr1, arr1.Length);
            var hash2 = Hasher.HashBuffer(arr2, arr2.Length);
            Console.WriteLine(Comparer.Compare(hash1, hash2));
        }
    }
}
