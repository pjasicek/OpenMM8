using UnityEngine;

public class TOD_LightAtNight : TOD_Light
{
	public  float fadeTime = 1;
	private float lerpTime = 0;

	private float maxIntensity;

	protected void Start()
	{
		maxIntensity = GetIntensity();
		SetIntensity(TOD_Sky.Instance.IsNight ? maxIntensity : 0);
	}

	protected void Update()
	{
		int sign = (TOD_Sky.Instance.IsNight) ? +1 : -1;
		lerpTime = Mathf.Clamp01(lerpTime + sign * Time.deltaTime / fadeTime);

		SetIntensity(Mathf.Lerp(0, maxIntensity, lerpTime));
	}
}
