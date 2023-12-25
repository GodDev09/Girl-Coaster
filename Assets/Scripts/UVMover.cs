using DG.Tweening;
using UnityEngine;

public class UVMover : MonoBehaviour
{
	[SerializeField] private float oneUvRotationDuration = 0.5f;
	
	private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");

	private void Start()
	{
		var mat = GetComponent<Renderer>().materials[1];
			mat.DOOffset(Vector2.up * -1, oneUvRotationDuration)
			.SetLoops(-1, LoopType.Restart)
			.SetEase(Ease.Linear);
			
		Vector2 EmissionMapOffsetGetter() => mat.GetTextureOffset(EmissionMap); 
		void EmissionMapOffsetSetter(Vector2 value) => mat.SetTextureOffset(EmissionMap, value);

		DOTween.To(EmissionMapOffsetGetter, EmissionMapOffsetSetter, Vector2.up * -1, oneUvRotationDuration)
			.SetLoops(-1, LoopType.Restart)
			.SetEase(Ease.Linear);
	}
}