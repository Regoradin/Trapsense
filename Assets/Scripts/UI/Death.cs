using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Death : MonoBehaviour
{
	public GameObject death_ui;

	private void Start()
	{
		death_ui.SetActive(false);
	}

	private void Update()
	{
		if(Player.player.health <= 0)
		{
			death_ui.SetActive(true);
		}
	}

}
