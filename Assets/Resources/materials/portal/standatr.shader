Shader "Custom/standatr" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf MyModel fullforwardshadows alpha

		half4 LightingMyModel (SurfaceOutput s, half3 lightDir, half atten) {
	 		half4 c;
	 		c.rgb = s.Albedo;
	 		c.a = s.Alpha;
			return c;
		}

		#pragma target 2.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
