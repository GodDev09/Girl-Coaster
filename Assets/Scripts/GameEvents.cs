using System;
using UnityEngine;

public static class GameEvents
{
	public static event Action TapToPlay;
	public static event Action<bool> UpdateHype;
	public static event Action<bool> EnterHelix;
	public static event Action ExitHelix;

	public static event Action<int> StartParade;
	public static event Action<int> AttackPlayer;

	public static event Action<Vector3> MainKartCrash, KartCrash;
	public static event Action PlayerDeath;
	public static event Action ReachEndOfTrack;

	//invoked when the coasters completely stops on the bonus ramp
	public static event Action RunOutOfPassengers;

	public static event Action GameWin;

	public static event Action PlayerOnFever;
	public static event Action PlayerOffFever;

	public static event Action ObstacleWarningOn;
	public static event Action ObstacleWarningOff;

	public static event Action JumpInBathTub;

	public static void InvokeTapToPlay() => TapToPlay?.Invoke();

	public static void InvokeMainKartCrash(Vector3 collisionPoint) => MainKartCrash?.Invoke(collisionPoint);
	public static void InvokeKartCrash(Vector3 collisionPoint) => KartCrash?.Invoke(collisionPoint);
	public static void InvokePlayerDeath() => PlayerDeath?.Invoke();
	public static void InvokeUpdateHype(bool status) => UpdateHype?.Invoke(status);
	public static void InvokeEnterHelix(bool isLeftHelix) => EnterHelix?.Invoke(isLeftHelix);
	public static void InvokeExitHelix() => ExitHelix?.Invoke();

	public static void InvokeReachEndOfTrack() => ReachEndOfTrack?.Invoke();

	public static void InvokeRunOutOfPassengers() => RunOutOfPassengers?.Invoke();

	public static void InvokeGameWin() => GameWin?.Invoke();
	public static void InvokeStartParade(int currentAreaCode) => StartParade?.Invoke(currentAreaCode);
	public static void InvokeAttackPlayer(int currentAreaCode) => AttackPlayer?.Invoke(currentAreaCode);
	
	public static void InvokePlayerOnFever() => PlayerOnFever?.Invoke();
	public static void InvokePlayerOffFever() => PlayerOffFever?.Invoke();
	
	public static void InvokeObstacleWarningOn() => ObstacleWarningOn?.Invoke();
	public static void InvokeObstacleWarningOff() => ObstacleWarningOff?.Invoke();

	public static void InvokeJumpInBathTub() => JumpInBathTub?.Invoke();
}