﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrapsGenerator : MonoBehaviour {
	public int corridor_height;
	public int corridor_width;
	public float space_size;

	[Range(0,1)]
	public float trap_density;

	private TrapInfo[,] corridor;
	private List<TrapInfo> traps; //Almost an inventory of traps to be replicated throughout the corridor

	private HashSet<TrapInfo> placed_traps;

	public List<GameObject> trap_prefabs;

	private void Start()
	{
		corridor = new TrapInfo[corridor_width, corridor_height];
		traps = new List<TrapInfo>();
		placed_traps = new HashSet<TrapInfo>();

		foreach(GameObject trap_obj in trap_prefabs)
		{
			Trap trap = trap_obj.GetComponentInChildren<Trap>();
			if (trap != null)
			{
				traps.Add(new TrapInfo(trap.height, trap.width, trap_obj.name, trap_obj));
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
		Debug.Log("Creating Corridor");
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
		List<TrapInfo> trap_infos = placed_traps.ToList();
		while(trap_infos.Count != 0)
		{
			TrapInfo trap = trap_infos[trap_infos.Count - 1];

			bool has_space = true;
			for (int x = trap.pos_x; x < trap.pos_x + trap.width; x++)
			{
				for (int y = trap.pos_y; y < trap.pos_y + trap.height; y++)
				{
					if (x < corridor.GetLength(0) && y < corridor.GetLength(1))
					{
						if (!trap_infos.Contains(corridor[x, y]) && corridor[x, y] != null)
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
				for (int x = trap.pos_x; x < trap.pos_x + trap.width; x++)
				{
					for (int y = trap.pos_y; y < trap.pos_y + trap.height; y++)
					{
						if (x < corridor.GetLength(0) && y < corridor.GetLength(1))
						{
							corridor[x, y] = trap;
						}
					}
				}
			}
			else
			{
				corridor[trap.pos_x, trap.pos_y] = null;
			}
			trap_infos.Remove(trap);
		}


		//Actually puts traps objects in the world.
		HashSet<TrapInfo> trapset = new HashSet<TrapInfo>();
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
			Instantiate(trap_info.prefab, new Vector3(trap_info.pos_x, 0, trap_info.pos_y + offset) * space_size, Quaternion.identity);
		}

	}

	private void PrintCorridor()
	{
		string result = "";
		for(int x = 0; x < corridor.GetLength(0); x++)
		{
			for (int y = 0; y < corridor.GetLength(1); y++)
			{
				result += "|";
				if (corridor[x, y] != null)
				{
					result += corridor[x, y].name;
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
		public string name;

		public int pos_x, pos_y;

		public GameObject prefab;

		public TrapInfo(int height, int width, string name, GameObject prefab)
		{
			this.height = height;
			this.width = width;
			this.name = name;
			this.prefab = prefab;
		}
		public TrapInfo(TrapInfo blueprint, int pos_x, int pos_y) : this(blueprint.height, blueprint.width, blueprint.name, blueprint.prefab)
		{
			this.pos_x = pos_x;
			this.pos_y = pos_y;
		}
	}
}
