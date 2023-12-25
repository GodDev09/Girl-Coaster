using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class TrafficHalter : MonoBehaviour
{
	[SerializeField] private List<SplineFollower> carSplineFollowers;

	public void StopTheCars()
	{
		foreach (var splineFollower in carSplineFollowers)
		{
			splineFollower.follow = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			StopTheCars();
		}
	}
}
