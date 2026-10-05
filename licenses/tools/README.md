# 构建及 SDK 安装工具声明

这些资料用于标识开发工具，不表示工具本身或其声明中的所有组件都包含在 `InvoicePrinting.exe` 中。

- `dotnet-install/LICENSE.TXT` 保留本地 SDK 安装脚本的官方 MIT 许可文字。脚本头部声明 .NET Foundation 版权和 MIT 许可，脚本自身被 `.gitignore` 忽略。许可文件固定到其自身上游提交；这不代表已确认本地脚本的精确版本。完整中文参考译文见 `LICENSE.zh-CN.txt`。
- `illink-10.0.12/THIRD-PARTY-NOTICES.TXT` 和 `package.nuspec` 逐字节复制自已安装的 Microsoft.NET.ILLink.Tasks 10.0.12 包，未修改原文。该包通过 SPDX 表达式声明 MIT，未附单独许可文件；`LICENSE.TXT` 保留包中记录的仓库提交对应的上游 MIT 文字。中文译文为 `LICENSE.zh-CN.txt` 和 `THIRD-PARTY-NOTICES.zh-CN.txt`。ILLink 与 Core 运行时包中的第三方声明逐字节相同，因此共享相同完整译文；这不改变工具仅用于构建的范围。

中文译文为非官方参考译文，**正式许可条款以保留的上游原文为准**。译文不替代原文、不改变许可范围，也不构成重新授权。`package.nuspec` 是保留原样的机器元数据，包名、作者、描述及版本等字段不作改写。

`sources.json` 记录来源网址、便携的本地来源路径、范围和 SHA-256；中文译文另外登记，并通过 `sourcePath` 对应原文。若以后再分发这些工具，应独立核对其发行条款。
