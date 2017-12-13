#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_4
#endif

using UnityEngine;

/// Image effect base class.
///
/// Based on PostEffectsBase from the Unity Standard Assets.
/// Extended for image effects that depend on a TOD_Sky reference.

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public abstract class TOD_ImageEffect : MonoBehaviour
{
	/// Sky dome reference inspector variable.
	/// Will automatically be searched in the scene if not set in the inspector.
	public TOD_Sky sky = null;

	protected Camera cam = null;

	protected Material CreateMaterial(Shader shader)
	{
		if (!shader)
		{
			Debug.Log("Missing shader in " + this.ToString());
			enabled = false;
			return null;
		}

		if (!shader.isSupported)
		{
			Debug.LogError("The shader " + shader.ToString() + " on effect " + this.ToString() + " is not supported on this platform!");
			enabled = false;
			return null;
		}

		var material = new Material(shader);
		material.hideFlags = HideFlags.DontSave;

		return material;
	}

	private TOD_Sky FindSky(bool fallback = false)
	{
		if (TOD_Sky.Instance) return TOD_Sky.Instance;
		if (fallback) return FindObjectOfType(typeof(TOD_Sky)) as TOD_Sky;
		return null;
	}

	protected void Awake()
	{
		if (!cam) cam = GetComponent<Camera>();
		if (!sky) sky = FindSky(true);
	}

	protected bool CheckSupport(bool needDepth = false, bool needHdr = false)
	{
		if (!cam) cam = GetComponent<Camera>();
		if (!cam) return false;

		if (!sky) sky = FindSky();
		if (!sky || !sky.Initialized) return false;

		if (!SystemInfo.supportsImageEffects)
		{
			Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
			enabled = false;
			return false;
		}

		#if !UNITY_5_5_OR_NEWER
		if (!SystemInfo.supportsRenderTextures)
		{
			Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
			enabled = false;
			return false;
		}
		#endif

		if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it requires a depth texture.");
			enabled = false;
			return false;
		}

		if (needHdr && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
		{
			Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it requires HDR.");
			enabled = false;
			return false;
		}

		if (needDepth)
		{
			cam.depthTextureMode |= DepthTextureMode.Depth;
		}

		if (needHdr)
		{
			#if UNITY_5_6_OR_NEWER
			cam.allowHDR = true;
			#else
			cam.hdr = true;
			#endif
		}

		return true;
	}

	protected void DrawBorder(RenderTexture dest, Material material)
	{
		float x1;
		float x2;
		float y1;
		float y2;

		RenderTexture.active = dest;
		bool invertY = true; // source.texelSize.y < 0.0f;

		// Set up the simple Matrix
		GL.PushMatrix();
		GL.LoadOrtho();

		for (int i = 0; i < material.passCount; i++)
		{
			material.SetPass(i);

			float y1_;
			float y2_;
			if (invertY)
			{
				y1_ = 1.0f; y2_ = 0.0f;
			}
			else
			{
				y1_ = 0.0f; y2_ = 1.0f;
			}

			// Left
			x1 = 0.0f;
			x2 = 0.0f + 1.0f / (dest.width*1.0f);
			y1 = 0.0f;
			y2 = 1.0f;
			GL.Begin(GL.QUADS);

			GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
			GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
			GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

			// Right
			x1 = 1.0f - 1.0f / (dest.width*1.0f);
			x2 = 1.0f;
			y1 = 0.0f;
			y2 = 1.0f;

			GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
			GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
			GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

			// Top
			x1 = 0.0f;
			x2 = 1.0f;
			y1 = 0.0f;
			y2 = 0.0f + 1.0f / (dest.height*1.0f);

			GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
			GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
			GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

			// Bottom
			x1 = 0.0f;
			x2 = 1.0f;
			y1 = 1.0f - 1.0f / (dest.height*1.0f);
			y2 = 1.0f;

			GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
			GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
			GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

			GL.End();
		}

		GL.PopMatrix();
	}

	#if UNITY_5_5_OR_NEWER
	private static Vector3[] frustumCornersArray = new Vector3[4];
	#endif

	protected Matrix4x4 FrustumCorners()
	{
		#if UNITY_5_5_OR_NEWER

		cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, cam.stereoActiveEye, frustumCornersArray);

		var bottomLeft = cam.transform.TransformVector(frustumCornersArray[0]);
		var topLeft = cam.transform.TransformVector(frustumCornersArray[1]);
		var topRight = cam.transform.TransformVector(frustumCornersArray[2]);
		var bottomRight = cam.transform.TransformVector(frustumCornersArray[3]);

		#else

		float CAMERA_NEAR = cam.nearClipPlane;
		float CAMERA_FAR = cam.farClipPlane;
		float CAMERA_FOV = cam.fieldOfView;
		float CAMERA_ASPECT_RATIO = cam.aspect;

		var forward = cam.transform.forward;
		var right = cam.transform.right;
		var up = cam.transform.up;

		float fovWHalf = CAMERA_FOV * 0.5f;

		var toRight = right * CAMERA_NEAR * Mathf.Tan (fovWHalf * Mathf.Deg2Rad) * CAMERA_ASPECT_RATIO;
		var toTop = up * CAMERA_NEAR * Mathf.Tan (fovWHalf * Mathf.Deg2Rad);

		var topLeft = (forward * CAMERA_NEAR - toRight + toTop);
		float CAMERA_SCALE = topLeft.magnitude * CAMERA_FAR/CAMERA_NEAR;

		topLeft.Normalize();
		topLeft *= CAMERA_SCALE;

		var topRight = (forward * CAMERA_NEAR + toRight + toTop);
		topRight.Normalize();
		topRight *= CAMERA_SCALE;

		var bottomRight = (forward * CAMERA_NEAR + toRight - toTop);
		bottomRight.Normalize();
		bottomRight *= CAMERA_SCALE;

		var bottomLeft = (forward * CAMERA_NEAR - toRight - toTop);
		bottomLeft.Normalize();
		bottomLeft *= CAMERA_SCALE;

		#endif

		Matrix4x4 frustumCornersMatrix = Matrix4x4.identity;

		frustumCornersMatrix.SetRow(0, bottomLeft);
		frustumCornersMatrix.SetRow(1, bottomRight);
		frustumCornersMatrix.SetRow(2, topLeft);
		frustumCornersMatrix.SetRow(3, topRight);

		return frustumCornersMatrix;
	}

	protected RenderTexture GetSkyMask(RenderTexture source, Material skyMaskMaterial, Material screenClearMaterial, ResolutionType resolution, Vector3 lightPos, int blurIterations, float blurRadius, float maxRadius)
	{
		const int PASS_RADIAL  = 0;
		const int PASS_DEPTH   = 1;
		const int PASS_NODEPTH = 2;

		// Selected resolution
		int width, height, depth;
		if (resolution == ResolutionType.High)
		{
			width  = source.width;
			height = source.height;
			depth  = 0;
		}
		else if (resolution == ResolutionType.Normal)
		{
			width  = source.width / 2;
			height = source.height / 2;
			depth  = 0;
		}
		else
		{
			width  = source.width / 4;
			height = source.height / 4;
			depth  = 0;
		}

		RenderTexture buffer1 = RenderTexture.GetTemporary(width, height, depth);
		RenderTexture buffer2 = null; // Will be allocated later

		skyMaskMaterial.SetVector("_BlurRadius4", new Vector4(1.0f, 1.0f, 0.0f, 0.0f) * blurRadius);
		skyMaskMaterial.SetVector("_LightPosition", new Vector4(lightPos.x, lightPos.y, lightPos.z, maxRadius));

		// Create blocker mask
		if ((cam.depthTextureMode & DepthTextureMode.Depth) != 0)
		{
			Graphics.Blit(source, buffer1, skyMaskMaterial, PASS_DEPTH);
		}
		else
		{
			Graphics.Blit(source, buffer1, skyMaskMaterial, PASS_NODEPTH);
		}

		// Paint a small black border to get rid of clamping problems
		#if UNITY_5_5_OR_NEWER
		if (cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Mono)
		#endif
		{
			DrawBorder(buffer1, screenClearMaterial);
		}

		// Radial blur
		{
			float ofs = blurRadius * (1.0f / 768.0f);
			skyMaskMaterial.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
			skyMaskMaterial.SetVector("_LightPosition", new Vector4(lightPos.x, lightPos.y, lightPos.z, maxRadius));

			for (int i = 0; i < blurIterations; i++ )
			{
				// Each iteration takes 2 * 6 samples
				// We update _BlurRadius each time to cheaply get a very smooth look

				buffer2 = RenderTexture.GetTemporary(width, height, depth);
				Graphics.Blit(buffer1, buffer2, skyMaskMaterial, PASS_RADIAL);
				RenderTexture.ReleaseTemporary(buffer1);

				ofs = blurRadius * (((i * 2.0f + 1.0f) * 6.0f)) / 768.0f;
				skyMaskMaterial.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f) );

				buffer1 = RenderTexture.GetTemporary(width, height, depth);
				Graphics.Blit(buffer2, buffer1, skyMaskMaterial, PASS_RADIAL);
				RenderTexture.ReleaseTemporary(buffer2);

				ofs = blurRadius * (((i * 2.0f + 2.0f) * 6.0f)) / 768.0f;
				skyMaskMaterial.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f) );
			}
		}

		return buffer1;
	}

	/// Image effect resolutions.
	public enum ResolutionType
	{
		Low,
		Normal,
		High,
	}
}
