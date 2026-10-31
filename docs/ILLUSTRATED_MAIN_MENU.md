# 插画主界面 · 2026-10-31

> 历史第一版。作者后续要求融入场景、删除字号、增加独立音乐 / 音效、分辨率与四语设置，当前以 [第二版说明](ILLUSTRATED_MAIN_MENU_V2.md) 为准。

本轮按作者要求完成打开游戏时的主界面，并接入当前互动小样。游戏名未确定，界面使用明确的“未定名”占位；没有新增剧情设定。作者确认的座位方位图为 `art/Concepts/v0.3/seating-scene-v6.png`，后续场景遵循该图。

## 打开与体验

- 直接运行仓库内 `Builds/MenuDemo/MenuDemo.exe`。必须保留同目录的数据文件和运行库，不能只复制 exe。
- Unity 中选择顶栏 **Tools → Story Prototype → Open Main Menu**，再点击顶栏三角形 Play。编辑状态仍保留原教学菜单，运行时显示新插画菜单。
- **开始游戏**进入 TableSlice 数学讲题与英语互助小样；右上角**返回主菜单**可回到新界面。完整主线尚未做成可玩版本。
- **设置**包含总音量、全屏、较大文字、恢复默认和返回。默认音量 80%、窗口模式、标准字号；选择自动保存到本机。
- **退出游戏**打开确认框；独立程序中确认即退出。Unity 中不会关闭编辑器，显示停止 Play 的提示。
- 再次生成程序：停止 Play，选择 **Tools → Story Prototype → Build Illustrated Menu**，输出到上述目录。

## 实现与取舍

`MainMenuTheme` 是保存字体和背景引用的 ScriptableObject，编辑器安装工具负责创建资源和导入引用，不手写 GUID。`MainMenuScreen` 使用 UGUI / TMP 绘制可交互按钮、滑杆、开关与弹窗；插画中没有烘焙按钮。`SceneNavigation.Awake` 在现有 MainMenu 场景安装新界面，旧 Canvas 仅在运行时隐藏，保留早期教学场景便于对照。

`MenuPreferences` 集中处理本地设置、音量边界、全屏应用和字号。字号设置同时作用于互动小样。菜单和小样随场景切换销毁重建，通过 PlayerPrefs 保留设置，不引入常驻管理器。返回菜单不会保存小样进行位置，重新开始从题目起点进入。

`MainMenuInstaller` 自动生成主题引用并补充 TableSlice 构建入口；构建显式包含 MainMenu、TableSlice、SampleScene。TableSliceBuilder 的已有场景判断改为 AssetDatabase 查询，避免编辑器工作目录不同导致重复创建。

背景由内置 image_gen 生成，原图存 `art/Menu/MainMenuBackground-v1.png`，Unity 使用 Resources 内副本；实际提示与参考关系见 `art/Menu/PROMPT.md`，登记在 ASSET_LICENSES.csv。当前背景是本地开发用插画，尚待作者审美验收。没有加入背景音乐和音效，因此本轮只验证音量数值 / 全局音量设置，不宣称完成听感验收。

## 验证证据与范围

| 层级 | 本次实际结果 |
| --- | --- |
| 已编写 / 静态检查 | 菜单、主题、偏好、安装与构建工具；检查新增脚本及现有入口差异 |
| 成功编译 | Unity 6000.6.7f1 实际导入并运行；Windows 构建脚本编译成功 |
| 自动检查 | 构建流程调用音量边界的 5 项断言，失败会中止构建；不属于 Unity Test Framework，也不是 UI 自动测试 |
| 编辑器运行 | 主菜单显示、设置弹窗、音量 80%→27%、文字放大、开始进入 TableSlice；观察 Console 0 红错 |
| 生成构建 | 编辑器 Console 输出 ILLUSTRATED_MENU_BUILD_PASS；本机生成 MenuDemo.exe 及数据目录 |
| Windows 本机实测 | 窗口 / 全屏双向切换、两种字号、设置跨场景与重启保存、开始→小样→菜单、确认退出、恢复默认；Player.log 未检出 Exception / Error / Warning |
| 尚未验证 | 工具发送 Esc 后未观察到弹窗关闭，暂不将键盘导航 / Esc 计为通过；其他显示器分辨率、其他电脑、音乐听感、完整主线均未验收 |

本轮窗口比例与全屏画面已目视检查，背景等比填充，中文无缺字；没有进行所有分辨率的穷举适配测试。构建产物、日志与缓存不提交 Git。

## 学习要点

1. UI 插画与按钮分开：美术负责氛围，真实 UI 负责点击、焦点与适配。
2. 设置与显示分开：MenuPreferences 保存值，MainMenuScreen 根据值更新文字与控件。
3. 编辑器运行和独立程序不同：退出、全屏必须在 Windows 构建中再测。

动手练习：进入设置启用较大文字，开始游戏，再返回菜单，检查两处字号是否一致。面试问题：为什么关闭设置面板后还要恢复主菜单按钮的可交互状态？

下一步：作者确认主界面视觉后，将已认可的人物与教室风格落实到第一段互动小样的场景、表情与手部动作；现有几何桌面仍是交互灰盒。
