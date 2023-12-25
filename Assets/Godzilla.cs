using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Godzilla : MonoBehaviour
{
	public float intensity = 10f;
    // Start is called before the first frame update

	private Animator _animator;

	private static readonly int RunHash = Animator.StringToHash("Walk");
	private static readonly int AttackHash = Animator.StringToHash("Attack");

	[SerializeField] private List<Transform> movePoints;
	[SerializeField] private int myAreaCode;
	[SerializeField] private float moveSpeed;

	[SerializeField] private float distanceTolerance = 0.01f;
	[SerializeField] private float rotationTweenTime = 0.5f;

	private bool _toWalk;

	private Vector3 _finalPosition;
	private int _index;

	private void Start()
	{
		_animator = GetComponent<Animator>();

		SetInitialDirection();
	}

	private void Update()
	{
		if (!_toWalk) return;
		
		MoveTheDinosaur();
	}

	private void OnRunAlong(int currentAreaCode)
	{
		if (myAreaCode != currentAreaCode) return;
		_toWalk = true;
		_animator.SetTrigger(RunHash);
	}
	
	private void OnAttackPlayer(int currentAreaCode)
	{
		if (myAreaCode != currentAreaCode) return;
		_animator.SetTrigger(AttackHash);
	}

	private void SetInitialDirection()
	{
		_finalPosition = movePoints[_index].position;
		transform.DORotateQuaternion(Quaternion.LookRotation(_finalPosition - transform.position), rotationTweenTime);
		_toWalk = true;
	}
	
	private void MoveTheDinosaur()
	{
		transform.position = Vector3.MoveTowards(transform.position,
			movePoints[_index].position, 
			Time.deltaTime * moveSpeed);
			
		if (!(Vector3.Distance(transform.position, _finalPosition) <= distanceTolerance)) return;
		
		_index++;
		//if (_index == movePoints.Count) _index = 0;
		if (_index == movePoints.Count) return;
		_finalPosition = movePoints[_index].position;
		
		transform.DORotateQuaternion(Quaternion.LookRotation(_finalPosition - transform.position), rotationTweenTime);
	}
	
	public void FootLandingEffect()
	{
		CameraFxController.only.ScreenShake(intensity);
	}
}
