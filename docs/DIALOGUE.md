# P1-02 对话原型

当前状态：用户已完成静态 UI、三句对话和中文显示的编辑器运行验收。Noto Sans CJK SC Regular 2.004 的 TMP SDF 已生成，原字体引用缺失已修复并保存；原字体、许可证、来源说明与全部 .meta 齐全。下一步 Windows 构建回归。文字与界面均为技术占位，不代表正式剧情或美术。

中文排查记录：首次运行显示方框与 missing character 警告；当时 SDF 为 Dynamic、Multi Atlas 开启，但运行时 Source Font File 为 None，来源 GUID 为空，字形表为空。停止 Play 后给 SDF 重新赋值原 .otf 并 Apply / Save Project，清除旧日志再运行；用户已反馈完成。保存文件确认来源 GUID 与运行时字体引用恢复且字形数据存在。保留上述失败与修复证据，独立构建仍需另测。

## 第一小步：底部面板和一行文字

1. 退出 Unity Play。Project 中打开 Assets/Scenes/SampleScene。
2. Hierarchy 选现有 Canvas；Inspector 的 Canvas Scaler：UI Scale Mode = Scale With Screen Size，Reference Resolution = 1920 × 1080，Screen Match Mode = Match Width Or Height，Match = 0.5。保持现有 Canvas、EventSystem 和返回事件。
3. 选 Canvas，顶部 GameObject → UI (Canvas) → Panel。新 Panel 应在 Canvas 下，改名 DialoguePanel。
4. DialoguePanel 的 Rect Transform 展开 Anchors：Min X 0 / Y 0，Max X 1 / Y 0；Pivot X 0.5 / Y 0。然后设 Left 60、Right 60、Pos Y 40、Height 240。Image 的 Color 改深灰（如 #202530，Alpha 255）；Raycast Target 保持勾选。
5. 选 DialoguePanel，顶部 GameObject → UI (Canvas) → Text - TextMeshPro，生成的文字应在 DialoguePanel 下，改名 DialogueText。避免选到 3D Object 下的同名文字菜单。
6. DialogueText 的 Rect Transform 展开 Anchors：Min X 0 / Y 0，Max X 1 / Y 1；设置 Left / Right 32，Top / Bottom 24。TMP Text Input = [Prototype] Dialogue will appear here.；Font Size 32、Auto Size 关闭、Vertex Color 白色、Alignment 左上。沿用现有 TMP 默认字体，不导入新字体。
7. Ctrl+S 保存。Game 顶部当前 Free Aspect 的下拉菜单改为 16:9 比例。Play 后检查底部面板和文字可见、中间方块仍可点击、Back 仍能返回菜单；重进后面板仍在。停止 Play 后再反馈结果。

界面预期层级：Canvas 下有 BackButton 和 DialoguePanel；DialoguePanel 下有 DialogueText。当前文字保持静态，尚无 Next 按钮。

字段显示排查：若只看到 Pos X / Pos Y / Width / Height，先修改 Anchors，再填写边距。对 DialogueText，Min 必须为 (0, 0)、Max 必须为 (1, 1)，按 Enter 确认后才会显示 Left / Right / Top / Bottom。对 DialoguePanel 只做水平拉伸，因此显示 Left / Right / Pos Y / Height，不显示 Top / Bottom。用户截图中的 Min 和 Max 同为 (0, 1)，表示固定左上角，尚未拉伸。

点击遮挡排查：保存文件发现 DialogueText 与 DialoguePanel 同属 Canvas 直接子对象；文字拉伸到几乎全屏，且 Raycast Target 勾选，会拦截 Back 和方块的指针输入。先退出 Play，把 Hierarchy 的 DialogueText 拖到 DialoguePanel 名称上，确认展开面板后文字缩进显示。改父对象后重新检查文字 Anchors Min (0, 0)、Max (1, 1)，再设 Left / Right 32、Top / Bottom 24。文字 TMP 组件底部 Extra Settings 中取消 Raycast Target；面板 Image 保持 Raycast Target 勾选，使底部对话区阻止点击穿透。保存并运行，验收文字在面板内、方块变色、Back 返回。占位文字暂未绑定任何点击推进逻辑。

知识点：Canvas Scaler 将设计尺寸适配窗口；Anchors 决定相对父对象的定位与拉伸，Pivot 是自身定位参考；GameObject 名称和 TMP 显示文字互相独立。

练习：在 16:9 比例下改变 Game 窗口大小，观察底部留白和文字。面试问题：为什么屏幕 UI 使用 Rect Transform，而场景中的方块使用 Transform？

## 第二小步：三句对话与 Next

先退出 Play；确认 DialogueText 在 DialoguePanel 下，并关闭文字的 Raycast Target；Ctrl+S。离开再重新打开 SampleScene，确认这些修改仍保留。运行模式中修改场景通常不能保留到停止后，配置应在停止状态进行。

1. 回到 Unity 等待新脚本导入，Console 无红错后继续；若 Project 尚未显示脚本，使用 Assets → Refresh。Unity 自动生成 .meta，本轮不手写。
2. Project 选 Assets/_Game，右键 Create → Folder 创建 Data。在 Data 内右键 Create → Prototype → Dialogue Sequence，命名 PrototypeDialogue。选中资源，Inspector 的 Lines 应有 3 个默认元素（如折叠则展开）；保留默认英文占位句。这里创建的是 .asset 数据，不是新 C# 脚本。
3. Hierarchy 选 DialoguePanel，顶部 GameObject → UI (Canvas) → Button - TextMeshPro，命名 NextButton，确认它缩进位于 DialoguePanel 下。Rect Transform 的 Anchors Min / Max 均为 X 1 / Y 0，Pivot 为 X 1 / Y 0；再设 Pos X -32、Pos Y 24、Width 180、Height 60。展开按钮，选文字子对象，在 TMP Text Input 填 Next。
4. 选 DialogueText，把 Rect Transform 的 Right 改为 260，其他边距保留，给右侧按钮留空间。
5. Project 中 Assets/_Game/Scripts/DialoguePresenter.cs 拖到 Hierarchy 的 DialoguePanel 上。选 DialoguePanel，在 Inspector 的 Dialogue Presenter 依次赋值：Dialogue = Project 的 Data/PrototypeDialogue 资源；Dialogue Text = Hierarchy 的 DialogueText；Next Button = Hierarchy 的 NextButton。不要把 DialogueSequence 脚本文件拖入 Dialogue 槽。
6. 选 NextButton，在 Inspector 的 Button → On Click() 点击 +，将 Hierarchy 的 DialoguePanel 拖入对象栏，函数选择 DialoguePresenter → NextLine()。保持仅一个事件。Ctrl+S。
7. Play：直接显示 [Prototype 1/3]；第一次点击 Next 显示 2/3；第二次 3/3；第三次显示 [Prototype] End of dialogue. 且 Next 变灰不可用。快速点击不应越界或报红错。
8. Back → Start 后应从第一句重新开始，方块仍能点击；停止 Play 后保存配置。反馈正常后再检查资源、引用、Unity 编译证据及 .meta，并安排中文字体。

## 实现与验证边界

- DialogueSequence : ScriptableObject：保存序列化字符串数组，只提供 LineCount 和 GetLine；运行中不写入对话资源。CreateAssetMenu 暴露上述创建菜单。参照 [Unity 6.6 ScriptableObject 文档](https://docs.unity.com/en-us/engine/6000.6/manual/scripting/get-started/fundamental-unity-types/class-scriptable-object)。
- DialoguePresenter : MonoBehaviour：Start 检查三个引用和非空数组，显示第 0 句；NextLine 先检查是否结束，再检查数组边界，推进或显示结束文字并禁用按钮。缺配置时输出明确错误并禁用组件，已分配的 Next 按钮先禁用。
- 内容资源与进度分开：索引与结束标记属于场景组件；场景重新加载会创建新组件，从第一句开始。当前进入场景自动开始，点击方块触发留到 P1-03；本轮没有分支、存档或打字机效果。
- 独立 csc 检查返回成功；CS0649 提示三个 SerializeField 引用没有在 C# 中赋值，它们需要通过 Inspector 赋值。此检查不代替 Unity 导入编译、事件接线或运行验收。
- 用户已反馈三句英文推进、结束与重新进入检查完成。尚未执行自动化测试，尚未生成带对话的新 Windows 构建。中文验收独立进行，不能把默认 LiberationSans 的英文显示通过当作中文通过。

练习：停止 Play 后修改 PrototypeDialogue 的第二句，再运行确认内容来自资源。面试问题：为什么当前句索引存放在 DialoguePresenter，而不是共享的 ScriptableObject 资源里？

## 第三小步：中文字体

已准备 Assets/_Game/Fonts/NotoSansCJKsc-Regular.otf、NotoSansCJK-OFL.txt 和 SOURCE.md，来源、版本和哈希见 SOURCE.md。字体仅为技术占位，尚未批准为正式美术选择。

1. 停止 Play，回到 Unity 等待导入；如没有 Fonts，顶部 Assets → Refresh。Project 打开 Assets/_Game/Fonts。
2. 单击 NotoSansCJKsc-Regular.otf 字体原文件，再用顶部 Assets → Create → TextMeshPro → Font Asset → SDF。当前项目 UGUI 2.6.0 的本地创建菜单确认为此路径。生成 NotoSansCJKsc-Regular SDF.asset，不通过手写 YAML 创建。
3. 选新 SDF 资源，Inspector 的 Generation Settings 中确认 Atlas Population Mode = Dynamic，Source Font File 指向原字体，勾选 Multi Atlas Textures。不要选 Dynamic OS，不能依赖开发电脑的系统字体。若提示无法加载字体，检查原字体 Import Settings 的 Include Font Data。
4. Hierarchy 展开 Canvas → DialoguePanel，选 DialogueText。在 TextMeshPro - Text (UI) → Font Asset 槽，拖入刚生成的 SDF 资源（不是 .otf）。保持字体大小 32、文字 Raycast Target 关闭。
5. Project 选 Assets/_Game/Data/PrototypeDialogue，在 Inspector 的 Lines 保持 3 个元素，依次填：[占位 1/3] 这是一段中文显示测试。；[占位 2/3] 点击 Next 查看下一句。；[占位 3/3] 再点一次，本轮对话结束。这里仅更改资源文字，不修改 DialogueSequence.cs 的默认值。
6. Ctrl+S，再 File → Save Project 保存资源设置。Play，从第一句推进到结束；确认中文、数字和全角标点正常，没有方框或 missing character 警告；快速点击不报红错，Back → Start 恢复第一句。结束提示暂时仍为英文，按钮标签暂时仍为英文。
7. 停止 Play 后确认 Font Asset 绑定仍在。当前仅记录编辑器结果，随后需要新版 Windows 构建验证字体打包和离线中文显示。

知识点：.otf 是原字体，TMP SDF.asset 是 TMP 使用的字形与图集资源；Dynamic 按实际字符添加字形，需要保留原字体引用。Multi Atlas 允许图集扩充，不代表所有中文字符都已验收。

练习：停止 Play 后把第二句改为另一句中文，观察新字能否显示。面试问题：为什么编辑器能显示中文，还要测试独立 Windows 构建？

## 第四小步：含中文对话的 Windows 构建

1. 停止 Play，Ctrl+S 保存场景，File → Save Project。关闭之前运行的独立程序。
2. File → Build Profiles，选现有 Windows，保持 Development Build；Scene List 应勾选 MainMenu（0）、SampleScene（1），无需创建新 Profile。
3. Build，选择仓库根 Builds/Windows，等待成功。当前这轮新构建尚未执行；不要运行旧版本后当成新对话验收。
4. 打开 Builds/Windows/UnityProject.exe：主菜单 → Start，确认三句中文、数字和中文标点都正常；Next 依次推进，结束提示出现且按钮变灰，快速点击无异常。
5. Back → Start，确认从第一句重置，方块仍能点击；返回菜单 Quit 关闭。重新打开 exe 再走一遍，可关闭 Unity 后运行以检查独立程序；无需断网或修改网络设置。
6. 反馈结果后核验最新构建日志、Player.log 和 Git 差异，再标记 P1-02 的构建回归。构建目录不入 Git，公开分发前还需将字体版权和许可说明随产物打包。
