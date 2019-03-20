using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireFlicker : MonoBehaviour
{
	private ParticleSystem fire;
	public float flicker_time;

	[Range(0, 1)]
	public float max_brightness;
	[Range(0, 1)]
	public float min_brightness;

	private float last_target;
	private float target;

	private void Start()
	{
		fire = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		var lights = fire.lights;

		if (Time.time % flicker_time <= 0.2)
		{
			last_target = target;
			target = Random.Range(min_brightness, max_brightness);
		}
		else
		{
			lights.ratio = Mathf.Lerp(last_target, target, (Time.time % flicker_time)/flicker_time);
		}

	}

}
