// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


//Console.Write("内容：");
//string text = Console.ReadLine() ?? "";

//var data = Encoding.UTF8.GetBytes(text);

//var data = File.ReadAllBytes(@"最终.png");

var data = Encoding.UTF8.GetBytes("这只是一段测试文本啦！啊哈哈哈！");

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
	outputData.AddRange(BitConverter.GetBytes((short)(data.Length % 7 + 0x3d00)));
	//output.Append(Encoding.Unicode.GetString(BitConverter.GetBytes((short)(rest + 0x4e00))));
	//output.Append(Encoding.Unicode.GetString(BitConverter.GetBytes((short)(data.Length % 7 + 0x3d00))));
}

string outputStr = Encoding.Unicode.GetString(outputData.ToArray());

var e = DateTime.Now;
//Console.WriteLine(outputStr);

using var file = File.Create(@"test.txt");
data = Encoding.Unicode.GetBytes(outputStr);
file.Write(data, 0, data.Length);
file.Flush();

Console.WriteLine((e - s).TotalMilliseconds);