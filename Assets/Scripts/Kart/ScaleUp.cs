using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class ScaleUp : MonoBehaviour
	{
		private Vector3 _initScale;

		private void Start() => _initScale = transform.localScale;

		public void ScaleMeUp() => transform.DOScale(_initScale * 1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
	}
}