Shader "BHS/BHS_SpecTexture"
{
Properties
{
	_MainTex ("Texture", 2D) = "white"{}
	_SpecMap ("SpecMap", 2D) = "black"{}
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1.0)
	_SpecPower ("Specular Power", Range(0, 1)) = 0.5
}
	SubShader
	{
		Tags {"RenderType" = "Opaque"}
		CGPROGRAM
		#pragma surface surf BlinnPhong
		#pragma exclude_renderers flash

		sampler2D _MainTex;
		sampler2D _SpecMap;
		float _SpecPower;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_SpecMap;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 tex = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 specTex = tex2D (_SpecMap, IN.uv_SpecMap);
			o.Albedo = tex.rgb;
			o.Specular = _SpecPower;
			o.Gloss = specTex.rgb;
		}
		ENDCG
	}
	Fallback "Diffuse"
}
