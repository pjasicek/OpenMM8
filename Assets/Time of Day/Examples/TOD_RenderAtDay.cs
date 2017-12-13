using UnityEngine;

public class TOD_RenderAtDay : TOD_Render
{
	protected void Start()
	{
		SetState(TOD_Sky.Instance.IsDay);
	}

	protected void Update()
	{
		SetState(TOD_Sky.Instance.IsDay);
	}
}
