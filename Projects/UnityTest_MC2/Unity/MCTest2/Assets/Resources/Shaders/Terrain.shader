Shader "Custom/Terrain" {
	Properties {
		_Front ("Front", 2D) = "white" {}
		_Top ("Top", 2D) = "white" {}
		_Side ("Side", 2D) = "white" {}
	}
	SubShader {
		Pass {
			CGPROGRAM
// Upgrade NOTE: excluded shader from Xbox360; has structs without semantics (struct v2f members uv)
#pragma exclude_renderers xbox360
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct v2f {
			    float4 pos : POSITION0;
			    float3 posOther : TEXCOORD0;
			    float3 normal : TEXCOORD1;
			};
			
			v2f vert (appdata_base v)
			{
			    v2f o;
			    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			    o.posOther = mul(UNITY_MATRIX_MVP, v.vertex);
			    o.normal = normalize(v.normal);
			    return o;
			}
			
		    sampler2D _Front;
		    sampler2D _Top;
		    sampler2D _Side;
			half4 frag (v2f i) : COLOR
			{
				float3 blend_weights = abs(i.normal.xyz);
				
				// Tighten up the blending zone:  
				blend_weights = (blend_weights - 0.2) * 7;  
				blend_weights = max(blend_weights, 0);
				
				// Force weights to sum to 1.0 (very important!)  
				blend_weights /= (blend_weights.x + blend_weights.y + blend_weights.z ).xxx;   
				
				// Now determine a color value for each of the 3 projections, blend them and store blended results
				float4 blended_color; // w holds spec value
				{
					// Compute the UV coords for each of the 3 planar projections
					float2 coord1 = i.posOther.yz;
					float2 coord2 = i.posOther.zx;
					float2 coord3 = i.posOther.xy;
					
					// Sample color maps for each projection, at those UV coords.  
					float4 col1 = tex2D(_Side, coord1);  
					float4 col2 = tex2D(_Top, coord2); 
					float4 col3 = tex2D(_Front, coord3);
					
					// Finally, blend the results of the 3 planar projections.  
					blended_color =
						col1.xyzw * blend_weights.xxxx +  
                		col2.xyzw * blend_weights.yyyy +  
                		col3.xyzw * blend_weights.zzzz;  
				}
				
			    //return half4 (i.color, 1);
			    return half4(blended_color);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
