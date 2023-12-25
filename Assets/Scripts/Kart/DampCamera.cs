using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class DampCamera : MonoBehaviour
	{
		public static DampCamera only;

		public float lerpMul;
		[SerializeField] private Transform target;
		[SerializeField] private float cameraTransitionDuration = 0.5f, perCartBonusCamDelta = 4.4f;
		//percart bonus cam delta calculated by taking difference between 5 carts local z value of -47 and 0 carts local z of -25
		//47-25 = 22
		//22/5 = 4.4f

		[SerializeField] private Transform obstacleOnLeftCam, obstacleOnRightCam, leftHelixCam, rightHelixCam, deathCamPos;
		[SerializeField] private Transform bonusCameraPos, postBonusCamera;

		[SerializeField] private Transform targetMediator;
		private bool _isMediatingTarget;
		
		private Transform _targetParent;
		private AddedKartsManager _player;
		private Transform _transform;
		private Quaternion _initLocalRotation;
		private Vector3 _initLocalPosition, _initBonusCamLocalPosition;

		private void OnEnable()
		{
			GameEvents.EnterHelix += OnEnterHelix;
			GameEvents.ExitHelix += OnExitHelix;

			GameEvents.PlayerDeath += OnPlayerDeath;
			
			GameEvents.ReachEndOfTrack += OnReachEndOfTrack;
			GameEvents.GameWin += OnMainKartEndBonusRampMovement;
		}

		private void OnDisable()
		{
			GameEvents.EnterHelix -= OnEnterHelix;
			GameEvents.ExitHelix -= OnExitHelix;
			
			GameEvents.PlayerDeath -= OnPlayerDeath;
			
			GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;
			GameEvents.GameWin -= OnMainKartEndBonusRampMovement;
		}

		private void Awake()
		{
			if (only) Destroy(this);
			else only = this;
		}
		
		private void Start()
		{
			_transform = transform;
			_player = _transform.parent.GetComponent<AddedKartsManager>();
			_transform.parent = null;
			_targetParent = target.parent;
			
			_initLocalPosition = target.localPosition;
			_initLocalRotation = target.localRotation;

			_initBonusCamLocalPosition = bonusCameraPos.localPosition;
		}

		private void Update()
		{
			if(!_isMediatingTarget) return;
			targetMediator.position = target.position;
			targetMediator.rotation = target.rotation;
		}

		private void LateUpdate()
		{
			if (_isMediatingTarget)
			{
				_transform.position = targetMediator.position;
				_transform.rotation = Quaternion.Lerp(_transform.rotation, targetMediator.rotation, Time.deltaTime * lerpMul);
				 return;
			}
			
			_transform.position = target.position;
			_transform.rotation = Quaternion.Lerp(_transform.rotation, target.rotation, Time.deltaTime * lerpMul);
		}

		public void UpdateFilledKartCount(int filledKarts, bool goSlow = false)
		{ 
			target.DOLocalMoveZ(_initBonusCamLocalPosition.z - (perCartBonusCamDelta * filledKarts), cameraTransitionDuration * (goSlow ? 3 : 1))
				.SetEase(Ease.InSine);
		}

		public void SendToObstacleCam(bool isObstacleOnRight)
		{
			target.DOLocalMove(isObstacleOnRight ? obstacleOnRightCam.localPosition : obstacleOnLeftCam.localPosition, cameraTransitionDuration);
			target.DOLocalRotateQuaternion(isObstacleOnRight ? obstacleOnRightCam.localRotation : obstacleOnLeftCam.localRotation, cameraTransitionDuration);
		}

		public void CameraResetPosition()
		{
			target.DOLocalMove(_initLocalPosition, cameraTransitionDuration);
			target.DOLocalRotateQuaternion(_initLocalRotation, cameraTransitionDuration);
		}
		private void OnEnterHelix(bool isLeftHelix)
		{
			target.DOLocalMove(isLeftHelix ? leftHelixCam.localPosition : rightHelixCam.localPosition, cameraTransitionDuration);
			target.DOLocalRotateQuaternion(isLeftHelix ? leftHelixCam.localRotation : rightHelixCam.localRotation , cameraTransitionDuration); 
		}

		public void OnEnterSpecialCamera(Transform specialCamera, bool slowTransition = false)
		{
			target.parent = null;
			target.DOMove(specialCamera.position, 2f);
			target.DORotateQuaternion(specialCamera.rotation, 0.5f);
		}

		public void OnExitSpecialCamera()
		{
			target.parent = _targetParent;
			CameraResetPosition();
		}

		public Transform TakeControlOfTarget()
		{
			target.parent = null;
			return target;
		}

		public Transform MediateTarget()
		{
			_isMediatingTarget = true;
			
			return targetMediator;
		}

		public void StopMediatingTarget() => _isMediatingTarget = false;

		public void ReleaseControlOfTarget(float overTime)
		{
			target.parent = _targetParent;
			target.DOLocalMove(_initLocalPosition, overTime);
			target.DOLocalRotateQuaternion(_initLocalRotation, overTime);
		}

		private void OnExitHelix() => CameraResetPosition();

		private void OnPlayerDeath()
		{
			target.DOMove(deathCamPos.position, cameraTransitionDuration);
			target.DORotateQuaternion(deathCamPos.rotation, cameraTransitionDuration);
		}

		private void OnReachEndOfTrack()
		{
			target.DOLocalMoveX(bonusCameraPos.localPosition.x, cameraTransitionDuration * 2);
			target.DOLocalMoveY(bonusCameraPos.localPosition.y, cameraTransitionDuration * 2);
			UpdateFilledKartCount(_player.PassengerCount / 2, true);
			
			target.DOLocalRotateQuaternion(bonusCameraPos.localRotation, cameraTransitionDuration);
		}

		private void OnMainKartEndBonusRampMovement()
		{
			target.DOLocalMove(postBonusCamera.localPosition, cameraTransitionDuration);
			target.DOLocalRotateQuaternion(postBonusCamera.localRotation, cameraTransitionDuration);
		}
	}
}
