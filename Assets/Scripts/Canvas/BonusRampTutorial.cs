using DG.Tweening;
using UnityEngine;

public class BonusRampTutorial : MonoBehaviour
{
	[SerializeField] private TMPro.TextMeshProUGUI instructionText;
	[SerializeField] private float scalingSize, scalingDuration;

	private void OnEnable()
	{
		GameEvents.ReachEndOfTrack += OnReachEndOfTrack;
		GameEvents.RunOutOfPassengers += OnReachEndOfBonusRamp;
	}

	private void OnDisable()
	{
		GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;
		GameEvents.RunOutOfPassengers -= OnReachEndOfBonusRamp;
	}

	private void ShowInstructions()
	{
		instructionText.gameObject.SetActive(true);
		instructionText.transform.DOScale(Vector3.one * scalingSize, scalingDuration).SetLoops(-1, LoopType.Yoyo);
	}
	
	private void HideInstructions()
	{
		instructionText.gameObject.SetActive(false);
		DOTween.Kill(instructionText.transform);
	}

	private void OnReachEndOfTrack() => ShowInstructions();

	private void OnReachEndOfBonusRamp() => HideInstructions();
}