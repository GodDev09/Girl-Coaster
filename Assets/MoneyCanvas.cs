using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyCanvas : MonoBehaviour
{
	[SerializeField] public TextMeshProUGUI moneyText;
	[SerializeField] private GameObject moneyImage;
	[SerializeField] private Transform moneyDestination;
	[SerializeField] private int currencyValue;

	private Tweener _moneyTween;
	private int _moneyCount, _moneyMultiplier;

	private void Start()
	{
		GetComponent<Canvas>().worldCamera = Camera.main;
		UpdateMoneyCount();
	}

	public void IncreaseMoneyCount()
	{
		_moneyCount += currencyValue * _moneyMultiplier;

		moneyText.text = _moneyCount.ToString();
		UpgradeShopCanvas.only.AddCollectedMoney(currencyValue * _moneyMultiplier);
	}

	public void UpdateMultiplier(int multiplier) => _moneyMultiplier = multiplier;

	public void ScaleMoneyImage()
	{
		if (_moneyTween.IsActive()) _moneyTween.Kill(true);
		
		_moneyTween = moneyImage.transform.DOScale(Vector3.one * 1.15f, 0.1f)
			.SetLoops(2,LoopType.Yoyo);
	}

	public void UpdateMoneyCount()
	{
		_moneyCount = ShopStateController.CurrentState.GetState().CoinCount;
		moneyText.text = _moneyCount.ToString();
	}
	
	public Transform GetMoneyDestination() => moneyDestination;
}
