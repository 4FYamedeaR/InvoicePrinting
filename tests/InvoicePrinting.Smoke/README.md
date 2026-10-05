# 导入、布局与界面验证

本项目是调用应用源码的可执行验证程序，使用 Windows x64 和 .NET 10 SDK。下面的命令均从仓库根目录运行，首次运行会还原 NuGet 包。

## 常规验证

```powershell
dotnet run --project tests/InvoicePrinting.Smoke/InvoicePrinting.Smoke.csproj -r win-x64
```

常规验证会在系统临时目录创建并清理自己生成的 PNG 和 PDF 样例，检查内容自动旋转、未填满四格的布局，以及图片和 PDF 导入。验证使用生成的样例，不读取用户发票，也不提交打印任务。

程序通过断言检查结果；验证成功时退出码为 `0`，出现异常或断言失败时为 `1`。这是可执行验证项目，请使用上述 `dotnet run` 命令运行。

## 离屏界面验证

```powershell
dotnet run --project tests/InvoicePrinting.Smoke/InvoicePrinting.Smoke.csproj -r win-x64 -- --visual-review
```

该模式加载应用真实的 WPF `MainWindow` 内容树，使用 `RenderTargetBitmap` 在离屏状态生成截图。发票图像由代码绘制，文件名、公司、号码和金额均为模拟数据。

检查包括空任务状态、纵向双格和横向四格、较小窗口的控件边界、翻页、缩放、适合窗口、旋转、移除及导入忙碌状态。它还比较预览与内存中打印文档的分页、图像和位置；不会显示应用窗口或打印对话框，也不会向打印机提交任务。

减少动画的验证单独运行：

```powershell
dotnet run --project tests/InvoicePrinting.Smoke/InvoicePrinting.Smoke.csproj -r win-x64 -- --visual-review --reduced-motion
```

此命令在验证进程内模拟 WPF 的减少动画设置，检查启动和忙碌状态的动画行为，不修改 Windows 的用户设置。部分状态通过反射和事件调用模拟；离屏验证不能替代对原生弹出窗口、打印机驱动和实际纸面输出的人工检查。

## 输出与维护

离屏验证将 PNG 截图与报告写入仓库根目录的 `artifacts/ui-review/`：普通模式的报告为 `verification.txt`，减少动画模式的报告为 `reduced-motion-verification.txt`。重复运行会覆盖同名输出，出现失败时请结合终端异常和报告检查原因。

- `Program.cs`：常规导入和布局断言，以及生成 PDF 样例的代码。
- `VisualReview.cs`：模拟票据、真实 WPF 内容树渲染和界面断言。
- `InvoicePrinting.Smoke.csproj`：验证项目，通过项目引用使用 `src/InvoicePrinting/` 中的应用代码。

生成的验证输出不纳入源码管理。需要更新项目展示截图时，先检查 `artifacts/ui-review/` 中的图片，再将选定图片保存到 `docs/screenshots/` 并更新 README 引用。

提交改进要求见[贡献指南](../../CONTRIBUTING.md)，目录约定见[文档索引](../../docs/README.md)。
