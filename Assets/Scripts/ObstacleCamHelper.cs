using Kart;
using UnityEngine;

public class ObstacleCamHelper : MonoBehaviour
{
	[SerializeField] private bool calledOnExit, isObstacleOnRight;
	private bool _doneOnce;

	private void Start() => GetComponent<MeshRenderer>().enabled = false;

	private void OnTriggerEnter(Collider other)
	{
		if(_doneOnce) return;
		if(!other.CompareTag("Player")) return;

		_doneOnce = true;
		if(calledOnExit)
			ReturnFromObstacleCam();
		else
		{
			SendToObstacleCam();
		}
	}

	private void SendToObstacleCam() => DampCamera.only.SendToObstacleCam(isObstacleOnRight);

	private static void ReturnFromObstacleCam() => DampCamera.only.CameraResetPosition();
}