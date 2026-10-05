中文参考译文
对应原文：libjpeg-turbo-README.ijg.txt
正式条款以原文为准；本译文不替代随附原文。
以下日期、价格、网站和版本描述均忠实保留历史原文，不表示当前状态。

libjpeg-turbo 说明：libjpeg-turbo 项目已修改本文件，使其仅包含与 libjpeg-turbo 有关的信息，润色部分章节，并删除 libjpeg v8 README 中曾存在的不恰当措辞。本文件仅供参考。有关 libjpeg-turbo 的具体信息，请参阅 README.md。

Independent JPEG Group 的 JPEG 软件

本发行包包含 Independent JPEG Group 免费 JPEG 软件的一个发布版本。在遵守下方“法律事项”条件的前提下，欢迎重新分发本软件并将其用于任何目的。

本软件由 Tom Lane、Guido Vollbeding、Philip Gladstone、Bill Allombert、Jim Boucher、Lee Crocker、Bob Friesenhahn、Ben Jackson、Julian Minguillon、Luis Ortiz、George Phillips、Davide Rossi、Ge' Weijers 及 Independent JPEG Group 的其他成员共同完成。

IJG 不隶属于 ISO/IEC JTC1/SC29/WG1 标准委员会；该委员会与 ITU-T SG16 一起也被称为 JPEG。

文档阅读路线

本文件包含以下章节：

概述（OVERVIEW）：JPEG 和 IJG 软件的一般说明。
法律事项（LEGAL ISSUES）：版权、无担保及分发条款。
参考资料（REFERENCES）：进一步了解 JPEG 的资料。
归档地址（ARCHIVE LOCATIONS）：寻找本软件较新版本的位置。
文件格式之争（FILE FORMAT WARS）：不应采用的软件。
待办事项（TO DO）：未来 IJG 发布版本的计划。

发行包中的其他文档文件包括：

用户文档：
usage.txt：cjpeg、djpeg、jpegtran、rdjpgcom 和 wrjpgcom 的使用说明。
*.1：程序的 Unix 风格手册页，内容与 usage.txt 相同。
wizard.txt：仅面向 JPEG 高级用户的进阶使用说明。
change.log：版本之间的重要变化。

程序员和内部文档：
libjpeg.txt：如何在自己的程序中使用 JPEG 库。
example.c：调用 JPEG 库的示例代码。
structure.txt：JPEG 库内部结构概述。
coderules.txt：代码风格规则；如果贡献代码，请阅读。

至少应阅读 usage.txt。JPEG 常见问题解答（FAQ）文章也包含一些信息。有关获取该文章的方法，见下面的“归档地址”。

如果想了解 JPEG 代码的工作方式，建议先阅读一项或多项“参考资料”，再大致按上面列出的顺序阅读文档文件，最后深入源代码。

概述

本软件包包含实现 JPEG 图像编码、解码和转码的 C 软件。JPEG（读作“jay-peg”）是彩色和灰度图像的标准化压缩方法。JPEG 擅长压缩照片或其他相邻像素间色彩、亮度平滑变化的图像。具有锐利线条或其他突变特征的图像，JPEG 压缩效果可能不佳；为避免可见压缩伪影，可能需要使用较高的 JPEG 质量设置。

JPEG 是有损压缩，这意味着输出像素不一定与输入像素完全相同。不过，对于照片和其他“平滑”图像，可在没有可见压缩伪影的情况下取得很好的压缩比；若愿意降低图像质量，即降低压缩器的“质量”设置，还可取得极高的压缩比。

本软件实现 JPEG 基线、扩展顺序和渐进压缩过程。设计已考虑支持这些过程的所有变体，不过某些少见参数设置尚未实现。我们没有计划支持标准定义的层次式或无损压缩过程。

我们提供读写 JPEG 图像文件的一组库函数，以及使用该库在 JPEG 与其他常见图像文件格式之间转换的示例程序 cjpeg 和 djpeg。本库旨在供其他应用重用。

为支持文件转换及图像查看软件，我们纳入了大量超出基本 JPEG 编码／解码能力的功能。例如，色彩量化模块并不严格属于 JPEG 解码，但向具有色彩映射表的文件格式或显示设备输出时必不可少。若特定应用不需要，可在编译时从库中移除这些附加功能。

我们还提供 jpegtran，用于在不同 JPEG 压缩过程之间进行无损转码；以及 rdjpgcom 和 wrjpgcom，用于在 JFIF 文件中提取和插入文本注释。

本软件的设计重点是可移植性和灵活性，同时保证速度足以实用。特别是，本软件不是作为 JPEG 教程编写的；入门资料见“参考资料”。它旨在成为可靠、可移植、适合工业使用的代码。我们不声称软件的每个部分都已达到这一目标，但一直为此努力。

欢迎将本软件作为商业产品的组件。无须支付许可费，但必须按“法律事项”的说明在产品文档中予以致谢。

法律事项

用普通语言说明：

1. 我们不承诺本软件能够正常工作，但如果发现错误，请告诉我们。
2. 您可为任何目的使用本软件，无须向我们付费。
3. 不得声称本软件由您编写。如果在程序中使用本软件，必须在文档某处确认您使用了 IJG 代码。

法律表述：

作者不就本软件及其质量、准确性、适销性或特定用途适用性提供任何明示或默示担保或陈述。本软件按“现状”提供；作为用户，您承担有关本软件质量和准确性的全部风险。

本软件版权归 © 1991-2016 Thomas G. Lane、Guido Vollbeding 所有；除下文明确规定外，保留所有权利。

现允许为任何目的免费使用、复制、修改和分发本软件或其部分内容，但须满足以下条件：

(1) 若分发本软件的任何部分源代码，必须包含本 README 文件，并原样保留本版权和无担保声明；对原始文件的任何增加、删除或修改必须在随附文档中明确说明。
(2) 若只分发可执行代码，随附文档必须说明以下内容，必要英文原句予以保留：
“this software is based in part on the work of the Independent JPEG Group”。
中文含义：“本软件的部分内容基于 Independent JPEG Group 的工作。”
(3) 只有在用户接受对任何不良后果承担全部责任的情况下，才授予使用本软件的许可；作者对任何形式的损害均不承担责任。

这些条件适用于从 IJG 代码衍生或基于其构建的任何软件，不仅限于未修改的库。如果使用我们的工作，应予以署名致谢。

不得在与本软件或其衍生产品有关的广告或宣传中使用任何 IJG 作者姓名或公司名称。仅可用“the Independent JPEG Group's software”（Independent JPEG Group 的软件）指称本软件。

我们明确允许并鼓励将本软件作为商业产品的基础，条件是所有担保及责任主张均由产品供应商承担。

Unix 配置脚本 configure 由 GNU Autoconf 生成，版权归 Free Software Foundation 所有，但可自由分发。其支持脚本 config.guess、config.sub 和 ltmain.sh 也是如此。另一支持脚本 install-sh 的版权归 X Consortium 所有，但同样可自由分发。

IJG 发行包曾包含读取和写入 GIF 文件的代码。为避免涉及 Unisys 的 LZW 专利（现已到期），GIF 读取支持已被完全移除，GIF 写入器也已简化为生成“未压缩的 GIF”。这种方法不使用 LZW 算法；生成的 GIF 文件比通常的大，但所有标准 GIF 解码器均可读取。

我们必须声明以下内容，原句予以保留：
“The Graphics Interchange Format(c) is the Copyright property of CompuServe Incorporated. GIF(sm) is a Service Mark property of CompuServe Incorporated.”
中文含义：“Graphics Interchange Format(c) 的版权归 CompuServe Incorporated 所有。GIF(sm) 是 CompuServe Incorporated 的服务标志。”

参考资料

尝试了解 JPEG 软件的内部机制之前，建议阅读以下一项或多项资料。

JPEG 压缩算法的最佳简短技术入门资料是：
Wallace, Gregory K.，“The JPEG Still Picture Compression Standard”，Communications of the ACM，1991 年 4 月，第 34 卷第 4 期，第 30-44 页。
同一期的相邻文章讨论 MPEG 动态图像压缩、JPEG 应用及相关主题。若无法取得该期 CACM，Wallace 文章修订版的 PDF 可在 http://www.ijg.org/files/Wallace.JPEG.pdf 获取。该文件实际上是后来发表于 IEEE Trans. Consumer Electronics 的文章预印本，省略了 CACM 中的示例图像，但包含修正和一些新增资料。注意：Wallace 文章的版权归 ACM 和 IEEE 所有，不得用于商业目的。

较少技术细节、节奏较舒缓的 JPEG 入门可见 Mark Nelson 和 Jean-loup Gailly 所著“The Data Compression Book”（数据压缩之书），M&T Books（New York）出版，第 2 版，1996 年，ISBN 1-55851-434-1。本书对包括 JPEG 在内的多种压缩方法提供良好解释及 C 示例代码。若能阅读 C 代码但对一般数据压缩不甚了解，这是很好的资料。本书的 JPEG 示例代码离工业级实现还很远；不过，当您准备阅读完整实现时，这里就有一份。

当时可获得的 JPEG 最佳描述是 William B. Pennebaker 和 Joan L. Mitchell 所著教材“JPEG Still Image Data Compression Standard”，Van Nostrand Reinhold 出版，1993 年，ISBN 0-442-01272-1。价格为 US$59.95，共 638 页。本书包含 ISO JPEG 标准的完整文字（DIS 10918-1 和 DIS 10918-2 草案）。

原始 JPEG 标准分为两部分，第 1 部分是实际规范，第 2 部分涵盖符合性测试方法。第 1 部分标题为“Digital Compression and Coding of Continuous-tone Still Images, Part 1: Requirements and guidelines”（连续色调静止图像的数字压缩和编码，第 1 部分：要求与指南），文档编号为 ISO/IEC IS 10918-1、ITU-T T.81。第 2 部分标题为“Digital Compression and Coding of Continuous-tone Still Images, Part 2: Compliance testing”（连续色调静止图像的数字压缩和编码，第 2 部分：符合性测试），文档编号为 ISO/IEC IS 10918-2、ITU-T T.83。

JPEG 标准没有规定可交换文件格式的全部细节。对于省略的细节，我们遵循 JFIF 1.02 版约定。JFIF 1.02 已被采纳为 Ecma International 技术报告，因此获得正式出版地位。可在 http://www.ecma-international.org/publications/techreports/E-TR-098.htm 免费下载 PDF。JFIF 文档的 PostScript 版本位于 http://www.ijg.org/files/jfif.ps.gz；纯文本版本位于 http://www.ijg.org/files/jfif.txt.gz，但缺少图示。

TIFF 6.0 文件格式规范可通过 FTP 从 ftp://ftp.sgi.com/graphics/tiff/TIFF6.ps.gz 获取。1992 年 6 月 3 日的 TIFF 6.0 规范中所采用的 JPEG 集成方案存在若干严重问题。IJG 不建议使用 TIFF 6.0 方案（TIFF Compression tag 6）；而建议采用 TIFF Technical Note #2 提出的 JPEG 方案（Compression tag 7）。该说明的副本可从 http://www.ijg.org/files/ 获取。预计下一版 TIFF 规范将以该说明中的方案替换 6.0 JPEG 方案。尽管 IJG 自己的代码不支持 TIFF/JPEG，免费的 libtiff 库使用我们的库，按该说明实现了 TIFF/JPEG。

归档地址

本软件的“官方”归档网站为 www.ijg.org，最新发布版本总可在该站 files 目录中找到。

JPEG FAQ（常见问题解答）文章提供一些有关 JPEG 的一般信息。它位于 http://www.faqs.org/faqs/jpeg-faq/ 及其他 news.answers 归档网站，包括 rtfm.mit.edu 的官方 news.answers 归档：ftp://rtfm.mit.edu/pub/usenet/news.answers/jpeg-faq/。
若无法访问 Web 或 FTP，可向 mail-server@rtfm.mit.edu 发送邮件，邮件正文为：

    send usenet/news.answers/jpeg-faq/part1
    send usenet/news.answers/jpeg-faq/part2

文件格式之争

ISO/IEC JTC1/SC29/WG1 标准委员会（它与 ITU-T SG16 一起也被称为 JPEG）当时正在推广名称中包含 JPEG 但与原始基于 DCT 的 JPEG 不兼容的其他格式。因此 IJG 不支持这些格式，详见“参考资料”。开发本自由软件最初的原因之一，就是推动 JPEG 文件向共同且可互操作的格式标准趋同。请勿使用不兼容的文件格式。无论如何，我们的解码器将始终能够读取现有 JPEG 图像文件。

待办事项

错误报告、协助意向等请发送至 jpeg-info@jpegclub.org。
