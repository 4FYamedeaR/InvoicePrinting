中文参考译文
对应原文：libopenjpeg20-README.pdfium.txt
正式条款以原文为准；本译文不替代随附原文。此文件为历史参考元数据，不证明当前 DLL 的确切版本或补丁。

名称：OpenJPEG
URL：http://www.openjpeg.org/
版本：2.3.0（还须在 opj_config* 中更新）
安全关键组件：是
许可证：二条款 BSD

说明：JPEG 2000 库。

本地修改：
0000-use-colorspace.patch：允许不调用 opj_jp2_apply_pclr()。
0003-dwt-decode.patch：检查 opj_dwt_decode_1() 及相关函数的数组边界。
0005-jp2_apply_pclr.patch：修复越界访问。
0006-tcd_init_tile.patch：修复 opj_tcd_init_tile() 中的除零错误。
0007-jp2_read_cmap.patch：修复采用索引色空间的灰度图像渲染错误。
0009-opj_pi_next.patch：修复 opj_pi_next* 函数中 precno 值可能不正确的问题。
0011-j2k_update_image_data.patch：防止错误的有符号到无符号类型转换。
0012-mct_sse.patch：32 位构建不使用 SSE 内建函数。
0014-opj_jp2_read_ihdr_leak.patch：修复 opj_jp2_read_ihdr() 中的内存泄漏。
0015-read_SPCod_SPCoc_overflow.patch：防止 opj_j2k_read_SPCod_SPCoc 中的缓冲区溢出。
0016-read_SQcd_SQcc_overflow.patch：防止 opj_j2k_read_SQcd_SQcc 中的缓冲区溢出。
0019-tcd_init_tile.patch：防止计算 |l_nb_code_blocks_size| 时的整数溢出。
0022-jp2_apply_pclr_overflow.patch：防止 opj_jp2_apply_pclr 中的整数溢出。
0023-opj_j2k_read_mct_records.patch：修复 opj_j2k_read，防止释放后的堆内存被使用。
0025-opj_j2k_add_mct_null_data.patch：尝试读取 m_data 前检查 m_data != null。
0026-use_opj_uint_ceildiv.patch：移除 (OPJ_UINT32)opj_int_ceildiv((OPJ_INT32)a, (OPJ_INT32) b)。
0033-undefined-shift-opj_t1_dec_clnpass.patch：修复源自 opj_t1_decode_cblk 的未定义移位。
0034-opj_malloc.patch：PDFium 在 opj_malloc 中的修改。
0035-opj_j2k_update_image_dimensions.patch：修复整数溢出。
