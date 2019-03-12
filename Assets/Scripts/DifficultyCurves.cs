using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyCurves : MonoBehaviour
{
	public TrapsGenerator trap_gen;
	public AnimationCurve trap_density_over_distance;
	public AnimationCurve pickup_density_over_distance;

	public AnimSpeedManager anim_speed;
	public AnimationCurve anim_speed_over_time;

	public DeathWall death_wall;
	public AnimationCurve death_wall_speed_over_time;


	private void Update()
	{
		trap_gen.trap_density_per_distance_curve = trap_density_over_distance;
		trap_gen.pickup_density_per_distance_curve = pickup_density_over_distance;

		anim_speed.anim_speed_over_time = anim_speed_over_time;

		death_wall.speed_over_time = death_wall_speed_over_time;
	}
}
