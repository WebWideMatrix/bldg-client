Shader "BHS/BHS_MetalicReflective" {

	Properties {
		_myColor ("Example Color", Color) = (1,1,1,1)
		_SpecMap ("SpecMap", 2D) = "black"{}
		_myRange ("Example Range", Range(0,5)) = 1 
		_myCube ("Example Cube", CUBE) = "" {}
		_emissiveRange ("Reflection Intensity", Range (0,5)) = 1
	}

	SubShader {
		
		CGPROGRAM
			#pragma surface surf Lambert

			fixed4 _myColor;
			sampler2D _SpecMap;
			half _myRange;
			samplerCUBE _myCube;
			half _emissiveRange;

			struct Input {
				float3 worldRefl;
				float2 uv_SpecMap;
			};

			void surf (Input IN, inout SurfaceOutput o) {
				fixed4 specTex = tex2D (_SpecMap, IN.uv_SpecMap);
				o.Albedo = _myColor.rgb * _myRange;
				o.Emission = ((texCUBE(_myCube, IN.worldRefl) * _emissiveRange)) *specTex.rgb;
			}

		ENDCG
	}

	FallBack "Diffuse"
}