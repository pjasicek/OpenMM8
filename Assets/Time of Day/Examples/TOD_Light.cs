using UnityEngine;

public abstract class TOD_Light : MonoBehaviour
{
	private Light lightComponent;

	protected float GetIntensity()
	{
		if (lightComponent)
		{
			return lightComponent.intensity;
		}
		else
		{
			return 0;
		}
	}

	protected void SetIntensity(float value)
	{
		if (lightComponent)
		{
			lightComponent.intensity = value;
			lightComponent.enabled = value > 0;
		}
	}

	protected void Awake()
	{
		lightComponent = GetComponent<Light>();
	}
}
