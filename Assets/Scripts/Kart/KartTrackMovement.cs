using DG.Tweening;
using Dreamteck.Splines;
using StateMachine;
using UnityEngine;

namespace Kart
{
	[System.Serializable] public struct Limits { public float min, max; }
	public class KartTrackMovement : MonoBehaviour
	{
		private MainKartController _my;
		private SplineFollower _follower;
		
		[SerializeField] private float currentSpeed = 10f;
		[SerializeField] private AnimationCurve speedGain, speedLoss;

		[SerializeField] private Limits plainSpeedLimits, highSpeedLimits;
		[SerializeField] private float brakeSpeed = 0f, brakeReleaseSpeed = 0f;

		[SerializeField] private float frictionForce = 0.1f, gravityForce = 1f;
		[SerializeField] private float slopeRange = 60f;

		public Limits CurrentLimits => _currentLimits;
		public float GetCurrentSpeed() => currentSpeed;
		private void SetCurrentSpeed(float value) => currentSpeed = value;
		
		private Limits _currentLimits;

		private const float BrakeTime = 0f;
		private float _brakeForce = 0f, _addForce = 0f;
		private bool _toMove;
		private bool _kartSlowedDownDueToCrash;

		private void OnEnable()
		{
			GameEvents.MainKartCrash += OnMainKartCrash;
			GameEvents.PlayerDeath += OnPlayerDeath;
			GameEvents.ReachEndOfTrack += OnReachEndOfTrack;
		}

		private void OnDisable()
		{
			GameEvents.MainKartCrash -= OnMainKartCrash;
			GameEvents.PlayerDeath -= OnPlayerDeath;
			GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;
		}

		private void Start()
		{
			_follower = GetComponent<SplineFollower>();
			_my = GetComponent<MainKartController>();
			_currentLimits = plainSpeedLimits;
		}

		public float BasicDecelerate()
		{
			var dot = Vector3.Dot(transform.forward, Vector3.down);
			var dotPercent = Mathf.Lerp(-slopeRange / 90f, slopeRange / 90f, (dot + 1f) / 2f);
			currentSpeed -= Time.deltaTime * frictionForce * (1f - _brakeForce);

			return dotPercent;
		}
		
		public void CalculateForces(float dotPercent)
		{
			var speedAdd = 0f;
			var speedPercent = Mathf.InverseLerp(_currentLimits.min, _currentLimits.max, currentSpeed);
			if (dotPercent > 0f)
				speedAdd = gravityForce * dotPercent * speedGain.Evaluate(speedPercent) * Time.deltaTime;
			else
				speedAdd = gravityForce * dotPercent * speedLoss.Evaluate(1f - speedPercent) * Time.deltaTime;
			currentSpeed += speedAdd * (1f - _brakeForce);
			currentSpeed = Mathf.Clamp(currentSpeed, _currentLimits.min, _currentLimits.max);
			if (_addForce < 0f) return;
		
			var lastAdd = _addForce;
			_addForce = Mathf.MoveTowards(_addForce, 0f, Time.deltaTime * 30f);
			currentSpeed += lastAdd - _addForce;
		}

		public void Accelerate()
		{
			_follower.followSpeed = currentSpeed;
			_follower.followSpeed *= 1f - _brakeForce;
		}

		public void Brake()
		{
			_follower.followSpeed -= Time.deltaTime * currentSpeed;
			_my.fever.DecreaseFeverAmount();
		}

		public void CalculateBraking()
		{
			_brakeForce = BrakeTime > Time.time ?
				Mathf.MoveTowards(_brakeForce, 1f, Time.deltaTime * brakeSpeed) :
				Mathf.MoveTowards(_brakeForce, 0f, Time.deltaTime * brakeReleaseSpeed);
		}

		public PlayerAudio GetAudio => _my.PlayerAudio;
		public Fever GetFever => _my.fever;

		public void StartFollowingTrack() => _follower.follow = true;
		public void StopFollowingTrack() => _follower.follow = false;

		public void PauseFollow() => _my.PlayerAudio.StopMoving();

		public void SetHighSpeedValues() => _currentLimits = highSpeedLimits;

		public void SetNormalSpeedValues() => _currentLimits = plainSpeedLimits;

		private void KnockBack()
		{
			_follower.applyDirectionRotation = false;
			_follower.direction = Spline.Direction.Backward;
			InputHandler.AssignNewState(InputState.Disabled);

			DOVirtual.DelayedCall(0.35f, RestoreMovingForward);
		}

		private void RestoreMovingForward()
		{
			_follower.applyDirectionRotation = true;
			_follower.direction = Spline.Direction.Forward;
			InputHandler.AssignNewState(InputState.IdleOnTracks);
		}

		private void OnMainKartCrash(Vector3 point)
		{
			if(_my.AddedKartsManager.isObstacleMainKart) return;
			
			if(_kartSlowedDownDueToCrash) return;
			_kartSlowedDownDueToCrash = true;
			
			var speed = currentSpeed;
			currentSpeed = 0f;

			KnockBack();
			DOTween.To(GetCurrentSpeed, SetCurrentSpeed, speed - (speed - _currentLimits.min) / 2, 0.25f)
				.OnComplete(() => DOTween.To(GetCurrentSpeed, SetCurrentSpeed, speed, 0.25f).SetDelay(2f)
					.OnComplete(() => _kartSlowedDownDueToCrash = false));
		}

		private void OnPlayerDeath() => StopFollowingTrack();

		private void OnReachEndOfTrack() => StopFollowingTrack();

		public void AddSpeedBoost() => DOTween.To(() => currentSpeed, value => currentSpeed = value, _currentLimits.max, 0.5f).SetRecyclable(true);
	}
}