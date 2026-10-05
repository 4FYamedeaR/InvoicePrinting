中文参考译文

对应原文：`libjpeg-turbo-LICENSE.md`。正式条款以原文为准；本译文不替代随附原文。

# libjpeg-turbo 的许可证

libjpeg-turbo 适用三种相互兼容的 BSD 风格开源许可证：

- IJG（Independent JPEG Group）许可证，列于 `README.ijg`。该许可证适用于 libjpeg API 库及相关程序，即从 libjpeg 继承的代码以及对该代码的任何修改。
- 修改版（三条款）BSD 许可证，列于 `turbojpeg.c`。该许可证适用于 TurboJPEG API 库及相关程序。
- zlib 许可证，列于 `simd/jsimdext.inc`。该许可证的条款是另外两种许可证条款的子集，适用于 libjpeg-turbo SIMD 扩展。

# 遵守 libjpeg-turbo 许可证

本节依据我们对条款的理解，对 libjpeg-turbo 的许可要求作出汇总。

1. 若分发修改过的 libjpeg-turbo 源代码：
   1. 不得修改或删除源代码中已有的版权或许可声明。依据：IJG 许可证第 1 条、修改版 BSD 许可证第 1 条、zlib 许可证第 1 条和第 3 条。
   2. 必须在每个修改过的源文件头部增加您自己的版权声明，使他人能够识别文件曾被修改；若文件原本没有版权头，可直接增加说明该文件已被修改的声明。依据：IJG 许可证第 1 条、zlib 许可证第 2 条。
   3. 必须包含 IJG 的 README 文件，且不得修改其中的任何版权或许可文字。依据：IJG 许可证第 1 条。
2. 若只分发不含源代码的 libjpeg-turbo 二进制文件，或分发静态链接 libjpeg-turbo 的应用：
   1. 产品文档必须包含以下声明，必要英文原句予以保留：

      “This software is based in part on the work of the Independent JPEG Group.”

      中文含义：“本软件的部分内容基于 Independent JPEG Group 的工作。”依据：IJG 许可证第 2 条。
   2. 若二进制发行包包含或使用 TurboJPEG API，产品文档必须包含修改版 BSD 许可证全文。依据：修改版 BSD 许可证第 2 条。
3. 不得在广告、宣传等活动中使用 IJG、libjpeg-turbo 项目或其贡献者的名称。依据：IJG 许可证、修改版 BSD 许可证第 3 条。
4. IJG 和 libjpeg-turbo 项目不担保 libjpeg-turbo 不存在缺陷，也不对您使用本软件造成的不良后果承担责任。依据：IJG 许可证、修改版 BSD 许可证及 zlib 许可证。
