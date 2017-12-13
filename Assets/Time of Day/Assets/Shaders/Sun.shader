Shader "Time of Day/Sun"
{
	Properties
	{
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "TOD_Base.cginc"

	struct v2f
	{
		float4 position : SV_POSITION;
		half3  tex      : TEXCOORD0;
	};

	v2f vert(appdata_base v)
	{
		v2f o;

		o.position = TOD_TRANSFORM_VERT(v.vertex);

		float3 skyPos = mul(TOD_World2Sky, mul(TOD_Object2World, v.vertex)).xyz;

		o.tex.xy = 2.0 * v.texcoord - 1.0;
		o.tex.z  = skyPos.y * 25;

		return o;
	}

	half4 frag(v2f i) : COLOR
	{
		half3 color = TOD_SunMeshColor;

		half dist = length(i.tex.xy);

		half sun  = step(dist, 0.5) * TOD_SunMeshBrightness;
		half glow = smoothstep(0.0, 1.0, 1.0 - pow(dist, TOD_SunMeshContrast)) * saturate(TOD_SunMeshBrightness);

		color *= saturate(i.tex.z) * (sun + glow);

		return half4(color, 0);
	}
	ENDCG

	SubShader
	{
		Tags
		{
			"Queue"="Background+30"
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
			ENDCG
		}
	}

	Fallback Off
}
