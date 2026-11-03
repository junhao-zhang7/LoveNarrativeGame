using UnityEngine;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    // Blend the illustrated wet plate over its matching dry plate. Vertex alpha makes wiped edges soft.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class MopWetSurface : MaskableGraphic
    {
        private Texture wetPlate;
        private float[] opacity;
        private int[] nearest;
        private const int Columns=48, Rows=28;
        public override Texture mainTexture => wetPlate!=null?wetPlate:Texture2D.whiteTexture;
        public void Configure(Texture texture,Vector2[] patches,float[] alpha)
        {
            wetPlate=texture;opacity=alpha;nearest=new int[(Columns+1)*(Rows+1)];
            for(int y=0;y<=Rows;y++)for(int x=0;x<=Columns;x++)
            {
                Vector2 p=new Vector2(x*1920f/Columns,y*1080f/Rows);float best=float.MaxValue;int index=0;
                for(int i=0;i<patches.Length;i++){float d=(patches[i]-p).sqrMagnitude;if(d<best){best=d;index=i;}}
                nearest[y*(Columns+1)+x]=index;
            }
            SetAllDirty();
        }
        public void RefreshSurface()=>SetVerticesDirty();
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();if(nearest==null||opacity==null)return;var r=rectTransform.rect;
            for(int y=0;y<=Rows;y++)for(int x=0;x<=Columns;x++)
            {
                float u=x/(float)Columns,v=y/(float)Rows;
                float border=Mathf.Clamp01(Mathf.Min(Mathf.Min(u-.12f,.90f-u),Mathf.Min(v-.12f,.90f-v))/.06f);
                float a=opacity[nearest[y*(Columns+1)+x]]*border;
                vh.AddVert(new Vector2(r.xMin+u*r.width,r.yMin+v*r.height),new Color(1,1,1,a),new Vector2(u,v));
            }
            for(int y=0;y<Rows;y++)for(int x=0;x<Columns;x++)
            {int n=y*(Columns+1)+x;vh.AddTriangle(n,n+1,n+Columns+2);vh.AddTriangle(n,n+Columns+2,n+Columns+1);}
        }
    }
}
