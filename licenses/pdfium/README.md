# PDFium 与内嵌组件的许可资料

本目录保存项目所用原生 PDF 库的上游许可、版权声明和来源记录。项目自身的 Apache-2.0 许可见根目录 `LICENSE`；本目录的上游文件各自适用原有条款，不能统一视为本项目的 Apache-2.0 代码。

所有许可证及版权正文均另附 `.zh-CN` 中文参考译文，例如 `PDFium-LICENSE.zh-CN.txt`。正式条款以随附上游原文为准，原文保持不变。中文译文完整翻译相应许可条款，不替代必须保留的原始版权和许可文字。`reference/` 中含整份源程序的文件仅翻译完整版权／许可注释，程序语法、标识符和数据表保持原样；具体范围见 `reference/TRANSLATION-SCOPE.zh-CN.md`。必要的 FreeType 和 IJG 英文署名同时保留并说明中文含义。

## 已确认的二进制来源

使用的包为 `PdfiumViewer.Native.x86_64.v8-xfa` **2018.4.8.256**，包内路径为 `Build/x64/pdfium.dll`。该 DLL 长度为 **15,807,488 字节**，SHA-256 为：

```text
2a5657754571dcffe5490908f46d5898c34aae0aa926b2ff7cf7a248f980a1e6
```

它的 Git blob SHA-1 为 `15abb143ad3606dd406b697d1dce01763718ad8a`，与[作者归档的 2018-04-08 构建](https://github.com/pvginkel/PdfiumBuild/blob/master/Builds/2018-04-08/PdfiumViewer-x64-v8-xfa/pdfium.dll)一致。应用使用、嵌入并加载该原始 DLL，没有修改 PDFium 或其第三方库源码。此说明不代表原包作者的构建不存在 Chromium 或 PdfiumViewer 的补丁。

## 原文与证据范围

`sources.json` 逐文件记录上游 URL、保存文件的 SHA-256、组件、范围、源码修订状态、下载转换及补充说明。许可全文按原文保存；若许可位于源文件的开头，保存完整连续的原始注释块，并记录原始源文件 SHA-256。Gitiles 的 base64 传输只进行解码，不修改解码后的文件字节。`ATTRIBUTIONS.txt` 提供 FreeType 和 IJG 等署名；`FONT-CMAP-NOTICES.txt` 说明字体与 CMap 资料的来源和界限。

范围字段的含义：

- `identified-binary-component`：包、构建配置或 DLL 字符串能证明该组件存在，但所收集原文对应的精确源码修订仍未确认。
- `reference-coverage`：作为历史参考保存声明；不能据此认定该组件、文件或数据的具体版本进入了此 DLL。
- `provenance-reference`：用于解释原始构建及版本来源的历史配置和文档。
- `project-maintenance-metadata`：本项目编写的说明和署名，不是上游许可正文。

历史 PDFium 参考提交为 `6058efdbdc186e120e7e2121c290ac4d820ffbf8`，它靠近归档构建日期，**没有被确证为该 DLL 的源码提交**。该参考提交的 `DEPS` 提供本目录使用的 V8、FreeType、ICU、libjpeg-turbo 和 zlib 历史修订。构建脚本参考提交为 `eb44ddc9d8863c8e79c2920e300b25ff849407e3`。此脚本执行未锁定源码修订的 `gclient sync`，不能由包版本日期反推出精确源码。

主要原文文件包括 PDFium BSD、V8 BSD 与外部组件声明、FreeType FTL、ICU 完整声明、libpng、libjpeg-turbo 的 IJG/BSD/zlib 组合、OpenJPEG、AGG 2.3、Little CMS、LibTIFF、zlib 和 Big Integer Library。`reference/` 保留历史依赖元数据、FreeType 文件级例外及字体/CMap 原始声明。

FreeType 在此采用历史 PDFium 元数据所选的 **FTL** 路径。`reference/FreeType-LICENSE.TXT` 原文提及可选 GPL，不代表本项目选择 GPL；没有将 GPL 全文作为已确定的 DLL 许可列入。V8 的 Valgrind 声明也只作为相应头文件的参考资料，不能据此推定整个 Valgrind 工具进入 Windows DLL。

## 尚待确认的原始构建信息

本资料集不是完整、已验证的二进制 SBOM，也不宣称已经穷尽全部许可。以下项目仍需由原构建记录或具有确定源码修订的替代构建闭合：

- PDFium 精确源码提交、编译依赖集合及补丁清单。
- 各内嵌库的精确版本、源码修订，以及实际编入的文件级许可例外。
- 原生 Windows C/C++ 运行库等可能存在的额外分发条款。
- 内嵌字体程序和 CMap 数据的精确来源、版本及底层版权链。

此前尝试下载 FreeType 的 `src/base/fthash.h` 得到 HTTP 404；该历史树中的正确位置为 `include/freetype/internal/fthash.h`，另有 `src/base/fthash.c`。两份正确文件均已保存，错误路径和更正记录见 `sources.json`。当前列入的上游下载文件均已取得；仍未确认的构建来源不因取得许可原文而被视为解决。

发行此应用时应随包保留根目录声明和本目录资料。修改依赖版本或替换原生 DLL 时，需要重新核对原文和来源记录。
