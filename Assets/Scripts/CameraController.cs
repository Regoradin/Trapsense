using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private float z_offset;
	private Transform player;

	private Vector3 velocity = Vector3.zero;
	public float smoothTime;

	private void Start()
	{
		player = Player.player.transform;
		transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
		z_offset = transform.position.z - player.position.z;
	}

	private void Update()
	{
		Vector3 target_pos = new Vector3(transform.position.x, transform.position.y, player.position.z + z_offset);

		transform.position = Vector3.SmoothDamp(transform.position, target_pos, ref velocity, smoothTime);
	}

}
