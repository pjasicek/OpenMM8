Shader "Time of Day/Atmosphere"
{
	Properties
	{
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "TOD_Base.cginc"
	#include "TOD_Scattering.cginc"

	struct v2f
	{
		float4 position   : SV_POSITION;
#if TOD_OUTPUT_DITHERING
		float2 texcoord   : TEXCOORD0;
#endif
#if TOD_SCATTERING_PER_PIXEL
		float3 inscatter  : TEXCOORD1;
		float3 outscatter : TEXCOORD2;
		float3 viewDir    : TEXCOORD3;
#else
		float4 color      : TEXCOORD1;
#endif
	};

	float4 Adjust(float4 color)
	{
#if !TOD_OUTPUT_HDR
		color = TOD_HDR2LDR(color);
#endif

#if !TOD_OUTPUT_LINEAR
		color = TOD_LINEAR2GAMMA(color);
#endif

		return color;
	}

	v2f vert(appdata_base v)
	{
		v2f o;

		o.position = TOD_TRANSFORM_VERT(v.vertex);

		float3 vertnorm = normalize(v.vertex.xyz);

#if TOD_SCATTERING_PER_PIXEL
		o.viewDir = vertnorm;
		ScatteringCoefficients(o.viewDir, o.inscatter, o.outscatter);
#else
		o.color = Adjust(ScatteringColor(vertnorm));
#endif

#if TOD_OUTPUT_DITHERING
		float4 projPos = ComputeScreenPos(o.position);
		o.texcoord = DitheringCoords(projPos.xy / projPos.w);
#endif

		return o;
	}

	float4 frag(v2f i) : COLOR
	{
#if TOD_SCATTERING_PER_PIXEL
		float4 color = Adjust(ScatteringColor(normalize(i.viewDir), i.inscatter, i.outscatter));
#else
		float4 color = i.color;
#endif

#if TOD_OUTPUT_DITHERING
		color.rgb += DitheringColor(i.texcoord);
#endif

		return float4(color.rgb, 0);
	}
	ENDCG

	SubShader
	{
		Tags
		{
			"Queue"="Background+50"
			"RenderType"="Background"
			"IgnoreProjector"="True"
		}

		Pass
		{
			ZWrite Off
			ZTest LEqual
			Blend One One
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ TOD_OUTPUT_HDR
			#pragma multi_compile _ TOD_OUTPUT_LINEAR
			#pragma multi_compile _ TOD_OUTPUT_DITHERING
			#pragma multi_compile _ TOD_SCATTERING_PER_PIXEL
			ENDCG
		}
	}

	Fallback Off
}
