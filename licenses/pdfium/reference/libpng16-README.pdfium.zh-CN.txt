中文参考译文
对应原文：libpng16-README.pdfium.txt
正式条款以原文为准；本译文不替代随附原文。此文件为历史参考元数据，不证明当前 DLL 的确切版本或补丁。

名称：libpng
URL：http://libpng.org/
版本：1.6.22
安全关键组件：是
许可证：libpng 许可证
许可证与 Android 兼容：是

说明：PNG 库。

本地修改：
Chromium 的 libpng 副本截至 https://crrev.com/404379 的本地修改，见 README.chromium。
pnglibconf.h：libpng 的 scripts/pnglibconf.h.prebuilt 的副本。
pngprefix.h：为避免与 Chromium 冲突而手动创建的重定义。
0000-build-config.patch：本地构建配置修改。
0002-static-png-gt.patch：在 png.c 中无条件使用静态 png_gt()，以避免编译警告。
0003-check-errors-in-set-pcal.patch：回移植 github.com/glennrp/libpng/pull/135。
0004-invalid-icc.patch：修复无效 ICC 导致的大量内存分配，见 https://github.com/glennrp/libpng/commit/92a7c79db2c962d04006b35e2603ba9d5ce75541。
