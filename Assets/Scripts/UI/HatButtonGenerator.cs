using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatButtonGenerator : MonoBehaviour
{
	private HatUnlockManager hat_man;
	public GameObject hat_button;

	private void Start()
	{
		hat_man = HatUnlockManager.hat_manager;

		for(int i = 0; i < hat_man.best_hats.Count; i++)
		{
			GameObject button_obj = Instantiate(hat_button, transform);

			HatButton button = button_obj.GetComponent<HatButton>();
			button.index = i;
			button.use_total = false;
			button.text.text = hat_man.best_hats[i].name;
			if(i < PlayerPrefs.GetInt("BestUnlockedHatCount"))
			{
				button.unlocked = true;
			}
			else
			{
				button.unlocked = false;
			}
		}

		for (int i = 0; i < hat_man.total_hats.Count; i++)
		{
			GameObject button_obj = Instantiate(hat_button, transform);

			HatButton button = button_obj.GetComponent<HatButton>();
			button.index = i;
			button.use_total = true;
			button.text.text = hat_man.total_hats[i].name;
			if (i < PlayerPrefs.GetInt("TotalUnlockedHatCount"))
			{
				button.unlocked = true;
			}
			else
			{
				button.unlocked = false;
			}
		}
	}



}
