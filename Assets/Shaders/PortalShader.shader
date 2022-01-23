Shader "Custom/Portal"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_MaskTex("Mask", 2D) = "white" {}
		_NoizeForce("NoizeForce", Range(0,1)) = 0
		_FramesX ("Frames X", int) = 1.0
		_FramesY ("Frames Y", int) = 1.0
        _FPS ("FPS", float) = 1.0
        _RotateSpeed ("Rotatation Speed", Range(-10,10)) = 1.0
	}
	SubShader
	{
		Tags {
			"RenderType"="Opaque"
		}

		LOD 200

		CGPROGRAM 
		#pragma surface surf MyModel //vertex:vert

	 	half4 LightingMyModel(SurfaceOutput s, half3 lightDir, half atten) {
	 		half4 c;
	 		c.rgb = s.Albedo;
	 		c.a = s.Alpha;
			return c;
		}

	 	struct Input 
	    {
	       float2 uv_MainTex;
	       float2 uv_NoiseTex;
	       float2 uv_MaskTex;
	    };
		
		sampler2D _MainTex;
		sampler2D _NoiseTex;
		sampler2D _MaskTex;
		fixed4 _Color;
		uniform float _NoizeForce;
		uniform int _FramesX;
		uniform int _FramesY;
        uniform float _FPS;
        uniform float _RotateSpeed;

//        void vert (inout appdata_full v) {
////        	cam.Position = new Point3D(
////                cam.Position.X * Math.Cos(rad) - cam.Position.Z * Math.Sin(rad),
////                0,
////                cam.Position.X * Math.Sin(rad) + cam.Position.Z * Math.Cos(rad)
////                );
//
//        	//v.vertex.xyz += v.normal * _Amount;
//        	//unity_ObjectToWorld
//        	//unity_WorldToObject
//
//        	//float4 w = v.vertex;//mul(unity_ObjectToWorld, v.vertex);
//
//        	float rad = fmod(_Time.z, 360.0); //fmod((3.14 * _Time) / 180.0, 360.0);
//        	float x = v.vertex.x;
//        	float y = v.vertex.y;
//        	float cosRad = cos(rad);
//        	float sinRad = sin(rad);
//
//        	v.vertex.x = x * cosRad - y * sinRad;
//        	v.vertex.y = x * sinRad + y * cosRad;
//      	}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float mask = length(tex2D(_MaskTex, IN.uv_MaskTex).rgb);
			clip (mask-1.5);

			float4 noise = tex2D(_NoiseTex, IN.uv_NoiseTex + _Time.y);
			float2 uvOffset = noise.rg*2-1;
			float2 uvNoise = IN.uv_MainTex + (uvOffset*(_NoizeForce/10.0));

//			uvNoise.x = (uvNoise.x+_Tiling.x)*_Tiling.y;
//			uvNoise.y = (uvNoise.y+_Tiling.z)*_Tiling.w;

			//float2 uvNoise = IN.uv_MainTex;


			float2 add = float2(
				fmod(floor(_Time.y*_FPS), _FramesX), 
				_FramesY-1-fmod(floor(_Time.y*_FPS/_FramesY), _FramesY)
			);
			uvNoise.x = (uvNoise.x+add.x)*1.0/_FramesX;
			uvNoise.y = (uvNoise.y+add.y)*1.0/_FramesY;

//			uvNoise.x = (uvNoise.x+ fmod(floor(_Time.y*_FPS), _FramesX))*1.0/_FramesX;
//			uvNoise.y = (uvNoise.y+fmod(floor(_Time.y*_FPS/_FramesY), _FramesY))*1.0/_FramesY;

			float rad = fmod(_Time.y*_RotateSpeed, 360.0);

			add = float2(
				(1.0/_FramesX)*add.x + (1.0/(_FramesX*2.0)),
				(1.0/_FramesY)*add.y + (1.0/(_FramesY*2.0))
			);

			float x = uvNoise.x-add.x;
        	float y = uvNoise.y-add.y;
        	float cosRad = cos(rad);
        	float sinRad = sin(rad);
        	uvNoise.x = (x * cosRad - y * sinRad)+add.x;
        	uvNoise.y = (x * sinRad + y * cosRad)+add.y;


			float4 tex = tex2D (_MainTex, uvNoise)*_Color;
			clip (tex.a-0.01);

			o.Albedo = tex.rgb;
			o.Alpha = 1;
		}
		ENDCG
	}
	Fallback "Diffuse"
}