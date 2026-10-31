using UnityEngine;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    // Native vector strokes keep paper edges and controls sharp at different resolutions.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class MenuInkGraphic : MaskableGraphic
    {
        public enum Shape { Paper, Stroke, Box, Tick, Dot, Chevron }
        public Shape kind;
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear(); var r = rectTransform.rect;
            if (kind == Shape.Paper)
            {
                const int count = 80; vh.AddVert(Vector2.zero, color, Vector2.zero);
                for (int i = 0; i <= count; i++)
                {
                    float t = i / (float)count * 4; int edge = Mathf.Min(3, (int)t); float f = t - edge;
                    Vector2 a = edge == 0 ? new Vector2(r.xMin, r.yMin) : edge == 1 ? new Vector2(r.xMax, r.yMin) : edge == 2 ? new Vector2(r.xMax, r.yMax) : new Vector2(r.xMin, r.yMax);
                    Vector2 b = edge == 0 ? new Vector2(r.xMax, r.yMin) : edge == 1 ? new Vector2(r.xMax, r.yMax) : edge == 2 ? new Vector2(r.xMin, r.yMax) : new Vector2(r.xMin, r.yMin);
                    var p = Vector2.Lerp(a, b, f) + new Vector2(Mathf.Sin(i * 5.3f), Mathf.Cos(i * 4.7f)) * 1.5f;
                    vh.AddVert(p, color, Vector2.zero); if (i > 0) vh.AddTriangle(0, i, i + 1);
                }
            }
            else if (kind == Shape.Stroke)
            { for (int i = 0; i < 12; i++) Line(vh, new Vector2(r.xMin + r.width * i / 12, Mathf.Sin(i * 1.8f) * .7f), new Vector2(r.xMin + r.width * (i + 1) / 12, Mathf.Sin((i + 1) * 1.8f) * .7f), Mathf.Max(2.5f, r.height * .48f)); }
            else if (kind == Shape.Box)
            {
                Line(vh, new Vector2(r.xMin, r.yMin + 1), new Vector2(r.xMax - 1, r.yMin), 2.2f);
                Line(vh, new Vector2(r.xMax - 1, r.yMin), new Vector2(r.xMax, r.yMax - 1), 2.2f);
                Line(vh, new Vector2(r.xMax, r.yMax - 1), new Vector2(r.xMin + 1, r.yMax), 2.2f);
                Line(vh, new Vector2(r.xMin + 1, r.yMax), new Vector2(r.xMin, r.yMin + 1), 2.2f);
            }
            else if (kind == Shape.Tick)
            { Line(vh, new Vector2(r.xMin + 3, 0), new Vector2(-2, r.yMin + 4), 4); Line(vh, new Vector2(-2, r.yMin + 4), new Vector2(r.xMax + 4, r.yMax + 3), 4); }
            else if (kind == Shape.Chevron)
            { Line(vh, new Vector2(r.xMin, r.yMax), new Vector2(0, r.yMin), 2); Line(vh, new Vector2(0, r.yMin), new Vector2(r.xMax, r.yMax), 2); }
            else
            {
                vh.AddVert(Vector2.zero, color, Vector2.zero);
                for (int i = 0; i <= 20; i++) { float a = i / 20f * Mathf.PI * 2; vh.AddVert(new Vector2(Mathf.Cos(a) * r.width / 2, Mathf.Sin(a) * r.height / 2), color, Vector2.zero); if (i > 0) vh.AddTriangle(0, i, i + 1); }
            }
        }
        private void Line(VertexHelper vh, Vector2 a, Vector2 b, float width)
        {
            int n = vh.currentVertCount; var p = new Vector2(-(b - a).y, (b - a).x).normalized * width / 2;
            vh.AddVert(a - p, color, Vector2.zero); vh.AddVert(a + p, color, Vector2.zero); vh.AddVert(b + p, color, Vector2.zero); vh.AddVert(b - p, color, Vector2.zero);
            vh.AddTriangle(n, n + 1, n + 2); vh.AddTriangle(n, n + 2, n + 3);
        }
    }
}
