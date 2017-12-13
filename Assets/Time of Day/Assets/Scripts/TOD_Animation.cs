using UnityEngine;

/// Cloud animation class.
///
/// Component of the sky dome parent game object.

public class TOD_Animation : MonoBehaviour
{
	/// How much to move the clouds when the camera moves.
	[Tooltip("How much to move the clouds when the camera moves.")]
	[TOD_Min(0f)] public float CameraMovement = 1.0f;

	/// Wind direction in degrees.
	/// 0 for wind blowing in northern direction.
	/// 90 for wind blowing in eastern direction.
	/// 180 for wind blowing in southern direction.
	/// 270 for wind blowing in western direction.
	[Tooltip("Wind direction in degrees.")]
	[TOD_Range(0f, 360f)] public float WindDegrees = 0.0f;

	/// Speed of the wind that is acting on the clouds.
	[Tooltip("Speed of the wind that is acting on the clouds.")]
	[TOD_Min(0f)] public float WindSpeed = 1.0f;

	/// Current cloud UV coordinates.
	/// Can be synchronized between multiple game clients to guarantee identical cloud positions.
	public Vector3 CloudUV
	{
		get; set;
	}

	/// Current offset UV coordinates.
	/// Calculated from the sky dome world position.
	public Vector3 OffsetUV
	{
		get
		{
			var pos = transform.position * (CameraMovement * 1e-4f);
			var rot = Quaternion.Euler(0, -transform.rotation.eulerAngles.y, 0);
			return rot * pos;
		}
	}

	private TOD_Sky sky;

	protected void Start()
	{
		sky = GetComponent<TOD_Sky>();

		CloudUV = new Vector3(Random.value, Random.value, Random.value);
	}

	protected void Update()
	{
		float u = Mathf.Sin(Mathf.Deg2Rad * WindDegrees);
		float v = Mathf.Cos(Mathf.Deg2Rad * WindDegrees);

		float time = 1e-3f * Time.deltaTime;
		float wind = WindSpeed * time;

		float x = CloudUV.x;
		float y = CloudUV.y;
		float z = CloudUV.z;

		y += time * 0.1f;
		x -= wind * u;
		z -= wind * v;

		x -= Mathf.Floor(x);
		y -= Mathf.Floor(y);
		z -= Mathf.Floor(z);

		CloudUV = new Vector3(x, y, z);

		sky.Components.BillboardTransform.localRotation = Quaternion.Euler(0, y * 360, 0);
	}
}
