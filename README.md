SsdeepNET
=========
A C# version of the ssdeep algorithm, based on the fuzzy.c source code, taken from version 2.12 of the ssdeep package (http://ssdeep.sourceforge.net/).

Example
------

```cs
var bytesFoo = File.ReadAllBytes("foo.txt");
var bytesBar = File.ReadAllBytes("bar.txt");

var hashFoo = Hasher.HashBuffer(bytesFoo, bytesFoo.Length);
var hashBar = Hasher.HashBuffer(bytesBar, bytesBar.Length);

var comparisionResult = Comparer.Compare(hashFoo, hashBar);

Console.WriteLine(comparisionResult);
```
