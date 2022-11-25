Shader "Hovl/Particles/SwordSlash" {
	Properties {
		_InvFade ("Soft Particles Factor", Range(0.01, 3)) = 1
		_MainTexture ("MainTexture", 2D) = "white" {}
		_EmissionTex ("EmissionTex", 2D) = "white" {}
		_Opacity ("Opacity", Float) = 20
		_Dissolve ("Dissolve", 2D) = "white" {}
		_SpeedMainTexUVNoiseZW ("Speed MainTex U/V + Noise Z/W", Vector) = (0,0,0,0)
		_Emission ("Emission", Float) = 5
		_Remap ("Remap", Vector) = (-2,1,0,0)
		_AddColor ("Add Color", Vector) = (0,0,0,0)
		_Desaturation ("Desaturation", Float) = 0
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01, 3)) = 1
		[MaterialToggle] _Usedepth ("Use depth?", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}