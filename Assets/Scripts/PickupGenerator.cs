using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupGenerator : MonoBehaviour
{

	public List<GameObject> pickups;

	public void CreateCorridor(int corridor_height, int corridor_width, int offset, float pickup_density)
	{
		Debug.Log("Pickup Density: " + pickup_density);
		for(int x = 0; x < corridor_width; x++)
		{
			for(int y = 0; y < corridor_height; y++)
			{
				if(Random.Range(0f, 1f) < pickup_density)
				{
					GameObject pickup = pickups[Random.Range(0, pickups.Count)];
					Instantiate(pickup, new Vector3(x, 0, y + offset), Quaternion.identity);
				}
			}
		}
	}	



}
