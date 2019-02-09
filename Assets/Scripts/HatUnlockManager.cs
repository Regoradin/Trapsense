using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatUnlockManager : MonoBehaviour
{
	public static HatUnlockManager hat_manager;
	public List<GameObject> best_hats;
	public List<GameObject> total_hats;

	public void Awake()
	{ 
		hat_manager = this;
		DontDestroyOnLoad(gameObject);
	}
}
