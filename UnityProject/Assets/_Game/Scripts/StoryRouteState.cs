using System;
using System.Collections.Generic;

namespace LoveNarrativeGame.Prototype
{
    // A replaceable notebook sketch, not an approved plan of the story's school.
    public sealed class StoryRouteState
    {
        private static readonly string[] names = { "教学楼后门", "操场边", "食堂正门", "食堂后门", "楼后转角", "花坛缺口", "小广场" };
        private static readonly float[] xs = { 140, 140, 1340, 1340, 410, 1010, 710 };
        private static readonly float[] ys = { 455, 80, 80, 455, 395, 395, 240 };
        private static readonly int[,] edges = { {0,1,90}, {1,2,200}, {2,3,80}, {0,4,40}, {4,5,80}, {5,3,40}, {4,6,80}, {6,2,100}, {6,5,100} };
        private readonly List<int> path = new List<int> { 0 };
        public IReadOnlyList<int> Path => path.AsReadOnly();
        public bool Solved { get; private set; }
        public bool AtGoal => path[path.Count - 1] == 3;
        public static int NodeCount => names.Length;
        public static int EdgeCount => edges.GetLength(0);
        public static string Name(int i) => names[i];
        public static float X(int i) => xs[i];
        public static float Y(int i) => ys[i];
        public static int Edge(int i, int field) => edges[i, field];
        public static int Distance(int a, int b)
        {
            for (int i = 0; i < EdgeCount; i++)
                if ((edges[i,0] == a && edges[i,1] == b) || (edges[i,0] == b && edges[i,1] == a)) return edges[i,2];
            return 0;
        }
        public int Length
        {
            get { int length = 0; for (int i = 1; i < path.Count; i++) length += Distance(path[i-1],path[i]); return length; }
        }
        public static int ShortestLength
        {
            get
            {
                var best = new int[NodeCount]; var used = new bool[NodeCount];
                for (int i = 0; i < best.Length; i++) best[i] = int.MaxValue; best[0] = 0;
                for (int pass = 0; pass < NodeCount; pass++)
                {
                    int a = -1;
                    for (int i = 0; i < NodeCount; i++) if (!used[i] && (a < 0 || best[i] < best[a])) a = i;
                    if (a < 0 || best[a] == int.MaxValue) break;
                    used[a] = true;
                    for (int b = 0; b < NodeCount; b++)
                    { int cost = Distance(a,b); if (cost > 0 && best[a] + cost < best[b]) best[b] = best[a] + cost; }
                }
                return best[3];
            }
        }
        public bool Visit(int node)
        {
            if (Solved || node < 0 || node >= NodeCount || node == path[path.Count-1]) return false;
            int previous = path.IndexOf(node);
            if (previous >= 0) { path.RemoveRange(previous + 1,path.Count - previous - 1); return true; }
            if (AtGoal || Distance(path[path.Count-1],node) == 0) return false;
            path.Add(node); return true;
        }
        public void Undo() { if (!Solved && path.Count > 1) path.RemoveAt(path.Count - 1); }
        public void Reset() { path.Clear(); path.Add(0); Solved = false; }
        public bool Submit()
        {
            if (Solved || !AtGoal || Length != ShortestLength) return false;
            Solved = true; return true;
        }
    }
}
