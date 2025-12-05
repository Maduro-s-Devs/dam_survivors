Shader "Custom/CharacterSelectBG"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BgColor ("Background Color", Color) = (0.98, 0.0, 0.395, 1) // El Rojo original
        _FgColor ("Foreground Color", Color) = (0.0, 0.0, 0.0, 1)    // El Negro original
        _Stripes ("Stripe Count", Float) = 7.0
        _Speed ("Speed", Float) = 1.0
        _Distortion ("Distortion", Float) = 0.2
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

            // Variables del Inspector
            float4 _BgColor;
            float4 _FgColor;
            float _Stripes;
            float _Speed;
            float _Distortion;

            // Función de rotación traducida
            float2 rot(float2 uv, float r) {
                float s = sin(r);
                float c = cos(r);
                // Matriz de rotación 2D manual
                return float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Corrección de aspecto para que no se estire en 16:9
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 uv = i.uv;
                uv.x *= aspect;

                // Tiempo
                float time = _Time.y * _Speed;

                // Rotación (La lógica original adaptada)
                // openfl_TextureCoordv se convierte en nuestra uv modificada
                uv = rot(uv, -0.2 + sin(time) * 0.05);

                // Lógica de distorsión original
                float osc = sin(uv.x * (uv.x + 0.5) * 15.0) * _Distortion;
                uv.y += osc * sin(time + uv.x * 2.0);
                
                // Patrón de rayas (frac es el equivalente a fract de GLSL)
                uv.y = frac(uv.y * _Stripes);
                
                // Máscara (smoothstep funciona igual)
                float st = 0.2; // Grosor raya
                float mask = smoothstep(0.5, 0.55, uv.y);
                mask += smoothstep(0.5 + st, 0.55 + st, 1.0 - uv.y);
                
                // Mezcla de colores
                float3 col = mask * _BgColor.rgb + (1.0 - mask) * _FgColor.rgb;

                return fixed4(col, 1.0);
            }
            ENDCG
        }
    }
}