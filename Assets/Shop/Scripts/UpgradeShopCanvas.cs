using DG.Tweening;
using Kart;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum KartSkin
{
	TrainWagon,
	BathTub,
	Toilet,
	WaterTube,
	DustBin,
	Duck
}

public enum CharacterSkin
{
	StickmanRed,
	StickmanBlue
}

public class UpgradeShopCanvas : MonoBehaviour
{
	public static UpgradeShopCanvas only;

	public KartSkin MyKartSkin;
	public CharacterSkin MyCharacterSkin;
	
	[SerializeField] private Sprite normalBtn;
	[SerializeField] private int[] feverLevelCosts, moneyLevelCosts;
	[SerializeField] private Button feverButton, moneyButton;
	[SerializeField] private GameObject feverHand, moneyHand;
	[SerializeField] private TextMeshProUGUI feverMultiplier, feverCostText, moneyMultiplier, moneyCostText;
	[SerializeField] private Animation feverButtonPressAnimation, moneyButtonPressAnimation;

	[Header("Coin Particle Effect"), SerializeField] private RectTransform coinHolder;
	[SerializeField] private ParticleControlScript coinParticles;
	
	private const float CooldownTimerDuration = 0.25f;
	private bool _allowedToPressButton = true;

	private Fever _fever;
	private MoneyCanvas _money;
	private Animation _anim;
	private int _currentFeverLevel, _currentMoneyLevel, _collectedMoney;

	private static int GetSidebarWagon() => ShopStateController.CurrentState.GetState().SidebarWagon;

	public static void AlterCoinCount(int change)
	{
		ShopStateController.CurrentState.GetState().CoinCount += change;
		
		ShopStateController.ShopStateSerializer.SaveCurrentState();
		MainShopController.Main.ReadCurrentShopState();
		only._money.UpdateMoneyCount();
	}

	public void AddCollectedMoney(int change) => _collectedMoney += change;

	public void SaveCollectedMoney() => AlterCoinCount(_collectedMoney);

	private static int GetCoinCount() => ShopStateController.CurrentState.GetState().CoinCount;

	private void Awake()
	{
		if (!only) only = this;
		else Destroy(gameObject);
	}

	private void OnEnable()
	{
		GameEvents.TapToPlay += OnTapToPlay;
		
		ShopEvents.WagonSkinSelect += OnWagonPurchase;
	}

	private void OnDisable()
	{
		GameEvents.TapToPlay -= OnTapToPlay;
		
		ShopEvents.WagonSkinSelect -= OnWagonPurchase;
	}

	private void Start()
	{
		_anim = GetComponent<Animation>();
		_fever = GameObject.FindGameObjectWithTag("Player").GetComponent<MainKartController>().fever;
		_money = GameObject.FindGameObjectWithTag("MoneyCanvas").GetComponent<MoneyCanvas>();

		_currentFeverLevel = ShopStateController.CurrentState.GetCurrentFeverLevel();
		_currentMoneyLevel = ShopStateController.CurrentState.GetCurrentMoneyLevel();
		
		_fever.UpdateFeverShopMultiplier(_currentFeverLevel);
		_money.UpdateMultiplier(_currentMoneyLevel);

		UpdateButtons();
	}

	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.O)) return;

		AlterCoinCount(500);
		UpdateButtons();
	}
	
	public void UpdateButtons()
	{
		//update speed and power texts and icons
		if(_currentFeverLevel < feverLevelCosts.Length - 1)
		{
			feverMultiplier.text = "Fever: x" + (_currentFeverLevel + 1);
			feverCostText.text = feverLevelCosts[_currentFeverLevel + 1].ToString();
			
			feverButton.interactable = GetCoinCount() >= feverLevelCosts[_currentFeverLevel + 1];
			/*
			if (GetCoinCount() >= feverLevelCosts[_currentFeverLevel + 1])
			{
				feverButton.image.sprite = normalBtn;
				feverCostText.gameObject.SetActive(true);
			}
			else
			{
				//show ads
				feverButton.image.sprite = normalBtn;
				feverCostText.gameObject.SetActive(true);
			}*/
		}
		else
		{
			feverCostText.text = "MAX";
			feverMultiplier.text = "MAX";
			feverButton.interactable = false;
		}
		feverHand.SetActive(feverButton.interactable);
		
		if(_currentMoneyLevel < moneyLevelCosts.Length - 1)
		{
			moneyMultiplier.text = "Money: x" + (_currentMoneyLevel + 1);
			moneyCostText.text = moneyLevelCosts[_currentMoneyLevel + 1].ToString();
			
			moneyButton.interactable = GetCoinCount() >= moneyLevelCosts[_currentMoneyLevel + 1];
			
			/*if (GetCoinCount() < moneyLevelCosts[_currentMoneyLevel + 1])
			{
				moneyButton.image.sprite = normalBtn;
				moneyCostText.gameObject.SetActive(true);
			}
			else
			{
				moneyButton.image.sprite = normalBtn;
				moneyCostText.gameObject.SetActive(true);
			}*/
		}
		else
		{
			moneyCostText.text = "MAX";
			moneyMultiplier.text = "MAX";
			moneyButton.interactable = false;
			print(feverButton.interactable);
		}
		moneyHand.SetActive(moneyButton.interactable);
	}

	private static void FindNewSideBarWagon(int currentWeapon)
	{
		var changed = false;
		
		//find a weapon from current index to last
		for (var i = currentWeapon + 1; i < MainShopController.GetWagonSkinCount(); i++)
		{
			if (ShopStateController.CurrentState.GetState().wagonStates[(WagonType) i] != ShopItemState.Locked)
				continue;

			ShopStateController.CurrentState.SetNewSideBarWagon(i);
			changed = true;
			break;
		}

		//if all weapons after me are unlocked, try to find new before me
		if (!changed)
		{
			for (var i = 1; i < currentWeapon; i++)
			{
				if (ShopStateController.CurrentState.GetState().wagonStates[(WagonType) i] != ShopItemState.Locked)
					continue;

				ShopStateController.CurrentState.SetNewSideBarWagon(i);
				changed = true;
				break;
			}
		}

		//if still didn't find anything make sure "MAX" is written
		if (!changed)
			ShopStateController.CurrentState.AllWagonsHaveBeenUnlocked();
	}

	public void ClickOnBuyFever()
	{
		if(!_allowedToPressButton) return;
		
		BuyFever();
		if(AudioManager.instance) AudioManager.instance.Play("BuyUpgrade");

		_allowedToPressButton = feverButton.interactable = false;
		DOVirtual.DelayedCall(CooldownTimerDuration, () =>
		{
			_allowedToPressButton = feverButton.interactable =
				feverLevelCosts.Length - 1 != _currentFeverLevel;
			
			UpdateButtons();
		});
	}

	public void ClickOnBuyMoney()
	{
		if(!_allowedToPressButton) return;
		
		BuyMoney();
		if(AudioManager.instance) AudioManager.instance.Play("BuyUpgrade");
		
		_allowedToPressButton = moneyButton.interactable = false;
		DOVirtual.DelayedCall(CooldownTimerDuration, () =>
		{
			_allowedToPressButton = moneyButton.interactable =
				moneyLevelCosts.Length - 1 != _currentMoneyLevel;
			UpdateButtons();
		});
	}

	private void BuyFever()
	{
		feverButtonPressAnimation.Play();
		
		AlterCoinCount(-feverLevelCosts[++_currentFeverLevel]);

		_fever.UpdateFeverShopMultiplier(_currentFeverLevel);
		ShopStateController.CurrentState.SetNewFeverLevel(_currentFeverLevel);
		
		ShopStateController.ShopStateSerializer.SaveCurrentState();
		MainShopController.Main.ReadCurrentShopState();
		
		UpdateButtons();
		AudioManager.instance.Play("Button");
	}

	private void BuyMoney()
	{
		moneyButtonPressAnimation.Play();
		
		AlterCoinCount(-moneyLevelCosts[++_currentMoneyLevel]);
		
		_money.UpdateMultiplier(_currentMoneyLevel);
		ShopStateController.CurrentState.SetNewMoneyLevel(_currentMoneyLevel);
		
		ShopStateController.ShopStateSerializer.SaveCurrentState();
		MainShopController.Main.ReadCurrentShopState();

		UpdateButtons();
		AudioManager.instance.Play("Button");
		//confetti and/or power up vfx
	}

	private void OnWagonPurchase(int index, bool shouldDeductCoins)
	{
		if(index != GetSidebarWagon()) return;
		
		FindNewSideBarWagon(GetSidebarWagon());

		DOVirtual.DelayedCall(0.25f, UpdateButtons);
	}
	
	private void OnTapToPlay() => _anim.Play();

	public void IncreaseCoinsBy(int coinIncreaseCount)
	{
		var seq = DOTween.Sequence();
		seq.AppendInterval(1.25f);

		var coinText = MainShopController.Main.GetCoinText();
		var initSize = MainShopController.Main.GetCoinText().fontSize;

		var dummyCoinCount = GetCoinCount();

		seq.AppendCallback(() => AudioManager.instance.Play("CoinCollect"));
		seq.Append(DOTween.To(() => coinText.fontSize, value => coinText.fontSize = value, initSize * 1.2f, .5f).SetEase(Ease.OutQuart));
		seq.Join(DOTween.To(() => dummyCoinCount, value => dummyCoinCount = value, dummyCoinCount + coinIncreaseCount,
			.5f).OnUpdate(() => coinText.text = dummyCoinCount.ToString()));
		seq.InsertCallback(.75f, () => coinParticles.PlayControlledParticles(coinParticles.transform.position, coinHolder));
		seq.Append(DOTween.To(() => coinText.fontSize, value => coinText.fontSize = value, initSize, .5f).SetEase(Ease.OutQuart));
		seq.AppendCallback(() => AlterCoinCount(coinIncreaseCount));
	}

	public void CoinsGoingUpEffect()
	{
		coinParticles.PlayControlledParticles(coinParticles.transform.position, coinHolder);
	}
}