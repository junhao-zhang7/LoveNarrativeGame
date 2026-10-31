# 统一教室多视图生成记录

2026-10-31。使用内置 image_gen。作者已认可 room-plan-8x6.png 的空间布局；新生成插画仍为审阅候选。

## 首轮：已否定

参考：classroom-structure-8x6.png（空间）、room-plan-8x6.png（数量及位置）、MainMenuBackground-v1.png（仅画风）。输出 classroom-multiview-approved-layout-v1.png。检查发现 B 只有五排，A 的前区位置漂移，不作为基准。

实际提示：

Use case: style-transfer / environment production reference.
Create a polished four-view classroom reference sheet by painting over Image 1's exact technical geometry. Image 1 (classroom-structure-8x6.png) is the IMMUTABLE spatial / count / camera reference; Image 2 (room-plan-8x6.png) is the author-approved floor plan, also authoritative. Image 3 (MainMenuBackground-v1.png) is ONLY a rendering style and materials reference, NOT a layout reference.
Output four panels in the same 2 by 2 arrangement as Image 1, with tiny labels A, B, C, D only, no long captions. Keep each panel's camera and furniture footprints. A is front-seat close view; B is exact overhead plan; C looks from rear toward blackboard; D looks from blackboard toward rear. Consistent warm, clean hand-painted Chinese high-school classroom, cream tiled lower walls, muted blue desk tops with wood edges and wooden chairs, pale olive curtains, green chalkboard, gray speckled terrazzo floor, warm natural afternoon sunlight. No people, no UI, no extra doors.
Geometry is the priority: exactly 48 normal single desks arranged in EIGHT columns and SIX rows, forming FOUR banks of two TOUCHING desks, each bank SIX rows deep. Do not separate desks within a pair. Three aisles between the four banks. Panel B must retain all 48 individual rectangles and 48 chair positions exactly; draw seams between touching desktops. No omitted rows, no five-row grid.
The pink desk in the reference is Li Wanyun's EXISTING column 6 row 1 desk (column numbers from teacher facing students); retain its exact position, use a small muted rose notebook as its marker instead of painting the entire desk pink. The extra blue sideways desk is Ling He's, chair on its left, facing right along the axis to the lectern then front door. His desk touches the end of the lectern on its right; its bottom edge touches Wanyun's desk top edge, right-angle seats, no gap. The lectern touches the front edge of the first row. Do not place a separate girl's desk outside the normal grid. Do not move the lectern away from row 1.
In B, front board is TOP; both doors are on RIGHT wall, one at front and one BEHIND all last-row chair backs. Windows between those two doors. All ordinary chairs and desks face the front board. In C the same door/window wall is screen RIGHT; in D it is screen LEFT. Never mirror or move doors to a different wall; no doors in the back wall or board wall. Preserve cutaway removed near walls where necessary to see geometry. In A the board wall is on image left, near-to-far boy chair -> sideways boy desk -> lectern -> front door, girl's normal desk immediately image-right.
Keep exact furniture count and positions, change only appearance/material/lighting into the supplied soft illustrated style. Crisp readable desk silhouettes, restrained thin outlines, no photorealistic rendering, no luxury, no collage of unrelated classrooms. Large landscape reference sheet.

## 修正：直接给结构图上色

仅参考 classroom-structure-8x6.png。输出 classroom-multiview-approved-layout-v2.png。已目视核对 B 为四组、每组六排，粉本位置为第六列第一排，讲台和男主桌靠第一排；C / D 为对应正反视图。插画不能替代坐标源作为尺寸依据，门位和遮挡在后续具体镜头仍须核对。作者尚未确认该插画。

实际提示：

对这张四视图结构图进行精细的手绘材质上色。严格保留原图四个分图的所有桌椅轮廓、数量、位置、朝向、透视和相互接触的边，禁止重新设计构图或重新摆家具。只把平涂的模型表面画成暖色校园插画：蓝色木边课桌，木制椅子，米色讲台，奶油色墙砖，灰色水磨石地面，绿色黑板，柔和阳光与浅绿窗帘。保持原图的剖切视角，不能改成另外的教室实景机位。
尤其原样保留B图四组双人并桌，每组六排，48张普通桌；原图蓝色男主桌紧贴米色讲台，男主桌另一边紧贴粉色女主桌，女主是第六列第一排，在普通桌的最前一排。讲台和男主桌就在第一排前缘，不能离第一排一张桌子的距离。粉色桌改为蓝色桌面加一小块粉色本子标记即可，不改变轮廓。
C、D图48张桌椅的位置和轮廓必须和原图一张不差，门仍在同一墙，后门在最后一排后方。A图男主座位→男主桌→讲台→前门同轴，女主桌在男主右手边直角相接。画面上方仅保留A B C D小字，不写长中文标题，其余文字删去。四宫格，精美清透手绘空间设定稿。只上色和细化，不重绘布局。
