using UnityEngine;

/// Component manager class.
///
/// Component of the main camera of the scene.

[ExecuteInEditMode]
public class TOD_Components : MonoBehaviour
{
	/// Space object.
	public GameObject Space = null;

	/// Star object.
	public GameObject Stars = null;

	/// Sun object.
	public GameObject Sun = null;

	/// Moon object.
	public GameObject Moon = null;

	/// Atmosphere object.
	public GameObject Atmosphere = null;

	/// Clear object.
	public GameObject Clear = null;

	/// Cloud object.
	public GameObject Clouds = null;

	/// Billboard parent.
	public GameObject Billboards = null;

	/// Light object.
	public GameObject Light = null;

	/// Dome transform.
	public Transform DomeTransform { get; set; }

	/// Space transform.
	public Transform SpaceTransform { get; set; }

	/// Star transform.
	public Transform StarTransform { get; set; }

	/// Sun transform.
	public Transform SunTransform { get; set; }

	/// Moon transform.
	public Transform MoonTransform { get; set; }

	/// Atmosphere transform.
	public Transform AtmosphereTransform { get; set; }

	/// Clear transform.
	public Transform ClearTransform { get; set; }

	/// Cloud transform.
	public Transform CloudTransform { get; set; }

	/// Billboard transform.
	public Transform BillboardTransform { get; set; }

	/// Light transform.
	public Transform LightTransform { get; set; }

	/// Space renderer.
	public Renderer SpaceRenderer { get; set; }

	/// Star renderer.
	public Renderer StarRenderer { get; set; }

	/// Sun renderer.
	public Renderer SunRenderer { get; set; }

	/// Moon renderer.
	public Renderer MoonRenderer { get; set; }

	/// Atmosphere renderer.
	public Renderer AtmosphereRenderer { get; set; }

	/// Clear renderer.
	public Renderer ClearRenderer { get; set; }

	/// Cloud renderer.
	public Renderer CloudRenderer { get; set; }

	/// Billboard renderers.
	public Renderer[] BillboardRenderers { get; set; }

	/// Space mesh filter.
	public MeshFilter SpaceMeshFilter { get; set; }

	/// Star mesh filter.
	public MeshFilter StarMeshFilter { get; set; }

	/// Sun mesh filter.
	public MeshFilter SunMeshFilter { get; set; }

	/// Moon mesh filter.
	public MeshFilter MoonMeshFilter { get; set; }

	/// Atmosphere mesh filter.
	public MeshFilter AtmosphereMeshFilter { get; set; }

	/// Clear mesh filter.
	public MeshFilter ClearMeshFilter { get; set; }

	/// Cloud mesh filter.
	public MeshFilter CloudMeshFilter { get; set; }

	/// Billboard mesh filters.
	public MeshFilter[] BillboardMeshFilters { get; set; }

	/// Space material.
	public Material SpaceMaterial { get; set; }

	/// Star material.
	public Material StarMaterial { get; set; }

	/// Sun material.
	public Material SunMaterial { get; set; }

	/// Moon material.
	public Material MoonMaterial { get; set; }

	/// Atmosphere material.
	public Material AtmosphereMaterial { get; set; }

	/// Clear material.
	public Material ClearMaterial { get; set; }

	/// Cloud material.
	public Material CloudMaterial { get; set; }

	/// Billboard materials.
	public Material[] BillboardMaterials { get; set; }

	/// Light source.
	public Light LightSource { get; set; }

	/// Dome sky script.
	public TOD_Sky Sky { get; set; }

	/// Dome animation script.
	public TOD_Animation Animation { get; set; }

	/// Dome time script.
	public TOD_Time Time { get; set; }

	/// Camera script.
	public TOD_Camera Camera { get; set; }

	/// Camera god ray script.
	public TOD_Rays Rays { get; set; }

	/// Camera atmospheric scattering script.
	public TOD_Scattering Scattering { get; set; }

	/// Camera cloud shadow script.
	public TOD_Shadows Shadows { get; set; }

	/// Initializes all component references.
	public void Initialize()
	{
		DomeTransform = GetComponent<Transform>();

		Sky       = GetComponent<TOD_Sky>();
		Animation = GetComponent<TOD_Animation>();
		Time      = GetComponent<TOD_Time>();

		if (Space)
		{
			SpaceTransform  = Space.GetComponent<Transform>();
			SpaceRenderer   = Space.GetComponent<Renderer>();
			SpaceMeshFilter = Space.GetComponent<MeshFilter>();
			SpaceMaterial   = SpaceRenderer.sharedMaterial;
		}

		if (Stars)
		{
			StarTransform  = Stars.GetComponent<Transform>();
			StarRenderer   = Stars.GetComponent<Renderer>();
			StarMeshFilter = Stars.GetComponent<MeshFilter>();
			StarMaterial   = StarRenderer.sharedMaterial;
		}

		if (Sun)
		{
			SunTransform  = Sun.GetComponent<Transform>();
			SunRenderer   = Sun.GetComponent<Renderer>();
			SunMeshFilter = Sun.GetComponent<MeshFilter>();
			SunMaterial   = SunRenderer.sharedMaterial;
		}

		if (Moon)
		{
			MoonTransform  = Moon.GetComponent<Transform>();
			MoonRenderer   = Moon.GetComponent<Renderer>();
			MoonMeshFilter = Moon.GetComponent<MeshFilter>();
			MoonMaterial   = MoonRenderer.sharedMaterial;
		}

		if (Atmosphere)
		{
			AtmosphereTransform  = Atmosphere.GetComponent<Transform>();
			AtmosphereRenderer   = Atmosphere.GetComponent<Renderer>();
			AtmosphereMeshFilter = Atmosphere.GetComponent<MeshFilter>();
			AtmosphereMaterial   = AtmosphereRenderer.sharedMaterial;
		}

		if (Clear)
		{
			ClearTransform  = Clear.GetComponent<Transform>();
			ClearRenderer   = Clear.GetComponent<Renderer>();
			ClearMeshFilter = Clear.GetComponent<MeshFilter>();
			ClearMaterial   = ClearRenderer.sharedMaterial;
		}

		if (Clouds)
		{
			CloudTransform  = Clouds.GetComponent<Transform>();
			CloudRenderer   = Clouds.GetComponent<Renderer>();
			CloudMeshFilter = Clouds.GetComponent<MeshFilter>();
			CloudMaterial   = CloudRenderer.sharedMaterial;
		}

		if (Billboards)
		{
			BillboardTransform   = Billboards.GetComponent<Transform>();
			BillboardRenderers   = Billboards.GetComponentsInChildren<Renderer>();
			BillboardMeshFilters = Billboards.GetComponentsInChildren<MeshFilter>();
			BillboardMaterials   = new Material[BillboardRenderers.Length];
			for (int i = 0; i < BillboardRenderers.Length; i++)
			{
				BillboardMaterials[i] = BillboardRenderers[i].sharedMaterial;
			}
		}

		if (Light)
		{
			LightTransform = Light.GetComponent<Transform>();
			LightSource    = Light.GetComponent<Light>();
		}
	}
}
