# Base16384.Net

参考：[fumiama/base16384](https://github.com/fumiama/base16384 "GitHub: fumiama/base16384")
实现：[lc6464/LC6464.Base16384](https://github.com/lc6464/LC6464.Base16384 "GitHub: lc6464/LC6464.Base16384")

此项目引用实现后的库，作为控制台应用程序供用户使用。

## 使用方法
```
Base16384.Net <"e" | "d"> <source | "-"> [out | "-"]
```
- 参数一：e 为编码；d 为解码
- 参数二：源文件(夹)路径，输入 `-` 则表示从标准输入读取数据。
- 参数三：输出文件(夹)路径，可选，其中输出到标准输出时无信息提示
	- 缺省
		- 输入为标准输入时，结果输出到标准输出
		- 输入为文件时，结果写入 `源文件名.encoded` 或 `源文件名.decoded`
		- 输入为文件夹时，结果写入到 `encoded` 或 `decoded` 文件夹内相应结构的文件内
	- `-` 表示输出到标准输出，输入为文件夹时无效
	- 其他
		- 输入为文件时，输出到指定文件
		- 输入为文件夹时，输出到指定文件夹
- 示例
	- 编码
		- `Base16384.Net.exe e C:\Users\lc6464\Desktop\test.mp4 C:\Users\lc6464\Desktop\test.mp4.txt`
		- `$PSVersionTable.OS | Base16384.Net e - /root/os.txt`
		- `Base16384.Net e "/home/lc6464/hello world.jpg" - > /dev/null`
	- 解码
		- `Base16384.Net.exe d C:\Users\lc6464\Desktop\test.encoded C:\Users\lc6464\Desktop\test.py`
		- `Base16384.Net d /root/os.txt -`
		- `Base16384.Net d /root/filesEncoded /home/lc6464/rawFiles`
	- 测试
		- `Base16384.Net.exe e C:\Users\lc6464\Desktop\test.mp4 - | Base16384.Net.exe d - C:\Users\lc6464\Desktop\test.mp4`
		- `$PSVersionTable.OS | Base16384.Net e - | Base16384.Net d -`

## 出错时调试
将环境变量 `Base16384_Net_Debug` 设为 `true` 或 `1` 等值，可输出调试信息。<br/>
特别注意，即使编解码到标准输出，也会输出异常信息！<br/>
只要环境变量 `Base16384_Net_Debug` 存在，且转小写后不为 `false` 或 `0`，就会进入调试模式。

## 注意事项
UTF16BE 编码的文件需要 BOM，使用输出到文件时，会自动添加 BOM，<br/>
但输出到标准输出时，**不会**添加 BOM，因此强烈不建议在终端通过*标准输出*输出后直接*重定向*到文件！<br/>
如果需要输出到标准输出时通过其他软件或表达式处理后输出到文件，务必手动添加 BOM，否则可能导致兼容性问题！

另外，通过*标准输出*输出的结果是 UTF16BE 编码后的二进制数据，终端直接显示可能会乱码。

## 下载软件
- 可直接通过 [GitHub Release](https://github.com/lc6464/Base16384.Net/releases/latest) 下载。
- 克隆项目通过 Visual Studio 或 dotnet cli 自行编译。