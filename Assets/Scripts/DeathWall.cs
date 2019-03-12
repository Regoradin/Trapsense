using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
	[HideInInspector]
	public AnimationCurve speed_over_time;
	private float start_time;

	public float kill_time;
	private float kill_start_time;

	private void Start()
	{
		start_time = Time.time;
	}

	private void OnTriggerEnter(Collider other)
	{
		Player player = other.GetComponent<Player>();
		if(player != null)
		{
			kill_start_time = Time.time;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		Player player = other.GetComponent<Player>();
		if(player != null)
		{
			if (Time.time - kill_start_time >= kill_time)
			{
				player.Die();
			}
		}
	}

	private void Update()
	{
		float speed = speed_over_time.Evaluate(Time.time - start_time);
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
	}


}
