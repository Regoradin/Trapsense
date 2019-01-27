using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrapsGenerator : MonoBehaviour {
	public PickupGenerator pickup_gen;

	public int corridor_height;
	public int corridor_width;

	[Range(0,1)]
//	private float trap_density;

	private List<TrapInfo> traps; //Stores an inventory of TrapInfos created from trap_prefabs


	public List<GameObject> trap_prefabs;
	public List<GameObject> floor_prefabs;
	public List<GameObject> wall_prefabs;


	[Header("Difficulty Settings")]
	[Header("Traps")]
	public float flat_trap_density;
	public bool trap_density_log_scaling;
	[Range(0,1)]
	public float initial_trap_density;
	[Range(0,1)]
	public float final_trap_density;
	public float trap_difficulty_scaling_factor;  //affects how dramatically the logistic difficulty function ramps
	public int trap_density_corridor_iterations;  //affects the length over which the logistic difficulty function has the desired effect

	[Header("Pickups")]
	public float flat_pickup_density;
	public bool pickup_density_log_scaling;
	[Range(0, 1)]
	public float initial_pickup_density;
	[Range(0, 1)]
	public float final_pickup_density;
	public float pickup_density_scaling_factor;
	public int pickup_density_corridor_iterations;

	private int iterations = 0;


	private void Start()
	{
		traps = new List<TrapInfo>();

		foreach(GameObject trap_obj in trap_prefabs)
		{
			Trap trap = trap_obj.GetComponentInChildren<Trap>();
			if (trap != null)
			{
				traps.Add(new TrapInfo(trap_obj.name, trap_obj));
			}
		}

		CreateCorridor(0);
	}

	/// <summary>
	/// Uses a logistic curve to return a difficulty between initial and final.
	/// </summary>
	/// <param name="this_iteration">The current point along the logistic curve.</param>
	/// <param name="initial">The lower bound on the resulting difficulty, and the starting point.</param>
	/// <param name="final">The upper bound on the resulting difficutly, and the ending point.</param>
	/// <param name="scaling_factor">A scaling factor that shapes the resulting curve. Smaller values will result in a smoother curve</param>
	/// <param name="intended_iterations">The intended number of iterations that this difficulty curve works for. Past this, values will simply approach the final value.</param>
	/// <returns></returns>
	public static float LogisticDifficultyCalcs(float this_iteration, float initial, float final, float scaling_factor, float intended_iterations)
	{
		float value = ((final - initial) / (1 + Mathf.Pow(3,(-scaling_factor * (this_iteration - (intended_iterations / 2)))))) + initial;

		return value;
	}

	/// <summary>
	/// Randomly generates and places a corridor of traps.
	/// </summary>
	/// <param name="offset">Distance forward from the origin in spaces</param>
	public void CreateCorridor(int offset = 0)
	{
		float trap_density = -1;
		if (trap_density_log_scaling)
		{
			trap_density = LogisticDifficultyCalcs(iterations, initial_trap_density, final_trap_density, trap_difficulty_scaling_factor, trap_density_corridor_iterations);
			iterations++;
		}
		else
		{
			trap_density = flat_trap_density;
		}

		float pickup_density = -1;
		if (pickup_density_log_scaling)
		{
			pickup_density = LogisticDifficultyCalcs(iterations, initial_pickup_density, final_pickup_density, pickup_density_scaling_factor, pickup_density_corridor_iterations);
		}
		else
		{
			pickup_density = flat_pickup_density;
		}
		pickup_gen.CreateCorridor(corridor_height, corridor_width, offset, pickup_density);


		TrapInfo[,] corridor = new TrapInfo[corridor_width, corridor_height];

		HashSet<TrapInfo> placed_traps = new HashSet<TrapInfo>();

		//Populates the corridor with random traps
		for (int x = 0; x < corridor.GetLength(0); x++)
		{
			for (int y = 0; y < corridor.GetLength(1); y++)
			{
				if (Random.Range(0f, 1f) <= trap_density)
				{
					TrapInfo blueprint = traps[Random.Range(0, traps.Count)];
					corridor[x, y] = new TrapInfo(blueprint, x, y);
					placed_traps.Add(corridor[x, y]);
				}
			}
		}


		//Clears overlapping traps
		//trap_infos tracks remaining traps, which are the one that haven't been expanded and are ok to clear for other traps.
		List<TrapInfo> trap_infos = placed_traps.ToList(); //List of traps that have been orignally seeded into the corridor
		HashSet<TrapInfo> trapset = new HashSet<TrapInfo>(); //Set of traps that have been checked and expanded and will actually be built
		while (trap_infos.Count != 0)
		{
			TrapInfo trap = trap_infos[trap_infos.Count - 1];

			
			bool has_space = true;
			//checks if its origin is on the edge if required
			if (trap.needs_edge)
			{
				if (trap.pos_x != 0 && trap.pos_x != corridor.GetLength(0) -1)
				{
					has_space = false;
				}
			}
			//checks if it has space to expand in the correct direction that doesn't get in the way of anything.
			for (int x = trap.pos_x; Mathf.Abs(x - trap.pos_x) < Mathf.Abs(trap.width); x += (int)Mathf.Sign(trap.width))
			{
				for (int y = trap.pos_y; Mathf.Abs(y - trap.pos_y) < Mathf.Abs(trap.height); y += (int)Mathf.Sign(trap.height))
				{
					if (x >= 0 && x < corridor.GetLength(0) && y >= 0 && y < corridor.GetLength(1))
					{
						if (trapset.Contains(corridor[x, y])) //i.e. there is an already expanded trap there
						{
							has_space = false;
						}
					}
					else
					{
						has_space = false;
					}

				}
			}


			//Expands itself if there is space for it, otherwise removes itself
			if (has_space)
			{
				trapset.Add(trap);
				for (int x = trap.pos_x; Mathf.Abs(x - trap.pos_x) < Mathf.Abs(trap.width); x += (int)Mathf.Sign(trap.width))
				{
					for (int y = trap.pos_y; Mathf.Abs(y - trap.pos_y) < Mathf.Abs(trap.height); y += (int)Mathf.Sign(trap.height))
					{
						if(corridor[x, y] != trap)
						{
							trap_infos.Remove(corridor[x, y]);
						}
						corridor[x, y] = trap;
					}
				}
			}
			else
			{
				//if nothing else has expanded to take its space, nullify itself.
				if (corridor[trap.pos_x, trap.pos_y] == trap)
				{
					corridor[trap.pos_x, trap.pos_y] = null;
				}
			}

			trap_infos.Remove(trap);
		}


		//Actually puts traps objects in the world.
		foreach(TrapInfo trap_info in corridor)
		{
			if (trap_info != null)
			{
				trapset.Add(trap_info);
			}
		}
		TrapInfo[] traps_to_build = trapset.ToArray();
		foreach(TrapInfo trap_info in traps_to_build)
		{
			Quaternion rotation = Quaternion.identity;
			for(int i = 0; i < trap_info.rotation; i++)
			{
				rotation *= Quaternion.Euler(0, 90, 0);
			}
			Trap new_trap = Instantiate(trap_info.prefab, new Vector3(trap_info.pos_x, 0, trap_info.pos_y + offset), rotation).GetComponentInChildren<Trap>();
			new_trap.IncrementDirection(trap_info.rotation);
		}

		//Creating Floor
		for(int x = 0; x < corridor.GetLength(0); x++)
		{
			for(int y = 0; y < corridor.GetLength(1); y++)
			{
				//Rotation is to cancel out the axis mismatch with blender.
				GameObject floor = floor_prefabs[Random.Range(0, floor_prefabs.Count())];
				Instantiate(floor, new Vector3(x, 0, y + offset), Quaternion.Euler(Vector3.right * -90 + Vector3.up * 90 * Random.Range(0, 4)));
			}
		}

		//Creating Walls
		for(int y = 0; y < corridor.GetLength(1); y++)
		{
			GameObject wall = wall_prefabs[Random.Range(0, wall_prefabs.Count())];
			Instantiate(wall, new Vector3(-1, 0, y + offset), Quaternion.Euler(Vector3.right * -90));

			wall = wall_prefabs[Random.Range(0, wall_prefabs.Count())];
			Instantiate(wall, new Vector3(corridor.GetLength(0), 0, y + offset), Quaternion.Euler(Vector3.right * -90 + Vector3.up * 180));

		}


	}

	private void PrintCorridor(TrapInfo[,] corridor)
	{
		string result = "";
		for(int x = 0; x < corridor.GetLength(0); x++)
		{
			for (int y = 0; y < corridor.GetLength(1); y++)
			{
				result += "|";
				if (corridor[x, y] != null)
				{
					result += corridor[x, y].name[0];
				}
				else
				{
					result += "_";
				}
			}
			result += "\n";
		}

		Debug.Log(result);
	}

	private class TrapInfo
	{
		public int height;
		public int width;
		public int rotation;
		public string name;
		public bool needs_edge;

		public int pos_x, pos_y;

		public GameObject prefab;

		public TrapInfo(string name, GameObject prefab)
		{
			Trap base_trap = prefab.GetComponentInChildren<Trap>();

			this.name = name;
			this.prefab = prefab;
			this.needs_edge = base_trap.needs_edge;

			List<int> allowed_rotations = new List<int>();
			if (base_trap.zero)
				allowed_rotations.Add(0);
			if (base_trap.ninety)
				allowed_rotations.Add(1);
			if (base_trap.one_eighty)
				allowed_rotations.Add(2);
			if (base_trap.two_seventy)
				allowed_rotations.Add(3);

			rotation = allowed_rotations[Random.Range(0, allowed_rotations.Count)];

			if (rotation == 0)
			{
				this.height = base_trap.height;
				this.width = base_trap.width;
			}
			else if(rotation == 1)
			{
				this.height = -base_trap.width;
				this.width = base_trap.height;
			}
			else if(rotation == 2)
			{
				this.height = -base_trap.height;
				this.width = -base_trap.width;
			}
			else if(rotation == 3)
			{
				this.height = base_trap.width;
				this.width = -base_trap.height;
			}
		}
		public TrapInfo(TrapInfo blueprint, int pos_x, int pos_y) : this(blueprint.name, blueprint.prefab)
		{
			this.pos_x = pos_x;
			this.pos_y = pos_y;
		}
	}
}
