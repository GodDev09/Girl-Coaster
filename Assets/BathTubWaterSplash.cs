using System;
using UnityEngine;

public class BathTubWaterSplash : MonoBehaviour
{
	[SerializeField] private ParticleSystem splash1, splash2;
	private bool _isSubscribed;

	private void OnEnable()
	{
		GameEvents.JumpInBathTub += Splash;
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void Start()
	{
		if (GetComponentInParent<Obstacle>()) Unsubscribe();
	}

	private void Unsubscribe()
	{
		if(!_isSubscribed) return;
		
		_isSubscribed = false;
		GameEvents.JumpInBathTub -= Splash;
	}

	private void Splash()
	{
		if (!splash1 && !splash2) return;
		
		splash1.Play();
		splash2.Play();
	}
}
