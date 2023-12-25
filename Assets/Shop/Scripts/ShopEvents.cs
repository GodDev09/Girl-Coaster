using System;

public static partial class ShopEvents
{
	public static event Action<int, bool> CharacterSkinSelect, WagonSkinSelect;
}

public static partial class ShopEvents
{
	public static void InvokeCharacterSkinSelect(int index, bool shouldDeductCoins) => CharacterSkinSelect?.Invoke(index, shouldDeductCoins);
	public static void InvokeWagonSkinSelect(int index, bool shouldDeductCoins) => WagonSkinSelect?.Invoke(index, shouldDeductCoins);
}