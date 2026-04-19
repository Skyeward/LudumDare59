Shader "Custom/WireMeshPulse"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.1,0.1,0.1,1)
        _GlowColor ("Glow Color", Color) = (0,1,1,1)
        _Speed ("Speed", Float) = 2
        _Falloff ("Falloff", Float) = 6
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float glow : TEXCOORD0;
            };

            float4 _BaseColor;
            float4 _GlowColor;
            float _Speed;
            float _Falloff;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float dist = v.uv2.x;

                float wave = _Time.y * _Speed;

                float diff = dist - wave;

                o.glow = exp(-abs(diff) * _Falloff);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 col = _BaseColor.rgb;
                col += _GlowColor.rgb * i.glow;

                return float4(col, 1);
            }

            ENDHLSL
        }
    }
}