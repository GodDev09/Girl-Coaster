using UnityEngine;

public class CarObstacle : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		
		var collisionPoint = other.ClosestPoint(transform.position);
		GameEvents.InvokeMainKartCrash(collisionPoint);
	}
}
