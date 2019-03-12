using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSpeedManager : MonoBehaviour
{

	public static AnimSpeedManager instance;
	private List<Animator> anims;

	[HideInInspector]
	public AnimationCurve anim_speed_over_time;
	private float start_time;

	private void Awake()
	{
		instance = this;
		anims = new List<Animator>();

		start_time = Time.time;
	}

	public void RegisterAnimator(Animator anim)
	{
		if (!anims.Contains(anim))
		{
			anims.Add(anim);
		}
	}

	public void SetAnimSpeed(float speed)
	{
		foreach(Animator anim in anims)
		{
			anim.speed = speed;
		}
	}

	private void Update()
	{
		float speed = anim_speed_over_time.Evaluate(Time.time - start_time);
		SetAnimSpeed(speed);
	}

}
