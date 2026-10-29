# 进度与证据

## 当前状态 · 2026-10-29

当前里程碑：P1-01 已完成 Windows 本机验收；P1-02 三句英文对话已获用户编辑器运行确认，保存的场景与引用亦检查通过。已准备 Noto 中文字体与许可证，下一步生成 TMP SDF 并验证中文。带对话的 Windows 构建尚未生成。Unity 6.6（6000.6.7f1），每周可投入 10 小时以上。

### 本次已经完成
- 读取用户完整项目要求；检查空工作区、工具命令、常见安装路径、注册表、Hub 配置与运行进程。
- Git 2.53.0.windows.1 可用；VS Code 可用；dotnet --info 显示只有运行库，无 SDK。
- 建立长期协作规则、README 手动初始化指南、含工时/知识/交付/验收的 Phase 0 与 1 计划、Git 规则和授权台账。
- 查阅 Unity 官方版本、模板与 Windows 构建资料，入口见 README。
- 初始化 main 分支的本地 Git 仓库；本轮文档作为基线提交，不配置远程或推送。

### 验证等级
| 层级 | 状态 / 证据 |
| --- | --- |
| 文档与代码已编写 | 点击、菜单和线性对话均已编写接线；中文字体已下载，TMP 资源待生成 |
| 静态与 Git 规则检查 | 通过：7 个代表缓存/构建路径被忽略，7 个代表源文件/.meta/包/设置路径未被忽略；README 本地链接均存在；授权台账表头为 13 列；提交前 git diff --cached --check 通过 |
| C# 编译 | 点击、菜单、对话已有 Unity 运行证据；新脚本导入日志、Assembly-CSharp.dll 更新时间与用户运行反馈支持对话编译成功。独立 csc 检查亦成功 |
| 自动化测试 | 未执行；尚无测试代码 |
| Unity 编辑器运行 | 点击、断点、菜单已有验收；用户反馈英文对话推进、结束禁用、快速点击及重新进入检查完成。保存场景引用正确；中文待验证 |
| Windows 构建生成 | 首次方块构建成功；新版菜单构建亦成功：UnityProject/Logs/Editor.log 记录 21:02:41–21:03:16，Succeeded，34428 ms。Windows 64 位 Development Build，输出包含 MonoBleedingEdge |
| 独立进程 / 实际设备测试 | 用户确认新版菜单程序的三次往返、颜色重置、退出与重新启动均正常。其他电脑及移动设备未测试；URP / 退出资源提示仍保留排查项 |

## 当前决策
- D001：Windows / Steam 优先，离线买断短篇，移动端后置（用户要求）。
- D002：原计划 Unity 6.3 LTS，后经讨论改选 Unity 6.6；已确认本地版本为 6000.6.7f1（b22767f04f04），官方包版本待创建工程后检查。
- D003：仓库根存协作文档，UnityProject/ 存模板生成的工程，避免 Hub 对非空仓库的处理差异。
- D004：先验证启动→场景→互动→对话→选择→两个结果→重玩闭环，不提前搭建完整游戏框架。
- D005：商业素材逐项登记；已导入 TMP 资源 / Liberation Sans，并下载 Noto 中文占位字体，均有单独授权记录。暂无外部美术、音频或 AI 生成素材；不为整个项目添加默认开源许可。
- D006：用户已采用 Universal 2D；实际 manifest 声明 URP 17.6.0。首轮 Windows 构建建议 Mono，具体设置待检查。
- D007：用户已选择 ScriptableObject 保存小规模对话文本。索引和结束状态放在场景组件中，不写共享内容资源。

## 下一项：P1-02c 中文字体与显示
经编辑器使用已下载的 NotoSansCJKsc-Regular.otf 创建 TMP SDF 资源，Dynamic 模式、保留 Source Font File、开启 Multi Atlas Textures，赋给 DialogueText。将数据资源三句替换为明确标注的中文测试文字，检查标点、缺字警告、推进和重进。详见 docs/DIALOGUE.md。中文编辑器和独立构建均尚未验收。

### 线性对话运行验收与中文准备 · 2026-10-29
- 用户在完整操作和验收步骤后反馈“已完成，进行下一步”，记录三句英文对话推进、结束禁用、快速点击及返回再进入为用户完成的编辑器检查；不是自动化测试。
- 保存场景确认 DialogueText 已在 DialoguePanel 下且 Raycast Target 为 0；DialoguePresenter 三个引用齐全，NextButton 只有一个 NextLine 事件。PrototypeDialogue.asset 保存三句占位文字，数据与脚本 .meta 由 Unity 生成。
- 新脚本导入日志存在，Assembly-CSharp.dll 更新于 22:30:15；结合用户运行结果记录为 Unity 编译并运行通过，不再仅为独立 csc 检查。
- 从 Noto 官方仓库下载 Regular 2.004，大小 16,437,364 字节；检查 OTTO 文件头、内嵌版权 / OFL 信息和 SHA256，证据存 Fonts/SOURCE.md。先登记素材，再下载；官方 LICENSE 原样保留。GitHub API 限流，未获得仓库提交号，因此只记录下载 URL、版本和哈希。
- 字体先作为中文技术占位，不作正式美术决策。单个约 16.4 MB 原字体计划用普通 Git，暂不整体启用 LFS，后续依据大资源与历史增长再评估。Unity 已生成原字体及许可证 .meta，原字体 includeFontData 为 1；SOURCE.md.meta 与 TMP SDF 资源尚未生成，字体将在后续资源齐全时一起提交。
- 带对话的新 Windows 构建尚未生成，中文运行与构建包含情况仍待验收；先为已验收的英文对话及完整 .meta 保留本地 Git 检查点。

### 线性对话代码准备 · 2026-10-29
- 用户反馈静态 UI 修复“已完成”；重新检查磁盘 SampleScene 仍为文字与面板同级且 Raycast Target 为 1，提示先停止运行并保存，不能把运行中完成扩大成持久配置已修复。
- 用户明确选 ScriptableObject。新增 DialogueSequence.cs（默认三句技术占位）和 DialoguePresenter.cs（引用校验、初始显示、边界检查、结束禁用）。未自动改场景、未手写 GUID、未创建伪造 .asset。
- 查阅 Unity 6000.6 官方 ScriptableObject / CreateAssetMenu 文档；用本机 Unity 的引用程序集与 csc 做独立编译，exit 0。三个 Inspector 引用有预期 CS0649 提示，需通过编辑器赋值；不声称 Unity 已导入或运行。
- 编译产物在忽略的 TestResults/ScriptCompile/Dialogue.dll；无自动化测试、无新对话构建。Unity 尚未为新脚本生成 .meta，本轮暂不建立不完整的脚本 Git 检查点，待导入后一起保存。

### 对话文字遮挡点击排查 · 2026-10-29
- 用户报告点击无反应，截图文字位于屏幕左上角。保存场景确认 DialogueText 的父对象是 Canvas（与 DialoguePanel 同级），Min (0, 0) / Max (1, 1)，边距 32 / 24，Raycast Target 为 1；因此其矩形覆盖几乎全屏。
- ClickableSquare 在 IsPointerOverGameObject 时跳过点击；覆盖其上的文字也会阻止底层 BackButton 接收事件。定位为层级及 UI 射线配置问题，尚未修改脚本或用户场景。
- 指导退出 Play 后将文字拖入 DialoguePanel，重新填写边距并关闭文字的 Raycast Target，保持面板的 Raycast Target；修复后方块 / Back 和文字位置待用户手测。当前占位文字没有点击推进功能。

### 对话 UI 锚点排查 · 2026-10-29
- 用户截图未显示边距字段，Anchors Min / Max 均为 (0, 1)，对应固定左上角。指导先选 DialogueText，把 Min 改为 (0, 0)、Max 改为 (1, 1)，再填写边距；面板仅水平拉伸时没有 Top / Bottom。
- 已保存 SampleScene 中存在 DialoguePanel，其水平拉伸及尺寸符合指南。截图未包含对象名称，不能据此确认当前选中对象；文字与整体 UI 运行仍待验收。本轮不自动修改用户场景。

### P1-01 Windows 菜单闭环验收 · 2026-10-29
- 用户在完整回归步骤后反馈“已完成，全部正常”，记录为用户执行的本机验收：菜单首先启动、Start 进入、方块交互、Back 返回、三次往返、再次进入时恢复原色、Quit 关闭和重新启动回到菜单均通过。
- 项目日志明确记录第二次构建 Succeeded，34428 ms；构建数据与 Assembly-CSharp.dll 更新。UnityProject.exe 启动器本身可复用，不能仅凭其旧时间戳判断未重新构建。
- Player.log 仍含 ComputeBuffer / D3D12 退出资源提示；本次功能通过不代表这些提示已修复。无自动化测试、无跨设备测试。
- 本轮仅记录验收、保留用户保存的 Back / Quit 场景改动并建立本地检查点；未修改运行时代码、未自动编辑 Unity 场景。
- 下一轮静态 UI 操作先检查 Canvas Scaler：当前 SampleScene 为 Constant Pixel Size（m_UiScaleMode 0），需按指南调整。新对话框布局尚待用户操作与验证。

### Quit 编辑器提示确认 · 2026-10-29
- 用户截图显示 Quit is available in the Windows build. Stop Play to exit this test.，确认 QuitGame 的编辑器分支已执行。
- 保存场景检查：QuitButton 位于 Pos Y -80，显示 Quit，On Click 绑定 QuitGame；StartButton 仍绑定 StartGame。Scene List 为启用的 MainMenu（0）和 SampleScene（1），Windows Profile 保持 Development Build。
- 未生成本轮新构建，未验证独立程序退出；下一步由用户通过 Unity Build Profiles 构建并手工回归。

### 返回菜单运行确认 · 2026-10-29
- 用户在修正文字并执行返回测试后反馈“已完成，进行下一步”，记录返回路径验收通过。
- 复查保存文件：TMP m_text 已变为 Back，按钮仍绑定 ReturnToMenu，Input System UI 模块保留。MainMenu 的 Start 位于 Pos Y 0，尚无 QuitButton；退出方法已存在，接线无需新增代码。
- 三次往返、方块颜色重置、独立程序退出将在新版 Windows 构建中集中回归；当前不扩大用户反馈为这些测试均已通过。

### 返回按钮配置检查 · 2026-10-29
- 用户截图显示 Game 中按钮仍为 Button，并询问 Standalone Input Module 未找到。
- 检查已保存 SampleScene：EventSystem 使用 InputSystemUIInputModule，Point 和 LeftClick 等动作引用已分配。此时无需寻找或替换旧模块。
- BackButton 的 On Click 已绑定 ReturnToMenu，Canvas 的 menuSceneName 为 MainMenu；Scene List 包含两场景，配置检查通过。
- 发现文字子对象的 GameObject 名称为 Back，TextMeshProUGUI.m_text 为 Button；重命名对象不改变显示文字。指导用户选 Canvas → BackButton → Back，在 Inspector 的 Text Input 修改为 Back。
- 本轮未修改场景；返回跳转和往返验收仍待用户实际执行，不用静态配置检查代替运行验证。

### 主菜单 Start 运行验收 · 2026-10-29
- 用户在执行 MainMenu Play / Start 跳转 / 方块交互检查后反馈“已完成，下一步”，记录该项运行验收通过。
- Scene List 已包含启用的 MainMenu（0）和 SampleScene（1）；ReturnToMenu 方法已存在，无需为返回按钮新增运行时代码。
- 返回按钮仍需用户在编辑器创建和绑定；本轮只记录结果并完善步骤，不自动修改原有交互场景。独立程序目前仍是之前的方块构建，不声称含新菜单。
- 主菜单本地检查点保留用户导入的 TMP 官方资源和 .meta；其原始 Shader / ShaderGraph 存在缩进、文件末尾空行等 whitespace 提示，保持上游内容原样。手写代码和文档按默认规则检查，其他 Unity 序列化文件仅豁免行尾空格；不为风格检查重写第三方资源。

### MainMenu 接线静态检查 · 2026-10-29
- 已保存 Assets/_Game/Scenes/MainMenu.unity 和 .meta；SceneNavigation.cs.meta 已由 Unity 生成。
- Canvas 上的 SceneNavigation 目标为 SampleScene / MainMenu；StartButton 的 On Click 持久化事件已绑定该组件的 StartGame。
- EventSystem 当前包含 InputSystemUIInputModule；文字子对象已设为 Start。
- EditorBuildSettings 的启用场景为 MainMenu（第 0 项）、SampleScene（第 1 项），与脚本目标一致。
- 用户已通过编辑器导入 TMP Essentials，包含 LiberationSans 字体和随附 OFL 文件；来源与许可证据登记到素材台账。本轮不改场景、不重新绑定事件，实际跳转尚未验证。

### 基础环境验收与进入原型阶段 · 2026-10-29
- 用户报告断点已命中并实际看到 false → true → false，记录为用户完成的调试验收；SDK 和语言服务启动已有此前日志证据。补全建议列表尚无单独反馈，后续编辑中继续观察，不虚报已手测。
- 已具备项目打开、保存场景、编辑器运行、脚本编译、断点、Git 源工程管理、Windows 构建与本机运行的证据，基础验收闭环通过。其他电脑测试、独立程序重启初始状态的单独反馈、URP 提示排查仍保留为后续回归项。
- SceneNavigation 的 Editor 与 Player 编译检查均成功，输出在 TestResults/ScriptCompile，不导入 Assets、不覆盖 Unity 的 Assembly-CSharp 程序集。没有新增自动化功能测试，菜单行为待接线后手工验收。

### SDK 安装与工程加载复验 · 2026-10-29
- 用户反馈安装完成。dotnet --list-sdks 实际返回 10.0.403 [C:\Program Files\dotnet\sdk]。
- 最新 VS Code 日志记录 C#、C# Dev Kit、Unity 扩展的激活，C# Dev Kit .NET 服务启动且 IPC 建立，UnityProject.slnx 成功打开、NuGet 恢复完成；C# 日志确认 Assembly-CSharp.csproj 由 C# Dev Kit 加载。
- 上述证据确认 SDK 与语言服务启动链路已有进展，不等同于补全已手工验证或 Unity 调试断点已命中。下一步仍为用户亲手验证第 60 行状态切换。

### 重载后的明确依赖要求 · 2026-10-29
- 用户新截图显示 C# Workspace Requirements：No .NET 10.x or later SDK installed，建议安装 SDK 10.0.403；底部显示 .slnx 无法打开和 No Solution，.NET 运行库仍在下载。
- 复查系统 dotnet --list-sdks 无输出。Microsoft .NET 10 官方下载页当前列出 SDK 10.0.403 和 Windows x64 安装程序，已核对。
- 下一步由用户点击 Resolve Requirements 安装该 SDK；如自动下载失败，使用 https://dotnet.microsoft.com/zh-cn/download/dotnet/10.0 的 SDK / Windows x64 安装程序。仅安装 Runtime 不能满足这项 SDK 要求。
- 安装后完全退出并重新打开 VS Code，从 Unity 双击脚本打开工程，检查工作区要求、解决方案加载、补全和 F5。此处仍为待执行方案，尚未确认 SDK 安装或断点通过；不修改 Unity 游戏目标框架。

### F5 调试适配器错误 · 2026-10-29
- 用户截图报 Couldn't find a debug adapter descriptor for debug type 'vstuc'。
- 读取 VS Code logs/20261029T161419/window1/exthost/exthost.log 和 C# Dev Kit.log：16:14:20，C# Dev Kit 因 C# 扩展未安装或未激活而启动失败；16:17:09 才出现 C# 扩展的激活记录；16:17:12 C# 日志仍报告依赖的 C# Dev Kit 激活失败。
- 已确认 C# 2.160.4 目录存在，launch.json 的 vstuc 配置与 Unity 扩展一致。本轮不改调试类型、不卸载扩展、不预先安装额外 SDK。
- 下一步先取消错误弹窗，在 VS Code 执行 Developer: Reload Window，让依赖按完整状态重新激活，然后重试 F5。该修复尚待用户执行和验证；若仍失败，再读取重载后新日志。

### IDE 检查记录 · 2026-10-29
- 已确认恢复的 Code.exe 和生成的工程/调试配置；配置存在不代表调试已命中。用户补全与断点反馈仍待获取。
- 本轮 CLI 列出扩展时受到沙箱对 VS Code 安装目录切换和本地日志目录写入的限制，但返回了扩展清单；不能把该 CLI 权限错误当作用户桌面 VS Code 启动失败。
- .csproj、.slnx、项目 .vscode 配置已确认被 Git 忽略。本轮不改用户正在使用的 IDE 配置，也不提交用户自行产生的 ProjectSettings.asset 差异。

## 首次 Windows 构建验收 · 2026-10-29
- 构建日志明确记录：2026-10-29 15:46:57 至 15:48:09，Succeeded，71464 ms；时间来自本次本地 Editor.log，仅作该机器此次构建记录。
- 输出：Builds/Windows/UnityProject.exe、UnityProject_Data、UnityPlayer.dll、MonoBleedingEdge 等。Windows.asset 已保存 m_Development: 1；构建不含 Script Debugging，本轮先对 Editor 做 IDE 断点验收。
- 用户确认“已完成。运行成功”，记录为用户执行的本机测试；没有把反馈扩大成跨设备、所有用例或无警告测试。
- Player.log 含 URP 后处理 shader 不受支持或被裁剪的提示，以及退出时 ComputeBuffer / D3D12 资源释放提示。记录为待排查项；不在本轮推断根因、不随意升级或改动渲染配置。
- 构建后发现 URP 预筛选字段、全局渲染运行时列表、Input System 预载资源等序列化变化，已检查差异并保留；不把它们描述为手工性能优化。
- 输出路径已确认受 Git 忽略；本次不提交可执行文件、日志、缓存或备份构建目录。

## 点击验收与首次构建准备 · 2026-10-29
- 用户已正确解释 wasPressedThisFrame 只在按下的那一帧成立，并报告点击交互三项均正常。
- 检查场景差异发现点击脚本挂在 SampleScene，原先文档按 Bootstrap 推断，现更正；本轮保留两个场景，构建 Scene List 继续使用已验证的 SampleScene。
- 用 Ctrl+S 保存 SampleScene；通过 Build Profiles 的 Add Build Profile 建立 Assets/Settings/Build Profiles/Windows.asset，.meta 由 Unity 生成。
- 已创建仓库根 Builds/Windows 目录供构建输出，目录受 .gitignore 排除。
- 系统文件夹对话框未能通过自动操作可靠聚焦/填入路径，已请求用户完成路径选择。尚无构建成功证据，不标记为已生成可执行文件。

## 首个输入脚本 · 2026-10-29
- 文件：Assets/_Game/Scripts/ClickableSquare.cs；命名空间 LoveNarrativeGame.Prototype。
- 项目 activeInputHandler 为 1，采用实际 Input System 1.20.1 的 Mouse.current；未切换旧输入系统。Awake 缓存组件和原色，Update 只在鼠标左键按下的那一帧处理，将屏幕坐标转为场景坐标，用 BoxCollider2D.OverlapPoint 判断命中并切换颜色。
- RequireComponent 在首次挂载时补齐 SpriteRenderer / BoxCollider2D；无需 Rigidbody2D。缺少 MainCamera 时记录一次错误并禁用脚本。若场景已有 EventSystem，指针位于 UI 上则不响应；这一分支尚无 UI 场景验证。
- 本轮只创建一个独立练习脚本，由 Unity 自动生成脚本和目录 .meta；未修改用户场景。此次编译结果不代表点击运行已通过。
- 此脚本用于单方块、鼠标、固定相机的入门验证；不是正式的多物体拾取、触屏或输入管理模块。Phase 1 再根据实际 UI 和交互需求设计输入入口。
- 动手练习：在 Inspector 修改 Selected Color，再预测连续两次点击后的颜色。面试问题：为什么在 Awake 缓存组件，为什么用 wasPressedThisFrame 而不是 isPressed？
- Git 检查点保留模板、用户已保存的场景、包锁、设置和 .meta；缓存及日志已确认忽略。Unity 自动序列化文件包含空字段后的行尾空格，原样保留；手写脚本与文档按默认 whitespace 规则检查，整体序列化文件检查仅豁免行尾空格。

## 工程创建检查 · 2026-10-29
- 实际目录 UnityProject/ 已存在，ProjectVersion.txt 为 6000.6.7f1（b22767f04f04），packages-lock.json 已生成。
- manifest 声明：Input System 1.20.1、UGUI 2.6.0、Test Framework 1.8.0、URP 17.6.0。此处仅记录配置，不代表功能测试已通过。
- EditorSettings.asset 的 m_SerializationMode 为 2；VersionControlSettings.asset 为 Visible Meta Files。
- 用户截图显示 SampleScene、Main Camera、Global Light 2D，确认编辑器已打开；尚未验证 Console、Play 或构建。

## 编辑器添加记录 · 2026-10-29
- 用户自行完成独立安装；检查 D:\zachary\Editor\Unity.exe 的 ProductVersion 为 6000.6.7f1_b22767f04f04。
- 在 Unity Hub 3.22.5 → 安装 → 选择位置，选中该 Unity.exe。
- 清除安装列表原有搜索词 D 后，界面实际显示 Unity 6.6（6000.6.7f1）、D:\zachary\Editor\Unity.exe、Windows 标记及 0 个项目，确认添加成功。
- 之前 Hub 下载失败日志为 connect ECONNREFUSED 127.0.0.1:65532；本次通过添加已安装编辑器完成配置，未验证该下载连接问题已经修复。
- 本次只验证文件版本和 Hub 识别，不宣称编辑器启动、C# 编译或项目构建成功。

## 模板加载检查 · 2026-10-29
- 用户截图显示“正在学习”筛选和“无法加载模板”。实际检查 Hub 的所有模板列表，已选择 Universal 2D；界面说明它使用 URP 和 2D Renderer，创建项目按钮可用。
- 编辑器安装目录内已有 com.unity.template.2d-cross-platform-2d-7.0.0.tgz；模板本体无需再次下载。
- Hub 日志中 HUB__getTemplates 请求失败，错误为 net::ERR_PROXY_CONNECTION_FAILED；另有 connect ECONNREFUSED 127.0.0.1:65532。在线模板查询仍有网络问题，本轮未修改代理或系统网络设置。
- 尚未点击创建项目；首次包解析、编辑器启动和运行仍需实际验证。6.6 中界面将 Built-In 模板标为已弃用；当前建议使用本地 Universal 2D，正式渲染选择尚待落实。

## 学习与作品集记录
本轮重点：① Hub 管理编辑器，Editor 编译并运行 Unity 工程；② Assets、Packages、ProjectSettings 是源工程，Library 等是可再生成缓存；③ Git commit 是本地快照，push 才会写入远端。
练习：完成安装后，用自己的话解释为什么 .meta 需要提交，而 Library 不需要；运行 git status，判断每个新目录是否应入库。
面试准备题：Unity 用什么保持资源引用？如果场景文件提交了但对应 .meta 丢失，可能发生什么？下一轮结合实际资源解释 GUID。
当前可记录的真实成果仅为环境诊断、仓库规范与里程碑计划；还不能在简历声称完成游戏、剧情系统、测试覆盖或性能优化。

## 后续验收记录格式
每项记录任务 ID、Editor 完整版本、提交号、检查步骤、预期、实际结果、错误/日志路径和未完成项。保存失败结果及修复过程；不要把计划中的检查写成已通过。
