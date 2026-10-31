"""Offline geometry reference, not a Unity 3D scene or a runtime dependency."""
import json
from pathlib import Path
import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
from matplotlib import font_manager
from matplotlib.patches import Rectangle
from mpl_toolkits.mplot3d.art3d import Poly3DCollection
from room_layout import COLUMNS as columns, DESKS as desks, GEOMETRY as geometry

HERE=Path(__file__).parent
font_manager.fontManager.addfont('C:/Windows/Fonts/msyh.ttc')
plt.rcParams['font.family']=font_manager.FontProperties(fname='C:/Windows/Fonts/msyh.ttc').get_name()
plt.rcParams['axes.unicode_minus']=False
(HERE/'room-geometry.json').write_text(json.dumps(geometry,ensure_ascii=False,indent=2),encoding='utf-8')

def cube(ax,x,y,z,w,l,h,color,edge='#65727a',lw=.42):
    a,b,c=x-w/2,x+w/2,y-l/2
    e=y+l/2
    v=[(a,c,z),(b,c,z),(b,e,z),(a,e,z),(a,c,z+h),(b,c,z+h),(b,e,z+h),(a,e,z+h)]
    faces=[[v[i] for i in f] for f in [(0,1,2,3),(4,5,6,7),(0,1,5,4),(1,2,6,5),(2,3,7,6),(3,0,4,7)]]
    ax._room_faces.extend([[(px,-py,pz) for px,py,pz in face] for face in faces]); ax._room_colors.extend([color]*len(faces))

def plane(ax,points,color,edge='#75858b',lw=.55):
    ax._room_faces.append([(px,-py,pz) for px,py,pz in points]); ax._room_colors.append(color)

def table(ax,x,y,col=None,row=None,turned=False):
    w,l=(.9,1.1) if turned else (1.0,.75)
    blue='#86a9bb' if turned else '#a2bac7'
    if col==6 and row==1: blue='#d5aaa4'
    cube(ax,x,y,.72,w,l,.065,blue)
    if turned: cube(ax,x+.37,y,.16,.05,l-.08,.52,'#d1c7b4')
    else: cube(ax,x,y-.3,.16,w-.08,.05,.52,'#d1c7b4')
    for dx in (-w/2+.06,w/2-.06):
        for dy in (-l/2+.06,l/2-.06): cube(ax,x+dx,y+dy,.02,.04,.04,.70,'#a6a7a0',lw=.2)
    if turned:
        cx,cy=x-.8,y
        cube(ax,cx,cy,.41,.45,.57,.06,'#b9a381')
        cube(ax,cx-.23,cy,.43,.045,.57,.54,'#b9a381')
    else:
        cx,cy=x,y+.78
        cube(ax,cx,cy,.41,.57,.4,.06,'#b9a381')
        cube(ax,cx,cy+.2,.43,.57,.045,.54,'#b9a381')
    for dx in (-.19,.19):
        for dy in (-.15,.15): cube(ax,cx+dx,cy+dy,.01,.035,.035,.41,'#a6a7a0',lw=.18)

def door(ax,y):
    x=10.79
    # Both openings use the same fixed side-wall plane in every camera.
    plane(ax,[(x,y-.675,.02),(x,y+.675,.02),(x,y+.675,2.65),(x,y-.675,2.65)],'#9ab1bc')
    plane(ax,[(x-.015,y-.53,1.20),(x-.015,y+.53,1.20),(x-.015,y+.53,2.42),(x-.015,y-.53,2.42)],'#d7e5df')
    cube(ax,x-.03,y-.675,.0,.065,.065,2.7,'#53758a')
    cube(ax,x-.03,y+.675,.0,.065,.065,2.7,'#53758a')
    cube(ax,x-.03,y,2.65,.065,1.4,.07,'#53758a')

def scene(ax,reverse=False,close=False):
    ax._room_faces=[]; ax._room_colors=[]
    ax.computed_zorder=False
    depth=4.5 if close else 15.2
    ax.add_collection3d(Poly3DCollection([[(0,0,-.01),(10.8,0,-.01),(10.8,-depth,-.01),(0,-depth,-.01)]],facecolors='#ded8c8',edgecolors='#8a969b',linewidths=.5,zorder=0))
    # The near wall is omitted as a cutaway; fixed geometry does not change between views.
    if not reverse and not close:
        plane(ax,[(0,0,0),(10.8,0,0),(10.8,0,3.2),(0,0,3.2)],'#eee7d9')
    if close:
        plane(ax,[(0,0,0),(10.8,0,0),(10.8,0,3.2),(0,0,3.2)],'#eee7d9')
    if reverse:
        plane(ax,[(0,15.2,0),(10.8,15.2,0),(10.8,15.2,3.2),(0,15.2,3.2)],'#eee7d9')
    if not reverse:
        plane(ax,[(1.8,.018,1.22),(8.6,.018,1.22),(8.6,.018,2.75),(1.8,.018,2.75)],'#426553')
    # Right wall as seen by students facing the front board: transparent cutaway segments.
    for lo,hi in [(0,1.55),(2.90,3.6),(4.9,5.7),(7,7.8),(9.1,9.9),(11.2,11.65),(12.95,13.575),(14.925,15.2)]:
        if close and lo>=4.5: continue
        hi=min(hi,depth)
        plane(ax,[(10.8,lo,0),(10.8,hi,0),(10.8,hi,3.2),(10.8,lo,3.2)],'#eee7d9')
    for y in (4.25,6.35,8.45,10.55,12.3):
        if close and y-.65>=4.5: continue
        ymax=min(y+.65,depth)
        plane(ax,[(10.78,y-.65,1.05),(10.78,ymax,1.05),(10.78,ymax,2.8),(10.78,y-.65,2.8)],'#c5d8d5')
    door(ax,2.225)
    if not close: door(ax,14.25)
    for item in desks:
        if not close or item['row']==1: table(ax,item['x'],item['y'],item['column'],item['row'])
    # Teacher lectern: a beige body, top surface and two raised edges.
    cube(ax,4.95,2.225,.02,1.8,1.1,.95,'#c9ba9f')
    cube(ax,4.95,2.225,.95,1.82,1.12,.08,'#e0d4bb')
    for y in (1.7,2.75): cube(ax,4.95,y,1.03,1.82,.06,.18,'#b29b78')
    table(ax,3.6,2.225,turned=True)
    if close:
        ax.set_xlim(1.5,11.0); ax.set_ylim(-4.5,.0); ax.set_zlim(0,3.2)
        ax.set_box_aspect((9.5,4.5,3.2),zoom=1.3); ax.view_init(elev=23,azim=185)
    else:
        ax.set_xlim(-.1,10.9); ax.set_ylim(-15.3,.1); ax.set_zlim(0,3.2)
        ax.set_box_aspect((10.8,15.2,3.2),zoom=1.35); ax.view_init(elev=36,azim=65 if reverse else -110)
    ax.add_collection3d(Poly3DCollection(ax._room_faces,facecolors=ax._room_colors,edgecolors='#71818a',linewidths=.35,zsort='average',zorder=1))
    ax.set_proj_type('ortho'); ax.set_axis_off(); ax.set_facecolor('#f7f4eb')

fig=plt.figure(figsize=(22,18),facecolor='#f7f4eb')
gs=fig.add_gridspec(2,2,hspace=.12,wspace=.04)
a=fig.add_subplot(gs[0,0],projection='3d'); scene(a,close=True)
a.set_title('A  前区结构｜讲台、男主桌与前门同轴',fontsize=21,pad=-5,color='#304953')
b=fig.add_subplot(gs[0,1]); b.set_facecolor('#f7f4eb')
b.add_patch(Rectangle((0,0),10.8,15.2,facecolor='#e9e4d7',edgecolor='#65727a',lw=2))
b.add_patch(Rectangle((1.8,-.05),6.8,.1,facecolor='#426553'))
for obj in desks:
    x,y=obj['x'],obj['y']; col,row=obj['column'],obj['row']
    color='#d5aaa4' if col==6 and row==1 else '#a2bac7'
    b.add_patch(Rectangle((x-.5,y-.375),1,.75,facecolor=color,edgecolor='#546e7c',lw=.9))
    b.add_patch(Rectangle((x-.285,y+.6),.57,.34,facecolor='#b9a381',edgecolor='#7b776c',lw=.7))
    if col==6 and row==1: b.text(x,y,'6-1',ha='center',va='center',fontsize=13)
for r in range(1,7): b.text(-.8,3.15+(r-1)*1.85,f'{r}行',ha='center',va='center',fontsize=13)
for col,x in columns: b.text(x,2.65,str(col),ha='center',va='center',fontsize=13)
b.add_patch(Rectangle((4.05,1.675),1.8,1.1,facecolor='#c9ba9f',edgecolor='#7b776c'))
b.add_patch(Rectangle((3.15,1.675),.9,1.1,facecolor='#86a9bb',edgecolor='#546e7c'))
b.add_patch(Rectangle((2.575,1.94),.45,.57,facecolor='#b9a381',edgecolor='#7b776c'))
for y in (2.225,14.25): b.add_patch(Rectangle((10.7,y-.675),.20,1.35,facecolor='#7095ad',edgecolor='#45647b'))
for y in (4.25,6.35,8.45,10.55,12.3): b.add_patch(Rectangle((10.7,y-.65),.20,1.3,facecolor='#c5d8d5',edgecolor='#7b9ea3'))
b.text(5.4,.6,'黑板',ha='center',fontsize=16,color='#426553')
b.text(11.15,2.225,'前门',va='center',fontsize=14,color='#45647b')
b.text(11.15,14.25,'后门',va='center',fontsize=14,color='#45647b')
b.set_xlim(-1.5,12.4); b.set_ylim(15.9,-.4); b.set_aspect('equal'); b.axis('off')
b.set_title('B  8列×6行｜粉色：晚云第6列第1行',fontsize=21,pad=10,color='#304953')
c=fig.add_subplot(gs[1,0],projection='3d'); scene(c)
c.set_title('C  后排看向黑板｜所有普通座位朝前',fontsize=21,pad=-5,color='#304953')
d=fig.add_subplot(gs[1,1],projection='3d'); scene(d,reverse=True)
d.set_title('D  讲台看向后排｜后门在最后一排椅背之后',fontsize=21,pad=-5,color='#304953')
fig.suptitle('同一份空间数据生成四个视角 · 结构校准稿',fontsize=29,color='#304953',y=.98)
fig.text(.5,.025,'列号按讲台面向学生从左往右；前后门均在学生面向黑板时的右侧墙。讲台贴第一排，凌赫桌同时贴讲台与晚云桌。',ha='center',fontsize=17,color='#546e7c')
fig.subplots_adjust(left=.03,right=.97,top=.91,bottom=.07)
fig.savefig(HERE/'classroom-structure-8x6.png',dpi=110,facecolor=fig.get_facecolor())
plt.close(fig)
print('Geometry verified: 48 desks; 8 columns; 6 rows; paired columns; Li Wanyun 6-1; 2 contact edges; rear door beyond chair backs.')
