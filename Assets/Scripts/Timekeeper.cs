using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timekeeper : MonoBehaviour {

	public static Timekeeper timekeeper;

	public delegate void Tick(string input);
	public event Tick TickEvent;

	private void Awake()
	{
		timekeeper = this;
		TickEvent += TickDebugger;
	}

	private void Update()
	{
		if (!Player.paused)
		{
			if (TickEvent != null)
			{
				if (Input.GetButtonDown("Forward"))
				{
					CreateTick("Forward");
				}
				else if (Input.GetButtonDown("Back"))
				{
					CreateTick("Back");
				}
				else if (Input.GetButtonDown("Right"))
				{
					CreateTick("Right");
				}
				else if (Input.GetButtonDown("Left"))
				{
					CreateTick("Left");
				}
				else if (Input.GetButtonDown("Space"))
				{
					CreateTick("Wait");
				}
			}
		}
	}

	/// <summary>
	/// Creates a tick event with the given input. Should be used with great caution.
	/// </summary>
	/// <param name="input"></param>
	public void CreateTick(string input)
	{
		TickEvent(input);
	}


	private void TickDebugger(string input)
	{

	}
}
