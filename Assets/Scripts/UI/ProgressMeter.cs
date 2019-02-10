using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressMeter : MonoBehaviour
{
	private Slider slider;
	public Image fill_image;
	public Light torch_light;
	public bool is_total;

	private void Start()
	{
		slider = GetComponent<Slider>();
	}

	private void Update()
	{
		int target = 0;
		if (is_total)
		{
			target = PlayerPrefs.GetInt("TotalTarget");
		}
		else
		{
			target = PlayerPrefs.GetInt("BestTarget");
		}

		slider.maxValue = target;

		if (is_total)
		{
			slider.value = PlayerPrefs.GetInt("TotalScore");
		}
		else
		{
			slider.value = Player.player.max_progress;
		}

		fill_image.color = torch_light.color;
		fill_image.color = new Color(fill_image.color.r, fill_image.color.g, fill_image.color.b, torch_light.intensity/5);
	}


}
