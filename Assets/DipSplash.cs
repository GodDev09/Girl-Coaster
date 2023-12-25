	using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DipSplash : MonoBehaviour
{
	[SerializeField] private ParticleSystem dipSplash;

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player") && !other.CompareTag("Kart")) return;

		dipSplash.Play();
	}
}
