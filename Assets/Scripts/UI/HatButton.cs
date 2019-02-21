using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatButton : MonoBehaviour
{
	//Due to some changes, it's actually a toggle, but whatever.
	public int index;
	public bool use_total;
	public bool unlocked;

	public Text text;

	private void Awake()
	{
		text = GetComponentInChildren<Text>();
	}

	private void Start()
	{
		Toggle toggle = GetComponent<Toggle>();
		toggle.interactable = unlocked;
		if(index == PlayerPrefs.GetInt("Hat"))
		{
			toggle.isOn = true;
		}
	}

	public void SetHat()
	{
		PlayerPrefs.SetInt("Hat", index);

		int use_total_int = use_total ? 1 : 0;
		PlayerPrefs.SetInt("UseTotalHat", use_total_int);

		PlayerPrefs.Save();
	}


}
