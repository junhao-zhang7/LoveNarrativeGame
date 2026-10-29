# 原创 2D 叙事游戏（开发代号待定）

目标：制作可商业发行的原创短篇单机游戏，并积累能够独立讲解的工程作品。优先 Windows / Steam，移动版在 Windows 稳定后评估。当前只完成仓库准备，**尚未生成或运行 Unity 工程**。

## 首轮环境检查 · 2026-10-29

| 项目 | 实际发现 |
| --- | --- |
| 工作目录 | 初始为空，无既有 Unity 工程、Git 仓库或 AGENTS.md |
| Git | 2.53.0.windows.1；已有本机提交身份 |
| 编辑器 | VS Code 命令可用；C# 补全、调试尚未验证 |
| .NET | 有运行库 7.0.20、9.0.5；无独立 SDK |
| Unity | 已检查常见目录、卸载注册表、Hub 配置与进程，未发现安装；用户确认尚未安装 |
| 可投入时间 | 用户确认每周 10 小时以上 |

Unity 自带自己的脚本编译工具链；独立 .NET SDK 不是编辑器内编译的替代品。VS Code 的 C# 工具若提示缺 SDK，再按其官方要求配置；暂不为此安装多个环境。

## 你现在需要执行的操作

1. 从 [Unity 官方下载页](https://unity.com/download)安装 Unity Hub，登录并完成适用的许可证激活。账号步骤由你操作。
2. 在 Hub 安装 Unity **6.3 LTS（6000.3.x）**中当时可用的稳定补丁。记录完整版本号。先满足 Windows 编辑器与 Windows 构建需求；当前不安装 Android / iOS 模块。首个测试构建使用 Mono；如切换 IL2CPP，再检查对应 Windows 构建模块与 C++ 工具链要求。
3. Hub → Projects → New project，选上述 Editor。优先选择描述为 **2D / Built-In Render Pipeline** 的核心模板；名称以实际 Hub 显示为准。其设置较少，适合当前占位交互。若只看到 Universal 2D，请先记录可用模板，不要自行改装渲染管线。
4. 项目名称填 `UnityProject`，Location 填 `C:\Users\张俊豪\Desktop\LoveNarrativeGame`，确保最终路径为 `...\LoveNarrativeGame\UnityProject`。当前仓库已有文档，不要让 Hub 覆盖它；无需额外启用 Unity Version Control。
5. 等首次导入结束，确认 Console 无红色错误。把完整版本号与任何错误原文带回；随后共同检查 Packages/manifest.json 和 packages-lock.json，再确定官方包的实际兼容版本。
6. 在 Project Settings 中检查 Version Control 为 Visible Meta Files、Asset Serialization 为 Force Text。将 VS Code 设为 External Script Editor；后续用一个小脚本验证补全、编译与断点。
7. 在编辑器 Project 窗口创建 `Assets/_Game/Scenes`、`Scripts`、`Prefabs`。把测试场景另存为 `Assets/_Game/Scenes/Bootstrap.unity`。暂不创建尚无用途的大量目录。
8. 给场景添加一个 Square Sprite，摆到正交相机可见处，保存。进入 Play，确认 Game 视图可见方块，退出 Play 后场景仍保留，Console 无红色错误。此时只验证渲染流程，不宣称有玩法。
9. File → Build Profiles，选择或创建 Windows 配置，使用 x86-64、Mono，把 Bootstrap 加入 Scene List 并启用。生成 Development Build 到仓库根目录 `Builds/Windows/`。退出编辑器 Play 后单独启动 exe，确认显示一致；分发时必须保留构建生成的配套文件。
10. 记录实际结果到 docs/PROGRESS.md，检查 `git status`：场景及 .meta、Packages、ProjectSettings 应出现；Library、Temp、Builds 应不出现。之后才建立 Unity 工程的初始化提交。

如出现 Hub 安装、许可证、包下载、中文路径或编译错误，先记录错误及日志位置，再决定是否调整；没有证据时不迁移项目或重复安装。

## 项目导航

- [执行计划与验收](docs/ROADMAP.md)
- [进度、决策与验证记录](docs/PROGRESS.md)
- [素材来源与授权台账](docs/ASSET_LICENSES.csv)
- [长期协作规则](AGENTS.md)

后续才按实际需要新增 GDD、模块设计、DEVLOG 和 PORTFOLIO；现在的决策及工作记录集中在 PROGRESS，避免空文档堆积。项目暂未授予公共开源许可证，也未创建远程仓库。

## 官方核验入口

- [Unity 6 支持策略](https://unity.com/releases/unity-6/support)：已确认 6.3 LTS 分支存在；安装后锁定实际补丁。
- [Unity 6.3 工程配置](https://docs.unity.com/en-us/engine/6000.3/manual/get-started/project-configuration)
- [Hub 模板说明](https://docs.unity.com/en-us/hub/templates)：以本机所选版本的模板描述为准。
- [Unity 6.3 Windows 构建说明](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/windows/standalone-binaries)
