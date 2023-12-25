using UnityEngine;
using UnityEngine.Serialization;

public class BounceStopper : MonoBehaviour
{
	[SerializeField] private BouncingBall bouncingBall;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			bouncingBall.StopBouncing();
		}
	}
}
