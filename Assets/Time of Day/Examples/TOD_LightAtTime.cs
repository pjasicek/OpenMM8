using UnityEngine;

public class TOD_LightAtTime : TOD_Light
{
	public AnimationCurve Intensity = new AnimationCurve() {
		keys = new Keyframe[] { new Keyframe(0, 0), new Keyframe(12, 1), new Keyframe(24, 0) }
	};

	protected void Update()
	{
		SetIntensity(Intensity.Evaluate(TOD_Sky.Instance.Cycle.Hour));
	}
}
