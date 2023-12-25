using DG.Tweening;
using UnityEngine;

public class FinishStation : MonoBehaviour
{
	[SerializeField] private AnimationCurve easing;
	[SerializeField] private float duration = 1f;
	
	private void OnEnable()
	{
		GameEvents.ReachEndOfTrack += OnReachEndOfTrack;
	}
	
	private void OnDisable()
	{
		GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;
	}

	private void OnReachEndOfTrack() => transform.DOScale(Vector3.zero, duration).SetEase(easing);
}