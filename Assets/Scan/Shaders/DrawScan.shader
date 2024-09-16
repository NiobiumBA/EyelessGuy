Shader "Custom/DrawScan"
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
            };
			
			//Buffer<float3> _ScanPositions;
			
            sampler2D _MainTex;
			
			//float4 _ScanColor;
			float _ScanBrightness;
			float4 _ScanPosition;
			float _ScanSquareRadius;

            v2f vert (appdata v)
            {
                v2f o;
				
                //o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.vertex = float4(2 * v.uv - 1, 1, 1);
				o.vertex.y *= -1;
                o.uv = v.uv;
				
                return o;
            }

            fixed frag (v2f i) : SV_Target
            {
                fixed source = tex2D(_MainTex, i.uv);
				
				float3 offset = i.worldPos - _ScanPosition.xyz;
				float squareDist = dot(offset, offset);
				
				fixed result = (squareDist < _ScanSquareRadius) ? _ScanBrightness : source;
				
                return result;
            }
            ENDCG
        }
    }
}
