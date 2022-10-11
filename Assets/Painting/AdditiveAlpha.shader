Shader "TNTC/AdditiveAlpha"{
    Properties{
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaBlend ("Alpha Texture", 2D) = "white" {}
    }

    SubShader{
        Tags { "RenderType"= "Opaque" }
        //Blend One One
        LOD 100

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f{
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _OffsetUV;
            sampler2D _AlphaBlend;

            v2f vert (appdata v){
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target{
				float2 uv = i.uv;
				float4 color = tex2D(_MainTex, uv);
                //float4 otherColor = tex2D(_AlphaBlend, uv);
                color.a = max(color.a, 0.5);
				return color;
            }
            ENDCG
        }
    }
}