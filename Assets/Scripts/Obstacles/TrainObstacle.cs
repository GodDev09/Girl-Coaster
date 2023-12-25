using DG.Tweening;
using UnityEngine;

public class TrainObstacle : MonoBehaviour
{
	[SerializeField] private Transform train1, train2;
	[SerializeField] private float travelLoopDistance, travelLoopDuration, train2Delay, tweenKillDelay;
	private Tween _train1Tween, _train2Tween;
	private bool _isKilled;

	private void Start()
	{
		_train1Tween = train1.DOLocalMove(train1.localPosition + train1.forward * travelLoopDistance, travelLoopDuration)
			.SetEase(Ease.Linear)
			.SetLoops(-1, LoopType.Restart);

		var train2InitLocalPos = train2.localPosition;
		train2.localPosition += train2.forward * travelLoopDistance;
		_train2Tween = train2.DOLocalMove(train2InitLocalPos, travelLoopDuration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).SetDelay(train2Delay);
	}

	private void StopTwain()
	{
		_isKilled = true;
		_train1Tween.Kill();
		_train2Tween.Kill();

		DOVirtual.DelayedCall(tweenKillDelay, () =>
		{
			train1.gameObject.SetActive(false);
			train2.gameObject.SetActive(false);
		});
	}

	private void OnTriggerEnter(Collider other)
	{
		if(_isKilled) return;
		if(!other.CompareTag("Player") && !other.CompareTag("Kart")) return;
		
		StopTwain();
	}
}