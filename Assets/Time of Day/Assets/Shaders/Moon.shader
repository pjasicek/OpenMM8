Shader "Time of Day/Moon"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "TOD_Base.cginc"

	uniform sampler2D _MainTex;
	uniform float4 _MainTex_ST;

	struct v2f
	{
		float4 position : SV_POSITION;
		half3  tex      : TEXCOORD0;
		half3  normal   : TEXCOORD1;
	};

	v2f vert(appdata_base v)
	{
		v2f o;

		o.position = TOD_TRANSFORM_VERT(v.vertex);

		o.normal = normalize(mul((float3x3)TOD_Object2World, v.normal));

		float3 skyPos = mul(TOD_World2Sky, mul(TOD_Object2World, v.vertex)).xyz;

		o.tex.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.tex.z  = skyPos.y * 25;

		return o;
	}

	half4 frag(v2f i) : COLOR
	{
		half4 color = half4(TOD_MoonMeshColor, 0);

		half alpha = max(0, dot(i.normal, TOD_SunDirection));
		alpha = saturate(i.tex.z) * TOD_MoonMeshBrightness * pow(alpha, TOD_MoonMeshContrast);

		half3 maintex = tex2D(_MainTex, i.tex).rgb;

		color.rgb *= maintex * alpha;

		return color;
	}
	ENDCG

	SubShader
	{
		Tags
		{
			"Queue"="Background+40"
			"RenderType"="Background"
			"IgnoreProjector"="True"
		}

		Pass
		{
			ZWrite Off
			ZTest LEqual
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}

	Fallback Off
}
