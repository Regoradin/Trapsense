using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timekeeper : MonoBehaviour {

	public static Timekeeper timekeeper;

	public delegate void Tick(string input, float mirror);
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
				if (Input.GetButtonDown("LeftForward"))
				{
					CreateTick("Forward", 0);
				}
				else if (Input.GetButtonDown("RightForward"))
				{
					CreateTick("Forward", 1);
				}
				else if (Input.GetButtonDown("LeftBack"))
				{
					CreateTick("Back", 0);
				}
				else if (Input.GetButtonDown("RightBack"))
				{
					CreateTick("Back", 1);
				}
				else if (Input.GetButtonDown("LeftRight"))
				{
					CreateTick("Right", 0);
				}
				else if (Input.GetButtonDown("RightRight"))
				{
					CreateTick("Right", 1);
				}
				else if (Input.GetButtonDown("LeftLeft"))
				{
					CreateTick("Left", 0);
				}
				else if (Input.GetButtonDown("RightLeft"))
				{
					CreateTick("Left", 1);
				}
				else if (Input.GetButtonDown("Space"))
				{
					CreateTick("Wait", 0);
				}
			}
		}
	}

	/// <summary>
	/// Creates a tick event with the given input. Should be used with great caution.
	/// </summary>
	/// <param name="input"></param>
	public void CreateTick(string input, float mirror)
	{
		TickEvent(input, mirror);
	}


	private void TickDebugger(string input, float mirror)
	{

	}
}
