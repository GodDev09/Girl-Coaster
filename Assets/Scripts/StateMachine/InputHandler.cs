using System;
using Kart;
using UnityEngine;

public enum InputState { Disabled, IdleOnTracks, MoveOnTracks, FallingFlying, ForwardFlying }

namespace StateMachine
{
	public class InputHandler : MonoBehaviour
	{
		//current state holder	
		private static InputStateBase _currentInputState;

		//all states
		private static readonly DisabledState DisabledState = new DisabledState();
		//track
		private static readonly IdleOnTrackState IdleOnTrackState = new IdleOnTrackState();
		private static readonly MoveOnTrackState MoveOnTrackState = new MoveOnTrackState();
		//flying
		private static readonly ForwardFlyingState ForwardFlyingState = new ForwardFlyingState();
		private static readonly FallingFlyingState FallingFlyingState = new FallingFlyingState();
		
		private bool _hasTappedToPlay;

		private void OnEnable()
		{
			GameEvents.TapToPlay += OnTapToPlay;
			GameEvents.ReachEndOfTrack += OnReachEndOfTrack;
			GameEvents.RunOutOfPassengers += OnStopOnBonusRamp;
			GameEvents.PlayerDeath += OnPlayerDeath;
		}

		private void OnDisable()
		{
			GameEvents.TapToPlay -= OnTapToPlay;
			GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;
			GameEvents.RunOutOfPassengers -= OnStopOnBonusRamp;
			GameEvents.PlayerDeath -= OnPlayerDeath;
		}

		private void Start()
		{
			InputExtensions.IsUsingTouch = Application.platform != RuntimePlatform.WindowsEditor &&
										   (Application.platform == RuntimePlatform.Android ||
											Application.platform == RuntimePlatform.IPhonePlayer);
			InputExtensions.TouchInputDivisor = MyHelpers.RemapClamped(1920, 2400, 30, 20, Screen.height);

			var player = GameObject.FindGameObjectWithTag("Player");

			_ = new TrackStateBase(player.GetComponent<KartTrackMovement>());
			_ = new FlyingStateBase(player.GetComponent<KartFlyMovement>());

			_currentInputState = IdleOnTrackState;
		}

		private void Update()
		{
			if(!_hasTappedToPlay) return;
			
			//print($"{_currentInputState}");
			if(!(_currentInputState is DisabledState) && !(_currentInputState is MoveOnTrackState && MoveOnTrackState.IsPersistent))
			{
				var newState = HandleInput();

				if (_currentInputState != newState)
				{
					_currentInputState?.OnExit();
					_currentInputState = newState;
					_currentInputState?.OnEnter();
				}
			}

			_currentInputState?.Execute();
		}

		private void FixedUpdate()
		{
			_currentInputState?.FixedExecute();
		}

		private static InputStateBase HandleInput()
		{
			if (_currentInputState is TrackStateBase)
			{
				if (InputExtensions.GetFingerHeld())
					return MoveOnTrackState;
				
				return IdleOnTrackState;
			}
			
			if (_currentInputState is FlyingStateBase)
			{
				if (InputExtensions.GetFingerHeld())
					return ForwardFlyingState;
				
				return FallingFlyingState;
			}
			
			return _currentInputState;
		}

		public static void AssignNewState(InputState state)
		{
			_currentInputState?.OnExit();
			_currentInputState = state switch
			{
				InputState.Disabled => DisabledState,
				InputState.IdleOnTracks => IdleOnTrackState,
				InputState.MoveOnTracks => MoveOnTrackState,
				InputState.FallingFlying => FallingFlyingState,
				InputState.ForwardFlying => ForwardFlyingState,
				_ => throw new ArgumentOutOfRangeException(nameof(state), state,
					"aisa kya pass kar diya vro tune yahaan")
			};

			_currentInputState?.OnEnter();
		}

		private static void AssignNewState(InputStateBase newState)
		{
			_currentInputState?.OnExit();
			_currentInputState = newState;
			_currentInputState?.OnEnter();
		}

		private void OnTapToPlay() => _hasTappedToPlay = true;

		private static void OnReachEndOfTrack() => AssignNewState(InputState.FallingFlying);

		private static void OnStopOnBonusRamp() => AssignNewState(InputState.Disabled);

		private static void OnPlayerDeath() => AssignNewState(InputState.Disabled);
	}
}