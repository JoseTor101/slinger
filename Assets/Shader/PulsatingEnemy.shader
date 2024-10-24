Shader "Custom/PulsatingEnemy"
{
    Properties
    {
        _Color ("Color", Color) = (1,0,0,1) // Red color
        _EmissionStrength ("Emission Strength", Range(0, 5)) = 1 // Control brightness
        _PulsateSpeed ("Pulsate Speed", Range(0.1, 10)) = 1 // Speed of pulsation
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

            fixed4 _Color; // Red color
            float _CustomTime; // Custom time passed from script
            float _PulsateSpeed; // Pulsation speed
            float _EmissionStrength; // Brightness multiplier

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Pulsation factor based on time
                float pulsateFactor = abs(sin(_CustomTime * _PulsateSpeed));

                // Interpolate between black and the set color
                fixed4 pulsatingColor = lerp(fixed4(0, 0, 0, 1), _Color, pulsateFactor);

                // Apply emission strength to make sure the color doesn't get too dark
                pulsatingColor.rgb *= (_EmissionStrength * pulsateFactor);

                return pulsatingColor; // Return the final pulsating color
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
