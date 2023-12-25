Shader "The Factory/Custom/SH_CausticReflections" {
	Properties {
		[Header(Default Settings)] [Space(20)] _Color ("Color", Vector) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0
		[Header(Caustics Effect)] [Space(20)] _CausticTex ("Caustics (RGB)", 2D) = "white" {}
		_Caustics1_ST ("Caustics ST 1", Vector) = (1,1,0,0)
		_Caustics2_ST ("Caustics ST 2", Vector) = (1,1,0,0)
		_CausticsSpeed_1 ("Caustics Speed 1", Vector) = (1,1,0,0)
		_CausticsSpeed_2 ("Caustics Speed 2", Vector) = (1,1,0,0)
		_SplitRGB ("_SplitRGB effect", Range(0, 5)) = 0.01
		_WaveA ("Wave A (DIRECTIONX, DirectionY, Steepness, WaveLength)", Vector) = (1,0,0.01,1)
		_WaveB ("Wave B (DIRECTIONX, DirectionY, Steepness, WaveLength)", Vector) = (0,1,0.05,2)
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