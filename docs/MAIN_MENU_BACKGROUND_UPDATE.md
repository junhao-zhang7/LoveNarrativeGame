# 已选主界面背景替换

后续更新：该背景已随第一幕后排片段重新打包进 MenuDemo，并在独立程序实际显示；以下保留最初仅替换资源时的检查记录。见 FIRST_ACT_REAR_ROW.md。

2026-10-31。作者指定使用本轮附件 codex-clipboard-a15aebb4-f194-4330-a9c8-8805f29725fd.png，并要求不做大量测试。

- 原样保存至 art/Menu/MainMenuBackground-selected.png；复制同一文件覆盖 UnityProject/Assets/_Game/Resources/Menu/MainMenuBackground.png。
- 保留现有 .meta、MainMenuTheme 资源引用、菜单按钮与设置。本轮没有重新生成、缩放或重绘图片。
- 仅核对文件哈希一致、尺寸为 1671×941、主题仍引用该图片 GUID。未做运行测试，未重新构建 Windows 程序。Unity 下次导入 / 进入 Play 使用新背景；现有 Builds/MenuDemo 是此前构建，尚不包含新图。

旧主背景仍保存在 art/Menu/MainMenuBackground-v1.png，各轮候选留作版本历史。本轮附件是当前主菜单背景的实际内容，不以 v6 或其他相似候选代替。
