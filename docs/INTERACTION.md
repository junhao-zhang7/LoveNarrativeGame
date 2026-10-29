# P1-03 点击物体启动对话

当前状态：点击触发基础流程已获用户编辑器运行确认，保存的单个 BeginDialogue 事件与 CanvasGroup 正确。真实 UI 覆盖专项与新 Windows 构建尚未验收。P1-02 自动开始的中文对话版本已完成本机 Windows 验收，检查点 1bcde8f；它不包含本轮行为。

## 本轮方案

ClickableSquare 保留 Input System 鼠标单帧输入、UI 遮挡、相机范围和 BoxCollider2D 命中判断；命中后变色并 Invoke 一个 UnityEvent。它不直接持有对话类引用。DialoguePresenter 增加 BeginDialogue，只有有效配置且本场景尚未开始过时才显示第 0 句。运行时的 hasStarted 阻止进行中和结束后的重复启动；场景重新加载后恢复待触发状态。全部仍为技术占位交互。

面板使用 CanvasGroup，Awake 时 alpha 0、interactable false、blocksRaycasts false；开始时三项恢复。仅透明不足以阻止 UI 吞掉输入，所以可见性和点击拦截一起设置。保留面板 GameObject 激活，保证它上面的控制脚本会执行初始化。

官方核对：[UnityEvent](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/events/unityevent)、[CanvasGroup](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/canvasgroup)。采用当前工程的 Unity 6000.6 / Input System 配置，没有升级包。

## 编辑器接线

1. 打开工程，退出 Play，Project 打开 Assets/Scenes/SampleScene；等脚本导入，Console 无红错后继续。
2. Hierarchy 展开 Canvas，选 DialoguePanel。右侧如已有 Canvas Group 就保留；没有则 Add Component，搜索 Canvas Group 并添加。RequireComponent 对既有组件的自动补齐不作为前提，需明确检查。保持 Hierarchy 对象激活，也不要取消 Dialogue Presenter 组件的勾选。Canvas Group 的 Alpha / Interactable / Blocks Raycasts 会在运行时由脚本控制，编辑状态先保持默认。
3. Dialogue Presenter 的 Dialogue / Dialogue Text / Next Button 仍保留之前赋值，NextButton 的 On Click 仍是该组件的 NextLine，不需要重绑。
4. Hierarchy 选 Square；右侧 Clickable Square 组件出现 On Clicked。点击 +，拖入 Hierarchy 的 DialoguePanel，在函数下拉框选 DialoguePresenter → BeginDialogue()。只保留一个事件，拖的是场景对象，不是脚本文件。
5. Ctrl+S，File → Save Project。Play 后确认面板与 Next 初始隐藏；点击空白不显示；点击方块变色且显示中文第一句。
6. Next 到第二句，再点击方块，内容仍停留第二句；继续到结束，Next 禁用，点击方块也不重新开始。方块仍保留原有变色反馈。Back → Start 后面板重新隐藏，再点击方块从第一句开始。
7. Console 无新红错；反馈后检查保存的组件和事件。新行为尚未生成 Windows 构建，不能把 P1-02 的已验收构建当成新行为的结果。

## 后续专项验收：真实 UI 覆盖

为避免只在互不重叠的区域点击就声称验证了遮挡，本轮将物体临时移到面板下面，操作只在 Play 期间进行：

1. 保存正常场景。当前 Square 的 Transform Position Y 为 0。点击 Play，初始面板隐藏。
2. 保持运行，在 Hierarchy 选 Square，Inspector → Transform → Position 只将 Y 改为 -3.5，X / Z 保留。该位置根据当前正交相机尺寸 5 和底部面板布局选择。
3. 切回 Game，点击移到底部的方块。面板应出现并显示第一句，方块变成选中色。这验证隐藏的面板不会吞掉输入。
4. 再连续点击刚才方块中心的同一屏幕位置 3 次，不点 Next。此时该位置被显示的面板覆盖；方块颜色应保持不变，对话仍是第一句。若方块被面板颜色遮得看不清，在 Hierarchy 选 Square，检查 Sprite Renderer → Color 是否不变。比较的是方块颜色，单看对话不重置不足以证明没有穿透，因为 hasStarted 本身也会拦截重复开始。
5. 再点击 Next，确认正常推进。停止 Play，检查 Square Position Y 自动恢复 0，不将 -3.5 测试位置另存到场景。若未恢复，手工还原为原值再保存。

面板暂时盖住方块是测试布置；测试后恢复正常布局，不需要改脚本、输入模式或取消面板 Raycast Target。当前尚待用户反馈实际结果。

基础和遮挡验收后再生成 Windows 新构建，检查点击启动、快速点击、进行中不重置、结束不重启与菜单重进。当前该专项与构建均未执行。进入分支选择前先完成这些输入边界。

## 代码讲解与练习

- Awake 隐藏整个 UI 组，Start 只检查配置并准备；BeginDialogue 才读取第 0 句并显示。NextLine 额外检查 hasStarted，防止未开始就推进。
- UnityEvent 是 Inspector 中保存的连接：Square 知道发生了命中，对话组件决定怎样响应。没有引入全局事件总线或寻找对象。
- 引用无效时 Next 不可用并输出具体错误；本轮不增加存档、分支、多物体优先级或触屏适配。

动手练习：Next 到第二句后点击方块三次，预测内容和颜色的变化，然后实际观察。面试问题：为什么把 CanvasGroup.alpha 设为 0 后，还要设置 blocksRaycasts 和 interactable？

验证边界：独立 csc 编译 exit 0，随后用户确认 Unity 基础运行通过；保存事件与组件检查通过。未执行自动化测试；真实遮挡专项、新 Windows 构建仍待用户验收。
