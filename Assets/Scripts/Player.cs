using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public static Player player;

	//private CharacterController controller;
	private Animator anim;

	public float move_distance;
	public int max_health;
	public int health;

	public int progress = 1;
	public int max_progress = 1;
	public TrapsGenerator trap_gen;
	private int corridor_gen_distance;

	private int horizontal_coord = 4; //This is based off the starting position of the player relative to the corridor generation, and will almost certainly need to be changed

	public delegate void Tick(string input);
	public event Tick PlayerTickEvent;

	public Text health_text;

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
		health_text.text = health.ToString();

	}

	public void Move (string input)
	{
		bool is_valid_move = true;

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

		if(input == "Left")
		{
			horizontal_coord -= 1;
			if(horizontal_coord < 0)
			{
				horizontal_coord += 1;
				is_valid_move = false;
			}
		}
		if(input == "Right")
		{
			horizontal_coord += 1;
			if(horizontal_coord >= trap_gen.corridor_width)
			{
				horizontal_coord -= 1;
				is_valid_move = false;
			}
		}

		if (input != "Wait" && is_valid_move)
		{
			anim.SetTrigger(input);
		}
		else
		{
			EndMove();
		}
	}

	public void EndMove()
	{
		if(PlayerTickEvent != null)
		{
			PlayerTickEvent("");
		}
		Debug.Log("Setting pos to " + horizontal_coord + " " + progress);
		transform.position = new Vector3(horizontal_coord, 0, progress);
	}


	public void Damage(int damage)
	{
		health -= damage;
		health_text.text = health.ToString();
		anim.SetTrigger("Damage");
	}

	public void Heal(int healing)
	{
		health += healing;
		if (healing > max_health)
		{
			health = max_health;
		}
	}


}
