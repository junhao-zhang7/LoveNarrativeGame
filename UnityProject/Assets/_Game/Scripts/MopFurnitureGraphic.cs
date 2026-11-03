using UnityEngine;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    // Sample the SAME background pixels over the moving mop. No second, misaligned furniture painting.
    public sealed class MopFurnitureGraphic : MaskableGraphic
    {
        public Texture plate;
        public override Texture mainTexture => plate != null ? plate : Texture2D.whiteTexture;
        private static readonly Vector2[][] pieces = {
            Quad(.318f,.386f,.627f,.515f), // front desktop
            Quad(.321f,.005f,.340f,.39f), Quad(.588f,.105f,.609f,.39f), // desk legs
            Quad(.354f,.005f,.374f,.348f), Quad(.535f,.005f,.555f,.35f), // chair frame
            Quad(.362f,.210f,.548f,.348f), // backrest
            Quad(.353f,.09f,.562f,.18f), Quad(.373f,.042f,.537f,.058f), // seat and brace; gaps stay open
            Quad(.28f,.31f,.55f,.70f), // lectern
            Quad(.70f,0,.999f,.75f) // foreground girl, her chair and right desk
        };
        private static Vector2[] Quad(float x,float y,float right,float top)
            =>new[]{new Vector2(x,y),new Vector2(right,y),new Vector2(right,top),new Vector2(x,top)};
        public bool Covers(Vector2 p)
        {
            foreach(var polygon in pieces)
                if(p.x>=polygon[0].x&&p.x<=polygon[2].x&&p.y>=polygon[0].y&&p.y<=polygon[2].y)return true;
            return false;
        }
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();var bounds=rectTransform.rect;
            foreach(var polygon in pieces)
            {
                int start=vh.currentVertCount;
                foreach(var uv in polygon)vh.AddVert(new Vector2(bounds.xMin+uv.x*bounds.width,bounds.yMin+uv.y*bounds.height),Color.white,uv);
                vh.AddTriangle(start,start+1,start+2);vh.AddTriangle(start,start+2,start+3);
            }
        }
    }
}
