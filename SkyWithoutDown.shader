Shader "Custom/SkyboxPanoramicStretchY"
{
    Properties
    {
        _MainTex ("Skybox (HDR)", 2D) = "white" {}
        _Exposure ("Exposure", Range(0, 8)) = 1
        _CutoffY ("Stretch Cut Y", Range(-1, 1)) = -0.2
        _BottomColor ("Bottom Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" }
        Cull Off ZWrite Off Lighting Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Exposure;
            float _CutoffY;
            float4 _BottomColor;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.dir = normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 dir = normalize(i.dir);

                // Przeskalowanie pionowe – symulacja "rozci¹gniêcia" nieba
                // Im wy¿szy cutoff (bli¿ej 0), tym silniej rozci¹gamy obraz w górê
                float scaleY = 1.0 / (1.0 - saturate(_CutoffY + 1.0) * 0.5);
                dir.y = dir.y * scaleY;

                // Obliczamy UV dla mapy equirectangular
                float2 uv;
                uv.x = atan2(dir.x, dir.z) / (2.0 * UNITY_PI) + 0.5;
                uv.y = asin(clamp(dir.y, -1, 1)) / UNITY_PI + 0.5;

                // Pobieramy kolor nieba
                fixed4 col = tex2D(_MainTex, uv) * _Exposure;

                // Jeœli niebo wychodzi za bardzo poza zakres (zbyt nisko), u¿ywamy koloru t³a
                if (dir.y < _CutoffY)
                    col = _BottomColor;

                return col;
            }
            ENDHLSL
        }
    }
    FallBack Off
}
