中文参考译文
对应原文：freetype-README.pdfium.txt
正式条款以原文为准；本译文不替代随附原文。此文件为历史参考元数据，不证明当前 DLL 的确切版本或补丁。

名称：FreeType
URL：http://www.freetype.org/
版本：VER-2-9-20
源码修订：4a03f17449ae45f0dacf4de4694ccd6e5e1b24d1
安全关键组件：是
许可证：FreeType 许可证（FTL）
许可文件：FTL.TXT

说明：FreeType 库。

本地修改：
include/pstables.h：freetype/src/psnames/pstables.h 的副本。该文件不是 FreeType 公共 API 的一部分，但 PDFium 需要它。使用系统 FreeType 构建时无法取得该文件，因此为方便使用而提供其副本。
0000-include.patch：修改配置头文件。

可通过 roll-freetype.sh 将大部分更新工作自动化。
