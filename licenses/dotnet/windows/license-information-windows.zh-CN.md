# Windows 上 .NET 的许可信息

> 非官方完整中文参考译文。对应原文为同目录 `license-information-windows.md`。正式许可及适用范围以保留的上游原文和其引用的许可证为准；本译文不改变条款。下文为原文全文的翻译。

Windows 版 .NET 包含依照多种许可证提供的文件。这些信息旨在帮助您理解适用于您使用行为的许可条款。使用 Windows 上的 .NET，即表示您同意以下许可条款。

本文件仅用于提供信息，本身不是许可证。

以下二进制文件依据 [.NET 库许可证](https://dotnet.microsoft.com/dotnet_library_license.htm) 许可：

- `coreclr.dll`，以及以单文件方式发布的二进制中包含的 .NET 运行时（原因是 .NET 运行时在 Windows 错误报告崩溃报告中包含[额外遥测](https://github.com/dotnet/runtime/blob/main/src/coreclr/vm/dwreport.cpp)）。
- `Microsoft.DiaSymReader.Native.{x86|amd64|arm|arm64}.dll`（由 .NET 运行时和 SDK 使用）。
- `PresentationNative_cor3.dll`（由 WPF 使用）。
- `vcruntime140_cor3.dll`（由 WPF 使用）。
- `wpfgfx_cor3.dll`（由 WPF 使用）。

注意：`vcruntime140_cor3.dll` 与 Visual Studio 中包含的 `vcruntime140.dll` 是相同的二进制文件，由微软依据 .NET 库许可证重新许可。

以下二进制文件依据 [Windows SDK 许可证](https://learn.microsoft.com/legal/windows-sdk/license) 许可：

- `D3DCompiler_47_cor3.dll`（由 WPF 使用）。

所有其他二进制文件和文件依据 [MIT 许可证](https://github.com/dotnet/core/blob/main/LICENSE.TXT) 许可。

其他操作系统的信息见[许可信息](./license-information.zh-CN.md)。
