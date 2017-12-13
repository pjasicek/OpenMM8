#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_4
#endif

using UnityEngine;

public abstract class TOD_Particle : MonoBehaviour
{
	private ParticleSystem particleComponent;

	protected float GetEmission()
	{
		if (particleComponent)
		{
			#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			return particleComponent.emissionRate;
			#elif UNITY_5_3 || UNITY_5_4
			return particleComponent.emission.rate.curveScalar;
			#else
			return particleComponent.emission.rateOverTimeMultiplier;
			#endif
		}
		else
		{
			return 0;
		}
	}

	protected void SetEmission(float value)
	{
		if (particleComponent)
		{
			#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			particleComponent.emissionRate = value;
			#elif UNITY_5_3 || UNITY_5_4
			var emission = particleComponent.emission;
			var rate = emission.rate;
			rate.curveScalar = value;
			emission.rate = rate;
			#else
			var emission = particleComponent.emission;
			emission.rateOverTimeMultiplier = value;
			#endif
		}
	}

	protected void Awake()
	{
		particleComponent = GetComponent<ParticleSystem>();
	}
}
