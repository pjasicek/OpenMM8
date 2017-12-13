using UnityEngine;

public class TOD_AudioAtTime : TOD_Audio
{
	public AnimationCurve Volume = new AnimationCurve() {
		keys = new Keyframe[] { new Keyframe(0, 0), new Keyframe(12, 1), new Keyframe(24, 0) }
	};

	protected void Update()
	{
		SetVolume(Volume.Evaluate(TOD_Sky.Instance.Cycle.Hour));
	}
}
