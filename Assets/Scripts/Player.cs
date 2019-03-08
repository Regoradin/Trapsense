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

	[Range(0, 1)]
	public float buffer_leniency;
	private string move_buffer;

	public delegate void Tick(string input);
	public event Tick PlayerTickEvent;

	[Header("UI")]
	public Slider health_bar;	
	public GameObject death_ui;
	public Text score_text;
	public Text best_score_text;
	public Text total_score_text;
	public Text total_goal_text;

	public GameObject pause_ui;
	public static bool paused = false;

	[Header("Animation")]
	public List<AnimationClip> idle_clips;
	public GameObject head_bone;

	public float mirror_duration;
	private float mirror_start_time;
	private float mirror_target = 0;

	[Header("Difficulty")]
	public int best_score_lin_increase;
	public int total_score_geom_increase;
	private int unadded_progress; //tracks progress that hasn't been added to the total score in PlayerPrefs yet.	

	public Dictionary<GameObject, int> hat_dict;

	[Header("Audio")]
	private bool left_step = false;
	public AudioSource damage_sound;
	public AudioSource left_foot;
	public AudioSource right_foot;

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
		health_bar.maxValue = max_health;
		health_bar.value = health;

		death_ui.SetActive(false);
		Pause(false);
		SetHat();

	}

	public void Pause(bool input)
	{
		paused = input;
		if (paused)
		{
			pause_ui.SetActive(true);
		}
		else
		{
			pause_ui.SetActive(false);
		}
	}
	private void Update()
	{
		if (Input.GetButtonDown("Pause"))
		{
			Pause(!Player.paused);
		}
		else
		{
			if(move_buffer != "")
			{
				Move(move_buffer);
			}
		}

		if (!anim.GetFloat("Mirror").Equals(mirror_target))
		{
			anim.SetFloat("Mirror", Mathf.Lerp(1 - mirror_target, mirror_target, (Time.time - mirror_start_time) / mirror_duration));
		}
	}

	//Soley exists to make events happy.
	public void Move(string input, float mirror)
	{
		Move(input, false, mirror);
	}

	public void Move(string input, bool damaging = false, float mirror = 0)
	{
		//prevents processing of move inputs while an animation is playing
		if (damaging || idle_clips.Contains(anim.GetCurrentAnimatorClipInfo(0)[0].clip))
		{
			move_buffer = "";
			mirror_target = mirror;
			mirror_start_time = Time.time;

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
		else if((1 - anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) <= buffer_leniency)
		{
			move_buffer = input;
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
		health_bar.value = health;
		damage_sound.Play();		
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
		health_bar.value = health;
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
		UpdateScores();

		int total_score = PlayerPrefs.GetInt("TotalScore");
		int best_target = PlayerPrefs.GetInt("BestTarget");
		int total_target = PlayerPrefs.GetInt("TotalTarget");
		score_text.text = "Distance: " + max_progress;
		best_score_text.text = "Next hat unlocked at: " + best_target;
		total_score_text.text = "Total Distance: " + total_score;
		total_goal_text.text = "Next hat unlocked at: " + total_target;

		//Delay a bit so player can see what killed them.
		Invoke("ActivateDeathUI", 0.4f);
	}
	private void ActivateDeathUI()
	{
		death_ui.SetActive(true);
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

	public void PlayStepNoise()
	{
		if (left_step)
		{
			left_step = false;
			left_foot.Play();
		}
		else
		{
			left_step = true;
			right_foot.Play();
		}
	}

}
