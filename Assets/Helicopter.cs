using DG.Tweening;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
	private Animator _animator;
	private Rigidbody _rb;

	[SerializeField] private float forceForExplosion, explosionUpForce;
	private AudioSource _audio;

	private bool _isPlayerOnFever;

	private void OnEnable()
	{
		GameEvents.PlayerOnFever += OnFever;
	}

	private void OnDisable()
	{
		GameEvents.PlayerOnFever -= OnFever;
	}
	
	private void Start()
	{
		_animator = GetComponent<Animator>();
		_rb = GetComponent<Rigidbody>();
		_audio = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player") && !other.CompareTag("FeverTrigger")) return;
		
		var direction = transform.position - other.transform.position;
		//Hulk Smash
		HeliDeath(direction.normalized, forceForExplosion);
		var collisionPoint = other.ClosestPoint(transform.position);
		if(AudioManager.instance) AudioManager.instance.Play("HeliCrash");

		if (_isPlayerOnFever) return;
		GameEvents.InvokeMainKartCrash(collisionPoint);
	}

	public void StopHelicopter() => _animator.enabled = false;

	private void HeliDeath(Vector3 direction, float explosionForce)
	{
		_rb.useGravity = true;
		_rb.isKinematic = false;
		_rb.constraints = RigidbodyConstraints.None;

		_rb.AddForce(direction * explosionForce + Vector3.up * explosionUpForce, ForceMode.Impulse);
		_rb.AddTorque(Vector3.up * 360f, ForceMode.Acceleration);

		DOTween.To(() => _audio.pitch, value => _audio.pitch = value, 0f, 1.5f)
			.OnComplete(() =>
		{
			_audio.Stop();
			_audio.enabled = false;
		});
	}

	private void OnFever()
	{
		_isPlayerOnFever = !_isPlayerOnFever;
	}
}
