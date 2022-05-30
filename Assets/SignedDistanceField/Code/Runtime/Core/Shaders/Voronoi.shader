Shader "McCore/Voronoi"
{
    Properties
    {
        _SignedDistanceFieldTexture("Texture", 2D) = "white" {}
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
                float4 screenPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            float random(in float2 coords)
            {
                return frac(sin(dot(coords, float2(19.3214234f, 33.3523434f))) * 432152.342343f);
            }

            sampler2D _SignedDistanceFieldTexture;

            float4 frag (v2f i) : SV_Target
            {
                float2 screenUv = i.screenPos.xy / i.screenPos.w;

                float2 uv = tex2D(_SignedDistanceFieldTexture, screenUv).rg;

                float rnd = saturate(random(uv * 1000.0f));
                return float4(rnd, rnd, rnd, 1.0f);
            }
            ENDCG
        }
    }
}
