using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class WarningText : MonoBehaviour
{
	[SerializeField] private TMP_Text text;
	[SerializeField] private Color finalColor;
	[SerializeField] private float loopDuration = 0.5f;
	private void Start()
	{
		text.DOColor(finalColor, loopDuration).SetLoops(-1, LoopType.Yoyo);
		text.transform.DOScale(1.5f, loopDuration).SetLoops(-1, LoopType.Yoyo);
	}
}
