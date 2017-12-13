using UnityEngine;

public class TOD_Billboard : MonoBehaviour
{
	public float Altitude = 0;
	public float Azimuth  = 0;
	public float Distance = 1;
	public float Size     = 1;

	private T GetComponentInParents<T>() where T : Component
	{
		var current = transform;
		var component = current.GetComponent<T>();

		while (component == null && current.parent != null)
		{
			current = current.parent;
			component = current.GetComponent<T>();
		}

		return component;
	}

#if UNITY_EDITOR

	protected void OnValidate()
	{
		var sky = GetComponentInParents<TOD_Sky>();

		if (sky == null) return;

		float theta = (90 - Altitude) * Mathf.Deg2Rad;
		float phi = Azimuth * Mathf.Deg2Rad;
		var position = sky.OrbitalToUnity(Distance, theta, phi);

		if (transform.localPosition != position)
		{
			transform.localPosition = position;
		}

		float uniform = 2.0f * Mathf.Tan(4.0f * 0.5f * Mathf.Deg2Rad * Size);
		var scale = new Vector3(uniform, uniform, uniform);

		if (transform.localScale != scale)
		{
			transform.localScale = scale;
		}

		transform.LookAt(sky.transform.position, Vector3.up);
	}

#endif // UNITY_EDITOR
}
