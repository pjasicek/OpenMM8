using UnityEngine;

public abstract class TOD_Audio : MonoBehaviour
{
	private AudioSource audioComponent;

	protected float GetVolume()
	{
		if (audioComponent)
		{
			return audioComponent.volume;
		}
		else
		{
			return 0;
		}
	}

	protected void SetVolume(float value)
	{
		if (audioComponent)
		{
			audioComponent.volume = value;
			audioComponent.enabled = value > 0;
		}
	}

	protected void Awake()
	{
		audioComponent = GetComponent<AudioSource>();
	}
}
