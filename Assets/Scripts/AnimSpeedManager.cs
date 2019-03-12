using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSpeedManager : MonoBehaviour
{

	public static AnimSpeedManager instance;
	private List<Animator> anims;

	public float init_speed;
	public float final_speed;
	public float duration;

	public float speed;

	private void Awake()
	{
		instance = this;
		anims = new List<Animator>();
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
		speed = Mathf.Lerp(init_speed, final_speed, Time.time / duration);

		SetAnimSpeed(speed);
	}

}
