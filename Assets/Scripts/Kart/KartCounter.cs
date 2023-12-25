using DG.Tweening;
using TMPro;
using UnityEngine;

public class KartCounter : MonoBehaviour
{
	[SerializeField] private TMP_Text text;
	[SerializeField] private SpriteRenderer sprite;
	[SerializeField] private Color increaseColor, decreaseColor;
	[SerializeField] private float colorToTime, colorHoldTime, colorReturnTime;

	private Color _initColor;
	
	private void OnEnable() => GameEvents.ReachEndOfTrack += OnReachEndOfTrack;

	private void OnDisable() => GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;

	private void Start() => _initColor = sprite.color;

	private void IncreaseSequence()
	{
		sprite.DOColor(increaseColor, sprite.color == increaseColor ? 0.01f : colorToTime)
			.SetTarget(sprite)
			.OnComplete(() => 
				sprite.DOColor(_initColor, colorReturnTime)
					.SetDelay(colorHoldTime)
					.SetTarget(sprite));
	}

	private void DecreaseSequence()
	{
		sprite.DOColor(decreaseColor, sprite.color == decreaseColor ? 0.01f : colorToTime)
			.SetTarget(sprite)
			.OnComplete(() => 
				sprite.DOColor(_initColor, colorReturnTime)
					.SetDelay(colorHoldTime)
					.SetTarget(sprite));
	}

	public void UpdateText(int number, bool hasIncreased)
	{
		text.text = number.ToString();

		sprite.transform.parent.DOScale(Vector3.one * 1.2f, colorToTime).SetLoops(2, LoopType.Yoyo);
		
		DOTween.Kill(sprite);

		if (hasIncreased) IncreaseSequence();
		else DecreaseSequence();
	}

	private void OnReachEndOfTrack() => text.transform.parent.gameObject.SetActive(false);
}