using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acid : MonoBehaviour
{
	public Color stain_color;

	private void OnTriggerEnter(Collider other)
	{
		other.GetComponentInChildren<Renderer>().material.color += stain_color;
	}
}
