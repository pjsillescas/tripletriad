Shader "Custom/PlaneOutlineSDF"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _ColorBase ("Base Color", Color) = (1,1,1,1)
        _UseBackground("Use Background", int) = 0
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,0.1)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _Color;
            float4 _ColorBase;
            float4 _OutlineColor;
            float _OutlineWidth;
            int _UseBackground;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate distance from the edge
                float2 edgeDist = min(i.uv, 1.0 - i.uv);
                float distance = min(edgeDist.x, edgeDist.y);

                // Sample the main texture
                fixed4 col = tex2D(_MainTex, i.uv);

                if(col.a == 0)
                {
                    float alpha = length(i.uv);
                    col = (fixed4) lerp(_ColorBase, _Color, clamp(sqrt(alpha),0,1));
                    /*
                    if(_UseBackground > 0)
                    {
                        float alpha = length(i.uv);
                        col = (fixed4) lerp(_ColorBase, _Color, clamp(sqrt(alpha),0,1));
                    }
                    else
                    {
                        col = fixed4(1,1,1,0);
                    }
                    */
                }
                
                // Apply outline based on distance
                float outlineFactor = 1 - smoothstep(_OutlineWidth, _OutlineWidth * 0.5, distance);
                col.rgb = lerp(_OutlineColor.rgb, col.rgb, outlineFactor);

                return col;
            }
            ENDCG
        }
    }
}