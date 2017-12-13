Shader "Time of Day/Cloud Billboard Far"
{
	Properties
	{
		_MainTex ("Density Map (RGBA)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "TOD_Base.cginc"
	#include "TOD_Clouds.cginc"
	#include "TOD_Scattering.cginc"

	uniform sampler2D _MainTex;
	uniform sampler2D _BumpMap;

	uniform float4 _MainTex_ST;
	uniform float4 _BumpMap_ST;

	struct v2f
	{
		float4 position : SV_POSITION;
		float4 color    : TEXCOORD0;
		float4 texcoord : TEXCOORD1;
		float3 viewDir  : TEXCOORD2;
		float3 lightDir : TEXCOORD3;
		float3 lightCol : TEXCOORD4;
	};

	v2f vert(appdata_tan v)
	{
		v2f o;

		o.position = TOD_TRANSFORM_VERT(v.vertex);

		o.viewDir  = normalize(mul(TOD_Object2World, v.vertex).xyz);
		o.lightDir = TOD_SunDirection;

		o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.texcoord.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);

		float3 inscatter, outscatter;
		ScatteringCoefficients(o.viewDir, inscatter, outscatter);

		float3 cloudColor = CloudColor(o.viewDir, o.lightDir);

		float sunCos  = dot(o.lightDir, o.viewDir);
		float sunCos2 = sunCos * sunCos;

		float3 nightScattering    = NightPhase(o.viewDir);
		float3 moonScattering     = MoonPhase(o.viewDir);
		float3 rayleighScattering = RayleighPhase(sunCos2) * inscatter;
		float3 mieScattering      = MiePhase(sunCos, sunCos2) * CloudPhase(sunCos, sunCos2) * outscatter;

		float3 dirColor  = PostProcess(moonScattering + mieScattering, o.viewDir);
		float3 baseColor = PostProcess(nightScattering + rayleighScattering, o.viewDir);

		o.lightCol  = dirColor;
		o.color.rgb = TOD_CloudBrightness * lerp(baseColor, cloudColor, TOD_CloudColoring);
		o.color.a   = TOD_CloudOpacity;

		TANGENT_SPACE_ROTATION;
		o.viewDir  = normalize(mul(rotation, o.viewDir));
		o.lightDir = normalize(mul(rotation, o.lightDir));

#if !TOD_CLOUDS_BUMPED

#if !TOD_OUTPUT_HDR
		o.color.rgb = TOD_HDR2LDR(o.color.rgb);
		o.lightCol.rgb = TOD_HDR2LDR(o.lightCol.rgb);
#endif

#if !TOD_OUTPUT_LINEAR
		o.color.rgb = TOD_LINEAR2GAMMA(o.color.rgb);
		o.lightCol.rgb = TOD_LINEAR2GAMMA(o.lightCol.rgb);
#endif

#endif

		return o;
	}

	half4 frag(v2f i) : COLOR
	{
		half4 color = CloudBillboardColor(_MainTex, _BumpMap, i.texcoord, i.color, i.viewDir, i.lightDir, i.lightCol);

#if TOD_CLOUDS_BUMPED

#if !TOD_OUTPUT_HDR
		color.rgb = TOD_HDR2LDR(color.rgb);
#endif

#if !TOD_OUTPUT_LINEAR
		color.rgb = TOD_LINEAR2GAMMA(color.rgb);
#endif

#endif

		return color;
	}
	ENDCG

	SubShader
	{
		Tags
		{
			"Queue"="Geometry+520"
			"RenderType"="Background"
			"IgnoreProjector"="True"
		}

		Pass
		{
			ZWrite Off
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ TOD_OUTPUT_HDR
			#pragma multi_compile _ TOD_OUTPUT_LINEAR
			#pragma multi_compile _ TOD_CLOUDS_DENSITY
			#pragma multi_compile _ TOD_CLOUDS_BUMPED
			ENDCG
		}
	}

	Fallback Off
}
