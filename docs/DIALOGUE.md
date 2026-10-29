# P1-02 对话原型

当前状态：以下为待用户执行的静态 UI 练习。尚未创建对话对象、编写推进代码或验证中文。文字与界面均为技术占位，不代表正式剧情或美术。

## 第一小步：底部面板和一行文字

1. 退出 Unity Play。Project 中打开 Assets/Scenes/SampleScene。
2. Hierarchy 选现有 Canvas；Inspector 的 Canvas Scaler：UI Scale Mode = Scale With Screen Size，Reference Resolution = 1920 × 1080，Screen Match Mode = Match Width Or Height，Match = 0.5。保持现有 Canvas、EventSystem 和返回事件。
3. 选 Canvas，顶部 GameObject → UI (Canvas) → Panel。新 Panel 应在 Canvas 下，改名 DialoguePanel。
4. DialoguePanel 的 Rect Transform 展开 Anchors：Min X 0 / Y 0，Max X 1 / Y 0；Pivot X 0.5 / Y 0。然后设 Left 60、Right 60、Pos Y 40、Height 240。Image 的 Color 改深灰（如 #202530，Alpha 255）；Raycast Target 保持勾选。
5. 选 DialoguePanel，顶部 GameObject → UI (Canvas) → Text - TextMeshPro，生成的文字应在 DialoguePanel 下，改名 DialogueText。避免选到 3D Object 下的同名文字菜单。
6. DialogueText 的 Rect Transform 展开 Anchors：Min X 0 / Y 0，Max X 1 / Y 1；设置 Left / Right 32，Top / Bottom 24。TMP Text Input = [Prototype] Dialogue will appear here.；Font Size 32、Auto Size 关闭、Vertex Color 白色、Alignment 左上。沿用现有 TMP 默认字体，不导入新字体。
7. Ctrl+S 保存。Game 顶部当前 Free Aspect 的下拉菜单改为 16:9 比例。Play 后检查底部面板和文字可见、中间方块仍可点击、Back 仍能返回菜单；重进后面板仍在。停止 Play 后再反馈结果。

界面预期层级：Canvas 下有 BackButton 和 DialoguePanel；DialoguePanel 下有 DialogueText。当前文字保持静态，尚无 Next 按钮。

知识点：Canvas Scaler 将设计尺寸适配窗口；Anchors 决定相对父对象的定位与拉伸，Pivot 是自身定位参考；GameObject 名称和 TMP 显示文字互相独立。

练习：在 16:9 比例下改变 Game 窗口大小，观察底部留白和文字。面试问题：为什么屏幕 UI 使用 Rect Transform，而场景中的方块使用 Transform？

## 后续步骤

静态 UI 验收后再确定小规模数据配置方式，增加三句占位对话、Next 按钮、结束提示与索引边界处理。中文显示需选择有商用授权证据的中文字体并登记，不能把默认 LiberationSans 的英文显示通过当作中文通过。
