using UnityEngine;
using System.Collections;

public class SwordAnimation : MonoBehaviour {

	//This is just a simple script to play our sword animation.

	string swingSwordAnimationName = "SwingSword";
	string walkAnimationName = "Move";
	string idleAnimationName = "Idle";
	Animation _AnimationComp;

	void Start () 
	{
		_AnimationComp = GetComponent<Animation>();
		swingSwordAnimationName = "SwingSword";
		walkAnimationName = "Move";
		idleAnimationName = "Idle";
	}

	void Update () 
	{
		if (Input.GetMouseButton(0))
		{
			_AnimationComp.CrossFade(swingSwordAnimationName);
		}
	
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
		{
			if (!Input.GetKey(KeyCode.LeftShift) && !_AnimationComp.IsPlaying(swingSwordAnimationName))
			{
				_AnimationComp[walkAnimationName].speed = 0.65f;
				_AnimationComp.CrossFade(walkAnimationName);
			}

			if (Input.GetKey(KeyCode.LeftShift) && !_AnimationComp.IsPlaying(swingSwordAnimationName))
			{
				_AnimationComp[walkAnimationName].speed = 1.0f;
				_AnimationComp.CrossFade(walkAnimationName);
			}
		}
		else
		{
			if (!_AnimationComp.IsPlaying(swingSwordAnimationName))
			{
				_AnimationComp[idleAnimationName].speed = 0.5f;
				_AnimationComp.CrossFade(idleAnimationName);
			}
		}

	}
}
