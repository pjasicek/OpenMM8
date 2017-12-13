using UnityEngine;

/// Utility method class.
///
/// All of those methods should really be in the Unity API, but they're not.

public static class TOD_Util
{
	/// Scale the RGB components of a color.
	/// \param color The color.
	/// \param multiplier The multiplier.
	/// \returns The input color with its RGB components multiplied by multiplier.
	public static Color MulRGB(Color color, float multiplier)
	{
		if (multiplier == 1) return color;
		return new Color(color.r * multiplier, color.g * multiplier, color.b * multiplier, color.a);
	}

	/// Scale the RGBA components of a color.
	/// \param color The color.
	/// \param multiplier The multiplier.
	/// \returns The input color with its RGB components multiplied by multiplier.
	public static Color MulRGBA(Color color, float multiplier)
	{
		if (multiplier == 1) return color;
		return new Color(color.r * multiplier, color.g * multiplier, color.b * multiplier, color.a * multiplier);
	}

	/// Take the power of the RGB components of a color.
	/// \param color The color.
	/// \param power The power.
	/// \returns The input color with its RGB components raised by the exponent power.
	public static Color PowRGB(Color color, float power)
	{
		if (power == 1) return color;
		return new Color(Mathf.Pow(color.r, power), Mathf.Pow(color.g, power), Mathf.Pow(color.b, power), color.a);
	}

	/// Take the power of the RGBA components of a color.
	/// \param color The color.
	/// \param power The power.
	/// \returns The input color with its RGBA components raised by the exponent power.
	public static Color PowRGBA(Color color, float power)
	{
		if (power == 1) return color;
		return new Color(Mathf.Pow(color.r, power), Mathf.Pow(color.g, power), Mathf.Pow(color.b, power), Mathf.Pow(color.a, power));
	}

	/// Change the saturation of the RGB components of a color.
	/// \param color The color.
	/// \param saturation The change in saturation.
	/// \returns The input color with adjusted saturation.
	public static Color SatRGB(Color color, float saturation)
	{
		float v = color.grayscale;

		color.r = v + (color.r - v) * saturation;
		color.g = v + (color.g - v) * saturation;
		color.b = v + (color.b - v) * saturation;

		return color;
	}

	/// Change the saturation of the RGBA components of a color.
	/// \param color The color.
	/// \param saturation The change in saturation.
	/// \returns The input color with adjusted saturation.
	public static Color SatRGBA(Color color, float saturation)
	{
		float v = color.grayscale;

		color.r = v + (color.r - v) * saturation;
		color.g = v + (color.g - v) * saturation;
		color.b = v + (color.b - v) * saturation;
		color.a = v + (color.a - v) * saturation;

		return color;
	}

	/// Change intensity and saturation of the RGB components of a color.
	/// \param color The color.
	/// \param intensity The change in intensity.
	/// \param saturation The change in saturation.
	/// \returns The input color with adjusted intensity and saturation.
	public static Color AdjustRGB(Color color, float intensity, float saturation)
	{
		return MulRGB(SatRGB(color, saturation), intensity);
	}

	/// Change intensity and saturation of the RGBA components of a color.
	/// \param color The color.
	/// \param intensity The change in intensity.
	/// \param saturation The change in saturation.
	/// \returns The input color with adjusted intensity and saturation.
	public static Color AdjustRGBA(Color color, float intensity, float saturation)
	{
		return MulRGBA(SatRGBA(color, saturation), intensity);
	}

	/// Apply the alpha value of a color to its color components.
	/// \param color The color.
	/// \returns The input color with its RGB components multiplied by its A component.
	public static Color ApplyAlpha(Color color)
	{
		return new Color(color.r * color.a, color.g * color.a, color.b * color.a, 1.0f);
	}
}
