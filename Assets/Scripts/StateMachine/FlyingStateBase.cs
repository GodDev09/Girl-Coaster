using Kart;

namespace StateMachine
{
	public class FlyingStateBase : InputStateBase
	{
		protected static KartFlyMovement Fly;

		protected FlyingStateBase() { }
		public FlyingStateBase(KartFlyMovement fly) => Fly = fly;

		public override void Execute()
		{
			Fly.CalculateForwardMovement();
			Fly.CalculateDownwardMovement();
			Fly.ApplyMovement();
		}
	}

	public sealed class FallingFlyingState : FlyingStateBase
	{
		public override void OnEnter()
		{
			Fly.SetDownwardOrientedValues();
			CameraFxController.only.DoNormalFov();
		}
	}

	public sealed class ForwardFlyingState : FlyingStateBase
	{
		public override void OnEnter()
		{
			Fly.SetForwardOrientedValues();
			CameraFxController.only.DoWideFov();
		}
	}
}