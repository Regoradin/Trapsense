﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

	public enum Direction
	{
		None,
		Forward,
		Right,
		Back,
		Left
	}
	public Direction direction;

	public void IncrementDirection(int rotation)
	{
		if (direction != 0)
		{
			int new_direction = (int)direction + rotation;

			while (!Enum.IsDefined(typeof(Direction), new_direction))
			{
				new_direction -= 4;
			}

			direction = (Direction)new_direction;
		}
	}

	private Animator anim;
	public int damage;

	public int height, width;
	public bool needs_edge;

	//This is gross and horrible for a variety of reasons and I'm very sorry to every CS teacher I've ever had
	[Header("Allowed Rotations")]
	public bool zero = true;
	public bool ninety = true;
	public bool one_eighty = true;
	public bool two_seventy = true;


	private void Start()
	{
		//		Timekeeper.timekeeper.TickEvent	+= Tick;
		Player.player.PlayerTickEvent += Tick;
		anim = GetComponent<Animator>();
		RandomizeState();
	}

	private void Tick(string input)
	{
		if (anim != null)
		{
			anim.SetTrigger("Tick");
		}
	}

	public void Activate()
	{
		if (anim != null)
		{
			anim.SetTrigger("Activate");
		}
	}
	/// <summary>
	/// Randomizes the animation state of the
	/// </summary>
	private void RandomizeState()
	{
		//higher number here will do more iterations, producing a wider range of results at the cost of a longer setup time
		if (UnityEngine.Random.Range(0f, 1f) < .85f)
		{
			if (anim != null)
			{
				anim.SetTrigger("Tick");
			}
			Invoke("RandomizeState", .03f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Player player = other.GetComponent<Player>();
		if(player != null)
		{
			player.Damage(damage, direction.ToString());
	//		player.Damage(damage);
//			if (direction.ToString() == "None")
//			{
//				player.Move(direction.ToString());
//			}
		}
	}
}
