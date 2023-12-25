using Kart;
using UnityEngine;

public class SpeedBooster : MonoBehaviour
{
	private AudioSource _source;
	private MainKartController _player;
	private bool _usedUp;
	
	private void Start()
	{
		_player = GameObject.FindWithTag("Player").GetComponent<MainKartController>();
		_source = GetComponent<AudioSource>();
	}

	private void AddSpeedBoost() => _player.TrackMovement.AddSpeedBoost();

	private void OnTriggerEnter(Collider other)
	{
		if(_usedUp) return;
		if(!other.CompareTag("Player")) return;

		_usedUp = true;
		_source.Play();
		AddSpeedBoost();
	}
}