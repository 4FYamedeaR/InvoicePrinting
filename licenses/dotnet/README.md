# .NET 与 WPF 许可资料

当前应用使用 **10.0.12** 版 .NET 运行时包发布包含运行时的 Windows x64 可执行文件。本目录保留对应构建的许可和声明；这些资料不为 InvoicePrinting 自身授予许可，也不替代微软的条款。

本项目的说明和参考译文提供中文阅读入口。**正式许可条款以完整保留的上游原文为准**；中文参考译文为非官方译文，不改变许可范围，也不构成重新授权。

- `runtime-10.0.12/LICENSE.TXT`、`runtime-10.0.12/THIRD-PARTY-NOTICES.TXT` 以及各 `package.nuspec` 均逐字节复制自已安装的 NuGet 包，未修改原文。对应中文译文为同目录 `LICENSE.zh-CN.txt` 和 `THIRD-PARTY-NOTICES.zh-CN.txt`；后者按原文顺序完整翻译全部第三方声明和内嵌许可。
- `wpf-runtime-10.0.12/LICENSE` 逐字节复制自 WPF 运行时包。该包没有单独的第三方声明文件，因此另存明确标识为上游源码补充资料的 `UPSTREAM-THIRD-PARTY-NOTICES.TXT`。它来自包中记录的仓库提交，并与 WPF 标签 `v10.0.12` 的对应文件核对一致，包含 Zlib 和 Json.NET 声明；保留该文件不代表其中列举的每个组件都进入本应用。中文译文为 `LICENSE.zh-CN.txt` 和 `UPSTREAM-THIRD-PARTY-NOTICES.zh-CN.txt`。
- `windows/NET-Library-License.html` 和 `windows/Windows-SDK-License.html` 保留官方法律页面的完整 HTML 响应及原始编码声明，没有将 HTML 冒充纯文本或改写条款。完整法律正文的中文参考译文分别见 `NET-Library-License.zh-CN.txt` 和 `Windows-SDK-License.zh-CN.txt`。
- `windows/license-information-windows.md` 是微软关于许可适用范围的说明，指出 Windows 运行时和 WPF 二进制文件存在 MIT 以外的例外。运行时包的 MIT 元数据必须结合这些例外阅读。`license-information.md` 和 `licensing-assets.md` 提供其他官方背景；这些说明文档本身不是许可证。完整中文译文见各文件对应的 `.zh-CN.md`。

已保留的范围说明将 .NET 库许可证适用于 `coreclr.dll`、单文件二进制中包含的 .NET 运行时、`Microsoft.DiaSymReader.Native.*.dll`、`PresentationNative_cor3.dll`、`vcruntime140_cor3.dll` 和 `wpfgfx_cor3.dll`；将 Windows SDK 许可证适用于 `D3DCompiler_47_cor3.dll`，并将其他 .NET 文件说明为 MIT。

`sources.json` 记录各文件的来源、版本、范围和 SHA-256。`${NuGetPackageRoot}` 是 NuGet 包缓存根目录的便携占位符；原文及译文分别登记，译文通过 `sourcePath` 对应原文。仅保存许可文字不代表分销商、最终用户协议要求或其他发行条件已经落实。发行前应核对相应条款；更改运行时版本或部署方式时应同步更新资料。
