using System;
using System.Collections.Generic;

[Serializable]
public class ShopState
{
	public Dictionary<WagonType, ShopItemState> wagonStates;
	public Dictionary<CharacterType, ShopItemState> characterStates;
	public int LoaderIndex { get; set; }
	public int SidebarWagon { get; set;}
	public int CoinCount { get; set;}
	public bool AllWagonsUnlocked { get; set; }
	public bool AllCharactersUnlocked { get; set; }
	public int CurrentFeverLevel { get; set; }
	public int CurrentMoneyLevel { get; set; }

	public ShopState(Dictionary<WagonType, ShopItemState> newWagonState, Dictionary<CharacterType, ShopItemState> newCharacterStates, int newCoinCount, int sidebarSkin, int loaderIndex, int feverLevel, int moneyLevel)
	{
		wagonStates = newWagonState;
		characterStates = newCharacterStates;
		CoinCount = newCoinCount;
		SidebarWagon = sidebarSkin;
		LoaderIndex = loaderIndex;
		CurrentFeverLevel = feverLevel;
		CurrentMoneyLevel = moneyLevel;
		AllWagonsUnlocked = false;
	}
}

public class ShopStateHelpers
{
	private readonly ShopState _shopState;

	public ShopStateHelpers(ShopState shopState) => _shopState = shopState;

	public ShopState GetState() => _shopState;
	
	public int GetCurrentWagon() => GetFirstSelected(_shopState.wagonStates);

	public int GetCurrentCharacterSkin() => GetFirstSelected(_shopState.characterStates);

	public void SetNewLoaderIndex(int index) => _shopState.LoaderIndex = index;
	public void SetNewSideBarWagon(int index) => _shopState.SidebarWagon = index;

	public int GetCurrentFeverLevel() => _shopState.CurrentFeverLevel;
	public int GetCurrentMoneyLevel() => _shopState.CurrentMoneyLevel;
	
	public void SetNewFeverLevel(int level) => _shopState.CurrentFeverLevel = level;
	public void SetNewMoneyLevel(int level) => _shopState.CurrentMoneyLevel = level;

	private static int GetFirstSelected<T>(Dictionary<T, ShopItemState> states) where T : Enum
	{
		//starting from -1 to account for case 0
		var index = -1;
		foreach (var state in states)
		{
			index++;
			if (state.Value == ShopItemState.Selected) break;
		}
		return index;
	}

	public bool AreAllWagonsUnlocked()
	{
		if (_shopState.AllWagonsUnlocked) return true;

		foreach (var state in _shopState.wagonStates)
			if (state.Value == ShopItemState.Locked)
				return false;

		AllWagonsHaveBeenUnlocked();
		return true;
	}

	public void AllWagonsHaveBeenUnlocked() => _shopState.AllWagonsUnlocked = true;

	public bool AreAllCharacterSkinsUnlocked()
	{
		if (_shopState.AllCharactersUnlocked) return true;

		foreach (var state in _shopState.wagonStates)
			if (state.Value == ShopItemState.Locked)
				return false;

		AllArmsSkinsHaveBeenUnlocked();
		return true;
	}

	public void AllArmsSkinsHaveBeenUnlocked() => _shopState.AllWagonsUnlocked = true;
}