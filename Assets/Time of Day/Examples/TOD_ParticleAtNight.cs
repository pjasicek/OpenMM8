using UnityEngine;

public class TOD_ParticleAtNight : TOD_Particle
{
	public  float fadeTime = 1;
	private float lerpTime = 0;

	private float maxEmission;

	protected void Start()
	{
		maxEmission = GetEmission();
		SetEmission(TOD_Sky.Instance.IsNight ? maxEmission : 0);
	}

	protected void Update()
	{
		int sign = (TOD_Sky.Instance.IsNight) ? +1 : -1;
		lerpTime = Mathf.Clamp01(lerpTime + sign * Time.deltaTime / fadeTime);

		SetEmission(Mathf.Lerp(0, maxEmission, lerpTime));
	}
}
