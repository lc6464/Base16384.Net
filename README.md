# Base16384.Net

参考：[fumiama/base16384](https://github.com/fumiama/base16384 "GitHub: fumiama/base16384")
实现：[lc6464/LC6464.Base16384](https://github.com/lc6464/LC6464.Base16384 "GitHub: lc6464/LC6464.Base16384")

此项目引用实现后的库，作为控制台应用程序供用户使用。

## 使用方法
### Tree
```
|-- Base16384.Net.exe
|-- Base16384.Net.dll
|-- Source
	|-- xxx.jpg
	|-- xxx.png
|-- Encoded
	|-- xxx.jpg.Encoded
	|-- xxx.png.Encoded
```
### Command
```
Base16384.Net.exe <e|d>
```
- e: Encode，将 Source 目录下的文件编码至 Result 目录下
- d: Decode，将 Encoded 目录下的文件解码至 Result 目录下

## 另一种使用方法

成熟的程序员应该自己看代码来发现使用方法。