Shader "LoveNarrativeGame/MopCutout"
{
    Properties { [PerRendererData] _MainTex("Mop",2D)="white" {} }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR; };
            struct v2f { float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR; };
            sampler2D _MainTex;
            v2f vert(appdata v){v2f o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o;}
            fixed4 frag(v2f i):SV_Target
            {
                fixed4 c=tex2D(_MainTex,i.uv);
                float2 h=(i.uv-float2(.5,.74))/float2(.255,.265);
                float head=1-smoothstep(.92,1,dot(h,h));
                float handle=step(.482,i.uv.x)*step(i.uv.x,.518)*step(.02,i.uv.y)*step(i.uv.y,.69);
                c.a=smoothstep(.94,.98,c.a)*max(head,handle);
                return c*i.color;
            }
            ENDCG
        }
    }
}
