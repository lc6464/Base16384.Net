// See https://aka.ms/new-console-template for more information
using Base16384Coder;
using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");


Console.Write("内容：");
var text = Console.ReadLine() ?? "";

var data = Encoding.UTF8.GetBytes(text);

//var data = File.ReadAllBytes(@"最终.png");

//var data = Encoding.UTF8.GetBytes("这只是一段测试文本啦！啊哈哈哈！");

int offset = 0, rest = 0;
List<byte> outputData = new();

var s = DateTime.Now;

foreach (var one in data) {
	offset += 8;
	if (offset < 14) {
		rest += (one << (14 - offset)) & 0x3fff;
		continue;
	}
	offset -= 14;
	outputData.AddRange(BitConverter.GetBytes((short)(rest + (one >> offset) + 0x4e00)));
	//output.Append(Encoding.Unicode.GetString(BitConverter.GetBytes((short)(rest + (one >> offset) + 0x4e00))));
	rest = (one << (14 - offset)) & 0x3fff;
}

if (offset != 0) {
	outputData.AddRange(BitConverter.GetBytes((short)(rest + 0x4e00)));
	outputData.AddRange(BitConverter.GetBytes((short)((data.Length % 7) + 0x3d00)));
	//output.Append(Encoding.Unicode.GetString(BitConverter.GetBytes((short)(rest + 0x4e00))));
	//output.Append(Encoding.Unicode.GetString(BitConverter.GetBytes((short)(data.Length % 7 + 0x3d00))));
}

var outputStr = Encoding.Unicode.GetString(outputData.ToArray());

var e = DateTime.Now;
//Console.WriteLine(outputStr);

using var file = File.Create(@"test.txt");
data = Encoding.BigEndianUnicode.GetBytes(outputStr);
file.Write(data, 0, data.Length);
file.Flush();

Console.WriteLine((e - s).TotalMilliseconds);


s = DateTime.Now;

unsafe {
	var outPtr = (byte*)Marshal.AllocHGlobal(8192 * 1024 / 7 * 7);
	var dataPtr = (byte*)Marshal.AllocHGlobal(data.Length);
	Marshal.Copy(data, 0, (IntPtr)dataPtr, data.Length);
	Base16384.Base16384_encode(dataPtr, data.Length, outPtr, 8192 * 1024 / 7 * 7);
	var outData = new byte[8192 * 1024 / 7 * 7];
	Marshal.Copy((IntPtr)outPtr, outData, 0, 8192 * 1024 / 7 * 7);
	e = DateTime.Now;
    using var file2 = File.Create(@"test1.txt");
	file2.Write(outData, 0, 8192 * 1024 / 7 * 7);
	file2.Flush();
	Console.WriteLine((e - s).TotalMilliseconds);
}