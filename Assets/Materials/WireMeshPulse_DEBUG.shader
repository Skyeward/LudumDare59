Shader "Custom/WireMeshOrganic"
{
    Properties
    {
        _GlowColor ("Glow Color", Color) = (0,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _GlowColor;

            int _PulseCount;
            float4 _Pulses[32];
            float4 _Velocities[32];

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = v.vertex.xyz;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float glow = 0;

                // normalize mesh position into unit sphere space
                float3 localPos = normalize(i.worldPos);

                for (int p = 0; p < _PulseCount; p++)
                {
                    float3 pos = _Pulses[p].xyz;
                    float intensity = _Pulses[p].w;
                    float3 vel = _Velocities[p].xyz;

                    float3 toPixel = localPos - pos;
                    float d = length(toPixel);

                    float3 dir = normalize(vel + 1e-5);
                    float3 toPixelDir = normalize(toPixel + 1e-5);

                    float alignment = dot(toPixelDir, dir) * 0.5 + 0.5;

                    // stable, localised falloff
                    float influence = exp(-d * d * 18.0) * alignment * intensity;

                    glow += influence;
                }

                glow *= 2.8;
                glow = glow / (1.0 + glow);
                glow = pow(glow, 1.3);

                float alpha = glow;
                float3 color = _GlowColor.rgb * glow;

                return float4(color, alpha);
            }

            ENDHLSL
        }
    }
}