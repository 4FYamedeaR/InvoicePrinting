# 许可信息

> 非官方完整中文参考译文。对应原文为同目录 `license-information.md`。正式许可以保留的上游原文及其引用的许可证为准；本译文不改变条款。下文为原文全文的翻译。

.NET 项目使用来自多个来源的源代码和二进制文件，这些来源可能对您使用 .NET 有重要影响。

本文件仅用于提供信息，本身不是许可证。

## 源代码

.NET 源代码使用 MIT 许可证。

[每个仓库](https://github.com/dotnet/core/blob/44927bc821d37f596e317c4316335632bd38c79b/Documentation/core-repos.md)都包含：

- 许可证，例如 [dotnet/runtime LICENSE.TXT](https://github.com/dotnet/runtime/blob/main/LICENSE.TXT)。
- 第三方声明文件，例如 [dotnet/runtime THIRD-PARTY-NOTICES.TXT](https://github.com/dotnet/runtime/blob/main/THIRD-PARTY-NOTICES.TXT)。

更多信息：

- [项目版权指引](https://github.com/dotnet/runtime/blob/main/docs/project/copyright.md)。

## 产品分发

产品分发使用以下许可证：

- Linux 和 macOS 上：[MIT 许可证](https://github.com/dotnet/core/blob/main/LICENSE.TXT)。
- Windows 上：[.NET 库许可证](https://dotnet.microsoft.com/dotnet_library_license.htm)。

产品分发包括[可下载资源](https://dotnet.microsoft.com/download/dotnet)和[运行时包](https://www.nuget.org/packages/Microsoft.NETCore.App.Runtime.win-x64/)。

更多信息：

- [Windows 许可信息](https://github.com/dotnet/core/blob/main/license-information-windows.md)。
- [.NET 资源许可模型](https://github.com/dotnet/runtime/blob/main/docs/project/licensing-assets.md)。

## 包分发

库包使用 MIT 许可证，例如 [System.Text.Json](https://www.nuget.org/packages/System.Text.Json)。

## 再分发

由 .NET SDK 编译器（C#、F#、VB）生成的二进制文件可以再分发，不附加其他限制。限制仅来自生成该二进制所使用的编译器输入内容的许可证。

应用程序受上文“产品分发”和“包分发”所述相同条款约束。

.NET 运行时的部分内容会嵌入应用程序，包括[特定平台的可执行宿主](https://learn.microsoft.com/dotnet/core/deploying/deploy-with-cli#framework-dependent-executable)，以及[包含运行时的部署](https://learn.microsoft.com/dotnet/core/deploying/deploy-with-cli#self-contained-deployment)。
