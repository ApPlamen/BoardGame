Shader "Unlit/DT_Baked"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        tint("Tint", Color) = (1,1,1)
        brightness("Brightness", Float) = 1
        contrast("Contrast", Float) = 1
        blackPoint("Black Point", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float brightness;
            float contrast;
            float blackPoint;
            fixed4 tint;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float lum = tex2D(_MainTex, i.uv);
                
                lum *= brightness;
                lum = (lum - 0.5) * contrast + 0.5;
                
                if(lum < 0) lum = 0;
                else if(lum > 1) lum = 1;
                
                lum -= blackPoint;
                lum /= (1-blackPoint);
                
                fixed4 col = lum * tint;
                             
                return col;
            }
            ENDCG
        }
    }
}
