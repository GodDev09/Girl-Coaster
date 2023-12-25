using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ParadeAnimal : MonoBehaviour
{
	private Animator _animator;

	private static readonly int RunHash = Animator.StringToHash("Run");
	private static readonly int AttackHash = Animator.StringToHash("Attack");

	[SerializeField] private List<Transform> movePoints;
	[SerializeField] private int myAreaCode;
	[SerializeField] private float moveSpeed;

	[SerializeField] private float distanceTolerance = 0.01f;
	[SerializeField] private float rotationTweenTime = 0.5f;

	private bool _toRun;

	private Vector3 _finalPosition;
	private int _index;

	private void OnEnable()
	{
		GameEvents.StartParade += OnRunAlong;
		GameEvents.AttackPlayer += OnAttackPlayer;
	}

	private void OnDisable()
	{
		GameEvents.StartParade -= OnRunAlong;
		GameEvents.AttackPlayer -= OnAttackPlayer;
	}
	
	private void Start()
	{
		_animator = GetComponent<Animator>();
		_toRun = true;
		SetInitialDirection();
	}

	private void Update()
	{
		if (!_toRun) return;
		
		MoveTheDinosaur();
	}

	private void OnRunAlong(int currentAreaCode)
	{
		//if (myAreaCode != currentAreaCode) return;
		_toRun = true;
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
	}
	
	private void MoveTheDinosaur()
	{
		transform.position = Vector3.MoveTowards(transform.position,
			movePoints[_index].position, 
			Time.deltaTime * moveSpeed);
			
		if (!(Vector3.Distance(transform.position, _finalPosition) <= distanceTolerance)) return;
		
		_index++;
		if (_index == movePoints.Count) _index = 0;
		_finalPosition = movePoints[_index].position;
		
		transform.DORotateQuaternion(Quaternion.LookRotation(_finalPosition - transform.position), rotationTweenTime);
	}
}
