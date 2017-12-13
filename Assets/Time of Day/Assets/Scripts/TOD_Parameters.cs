#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_4
#endif

using UnityEngine;
#if !UNITY_4
using UnityEngine.Rendering;
#endif
using System;

/// All parameters of the sky dome.
[Serializable] public class TOD_Parameters
{
	public TOD_CycleParameters      Cycle;
	public TOD_WorldParameters      World;
	public TOD_AtmosphereParameters Atmosphere;
	public TOD_DayParameters        Day;
	public TOD_NightParameters      Night;
	public TOD_SunParameters        Sun;
	public TOD_MoonParameters       Moon;
	public TOD_LightParameters      Light;
	public TOD_StarParameters       Stars;
	public TOD_CloudParameters      Clouds;
	public TOD_FogParameters        Fog;
	public TOD_AmbientParameters    Ambient;
	public TOD_ReflectionParameters Reflection;

	public TOD_Parameters()
	{
	}

	public TOD_Parameters(TOD_Sky sky)
	{
		Cycle      = sky.Cycle;
		World      = sky.World;
		Atmosphere = sky.Atmosphere;
		Day        = sky.Day;
		Night      = sky.Night;
		Sun        = sky.Sun;
		Moon       = sky.Moon;
		Light      = sky.Light;
		Stars      = sky.Stars;
		Clouds     = sky.Clouds;
		Fog        = sky.Fog;
		Ambient    = sky.Ambient;
		Reflection = sky.Reflection;
	}

	public void ToSky(TOD_Sky sky)
	{
		sky.Cycle      = Cycle;
		sky.World      = World;
		sky.Atmosphere = Atmosphere;
		sky.Day        = Day;
		sky.Night      = Night;
		sky.Sun        = Sun;
		sky.Moon       = Moon;
		sky.Light      = Light;
		sky.Stars      = Stars;
		sky.Clouds     = Clouds;
		sky.Fog        = Fog;
		sky.Ambient    = Ambient;
		sky.Reflection = Reflection;
	}
}

/// Parameters of the day and night cycle.
/// This is the type of the TOD_Sky.Cycle inspector variable group.
[Serializable] public class TOD_CycleParameters
{
	/// [0, 24]
	/// Current hour of the day.
	[Tooltip("Current hour of the day.")]
	public float Hour = 12;

	/// [1, 31]
	/// Current day of the month.
	[Tooltip("Current day of the month.")]
	public int Day = 15;

	/// [1, 12]
	/// Current month of the year.
	[Tooltip("Current month of the year.")]
	public int Month = 6;

	/// [1, 9999]
	/// Current year.
	[Tooltip("Current year.")]
	[TOD_Range(1, 9999)] public int Year = 2000;

	/// All time information as a System.DateTime instance.
	public System.DateTime DateTime
	{
		get
		{
			var res = new DateTime(0, DateTimeKind.Utc);

			if (Year  > 0) res = res.AddYears(Year-1);
			if (Month > 0) res = res.AddMonths(Month-1);
			if (Day   > 0) res = res.AddDays(Day-1);
			if (Hour  > 0) res = res.AddHours(Hour);

			return res;
		}
		set
		{
			Year  = value.Year;
			Month = value.Month;
			Day   = value.Day;
			Hour  = value.Hour + value.Minute / 60f + value.Second / 3600f + value.Millisecond / 3600000f;
		}
	}

	/// All time information as a single long.
	/// Value corresponds to the System.DateTime.Ticks property.
	public long Ticks
	{
		get
		{
			return DateTime.Ticks;
		}
		set
		{
			DateTime = new System.DateTime(value, DateTimeKind.Utc);
		}
	}
}

/// Parameters of the world.
/// This is the type of the TOD_Sky.World inspector variable group.
[Serializable] public class TOD_WorldParameters
{
	/// [-90, +90]
	/// Latitude of the current location in degrees.
	[Tooltip("Latitude of the current location in degrees.")]
	[Range(-90f, +90f)] public float Latitude = 0;

	/// [-180, +180]
	/// Longitude of the current location in degrees.
	[Tooltip("Longitude of the current location in degrees.")]
	[Range(-180f, +180f)] public float Longitude = 0;

	/// [-14, +14]
	/// UTC/GMT time zone of the current location in hours.
	[Tooltip("UTC/GMT time zone of the current location in hours.")]
	[Range(-14f, +14f)] public float UTC = 0;
}

/// Parameters of the atmosphere.
/// This is the type of the TOD_Sky.Atmosphere inspector variable group.
[Serializable] public class TOD_AtmosphereParameters
{
	/// [0, &infin;]
	/// Intensity of the atmospheric Rayleigh scattering.
	[Tooltip("Intensity of the atmospheric Rayleigh scattering.")]
	[TOD_Min(0f)] public float RayleighMultiplier = 1.0f;

	/// [0, &infin;]
	/// Intensity of the atmospheric Mie scattering.
	[Tooltip("Intensity of the atmospheric Mie scattering.")]
	[TOD_Min(0f)] public float MieMultiplier = 1.0f;

	/// [0, &infin;]
	/// Overall brightness of the atmosphere.
	[Tooltip("Overall brightness of the atmosphere.")]
	[TOD_Min(0f)] public float Brightness = 1.5f;

	/// [0, &infin;]
	/// Overall contrast of the atmosphere.
	[Tooltip("Overall contrast of the atmosphere.")]
	[TOD_Min(0f)] public float Contrast = 1.5f;

	/// [0, 1]
	/// Directionality factor that determines the size of the glow around the sun.
	[Tooltip("Directionality factor that determines the size of the glow around the sun.")]
	[TOD_Range(0f, 1f)] public float Directionality = 0.7f;

	/// [0, 1]
	/// Density of the fog covering the sky.
	[Tooltip("Density of the fog covering the sky.")]
	[TOD_Range(0f, 1f)] public float Fogginess = 0.0f;
}

/// Parameters that are unique to the day.
/// This is the type of the TOD_Sky.Day inspector variable group.
[Serializable] public class TOD_DayParameters
{
	/// Color of the sun spot.
	/// Left value: Sun at zenith.
	/// Right value: Sun at horizon.
	[Tooltip("Color of the sun spot.\nLeft value: Sun at zenith.\nRight value: Sun at horizon.")]
	public Gradient SunColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(1.0f, 0.5f),
			new GradientAlphaKey(1.0f, 1.0f)
		},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(253, 171, 050, 255), 0.0f),
			new GradientColorKey(new Color32(253, 171, 050, 255), 0.5f),
			new GradientColorKey(new Color32(253, 171, 050, 255), 1.0f)
		}
	};

	/// Color of the light that hits the ground.
	/// Left value: Sun at zenith.
	/// Right value: Sun at horizon.
	[Tooltip("Color of the light that hits the ground.\nLeft value: Sun at zenith.\nRight value: Sun at horizon.")]
	public Gradient LightColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(1.0f, 0.5f),
			new GradientAlphaKey(1.0f, 1.0f)
		},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(255, 243, 234, 255), 0.0f),
			new GradientColorKey(new Color32(255, 243, 234, 255), 0.5f),
			new GradientColorKey(new Color32(255, 154, 000, 255), 1.0f)
		}
	};

	/// Color of the god rays.
	/// Left value: Sun at zenith.
	/// Right value: Sun at horizon.
	[Tooltip("Color of the god rays.\nLeft value: Sun at zenith.\nRight value: Sun at horizon.")]
	public Gradient RayColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(1.0f, 0.5f),
			new GradientAlphaKey(1.0f, 1.0f)
		},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(255, 243, 234, 255), 0.0f),
			new GradientColorKey(new Color32(255, 243, 234, 255), 0.5f),
			new GradientColorKey(new Color32(255, 154, 000, 255), 1.0f)
		}
	};

	/// Color of the light that hits the atmosphere.
	/// Left value: Sun at zenith.
	/// Right value: Sun at horizon.
	[Tooltip("Color of the light that hits the atmosphere.\nLeft value: Sun at zenith.\nRight value: Sun at horizon.")]
	public Gradient SkyColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(1.0f, 0.5f),
			new GradientAlphaKey(1.0f, 1.0f)
		},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(255, 243, 234, 255), 0.0f),
			new GradientColorKey(new Color32(255, 243, 234, 255), 0.5f),
			new GradientColorKey(new Color32(255, 243, 234, 255), 1.0f)
		}
	};

	/// Color of the clouds.
	/// Left value: Sun at zenith.
	/// Right value: Sun at horizon.
	[Tooltip("Color of the clouds.\nLeft value: Sun at zenith.\nRight value: Sun at horizon.")]
	public Gradient CloudColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(1.0f, 0.5f),
			new GradientAlphaKey(1.0f, 1.0f)
		},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(255, 255, 255, 255), 0.0f),
			new GradientColorKey(new Color32(255, 255, 255, 255), 0.5f),
			new GradientColorKey(new Color32(255, 195, 145, 255), 1.0f)
		}
	};

	/// Color of the atmosphere fog.
	/// Left value: Sun at zenith.
	/// Right value: Sun at horizon.
	[Tooltip("Color of the atmosphere fog.\nLeft value: Sun at zenith.\nRight value: Sun at horizon.")]
	public Gradient FogColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(1.0f, 0.5f),
			new GradientAlphaKey(1.0f, 1.0f)
		},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(191, 191, 191, 255), 0.0f),
			new GradientColorKey(new Color32(191, 191, 191, 255), 0.5f),
			new GradientColorKey(new Color32(127, 127, 127, 255), 1.0f)
		}
	};

	/// Color of the ambient light.
	/// Left value: Sun at zenith.
	/// Right value: Sun at horizon.
	[Tooltip("Color of the ambient light.\nLeft value: Sun at zenith.\nRight value: Sun at horizon.")]
	public Gradient AmbientColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(1.0f, 0.5f),
			new GradientAlphaKey(1.0f, 1.0f)
		},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(094, 089, 087, 255), 0.0f),
			new GradientColorKey(new Color32(094, 089, 087, 255), 0.5f),
			new GradientColorKey(new Color32(094, 089, 087, 255), 1.0f)
		}
	};

	/// [0, 8]
	/// Intensity of the light source.
	[Tooltip("Intensity of the light source.")]
	[Range(0f, 8f)] public float LightIntensity = 1.0f;

	/// [0, 1]
	/// Opacity of the shadows dropped by the light source.
	[Tooltip("Opacity of the shadows dropped by the light source.")]
	[Range(0f, 1f)] public float ShadowStrength = 1.0f;

	/// [0, 1]
	/// Brightness multiplier of the ambient light.
	[Tooltip("Brightness multiplier of the ambient light.")]
	[Range(0f, 8f)] public float AmbientMultiplier = 1.0f;

	/// [0, 1]
	/// Brightness multiplier of the reflection probe.
	[Tooltip("Brightness multiplier of the reflection probe.")]
	[Range(0f, 1f)] public float ReflectionMultiplier = 1.0f;
}

/// Parameters that are unique to the night.
/// This is the type of the TOD_Sky.Night inspector variable group.
[Serializable] public class TOD_NightParameters
{
	/// Color of the moon mesh.
	/// Left value: Sun at horizon.
	/// Right value: Sun opposite to zenith.
	[Tooltip("Color of the moon mesh.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
	public Gradient MoonColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(1.0f, 0.5f),
			new GradientAlphaKey(1.0f, 1.0f)
		},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(255, 233, 200, 255), 0.0f),
			new GradientColorKey(new Color32(255, 233, 200, 255), 0.5f),
			new GradientColorKey(new Color32(255, 233, 200, 255), 1.0f)
		}
	};

	/// Color of the light that hits the ground.
	/// Left value: Sun at horizon.
	/// Right value: Sun opposite to zenith.
	[Tooltip("Color of the light that hits the ground.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
	public Gradient LightColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(1.0f, 0.5f),
			new GradientAlphaKey(1.0f, 1.0f)
		},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(025, 040, 065, 255), 0.0f),
			new GradientColorKey(new Color32(025, 040, 065, 255), 0.5f),
			new GradientColorKey(new Color32(025, 040, 065, 255), 1.0f)
		}
	};

	/// Color of the god rays.
	/// Left value: Sun at horizon.
	/// Right value: Sun opposite to zenith.
	[Tooltip("Color of the god rays.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
	public Gradient RayColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(0.2f, 0.5f),
			new GradientAlphaKey(0.2f, 1.0f)
		},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(025, 040, 065, 255), 0.0f),
			new GradientColorKey(new Color32(025, 040, 065, 255), 0.5f),
			new GradientColorKey(new Color32(025, 040, 065, 255), 1.0f)
		}
	};

	/// Color of the light that hits the atmosphere.
	/// Left value: Sun at horizon.
	/// Right value: Sun opposite to zenith.
	[Tooltip("Color of the light that hits the atmosphere.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
	public Gradient SkyColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(0.2f, 0.5f),
			new GradientAlphaKey(0.2f, 1.0f)
		},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(025, 040, 065, 255), 0.0f),
			new GradientColorKey(new Color32(025, 040, 065, 255), 0.5f),
			new GradientColorKey(new Color32(025, 040, 065, 255), 1.0f)
		}
	};

	/// Color of the clouds.
	/// Left value: Sun at horizon.
	/// Right value: Sun opposite to zenith.
	[Tooltip("Color of the clouds.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
	public Gradient CloudColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(0.1f, 0.5f),
			new GradientAlphaKey(0.1f, 1.0f)
			},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(025, 040, 065, 255), 0.0f),
			new GradientColorKey(new Color32(025, 040, 065, 255), 0.5f),
			new GradientColorKey(new Color32(025, 040, 065, 255), 1.0f)
		}
	};

	/// Color of the atmosphere fog.
	/// Left value: Sun at horizon.
	/// Right value: Sun opposite to zenith.
	[Tooltip("Color of the atmosphere fog.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
	public Gradient FogColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(0.2f, 0.5f),
			new GradientAlphaKey(0.2f, 1.0f)
		},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(025, 040, 065, 255), 0.0f),
			new GradientColorKey(new Color32(025, 040, 065, 255), 0.5f),
			new GradientColorKey(new Color32(025, 040, 065, 255), 1.0f)
		}
	};

	/// Color of the ambient light.
	/// Left value: Sun at horizon.
	/// Right value: Sun opposite to zenith.
	[Tooltip("Color of the ambient light.\nLeft value: Sun at horizon.\nRight value: Sun opposite to zenith.")]
	public Gradient AmbientColor = new Gradient()
	{
		alphaKeys = new GradientAlphaKey[] {
			new GradientAlphaKey(1.0f, 0.0f),
			new GradientAlphaKey(0.2f, 0.5f),
			new GradientAlphaKey(0.2f, 1.0f)
		},
		colorKeys = new GradientColorKey[] {
			new GradientColorKey(new Color32(025, 040, 065, 255), 0.0f),
			new GradientColorKey(new Color32(025, 040, 065, 255), 0.5f),
			new GradientColorKey(new Color32(025, 040, 065, 255), 1.0f)
		}
	};

	/// [0, 8]
	/// Intensity of the light source.
	[Tooltip("Intensity of the light source.")]
	[Range(0f, 8f)] public float LightIntensity = 0.1f;

	/// [0, 1]
	/// Opacity of the shadows dropped by the light source.
	[Tooltip("Opacity of the shadows dropped by the light source.")]
	[Range(0f, 1f)] public float ShadowStrength = 1.0f;

	/// [0, 1]
	/// Brightness multiplier of the ambient light.
	[Tooltip("Brightness multiplier of the ambient light.")]
	[Range(0f, 8f)] public float AmbientMultiplier = 1.0f;

	/// [0, 1]
	/// Brightness multiplier of the reflection probe.
	[Tooltip("Brightness multiplier of the reflection probe.")]
	[Range(0f, 1f)] public float ReflectionMultiplier = 1.0f;
}

/// Parameters that are unique to the sun.
/// This is the type of the TOD_Sky.Sun inspector variable group.
[Serializable] public class TOD_SunParameters
{
	/// [0, &infin;]
	/// Diameter of the sun in degrees.
	/// The diameter as seen from earth is 0.5 degrees.
	[Tooltip("Diameter of the sun in degrees.\nThe diameter as seen from earth is 0.5 degrees.")]
	[TOD_Min(0f)] public float MeshSize = 1.0f;

	/// [0, &infin;]
	/// Brightness of the sun.
	[Tooltip("Brightness of the sun.")]
	[TOD_Min(0f)] public float MeshBrightness = 2.0f;

	/// [0, &infin;]
	/// Contrast of the sun.
	[Tooltip("Contrast of the sun.")]
	[TOD_Min(0f)] public float MeshContrast = 1.0f;
}

/// Parameters that are unique to the moon.
/// This is the type of the TOD_Sky.Moon inspector variable group.
[Serializable] public class TOD_MoonParameters
{
	/// [0, &infin;]
	/// Diameter of the moon in degrees.
	/// The diameter as seen from earth is 0.5 degrees.
	[Tooltip("Diameter of the moon in degrees.\nThe diameter as seen from earth is 0.5 degrees.")]
	[TOD_Min(0f)] public float MeshSize = 1.0f;

	/// [0, &infin;]
	/// Brightness of the moon.
	[Tooltip("Brightness of the moon.")]
	[TOD_Min(0f)] public float MeshBrightness = 2.0f;

	/// [0, &infin;]
	/// Contrast of the moon.
	[Tooltip("Contrast of the moon.")]
	[TOD_Min(0f)] public float MeshContrast = 1.0f;

	/// [0, &infin;]
	/// Size of the moon halo.
	[Tooltip("Size of the moon halo.")]
	[TOD_Min(0f)] public float HaloSize = 0.1f;

	/// [0, &infin;]
	/// Brightness of the moon halo.
	[Tooltip("Brightness of the moon halo.")]
	[TOD_Min(0f)] public float HaloBrightness = 1.0f;

	/// Type of the moon position calculation.
	[Tooltip("Type of the moon position calculation.")]
	public TOD_MoonPositionType Position = TOD_MoonPositionType.Realistic;
}

/// Parameters of the stars.
/// This is the type of the TOD_Sky.Stars inspector variable group.
[Serializable] public class TOD_StarParameters
{
	/// [0, &infin;]
	/// Size of the stars.
	[Tooltip("Size of the stars.")]
	[TOD_Min(0f)] public float Size = 1.0f;

	/// [0, &infin;]
	/// Brightness of the stars.
	[Tooltip("Brightness of the stars.")]
	[TOD_Min(0f)] public float Brightness = 1.0f;

	/// Type of the stars position calculation.
	[Tooltip("Type of the stars position calculation.")]
	public TOD_StarsPositionType Position = TOD_StarsPositionType.Rotating;
}

/// Parameters of the clouds.
/// This is the type of the TOD_Sky.Clouds inspector variable group.
[Serializable] public class TOD_CloudParameters
{
	/// [1, &infin;]
	/// Size of the clouds.
	[Tooltip("Size of the clouds.")]
	[TOD_Min(1f)] public float Size = 2.0f;

	/// [0, 1]
	/// Opacity of the clouds.
	[Tooltip("Opacity of the clouds.")]
	[TOD_Range(0f, 1f)] public float Opacity = 1.0f;

	/// [0, 1]
	/// How much sky is covered by clouds.
	[Tooltip("How much sky is covered by clouds.")]
	[TOD_Range(0f, 1f)] public float Coverage = 0.5f;

	/// [0, 1]
	/// Sharpness of the cloud to sky transition.
	[Tooltip("Sharpness of the cloud to sky transition.")]
	[TOD_Range(0f, 1f)] public float Sharpness = 0.5f;

	/// [0, 1]
	/// Coloring of the clouds.
	[Tooltip("Coloring of the clouds.")]
	[TOD_Range(0f, 1f)] public float Coloring = 0.5f;

	/// [0, 1]
	/// Amount of skylight that is blocked.
	[Tooltip("Amount of skylight that is blocked.")]
	[TOD_Range(0f, 1f)] public float Attenuation = 0.5f;

	/// [0, 1]
	/// Amount of sunlight that is blocked.
	/// Only affects the highest cloud quality setting.
	[Tooltip("Amount of sunlight that is blocked.\nOnly affects the highest cloud quality setting.")]
	[TOD_Range(0f, 1f)] public float Saturation = 0.5f;

	/// [0, &infin;]
	/// Intensity of the cloud translucency glow.
	/// Only affects the highest cloud quality setting.
	[Tooltip("Intensity of the cloud translucency glow.\nOnly affects the highest cloud quality setting.")]
	[TOD_Min(0f)] public float Scattering = 1.0f;

	/// [0, &infin;]
	/// Brightness of the clouds.
	[Tooltip("Brightness of the clouds.")]
	[TOD_Min(0f)] public float Brightness = 1.5f;
}

/// Parameters of the light source.
/// This is the type of the TOD_Sky.Light inspector variable group.
[Serializable] public class TOD_LightParameters
{
	/// [0, &infin;]
	/// Refresh interval of the light source position in seconds.
	[Tooltip("Refresh interval of the light source position in seconds.")]
	[TOD_Min(0f)] public float UpdateInterval = 0.0f;

	/// [-1, 1]
	/// Controls how low the light source is allowed to go.
	/// \n = -1 light source can go as low as it wants.
	/// \n = 0 light source will never go below the horizon.
	/// \n = +1 light source will never leave zenith.
	[Tooltip("Controls how low the light source is allowed to go.\n = -1 light source can go as low as it wants.\n = 0 light source will never go below the horizon.\n = +1 light source will never leave zenith.")]
	[TOD_Range(-1f, 1f)] public float MinimumHeight = 0.0f;
}

/// Parameters of the fog mode.
/// This is the type of the TOD_Sky.Fog inspector variable group.
[Serializable] public class TOD_FogParameters
{
	/// Fog color mode.
	[Tooltip("Fog color mode.")]
	public TOD_FogType Mode = TOD_FogType.Atmosphere;

	/// [0, 1]
	/// Fog color sampling height.
	/// \n = 0 fog is atmosphere color at horizon.
	/// \n = 1 fog is atmosphere color at zenith.
	[Tooltip("Fog color sampling height.\n = 0 fog is atmosphere color at horizon.\n = 1 fog is atmosphere color at zenith.")]
	[TOD_Range(0f, 1f)] public float HeightBias = 0.0f;
}

/// Parameters of the ambient mode.
/// This is the type of the TOD_Sky.Ambient inspector variable group.
[Serializable] public class TOD_AmbientParameters
{
	/// Ambient light mode.
	[Tooltip("Ambient light mode.")]
	public TOD_AmbientType Mode = TOD_AmbientType.Color;

	/// Saturation of the ambient light.
	[Tooltip("Saturation of the ambient light.")]
	[TOD_Min(0f)] public float Saturation = 1.0f;

	/// Refresh interval of the ambient light probe in seconds.
	[Tooltip("Refresh interval of the ambient light probe in seconds.")]
	[TOD_Min(0f)] public float UpdateInterval = 1.0f;
}

/// Parameters of the reflection mode.
/// This is the type of the TOD_Sky.Reflection inspector variable group.
[Serializable] public class TOD_ReflectionParameters
{
	/// Reflection probe mode.
	[Tooltip("Reflection probe mode.")]
	public TOD_ReflectionType Mode = TOD_ReflectionType.None;

	#if !UNITY_4

	/// Clear flags to use for the reflection.
	[Tooltip("Clear flags to use for the reflection.")]
	public ReflectionProbeClearFlags ClearFlags = ReflectionProbeClearFlags.Skybox;

	/// Layers to include in the reflection.
	[Tooltip("Layers to include in the reflection.")]
	public LayerMask CullingMask = 0;

	/// Time slicing behaviour to spread out rendering cost over multiple frames.
	[Tooltip("Time slicing behaviour to spread out rendering cost over multiple frames.")]
	public ReflectionProbeTimeSlicingMode TimeSlicing = ReflectionProbeTimeSlicingMode.AllFacesAtOnce;

	/// Resolution of the reflection bake.
	[Tooltip("Resolution of the reflection bake.")]
	[TOD_Range(16, 2048)] public int Resolution = 128;

	#endif

	/// Refresh interval of the reflection cubemap in seconds.
	[Tooltip("Refresh interval of the reflection cubemap in seconds.")]
	[TOD_Min(0f)] public float UpdateInterval = 1.0f;
}
