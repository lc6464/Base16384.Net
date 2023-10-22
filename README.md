# Base16384.Net

参考：[fumiama/base16384](https://github.com/fumiama/base16384 "GitHub: fumiama/base16384")
实现：[lc6464/LC6464.Base16384](https://github.com/lc6464/LC6464.Base16384 "GitHub: lc6464/LC6464.Base16384")

此项目引用实现后的库，作为控制台应用程序供用户使用。

## 使用方法
### Command
```
Base16384.Net.exe <"e" | "d"> <source | "-"> [out | "-"]
```
- e: Encode，将指定 文件/目录下的文件编码至 Result 目录下。
- d: Decode，将 文件/目录下的文件解码至 Result 目录下。
- source: 源文件路径，可以是文件夹，也可以是文件。
- out: 输出文件路径，可以是文件夹，也可以是文件。
- "-" 代表 Standard IO。
#### 未指定输出路径或输入为文件而输出为文件夹时时，默认输出到该目录 源文件名.encoded/decoded,输入为文件夹但输出未指定文件夹时，输出到同级文件夹out