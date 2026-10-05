# 许可证资料

根目录的 [LICENSE.zh-CN](../LICENSE.zh-CN) 是项目原始代码和文档所用 Apache 许可证 2.0 版的完整中文参考译文，[LICENSE](../LICENSE) 保存英文正式条款。这里保存第三方组件的中文参考译文、上游原文许可、版权声明、包元数据和来源记录；第三方组件不因项目采用 Apache-2.0 而更改许可。

名称含 `.zh-CN` 的文件为对应原文的中文参考译文。上游原文单独保留，正式条款以原文为准。包名、文件路径、网址、SPDX 许可标识、版本、哈希值及必要署名保留原样，便于核对来源。

| 目录 | 内容 |
| --- | --- |
| [pdfiumviewer/](pdfiumviewer/) | 两个直接 NuGet 包的元数据和 Apache-2.0 正文 |
| [pdfium/](pdfium/) | 原生 PDFium 及相关组件的原文许可、署名和历史来源记录 |
| [dotnet/](dotnet/) | .NET/WPF 10.0.12 包内声明、Windows 二进制许可及上游说明 |
| [tools/](tools/) | ILLink 和 SDK 安装脚本等开发工具的许可资料 |

组件概览见 [THIRD-PARTY-NOTICES.txt](../THIRD-PARTY-NOTICES.txt)，二进制发行的许可范围见 [DISTRIBUTION-TERMS.txt](../DISTRIBUTION-TERMS.txt)。

## 资料维护

1. 按实际引用或发布的版本收集上游许可，不使用新版本文件替代旧版本资料。
2. 保留上游原文；翻译、提取或补充说明应与原文分别保存并标明来源。
3. 在各目录的 `sources.json` 中记录文件路径、来源 URL、适用组件及 SHA-256。本地路径仅用于说明采集来源，不是用户电脑需要具备的路径。
4. 区分确定的二进制来源、识别出的组件和同期源码参考。历史参考提交不等于二进制的确定构建提交。
5. 添加或升级 NuGet 包、PDFium DLL 或自包含运行时时，同步刷新许可正文、来源记录及根目录第三方声明。
6. 执行发布命令后，检查声明文件和整个 `licenses/` 目录随 EXE 一起输出。打包 ZIP、安装包或 GitHub Release 时，也应保留这些文件。

各目录 `sources.json` 的校验值针对记录中的原文、中文译文或项目说明文件，用于核对文件完整性；它们不等同于完整构建生成的软件物料清单（SBOM）。
