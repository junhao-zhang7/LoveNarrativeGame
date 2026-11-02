using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    public sealed partial class StoryScreen
    {
        private RectTransform routePanel, routeBook, routePencil;
        private CanvasGroup routeCanvas;
        private readonly StoryRouteState routeState = new StoryRouteState();
        private TMP_Text routeFeedback, routeLength, routeNotes;
        private RectTransform[] routeHighlights, routeMarks;
        private GameObject[] routeCircles;
        private float[] routeInk;
        private bool[] routeSelected;
        private Image[] routeNodes;
        private Button routeSubmit;
        private float routeOpening, routeClosing, routePulse;
        private int routeLastNode;
        private bool routeLeaving;
        private Rect routeClassroomView;
        private AudioSource routeAudio;
        private AudioClip routeScratch;
        private readonly Color pencilInk = new Color(.31f, .38f, .39f);

        // Presentation coordinates are independent of the graph's abstract distances.
        private static Vector2 RoutePoint(int node)
        {
            switch (node)
            {
                case 0: return new Vector2(90, 460);
                case 1: return new Vector2(90, 100);
                case 2: return new Vector2(690, 100);
                case 3: return new Vector2(690, 460);
                case 4: return new Vector2(265, 400);
                case 5: return new Vector2(515, 400);
                default: return new Vector2(390, 270);
            }
        }

        private void BuildRouteInput()
        {
            routePanel = Rect("Notebook close-up", playback, new Vector2(960, 540), new Vector2(1920, 1080));
            routeCanvas = routePanel.gameObject.AddComponent<CanvasGroup>();
            Fill("Classroom shade", routePanel, new Vector2(960, 540), new Vector2(1920, 1080), new Color(.12f,.18f,.22f,.60f));
            Paper("Notebook shadow", routePanel, new Vector2(970, 500), new Vector2(1705, 875), new Color(.10f,.14f,.17f,.20f));
            routeBook = Rect("Open exercise book", routePanel, new Vector2(960, 520), new Vector2(1680, 850));
            Paper("Worn blue cover", routeBook, new Vector2(840, 422), new Vector2(1680, 850), new Color(.32f,.45f,.50f));
            Paper("Page stack", routeBook, new Vector2(840, 424), new Vector2(1654, 831), new Color(.82f,.79f,.70f));
            Paper("Left ruled page", routeBook, new Vector2(430, 432), new Vector2(810, 820), new Color(.98f,.96f,.89f));
            Paper("Right ruled page", routeBook, new Vector2(1250, 432), new Vector2(810, 820), new Color(.99f,.97f,.91f));
            for (int y = 112; y < 810; y += 32)
            {
                RouteLine(routeBook, new Vector2(56,y), new Vector2(808,y), 1, new Color(.46f,.58f,.63f,.13f));
                RouteLine(routeBook, new Vector2(878,y), new Vector2(1628,y), 1, new Color(.46f,.58f,.63f,.13f));
            }
            RouteLine(routeBook, new Vector2(95,84), new Vector2(95,798), 1, new Color(.66f,.37f,.34f,.18f));
            for (int i = 0; i < 12; i++)
                Fill("Page fold", routeBook, new Vector2(834+i*2,432), new Vector2(2,810), new Color(.39f,.36f,.28f,.09f*(1-i/12f))).raycastTarget = false;
            Text(routeBook,new Vector2(440,766),new Vector2(690,48),"随手画的校园草图",29,true,pencilInk);
            Text(routeBook,new Vector2(440,712),new Vector2(690,42),"从教学楼后门，到食堂后门。",22,true,blue);
            var map = Rect("Pencil school map",routeBook,new Vector2(430,430),new Vector2(780,580));
            var field = Rect("Sketched sports field",map,new Vector2(390,142),new Vector2(280,92));
            var fieldInk = field.gameObject.AddComponent<MenuInkGraphic>(); fieldInk.kind = MenuInkGraphic.Shape.Box;
            fieldInk.color = new Color(.43f,.52f,.43f,.35f); fieldInk.raycastTarget = false;
            Text(field,new Vector2(140,46),new Vector2(220,40),"操 场",24,true,new Color(.43f,.52f,.43f,.65f));
            routeHighlights = new RectTransform[StoryRouteState.EdgeCount];
            routeInk = new float[routeHighlights.Length]; routeSelected = new bool[routeHighlights.Length];
            for (int i = 0; i < routeHighlights.Length; i++)
            {
                Vector2 from = RoutePoint(StoryRouteState.Edge(i,0)), to = RoutePoint(StoryRouteState.Edge(i,1));
                RouteLine(map,from,to,2,new Color(.45f,.47f,.43f,.28f));
                routeHighlights[i] = RouteLine(map,from,to,4,pencilInk);
                Text(map,(from+to)/2+RouteDistanceOffset(i),new Vector2(66,30),StoryRouteState.Edge(i,2).ToString(),19,true,blue);
            }
            routeNodes = new Image[StoryRouteState.NodeCount]; routeMarks = new RectTransform[routeNodes.Length];
            routeCircles = new GameObject[routeNodes.Length];
            for (int i = 0; i < routeNodes.Length; i++)
            {
                int index = i; Vector2 point = RoutePoint(i);
                Paper("Label paper",map,point+new Vector2(0,-38),new Vector2(150,36),new Color(.98f,.96f,.90f));
                var circle = Rect("Circled place",map,point+new Vector2(0,3),new Vector2(66,54));
                var circleInk=circle.gameObject.AddComponent<MenuInkGraphic>(); circleInk.kind=MenuInkGraphic.Shape.Dot;
                circleInk.color=new Color(.36f,.49f,.52f,.12f); circleInk.raycastTarget=false;
                routeCircles[i]=circle.gameObject;
                routeMarks[i] = Rect("Pencil landmark",map,point,new Vector2(44,40));
                DrawRouteLandmark(routeMarks[i],i);
                var node = Button(map,point+new Vector2(0,-13),new Vector2(158,98),"",() => VisitRoute(index),20);
                routeNodes[i] = node.targetGraphic as Image;
                Text(node.transform,new Vector2(79,24),new Vector2(154,36),StoryRouteState.Name(i),20,true,pencilInk);
            }
            Text(map,RoutePoint(0)+new Vector2(0,52),new Vector2(150,30),"从这里出发",18,true,blue);
            Text(map,RoutePoint(3)+new Vector2(0,52),new Vector2(150,30),"到这里",18,true,blue);
            Text(routeBook,new Vector2(1250,766),new Vector2(650,54),"去食堂的另一条路",32,true,pencilInk);
            Text(routeBook,new Vector2(1250,703),new Vector2(650,46),"试着画出去食堂后门最近的那条路。",22,true,blue);
            routeNotes = Text(routeBook,new Vector2(1250,495),new Vector2(590,300),"",26,false,pencilInk);
            routeNotes.alignment = TextAlignmentOptions.TopLeft;
            routeLength = Text(routeBook,new Vector2(1250,325),new Vector2(620,44),"",21,true,blue);
            routeFeedback = Text(routeBook,new Vector2(1250,233),new Vector2(630,110),"",25,true,pencilInk);
            Button(routeBook,new Vector2(970,115),new Vector2(170,66),"擦掉一段",UndoRoute,23);
            Button(routeBook,new Vector2(1160,115),new Vector2(160,66),"重新画",ResetRoute,23);
            routeSubmit = Button(routeBook,new Vector2(1430,115),new Vector2(290,66),"指给同桌看 →",SubmitRoute,25);
            Text(routeBook,new Vector2(430,100),new Vector2(660,46),"点相连地点画线 · 点回旧地点可撤回",21,true,blue);
            Text(routeBook,new Vector2(1250,47),new Vector2(650,32),"草图数字仅表示远近",17,true,new Color(.40f,.44f,.42f,.7f));
            routePencil = Rect("Moving pencil tip",map,Vector2.zero,new Vector2(150,20)); routePencil.pivot = Vector2.zero;
            RouteLine(routePencil,new Vector2(-135,9),new Vector2(-22,9),10,new Color(.60f,.45f,.26f));
            RouteLine(routePencil,new Vector2(-132,12),new Vector2(-26,12),2,new Color(.85f,.72f,.46f));
            RouteLine(routePencil,new Vector2(-22,9),new Vector2(-2,1),7,new Color(.81f,.70f,.51f));
            RouteLine(routePencil,new Vector2(-6,3),Vector2.zero,3,pencilInk);
            routeAudio = gameObject.AddComponent<AudioSource>(); routeAudio.playOnAwake = false;
            routeAudio.spatialBlend = 0; routeAudio.loop = false;
            routePanel.gameObject.SetActive(false);
        }

        private void DrawRouteLandmark(RectTransform parent, int node)
        {
            if (node == 5)
            {
                for (int i = 0; i < 3; i++)
                {
                    float x = 10+i*12;
                    RouteLine(parent,new Vector2(x,4),new Vector2(x,21+i%2*5),2,pencilInk);
                    var flower = Rect("Flower",parent,new Vector2(x,26+i%2*5),new Vector2(9,9));
                    var dot = flower.gameObject.AddComponent<MenuInkGraphic>(); dot.kind = MenuInkGraphic.Shape.Dot;
                    dot.color = new Color(.63f,.49f,.38f); dot.raycastTarget = false;
                }
                RouteLine(parent,new Vector2(0,3),new Vector2(44,3),2,pencilInk);
            }
            else if (node == 4)
            {
                RouteLine(parent,new Vector2(5,34),new Vector2(5,8),3,pencilInk);
                RouteLine(parent,new Vector2(5,8),new Vector2(40,8),3,pencilInk);
            }
            else if (node == 1 || node == 6)
            {
                RouteLine(parent,new Vector2(0,8),new Vector2(44,8),2,pencilInk);
                RouteLine(parent,new Vector2(7,18),new Vector2(37,18),2,pencilInk);
                RouteLine(parent,new Vector2(14,28),new Vector2(30,28),2,pencilInk);
            }
            else
            {
                RouteLine(parent,new Vector2(3,10),new Vector2(3,30),2,pencilInk);
                RouteLine(parent,new Vector2(3,30),new Vector2(22,39),2,pencilInk);
                RouteLine(parent,new Vector2(22,39),new Vector2(41,30),2,pencilInk);
                RouteLine(parent,new Vector2(41,30),new Vector2(41,10),2,pencilInk);
                RouteLine(parent,new Vector2(3,10),new Vector2(41,10),2,pencilInk);
                RouteLine(parent,new Vector2(18,10),new Vector2(18,24),2,pencilInk);
                RouteLine(parent,new Vector2(18,24),new Vector2(27,24),2,pencilInk);
                RouteLine(parent,new Vector2(27,24),new Vector2(27,10),2,pencilInk);
            }
        }

        private RectTransform RouteLine(Transform parent, Vector2 from, Vector2 to, float thickness, Color color)
        {
            var line = Rect("Pencil stroke",parent,from,new Vector2(Vector2.Distance(from,to),thickness));
            line.pivot = new Vector2(0,.5f);
            line.localRotation = Quaternion.Euler(0,0,Mathf.Atan2(to.y-from.y,to.x-from.x)*Mathf.Rad2Deg);
            var graphic = line.gameObject.AddComponent<MenuInkGraphic>(); graphic.kind = MenuInkGraphic.Shape.Stroke;
            graphic.color = color; graphic.raycastTarget = false; return line;
        }

        private static Vector2 RouteDistanceOffset(int edge)
        {
            // Keep every road weight clear of landmark labels, including the two diagonals.
            switch (edge)
            {
                case 0: return new Vector2(26,0);
                case 1: return new Vector2(0,-28);
                case 2: return new Vector2(-26,0);
                case 3: return new Vector2(4,30);
                case 4: return new Vector2(0,25);
                case 5: return new Vector2(-4,30);
                case 6: return new Vector2(-34,-20);
                case 7: return new Vector2(35,0);
                default: return new Vector2(34,-20);
            }
        }

        private bool RouteReady => reading && !paused && !transitioning && !routeLeaving && routeOpening>=1
            && routePanel.gameObject.activeInHierarchy;

        private void BeginRoute()
        {
            routeState.Reset(); routeOpening = routeClosing = routePulse = 0; routeLeaving = false;
            routeClassroomView = art.uvRect;
            cameraTarget = new Rect(routeClassroomView.x+routeClassroomView.width*.06f,
                routeClassroomView.y+routeClassroomView.height*.04f,
                routeClassroomView.width*.88f,routeClassroomView.height*.88f);
            routeCanvas.alpha = 0; routeCanvas.interactable = false;
            for (int i=0; i<routeInk.Length; i++) routeInk[i]=0;
            routeBook.localScale = Vector3.one*.96f;
            routeBook.anchoredPosition = new Vector2(960,490);
            routePencil.gameObject.SetActive(false); RefreshRoute();
            routeFeedback.text = "同桌凑近了一点，等着看他的小道。";
        }

        private void VisitRoute(int node)
        {
            if (!RouteReady) return;
            if (!routeState.Visit(node)) { routeFeedback.text = "笔尖停了停。沿着相连的路接着画。"; return; }
            routeLastNode = node; routePulse = 1; PlayRoutePencil(); RefreshRoute();
        }
        private void UndoRoute()
        {
            if (!RouteReady) return;
            routeState.Undo(); RefreshRoute();
        }
        private void ResetRoute()
        {
            if (!RouteReady) return;
            routeState.Reset(); RefreshRoute();
        }
        private void RefreshRoute()
        {
            var path = routeState.Path;
            for (int i = 0; i < routeHighlights.Length; i++)
            {
                int a = StoryRouteState.Edge(i,0), b = StoryRouteState.Edge(i,1); routeSelected[i]=false;
                for (int j = 1; j < path.Count; j++)
                    if ((path[j-1]==a && path[j]==b) || (path[j-1]==b && path[j]==a))
                    {
                        routeSelected[i]=true;
                        Vector2 from=RoutePoint(path[j-1]),to=RoutePoint(path[j]);
                        routeHighlights[i].anchoredPosition=from;
                        routeHighlights[i].localRotation=Quaternion.Euler(0,0,Mathf.Atan2(to.y-from.y,to.x-from.x)*Mathf.Rad2Deg);
                    }
            }
            string notes = "";
            for (int i=0;i<path.Count;i++) notes += (i==0 ? "起笔  " : "  ↓   ") + StoryRouteState.Name(path[i]) + "\n";
            routeNotes.text = notes;
            for (int i = 0; i < routeNodes.Length; i++)
            {
                bool selected = false; for (int j = 0; j < path.Count; j++) if (path[j]==i) selected = true;
                routeNodes[i].color = Color.clear; routeCircles[i].SetActive(selected);
            }
            routeLength.text = path.Count==1 ? "还没落下第一笔" : "纸上记下的路程：" + routeState.Length + "（示意）";
            routeFeedback.text = routeState.AtGoal ? "笔尖到了食堂后门。\n把这条小道指给同桌看看？" : "从相连的地点接着画。\n绕远了，就擦掉一段。";
            routeSubmit.interactable = routeState.AtGoal;
            routeSubmit.GetComponentInChildren<TMP_Text>().color = routeState.AtGoal ? ink : new Color(.45f,.49f,.48f,.65f);
            routeSubmit.GetComponent<MenuHoverInk>().resting = routeState.AtGoal ? ink : new Color(.45f,.49f,.48f,.65f);
        }

        // Called from the unpaused reading loop; pausing also freezes the close animation.
        private void TickRoute()
        {
            if (routePanel == null || !routePanel.gameObject.activeInHierarchy) return;
            float dt=Time.unscaledDeltaTime; routeAudio.volume=MenuPreferences.Sound;
            routeOpening=Mathf.Min(1,routeOpening+dt/.42f);
            float open=Mathf.SmoothStep(0,1,routeOpening);
            routeCanvas.alpha=open;
            routeBook.localScale=Vector3.one*Mathf.Lerp(.96f,1,open);
            routeBook.anchoredPosition=new Vector2(960,Mathf.Lerp(490,520,open));
            routeCanvas.interactable=routeOpening>=1 && !routeLeaving;
            routePencil.gameObject.SetActive(false);
            for (int i=0;i<routeInk.Length;i++)
            {
                float target=routeSelected[i]?1:0;
                routeInk[i]=Mathf.MoveTowards(routeInk[i],target,dt/(routeSelected[i]?.28f:.17f));
                routeHighlights[i].gameObject.SetActive(routeInk[i]>.001f);
                float distance=Vector2.Distance(RoutePoint(StoryRouteState.Edge(i,0)),RoutePoint(StoryRouteState.Edge(i,1)));
                routeHighlights[i].sizeDelta=new Vector2(distance*routeInk[i],4);
                if (routeSelected[i] && routeInk[i]<1)
                {
                    routePencil.gameObject.SetActive(true);
                    routePencil.anchoredPosition=routeHighlights[i].anchoredPosition+(Vector2)(routeHighlights[i].localRotation*Vector3.right)*distance*routeInk[i];
                    routePencil.localRotation=Quaternion.Euler(0,0,-135);
                }
            }
            routePulse=Mathf.Max(0,routePulse-dt*4);
            for (int i=0;i<routeMarks.Length;i++) routeMarks[i].localScale=Vector3.one*(i==routeLastNode?1+Mathf.Sin(routePulse*Mathf.PI)*.12f:1);
            if (!routePencil.gameObject.activeSelf)
            {
                routePencil.gameObject.SetActive(true);
                routePencil.anchoredPosition=new Vector2(680,15); routePencil.localRotation=Quaternion.Euler(0,0,-18);
            }
            if (!routeLeaving) return;
            routeClosing+=dt;
            float close=Mathf.SmoothStep(0,1,Mathf.Clamp01((routeClosing-.55f)/.38f));
            if (close>0) cameraTarget=routeClassroomView;
            routeCanvas.alpha=1-close;
            routeBook.localScale=Vector3.one*Mathf.Lerp(1,.96f,close);
            routeBook.anchoredPosition=new Vector2(960,Mathf.Lerp(520,490,close));
            if (routeClosing<.93f) return;
            routePanel.gameObject.SetActive(false); dialoguePanel.gameObject.SetActive(true);
            routeLeaving=false; lastAdvanceFrame=Time.frameCount; AdvanceBeat();
        }

        private void SubmitRoute()
        {
            if (!RouteReady) return;
            if (!routeState.Submit())
            {
                routeFeedback.text = routeState.AtGoal ? "同桌：还得绕这么一圈？\n看看楼后面能不能少走一点。" : "先画到食堂后门。";
                return;
            }
            routeLeaving=true; routeClosing=0; routeCanvas.interactable=false;
            routeFeedback.text="凌赫把练习本朝同桌推近了一点。";
            if (EventSystem.current!=null) EventSystem.current.SetSelectedGameObject(null);
        }

        private void PlayRoutePencil()
        {
            if (routeScratch==null)
            {
                const int count=3969,rate=22050; var samples=new float[count];
                var noise=new System.Random(17); float smooth=0;
                for (int i=0;i<count;i++)
                {
                    float sample=(float)noise.NextDouble()*2-1; smooth=Mathf.Lerp(smooth,sample,.23f);
                    float envelope=Mathf.Sin(Mathf.PI*i/(count-1));
                    samples[i]=(sample-smooth)*envelope*.07f;
                }
                routeScratch=AudioClip.Create("Soft pencil on paper (original synthesis)",count,1,rate,false);
                routeScratch.SetData(samples,0);
            }
            routeAudio.volume=MenuPreferences.Sound; routeAudio.PlayOneShot(routeScratch);
        }
    }
}
