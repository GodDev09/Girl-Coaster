using System;
using StateMachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainShopController : MonoBehaviour//, IWantsAds
{
	public static MainShopController Main;
	
	[SerializeField] private Sprite[] coloredWagonSprites, blackWagonSprites;
	[SerializeField] private Sprite[] coloredCharacterSprites, blackCharacterSprites;
	
	[Header("Coins and costs"), SerializeField] private TextMeshProUGUI coinText;
	public int[] weaponSkinCosts, armsSkinCosts;

	[SerializeField] private GridLayoutGroup weaponsHolder, armsHolder;
	[SerializeField] private GameObject shopItemPrefab;
	
	[Header("Arms and Weapons Panels"), SerializeField] private Button weaponsButton;
	[SerializeField] private Button armsButton;
	[SerializeField] private TextMeshProUGUI weaponsText, armsText;
	[SerializeField] private Color black, grey;
	
	private MoneyCanvas _moneyCanvas;

	private Animator _anim;
	private bool _canClick = true;

	#region Animator Hashes

	private static readonly int Close = Animator.StringToHash("Close");
	private static readonly int Open = Animator.StringToHash("Open");
	private static readonly int ShowShopButton = Animator.StringToHash("showShopButton");
	private static readonly int HideShopButton = Animator.StringToHash("hideShopButton");

	#endregion
		
	#region Helpers and Getters
	public TextMeshProUGUI GetCoinText() => _moneyCanvas.moneyText;
	public void UpdateCoinText() => _moneyCanvas.moneyText.text = ShopStateController.CurrentState.GetState().CoinCount.ToString();
	public static int GetWagonSkinCount() => Enum.GetNames(typeof(WagonType)).Length;
	public static int GetCharacterSkinCount() => Enum.GetNames(typeof(CharacterType)).Length;

	public Sprite GetWeaponSprite(int index, bool wantsBlackSprite = false)
	{
		var currentList = wantsBlackSprite ? blackWagonSprites : coloredWagonSprites;
		
		if (index >= currentList.Length)
			return currentList[^1];

		return currentList[index];
	}

	public Sprite GetArmsSkinSprite(int index, bool wantsBlackSprite = false)
	{
		var currentList = wantsBlackSprite ? blackCharacterSprites : coloredCharacterSprites;
		
		if (index >= currentList.Length)
			return currentList[^1];

		return currentList[index];
	}
	
	private int GetWeaponSkinPrice(int index) => weaponSkinCosts[index];
	private int GetArmsSkinPrice(int index) => armsSkinCosts[index];
	#endregion
	
	private void OnEnable()
	{
		ShopEvents.WagonSkinSelect += OnWagonSkinPurchase;
		ShopEvents.CharacterSkinSelect += OnCharacterSkinPurchase;
		
		GameEvents.TapToPlay += OnTapToPlay;
		GameEvents.GameWin += OnGameEnd;
	}

	private void OnDisable()
	{
		ShopEvents.WagonSkinSelect -= OnWagonSkinPurchase;
		ShopEvents.CharacterSkinSelect -= OnCharacterSkinPurchase;
		
		GameEvents.TapToPlay -= OnTapToPlay;
		GameEvents.GameWin -= OnGameEnd;
	}
	
	private void Awake()
	{
		if (!Main) Main = this;
		else Destroy(gameObject);

		_moneyCanvas = GameObject.FindGameObjectWithTag("MoneyCanvas").GetComponent<MoneyCanvas>();
		ReadCurrentShopState(true);
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		
		_anim.SetTrigger(ShowShopButton);
		ClickWagons();
		
		weaponsHolder.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		armsHolder.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
	}

	/// <summary>
	/// This function not only reads the change of the values, but also sets all the ShopItems to the visual and behavioural representation they should be in.
	/// </summary>
	/// <param name="initialising"> Optional parameter. Only set to true when calling to initialise values/ generate scroll views.</param>
	public void ReadCurrentShopState(bool initialising = false)
	{
		var currentShopState = initialising
			? ShopStateController.ShopStateSerializer.LoadSavedState()
			: ShopStateController.CurrentState.GetState();

		var weaponCount = GetWagonSkinCount();
		var armsCount = GetCharacterSkinCount();
		
		var weaponRect = weaponsHolder.GetComponent<RectTransform>();
		weaponRect.sizeDelta = new Vector2(weaponRect.sizeDelta.x,
			Mathf.CeilToInt(weaponCount / (float) weaponsHolder.constraintCount) * (weaponsHolder.cellSize.y + weaponsHolder.spacing.y));

		var armsRect = armsHolder.GetComponent<RectTransform>();
		armsRect.sizeDelta = new Vector2(armsRect.sizeDelta.x,
			Mathf.CeilToInt(armsCount / (float) armsHolder.constraintCount) * (armsHolder.cellSize.y + 2 * armsHolder.spacing.y));
		
		for (var i = 0; i < weaponCount; i++)
		{
			var item = initialising 
				? Instantiate(shopItemPrefab, weaponsHolder.transform).GetComponent<ShopItem>() 
				: weaponsHolder.transform.GetChild(i).GetComponent<ShopItem>();

			var itemState = currentShopState.wagonStates[(WagonType) i];
			item.SetSkinIndex(i);
			item.SetState(itemState);
			//item.SetIconSprite(GetWeaponSprite(i, itemState == ShopItemState.Locked));
			item.SetIconSprite(GetWeaponSprite(i, false));
			item.SetIsWeaponItem(true);
			
			//if you are having an index out of bounds error over here, check if prices and colored & black sprites have equal no of items
			item.SetPriceAndAvailability(GetWeaponSkinPrice(i));
		}

		for (var i = 0; i < armsCount; i++)
		{
			var item = initialising
				? Instantiate(shopItemPrefab, armsHolder.transform).GetComponent<ShopItem>()
				: armsHolder.transform.GetChild(i).GetComponent<ShopItem>();
			
			var itemState = currentShopState.characterStates[(CharacterType) i];
			item.SetSkinIndex(i);
			item.SetState(itemState);
			//item.SetIconSprite(GetArmsSkinSprite(i, itemState == ShopItemState.Locked));
			item.SetIconSprite(GetArmsSkinSprite(i, false));
			item.SetIsWeaponItem(false);
			
			//if you are having an index out of bounds error over here, check if prices and colored & black sprites have equal no of items
			item.SetPriceAndAvailability(GetArmsSkinPrice(i));
		}
		
		UpdateCoinText();
	}

	private void SaveCurrentShopState()
	{
		//save the newest made change of state
		ShopStateController.ShopStateSerializer.SaveCurrentState();
		
		//make shop items represent their state acc to new change of state 
		ReadCurrentShopState();
	}

	private static void ChangeSelectedWeapon(int index = -1)
	{
		for (var i = 0; i < GetWagonSkinCount(); i++)
		{
			if (i == index) continue;

			if (ShopStateController.CurrentState.GetState().wagonStates[(WagonType) i] == ShopItemState.Selected)
				ShopStateController.CurrentState.GetState().wagonStates[(WagonType) i] = ShopItemState.Unlocked;
		}
	}

	private static void ChangeSelectedArmsSkin(int index = -1)
	{
		for (var i = 0; i < GetCharacterSkinCount(); i++)
		{
			if (i == index) continue;
			
			if (ShopStateController.CurrentState.GetState().characterStates[(CharacterType) i] == ShopItemState.Selected)
				ShopStateController.CurrentState.GetState().characterStates[(CharacterType) i] = ShopItemState.Unlocked;
		}
	}

	public void OpenShop()
	{
		if(!_canClick) return;
		
		_anim.SetTrigger(Open);
		_anim.SetTrigger(HideShopButton);
		InputHandler.AssignNewState(InputState.Disabled);
	}

	public void CloseShop()
	{
		_anim.SetTrigger(Close);
		_anim.SetTrigger(ShowShopButton);
		InputHandler.AssignNewState(InputState.IdleOnTracks);
		UpgradeShopCanvas.only.UpdateButtons();
	}

	public void ClickCharacterSkins() => OpenArmSkinShopBehaviour();

	private void OpenArmSkinShopBehaviour()
	{
		armsButton.interactable = false;
		armsText.color = grey;
		
		weaponsButton.interactable = true;
		weaponsText.color = black;

		//switch to arms panel
		armsHolder.transform.parent.parent.gameObject.SetActive(true);
		weaponsHolder.transform.parent.parent.gameObject.SetActive(false);
	}

	public void ClickWagons() => OpenWagonShopBehaviour();

	private void OpenWagonShopBehaviour()
	{
		weaponsButton.interactable = false;
		weaponsText.color = grey;

		armsButton.interactable = true;
		armsText.color = black;

		//switch to weapons panel
		weaponsHolder.transform.parent.parent.gameObject.SetActive(true);
		armsHolder.transform.parent.parent.gameObject.SetActive(false);
	}

	private void OnWagonSkinPurchase(int index, bool shouldDeductCoins)
	{
		if(shouldDeductCoins)
		{
			//if was locked before this, Decrease coin count
			ShopStateController.CurrentState.GetState().CoinCount -= weaponSkinCosts[index];
			UpdateCoinText();
		} 

		//mark purchased weapon as selected
		ShopStateController.CurrentState.GetState().wagonStates[(WagonType) index] = ShopItemState.Selected;
		
		//make sure nobody else is selected/ old one is now marked as unlocked
		ChangeSelectedWeapon(index);

		//Save the state and reflect it in Shop UI
		SaveCurrentShopState();
	}
	
	private void OnCharacterSkinPurchase(int index, bool shouldDeductCoins)
	{
		if(shouldDeductCoins)
		{
			//if was locked before this, Decrease coin count
			ShopStateController.CurrentState.GetState().CoinCount -= armsSkinCosts[index];
			UpdateCoinText();
		}
		
		//mark purchased skin as selected
		ShopStateController.CurrentState.GetState().characterStates[(CharacterType) index] = ShopItemState.Selected;
		
		//make sure nobody else is selected/ old one is now marked as unlocked
		ChangeSelectedArmsSkin(index);
		
		//Save the state and reflect it in Shop UI
		SaveCurrentShopState();
	}

	private void OnTapToPlay()
	{
		_anim.SetTrigger(HideShopButton);
		_canClick = false;
		coinText.transform.parent.gameObject.SetActive(false);
	}

	private void OnGameEnd() => coinText.transform.parent.gameObject.SetActive(true);
}

public enum WagonType
{
	Standard,
}

public enum CharacterType
{
	Stickman,
}