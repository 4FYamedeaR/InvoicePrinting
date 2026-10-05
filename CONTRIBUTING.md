# 贡献指南

感谢您参与 InvoicePrinting。项目原始代码和文档采用 Apache 许可证 2.0 版（Apache-2.0），详见[中文参考译文](LICENSE.zh-CN)和[英文正式条款](LICENSE)；第三方组件保留各自的许可证。

## 开发环境

使用 Windows x64 和 .NET 10 SDK，在仓库根目录执行：

```powershell
dotnet restore src/InvoicePrinting/InvoicePrinting.csproj -r win-x64
dotnet build InvoicePrinting.slnx -c Release
dotnet run --project src/InvoicePrinting/InvoicePrinting.csproj -r win-x64
```

功能和发布说明见 [README](README.md)，文档与文件存放约定见[文档索引](docs/README.md)。应用源码位于 `src/InvoicePrinting/`，验证代码位于 `tests/InvoicePrinting.Smoke/`；构建二进制和中间文件集中写入 `artifacts/bin/`、`artifacts/obj/`。

开发工具存放于 `tools/`，产品设计和展示截图存放于 `docs/`。`artifacts/legacy-cache/root-project/` 只保留旧根目录的 `bin/`、`obj/` 文件，不参与当前构建；当前构建和发布命令仍使用上述源码路径。

## 本地验证

在仓库根目录运行导入和布局验证：

```powershell
dotnet run --project tests/InvoicePrinting.Smoke/InvoicePrinting.Smoke.csproj -r win-x64
```

常规验证会在系统临时目录创建并清理自己生成的样例。修改界面时，再运行离屏界面验证：

```powershell
dotnet run --project tests/InvoicePrinting.Smoke/InvoicePrinting.Smoke.csproj -r win-x64 -- --visual-review
dotnet run --project tests/InvoicePrinting.Smoke/InvoicePrinting.Smoke.csproj -r win-x64 -- --visual-review --reduced-motion
```

离屏验证使用真实 WPF 界面和模拟发票数据，检查布局与交互并将截图和报告写入 `artifacts/ui-review/`，不会提交打印任务。命令、覆盖范围及限制见[验证项目说明](tests/InvoicePrinting.Smoke/README.md)。涉及打印的修改仍应在实际打印机上检查纸面结果。

许可资料修改后，可使用[许可资料核验工具](tools/licensing/README.md)检查文件、来源记录和发布副本是否一致。该工具需要 Python 3.9 或更高版本；Python 只用于这项可选开发检查，应用运行不需要安装 Python。

## 提交问题

请说明 Windows 版本、文件格式、复现步骤、预期结果和实际结果。打印问题请补充打印机型号、纸张方向和排版选项。附带样例时，请使用合成票据或已删除个人、财务敏感信息的文件。

## 提交改进

- 一份合并请求（Pull Request）尽量解决一个明确的问题，并说明修改原因和验证方式。
- 修改导入、布局或打印逻辑时，请验证相关格式、横纵方向、分页和预览；涉及实际纸面输出时，请说明使用的打印机及结果。
- 修改用户操作或功能限制时，请同步更新 README。
- 提交前检查 `git diff --cached`，确认没有密钥、令牌、私钥、真实票据、个人信息或本机私密配置；展示及验证样例使用模拟数据。`.gitignore` 已忽略常见私密配置和密钥文件，其他敏感内容仍需检查。
- 添加或更新依赖时，请同步更新第三方组件说明、许可证正文及来源记录。原生 DLL 中的内嵌组件也属于依赖范围。
- 保留上游代码中的版权、许可证和署名声明；修改第三方文件时，按其许可证要求注明修改。

请仅提交您有权贡献的代码、文档和素材。除非另有明确约定，提交用于合入本项目的贡献适用 Apache-2.0。雇佣、委托或合作开发的内容应事先取得相应权利人的授权。
