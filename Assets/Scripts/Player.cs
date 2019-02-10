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

	[HideInInspector]
	public int progress = 0;
	public int max_progress = 1;
	public TrapsGenerator trap_gen;
	private int corridor_gen_distance;

	private int horizontal_coord = 4; //This is based off the starting position of the player relative to the corridor generation, and will almost certainly need to be changed

	public delegate void Tick(string input);
	public event Tick PlayerTickEvent;

	public Text health_text;
	public GameObject death_ui;
	public Text best_score_text;
	public Text total_score_text;
	public AnimationClip idle_clip;

	public GameObject head_bone;

	public int best_score_lin_increase;
	public int total_score_geom_increase;
	private int unadded_progress; //tracks progress that hasn't been added to the total score in PlayerPrefs yet.	

	public Dictionary<GameObject, int> hat_dict;

	private void Awake()
	{
		player = this;
	
		corridor_gen_distance = trap_gen.corridor_height;

		anim = GetComponent<Animator>();

	}

	private void Start()
	{
		Timekeeper.timekeeper.TickEvent += Move;
		health = max_health;
		health_text.text = health.ToString();

		death_ui.SetActive(false);		
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
					unadded_progress++;
					if (max_progress > PlayerPrefs.GetInt("BestTarget") || unadded_progress + PlayerPrefs.GetInt("TotalScore") > PlayerPrefs.GetInt("TotalTarget")) ;
					{
						UpdateScores();
					}
					if (max_progress % corridor_gen_distance == 1) //Generates next segment of corridor one space into the current segment
					{
						trap_gen.CreateCorridor(max_progress + corridor_gen_distance - 1);
					}
				}
			}
			if (input == "Back")
			{
				progress--;
				if(progress < -trap_gen.start_height)
				{
					progress++;
					is_valid_move = false;
				}
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
		else
		{
			anim.SetTrigger("Damage");
		}
		if(health <= 0)
		{
			Die();
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
		bool use_total_hat = PlayerPrefs.GetInt("UseTotalHat") == 1 ? true : false;
		if (index != -1) {
			if (use_total_hat)
			{
GameObject hat = Instantiate(HatUnlockManager.hat_manager.total_hats[index], head_bone.transform);
			}
			else
			{
				GameObject hat = Instantiate(HatUnlockManager.hat_manager.best_hats[index], head_bone.transform);
			}
		}
	}

	public void Die()
	{
		death_ui.SetActive(true);

		UpdateScores();

		int total_score = PlayerPrefs.GetInt("TotalScore");
		int best_target = PlayerPrefs.GetInt("BestTarget");
		int total_target = PlayerPrefs.GetInt("TotalTarget");
		best_score_text.text = "Score: " + max_progress + " / " + best_target;
		total_score_text.text = "Total Score: " + total_score + " / " + total_target;
	}

	public void UpdateScores()
	{
		//Update best and total progress counters
		if (max_progress > PlayerPrefs.GetInt("BestScore", 0))
		{
			PlayerPrefs.SetInt("BestScore", max_progress);
		}

		PlayerPrefs.SetInt("TotalScore", PlayerPrefs.GetInt("TotalScore", 0) + unadded_progress);
		unadded_progress = 0;

		//Update target best and total scores if necessary, and increase the number of unlocked hats accordingly
		while (PlayerPrefs.GetInt("BestScore") > PlayerPrefs.GetInt("BestTarget", 0))
		{
			PlayerPrefs.SetInt("BestTarget", PlayerPrefs.GetInt("BestTarget", 0) + best_score_lin_increase);

			PlayerPrefs.SetInt("BestUnlockedHatCount", PlayerPrefs.GetInt("BestUnlockedHatCount", 0) + 1);
		}
		while (PlayerPrefs.GetInt("TotalScore") > PlayerPrefs.GetInt("TotalTarget", 1))
		{
			PlayerPrefs.SetInt("TotalTarget", PlayerPrefs.GetInt("TotalTarget", 1) * total_score_geom_increase);

			PlayerPrefs.SetInt("TotalUnlockedHatCount", PlayerPrefs.GetInt("TotalUnlockedHatCount", 0) + 1);
		}

		PlayerPrefs.Save();

	}

}
