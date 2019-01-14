using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour {

	public Trap trap;

	private void OnTriggerEnter(Collider other)
	{
		trap.Activate();
	}

}
