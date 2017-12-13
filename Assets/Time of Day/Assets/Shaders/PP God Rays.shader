Shader "Hidden/Time of Day/God Rays"
{
	Properties
	{
		_MainTex ("Base", 2D) = "white" {}
		_SkyMask ("Sky", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "TOD_Base.cginc"

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv  : TEXCOORD0;
	};

	uniform sampler2D _MainTex;
	uniform sampler2D _SkyMask;

	uniform half4 _LightColor;
	uniform half4 _MainTex_TexelSize;
	uniform half4 _MainTex_ST;
	uniform half4 _SkyMask_ST;

	v2f vert(appdata_img v)
	{
		v2f o;

		o.pos = TOD_TRANSFORM_VERT(v.vertex);

		o.uv = v.texcoord.xy;

		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1-o.uv.y;
		#endif

		return o;
	}

	half4 frag_screen(v2f i) : COLOR
	{
		half4 colorA = tex2D(_MainTex, TOD_UV(i.uv, _MainTex_ST));
		half4 colorB = tex2D(_SkyMask, TOD_UV(i.uv, _SkyMask_ST));
		half4 depthMask = saturate(colorB * _LightColor);
		return 1.0f - (1.0f-colorA) * (1.0f-depthMask);
	}

	half4 frag_add(v2f i) : COLOR
	{
		half4 colorA = tex2D(_MainTex, TOD_UV(i.uv, _MainTex_ST));
		half4 colorB = tex2D(_SkyMask, TOD_UV(i.uv, _SkyMask_ST));
		half4 depthMask = saturate(colorB * _LightColor);
		return colorA + depthMask;
	}
	ENDCG

	Subshader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_screen
			ENDCG
		}

		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_add
			ENDCG
		}
	}

	Fallback Off
}
