using Kart;

namespace StateMachine
{
	public class TrackStateBase : InputStateBase
	{
		protected static KartTrackMovement Player { get; private set; }
		protected TrackStateBase() { }

		public TrackStateBase(KartTrackMovement player)
		{
			Player = player;
		}

		protected static float BasicDecelerate() => Player.BasicDecelerate();
		protected static void CalculateForces(float dotPercent) => Player.CalculateForces(dotPercent);

		protected static void CalculateBrakingForces() => Player.CalculateBraking();
	}
	
	public sealed class IdleOnTrackState : TrackStateBase
	{
		public override void Execute()
		{
			Player.BasicDecelerate();
			
			Player.Brake();
			Player.GetAudio.UpdatePitch();
			Player.GetFever.DecreaseFeverAmount();

			CalculateBrakingForces();
		}
	}

	public sealed class MoveOnTrackState : TrackStateBase
	{
		public static bool IsPersistent { get; private set; } = false;
		
		public override void OnEnter()
		{
			Player.StartFollowingTrack();
			Player.GetAudio.PlayIfNotPlaying();
			Player.GetAudio.StartMoving();
		}

		public override void Execute()
		{
			CalculateForces(BasicDecelerate());
			
			Player.Accelerate();
			Player.GetAudio.UpdatePitch();
			Player.GetFever.HandleFeverAmount();

			CalculateBrakingForces();
		}

		public override void OnExit()
		{
			Player.PauseFollow();
			Player.GetAudio.StopMoving();
			
			IsPersistent = false;
		}

		public static void ChangeStatePersistence(bool newStatus) => IsPersistent = newStatus;
	}
}