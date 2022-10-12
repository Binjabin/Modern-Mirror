Shader "TNTC/TexturePainter"{   

    Properties{
        _PainterColor ("Painter Color", Color) = (0, 0, 0, 0)
    }

    SubShader{
        Cull Off ZWrite Off ZTest Off

        Pass{
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float3 _PainterPosition;
            float _Radius;
            float _Hardness;
            float _Strength;
            float4 _PainterColor;
            float _PrepareUV;
            

            struct appdata{
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
            };

            struct v2f{
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            float mask(float3 position, float3 center, float radius, float hardness){
                float m = distance(center, position);
                return 1 - smoothstep(radius * hardness, radius, m);    
            }

            v2f vert (appdata v){
                v2f o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
				float4 uv = float4(0, 0, 0, 1);
                uv.xy = float2(1, _ProjectionParams.x) * (v.uv.xy * float2( 2, 2) - float2(1, 1));
				o.vertex = uv; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target{   
                if(_PrepareUV > 0 ){
                    return float4(0, 0, 1, 1);
                }         

                float4 col = tex2D(_MainTex, i.uv);
                float f = mask(i.worldPos, _PainterPosition, _Radius, _Hardness);
                float edge = f * _Strength;
                float4 newCol = lerp(col, _PainterColor, edge);
                
                float4 outCol = newCol;
                outCol.a = max(newCol.a, col.a);
                float oFac = (newCol.a > 0.01);
                newCol.a = newCol.a * oFac;
                float aFac = (1 > newCol.a);
                //outCol.a = (aFac * newCol.a * 0.01) + col.a;

                //float rFac = -(col.r - newCol.r);
                //float rEqual = (abs(col.r - newCol.r) > 0.01);
                //rFac = rFac * rEqual;

                //float gFac = -(col.g - newCol.g);
                //float gEqual = (abs(col.g - newCol.g) > 0.01);
                //gFac = gFac * gEqual;

                //float bFac = -(col.b - newCol.b);
                //float bEqual = (abs(col.b - newCol.b) > 0.01);
                //bFac = bFac * bEqual;
                
                //float4 outCol  = float4(.5, .5, .5, 1);

                //outCol.a = (aFac * (0.01)) + col.a;
                //outCol.r = (rFac * (0.01 * newCol.a)) + col.r;
                //outCol.g = (gFac * (0.01 * newCol.a)) + col.g;
                //outCol.b = (bFac * (0.01 * newCol.a)) + col.b;
                return outCol;
                
            }
            ENDCG
        }
    }
}