Shader "Custom/Triplanar" {
	Properties {
		_Front ("Base (RGB)", 2D) = "white" {}
		_Top ("Base (RGB)", 2D) = "white" {}
		_Side ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _Front;

		struct Input {
			float2 uv_Front;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_Front, IN.uv_Front);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
