Shader "Hidden/Time of Day/Scattering"
{
	Properties
	{
		_MainTex ("Base", 2D) = "white" {}
		_SkyMask ("Sky", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "TOD_Base.cginc"
	#include "TOD_Scattering.cginc"

	uniform sampler2D _MainTex;
	uniform sampler2D _SkyMask;
	uniform sampler2D_float _CameraDepthTexture;

	uniform float4x4 _FrustumCornersWS;
	uniform float4 _MainTex_TexelSize;
	uniform float4 _MainTex_ST;
	uniform float4 _SkyMask_ST;
	uniform float4 _CameraDepthTexture_ST;

	struct v2f
	{
		float4 pos       : SV_POSITION;
		float2 uv        : TEXCOORD0;
		float2 uv_depth  : TEXCOORD2;
#if TOD_OUTPUT_DITHERING
		float2 uv_dither : TEXCOORD3;
#endif
		float4 cameraRay : TEXCOORD4;
		TOD_VERTEX_OUTPUT_STEREO
	};

	v2f vert(appdata_img v)
	{
		v2f o;

		o.pos = TOD_TRANSFORM_VERT(v.vertex);

		o.uv        = v.texcoord.xy;
		o.uv_depth  = v.texcoord.xy;

#if TOD_OUTPUT_DITHERING
		o.uv_dither = DitheringCoords(v.texcoord.xy);
#endif

#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1-o.uv.y;
#endif

		int frustumIndex = v.texcoord.x + (2 * o.uv.y);
		o.cameraRay = _FrustumCornersWS[frustumIndex];
		o.cameraRay.w = frustumIndex;

		return o;
	}

	half4 frag(v2f i) : COLOR
	{
		half4 color = tex2D(_MainTex, TOD_UV(i.uv, _MainTex_ST));

		float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, TOD_UV(i.uv_depth, _CameraDepthTexture_ST));
		float depth = Linear01Depth(rawDepth);
		float4 cameraToWorldPos = depth * i.cameraRay;
		float3 worldPos = _WorldSpaceCameraPos + cameraToWorldPos;

		half4 mask = tex2D(_SkyMask, TOD_UV(i.uv, _SkyMask_ST));
		half4 scattering = AtmosphericScattering(i.cameraRay, worldPos, depth, mask);

#if TOD_OUTPUT_DITHERING
		scattering.rgb += DitheringColor(i.uv_dither);
#endif

#if !TOD_OUTPUT_HDR
		scattering = TOD_HDR2LDR(scattering);
#endif

#if !TOD_OUTPUT_LINEAR
		scattering = TOD_LINEAR2GAMMA(scattering);
#endif

		if (depth == 1)
		{
#if TOD_SCATTERING_SINGLE_PASS
			color.rgb += scattering.rgb;
#endif
		}
		else
		{
			color.rgb = lerp(color.rgb, scattering.rgb, scattering.a);
		}

		return color;
	}
	ENDCG

	SubShader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ TOD_OUTPUT_HDR
			#pragma multi_compile _ TOD_OUTPUT_LINEAR
			#pragma multi_compile _ TOD_OUTPUT_DITHERING
			#pragma multi_compile _ TOD_SCATTERING_SINGLE_PASS
			ENDCG
		}
	}

	Fallback Off
}
