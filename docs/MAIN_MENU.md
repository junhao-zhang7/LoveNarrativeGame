# P1-01 主菜单与场景切换

目标：MainMenu → Start → SampleScene，接着补齐返回菜单和独立程序退出。全部为技术占位 UI，尚无正式游戏名称、美术或剧情。

当前 MainMenu → Start → SampleScene 已通过用户编辑器运行验收。Scene List 顺序正确，TMP Essentials 已导入并登记来源。现在添加 Back 按钮；下方保留完整搭建步骤供复查。

## 第一轮：一个开始按钮

1. 结束断点练习：如仍暂停，先 F5 继续，在 VS Code 用 Shift+F5 断开调试，再退出 Unity Play。确认现有场景已保存。
2. Unity → File → New Scene，选择 Empty 并 Create；用 GameObject → Camera 添加一台摄像机。File → Save As 保存为 Assets/_Game/Scenes/MainMenu.unity，不覆盖已有场景。
3. 顶部 GameObject → UI (Canvas) → Button - TextMeshPro。当前 UGUI 2.6.0 的本地菜单名是 UI (Canvas)；若弹出 TMP 资源导入窗，先 Import TMP Essentials，然后继续。先不导入 Examples & Extras。实际导入后的字体和许可证需登记到素材台账；本机已完成导入和登记。
4. Hierarchy 应出现 Canvas、EventSystem、Button（以及 Button 下的文字子对象）。选 Canvas，确认 Render Mode 为 Screen Space - Overlay；Canvas Scaler 的 UI Scale Mode 设为 Scale With Screen Size，Reference Resolution 1920 × 1080，Match 0.5。
5. 将 Button 重命名为 StartButton；Rect Transform 使用中心锚点，Pos X/Y 为 0/0，Width/Height 为 280/80。选它下面的文字子对象，把文本设为 Start。临时使用英文避免引入尚未选定的中文字体。
6. 在 Project 的 Assets/_Game/Scripts 找到 SceneNavigation，把脚本拖到 Hierarchy 的 Canvas 上，确认 Inspector 出现 Scene Navigation；Gameplay Scene Name 保持 SampleScene，Menu Scene Name 保持 MainMenu。
7. 选 StartButton，在 Inspector 的 Button 组件底部 On Click() 点击 +。把 Hierarchy 中的 Canvas 对象拖到新事件的对象栏，在函数下拉框选择 SceneNavigation → StartGame()。这里拖场景对象，不是 Project 中的脚本文件。
8. 选 EventSystem，检查输入模块。若是 Standalone Input Module，点击其 Replace with InputSystemUIInputModule；已经是 Input System UI Input Module 则保留。若新模块的输入动作未分配，使用 Assign Default Actions。保持单个 EventSystem。
9. 保存 MainMenu。File → Build Profiles → Scene List，加入并勾选 MainMenu 和 SampleScene，把 MainMenu 排在第 0 项，SampleScene 第 1 项。当前 Windows Profile 使用全局 Scene List，不需要额外启用场景列表覆盖。
10. 保持 MainMenu 打开，Play → Game 点击 Start。预期进入 SampleScene，方块可点击变色，Console 无红错。先完成这一轮再做下面的扩展。

## 第二轮：退出与返回

### 当前练习：返回按钮

1. 点击 Unity 顶部 Play 退出运行，先保存现有场景。
2. 在底部 Project 窗口打开 Assets → Scenes，双击 SampleScene；左侧 Hierarchy 顶部应为 SampleScene。
3. 顶部 GameObject → UI (Canvas) → Button - TextMeshPro，自动生成 Canvas / EventSystem。在 Hierarchy 把新 Button 改名 BackButton；选中它，在右侧 Inspector 的 Rect Transform 设置 Pos X 0、Pos Y 180、Width 220、Height 60。
4. 点击 BackButton 左边小三角展开，选它下面的文字对象，在右侧 TextMeshPro - Text (UI) 的 Text Input 中填 Back。
5. 在底部 Project 的 Assets/_Game/Scripts 找到 SceneNavigation，拖到左侧 Hierarchy 的 Canvas 上。选 Canvas，确认右侧 Scene Navigation 的 Menu Scene Name 为 MainMenu。
6. 选 BackButton，在右侧 Inspector 找 Button → On Click()，点击 +；把左侧 Canvas 对象拖入事件对象栏，函数选 SceneNavigation → ReturnToMenu()。
7. 选左侧 EventSystem，右侧若为 Standalone Input Module，点击 Replace with InputSystemUIInputModule；已是 Input System UI Input Module 则保留。确认输入动作已分配，必要时 Assign Default Actions。按 Ctrl+S 保存场景。
8. 在 Project 打开 Assets/_Game/Scenes/MainMenu，Play → Game → Start → Back，连续往返 3 次。中途把方块点成选中色，再 Back / Start，确认回到初始颜色。检查 Console 无红错。

通过返回按钮验收后再添加 Quit 和构建回归：

- MainMenu 中复制 StartButton 为 QuitButton，分开摆放，把文字改为 Quit；On Click 事件改为同一 Canvas 上 SceneNavigation.QuitGame()，避免仍保留 StartGame 的事件。编辑器中只输出提示；独立 Windows 程序中才实际退出。
- 打开 SampleScene，添加一个 UGUI / TMP 按钮，文字 Back。在它的 Canvas 上挂 SceneNavigation，按钮绑定 ReturnToMenu()。正确配置 EventSystem / Input System UI 模块。保存两个场景。
- MainMenu → Start → SampleScene → Back → MainMenu，重复 3 次；检验没有重复跳转、没有 UI 点击穿透到方块、重新进入场景时方块恢复原始状态。
- 再用 Windows Profile 构建，确认启动的是 MainMenu，Start 和 Back 正常，Quit 关闭独立程序。本轮主菜单功能只有经过这些实际检查才算完成。

## 关键代码

SceneNavigation.cs 只有场景按钮入口，不是全局管理器：
- StartGame / ReturnToMenu：向同一个私有 LoadScene 方法提供目标场景名。
- CanStreamedLevelBeLoaded：目标未加入并启用在 Scene List 时输出明确错误，不继续加载。
- isLoading：防止按钮连续点击导致同一对象重复发起切换；普通场景对象随 Single 场景切换销毁，新场景从初始状态开始。
- LoadSceneAsync：异步切换场景，暂不引入加载画面、常驻对象或资源管理框架。
- QuitGame：用 UNITY_EDITOR 区分编辑器提示和独立程序退出。

练习：尝试在 Inspector 把 Gameplay Scene Name 临时改成不存在的名字，预测点击后的结果，验证后恢复 SampleScene；不要为此改业务代码。
面试问题：为什么入口场景需要放在 Scene List 第 0 项？为什么只在编辑器中打开过场景，不代表构建包含它？

## 当前验证边界

已用现有 Unity 编译响应文件中的引用程序集，对 SceneNavigation 分别定义/不定义 UNITY_EDITOR 做独立 C# 编译，两分支均成功。随后用户已通过 Unity 实际运行 StartGame；ReturnToMenu / QuitGame 尚未接线和验证。菜单场景、脚本 .meta、TMP 资源由编辑器操作生成，没有手写 GUID。编译临时文件位于忽略的 TestResults/ScriptCompile。
