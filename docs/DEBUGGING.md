# C# 编辑器与断点验收

当前项目：Unity 6000.6.7f1，Visual Studio Editor 包 2.0.26 已在 manifest 中，无需安装旧版 Visual Studio Code Editor 包。

## 当前状态 · 2026-10-29

用户已完成安装修复与 Unity 关联设置。Code.exe 已恢复，版本 1.144.0；已检测到 Unity 1.3.2、C# Dev Kit 3.40.215、.NET Install Tool 3.2.0、C# 2.160.4。后续安装的系统 SDK 10.0.403 已由 dotnet --list-sdks 确认；日志显示 .NET 服务启动、UnityProject.slnx 成功打开和 Assembly-CSharp.csproj 加载。用户已确认断点命中，并观察到 false → true → false；断点验收通过，补全建议列表尚无单独反馈。以下故障段落保留为排障历史。

### 重载后提示 Workspace Requirements Not Met

最新截图明确要求 .NET 10.x 或更高 SDK，建议 10.0.403；本机 dotnet --list-sdks 仍无输出。先点击界面的 Resolve Requirements，按提示安装 SDK。自动下载失败时，从 [Microsoft .NET 10 下载页](https://dotnet.microsoft.com/zh-cn/download/dotnet/10.0)的“生成应用 - SDK”选择 SDK 10.0.403 的 Windows x64 安装程序。截图底部下载的 Runtime 不是 SDK，不能单独满足要求。

完成后完全退出并重开 VS Code，从 Unity 双击脚本打开工程。预期工作区依赖错误消失，UnityProject.slnx 加载，随后再验证补全与 F5。新终端可用 dotnet --list-sdks 检查系统安装的版本；若扩展采用私有安装，还应检查其 Found SDKs and Runtimes 面板。未实际确认前，不记录为安装通过。此 SDK 用于 VS Code 开发工具，不改变 Unity 项目的目标框架。

### F5 提示找不到 vstuc 调试适配器

本机日志已定位：本次 VS Code 会话启动时，C# Dev Kit 因 C# 扩展未就绪而激活失败；随后 C# 扩展才完成安装，但失败的依赖尚未成功重新激活。先保存编辑内容、取消错误弹窗，按 Ctrl+Shift+P 执行 `Developer: Reload Window`（开发人员：重新加载窗口），等待扩展启动后再按 F5。保留现有 launch.json，`type: vstuc` 是正确的 Unity 调试配置。

重载后仍失败时，记录新的通知和 Output 中 C# Dev Kit / C# / .NET Install Tool 的最新错误，再处理缺失组件或网络问题；不要根据此弹窗直接更改项目代码或更换调试器类型。

## 1. 先确认 VS Code 能正常启动

2026-10-29 检查：PATH 中的 code.cmd 指向 D:\zachary\courses\vscode\Code.exe，但该可执行文件不存在。目录存在 new_Code.exe，疑似更新未完成，尚未执行文件更名或软件修复。用户报告从 Unity 双击脚本打开为“txt”，所以当前不能声称 IDE 已配置。

用户反馈 External Script Editor 中找不到 VS Code 后再次检查：旧安装目录及其版本子目录仅找到 new_Code.exe（文件产品信息为 Visual Studio Code 1.144.0），没有 Code.exe；常见 C 盘安装位置也未找到 Code.exe。建议用官方 Windows User Setup 修复安装到原目录，再核实 Code.exe 出现并能启动，随后设置 External Script Editor；不把更新临时文件直接作为长期路径。

先正常退出 VS Code 并重新启动，检查是否能完成自身更新；如果仍不能启动，从 [VS Code 官方下载页](https://code.visualstudio.com/Download)获取 Windows 稳定版安装程序，修复现有安装。保留用户设置和扩展，不删除原目录。安装软件由用户操作或在明确授权后执行。

## 2. 安装官方 Unity 扩展

VS Code → Extensions（Ctrl+Shift+X），安装名称 Unity、发布者 Microsoft、标识 VisualStudioToolsForUnity.vstuc 的扩展；它会安装所需的 C#、C# Dev Kit 依赖。根据扩展提示完成所需 .NET 环境，不根据旧教程随意固定 SDK 版本。

可搜索 `@id:ms-dotnettools.csharp`，确认 Microsoft 的 C# 扩展安装完成且已启用；本机已检测到它的安装目录，无需重复安装。仅有扩展目录和工程文件不能证明语言服务已成功启动。

## 3. 从 Unity 打开整个 C# 工程

Unity → Edit → Preferences → External Tools：
- External Script Editor 选择 Visual Studio Code；未列出时 Browse 选择修复后实际存在的 Code.exe。
- 使用 Regenerate project files 重新生成工程文件。
- 回到 Project 窗口，双击 Assets/_Game/Scripts/ClickableSquare.cs。

应打开包含本工程的 VS Code 窗口，而不是只有一个独立文本文件的查看器。等待 C# 工程加载完成。

## 4. 补全验证

在现有 `mouse.leftButton` 中，把光标放在点号后、leftButton 前，按 Ctrl+Space，检查建议是否包含 leftButton、position，按 Escape 关闭列表，不修改脚本。如果快捷键被输入法拦截，用命令面板运行 Trigger Suggest。也可悬停 SpriteRenderer 查看类型说明。这样即使用户开了自动保存，也不会因临时输入不完整语句而触发 Unity 编译错误。

## 5. 断点验证

1. 退出 Unity Play。在 `isSelected = !isSelected;` 那行左侧点击，设置断点（当前脚本第 60 行，以代码内容为准）。
2. 在 VS Code 按 F5；如需选择，使用命令面板 Attach Unity Debugger，选择本工程 Unity Editor。目标是 Editor，不是刚才生成的独立程序。
3. 回 Unity 进入 Play，切换到 Game 并点击方块。
4. 预期暂停在断点行；观察 isSelected 初值为 false。
5. 按 F10 执行当前行后，它应变为 true；再 F10 执行赋色行，F5 继续。游戏画面恢复刷新后显示选中色。
6. 再点击一次并检查 true → false，F5 继续。完成后 Shift+F5 断开调试，再退出 Unity Play。

空心断点通常表示尚未绑定；没有命中前不记录为通过。记录编辑器及扩展版本、命中结果、状态值；有错误时记录原文。

## 学习目标

断点暂停在当前行执行前；F10 单步执行，F5 继续运行。练习用观察到的变量值解释状态切换，避免只凭画面猜测执行顺序。

官方依据：[VS Code 的 Unity 配置与调试](https://code.visualstudio.com/docs/other/unity)、[Microsoft Unity 扩展](https://marketplace.visualstudio.com/items?itemName=VisualStudioToolsForUnity.vstuc)。
