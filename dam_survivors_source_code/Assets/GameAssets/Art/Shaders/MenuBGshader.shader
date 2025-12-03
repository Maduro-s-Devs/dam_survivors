Shader "Custom/MenuBackground"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Necesario para UI aunque no se use
        _Scale ("Scale", Float) = 1.0
        _Speed ("Speed", Float) = 1.0
        _Mix ("Mix Intensity", Range(0,1)) = 1.0
        _Offset ("Offset", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 vertex : SV_POSITION;
            };

            // Variables expuestas
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Scale;
            float _Speed;
            float _Mix;
            float2 _Offset;

            // Constantes matemáticas del shader original
            static const float2 k = float2(
                (sqrt(3.0) - 1.0) / 2.0,
                (3.0 - sqrt(3.0)) / 6.0
            );

            // Traducción de funciones GLSL a HLSL
            float2 hash(float2 v) {
                v = float2(dot(v, float2(127.1, 311.7)), dot(v, float2(269.5, 183.3)));
                return 2.0 * frac(sin(v) * 43758.5453123) - 1.0;
            }

            float noise(float2 v) {
                float2 i = floor(v + (v.x + v.y) * k.x);
                float2 a = v - i + (i.x + i.y) * k.y;
                float m = step(a.y, a.x); 
                float2 o = float2(m, 1.0 - m);
                float2 b = a - o + k.y;
                float2 c = a - 1.0 + 2.0 * k.y;
                float3 h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c)), 0.0);
                float3 n = pow(h, float3(4.0, 4.0, 4.0)) * float3(dot(a, hash(i)), dot(b, hash(i + o)), dot(c, hash(i + 1.0)));
                return dot(n, float3(70.0, 70.0, 70.0));
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // Usamos UVs estándar de la UI
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Si Mix es 0, devolvemos blanco (según tu código original)
                if (_Mix == 0.0) return fixed4(1,1,1,1);

                // Ajuste de relación de aspecto para que no se estire
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 uv = i.uv;
                uv.x *= aspect; 
                
                // Aplicamos escala y tiempo
                float time = _Time.y * _Speed;
                uv = uv * _Scale - time / 10.0;

                float3 weight = float3(0.0, 0.0, 0.0);
                
                // Bucle de iteraciones (el "loop" del shader original)
                for (float j = 0.0; j < 4.0; j++) {
                    uv = float2(
                        noise(uv + 100.0 + _Offset.x + time / 17.0 + j),
                        noise(uv + 10.0 + _Offset.y)
                    );
                    
                    weight.r += sin(10.0 * (uv.x - uv.y - 0.01));
                    weight.g += sin(10.0 * (uv.x - uv.y));
                    weight.b += sin(10.0 * (uv.x - uv.y + 0.01));
                }

                // Smoothstep y Mix final
                float3 finalColor = lerp(float3(1.0, 1.0, 1.0), smoothstep(-0.25, 0.25, weight) * 0.1 + 0.9, _Mix);

                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}