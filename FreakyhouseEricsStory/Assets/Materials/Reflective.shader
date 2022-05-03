Shader "Retro AAA/Reflective"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Max("Smoothness", Range(1, 10)) = 5
    }
        SubShader
        {
            LOD 100
            Pass
            {
                Tags { "RenderType" = "Opaque" "Queue" = "Geometry"}

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
                    float mask : TEXCOORD1;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.mask = mul(unity_ObjectToWorld, v.vertex).y;
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    clip(i.mask);
                    return col;
                }
                ENDCG
            }

            //reflection
            Pass
            {
                Tags { "RenderType" = "Opaque" "Queue" = "Geometry"}
                Cull Front
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
                    float mask : TEXCOORD1;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _Max;

                v2f vert(appdata v)
                {
                    v2f o;
                    v.vertex = mul(unity_ObjectToWorld, v.vertex);
                    o.mask = v.vertex.y;
                    v.vertex *= float4(1, -1, 1, 1);
                    o.vertex = mul(UNITY_MATRIX_VP, v.vertex);

                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2Dlod(_MainTex, float4(i.uv, 0, clamp(3, 6, sqrt(smoothstep(0, _Max, i.mask)) * 8)));
                    clip(i.mask);

                    return col * 0.5;
                }
                ENDCG
            }
        }
}