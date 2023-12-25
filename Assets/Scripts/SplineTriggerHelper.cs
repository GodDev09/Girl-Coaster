using DG.Tweening;
using Kart;
using StateMachine;
using UnityEngine;

public class SplineTriggerHelper : MonoBehaviour
{
	private MainKartController _player;

	private void Start()
	{
		_player = GameObject.FindWithTag("Player").GetComponent<MainKartController>();
	}

	public void EnterHighSpeed() => EnterAction();
	public void EnterNormalSpeed() => EnterNormalcy();
	
	public void Cheer()
	{
		if(!AudioManager.instance) return;
		
		AudioManager.instance.Play("Hype" + Random.Range(1, 5));
		AudioManager.instance.Play("Hype" + Random.Range(1, 5));
		AudioManager.instance.Play("Hype" + Random.Range(1, 5));
	}

	public void EnterHypeArea()
	{
		//if(AudioManager.instance) AudioManager.instance.Play("Hype" + Random.Range(1, 5));
		if (AudioManager.instance)
		{
			var random = Random.Range(1, 3);
			AudioManager.instance.Play("Jump" + random);
		}

		GameEvents.InvokeUpdateHype(true);
	}

	public void ExitHypeArea() => 	GameEvents.InvokeUpdateHype(false);

	public void RemoveInputControl() => MoveOnTrackState.ChangeStatePersistence(true);
	public void RestoreInputControl() => MoveOnTrackState.ChangeStatePersistence(false);
	
	public void EnterLeftHelix()
	{
		EnterAction();
		
		_player.PlayerAudio.DistantCameraDistanceVolume();
		CameraFxController.only.SetSpeedLinesStatus(false);
		GameEvents.InvokeEnterHelix(true);
	}

	public void EnterRightHelix()
	{
	    EnterAction();
    	
		_player.PlayerAudio.DistantCameraDistanceVolume();
		CameraFxController.only.SetSpeedLinesStatus(false);
		GameEvents.InvokeEnterHelix(false);
	}

	public void ExitHelix()
	{
		EnterNormalcy();
		
		_player.PlayerAudio.NormalCameraDistanceVolume();
		GameEvents.InvokeExitHelix();
	}

	public void PassengerJump()
	{
		_player.AddedKartsManager.MakePassengersJump(1);
		CameraFxController.only.DoCustomFov(75);
		_player.PlayerAudio.SlowMoPitch();
		TimeController.only.SlowDownTime();
		GameEvents.InvokeUpdateHype(true);
		RemoveInputControl();
		
		if(AudioManager.instance) AudioManager.instance.Play("Jump" + Random.Range(1, 3));

		DOVirtual.DelayedCall(0.75f, () =>
		{
			TimeController.only.RevertTime();
			_player.PlayerAudio.NormalTimeScalePitch();
			RestoreInputControl();
		});
	}
	
	public void PassengerJumpUninterrupted()
	{
		_player.AddedKartsManager.MakePassengersJump(1);
		CameraFxController.only.DoCustomFov(75);
		_player.PlayerAudio.SlowMoPitch();
		TimeController.only.SlowDownTime();
		GameEvents.InvokeUpdateHype(true);
		RemoveInputControl();
		if(AudioManager.instance) AudioManager.instance.Play("Jump");

		DOVirtual.DelayedCall(0.75f, () =>
		{
			TimeController.only.RevertTime();
			_player.PlayerAudio.NormalTimeScalePitch();
//			RestoreInputControl();
		});
	}

	public void PassengerJumpNoSloMo()
	{
		_player.AddedKartsManager.MakePassengersJump(1);
		CameraFxController.only.DoCustomFov(75);
		GameEvents.InvokeUpdateHype(true);
	}

	public void PassengerJumpCustomDuration(float duration = 1f)
	{
		_player.AddedKartsManager.MakePassengersJump(duration);
		CameraFxController.only.DoCustomFov(75);
		_player.PlayerAudio.SlowMoPitch();
		TimeController.only.SlowDownTime();
		GameEvents.InvokeUpdateHype(true);
		RemoveInputControl();
		if(AudioManager.instance) AudioManager.instance.Play("Jump" + Random.Range(1, 3));

		DOVirtual.DelayedCall(duration * 0.75f, () =>
		{
			TimeController.only.RevertTime();
			_player.PlayerAudio.NormalTimeScalePitch();
			RestoreInputControl();
		});
	}

	public void OnReachTrackEnd()
	{
		GameEvents.InvokeReachEndOfTrack();
		EnterHypeArea();
		
		if(AudioManager.instance)
			AudioManager.instance?.Play("ReachEndTrack");
		
		GameEvents.InvokePlayerOffFever();
	}

	public void EnterArea(int currentAreaCode) => GameEvents.InvokeStartParade(currentAreaCode);

	public void AttackAction(int currentAreaCode) => GameEvents.InvokeAttackPlayer(currentAreaCode);

	public void OnEnterSpecialCamera(Transform specialCamera) => DampCamera.only.OnEnterSpecialCamera(specialCamera);
	public void OnEnterSpecialCameraSlow(Transform specialCamera) => DampCamera.only.OnEnterSpecialCamera(specialCamera, true);

	public void OnExitSpecialCamera() => DampCamera.only.OnExitSpecialCamera();

	private void EnterNormalcy()
	{
		_player.TrackMovement.SetNormalSpeedValues();
		ExitHypeArea();
		CameraFxController.only.SetSpeedLinesStatus(false);
		CameraFxController.only.DoNormalFov();
	}


	private void EnterAction()
	{
		_player.TrackMovement.SetHighSpeedValues();
		CameraFxController.only.SetSpeedLinesStatus(true);
		EnterHypeArea();
		CameraFxController.only.DoWideFov();
	}
}
