# 进度与证据

## 当前状态 · 2026-10-29

当前里程碑：Phase 0，用户确认点击、再次点击还原及按住不连续切换均正常。检查发现实际交互位于 Assets/Scenes/SampleScene.unity；Bootstrap 仍为静态方块场景。已创建 Windows Build Profile，首次独立构建待完成。Unity 6.6（6000.6.7f1），每周可投入 10 小时以上。

### 本次已经完成
- 读取用户完整项目要求；检查空工作区、工具命令、常见安装路径、注册表、Hub 配置与运行进程。
- Git 2.53.0.windows.1 可用；VS Code 可用；dotnet --info 显示只有运行库，无 SDK。
- 建立长期协作规则、README 手动初始化指南、含工时/知识/交付/验收的 Phase 0 与 1 计划、Git 规则和授权台账。
- 查阅 Unity 官方版本、模板与 Windows 构建资料，入口见 README。
- 初始化 main 分支的本地 Git 仓库；本轮文档作为基线提交，不配置远程或推送。

### 验证等级
| 层级 | 状态 / 证据 |
| --- | --- |
| 文档与代码已编写 | 是；新增 ClickableSquare 输入练习脚本 |
| 静态与 Git 规则检查 | 通过：7 个代表缓存/构建路径被忽略，7 个代表源文件/.meta/包/设置路径未被忽略；README 本地链接均存在；授权台账表头为 13 列；提交前 git diff --cached --check 通过 |
| C# 编译 | ClickableSquare 已通过 Unity 编译：Logs/Editor.log 记录 Csc、ILPostProcess 和 CopyFiles Assembly-CSharp.dll；Library/ScriptAssemblies/Assembly-CSharp.dll 已更新；断点尚未验证 |
| 自动化测试 | 未执行；尚无测试代码 |
| Unity 编辑器运行 | 用户确认点击交互正常；实际保存的 SampleScene 已含 ClickableSquare 和 BoxCollider2D；已退出 Play 并保存 |
| Windows 构建生成 | 未执行 |
| 独立进程 / 实际设备测试 | 未执行 |

## 当前决策
- D001：Windows / Steam 优先，离线买断短篇，移动端后置（用户要求）。
- D002：原计划 Unity 6.3 LTS，后经讨论改选 Unity 6.6；已确认本地版本为 6000.6.7f1（b22767f04f04），官方包版本待创建工程后检查。
- D003：仓库根存协作文档，UnityProject/ 存模板生成的工程，避免 Hub 对非空仓库的处理差异。
- D004：先验证启动→场景→互动→对话→选择→两个结果→重玩闭环，不提前搭建完整游戏框架。
- D005：商业素材逐项登记；当前无外部美术、音频、字体或 AI 生成素材；不添加默认开源许可。
- D006：用户已采用 Universal 2D；实际 manifest 声明 URP 17.6.0。首轮 Windows 构建建议 Mono，具体设置待检查。

## 下一项：P0-05（P0-04 的 IDE 断点验证仍未完成）
已通过编辑器 Build Profiles 创建 Windows 配置，界面显示 Active、Intel 64-bit；已勾选 Development Build 并点击 Build。当前停在 Build Windows 输出文件夹对话框，需选择仓库根 Builds/Windows。独立构建后验证点击、还原、空白区域无响应、退出并重启后的初始状态。构建后再补齐 IDE 补全和断点验证，Phase 0 尚未全部完成。

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
