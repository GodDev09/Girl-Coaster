using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FeverOverlay : MonoBehaviour
{
	[SerializeField] private Image overlayImage;

	[SerializeField] private Color finalColor;
	[SerializeField] private float loopDuration = 0.4f;
	private void Start()
	{
		overlayImage.DOColor(finalColor, loopDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Flash);
		overlayImage.transform.DOScale(1.5f, loopDuration).SetLoops(-1,LoopType.Yoyo);
	}
}
