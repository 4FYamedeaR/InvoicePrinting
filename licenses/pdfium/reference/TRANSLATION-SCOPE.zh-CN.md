# 中文译文范围

本目录中的 `.zh-CN` 文件是中文参考译文，正式条款以相应上游原文为准。原文单独保留，未将原始版权文字替换为译文。

完整的独立许可证、版权声明和无代码说明文件均翻译自然语言全文，包括 FreeType 的许可选择及 BDF／PCF 驱动声明。原始参考元数据另附完整中文译文；其中的路径、URL、哈希、版本、补丁名称、函数名、术语名称等标识信息保留。

包含整份源程序或构建文件的资料，仅翻译完整的版权及许可注释，程序语法、标识符、数据表、构建目标列表、算法注释和非许可 API 使用文档不作逐行翻译：

- `FreeType-include-freetype-internal-fthash.h.txt`、`FreeType-src-base-fthash.c.txt`：完整翻译开头版权、许可及其来源说明；保留代码。
- `FreeType-src-gzip-zlib.h.txt`：完整翻译开头版权及 zlib 许可注释；保留后续代码和 API 文档。
- `FreeType-src-psnames-pstables.h.txt`：完整翻译开头版权、FTL 选择声明及自动生成说明；保留字形名、数据表和代码。
- `PDFium-third_party-BUILD.gn.txt`、`PDFium-build_overrides.gni.txt`：完整翻译开头版权及许可说明；保留 GN 代码。
- `PDFium-DEPS.txt`、`PdfiumBuild-args.gn.txt`、`PdfiumBuild-Env.cs.txt`：属于原始依赖配置或构建程序，没有需要单独翻译的开头版权／许可正文；保留代码及历史配置，通过 `sources.json` 的中文说明交代用途和限制。
- 根 PDFium 资料目录的 `libjpeg-turbo-SIMD-LICENSE.txt`：完整翻译开头版权和许可注释；后面的历史 NASM 配置片段保持于原文。

`font-cmap/` 中每份版权头都有对应中文参考译文。Adobe PostScript 资源标记、资源名和版本号作为原始标识保留，完整版权、许可条件和免责条款已翻译。

所有翻译范围和原文对应关系均登记在 `../sources.json`，译文条目含 `sourcePath`。这些翻译不改变资料中已有的历史参考范围，也不将尚未确认的源码修订变为已确定来源。
