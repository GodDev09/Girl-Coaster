using DG.Tweening;
using UnityEngine;

public class BridgeLights : MonoBehaviour
{
	[SerializeField] private LightSignal lightSignal;

	[SerializeField] private float delay = 0f;
	
	private Vector3 _startPosition;
	private Sequence _sequence;

	private TweenCallback _callback;
	private void Start()
	{
		_sequence = DOTween.Sequence();
		_sequence.AppendCallback(()=> lightSignal.ChangeColor(Color.red));
		_sequence.AppendInterval(delay);
		_sequence.AppendCallback(()=> lightSignal.ChangeColor(Color.green));
		_sequence.AppendInterval(delay);
		_sequence.SetLoops(-1);	
	}
}
