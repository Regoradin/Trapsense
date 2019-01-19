using System;
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
			player.Damage(damage);
			Debug.Log(name + " contacting player and moving " + direction.ToString());
			if (direction.ToString() != "None")
			{
				//Calling player.move() directly will move the player without causing traps to progress another tick
				//Still not sure if that's the behaviour I'm looking for.
				//Timekeeper.timekeeper.CreateTick(direction.ToString());
				player.Move(direction.ToString());
			}
		}
	}
}
