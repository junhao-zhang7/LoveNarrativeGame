# 循环背景音乐

更新：2026-11-01 19:24第三幕构建已成功包含本音乐，导入时长223.58秒。下方“音乐版未构建”为此前历史状态；实际出声、音量及循环接缝仍待作者试听。

2026-11-01。作者提供 Desktop/11月1日.MP3（实际为音频文件），原样复制到 UnityProject/Assets/_Game/Resources/Audio/BackgroundMusic.mp3，输入与副本SHA256一致。来源已登记 ASSET_LICENSES.csv；未独立核验作品来源和商业发行权。

BackgroundMusic 在程序启动时创建一个持久播放器，AudioSource 使用2D模式、loop=true，跨场景不销毁，重复返回主菜单不会重新叠加播放器。使用现有 MenuAudioChannel 的音乐通道，响应“音乐”音量滑杆；“音效”滑杆不控制这段音乐。主菜单、故事目录、剧情、结束页之间持续播放，退出程序停止。原始音频未裁剪，未声称首尾已经无缝处理。

长音频以Streaming、Vorbis最高质量、保留采样率导入；配置依据[Unity AudioImporterSampleSettings](https://docs.unity3d.com/cn/6000.0/ScriptReference/AudioImporterSampleSettings.html)。安装器只对本音乐文件配置导入设置，构建前检查AudioClip存在且时长大于0。

## 本轮证据与限制

- 已编写循环播放、跨场景单实例、音乐音量接线与导入配置。
- 独立C#编译通过（既有CS1701工具链提示）。Unity已实际导入MP3并生成.meta，Streaming配置已从导入文件核对。
- 尝试切回编辑器打包时窗口激活超时，刷新后Unity进程仍在但没有可操作窗口；另一个后台构建提前退出1，未更新音乐版Windows构建。没有强制终止编辑器进程或启动游戏试听。
- 实际出声、跨页面连续性、完整一轮后的循环接缝尚待作者验收，不能把loop=true当作已经试听通过。

## 构建与试听

1. 从Unity Hub打开/恢复UnityProject编辑器窗口，等待导入与编译结束。若提示工程已打开，先恢复原有窗口，不要另开第二个编辑器。
2. 顶栏 Tools → Story Prototype → Build Illustrated Menu，等待新的 ILLUSTRATED_MENU_BUILD_PASS。此次还会打印 BACKGROUND_MUSIC_IMPORTED 及实际时长。
3. 打开 Builds/MenuDemo/MenuDemo.exe。进入主菜单应听到音乐；如果此前音乐音量保存为0，先在“设置 → 音乐”调高。
4. 把音乐滑杆拉到0应静音，拉回恢复；“音效”滑杆不应改变音乐音量。退出再开应保留选择。
5. 主菜单→故事选择→第一幕→第二幕→返回目录，音乐应持续、无重复叠音。让整段播放到末尾，确认再次从开头播放；若接缝有明显停顿，可后续单独制作循环首尾。

学习点：持久音源管理播放生命周期，音量通道管理偏好，两者分开后，切换页面无需重新播放音乐。练习：静音后跨页面返回，确认恢复音量时不会同时出现两份音乐。思考：为什么不能在每次打开菜单时都新建并播放一个AudioSource？
