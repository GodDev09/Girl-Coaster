Shader "The Factory/Lit/ToonScrollingTexture" {
	Properties {
		[Header(Object colors)] [Space(10)] _ColorLight ("Color Light", Vector) = (1,1,1,1)
		_ColorShadow ("Color Shadow", Vector) = (0.5,0.5,0.5,1)
		_ColorSpec ("Color Specular", Vector) = (1,1,1,0.5)
		[Header(Light settings)] [Space(10)] _SpecValue ("Specular size", Range(0, 0.5)) = 0.01
		_LightLimit ("Light Limit", Range(-0.2, 1)) = 0
		_Shading ("Smoothing shade", Range(0.001, 0.5)) = 0.001
		[Space(50)] _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ScrollXSpeed ("Scroll value x", Float) = 0.1
		_ScrollYSpeed ("Scroll value y", Float) = 0.1
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
	Fallback "Diffuse"
}