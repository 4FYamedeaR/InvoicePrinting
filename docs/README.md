# 文档与目录说明

使用方法、环境要求和发布命令见[项目 README](../README.md)，开发与提交要求见[贡献指南](../CONTRIBUTING.md)。

## 文档索引

| 内容 | 位置 | 用途 |
| --- | --- | --- |
| 产品设计 | [产品设计文档](design/产品设计文档.md) | 产品规划、界面和后续功能记录；其中的规划不等同于当前已实现功能 |
| 展示截图 | [screenshots/](screenshots/) | README 使用的应用截图，内容为模拟发票数据 |
| 验证说明 | [验证项目 README](../tests/InvoicePrinting.Smoke/README.md) | 导入、布局和离屏界面验证的运行方式与覆盖范围 |
| 许可资料 | [许可证资料索引](../licenses/README.md) | 第三方许可原文、中文参考译文和来源记录 |
| 许可核验 | [工具说明](../tools/licensing/README.md) | 许可资料一致性和发布副本检查 |

## 文件存放约定

- 根目录保留 `README.md`、`CONTRIBUTING.md`、`InvoicePrinting.slnx`、`Directory.Build.props`、`.gitignore`、`.gitattributes`，以及项目许可证和发布声明，作为使用、开发与构建入口。
- `src/InvoicePrinting/` 存放 WPF 应用源码、项目文件和发布配置。
- `tests/InvoicePrinting.Smoke/` 存放可公开运行的验证代码及其说明，样例由代码生成。
- `tools/` 存放可复用的开发辅助工具；工具自身的用法记录在对应目录的 README。
- `docs/design/` 集中存放产品设计文档。描述当前功能时，同步检查项目 README 和源码实现。
- `docs/screenshots/` 保存经选择、需要随源码展示的截图。界面变更后先生成并检查新截图，再更新此目录及 README 引用。
- `licenses/` 保存需要随项目及发布版分发的第三方许可资料。
- `artifacts/` 存放本地生成的构建、发布、截图和核验报告，以及旧缓存归档，不纳入源码管理。当前二进制输出为 `artifacts/bin/`，中间文件为 `artifacts/obj/`，正式发布输出为 `artifacts/single-file/`。
- `artifacts/legacy-cache/root-project/bin/` 和 `artifacts/legacy-cache/root-project/obj/` 保存旧根目录的构建输出与中间文件。本次整理仅将内容归档保留，没有删除这些文件；归档不参与当前构建，也不作为运行或发布入口。

离屏界面验证会将完整截图集写入 `artifacts/ui-review/`。对外展示的图片从中选取后保存到 `docs/screenshots/`，并使用模拟数据或已去除敏感信息的样例。
