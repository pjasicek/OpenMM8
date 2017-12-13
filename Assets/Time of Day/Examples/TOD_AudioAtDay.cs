using UnityEngine;

public class TOD_AudioAtDay : TOD_Audio
{
	public  float fadeTime = 1;
	private float lerpTime = 0;

	private float maxVolume;

	protected void Start()
	{
		maxVolume = GetVolume();
		SetVolume(TOD_Sky.Instance.IsDay ? maxVolume : 0);;
	}

	protected void Update()
	{
		int sign = (TOD_Sky.Instance.IsDay) ? +1 : -1;
		lerpTime = Mathf.Clamp01(lerpTime + sign * Time.deltaTime / fadeTime);

		SetVolume(Mathf.Lerp(0, maxVolume, lerpTime));
	}
}
