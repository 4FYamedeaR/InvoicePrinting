中文参考译文
对应原文：FreeType-src-bdf-README.txt
正式条款以原文为准；本译文不替代随附原文。

用于 BDF 字体的 FreeType 字体驱动
Francesco Zappa Nardelli
<francesco.zappa.nardelli@ens.fr>

引言

BDF（Bitmap Distribution Format，位图分发格式）是 Adobe 定义的位图字体格式，旨在方便人和计算机理解。本代码按照 Adobe 2.2 版规范，实现 FreeType 库的 BDF 驱动。BDF 字体格式规范可从 Adobe 网站获取：
https://www.adobe.com/content/dam/acom/en/devnet/font/pdfs/5005.BDF_Spec.pdf

XFree86 (www.XFree86.org) 提供很多优质 BDF 位图字体。它们不定义垂直度量，因为 X Consortium 的 BDF 规范已将其移除。

编码

BDF 字体所附编码的种类，似乎超出了 freetype.h 中定义的少量编码。另一方面，BDF 字体通常定义两个用于指定编码和注册表的属性。

我决定让客户端应用能够直接访问这两个属性，并自行解释其含义。例如：

    #include FT_INTERNAL_BDF_TYPES_H
    FT_Face          face;
    BDF_Public_Face  bdfface;
    FT_New_Face( library, ..., &face );
    bdfface = (BDF_Public_Face)face;
    if ( ( bdfface->charset_registry == "ISO10646" ) &&
         ( bdfface->charset_encoding == "1" )        )
      [..]

因此，驱动始终将 ft_encoding_none 作为 face->charmap.encoding 导出。FT_Get_Char_Index 的行为没有变化，仍将给定的 ULong 值转换为相应字形编号。

如果这两个属性不存在，应假定采用 Adobe Standard Encoding。

抗锯齿位图

驱动支持 Mark Leisher 的 xmbdfed 位图字体编辑器所使用的 BDF 扩展。Microsoft 的 SBIT 工具在向 TrueType 字体添加抗锯齿字体时，需要此格式的位图字体。该扩展在 SIZE 关键字后引入第四个字段，用于给出字体中字形的每像素位数（bpp）。可能值为 1（默认值）、2（四级灰度）、4（16 级灰度）和 8（256 级灰度）。驱动返回每像素 1 位的位图，或每像素 8 位的像素图；后者分别用于 4、16 和 256 级灰度。

已知问题

* 字体全部加载到内存。这显然不是正确的做法。如果字体很大，建议使用 bdftopcf 工具转换为 PCF 格式；FreeType 的 PCF 字体驱动可增量加载字形。有时间时，我会实现按需解析字形。
* 除编码属性外，客户端应用无法查看 PCF_Face 对象，必须信任 FreeType，不能直接访问字体表。
* 目前忽略字形名称。我计划在下一次修订中完全开放 BDF_Face 对象，并实现字形名称。
* 我从未见过定义垂直度量的 BDF 字体，因此垂直度量在解析后被丢弃。如果您有这样的字体，请告知我，我会在 5-10 分钟内实现它们。

许可证

版权所有 © 2001-2002 Francesco Zappa Nardelli

现免费授予任何获得本软件及相关文档文件（以下简称“软件”）副本的人，不受限制地处置本软件的权利，包括但不限于使用、复制、修改、合并、发布、分发、再许可及／或销售软件副本的权利，并允许向其提供软件的人享有这些权利，但须满足以下条件：

上述版权声明及本许可声明必须包含在本软件的所有副本或实质性部分中。

本软件按“现状”提供，不提供任何明示或默示担保，包括但不限于适销性、特定用途适用性及不侵权的担保。无论依据合同、侵权或其他法律关系，作者或版权持有人在任何情况下均不对因本软件、使用本软件或与本软件有关的其他交易而产生或与之有关的任何索赔、损害或其他责任承担责任。

驱动的部分内容，即 bdflib.c 和 bdf.h：

版权所有 © 2000 Computing Research Labs, New Mexico State University
版权所有 © 2001-2002、2011 Francesco Zappa Nardelli

现免费授予任何获得本软件及相关文档文件（以下简称“软件”）副本的人，不受限制地处置本软件的权利，包括但不限于使用、复制、修改、合并、发布、分发、再许可及／或销售软件副本的权利，并允许向其提供软件的人享有这些权利，但须满足以下条件：

上述版权声明及本许可声明必须包含在本软件的所有副本或实质性部分中。

本软件按“现状”提供，不提供任何明示或默示担保，包括但不限于适销性、特定用途适用性及不侵权的担保。无论依据合同、侵权或其他法律关系，Computing Research Lab 或 New Mexico State University 在任何情况下均不对因本软件、使用本软件或与本软件有关的其他交易而产生或与之有关的任何索赔、损害或其他责任承担责任。

致谢

本驱动基于 Mark Leisher 出色的 BDF 库。如果您觉得驱动有好的地方，可能应感谢他而不是我。
