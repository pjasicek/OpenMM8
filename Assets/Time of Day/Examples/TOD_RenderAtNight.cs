using UnityEngine;

public class TOD_RenderAtNight : TOD_Render
{
	protected void Start()
	{
		SetState(TOD_Sky.Instance.IsNight);
	}

	protected void Update()
	{
		SetState(TOD_Sky.Instance.IsNight);
	}
}
