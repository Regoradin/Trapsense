using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public static Player player;

	//private CharacterController controller;
	private Animator anim;

	public float move_distance;
	public int max_health;
	public int health;

	public int progress = 0;
	public int max_progress = 0;
	public TrapsGenerator trap_gen;
	private int corridor_gen_distance;

	private void Awake()
	{
		player = this;
		//controller = GetComponent<CharacterController>();

		corridor_gen_distance = trap_gen.corridor_height;

		anim = GetComponent<Animator>();
	}

	private void Start()
	{
	Timekeeper.timekeeper.TickEvent += Move;
	health = max_health;
	}

	public void Move (string input)
	{
		if(input == "Forward")
		{
		//	controller.Move(Vector3.forward * move_distance);
			progress++;
			if(progress > max_progress)
			{
				max_progress = progress;
				if(max_progress % corridor_gen_distance == 1) //Generates next segment of corridor one space into the current segment
				{
					trap_gen.CreateCorridor(max_progress + corridor_gen_distance);
				}
			}
		}
		if(input == "Back")
		{
			progress--;
		}

		anim.SetTrigger(input);
	}


	public void Damage(int damage)
	{
		health -= damage;
	}


}
