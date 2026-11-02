# 原创 2D 叙事游戏（开发代号待定）

**第三至第五幕互动已修订（2026-11-02）：** 第三幕新增脸红对视特写与拖把擦水；第四幕可选择直接递卡或装酷，两者都归还；第五幕默认合作推导，自主解题可选且可随时返回。保留连续播放和解锁存档，编辑器检查与Windows构建通过，[打开新版](Builds/MenuDemo/MenuDemo.exe)，按[本轮测试步骤](docs/ACTS_3_4_5_INTERACTIONS.md)验收。以下为历史记录。

**第一幕小道寻路已接入（2026-11-02）：** “我发现一条小道”后进入练习本校园图，找到最短路线再接“从后面绕——”与老师打断。可撤回/重画，保留连续播放和存档。独立编译、路径/资源检查与Unity构建通过，[打开新版](Builds/MenuDemo/MenuDemo.exe)，按[第一幕测试步骤](docs/FIRST_ACT_ROUTE.md)验收。校园布局为可替换原创示意，以下为历史记录。

**第五幕答案交互已接入（2026-11-02）：** 题目特写加入凌赫默读/发现最大值的心声，玩家输入520后才接原讲解；答错由李晚云提醒重算，不能用继续跳过。保留连续播放与解锁存档，五幕101段。独立编译、输入/资源检查和Unity构建通过，[打开新版](Builds/MenuDemo/MenuDemo.exe)，按[交互测试步骤](docs/FIFTH_ACT_ANSWER.md)验收。以下为历史记录。

**连续播放与第五幕新图已更新（2026-11-02）：** 幕末继续会自动解锁并淡入相邻下一幕，不再中途返回目录；续读保存和手动选章保留。第五幕使用作者资产包五张新图，Unity实际编译与Windows构建通过，[打开新版游戏](Builds/MenuDemo/MenuDemo.exe)。未代做游玩测试。以下为历史记录。

**章节目录、解锁与续读已接入（2026-11-02）：** 进入故事卡后先显示横向五幕目录；首次只开放第一幕，读完解锁下一幕，已完成可重看。自动保存到本机，重启后可继续上次那一段。独立编译、状态检查及Unity构建成功，[打开新版游戏](Builds/MenuDemo/MenuDemo.exe)，按[章节验收步骤](docs/CHAPTER_READING.md)检查。旧版没有存档，首次从第一幕开始。以下为历史记录。

**第三幕宽屏原图已更新（2026-11-02 17:41）：** 作者最新五张1672×941图片已原样接入，保留全屏画面、半透明对白框与原剧情。Unity实际导入及Windows构建成功，[打开新版游戏](Builds/MenuDemo/MenuDemo.exe)查看；未代做游玩测试。以下为历史记录。

**第三幕显示调整（2026-11-02）：** 作者新版五图改为直接拉伸铺满16:9，不裁切、不留白；全故事对白框改为55%不透明度，文字保持不透明。独立编译与Unity实际编译通过，Windows新版构建成功，详见[第三幕说明](docs/THIRD_ACT.md)。未代做游玩测试，以下为历史记录。

**第三幕作者新版图片已替换（2026-11-01）：** 五张图片已原样更新到工程，第三幕等比完整显示于对话框上方，保留18段剧情及其他四幕。独立C#编译通过；本次后台Unity启动退出1，未生成新版Windows包，现有exe仍为20:41的旧图版。请从Hub打开工程，选择Tools → Story Prototype → Build Illustrated Menu后查看。见[第三幕说明](docs/THIRD_ACT.md)。

**第五幕《这道题》已构建（2026-11-01 20:41）：** [打开 Windows 新版](Builds/MenuDemo/MenuDemo.exe)，《旁边的位置》连续五幕，共99段、23图。新增留下讲题、清晰题面与解法、520结果及次日约饭的线性演出；数学已核对，背景音乐保留。Unity实际编译与Windows构建通过，阅读验收请按[第五幕测试步骤](docs/FIFTH_ACT.md)。解题输入等互动后补，第六幕待制作。以下为历史记录。

**第四幕《你的核酸码》已构建（2026-11-01 19:59）：** [打开 Windows 新版](Builds/MenuDemo/MenuDemo.exe)，同一《旁边的位置》故事卡连续播放四幕，共72段、18图；新增教室取卡与体育馆递卡演出，保留循环背景音乐。独立编译、Unity实际编译和Windows构建通过；未代做剧情阅读测试，请按[第四幕测试步骤](docs/FOURTH_ACT.md)验收。第五幕《这道题》尚未制作，以下保留历史。

**第三幕《水杯》已构建（2026-11-01 19:24）：** [打开 Windows 新版](Builds/MenuDemo/MenuDemo.exe)，开始游戏 →《旁边的位置》→ 前两幕后连续进入晚自习第三幕。三幕共52段、13图，包含作者提供的循环背景音乐；独立编译、Unity实际编译和Windows构建成功。未代做游戏阅读或试听，按[第三幕测试步骤](docs/THIRD_ACT.md)验收。原图及实际生成提示在 art/Story/Act3/WaterCup。以下为历史记录。

**第二幕新版已构建（2026-11-01 17:41）：** [打开 Windows 新版](Builds/MenuDemo/MenuDemo.exe)，开始游戏 →《旁边的位置》→ 依次阅读两幕。Unity 实际编译和构建成功，2章 / 8图 / 34段资源检查通过；运行与清晰度由作者按[测试步骤](docs/SECOND_ACT.md)验收。下方许可证阻塞为本次成功构建之前的历史记录。

**第二幕与故事目录（2026-11-01）：** 已编写《旁边的位置》整部故事入口与第一幕→第二幕连续演出，使用作者最终四图，共34段；新增五档分辨率，默认全屏1920×1080及更高精度字体。独立脚本编译通过；Unity 后台打包被许可证阻止，当前 exe 仍是旧版。请先按[第二幕制作与测试步骤](docs/SECOND_ACT.md)从 Hub 打开工程并构建，再验收。以下保留历史记录。

**第一幕后排片段已接入：** [打开更新后的 Windows 游戏](Builds/MenuDemo/MenuDemo.exe)，依次选择“开始游戏 → 校园故事 → 进入故事”。四张后排图片配合18段对白 / 旁白，以淡入衔接到下课收书；无配音和音乐。已在本机独立程序走通、重看并返回主界面，详见 [本段说明](docs/FIRST_ACT_REAR_ROW.md)。讲台旁初次交谈尚待下一段制作。

**教室布局更新：** 已按作者确定的 8 列 × 6 行、两列并桌、晚云第 6 列第 1 行建立[统一空间基准](docs/CLASSROOM_SPACE.md)。讲台贴近第一排，凌赫的垂直桌同时紧贴讲台与晚云桌；前后门都在学生右侧墙。当前交付结构参考，第一幕手绘背景和接入验证仍在制作。

**主界面第二版：** 按钮已融入完整教室画面，设置改为独立音乐 / 音效、分辨率、全屏和简体 / 繁体 / 英语 / 日语，已移除字号选项。程序仍从下方 MenuDemo 入口打开，详见 [第二版说明](docs/ILLUSTRATED_MAIN_MENU_V2.md)。四语目前覆盖菜单，正式剧情翻译另计。

**最新可运行版本（2026-10-31）：** [打开 Windows 主界面演示](Builds/MenuDemo/MenuDemo.exe)。已加入插画主菜单、设置、退出确认，并连接数学讲题 / 英语互助小样。Unity 顶栏 **Tools → Story Prototype → Open Main Menu** 后按 Play 即可查看。游戏名使用“未定名”占位，完整主线制作中。详情与验证范围见 [主界面说明](docs/ILLUSTRATED_MAIN_MENU.md)；下方早期环境记录保留作学习历史。

目标：制作可商业发行的原创短篇单机游戏，并积累能够独立讲解的工程作品。优先 Windows / Steam，移动版在 Windows 稳定后评估。基础环境、P1-01 主菜单和 P1-02 中文线性对话已完成编辑器与 Windows 本机验收。P1-03 点击触发的基础编辑器流程已获用户确认，下一步专项验证 UI 覆盖物体时不点击穿透，再构建新版本。实际交互场景是 Assets/Scenes/SampleScene.unity；Bootstrap 是此前的静态测试场景。补全列表手测和 URP / 退出资源提示排查仍有后续记录项。

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

### 后续环境更新 · 2026-10-29

用户已独立安装 Unity 6.6（6000.6.7f1），实际路径为 `D:\zachary\Editor\Unity.exe`，已通过 Hub 的“选择位置”添加。后续已完成工程创建、编辑器点击测试和首次 Windows 构建；下方保留初始化操作顺序，最新验收结果见 PROGRESS。

## 你现在需要执行的操作

1. 从 [Unity 官方下载页](https://unity.com/download)安装 Unity Hub，登录并完成适用的许可证激活。账号步骤由你操作。
2. 使用已添加的 Unity **6.6（6000.6.7f1）**，无需重复安装。先满足 Windows 编辑器与 Windows 构建需求；当前不安装 Android / iOS 模块。首个测试构建使用 Mono；如切换 IL2CPP，再检查对应 Windows 构建模块与 C++ 工具链要求。
3. 已使用 **Universal 2D** 模板创建工程；继续使用其 URP / 2D Renderer 配置，无需重新创建或切换渲染管线。
4. 项目名称填 `UnityProject`，Location 填 `C:\Users\张俊豪\Desktop\LoveNarrativeGame`，确保最终路径为 `...\LoveNarrativeGame\UnityProject`。当前仓库已有文档，不要让 Hub 覆盖它；无需额外启用 Unity Version Control。
5. 等首次导入结束，确认 Console 无红色错误。把完整版本号与任何错误原文带回；随后共同检查 Packages/manifest.json 和 packages-lock.json，再确定官方包的实际兼容版本。
6. 在 Project Settings 中检查 Version Control 为 Visible Meta Files、Asset Serialization 为 Force Text。将 VS Code 设为 External Script Editor；后续用一个小脚本验证补全、编译与断点。
7. 在编辑器 Project 窗口创建 `Assets/_Game/Scenes`、`Scripts`、`Prefabs`。把测试场景另存为 `Assets/_Game/Scenes/Bootstrap.unity`。暂不创建尚无用途的大量目录。
8. 给场景添加一个 Square Sprite，摆到正交相机可见处，保存。进入 Play，确认 Game 视图可见方块，退出 Play 后场景仍保留，Console 无红色错误。此时只验证渲染流程，不宣称有玩法。
9. File → Build Profiles（Ctrl+Shift+B），使用已创建的 Windows 配置，Intel 64-bit、Development Build。Scene List 当前为实际含点击脚本的 `Assets/Scenes/SampleScene.unity`；不要误换成尚无点击脚本的 Bootstrap。构建输出选择仓库根目录 `Builds/Windows/`。首轮预期使用 Mono，构建后检查实际后端与日志。退出编辑器 Play 后单独启动 exe，确认显示一致；分发时必须保留构建生成的配套文件。
10. 记录实际结果到 docs/PROGRESS.md，检查 `git status`：场景及 .meta、Packages、ProjectSettings 应出现；Library、Temp、Builds 应不出现。之后才建立 Unity 工程的初始化提交。

如出现 Hub 安装、许可证、包下载、中文路径或编译错误，先记录错误及日志位置，再决定是否调整；没有证据时不迁移项目或重复安装。

## 项目导航

- [执行计划与验收](docs/ROADMAP.md)
- [进度、决策与验证记录](docs/PROGRESS.md)
- [素材来源与授权台账](docs/ASSET_LICENSES.csv)
- [C# 编辑器与断点验收](docs/DEBUGGING.md)
- [主菜单搭建与验收](docs/MAIN_MENU.md)
- [对话原型搭建与验收](docs/DIALOGUE.md)
- [点击物体触发对话](docs/INTERACTION.md)
- [长期协作规则](AGENTS.md)

后续才按实际需要新增 GDD、模块设计、DEVLOG 和 PORTFOLIO；现在的决策及工作记录集中在 PROGRESS，避免空文档堆积。项目暂未授予公共开源许可证，也未创建远程仓库。

## 官方核验入口

- [Unity 6 支持策略](https://unity.com/releases/unity-6/support)：已确认 6.3 LTS 分支存在；安装后锁定实际补丁。
- [Unity 6.3 工程配置](https://docs.unity.com/en-us/engine/6000.3/manual/get-started/project-configuration)
- [Hub 模板说明](https://docs.unity.com/en-us/hub/templates)：以本机所选版本的模板描述为准。
- [Unity 6.3 Windows 构建说明](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/windows/standalone-binaries)
