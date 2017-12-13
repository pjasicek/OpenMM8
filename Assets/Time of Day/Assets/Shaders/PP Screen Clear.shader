Shader "Hidden/Time of Day/Screen Clear"
{
	Properties
	{
		_MainTex ("Base", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "TOD_Base.cginc"

	uniform sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;

	struct v2f
	{
		float4 position : SV_POSITION;
	};

	v2f vert(appdata_img v)
	{
		v2f o;

		o.position = TOD_TRANSFORM_VERT(v.vertex);

		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		return fixed4(0, 0, 0, 0);
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
			ENDCG
		}
	}

	Fallback Off
}
