using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    public sealed partial class StoryScreen
    {
        private RectTransform routePanel;
        private StoryRouteState routeState = new StoryRouteState();
        private TMP_Text routeFeedback, routeLength;
        private GameObject[] routeHighlights;
        private Image[] routeNodes;
        private void BuildRouteInput()
        {
            routePanel = Paper("Notebook route", playback, new Vector2(960, 540), new Vector2(1720, 840), new Color(.98f,.97f,.93f,.98f));
            Text(routePanel,new Vector2(860,790),new Vector2(1300,60),"练习本上的小道",36,true,blue);
            Text(routePanel,new Vector2(860,738),new Vector2(1500,45),"找到去食堂后门的最短路线，再指给同桌看。",25,true);
            var map = Rect("School sketch",routePanel,new Vector2(860,445),new Vector2(1480,540));
            Paper("Classroom block",map,new Vector2(710,505),new Vector2(630,66),new Color(.85f,.88f,.86f));
            Text(map,new Vector2(710,505),new Vector2(550,50),"教学楼 · 背面",24,true,blue);
            Paper("Sports field",map,new Vector2(710,140),new Vector2(670,90),new Color(.87f,.91f,.84f));
            Text(map,new Vector2(710,140),new Vector2(550,50),"操场",24,true,blue);
            routeHighlights = new GameObject[StoryRouteState.EdgeCount];
            for (int i = 0; i < StoryRouteState.EdgeCount; i++)
            {
                int a = StoryRouteState.Edge(i,0), b = StoryRouteState.Edge(i,1);
                Vector2 from = RoutePoint(a), to = RoutePoint(b);
                RouteLine(map,from,to,4,new Color(.65f,.68f,.65f));
                routeHighlights[i] = RouteLine(map,from,to,7,blue).gameObject;
                Text(map,(from+to)/2+new Vector2(0,24),new Vector2(86,35),StoryRouteState.Edge(i,2).ToString(),21,true,blue);
            }
            routeNodes = new Image[StoryRouteState.NodeCount];
            for (int i = 0; i < StoryRouteState.NodeCount; i++)
            {
                int index = i;
                Paper("Place paper",map,RoutePoint(i),new Vector2(205,70),new Color(.97f,.96f,.92f));
                var node = Button(map,RoutePoint(i),new Vector2(195,64),StoryRouteState.Name(i),() => VisitRoute(index),24);
                routeNodes[i] = node.targetGraphic as Image;
            }
            Text(map,RoutePoint(0)+new Vector2(0,58),new Vector2(180,40),"起点",21,true,blue);
            Text(map,RoutePoint(3)+new Vector2(0,58),new Vector2(180,40),"终点",21,true,blue);
            routeFeedback = Text(routePanel,new Vector2(860,145),new Vector2(1520,70),"",25,true,blue);
            Button(routePanel,new Vector2(240,65),new Vector2(190,62),"撤回一步",() => { if (!reading || paused || transitioning) return; routeState.Undo(); RefreshRoute(); },24);
            Button(routePanel,new Vector2(460,65),new Vector2(190,62),"重新画",() => { if (!reading || paused || transitioning) return; routeState.Reset(); RefreshRoute(); },24);
            routeLength = Text(routePanel,new Vector2(950,65),new Vector2(670,50),"",21,true,blue);
            Button(routePanel,new Vector2(1450,65),new Vector2(310,62),"就走这条 →",SubmitRoute,26);
            routePanel.gameObject.SetActive(false);
        }
        private static Vector2 RoutePoint(int node) => new Vector2(StoryRouteState.X(node),StoryRouteState.Y(node));
        private RectTransform RouteLine(Transform parent, Vector2 from, Vector2 to, float thickness, Color color)
        {
            var line = Rect("Pencil road",parent,(from+to)/2,new Vector2(Vector2.Distance(from,to),thickness));
            line.localRotation = Quaternion.Euler(0,0,Mathf.Atan2(to.y-from.y,to.x-from.x)*Mathf.Rad2Deg);
            var graphic = line.gameObject.AddComponent<MenuInkGraphic>(); graphic.kind = MenuInkGraphic.Shape.Stroke;
            graphic.color = color; graphic.raycastTarget = false; return line;
        }
        private void VisitRoute(int node)
        {
            if (!reading || paused || transitioning || !routePanel.gameObject.activeInHierarchy) return;
            if (!routeState.Visit(node)) { routeFeedback.text = "先沿着相连的道路走，点亮过的地点可以点回去。"; return; }
            RefreshRoute();
        }
        private void RefreshRoute()
        {
            var path = routeState.Path;
            for (int i = 0; i < routeHighlights.Length; i++)
            {
                int a = StoryRouteState.Edge(i,0), b = StoryRouteState.Edge(i,1); bool selected = false;
                for (int j = 1; j < path.Count; j++) if ((path[j-1]==a && path[j]==b) || (path[j-1]==b && path[j]==a)) selected = true;
                routeHighlights[i].SetActive(selected);
            }
            for (int i = 0; i < routeNodes.Length; i++)
            {
                bool selected = false; for (int j = 0; j < path.Count; j++) if (path[j]==i) selected = true;
                routeNodes[i].color = selected ? new Color(.45f,.65f,.73f,.3f) : Color.clear;
            }
            routeLength.text = "已画路线：" + routeState.Length + "　·　数字为示意距离";
            routeFeedback.text = routeState.AtGoal ? "已经画到食堂后门了。看看这条是不是最近，再指给同桌。" : "点相连的地点继续画，走远了可以撤回。";
        }
        private void SubmitRoute()
        {
            if (!reading || paused || transitioning || !routePanel.gameObject.activeInHierarchy) return;
            if (!routeState.Submit())
            {
                routeFeedback.text = routeState.AtGoal ? "同桌：还得绕这么一圈？看看楼后面能不能少走一点。" : "先把路线画到食堂后门，再告诉同桌。";
                return;
            }
            routePanel.gameObject.SetActive(false); dialoguePanel.gameObject.SetActive(true);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            lastAdvanceFrame = Time.frameCount; AdvanceBeat();
        }
    }
}
