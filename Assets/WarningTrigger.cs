using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WarningTrigger : MonoBehaviour
{
	[SerializeField] private float delayForDeactivation = 3f;

	private bool _isPlayerOnFever;

	private void OnEnable()
	{
		GameEvents.PlayerOnFever += DisableWarningPanel;
		GameEvents.PlayerOffFever += ResetTrigger;
	}

	private void OnDisable()
	{
		GameEvents.PlayerOnFever -= DisableWarningPanel;
		GameEvents.PlayerOffFever -= ResetTrigger;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_isPlayerOnFever) return;
			
		if (!other.CompareTag("Player")) return;
		
		GameEvents.InvokeObstacleWarningOn();
		DOVirtual.DelayedCall(delayForDeactivation,GameEvents.InvokeObstacleWarningOff);
	}

	private void DisableWarningPanel()
	{
		_isPlayerOnFever = true;
		GameEvents.InvokeObstacleWarningOff();
	}

	private void ResetTrigger()
	{
		_isPlayerOnFever = false;
	}
}
