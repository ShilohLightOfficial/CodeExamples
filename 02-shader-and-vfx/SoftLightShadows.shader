Shader "Unlit/SoftLightShadows"
{
    Properties
    {
        _MainTex ("Shadow Shape Mask", 2D) = "white" {}
        _Color ("Shadow Control Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _COLOR
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _PlatformColorTex;
            fixed4 _Color; // SpriteRenderer color

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 screenUV : TEXCOORD1;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                float4 clipPos = o.vertex;
                o.screenUV = (clipPos.xy / clipPos.w) * 0.5 + 0.5;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 shapeTex = tex2D(_MainTex, i.uv);
                if (shapeTex.a < 0.5) discard;

                fixed4 platformColor = tex2D(_PlatformColorTex, i.screenUV);
                if (platformColor.r == 1.0 && platformColor.g == 0.0 && platformColor.b == 0.0) discard;

                fixed shadowStrength = saturate(_Color.a);

                fixed3 B = platformColor.rgb;
                fixed3 S = fixed3(0.27, 0.27, 0.3); // soft light tint

                fixed3 softLight;
                for (int k = 0; k < 3; ++k)
                {
                    if (S[k] < 0.5)
                        softLight[k] = B[k] - (1.0 - 2.0 * S[k]) * B[k] * (1.0 - B[k]);
                    else
                        softLight[k] = B[k] + (2.0 * S[k] - 1.0) * (sqrt(B[k]) - B[k]);
                }

                float boost = 2; // Try values like 1.2 to 2.0
                fixed3 finalColor = lerp(B, softLight, saturate(shadowStrength * boost));
                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}
