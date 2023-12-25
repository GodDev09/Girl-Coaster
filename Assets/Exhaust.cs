using UnityEngine;

public class Exhaust : MonoBehaviour
{
	[SerializeField] private GameObject exhaustParticleSystem;
	private void OnEnable()
	{
		GameEvents.PlayerOnFever += ExhaustOn;
		GameEvents.PlayerOffFever += ExhaustOff;
	}

	private void OnDisable()
	{
		GameEvents.PlayerOnFever -= ExhaustOn;
		GameEvents.PlayerOffFever -= ExhaustOff;		
	}
	
	private void ExhaustOn()
	{
		exhaustParticleSystem.SetActive(true);
	}

	private void ExhaustOff() => exhaustParticleSystem.SetActive(false);
}
