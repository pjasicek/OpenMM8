using UnityEngine;

public partial class TOD_Sky : MonoBehaviour
{
	private const int TOD_SAMPLES = 2;

	private Vector3 kBetaMie;
	private Vector4 kSun;
	private Vector4 k4PI;
	private Vector4 kRadius;
	private Vector4 kScale;

	private float ShaderScale(float inCos)
	{
		float x = 1.0f - inCos;
		return 0.25f * Mathf.Exp(-0.00287f + x*(0.459f + x*(3.83f + x*(-6.80f + x*5.25f))));
	}

	private float ShaderMiePhase(float eyeCos, float eyeCos2)
	{
		return kBetaMie.x * (1.0f + eyeCos2) / Mathf.Pow(kBetaMie.y + kBetaMie.z * eyeCos, 1.5f);
	}

	private float ShaderRayleighPhase(float eyeCos2)
	{
		return 0.75f + 0.75f * eyeCos2;
	}

	private Color ShaderNightSkyColor(Vector3 dir)
	{
		dir.y = Mathf.Max(0, dir.y);

		return MoonSkyColor * (1.0f - 0.75f * dir.y);
	}

	private Color ShaderMoonHaloColor(Vector3 dir)
	{
		return MoonHaloColor * Mathf.Pow(Mathf.Max(0, Vector3.Dot(dir, LocalMoonDirection)), 1f / Moon.MeshSize);
	}

	private Color TOD_HDR2LDR(Color color)
	{
		return new Color(1 - Mathf.Pow(2, -Atmosphere.Brightness * color.r), 1 - Mathf.Pow(2, -Atmosphere.Brightness * color.g), 1 - Mathf.Pow(2, -Atmosphere.Brightness * color.b), color.a);
	}

	private Color TOD_GAMMA2LINEAR(Color color)
	{
		return new Color(color.r * color.r, color.g * color.g, color.b * color.b, color.a);
	}

	private Color TOD_LINEAR2GAMMA(Color color)
	{
		return new Color(Mathf.Sqrt(color.r), Mathf.Sqrt(color.g), Mathf.Sqrt(color.b), color.a);
	}

	private Color ShaderScatteringColor(Vector3 dir, bool directLight = true)
	{
		dir.y = Mathf.Max(0, dir.y);

		//
		// Vertex shader
		//

		float kInnerRadius  = this.kRadius.x;
		float kInnerRadius2 = this.kRadius.y;
		float kOuterRadius2 = this.kRadius.w;

		float kScale               = this.kScale.x;
		float kScaleOverScaleDepth = this.kScale.z;
		float kCameraHeight        = this.kScale.w;

		float kKr4PI_r = this.k4PI.x;
		float kKr4PI_g = this.k4PI.y;
		float kKr4PI_b = this.k4PI.z;
		float kKm4PI   = this.k4PI.w;

		float kKrESun_r = this.kSun.x;
		float kKrESun_g = this.kSun.y;
		float kKrESun_b = this.kSun.z;
		float kKmESun   = this.kSun.w;

		// Current camera position
		Vector3 cameraPos = new Vector3(0, kInnerRadius + kCameraHeight, 0);

		// Length of the atmosphere
		float far = Mathf.Sqrt(kOuterRadius2 + kInnerRadius2 * dir.y * dir.y - kInnerRadius2) - kInnerRadius * dir.y;

		// Ray starting position and its scattering offset
		float startDepth  = Mathf.Exp(kScaleOverScaleDepth * (-kCameraHeight));
		float startAngle  = Vector3.Dot(dir, cameraPos) / (kInnerRadius + kCameraHeight);
		float startOffset = startDepth * ShaderScale(startAngle);

		// Scattering loop variables
		float sampleLength  = far / (float)(TOD_SAMPLES);
		float scaledLength  = sampleLength * kScale;
		Vector3 sampleRay   = dir * sampleLength;
		Vector3 samplePoint = cameraPos + sampleRay * 0.5f;

		float sunColor_r = 0;
		float sunColor_g = 0;
		float sunColor_b = 0;

		// Lop through the sample rays
		for (int i = 0; i < (int)(TOD_SAMPLES); i++)
		{
			float height    = samplePoint.magnitude;
			float invHeight = 1.0f / height;

			float depth = Mathf.Exp(kScaleOverScaleDepth * (kInnerRadius - height));
			float atten = depth * scaledLength;

			float cameraAngle = Vector3.Dot(dir, samplePoint) * invHeight;
			float sunAngle    = Vector3.Dot(LocalSunDirection,  samplePoint) * invHeight;
			float sunScatter  = startOffset + depth * (ShaderScale(sunAngle)  - ShaderScale(cameraAngle));

			float sunAtten_r = Mathf.Exp(-sunScatter * (kKr4PI_r + kKm4PI));
			float sunAtten_g = Mathf.Exp(-sunScatter * (kKr4PI_g + kKm4PI));
			float sunAtten_b = Mathf.Exp(-sunScatter * (kKr4PI_b + kKm4PI));

			sunColor_r  += sunAtten_r * atten;
			sunColor_g  += sunAtten_g * atten;
			sunColor_b  += sunAtten_b * atten;
			samplePoint += sampleRay;
		}

		// Sun scattering
		float inscatter_r  = SunSkyColor.r * sunColor_r * kKrESun_r;
		float inscatter_g  = SunSkyColor.g * sunColor_g * kKrESun_g;
		float inscatter_b  = SunSkyColor.b * sunColor_b * kKrESun_b;
		float outscatter_r = SunSkyColor.r * sunColor_r * kKmESun;
		float outscatter_g = SunSkyColor.g * sunColor_g * kKmESun;
		float outscatter_b = SunSkyColor.b * sunColor_b * kKmESun;

		//
		// Fragment shader
		//

		float resultColor_r = 0;
		float resultColor_g = 0;
		float resultColor_b = 0;

		// Sun angle
		float sunCos  = Vector3.Dot(LocalSunDirection, dir);
		float sunCos2 = sunCos * sunCos;

		// Add sun light
		float rayleighPhase = ShaderRayleighPhase(sunCos2);

		resultColor_r += rayleighPhase * inscatter_r;
		resultColor_g += rayleighPhase * inscatter_g;
		resultColor_b += rayleighPhase * inscatter_b;

		if (directLight)
		{
			float miePhase = ShaderMiePhase(sunCos, sunCos2);

			resultColor_r += miePhase * outscatter_r;
			resultColor_g += miePhase * outscatter_g;
			resultColor_b += miePhase * outscatter_b;
		}

		// Add moon light
		Color nightSkyColor = ShaderNightSkyColor(dir);

		resultColor_r += nightSkyColor.r;
		resultColor_g += nightSkyColor.g;
		resultColor_b += nightSkyColor.b;

		if (directLight)
		{
			Color moonHaloColor = ShaderMoonHaloColor(dir);

			resultColor_r += moonHaloColor.r;
			resultColor_g += moonHaloColor.g;
			resultColor_b += moonHaloColor.b;
		}

		// Lerp to fog color
		resultColor_r = Mathf.Lerp(resultColor_r, FogColor.r, Atmosphere.Fogginess);
		resultColor_g = Mathf.Lerp(resultColor_g, FogColor.g, Atmosphere.Fogginess);
		resultColor_b = Mathf.Lerp(resultColor_b, FogColor.b, Atmosphere.Fogginess);

		// Adjust output color
		resultColor_r = Mathf.Pow(resultColor_r * Atmosphere.Brightness, Atmosphere.Contrast);
		resultColor_g = Mathf.Pow(resultColor_g * Atmosphere.Brightness, Atmosphere.Contrast);
		resultColor_b = Mathf.Pow(resultColor_b * Atmosphere.Brightness, Atmosphere.Contrast);

		return new Color(resultColor_r, resultColor_g, resultColor_b, 1);
	}
}
