Shader "Hidden/ScanRender"
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
			#include "ScanUtils.hlsl"
			
			StructuredBuffer<SphereInfo> _ProjectedSpheres;
			
            sampler2D _MainTex;
			sampler2D _CircleTex;
			
			float _SquareSphereRadius;
			int _SpheresCount;
			float _AspectRatio;
			float _FarClipPlane;

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
			
			void AllSpheresIntersect(float2 viewport, inout fixed3 color)
			{
				viewport.x *= _AspectRatio;
				float invFarClipPlane = 1.0f / _FarClipPlane;
				
				float minDist = _FarClipPlane;
				float3 sphereProjPos;
				float2 offsetViewport;
				SphereInfo currentSphere;
				
				for (int i = 0; i < _SpheresCount; i++)
				{
					currentSphere = _ProjectedSpheres[i];
					sphereProjPos = currentSphere.position;
					
					if (sphereProjPos.z < minDist)
					{
						offsetViewport = viewport - sphereProjPos.xy;
						offsetViewport *= sphereProjPos.z;
						//color = tex2D(_CircleTex, saturate(halfSizeSphere + offsetViewport) * _SphereRadius).xyz * currentSphere.color;
						
						if (dot(offsetViewport, offsetViewport) < _SquareSphereRadius)
						{
							minDist = sphereProjPos.z;
							color = currentSphere.color;
							//color = max(color, currentSphere.color);
						}
					}
				}
			}

            fixed4 frag (v2f i) : SV_Target
            {
				float2 viewport = 2.0f * i.uv - 1.0f;
				
				fixed3 result = tex2D(_MainTex, i.uv).xyz;
				
				AllSpheresIntersect(viewport, result);
				
				//return fixed4(tex2D(_CircleTex, i.uv).xyz, 1);
				return fixed4(result, 1);
            }
            ENDCG
        }
    }
}
