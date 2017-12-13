using UnityEngine;

public abstract class TOD_Render : MonoBehaviour
{
	private Renderer rendererComponent;

	protected void SetState(bool value)
	{
		if (rendererComponent) rendererComponent.enabled = value;
	}

	protected void Awake()
	{
		rendererComponent = GetComponent<Renderer>();
	}
}
