using System.Runtime.InteropServices;

Console.WriteLine("Hello, World!");


Console.Write("内容：");

var text = Console.ReadLine() ?? "";

var data = Encoding.UTF8.GetBytes(text);

var u16bes = new byte[] { 0xfe, 0xff };

unsafe {
	var s = DateTime.Now;

	var dataPtr = (byte*)Marshal.AllocHGlobal(data.Length);
	Marshal.Copy(data, 0, (IntPtr)dataPtr, data.Length);
	var outPtr = (byte*)Marshal.AllocHGlobal(8192 * 1024 / 7 * 7);
	var outData = new byte[Base16384.Base16384.Encode(dataPtr, data.Length, outPtr, 8192 * 1024 / 7 * 7)];
	Marshal.Copy((IntPtr)outPtr, outData, 0, outData.Length);
	Marshal.FreeHGlobal((IntPtr)outPtr);
	Marshal.FreeHGlobal((IntPtr)dataPtr);

	var e = DateTime.Now;
	Console.WriteLine((e - s).TotalMilliseconds);

	using var file2 = File.Create(@"D:\LC\Desktop\testCPtr.txt");
	file2.Write(u16bes, 0, 2);
	file2.Write(outData, 0, outData.Length);
	file2.Flush();


	s = DateTime.Now;

	var decodingPtr = (byte*)Marshal.AllocHGlobal(outData.Length);
	Marshal.Copy(outData, 0, (IntPtr)decodingPtr, outData.Length);
	var decodedPtr = (byte*)Marshal.AllocHGlobal(8192 * 1024 / 7 * 7);
	var decodedData = new byte[Base16384.Base16384.Decode(decodingPtr, outData.Length, decodedPtr, 8192 * 1024 / 7 * 7)];
	Marshal.Copy((IntPtr)decodedPtr, decodedData, 0, decodedData.Length);
	Marshal.FreeHGlobal((IntPtr)decodingPtr);
	Marshal.FreeHGlobal((IntPtr)decodedPtr);

	e = DateTime.Now;
	Console.WriteLine((e - s).TotalMilliseconds);

	File.WriteAllBytes(@"D:\LC\Desktop\testDecode.txt", decodedData);
}