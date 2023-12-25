using UnityEngine;

namespace StateMachine
{
	public class InputStateBase
	{
		protected InputStateBase() { }

		public virtual void OnEnter() { }

		public virtual void Execute() { }

		public virtual void FixedExecute() { }

		public virtual void OnExit() { }

		public static void print(object message) => Debug.Log(message);
	}
	
	public sealed class DisabledState : InputStateBase { }
}