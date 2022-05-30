Shader "McCore/SignedDistanceFieldJFA"
{
    SubShader
    {
        Tags { "RenderType" = "Transparent" }
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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            sampler2D _SignedDistanceFieldPreparationTexture;
            float4 _SignedDistanceFieldPreparationTexture_TexelSize;

            float _Offset;

            float4 frag(v2f i) : SV_Target
            {
                float2 screenUv = i.screenPos.xy / i.screenPos.w;
                float4 x[8];
                float4 x00 = tex2D(_SignedDistanceFieldPreparationTexture, screenUv);

                x[0] = tex2D(_SignedDistanceFieldPreparationTexture,
                    screenUv + float2(-_SignedDistanceFieldPreparationTexture_TexelSize.x, _SignedDistanceFieldPreparationTexture_TexelSize.y) * _Offset);
                x[1] = tex2D(_SignedDistanceFieldPreparationTexture,
                                   screenUv + float2(0.0f, _SignedDistanceFieldPreparationTexture_TexelSize.y * _Offset));
                x[2] = tex2D(_SignedDistanceFieldPreparationTexture,
                    screenUv + _Offset * _SignedDistanceFieldPreparationTexture_TexelSize.xy);
                x[3] = tex2D(_SignedDistanceFieldPreparationTexture,
                    screenUv - float2(_SignedDistanceFieldPreparationTexture_TexelSize.x * _Offset, 0.0f));

                x[4] = tex2D(_SignedDistanceFieldPreparationTexture,
                                   screenUv + _SignedDistanceFieldPreparationTexture_TexelSize.x * _Offset);
                x[5] = tex2D(_SignedDistanceFieldPreparationTexture,
                                   screenUv - _Offset * _SignedDistanceFieldPreparationTexture_TexelSize.xy);
                x[6] = tex2D(_SignedDistanceFieldPreparationTexture,
                                   screenUv - _Offset * _SignedDistanceFieldPreparationTexture_TexelSize.y);
                x[7] = tex2D(_SignedDistanceFieldPreparationTexture,
                                   screenUv + _Offset * float2(_SignedDistanceFieldPreparationTexture_TexelSize.x, -_SignedDistanceFieldPreparationTexture_TexelSize.y));

                float minDist = distance(x00, x[0]);
                float2 closestPoint = x00.xy;
                for (int i = 1; i < 8; ++i)
                {
                    float dist = distance(x00, x[i]);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestPoint = x[i].xy;
                    }
                }

                return float4(closestPoint, 0.0f, 1.0f);
            }
            ENDCG
        }
    }
}
