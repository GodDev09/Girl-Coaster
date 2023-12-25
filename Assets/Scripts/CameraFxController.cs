using DG.Tweening;
using Kart;
using UnityEngine;

public class CameraFxController : MonoBehaviour
{
	public static CameraFxController only;

	[SerializeField] private GameObject speedParticleSystem;
	[SerializeField] private float shakeDuration = 5f,shakeStrength = 5f;
	
	private Camera _cam;
	private Tween _fovTween, _screenShakeEnd;

	private Tweener _screenShakeTween;

	private Tweener _cameraRumbleTween;

	[SerializeField] private float rumbleDuration = 0.005f;
	[SerializeField] private float rumbleMaxRotation = 0.5f;
	[SerializeField] private float rumbleCameraFovDelta = 2.5f;
	private float _defaultFov = 60f;
	private Vector3 _positiveRumbler;

	private DampCamera _dampCamera;

	private bool _inCollisionCoolDown;
	
	private void Awake()
	{
		if (!only) only = this;
		else Destroy(only);
	}

	private void OnEnable()
	{
		GameEvents.PlayerDeath += OnPlayerDeath;
		GameEvents.ReachEndOfTrack += OnReachEndOfTrack;
		GameEvents.RunOutOfPassengers += OnRunOutOfPassengers;
		GameEvents.PlayerOnFever += OnFever;
		GameEvents.PlayerOffFever += OffFever;
	}

	private void OnDisable()
	{
		GameEvents.PlayerDeath -= OnPlayerDeath;
		GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;
		GameEvents.RunOutOfPassengers -= OnRunOutOfPassengers;
		GameEvents.PlayerOnFever -= OnFever;
		GameEvents.PlayerOffFever -= OffFever;
	}

	private void Start()
	{
		_cam = Camera.main;
		_positiveRumbler = Vector3.forward * rumbleMaxRotation;
		_dampCamera = DampCamera.only;
	}

	public void DoNormalFov()
	{
		if (_fovTween.IsActive()) _fovTween.Kill();
		_fovTween = _cam.DOFieldOfView(60, 0.5f);
	}

	public void DoWideFov()
	{
		if (_fovTween.IsActive()) _fovTween.Kill();
		_fovTween = _cam.DOFieldOfView(70, 0.5f);
	}

	public void DoCustomFov(float fov)
	{
		if (_fovTween.IsActive()) _fovTween.Kill();
		_fovTween = _cam.DOFieldOfView(fov, 0.5f);
	}

	public void DoCollisionFov(float fov)
	{
		if (_fovTween.IsActive()) _fovTween.Kill();
		_fovTween = _cam.DOFieldOfView(fov, 0.1f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutElastic);
	}
	
	private void DoFeverFov(float fov)
	{
		if (_fovTween.IsActive()) _fovTween.Kill();
		_fovTween = _cam.DOFieldOfView(fov, 0.25f).SetEase(Ease.OutBack);
	}

	public void ScreenShake(float intensity)
	{
		var target = _dampCamera.MediateTarget();

		if (_screenShakeTween.IsActive()) _screenShakeTween.Kill(true);
		_screenShakeTween = target.DOShakePosition(shakeDuration, shakeStrength * intensity, 10, 45f)
			.OnComplete(RequestEndScreenShaker);
	}

	private void RequestEndScreenShaker()
	{
		if (_screenShakeEnd.IsActive()) _screenShakeEnd.Kill();
		
		_screenShakeEnd = DOVirtual.DelayedCall(0.5f, () => DampCamera.only.StopMediatingTarget());
	}
	
	public void StartCameraRumble()
	{
		_cameraRumbleTween.Kill(true);
		
		_dampCamera.MediateTarget().rotation = Quaternion.Euler(_dampCamera.MediateTarget().eulerAngles.x, _dampCamera.MediateTarget().eulerAngles.y, 0);
		_cameraRumbleTween = _dampCamera.MediateTarget().DOLocalRotate(_positiveRumbler, rumbleDuration, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Yoyo);
	}

	public void UpdateRumblerRotation(float lerpValue)
	{
		_cameraRumbleTween.ChangeEndValue(Vector3.forward * Mathf.Lerp(0, rumbleMaxRotation, lerpValue));
		_cam.fieldOfView = Mathf.Lerp(_defaultFov, _defaultFov + rumbleCameraFovDelta, lerpValue);
	}

	public void EndCameraRumble() => _cameraRumbleTween.Kill(true);
	
	public void SetSpeedLinesStatus(bool status) => speedParticleSystem.SetActive(status);

	public void ObstacleCollisionFov()
	{
		if (_inCollisionCoolDown) return;
		
		_inCollisionCoolDown = true;
		DoCollisionFov(56);
		DOVirtual.DelayedCall(2f, ResetCollisionCooldown);
	}

	private void ResetCollisionCooldown() => _inCollisionCoolDown = false;

	private void OnPlayerDeath() => SetSpeedLinesStatus(false);

	private void OnReachEndOfTrack() => SetSpeedLinesStatus(true);

	private void OnRunOutOfPassengers() => SetSpeedLinesStatus(false);

	private void OnFever()
	{
		DoFeverFov(90);
		//StartCameraRumble();
		SetSpeedLinesStatus(true);
	}

	private void OffFever()
	{
		DoNormalFov();
		//EndCameraRumble();
		SetSpeedLinesStatus(false);
	}
}
