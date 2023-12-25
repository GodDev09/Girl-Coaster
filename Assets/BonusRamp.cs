using DG.Tweening;
using UnityEngine;

public class BonusRamp : MonoBehaviour
{
	[SerializeField] private BonusTile[] leftTiles;
	[SerializeField] private Color[] colors;

	public float LowestPointY => transform.position.y;
	
	private void Start()
	{
		DOVirtual.DelayedCall(0.2f, GiveColors);
	}

	private void GiveColors()
	{
		if(leftTiles.Length == 0) return;

		var lastToColor = 1;
		var currentStartColor = colors[lastToColor - 1];
		var currentEndColor = colors[lastToColor];

		var perCombo = Mathf.CeilToInt((float) leftTiles.Length / colors.Length) + 1;
		for (var i = 0; i < leftTiles.Length; i++)
		{
			var color = Color.Lerp(currentStartColor, currentEndColor, (float) (i - lastToColor % (perCombo + 1)) / perCombo);
			leftTiles[i].meshRenderer.material.color = color;
			
			if (i == 0 || (i % perCombo) != 0) continue;
			lastToColor++;
			currentStartColor = colors[lastToColor - 1];
			currentEndColor = colors[lastToColor];
		}
	}
	
	
}
