# .NET 资源许可模型

> 非官方完整中文参考译文。对应原文为同目录 `licensing-assets.md`。正式许可以保留的上游原文及其引用的许可证为准；本译文不改变条款。下文为原文全文的翻译。

> 上述版权声明和本许可声明必须包含在软件的所有副本或实质性部分中。

[MIT](https://github.com/dotnet/core/blob/main/LICENSE.TXT) 等许可证要求软件分发时附带许可证。我们应遵循这一模型。

每次 .NET 二进制分发都必须附带：

- 其许可证。
- 第三方声明。

二进制分发包括压缩归档、运行时包、安装程序、容器镜像、包，以及我们交付“软件实质性部分”的其他任何形式。

注意：分发必须包含并展示正确的许可证。例如 [Microsoft.NETCore.App.Runtime.win-x64](https://www.nuget.org/packages/Microsoft.NETCore.App.Runtime.win-x64/) 运行时包，必须包含正确的许可证和正确的许可元数据，并在 NuGet 库中展示。安装程序也采用类似模型，通过界面展示许可。

## 产品分发

产品分发应使用以下许可证：

- Linux 和 macOS 上：[.NET MIT 许可证](https://github.com/dotnet/core/blob/main/LICENSE.TXT)。
- Windows 上：依照 [Windows 许可信息](https://github.com/dotnet/core/blob/main/license-information-windows.md)，使用 [.NET 库许可证](https://dotnet.microsoft.com/dotnet_library_license.htm)。

产品分发包括[可下载资源](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)和 NuGet 运行时包。

## 包分发

库包，例如 [System.Text.Json](https://www.nuget.org/packages/System.Text.Json)，应使用 .NET MIT 许可证。

这里的“包”不包括运行时包。
