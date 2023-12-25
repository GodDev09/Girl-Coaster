using System;
using DG.Tweening;
using Kart;
using TMPro;
using UnityEngine;

public class BodyChangeGate : MonoBehaviour
{
	private enum ChangeType { Add, Subtract, Multiply, Divide }

	private AddedKartsManager _additionalKartManager;

	[SerializeField] private float moveDuration = 3f;
	[SerializeField] private float horizontalDistance;

	[SerializeField] private int factor;
	[SerializeField] private TMP_Text text;
	[SerializeField] private ChangeType changeType;
	[SerializeField] private Color positiveColor, negativeColor;

	private bool _hasBeenUsed;

	private void Start()
	{
		_additionalKartManager = GameObject.FindGameObjectWithTag("Player").GetComponent<AddedKartsManager>();
		transform.GetChild(0).GetComponent<MeshRenderer>().material.color = (changeType == ChangeType.Add || changeType == ChangeType.Multiply) ? positiveColor : negativeColor;

		transform.position -= transform.right * horizontalDistance / 2;
		transform.DOMove(transform.position + transform.right * horizontalDistance, moveDuration)
			.SetEase(Ease.Linear)
			.SetLoops(-1, LoopType.Yoyo);
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (_hasBeenUsed) return;
		if (!other.CompareTag("Player")) return;
		
		_hasBeenUsed = true;
		switch (changeType)
		{
			case ChangeType.Add:
				_additionalKartManager.SpawnKarts(factor);
				break;
			case ChangeType.Subtract:
				//rollerCoasterManager.HideKarts(factor);
				print("hi");
				break;
			case ChangeType.Multiply:
				_additionalKartManager.SpawnKarts(factor);
				break;
			case ChangeType.Divide:
				//rollerCoasterManager.HideKarts(factor);
				print("hi");
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void OnValidate()
	{
		text.text = name = changeType switch
		{
			ChangeType.Add =>  "+" + factor,
			ChangeType.Subtract => "-" + factor,
			ChangeType.Multiply => "+" + factor,
			ChangeType.Divide => "-" + factor,
			_ => name
		};
	}
}