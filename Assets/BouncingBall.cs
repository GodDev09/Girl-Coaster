using DG.Tweening;
using UnityEngine;

public class BouncingBall : MonoBehaviour
{
	[SerializeField] private float delay;
	
	private Animator _animator;
	private Rigidbody _rb;
	private Collider _collider;

	private Vector3 _initScale;
	private AudioSource _audio;

	private float _initPitch;

	private void Start()
	{
		_animator = GetComponent<Animator>();
		_rb = GetComponent<Rigidbody>();
		_collider = GetComponent<Collider>();
		_audio = GetComponent<AudioSource>();
		_initScale = transform.localScale;
		
		if (delay < 0.001f)
		{
			_animator.enabled = true;
			return;
		}

		DOVirtual.DelayedCall(delay, () => _animator.enabled = true);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		
		//Hulk Smash
		var collisionPoint = other.ClosestPoint(transform.position);
		GameEvents.InvokeMainKartCrash(collisionPoint);

		_rb.isKinematic = false;
		_collider.isTrigger = false;
		transform.DOScale(_initScale, 0.25f);
		StopBouncing();
	}

	public void StopBouncing()
	{
		_animator.enabled = false;
	}

	public void PlayBounceSound()
	{
		_audio.pitch = _initPitch + Random.Range(0, 0.2f);
		_audio.Play();
	}
}
