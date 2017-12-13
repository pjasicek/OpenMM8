using UnityEngine;

/// God ray camera component.
///
/// Based on SunShafts from the Unity Standard Assets.
/// Extended to get the god ray color from TOD_Sky and properly handle transparent meshes like clouds.

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Time of Day/Camera God Rays")]
public class TOD_Rays : TOD_ImageEffect
{
	public Shader GodRayShader = null;
	public Shader ScreenClearShader = null;
	public Shader SkyMaskShader = null;

	/// Whether or not to use the depth buffer.
	/// If enabled, requires the target platform to allow the camera to create a depth texture.
	/// Unity always creates this depth texture if deferred lighting is enabled.
	/// Otherwise this script will enable it for the camera it is attached to.
	/// If disabled, requires all shaders writing to the depth buffer to also write to the frame buffer alpha channel.
	/// Only the frame buffer alpha channel will then be used to check for ray blockers in the image effect.
	[Tooltip("Whether or not to use the depth buffer.")]
	public bool UseDepthTexture = true;

	[Header("Rays")]

	/// The god ray rendering blend mode.
	[Tooltip("The god ray rendering blend mode.")]
	public BlendModeType BlendMode = BlendModeType.Screen;

	/// The intensity of the god rays.
	[Tooltip("The intensity of the god rays.")]
	[TOD_Min(0f)] public float Intensity = 1;

	[Header("Blur")]

	/// The god ray rendering resolution.
	[Tooltip("The god ray rendering resolution.")]
	public ResolutionType Resolution = ResolutionType.Normal;

	/// The number of blur iterations to be performed.
	[Tooltip("The number of blur iterations to be performed.")]
	[TOD_Range(0, 4)] public int BlurIterations = 2;

	/// The radius to blur filter applied to the god rays.
	[Tooltip("The radius to blur filter applied to the god rays.")]
	[TOD_Min(0f)] public float BlurRadius = 2;

	/// The maximum radius of the god rays.
	[Tooltip("The maximum radius of the god rays.")]
	[TOD_Min(0f)] public float MaxRadius = 0.5f;

	private Material godRayMaterial = null;
	private Material screenClearMaterial = null;
	private Material skyMaskMaterial = null;

	private const int PASS_SCREEN  = 0;
	private const int PASS_ADD     = 1;

	protected void OnEnable()
	{
		if (!GodRayShader) GodRayShader = Shader.Find("Hidden/Time of Day/God Rays");
		if (!ScreenClearShader) ScreenClearShader = Shader.Find("Hidden/Time of Day/Screen Clear");
		if (!SkyMaskShader) SkyMaskShader = Shader.Find("Hidden/Time of Day/Sky Mask");

		godRayMaterial = CreateMaterial(GodRayShader);
		screenClearMaterial = CreateMaterial(ScreenClearShader);
		skyMaskMaterial = CreateMaterial(SkyMaskShader);
	}

	protected void OnDisable()
	{
		if (godRayMaterial) DestroyImmediate(godRayMaterial);
		if (screenClearMaterial) DestroyImmediate(screenClearMaterial);
		if (skyMaskMaterial) DestroyImmediate(skyMaskMaterial);
	}

	protected void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckSupport(UseDepthTexture))
		{
			Graphics.Blit(source, destination);
			return;
		}

		sky.Components.Rays = this;

		// Light position
		Vector3 lightPos = cam.WorldToViewportPoint(sky.Components.LightTransform.position);

		// Sky mask
		var mask = GetSkyMask(source, skyMaskMaterial, screenClearMaterial, Resolution, lightPos, BlurIterations, BlurRadius, MaxRadius);

		// Blend together
		{
			var color = Color.black;

			if (lightPos.z >= 0.0)
			{
				if (sky.IsDay)
				{
					color = Intensity * sky.SunVisibility * sky.SunRayColor;
				}
				else
				{
					color = Intensity * sky.MoonVisibility * sky.MoonRayColor;
				}
			}

			godRayMaterial.SetColor("_LightColor", color);
			godRayMaterial.SetTexture("_SkyMask", mask);

			if (BlendMode == BlendModeType.Screen)
			{
				Graphics.Blit(source, destination, godRayMaterial, PASS_SCREEN);
			}
			else
			{
				Graphics.Blit(source, destination, godRayMaterial, PASS_ADD);
			}

			RenderTexture.ReleaseTemporary(mask);
		}
	}

	/// Methods to blend the god rays with the image.
	public enum BlendModeType
	{
		Screen,
		Add,
	}
}
