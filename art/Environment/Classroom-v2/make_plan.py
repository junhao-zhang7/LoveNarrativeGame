from PIL import Image, ImageDraw, ImageFont
from pathlib import Path
import math
from room_layout import COLUMNS as columns, DESKS

im = Image.new('RGB',(1900,1480),'#f7f4eb'); d=ImageDraw.Draw(im)
def f(n): return ImageFont.truetype('C:/Windows/Fonts/msyh.ttc',n)
def text(x,y,s,n=25,c='#304953'): d.text((x,y),s,font=f(n),fill=c)
def xy(x,y): return 130+x*75, 170+y*75
def box(x,y,w,h,c,outline='#607983',line=2):
    d.rectangle((*xy(x-w/2,y-h/2),*xy(x+w/2,y+h/2)),fill=c,outline=outline,width=line)
def arrow(a,b,c='#546e7c',w=3):
    ax,ay=xy(*a); bx,by=xy(*b); t=math.atan2(by-ay,bx-ax)
    d.line((ax,ay,bx,by),fill=c,width=w)
    d.polygon([(bx,by),(bx-12*math.cos(t-.5),by-12*math.sin(t-.5)),(bx-12*math.cos(t+.5),by-12*math.sin(t+.5))],fill=c)

text(70,35,'教室空间基准 · 8列 × 6行',40)
text(70,100,'列号按站在讲台面向学生时，从左向右数；本俯视图上方是黑板。',25)
d.rectangle((*xy(0,0),*xy(10.8,15.2)),fill='#eeeadd',outline='#59696d',width=5)
box(5.2,0,6.8,.13,'#45644f'); text(390,185,'前墙 · 黑板',25)
box(4.95,2.225,1.8,1.1,'#c5ae8a'); text(460,316,'讲台',21)
box(3.6,2.225,.9,1.1,'#91afc0'); box(2.8,2.225,.42,.58,'#53758a')
arrow((2.8,2.225),(10.7,2.225),'#b67550')
text(255,264,'凌赫新座位',21,'#4e7187')
box(10.8,2.225,.16,1.35,'#e0c49f','#b67550',4)
box(10.8,14.25,.16,1.35,'#b8cedd','#52738e',4)
for y in (4.3,6.4,8.5,10.6,12.3): box(10.8,y,.16,1.15,'#bed7dd',line=3)
for col,x in columns: text(xy(x,0)[0]-10,xy(0,3.15)[1]-49,str(col),24)
for row in range(1,7):
    y=next(item['y'] for item in DESKS if item['row']==row)
    text(65,xy(0,y)[1]-18,str(row)+'行',24)
    for col,x in columns:
        color='#c5d7dd'
        if row==1 and col==6: color='#ddb4ac'
        box(x,y,1.0,.75,color)
        box(x,y+.78,.62,.34,'#b9a68d')
        if row==1 and col==6: text(xy(x,y)[0]-28,xy(x,y)[1]-14,'晚云',19,'#77544e')
    for x in (1.5,4.1,6.7,9.3): arrow((x,y+.45),(x,y-.48))
box(9.3,12.4,2.02,.79,None,'#3e6680',4)
arrow((10.6,14.25),(9.3,13.7),'#957254',4)
text(1010,310,'前门',28,'#ad6e48'); text(1010,730,'窗户',28)
text(1000,1240,'后门',28,'#4e7187')
text(1160,230,'已锁定的空间关系',31)
for i,line in enumerate([
    '普通区：8列 × 6行 = 48张课桌。',
    '两列紧贴一组，共4组，每组6行。',
    '李晚云：讲台视角第6列、第1行。',
    '她属于阵列里的原有座位，不另加一桌。',
    '凌赫换座后，桌子在讲台的教师右侧，',
    '讲台贴近第一排；凌赫桌同时贴两边。',
    '两人桌面相差90°；普通课桌均朝黑板。',
    '前门、后门：学生朝黑板时的右侧墙。',
    '后门必须在最后一排椅背之后。',
]): text(1160,295+i*64,line,25)
text(1160,965,'镜头与列号',31)
text(1160,1035,'俯视图左到右是8、7、6……1，',25)
text(1160,1080,'因为列号从对面的讲台视角计数。',25)
text(1160,1155,'蓝色：凌赫新座位；粉色：晚云。',24)
text(1160,1200,'深蓝框：凌赫原先靠后门的最后一排。',24)
text(70,1390,'前区保持已认可图片；本图锁定数量与相对位置，建筑尺寸仅作制图示意。',25)
im.save(Path(__file__).with_name('room-plan-8x6.png'))
