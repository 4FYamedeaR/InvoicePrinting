中文参考译文
对应原文：FreeType-src-pcf-README.txt
正式条款以原文为准；本译文不替代随附原文。

用于 PCF 字体的 FreeType 字体驱动
Francesco Zappa Nardelli
<francesco.zappa.nardelli@ens.fr>

引言

PCF（Portable Compiled Format，可移植编译格式）是 X 环境广泛使用的二进制位图字体格式。本代码实现 FreeType 库的 PCF 驱动。字形图像仅在需要时加载到内存，因此内存占用很小。

只能从 pcfread.c 和 pcfwrite.c 推断 PCF 字体格式信息；例如可在 XFree86 (www.xfree86.org) 源码树 xc/lib/font/bitmap/ 中找到这些文件。

XFree86 提供很多优质 BDF 位图字体，可用 bdftopcf 工具编译为 PCF 格式。

支持的硬件

该驱动已在 linux/x86 和 sunos5.5/sparc 上测试，两者都使用 gcc 编译器。回到巴黎后，我也会在 linux/alpha 上测试。

编码

使用 FT_Get_BDF_Charset_ID 可访问编码和注册表。

驱动始终将 ft_encoding_none 作为 face->charmap.encoding 导出。FT_Get_Char_Index() 的行为没有变化，仍将给定的 ULong 值转换为相应字形编号。

已知问题

* 显式处理编码打破了 FreeType 2 API 的统一性。
* 除编码属性外，客户端应用无法查看 PCF_Face 对象，必须信任 FreeType，不能直接访问字体表。
* 目前忽略字形名称和 ink_metrics。我计划在下一次修订中完全开放 PCF_Face 对象，并实现字形名称和 ink_metrics。
* 高度定义为 ascent - descent（上升高度减下降高度），这是否正确？
* 若无法从字体读取尺寸信息，PCF_Init_Face 将 available_size->width 和 available_size->height 都设为 12。
* README 文件中有太多英语语法错误 :-(

许可证

版权所有 © 2000 Francesco Zappa Nardelli

现免费授予任何获得本软件及相关文档文件（以下简称“软件”）副本的人，不受限制地处置本软件的权利，包括但不限于使用、复制、修改、合并、发布、分发、再许可及／或销售软件副本的权利，并允许向其提供软件的人享有这些权利，但须满足以下条件：

上述版权声明及本许可声明必须包含在本软件的所有副本或实质性部分中。

本软件按“现状”提供，不提供任何明示或默示担保，包括但不限于适销性、特定用途适用性及不侵权的担保。无论依据合同、侵权或其他法律关系，作者或版权持有人在任何情况下均不对因本软件、使用本软件或与本软件有关的其他交易而产生或与之有关的任何索赔、损害或其他责任承担责任。

致谢

Keith Packard 编写了 XFree86 中的 PCF 驱动。他的工作同时充当 PCF 格式的规范和示例实现。毫无疑问，本驱动受到他的工作启发。
