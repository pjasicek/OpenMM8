using UnityEngine;

public class TOD_ParticleAtTime : TOD_Particle
{
	public AnimationCurve Emission = new AnimationCurve() {
		keys = new Keyframe[] { new Keyframe(0, 0), new Keyframe(12, 1), new Keyframe(24, 0) }
	};

	protected void Update()
	{
		SetEmission(Emission.Evaluate(TOD_Sky.Instance.Cycle.Hour));
	}
}
