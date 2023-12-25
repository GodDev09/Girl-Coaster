using DG.Tweening;
using UnityEngine;

public class AirboneTrigger : MonoBehaviour
{
	[SerializeField] private GameObject helicopter;
	[SerializeField] private float altitude;
	[SerializeField] private float flightTime;

	[SerializeField] private float delayTime = 0.2f;

	[SerializeField] private LightSignal light1,light2;
	
	private Collider _collider;
	private void Start()
	{
		_collider = GetComponent<Collider>();
	}

	private void Fly()
	{
		helicopter.transform.DOLocalMove(helicopter.transform.up * altitude, flightTime);
		light1.ChangeColor(Color.green);
		light2.ChangeColor(Color.green);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		
		DOVirtual.DelayedCall(delayTime, Fly);

		_collider.enabled = false;
	}
}
