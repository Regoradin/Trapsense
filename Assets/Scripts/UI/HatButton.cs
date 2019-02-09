using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatButton : MonoBehaviour
{
	public int index;
	public bool use_total;

	public Text text;

	public void Awake()
	{
		text = GetComponentInChildren<Text>();
	}

	public void SetHat()
	{
		PlayerPrefs.SetInt("Hat", index);

		int use_total_int = use_total ? 1 : 0;
		PlayerPrefs.SetInt("UseTotalHat", use_total_int);

		PlayerPrefs.Save();
	}


}
