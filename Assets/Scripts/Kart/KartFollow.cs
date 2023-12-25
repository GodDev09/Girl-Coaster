using UnityEngine;

namespace Kart
{
	public class KartFollow : MonoBehaviour
	{
		[SerializeField] private float followOffset, damping;
		private Transform _kartToFollow, _transform;

		private void Start() => _transform = transform;

		private void LateUpdate()
		{
			if(!_kartToFollow) return;
			
			var smoothPos = Vector3.Lerp(_transform.position, _kartToFollow.position + -_kartToFollow.forward * followOffset,
				Time.deltaTime * damping);
			_transform.position = smoothPos;
			_transform.eulerAngles = _kartToFollow.eulerAngles;
		}

		public void SetKartToFollow(Transform kart) => _kartToFollow = kart;
	}
}