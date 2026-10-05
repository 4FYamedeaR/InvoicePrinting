中文参考译文
对应原文：libtiff-README.pdfium.txt
正式条款以原文为准；本译文不替代随附原文。此文件为历史参考元数据，不证明当前 DLL 的确切版本或补丁。

名称：LibTIFF
URL：http://www.simplesystems.org/libtiff/
版本：4.0.8
安全关键组件：是
许可证：BSD

说明：TIFF 库。

本地修改：
0000-build-config.patch：本地构建配置修改。
0001-build-config.patch：为 VS 2015 在 tiffconf.h 中启用 HAVE_SEARCH_H。
0006-HeapBufferOverflow-ChopUpSingleUncompressedStrip.patch：修复堆缓冲区溢出。
0008-HeapBufferOverflow-ChopUpSingleUncompressedStrip.patch：修复堆缓冲区溢出。
0017-safe_skews_in_gtTileContig.patch：如果转换为或转换自 int32 时偏移量溢出，则返回错误。
0025-upstream-OOM-gtTileContig：仅在第一次 TIFFFillStrip 成功之后分配解码缓冲区。
0026-upstream-null-dereference：启用 stoponerr 时正确退出，并避免空指针解引用。
0027-build-config.patch：使用 #define 定义变量，以便其值可用于 #if。
0028-nstrips-OOM.patch：条带或分块数量过多时返回错误。
