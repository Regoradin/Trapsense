﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
	public int noise_height, noise_width;
	public float scale = 1f;
	public float speed = 1;
	public float brightness;

	public float color_flicker;
	public float intensity_flicker_upper;
	public float intensity_flicker_lower;

	private Cubemap noise;
	private Color[] pix;
	private Light torch;

	private float x_org = 1;
	private float y_org = 1;

	public AudioSource audio_source;
	private float initial_intensity;

	void Start()
	{
		torch = GetComponentInChildren<Light>();

		noise = new Cubemap(noise_width, TextureFormat.Alpha8, false);
		torch.cookie = noise;

		pix = new Color[noise_height * noise_width];

		initial_intensity = torch.intensity;

	}

	private void Update()
	{
		if (!Player.paused)
		{
			FlickerColor();
			SetAudio();
		}
	}

	void FlickerColor()
	{
		torch.color = new Color(
			torch.color.r + Random.Range(-color_flicker, color_flicker),
			torch.color.g + Random.Range(-color_flicker, color_flicker),
			torch.color.b + Random.Range(-color_flicker, color_flicker));

		torch.intensity += Random.Range(intensity_flicker_lower, intensity_flicker_upper);
	}

	void SetCookie()
	{
		float y = 0.0F;

		while (y < noise.height)
		{
			float x = 0.0F;
			while (x < noise.width)
			{
				//Perlin Noise
				float xCoord = x_org + x / noise.width * scale;
				float yCoord = y_org + y / noise.height * scale;
				float sample = Mathf.PerlinNoise(xCoord, yCoord);

				sample += brightness;
				pix[(int)y * noise.width + (int)x] = new Color(sample, sample, sample, sample);
				x++;
			}
			y++;
		}

		x_org += speed;
		y_org += speed;

		noise.SetPixels(pix, CubemapFace.NegativeX);
//		noise.SetPixels(pix, CubemapFace.NegativeY);
		noise.SetPixels(pix, CubemapFace.NegativeZ);
		noise.SetPixels(pix, CubemapFace.PositiveX);
		noise.SetPixels(pix, CubemapFace.PositiveY);
		noise.SetPixels(pix, CubemapFace.PositiveZ);
		noise.Apply();
	}

	void SetAudio()
	{
		audio_source.pitch = torch.intensity / initial_intensity;
	}
}
