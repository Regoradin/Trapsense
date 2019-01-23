using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrapsGenerator : MonoBehaviour {
	public int corridor_height;
	public int corridor_width;
	public float space_size;

	[Range(0,1)]
	public float trap_density;

	private List<TrapInfo> traps; //Stores an inventory of TrapInfos created from trap_prefabs


	public List<GameObject> trap_prefabs;
	public List<GameObject> floor_prefabs;
	public List<GameObject> wall_prefabs;

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
	/// Randomly generates and places a corridor of traps.
	/// </summary>
	/// <param name="offset">Distance forward from the origin in spaces</param>
	public void CreateCorridor(int offset = 0)
	{
		TrapInfo[,] corridor = new TrapInfo[corridor_width, corridor_height];

		HashSet<TrapInfo> placed_traps = new HashSet<TrapInfo>();

		Debug.Log("Creating	Corridor " + offset);
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
			Trap new_trap = Instantiate(trap_info.prefab, new Vector3(trap_info.pos_x, 0, trap_info.pos_y + offset) * space_size, rotation).GetComponentInChildren<Trap>();
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
