Shader "GradientSkybox/Linear/Three Color" {
	Properties {
		_TopColor ("Top Color", Vector) = (1,0.3,0.3,0)
		_MiddleColor ("MiddleColor", Vector) = (1,1,0.8,1)
		_BottomColor ("Bottom Color", Vector) = (0.3,0.3,1,0)
		_Up ("Up", Vector) = (0,1,0,1)
		_Exp ("Exp", Range(0, 16)) = 1
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
	//CustomEditor "GradientSkybox.LinearThreeColorGradientSkyboxGUI"
}