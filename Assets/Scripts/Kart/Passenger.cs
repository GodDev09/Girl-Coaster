using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class Passenger : MonoBehaviour
	{
		[Header("Jump"), SerializeField] private float distance;
		[SerializeField] private float upTime, holdTime, downTime;
		private Animator _anim;

		private Sequence _jumpSequence;
		private Transform _transform;
		private float _initLocalY;
		private bool _isHypedUp;

		private static readonly int Hype = Animator.StringToHash("Hype");
		private static readonly int Dance = Animator.StringToHash("Win");
		private Tween _delayedCall;

		private void OnEnable()
		{
			GameEvents.UpdateHype += OnUpdateHype;
		}

		private void OnDisable()
		{
			GameEvents.UpdateHype -= OnUpdateHype;
		}

		private void Start()
		{
			_anim = GetComponent<Animator>();
			
			_transform = transform;
			_initLocalY = _transform.localPosition.y;
		}

		private void OnUpdateHype(bool newStatus)
		{
			if(_isHypedUp == newStatus) return;
			
			_anim.SetBool(Hype, newStatus);
			_isHypedUp = newStatus;
		}

		public void MakePassengerJump(float duration, float delay)
		{
			if (delay < 0.01f)
			{
				_jumpSequence.Restart();
				return;
			}
			
			_delayedCall = DOVirtual.DelayedCall(delay, () => Jump(duration));
			_delayedCall.SetRecyclable(true);
		}

		public void StartDancing() => _anim.SetTrigger(Dance);

		private void Jump(float duration)
		{
			_jumpSequence = DOTween.Sequence();
			
			_jumpSequence.AppendCallback(() =>
			{
				_anim.SetBool(Hype, true);
				_transform.localPosition = new Vector3(transform.localPosition.x, _initLocalY, _transform.localPosition.z);
			});
			
			_jumpSequence.Join(_transform.DOLocalMoveY(_initLocalY + distance, upTime * duration));
			//_jumpSequence.AppendInterval(holdTime * duration);
			_jumpSequence.AppendCallback(() => _anim.SetBool(Hype, false));
			_jumpSequence.Join(transform.DOLocalMoveY(_initLocalY, downTime * duration));

			_jumpSequence.SetRecyclable(true);
		}
	}
}