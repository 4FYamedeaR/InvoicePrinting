中文参考译文
对应原文：agg23-README.pdfium.txt
正式条款以原文为准；本译文不替代随附原文。此文件为历史参考元数据，不证明当前 DLL 的确切版本或补丁。

名称：Anti-Grain Geometry
URL：https://sourceforge.net/projects/agg/
版本：2.3
安全关键组件：是
许可证：MIT

说明：二维矢量图形库。

本地修改：
0000-bug-466.patch：修复 stroke_calc_arc() 中的无限循环。
为使用 FX_ 库函数而进行的各种修改。
可能还有更多修改？
0001-gcc-warning.patch：修复条件表达式中枚举与非枚举类型混用所导致的 GCC 警告。
0002-ubsan-error-fixes.path：修复溢出导致的 UBSan 错误。
0003-ubsan-render-line-error.patch：修复 render_line 中的 UBSan 溢出错误。
