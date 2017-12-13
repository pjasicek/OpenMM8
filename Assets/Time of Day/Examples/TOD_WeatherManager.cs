#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_4
#endif

using UnityEngine;

public class TOD_WeatherManager : MonoBehaviour
{
	public enum RainType
	{
		None,
		Light,
		Heavy
	}

	public enum CloudType
	{
		None,
		Few,
		Scattered,
		Broken,
		Overcast
	}

	public enum AtmosphereType
	{
		Clear,
		Storm,
		Dust,
		Fog
	}

	public ParticleSystem RainParticleSystem = null;

	public float FadeTime = 10f;

	public RainType       Rain       = default(RainType);
	public CloudType      Clouds     = default(CloudType);
	public AtmosphereType Atmosphere = default(AtmosphereType);

	private float cloudOpacityMax;
	private float cloudBrightnessMax;
	private float atmosphereBrightnessMax;
	private float rainEmissionMax;

	private float cloudOpacity;
	private float cloudCoverage;
	private float cloudBrightness;
	private float atmosphereFog;
	private float atmosphereBrightness;
	private float rainEmission;

	private float GetRainEmission()
	{
		if (RainParticleSystem)
		{
			#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			return RainParticleSystem.emissionRate;
			#elif UNITY_5_3 || UNITY_5_4
			return RainParticleSystem.emission.rate.curveScalar;
			#else
			return RainParticleSystem.emission.rateOverTimeMultiplier;
			#endif
		}
		else
		{
			return 0;
		}
	}

	private void SetRainEmission(float value)
	{
		if (RainParticleSystem)
		{
			#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			RainParticleSystem.emissionRate = value;
			#elif UNITY_5_3 || UNITY_5_4
			var emission = RainParticleSystem.emission;
			var rate = emission.rate;
			rate.curveScalar = value;
			emission.rate = rate;
			#else
			var emission = RainParticleSystem.emission;
			emission.rateOverTimeMultiplier = value;
			#endif
		}
	}

	protected void Start()
	{
		var sky = TOD_Sky.Instance;

		// Get current values
		cloudOpacity         = sky.Clouds.Opacity;
		cloudCoverage        = sky.Clouds.Coverage;
		cloudBrightness      = sky.Clouds.Brightness;
		atmosphereFog        = sky.Atmosphere.Fogginess;
		atmosphereBrightness = sky.Atmosphere.Brightness;
		rainEmission         = GetRainEmission();

		// Get maximum values
		cloudOpacityMax         = cloudOpacity;
		cloudBrightnessMax      = cloudBrightness;
		atmosphereBrightnessMax = atmosphereBrightness;
		rainEmissionMax         = rainEmission;
	}

	protected void Update()
	{
		var sky = TOD_Sky.Instance;

		// Update rain state
		switch (Rain)
		{
			case RainType.None:
				rainEmission = 0.0f;
				break;

			case RainType.Light:
				rainEmission = rainEmissionMax * 0.5f;
				break;

			case RainType.Heavy:
				rainEmission = rainEmissionMax;
				break;
		}

		// Update cloud state
		switch (Clouds)
		{
			case CloudType.None:
				cloudOpacity  = 0.0f;
				cloudCoverage = 0.0f;
				break;

			case CloudType.Few:
				cloudOpacity  = cloudOpacityMax;
				cloudCoverage = 0.1f;
				break;

			case CloudType.Scattered:
				cloudOpacity  = cloudOpacityMax;
				cloudCoverage = 0.3f;
				break;

			case CloudType.Broken:
				cloudOpacity  = cloudOpacityMax;
				cloudCoverage = 0.6f;
				break;

			case CloudType.Overcast:
				cloudOpacity  = cloudOpacityMax;
				cloudCoverage = 1.0f;
				break;
		}

		// Update atmosphere state
		switch (Atmosphere)
		{
			case AtmosphereType.Clear:
				cloudBrightness      = cloudBrightnessMax;
				atmosphereBrightness = atmosphereBrightnessMax;
				atmosphereFog        = 0.0f;
				break;

			case AtmosphereType.Storm:
				cloudBrightness      = cloudBrightnessMax * 0.3f;
				atmosphereBrightness = atmosphereBrightnessMax * 0.5f;
				atmosphereFog        = 1.0f;
				break;

			case AtmosphereType.Dust:
				cloudBrightness      = cloudBrightnessMax;
				atmosphereBrightness = atmosphereBrightnessMax;
				atmosphereFog        = 0.5f;
				break;

			case AtmosphereType.Fog:
				cloudBrightness      = cloudBrightnessMax;
				atmosphereBrightness = atmosphereBrightnessMax;
				atmosphereFog        = 1.0f;
				break;
		}

		// FadeTime is not exact as the fade smoothens a little towards the end
		float t = FadeTime > 0.0f ? Mathf.Clamp01(Time.deltaTime / FadeTime) : 1.0f;

		// Update visuals
		sky.Clouds.Opacity        = Mathf.Lerp(sky.Clouds.Opacity,        cloudOpacity,         t);
		sky.Clouds.Coverage       = Mathf.Lerp(sky.Clouds.Coverage,       cloudCoverage,        t);
		sky.Clouds.Brightness     = Mathf.Lerp(sky.Clouds.Brightness,     cloudBrightness,      t);
		sky.Atmosphere.Fogginess  = Mathf.Lerp(sky.Atmosphere.Fogginess,  atmosphereFog,        t);
		sky.Atmosphere.Brightness = Mathf.Lerp(sky.Atmosphere.Brightness, atmosphereBrightness, t);

		SetRainEmission(Mathf.Lerp(GetRainEmission(), rainEmission, t));
	}
}
