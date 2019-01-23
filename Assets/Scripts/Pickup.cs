using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	public float light_increase;
	public int health_increase;

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Player>())
		{
			other.GetComponentInChildren<Light>().intensity += light_increase;
			Player.player.Heal(health_increase);

			Destroy(gameObject);
		}
	}

}
