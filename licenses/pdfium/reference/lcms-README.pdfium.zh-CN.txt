中文参考译文
对应原文：lcms-README.pdfium.txt
正式条款以原文为准；本译文不替代随附原文。此文件为历史参考元数据，不证明当前 DLL 的确切版本或补丁。

名称：Little CMS
URL：http://www.littlecms.com/
版本：2.8
安全关键组件：是
许可证：MIT 许可证

说明：色彩管理引擎。

本地修改：
0000-cmserr-changes.patch：将 LCMS 的内存管理方法改为使用 PDFium 的方法。
0001-fix-include.patch：修复 lcms2_internal.h 中的 include。
0002-old-performance-fix.patch：https://codereview.chromium.org/534363002/
0003-old-uninitialized-in-LUTevalFloat.patch：https://codereview.chromium.org/380293002/
0004-old-uninitialized-in-LUTeval16.patch：https://codereview.chromium.org/387273002/
0005-old-fix-e-with-tilde.patch：类似 https://codereview.chromium.org/2411123003/，但有所改进。
0006-tag-type-confusion.patch：修复类型混淆。
0008-infinite-loop-GrowNamedColorList.patch：修复调用 GrowNamedColorList 时的无限循环。
0009-uninit.patch：修复使用未初始化值和栈缓冲区越界读取。
0010-memory-leak-Type_Curve_Read.patch：修复 Type_Curve_Read 中的内存泄漏。
0011-memory-leak-AllocEmptyTransform.patch：修复 AllocEmptyTransform 中的内存泄漏。
0012-memory-leak-Type_NamedColor_Read.patch：修复 Type_NamedColor_Read 中的内存泄漏。
0013-memory-leak-OptimizeByResampling.patch：修复 OptimizeByResampling 中的内存泄漏。
0014-memory-leak-Type_MPEmatrix_Read.patch：修复 Type_MPEmatrix_Read 中的内存泄漏。
0015-cmsStageAllocMatrix-param-swap.patch：修复 cmsStageAllocMatrix 中行列交换的问题。
0016-reject-nan.patch：读取浮点数时拒绝 NaN。
0017-memory-leak-ReadSegmentedCurve.patch：修复 ReadSegmentedCurve 中的内存泄漏。
0018-backport-c0a98d86.patch：修复若干问题，从上游 https://github.com/mm2/Little-CMS/commit/c0a98d86 回移植。
0019-utf8.patch：将源文件编码为 UTF-8。
0020-avoid-fixed-inf.patch：避免对无穷值进行定点数字 LUT 优化。
0021-sanitize-float-read.patch：校验浮点读取；部分回移植自上游 https://github.com/mm2/Little-CMS/commit/4011a6e3。
0022-check-LUT-and-MPE.patch：检查 LUT 一致性和校验 MPE 配置文件。
0023-upstream-integer-overflow-MPEmatrix_Read.patch：修复一些整数溢出。
0024-verify-size-before-reading.patch：若实际没有足够数据可读，修复由此造成的内存不足问题。
0025-upstream-direct-leak-Type_MPE_Read.patch：修复 cmstypes.c 中的内存泄漏。
0026-more-unsupported-characters.patch：移除其他不受支持的字符。
0027-changes-from-beginning-of-time.patch：为初始提交以来的修改增加注释。
0028-do-not-quickfloor.patch：取整错误可能造成堆缓冲区越界。
0029-drop-register-keyword.patch：移除已弃用的 register 关键字。
