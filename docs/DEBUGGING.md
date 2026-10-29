# C# 编辑器与断点验收

当前项目：Unity 6000.6.7f1，Visual Studio Editor 包 2.0.26 已在 manifest 中，无需安装旧版 Visual Studio Code Editor 包。

## 1. 先确认 VS Code 能正常启动

2026-10-29 检查：PATH 中的 code.cmd 指向 D:\zachary\courses\vscode\Code.exe，但该可执行文件不存在。目录存在 new_Code.exe，疑似更新未完成，尚未执行文件更名或软件修复。用户报告从 Unity 双击脚本打开为“txt”，所以当前不能声称 IDE 已配置。

先正常退出 VS Code 并重新启动，检查是否能完成自身更新；如果仍不能启动，从 [VS Code 官方下载页](https://code.visualstudio.com/Download)获取 Windows 稳定版安装程序，修复现有安装。保留用户设置和扩展，不删除原目录。安装软件由用户操作或在明确授权后执行。

## 2. 安装官方 Unity 扩展

VS Code → Extensions（Ctrl+Shift+X），安装名称 Unity、发布者 Microsoft、标识 VisualStudioToolsForUnity.vstuc 的扩展；它会安装所需的 C#、C# Dev Kit 依赖。根据扩展提示完成所需 .NET 环境，不根据旧教程随意固定 SDK 版本。

## 3. 从 Unity 打开整个 C# 工程

Unity → Edit → Preferences → External Tools：
- External Script Editor 选择 Visual Studio Code；未列出时 Browse 选择修复后实际存在的 Code.exe。
- 使用 Regenerate project files 重新生成工程文件。
- 回到 Project 窗口，双击 Assets/_Game/Scripts/ClickableSquare.cs。

应打开包含本工程的 VS Code 窗口，而不是只有一个独立文本文件的查看器。等待 C# 工程加载完成。

## 4. 补全验证

在 Update 中临时输入 `mouse.`，检查建议是否包含 leftButton、position；随后撤销临时输入，保持脚本原样。也可悬停 SpriteRenderer 查看类型说明。

## 5. 断点验证

1. 退出 Unity Play。在 `isSelected = !isSelected;` 那行左侧点击，设置断点。
2. 在 VS Code 按 F5；如需选择，使用命令面板 Attach Unity Debugger，选择本工程 Unity Editor。目标是 Editor，不是刚才生成的独立程序。
3. 回 Unity 进入 Play，切换到 Game 并点击方块。
4. 预期暂停在断点行；观察 isSelected 初值为 false。
5. 按 F10 执行当前行后，它应变为 true；再 F10 执行赋色行，F5 继续。游戏画面恢复刷新后显示选中色。
6. 再点击一次并检查 true → false，F5 继续。完成后 Shift+F5 断开调试，再退出 Unity Play。

空心断点通常表示尚未绑定；没有命中前不记录为通过。记录编辑器及扩展版本、命中结果、状态值；有错误时记录原文。

## 学习目标

断点暂停在当前行执行前；F10 单步执行，F5 继续运行。练习用观察到的变量值解释状态切换，避免只凭画面猜测执行顺序。

官方依据：[VS Code 的 Unity 配置与调试](https://code.visualstudio.com/docs/other/unity)、[Microsoft Unity 扩展](https://marketplace.visualstudio.com/items?itemName=VisualStudioToolsForUnity.vstuc)。
