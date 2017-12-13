Shader "Hidden/Time of Day/Sky Mask"
{
	Properties
	{
		_MainTex ("Base", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "TOD_Base.cginc"

	struct v2f
	{
		float4 pos      : SV_POSITION;
		float2 uv       : TEXCOORD0;
		float2 uv_depth : TEXCOORD1;
	};

	struct v2f_radial
	{
		float4 pos        : SV_POSITION;
		float2 uv         : TEXCOORD0;
		float2 blurVector : TEXCOORD1;
	};

	uniform sampler2D _MainTex;
	uniform sampler2D_float _CameraDepthTexture;

	uniform half4 _BlurRadius4;
	uniform half4 _LightPosition;
	uniform half4 _MainTex_TexelSize;
	uniform half4 _MainTex_ST;
	uniform half4 _CameraDepthTexture_ST;

	#define SAMPLES 6

	v2f vert(appdata_img v)
	{
		v2f o;

		o.pos = TOD_TRANSFORM_VERT(v.vertex);
		o.uv = v.texcoord.xy;
		o.uv_depth = v.texcoord.xy;

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1-o.uv.y;
		#endif

		return o;
	}

	v2f_radial vert_radial(appdata_img v)
	{
		v2f_radial o;

		o.pos = TOD_TRANSFORM_VERT(v.vertex);
		o.uv = v.texcoord.xy;
		o.blurVector = (_LightPosition.xy - v.texcoord.xy) * _BlurRadius4.xy;

		return o;
	}

	half TransformColor(half4 skyboxValue)
	{
		return 1.01-skyboxValue.a;
	}

	half4 frag_depth(v2f i) : COLOR
	{
		half4 sceneColor = tex2D(_MainTex, TOD_UV(i.uv, _MainTex_ST));

		// Sample depth
		float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, TOD_UV(i.uv_depth, _CameraDepthTexture_ST));
		float depth = Linear01Depth(rawDepth);

		// Consider maximum radius
		half2 vec = _LightPosition.xy - i.uv;
		half dist = saturate(_LightPosition.w - length(vec));

		// Consider shafts blockers
		return (depth > 0.99) ? TransformColor(sceneColor) * dist : 0;
	}

	half4 frag_nodepth(v2f i) : COLOR
	{
		half4 sceneColor = tex2D(_MainTex, TOD_UV(i.uv, _MainTex_ST));

		// Consider maximum radius
		half2 vec = _LightPosition.xy - i.uv;
		half dist = saturate(_LightPosition.w - length(vec));

		// Consider shafts blockers
		return TransformColor(sceneColor) * dist;
	}

	half4 frag_radial(v2f_radial i) : COLOR
	{
		half4 color = half4(0,0,0,0);
		for(int j = 0; j < int(SAMPLES); j++)
		{
			half4 tmpColor = tex2D(_MainTex, TOD_UV(i.uv, _MainTex_ST));
			color += tmpColor;
			i.uv += i.blurVector;
		}
		return color / float(SAMPLES);
	}

	ENDCG

	Subshader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert_radial
			#pragma fragment frag_radial
			ENDCG
		}

		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_depth
			ENDCG
		}

		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_nodepth
			ENDCG
		}
	}

	Fallback Off
}
