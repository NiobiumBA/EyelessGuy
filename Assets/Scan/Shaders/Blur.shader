Shader "Hidden/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
			
			float _WidthMainTex;
			float _HeightMainTex;
			
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float2 resolution = float2(_WidthMainTex, _HeightMainTex);
				
				float2 pixelMainTex = ceil(i.uv * resolution);
				
				float3 colorSum = 0;
				
				for (int offsetX = -1; offsetX < 1; offsetX++)
				{
					for (int offsetY = -1; offsetY < 1; offsetY++)
					{
						float2 offsettedPixel = pixelMainTex + float2(offsetX, offsetY);
						
						float2 offsetUV = saturate(offsettedPixel / resolution);
						
						colorSum += tex2D(_MainTex, offsetUV).xyz;
					}
				}
				colorSum *= 1.0f / 3.0f;
				
				return fixed4(colorSum, 1);
				
                /*fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;*/
            }
            ENDCG
        }
    }
}
