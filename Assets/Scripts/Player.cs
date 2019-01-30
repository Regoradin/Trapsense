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
	public AnimationClip idle_clip;

	public GameObject head_bone;
	public List<GameObject> hats;

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

		SetHat();

	}

	//Soley exists to make events happy.
	public void Move(string input)
	{
		Move(input, false);
	}

	public void Move(string input, bool damaging = false)
	{
		//prevents processing of move inputs while an animation is playing
		if (anim.GetCurrentAnimatorClipInfo(0)[0].clip == idle_clip)
		{

			bool is_valid_move = true;

			if (input == "Forward")
			{
				progress++;
				if (progress > max_progress)
				{
					max_progress = progress;
					if (max_progress % corridor_gen_distance == 1) //Generates next segment of corridor one space into the current segment
					{
						trap_gen.CreateCorridor(max_progress + corridor_gen_distance - 1);
					}
				}
			}
			if (input == "Back")
			{
				progress--;
			}

			if (input == "Left")
			{
				horizontal_coord -= 1;
				if (horizontal_coord < 0)
				{
					horizontal_coord += 1;
					is_valid_move = false;
				}
			}
			if (input == "Right")
			{
				horizontal_coord += 1;
				if (horizontal_coord >= trap_gen.corridor_width)
				{
					horizontal_coord -= 1;
					is_valid_move = false;
				}
			}

			if (input != "Wait" && is_valid_move)
			{
				if (!damaging)
				{
					anim.SetTrigger(input);
				}
				else
				{
					anim.SetTrigger("Damage" + input);
				}
			}
			else
			{
				EndMove();
			}
		}
	}

	public void EndMove()
	{
		if(PlayerTickEvent != null)
		{
			PlayerTickEvent("");
		}
		transform.position = new Vector3(horizontal_coord, 0, progress);
	}


	public void Damage(int damage, string direction = "None")
	{
		health -= damage;
		health_text.text = health.ToString();
		if (direction != "None")
		{
			Move(direction, true);
		}
		
	}

	public void Heal(int healing)
	{
		health += healing;
		if (health > max_health)
		{
			health = max_health;
		}
		health_text.text = health.ToString();		
	}

	private void SetHat()
	{
		int index = PlayerPrefs.GetInt("Hat", -1);
		Vector3 offset = Vector3.right * -.27f;
		if (index != -1) {
			GameObject hat =Instantiate(hats[index], head_bone.transform);
			//hat.transform.position = offset;
		}
	}

}
