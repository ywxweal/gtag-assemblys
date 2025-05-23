using System;
using System.Collections.Generic;
using System.Linq;
using GorillaNetworking;
using GorillaNetworking.Store;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000448 RID: 1096
public class TryOnBundlesStand : MonoBehaviour
{
	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x06001AFE RID: 6910 RVA: 0x000847E0 File Offset: 0x000829E0
	private string SelectedBundlePlayFabID
	{
		get
		{
			return this.TryOnBundleButtons[this.SelectedButtonIndex].playfabBundleID;
		}
	}

	// Token: 0x06001AFF RID: 6911 RVA: 0x000847F4 File Offset: 0x000829F4
	public static string CleanUpTitleDataValues(string titleDataResult)
	{
		string text = titleDataResult.Replace("\\r", "\r").Replace("\\n", "\n");
		if (text[0] == '"' && text[text.Length - 1] == '"')
		{
			text = text.Substring(1, text.Length - 2);
		}
		return text;
	}

	// Token: 0x06001B00 RID: 6912 RVA: 0x00084850 File Offset: 0x00082A50
	private void InitalizeButtons()
	{
		this.GetTryOnButtons();
		for (int i = 0; i < this.TryOnBundleButtons.Length; i++)
		{
			if (!CosmeticsController.instance.GetItemFromDict(this.TryOnBundleButtons[i].playfabBundleID).isNullItem)
			{
				this.TryOnBundleButtons[i].UpdateColor();
			}
		}
	}

	// Token: 0x06001B01 RID: 6913 RVA: 0x000848A4 File Offset: 0x00082AA4
	private void Start()
	{
		PlayFabTitleDataCache.Instance.GetTitleData(this.ComputerDefaultTextTitleDataKey, new Action<string>(this.OnComputerDefaultTextTitleDataSuccess), new Action<PlayFabError>(this.OnComputerDefaultTextTitleDataFailure));
		PlayFabTitleDataCache.Instance.GetTitleData(this.ComputerAlreadyOwnTextTitleDataKey, new Action<string>(this.OnComputerAlreadyOwnTextTitleDataSuccess), new Action<PlayFabError>(this.OnComputerAlreadyOwnTextTitleDataFailure));
		PlayFabTitleDataCache.Instance.GetTitleData(this.PurchaseButtonDefaultTextTitleDataKey, new Action<string>(this.OnPurchaseButtonDefaultTextTitleDataSuccess), new Action<PlayFabError>(this.OnPurchaseButtonDefaultTextTitleDataFailure));
		PlayFabTitleDataCache.Instance.GetTitleData(this.PurchaseButtonAlreadyOwnTextTitleDataKey, new Action<string>(this.OnPurchaseButtonAlreadyOwnTextTitleDataSuccess), new Action<PlayFabError>(this.OnPurchaseButtonAlreadyOwnTextTitleDataFailure));
		this.InitalizeButtons();
	}

	// Token: 0x06001B02 RID: 6914 RVA: 0x00084957 File Offset: 0x00082B57
	private void OnComputerDefaultTextTitleDataSuccess(string data)
	{
		this.ComputerDefaultTextTitleDataValue = TryOnBundlesStand.CleanUpTitleDataValues(data);
		this.computerScreenText.text = this.ComputerDefaultTextTitleDataValue;
	}

	// Token: 0x06001B03 RID: 6915 RVA: 0x00084976 File Offset: 0x00082B76
	private void OnComputerDefaultTextTitleDataFailure(PlayFabError error)
	{
		this.ComputerDefaultTextTitleDataValue = "Failed to get TD Key : " + this.ComputerDefaultTextTitleDataKey;
		this.computerScreenText.text = this.ComputerDefaultTextTitleDataValue;
		Debug.LogError(string.Format("Error getting Computer Screen Title Data: {0}", error));
	}

	// Token: 0x06001B04 RID: 6916 RVA: 0x000849AF File Offset: 0x00082BAF
	private void OnComputerAlreadyOwnTextTitleDataSuccess(string data)
	{
		this.ComputerAlreadyOwnTextTitleDataValue = TryOnBundlesStand.CleanUpTitleDataValues(data);
	}

	// Token: 0x06001B05 RID: 6917 RVA: 0x000849BD File Offset: 0x00082BBD
	private void OnComputerAlreadyOwnTextTitleDataFailure(PlayFabError error)
	{
		this.ComputerAlreadyOwnTextTitleDataValue = "Failed to get TD Key : " + this.ComputerAlreadyOwnTextTitleDataKey;
		Debug.LogError(string.Format("Error getting Computer Already Screen Title Data: {0}", error));
	}

	// Token: 0x06001B06 RID: 6918 RVA: 0x000849E5 File Offset: 0x00082BE5
	private void OnPurchaseButtonDefaultTextTitleDataSuccess(string data)
	{
		this.PurchaseButtonDefaultTextTitleDataValue = TryOnBundlesStand.CleanUpTitleDataValues(data);
		this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
		this.purchaseButton.UpdateColor();
	}

	// Token: 0x06001B07 RID: 6919 RVA: 0x00084A10 File Offset: 0x00082C10
	private void OnPurchaseButtonDefaultTextTitleDataFailure(PlayFabError error)
	{
		this.PurchaseButtonDefaultTextTitleDataValue = "Failed to get TD Key : " + this.PurchaseButtonDefaultTextTitleDataKey;
		this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
		this.purchaseButton.UpdateColor();
		Debug.LogError(string.Format("Error getting Tryon Purchase Button Default Text Title Data: {0}", error));
	}

	// Token: 0x06001B08 RID: 6920 RVA: 0x00084A5F File Offset: 0x00082C5F
	private void OnPurchaseButtonAlreadyOwnTextTitleDataSuccess(string data)
	{
		this.PurchaseButtonAlreadyOwnTextTitleDataValue = TryOnBundlesStand.CleanUpTitleDataValues(data);
		this.purchaseButton.AlreadyOwnText = this.PurchaseButtonAlreadyOwnTextTitleDataValue;
	}

	// Token: 0x06001B09 RID: 6921 RVA: 0x00084A7E File Offset: 0x00082C7E
	private void OnPurchaseButtonAlreadyOwnTextTitleDataFailure(PlayFabError error)
	{
		this.PurchaseButtonAlreadyOwnTextTitleDataValue = "Failed to get TD Key : " + this.PurchaseButtonAlreadyOwnTextTitleDataKey;
		this.purchaseButton.AlreadyOwnText = this.PurchaseButtonAlreadyOwnTextTitleDataValue;
		Debug.LogError(string.Format("Error getting Tryon Purchase Button Already Own Text Title Data: {0}", error));
	}

	// Token: 0x06001B0A RID: 6922 RVA: 0x00084AB8 File Offset: 0x00082CB8
	public void ClearSelectedBundle()
	{
		if (this.SelectedButtonIndex != -1)
		{
			this.TryOnBundleButtons[this.SelectedButtonIndex].isOn = false;
			if (this.TryOnBundleButtons[this.SelectedButtonIndex].playfabBundleID != "NULL" || this.TryOnBundleButtons[this.SelectedButtonIndex].playfabBundleID != "")
			{
				this.RemoveBundle(this.SelectedBundlePlayFabID);
				this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
				this.purchaseButton.ResetButton();
				this.selectedBundleImage.sprite = null;
				this.TryOnBundleButtons[this.SelectedButtonIndex].UpdateColor();
				this.SelectedButtonIndex = -1;
			}
		}
		this.computerScreenText.text = (this.bError ? this.computerScreeErrorText : this.ComputerDefaultTextTitleDataValue);
	}

	// Token: 0x06001B0B RID: 6923 RVA: 0x00084B90 File Offset: 0x00082D90
	private void RemoveBundle(string BundleID)
	{
		CosmeticsController.CosmeticItem itemFromDict = CosmeticsController.instance.GetItemFromDict(BundleID);
		if (itemFromDict.isNullItem)
		{
			return;
		}
		foreach (string text in itemFromDict.bundledItems)
		{
			CosmeticsController.instance.RemoveCosmeticItemFromSet(CosmeticsController.instance.tryOnSet, text, false);
		}
	}

	// Token: 0x06001B0C RID: 6924 RVA: 0x00084BE8 File Offset: 0x00082DE8
	private void TryOnBundle(string BundleID)
	{
		CosmeticsController.CosmeticItem itemFromDict = CosmeticsController.instance.GetItemFromDict(BundleID);
		if (itemFromDict.isNullItem)
		{
			return;
		}
		foreach (CosmeticsController.CosmeticItem cosmeticItem in CosmeticsController.instance.tryOnSet.items)
		{
			if (!itemFromDict.bundledItems.Contains(cosmeticItem.itemName))
			{
				CosmeticsController.instance.RemoveCosmeticItemFromSet(CosmeticsController.instance.tryOnSet, cosmeticItem.itemName, false);
			}
		}
		foreach (string text in itemFromDict.bundledItems)
		{
			if (!CosmeticsController.instance.tryOnSet.HasItem(text))
			{
				CosmeticsController.instance.ApplyCosmeticItemToSet(CosmeticsController.instance.tryOnSet, CosmeticsController.instance.GetItemFromDict(text), false, false);
			}
		}
	}

	// Token: 0x06001B0D RID: 6925 RVA: 0x00084CC0 File Offset: 0x00082EC0
	public void PressTryOnBundleButton(TryOnBundleButton pressedTryOnBundleButton, bool isLeftHand)
	{
		if (pressedTryOnBundleButton.playfabBundleID == "NULL")
		{
			Debug.LogError("TryOnBundlesStand - PressTryOnBundleButton - Invalid bundle ID");
			return;
		}
		if (CosmeticsController.instance.GetItemFromDict(pressedTryOnBundleButton.playfabBundleID).isNullItem)
		{
			Debug.LogError("TryOnBundlesStand - PressTryOnBundleButton - Bundle is Null + " + pressedTryOnBundleButton.playfabBundleID);
			return;
		}
		if (this.SelectedButtonIndex != pressedTryOnBundleButton.buttonIndex)
		{
			this.ClearSelectedBundle();
		}
		switch (CosmeticsController.instance.CheckIfCosmeticSetMatchesItemSet(CosmeticsController.instance.tryOnSet, pressedTryOnBundleButton.playfabBundleID))
		{
		case CosmeticsController.EWearingCosmeticSet.NotASet:
			Debug.LogError("TryOnBundlesStand - PressTryOnBundleButton - Item is Not A Set");
			break;
		case CosmeticsController.EWearingCosmeticSet.NotWearing:
			this.TryOnBundle(pressedTryOnBundleButton.playfabBundleID);
			this.SelectedButtonIndex = pressedTryOnBundleButton.buttonIndex;
			break;
		case CosmeticsController.EWearingCosmeticSet.Partial:
			if (pressedTryOnBundleButton.isOn)
			{
				this.ClearSelectedBundle();
			}
			else
			{
				this.TryOnBundle(pressedTryOnBundleButton.playfabBundleID);
				this.SelectedButtonIndex = pressedTryOnBundleButton.buttonIndex;
			}
			break;
		case CosmeticsController.EWearingCosmeticSet.Complete:
			this.ClearSelectedBundle();
			break;
		}
		if (this.SelectedButtonIndex != -1)
		{
			if (!this.bError)
			{
				this.selectedBundleImage.sprite = BundleManager.instance.storeBundlesById[pressedTryOnBundleButton.playfabBundleID].bundleImage;
				pressedTryOnBundleButton.isOn = true;
				this.purchaseButton.offText = this.GetPurchaseButtonText(pressedTryOnBundleButton.playfabBundleID);
				this.computerScreenText.text = this.GetComputerScreenText(pressedTryOnBundleButton.playfabBundleID);
				this.AlreadyOwnCheck();
			}
			pressedTryOnBundleButton.UpdateColor();
		}
		else
		{
			if (!this.bError)
			{
				this.computerScreenText.text = this.ComputerDefaultTextTitleDataValue;
				this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
			}
			pressedTryOnBundleButton.isOn = false;
			this.selectedBundleImage.sprite = null;
			this.purchaseButton.offText = this.PurchaseButtonDefaultTextTitleDataValue;
			this.purchaseButton.ResetButton();
			this.purchaseButton.UpdateColor();
		}
		CosmeticsController.instance.UpdateShoppingCart();
		CosmeticsController.instance.UpdateWornCosmetics(true);
		pressedTryOnBundleButton.UpdateColor();
	}

	// Token: 0x06001B0E RID: 6926 RVA: 0x00084EB8 File Offset: 0x000830B8
	private string GetComputerScreenText(string playfabBundleID)
	{
		return BundleManager.instance.storeBundlesById[playfabBundleID].bundleDescriptionText;
	}

	// Token: 0x06001B0F RID: 6927 RVA: 0x00084ED1 File Offset: 0x000830D1
	private string GetPurchaseButtonText(string playfabBundleID)
	{
		return BundleManager.instance.storeBundlesById[playfabBundleID].purchaseButtonText;
	}

	// Token: 0x06001B10 RID: 6928 RVA: 0x00084EEA File Offset: 0x000830EA
	public void PurchaseButtonPressed()
	{
		if (this.SelectedButtonIndex == -1)
		{
			return;
		}
		CosmeticsController.instance.PurchaseBundle(BundleManager.instance.storeBundlesById[this.SelectedBundlePlayFabID]);
	}

	// Token: 0x06001B11 RID: 6929 RVA: 0x00084F1C File Offset: 0x0008311C
	public void AlreadyOwnCheck()
	{
		if (this.SelectedButtonIndex == -1)
		{
			return;
		}
		if (BundleManager.instance.storeBundlesById[this.SelectedBundlePlayFabID].isOwned)
		{
			this.purchaseButton.AlreadyOwn();
			if (!this.bError)
			{
				this.computerScreenText.text = this.ComputerAlreadyOwnTextTitleDataValue;
				return;
			}
		}
		else
		{
			if (!this.bError)
			{
				this.computerScreenText.text = this.GetBundleComputerText(this.SelectedBundlePlayFabID);
			}
			this.purchaseButton.UpdateColor();
		}
	}

	// Token: 0x06001B12 RID: 6930 RVA: 0x00084FA0 File Offset: 0x000831A0
	public void GetTryOnButtons()
	{
		StoreBundleData[] tryOnButtons = BundleManager.instance.GetTryOnButtons();
		for (int i = 0; i < this.TryOnBundleButtons.Length; i++)
		{
			if (i < tryOnButtons.Length)
			{
				if (tryOnButtons[i] != null && tryOnButtons[i].playfabBundleID != "NULL" && tryOnButtons[i].bundleImage != null)
				{
					this.TryOnBundleButtons[i].playfabBundleID = tryOnButtons[i].playfabBundleID;
					this.BundleIcons[i].sprite = tryOnButtons[i].bundleImage;
				}
				else
				{
					this.TryOnBundleButtons[i].playfabBundleID = "NULL";
					this.BundleIcons[i].sprite = null;
				}
			}
			else
			{
				this.TryOnBundleButtons[i].playfabBundleID = "NULL";
				this.BundleIcons[i].sprite = null;
			}
			this.TryOnBundleButtons[i].UpdateColor();
		}
	}

	// Token: 0x06001B13 RID: 6931 RVA: 0x00085083 File Offset: 0x00083283
	public void UpdateBundles(StoreBundleData[] Bundles)
	{
		Debug.LogWarning("TryOnBundlesStand - UpdateBundles is an editor only function!");
	}

	// Token: 0x06001B14 RID: 6932 RVA: 0x00085090 File Offset: 0x00083290
	private string GetBundleComputerText(string PlayFabID)
	{
		StoreBundle storeBundle;
		if (BundleManager.instance.storeBundlesById.TryGetValue(PlayFabID, out storeBundle))
		{
			return storeBundle.bundleDescriptionText;
		}
		return "ERROR THIS DOES NOT EXIST YET";
	}

	// Token: 0x06001B15 RID: 6933 RVA: 0x000850BF File Offset: 0x000832BF
	public void ErrorCompleting()
	{
		this.bError = true;
		this.purchaseButton.ErrorHappened();
		this.computerScreenText.text = this.computerScreeErrorText;
	}

	// Token: 0x04001E00 RID: 7680
	[SerializeField]
	private TryOnBundleButton[] TryOnBundleButtons;

	// Token: 0x04001E01 RID: 7681
	[SerializeField]
	private Image[] BundleIcons;

	// Token: 0x04001E02 RID: 7682
	[Header("The Index of the Selected Bundle from CosmeticsBundle Array in CosmeticsController")]
	private int SelectedButtonIndex = -1;

	// Token: 0x04001E03 RID: 7683
	public TryOnPurchaseButton purchaseButton;

	// Token: 0x04001E04 RID: 7684
	public Image selectedBundleImage;

	// Token: 0x04001E05 RID: 7685
	public Text computerScreenText;

	// Token: 0x04001E06 RID: 7686
	public string ComputerDefaultTextTitleDataKey;

	// Token: 0x04001E07 RID: 7687
	[SerializeField]
	private string ComputerDefaultTextTitleDataValue = "";

	// Token: 0x04001E08 RID: 7688
	public string ComputerAlreadyOwnTextTitleDataKey;

	// Token: 0x04001E09 RID: 7689
	[SerializeField]
	private string ComputerAlreadyOwnTextTitleDataValue = "";

	// Token: 0x04001E0A RID: 7690
	public string PurchaseButtonDefaultTextTitleDataKey;

	// Token: 0x04001E0B RID: 7691
	[SerializeField]
	private string PurchaseButtonDefaultTextTitleDataValue = "";

	// Token: 0x04001E0C RID: 7692
	public string PurchaseButtonAlreadyOwnTextTitleDataKey;

	// Token: 0x04001E0D RID: 7693
	[SerializeField]
	private string PurchaseButtonAlreadyOwnTextTitleDataValue = "";

	// Token: 0x04001E0E RID: 7694
	private bool bError;

	// Token: 0x04001E0F RID: 7695
	[Header("Error Text for Computer Screen")]
	public string computerScreeErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME, AND MAKE SURE YOU HAVE A STABLE INTERNET CONNECTION. ";

	// Token: 0x04001E10 RID: 7696
	private List<StoreBundle> storeBundles = new List<StoreBundle>();
}
