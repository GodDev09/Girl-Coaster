Shader "The Factory/Lit/Vertex Lit Outlined Mesh" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Vector) = (1,1,1,1)
		_SpecColor ("Spec Color", Vector) = (1,1,1,1)
		_Emission ("Emissive Color", Vector) = (0,0,0,0)
		[PowerSlider(5.0)] _Shininess ("Shininess", Range(0.01, 1)) = 0.7
		_OutlineWidthColor ("Outline Color", Vector) = (0,0,0,1)
		_OutlineWidth ("Outline width", Range(0, 1)) = 0.1
	}
	
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}