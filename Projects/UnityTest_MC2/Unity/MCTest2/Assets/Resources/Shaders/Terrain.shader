Shader "Custom/Terrain" {

	Properties {
		_Front ("Front", 2D) = "white" {}
		_Top ("Top", 2D) = "white" {}
		_Side ("Side", 2D) = "white" {}
		_Scale ("Scale", Float) = 1.0
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		#pragma surface surf Lambert

		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

	    sampler2D _Front;
	    sampler2D _Top;
	    sampler2D _Side;
	    float _Scale;
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			// Calculate the weights to use in blending each of the 3 projections
			float3 blend_weights;
			{
				blend_weights = abs(IN.worldNormal);
				
				// Tighten up the blending zone:  
				blend_weights = (blend_weights - 0.2) * 7;  
				blend_weights = max(blend_weights, 0);
				
				// Force weights to sum to 1.0 (very important!)  
				blend_weights /= (blend_weights.x + blend_weights.y + blend_weights.z ).xxx;   
			}
			
			// Determine a color value for each of the 3 projections, blend them and store blended results
			float3 blended_color;
			{
				float3 scaledPos = IN.worldPos * _Scale;
			
				// Compute the UV coords for each of the 3 planar projections
				float2 coord1 = scaledPos.yz;
				float2 coord2 = scaledPos.zx;
				float2 coord3 = scaledPos.xy;
				
				// Sample color maps for each projection, at those UV coords.  
				float4 col1 = tex2D(_Side, coord1);  
				float4 col2 = tex2D(_Top, coord2); 
				float4 col3 = tex2D(_Front, coord3);
				
				// Finally, blend the results of the 3 planar projections.  
				blended_color =
					col1.xyz * blend_weights.xxx +  
            		col2.xyz * blend_weights.yyy +  
            		col3.xyz * blend_weights.zzz;  
			}
			
			o.Albedo = blended_color;
		}
		
		ENDCG
	}
	
	FallBack "Diffuse"
}