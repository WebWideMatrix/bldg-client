Shader "BHS/BHS_BumpAOSpecTexture"
{
Properties
{
	_Color ("Color", Color) = (0.5, 0.5, 0.5, 1.0)
	_AOTex("AO Texture", 2D) = "black"
	_BumpMap ("Bumpmap", 2D) = "bump"{}
	_SpecMap ("SpecMap", 2D) = "black"{}
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1.0)
	_SpecPower ("Specular Power", Range(0, 1)) = 0.5
	
}
	SubShader
	{
		Tags {"RenderType" = "Opaque"}
		CGPROGRAM
		#pragma surface surf BlinnPhong

		fixed4 _Color;
		sampler2D _BumpMap;
		sampler2D _AOTex;
		sampler2D _SpecMap;
		float _SpecPower;

		struct Input
		{
			float2 uv_BumpMap;
			float2 uv_AOTex;
			float2 uv_SpecMap;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D (_AOTex, IN.uv_AOTex);
			fixed4 specTex = tex2D (_SpecMap, IN.uv_SpecMap);
			o.Normal = UnpackNormal (tex2D(_BumpMap, IN.uv_BumpMap));
			o.Specular = _SpecPower;
			o.Gloss = specTex.rgb;
			o.Albedo = c.rgb *= _Color;
		}
		ENDCG
	}
	Fallback "Diffuse"
}
