using UnityEngine;

/// Atmospheric scattering and aerial perspective camera component.

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Time of Day/Camera Atmospheric Scattering")]
public class TOD_Scattering : TOD_ImageEffect
{
	public Shader ScatteringShader = null;
	public Shader ScreenClearShader = null;
	public Shader SkyMaskShader = null;

	public Texture2D DitheringTexture = null;

	[Tooltip("Whether to render atmosphere and fog in a single pass or two separate passes. Disable when using anti-aliasing in forward rendering or when your manual reflection scripts need the sky dome to be present before the image effects are rendered.")]
	public bool SinglePass = true;

	[Header("Fog")]

	/// How quickly the fog thickens with increasing distance.
	[Tooltip("How quickly the fog thickens with increasing distance.")]
	[Range(0f, 1f)] public float GlobalDensity = 0.01f;

	/// How quickly the fog falls off with increasing altitude.
	[Tooltip("How quickly the fog falls off with increasing altitude.")]
	[Range(0f, 1f)] public float HeightFalloff = 0.01f;

	/// The height where fog reaches its maximum density.
	[Tooltip("The height where fog reaches its maximum density.")]
	public float ZeroLevel = 0.0f;

	[Header("Blur")]

	/// The scattering resolution.
	[Tooltip("The scattering resolution.")]
	public ResolutionType Resolution = ResolutionType.Normal;

	/// The number of blur iterations to be performed.
	[Tooltip("The number of blur iterations to be performed.")]
	[TOD_Range(0, 4)] public int BlurIterations = 2;

	/// The radius to blur filter applied to the directional scattering.
	[Tooltip("The radius to blur filter applied to the directional scattering.")]
	[TOD_Min(0f)] public float BlurRadius = 2;

	/// The maximum radius of the directional scattering.
	[Tooltip("The maximum radius of the directional scattering.")]
	[TOD_Min(0f)] public float MaxRadius = 1;

	private Material scatteringMaterial = null;
	private Material screenClearMaterial = null;
	private Material skyMaskMaterial = null;

	protected void OnEnable()
	{
		if (!ScatteringShader) ScatteringShader = Shader.Find("Hidden/Time of Day/Scattering");
		if (!ScreenClearShader) ScreenClearShader = Shader.Find("Hidden/Time of Day/Screen Clear");
		if (!SkyMaskShader) SkyMaskShader = Shader.Find("Hidden/Time of Day/Sky Mask");

		scatteringMaterial = CreateMaterial(ScatteringShader);
		screenClearMaterial = CreateMaterial(ScreenClearShader);
		skyMaskMaterial = CreateMaterial(SkyMaskShader);
	}

	protected void OnDisable()
	{
		if (scatteringMaterial) DestroyImmediate(scatteringMaterial);
		if (screenClearMaterial) DestroyImmediate(screenClearMaterial);
		if (skyMaskMaterial) DestroyImmediate(skyMaskMaterial);
	}

	protected void OnPreCull()
	{
		if (SinglePass)
		{
			if (sky && sky.Initialized) sky.Components.AtmosphereRenderer.enabled = false;
		}
	}

	protected void OnPostRender()
	{
		if (SinglePass)
		{
			if (sky && sky.Initialized) sky.Components.AtmosphereRenderer.enabled = true;
		}
	}

	[ImageEffectOpaque]
	protected void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckSupport(true))
		{
			Graphics.Blit(source, destination);
			return;
		}

		sky.Components.Scattering = this;

		// Light position
		Vector3 lightPos = cam.WorldToViewportPoint(sky.Components.SunTransform.position);

		// Sky mask
		var mask = GetSkyMask(source, skyMaskMaterial, screenClearMaterial, Resolution, lightPos, BlurIterations, BlurRadius, MaxRadius);

		scatteringMaterial.SetMatrix("_FrustumCornersWS", FrustumCorners());
		scatteringMaterial.SetTexture("_SkyMask", mask);

		if (SinglePass)
		{
			scatteringMaterial.EnableKeyword("TOD_SCATTERING_SINGLE_PASS");
		}
		else
		{
			scatteringMaterial.DisableKeyword("TOD_SCATTERING_SINGLE_PASS");
		}

		Shader.SetGlobalTexture("TOD_BayerTexture", DitheringTexture);
		Shader.SetGlobalVector("TOD_ScatterDensity", new Vector4(HeightFalloff, ZeroLevel, GlobalDensity, 0));

		Graphics.Blit(source, destination, scatteringMaterial);

		RenderTexture.ReleaseTemporary(mask);
	}
}
