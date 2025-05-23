using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GorillaNetworking;
using GorillaNetworking.Store;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000441 RID: 1089
public class ATM_Manager : MonoBehaviour
{
	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x06001AD9 RID: 6873 RVA: 0x000830DD File Offset: 0x000812DD
	// (set) Token: 0x06001ADA RID: 6874 RVA: 0x000830E5 File Offset: 0x000812E5
	public string ValidatedCreatorCode { get; set; }

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06001ADB RID: 6875 RVA: 0x000830EE File Offset: 0x000812EE
	public ATM_Manager.ATMStages CurrentATMStage
	{
		get
		{
			return this.currentATMStage;
		}
	}

	// Token: 0x06001ADC RID: 6876 RVA: 0x000830F8 File Offset: 0x000812F8
	public void Awake()
	{
		if (ATM_Manager.instance)
		{
			Object.Destroy(this);
		}
		else
		{
			ATM_Manager.instance = this;
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: ";
		}
		this.SwitchToStage(ATM_Manager.ATMStages.Unavailable);
		this.smallDisplays = new List<CreatorCodeSmallDisplay>();
	}

	// Token: 0x06001ADD RID: 6877 RVA: 0x00083184 File Offset: 0x00081384
	public void Start()
	{
		Debug.Log("ATM COUNT: " + this.atmUIs.Count.ToString());
		Debug.Log("SMALL DISPLAY COUNT: " + this.smallDisplays.Count.ToString());
		GameEvents.OnGorrillaATMKeyButtonPressedEvent.AddListener(new UnityAction<GorillaATMKeyBindings>(this.PressButton));
		this.currentCreatorCode = "";
		if (PlayerPrefs.HasKey("CodeUsedTime"))
		{
			this.codeFirstUsedTime = PlayerPrefs.GetString("CodeUsedTime");
			DateTime dateTime = DateTime.Parse(this.codeFirstUsedTime);
			if ((DateTime.Now - dateTime).TotalDays > 14.0)
			{
				PlayerPrefs.SetString("CreatorCode", "");
			}
			else
			{
				this.currentCreatorCode = PlayerPrefs.GetString("CreatorCode", "");
				this.initialCode = this.currentCreatorCode;
				Debug.Log("Initial code: " + this.initialCode);
				if (string.IsNullOrEmpty(this.currentCreatorCode))
				{
					this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Empty;
				}
				else
				{
					this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Unchecked;
				}
				foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
				{
					creatorCodeSmallDisplay.SetCode(this.currentCreatorCode);
				}
			}
		}
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeField.text = this.currentCreatorCode;
		}
	}

	// Token: 0x06001ADE RID: 6878 RVA: 0x0008333C File Offset: 0x0008153C
	public void PressButton(GorillaATMKeyBindings buttonPressed)
	{
		if (this.currentATMStage == ATM_Manager.ATMStages.Confirm && this.creatorCodeStatus != ATM_Manager.CreatorCodeStatus.Validating)
		{
			foreach (ATM_UI atm_UI in this.atmUIs)
			{
				atm_UI.creatorCodeTitle.text = "CREATOR CODE:";
			}
			if (buttonPressed == GorillaATMKeyBindings.delete)
			{
				if (this.currentCreatorCode.Length > 0)
				{
					this.currentCreatorCode = this.currentCreatorCode.Substring(0, this.currentCreatorCode.Length - 1);
					if (this.currentCreatorCode.Length == 0)
					{
						this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Empty;
						this.ValidatedCreatorCode = "";
						foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
						{
							creatorCodeSmallDisplay.SetCode("");
						}
						PlayerPrefs.SetString("CreatorCode", "");
						PlayerPrefs.Save();
					}
					else
					{
						this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Unchecked;
					}
				}
			}
			else if (this.currentCreatorCode.Length < 10)
			{
				string text = this.currentCreatorCode;
				string text2;
				if (buttonPressed >= GorillaATMKeyBindings.delete)
				{
					text2 = buttonPressed.ToString();
				}
				else
				{
					int num = (int)buttonPressed;
					text2 = num.ToString();
				}
				this.currentCreatorCode = text + text2;
				this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Unchecked;
			}
			foreach (ATM_UI atm_UI2 in this.atmUIs)
			{
				atm_UI2.creatorCodeField.text = this.currentCreatorCode;
			}
		}
	}

	// Token: 0x06001ADF RID: 6879 RVA: 0x000834F8 File Offset: 0x000816F8
	public void ProcessATMState(string currencyButton)
	{
		switch (this.currentATMStage)
		{
		case ATM_Manager.ATMStages.Unavailable:
		case ATM_Manager.ATMStages.Purchasing:
			break;
		case ATM_Manager.ATMStages.Begin:
			this.SwitchToStage(ATM_Manager.ATMStages.Menu);
			return;
		case ATM_Manager.ATMStages.Menu:
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				if (currencyButton == "one")
				{
					this.SwitchToStage(ATM_Manager.ATMStages.Balance);
					return;
				}
				if (!(currencyButton == "four"))
				{
					return;
				}
				this.SwitchToStage(ATM_Manager.ATMStages.Begin);
				return;
			}
			else
			{
				if (currencyButton == "one")
				{
					this.SwitchToStage(ATM_Manager.ATMStages.Balance);
					return;
				}
				if (currencyButton == "two")
				{
					this.SwitchToStage(ATM_Manager.ATMStages.Choose);
					return;
				}
				if (!(currencyButton == "back"))
				{
					return;
				}
				this.SwitchToStage(ATM_Manager.ATMStages.Begin);
				return;
			}
			break;
		case ATM_Manager.ATMStages.Balance:
			if (currencyButton == "back")
			{
				this.SwitchToStage(ATM_Manager.ATMStages.Menu);
				return;
			}
			break;
		case ATM_Manager.ATMStages.Choose:
			if (currencyButton == "one")
			{
				this.numShinyRocksToBuy = 1000;
				this.shinyRocksCost = 4.99f;
				CosmeticsController.instance.itemToPurchase = "1000SHINYROCKS";
				CosmeticsController.instance.buyingBundle = false;
				this.SwitchToStage(ATM_Manager.ATMStages.Confirm);
				return;
			}
			if (currencyButton == "two")
			{
				this.numShinyRocksToBuy = 2200;
				this.shinyRocksCost = 9.99f;
				CosmeticsController.instance.itemToPurchase = "2200SHINYROCKS";
				CosmeticsController.instance.buyingBundle = false;
				this.SwitchToStage(ATM_Manager.ATMStages.Confirm);
				return;
			}
			if (currencyButton == "three")
			{
				this.numShinyRocksToBuy = 5000;
				this.shinyRocksCost = 19.99f;
				CosmeticsController.instance.itemToPurchase = "5000SHINYROCKS";
				CosmeticsController.instance.buyingBundle = false;
				this.SwitchToStage(ATM_Manager.ATMStages.Confirm);
				return;
			}
			if (currencyButton == "four")
			{
				this.numShinyRocksToBuy = 11000;
				this.shinyRocksCost = 39.99f;
				CosmeticsController.instance.itemToPurchase = "11000SHINYROCKS";
				CosmeticsController.instance.buyingBundle = false;
				this.SwitchToStage(ATM_Manager.ATMStages.Confirm);
				return;
			}
			if (!(currencyButton == "back"))
			{
				return;
			}
			this.SwitchToStage(ATM_Manager.ATMStages.Menu);
			return;
		case ATM_Manager.ATMStages.Confirm:
			if (!(currencyButton == "one"))
			{
				if (!(currencyButton == "back"))
				{
					return;
				}
				this.SwitchToStage(ATM_Manager.ATMStages.Choose);
				return;
			}
			else
			{
				if (this.creatorCodeStatus == ATM_Manager.CreatorCodeStatus.Empty)
				{
					CosmeticsController.instance.SteamPurchase();
					this.SwitchToStage(ATM_Manager.ATMStages.Purchasing);
					return;
				}
				base.StartCoroutine(this.CheckValidationCoroutine());
				return;
			}
			break;
		default:
			this.SwitchToStage(ATM_Manager.ATMStages.Menu);
			break;
		}
	}

	// Token: 0x06001AE0 RID: 6880 RVA: 0x00083763 File Offset: 0x00081963
	public void AddATM(ATM_UI newATM)
	{
		this.atmUIs.Add(newATM);
		this.SwitchToStage(this.currentATMStage);
	}

	// Token: 0x06001AE1 RID: 6881 RVA: 0x0008377D File Offset: 0x0008197D
	public void RemoveATM(ATM_UI atmToRemove)
	{
		this.atmUIs.Remove(atmToRemove);
	}

	// Token: 0x06001AE2 RID: 6882 RVA: 0x0008378C File Offset: 0x0008198C
	public void SetTemporaryCreatorCode(string creatorCode, bool onlyIfEmpty = true, Action<bool> OnComplete = null)
	{
		if (onlyIfEmpty && (this.creatorCodeStatus != ATM_Manager.CreatorCodeStatus.Empty || !this.currentCreatorCode.IsNullOrEmpty()))
		{
			Action<bool> onComplete = OnComplete;
			if (onComplete == null)
			{
				return;
			}
			onComplete(false);
			return;
		}
		else
		{
			string text = "^[a-zA-Z0-9]+$";
			if (creatorCode.Length <= 10 && Regex.IsMatch(creatorCode, text))
			{
				NexusManager.instance.VerifyCreatorCode(creatorCode, delegate(Member member)
				{
					if (this.currentATMStage > ATM_Manager.ATMStages.Confirm)
					{
						Action<bool> onComplete3 = OnComplete;
						if (onComplete3 == null)
						{
							return;
						}
						onComplete3(false);
						return;
					}
					else if (onlyIfEmpty && (this.creatorCodeStatus != ATM_Manager.CreatorCodeStatus.Empty || !this.currentCreatorCode.IsNullOrEmpty()))
					{
						Action<bool> onComplete4 = OnComplete;
						if (onComplete4 == null)
						{
							return;
						}
						onComplete4(false);
						return;
					}
					else
					{
						this.temporaryOverrideCode = creatorCode;
						this.currentCreatorCode = creatorCode;
						this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Unchecked;
						foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
						{
							creatorCodeSmallDisplay.SetCode(this.currentCreatorCode);
						}
						foreach (ATM_UI atm_UI in this.atmUIs)
						{
							atm_UI.creatorCodeField.text = this.currentCreatorCode;
						}
						Action<bool> onComplete5 = OnComplete;
						if (onComplete5 == null)
						{
							return;
						}
						onComplete5(true);
						return;
					}
				}, delegate
				{
					Action<bool> onComplete6 = OnComplete;
					if (onComplete6 == null)
					{
						return;
					}
					onComplete6(false);
				});
				return;
			}
			Action<bool> onComplete2 = OnComplete;
			if (onComplete2 == null)
			{
				return;
			}
			onComplete2(false);
			return;
		}
	}

	// Token: 0x06001AE3 RID: 6883 RVA: 0x0008384C File Offset: 0x00081A4C
	public void ResetTemporaryCreatorCode()
	{
		if (this.creatorCodeStatus == ATM_Manager.CreatorCodeStatus.Unchecked && this.currentCreatorCode.Equals(this.temporaryOverrideCode))
		{
			this.currentCreatorCode = "";
			this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Empty;
			foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
			{
				creatorCodeSmallDisplay.SetCode("");
			}
			foreach (ATM_UI atm_UI in this.atmUIs)
			{
				atm_UI.creatorCodeField.text = this.currentCreatorCode;
			}
		}
		this.temporaryOverrideCode = "";
	}

	// Token: 0x06001AE4 RID: 6884 RVA: 0x0008392C File Offset: 0x00081B2C
	private void ResetCreatorCode()
	{
		Debug.Log("Resetting creator code");
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE:";
		}
		foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
		{
			creatorCodeSmallDisplay.SetCode("");
		}
		this.currentCreatorCode = "";
		this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Empty;
		this.supportedMember = default(Member);
		this.ValidatedCreatorCode = "";
		PlayerPrefs.SetString("CreatorCode", "");
		PlayerPrefs.Save();
		foreach (ATM_UI atm_UI2 in this.atmUIs)
		{
			atm_UI2.creatorCodeField.text = this.currentCreatorCode;
		}
	}

	// Token: 0x06001AE5 RID: 6885 RVA: 0x00083A5C File Offset: 0x00081C5C
	private IEnumerator CheckValidationCoroutine()
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: VALIDATING";
		}
		this.VerifyCreatorCode();
		while (this.creatorCodeStatus == ATM_Manager.CreatorCodeStatus.Validating)
		{
			yield return new WaitForSeconds(0.5f);
		}
		if (this.creatorCodeStatus == ATM_Manager.CreatorCodeStatus.Valid)
		{
			foreach (ATM_UI atm_UI2 in this.atmUIs)
			{
				atm_UI2.creatorCodeTitle.text = "CREATOR CODE: VALID";
			}
			this.SwitchToStage(ATM_Manager.ATMStages.Purchasing);
			CosmeticsController.instance.SteamPurchase();
		}
		yield break;
	}

	// Token: 0x06001AE6 RID: 6886 RVA: 0x00083A6C File Offset: 0x00081C6C
	public void SwitchToStage(ATM_Manager.ATMStages newStage)
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			if (!atm_UI.atmText)
			{
				break;
			}
			this.currentATMStage = newStage;
			switch (newStage)
			{
			case ATM_Manager.ATMStages.Unavailable:
				atm_UI.atmText.text = "ATM NOT AVAILABLE! PLEASE TRY AGAIN LATER!";
				atm_UI.ATM_RightColumnButtonText[0].text = "";
				atm_UI.ATM_RightColumnArrowText[0].enabled = false;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "";
				atm_UI.ATM_RightColumnArrowText[3].enabled = false;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.Begin:
				atm_UI.atmText.text = "WELCOME! PRESS ANY BUTTON TO BEGIN.";
				atm_UI.ATM_RightColumnButtonText[0].text = "";
				atm_UI.ATM_RightColumnArrowText[0].enabled = false;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "BEGIN";
				atm_UI.ATM_RightColumnArrowText[3].enabled = true;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.Menu:
				if (PlayFabAuthenticator.instance.GetSafety())
				{
					atm_UI.atmText.text = "CHECK YOUR BALANCE.";
					atm_UI.ATM_RightColumnButtonText[0].text = "BALANCE";
					atm_UI.ATM_RightColumnArrowText[0].enabled = true;
					atm_UI.ATM_RightColumnButtonText[1].text = "";
					atm_UI.ATM_RightColumnArrowText[1].enabled = false;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
				}
				else
				{
					atm_UI.atmText.text = "CHECK YOUR BALANCE OR PURCHASE MORE SHINY ROCKS.";
					atm_UI.ATM_RightColumnButtonText[0].text = "BALANCE";
					atm_UI.ATM_RightColumnArrowText[0].enabled = true;
					atm_UI.ATM_RightColumnButtonText[1].text = "PURCHASE";
					atm_UI.ATM_RightColumnArrowText[1].enabled = true;
					atm_UI.ATM_RightColumnButtonText[2].text = "";
					atm_UI.ATM_RightColumnArrowText[2].enabled = false;
					atm_UI.ATM_RightColumnButtonText[3].text = "";
					atm_UI.ATM_RightColumnArrowText[3].enabled = false;
					atm_UI.creatorCodeObject.SetActive(false);
				}
				break;
			case ATM_Manager.ATMStages.Balance:
				atm_UI.atmText.text = "CURRENT BALANCE:\n\n" + CosmeticsController.instance.CurrencyBalance.ToString();
				atm_UI.ATM_RightColumnButtonText[0].text = "";
				atm_UI.ATM_RightColumnArrowText[0].enabled = false;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "";
				atm_UI.ATM_RightColumnArrowText[3].enabled = false;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.Choose:
				atm_UI.atmText.text = "CHOOSE AN AMOUNT OF SHINY ROCKS TO PURCHASE.";
				atm_UI.ATM_RightColumnButtonText[0].text = "1000 for $4.99";
				atm_UI.ATM_RightColumnArrowText[0].enabled = true;
				atm_UI.ATM_RightColumnButtonText[1].text = "2200 for $9.99\n(10% BONUS!)";
				atm_UI.ATM_RightColumnArrowText[1].enabled = true;
				atm_UI.ATM_RightColumnButtonText[2].text = "5000 for $19.99\n(25% BONUS!)";
				atm_UI.ATM_RightColumnArrowText[2].enabled = true;
				atm_UI.ATM_RightColumnButtonText[3].text = "11000 for $39.99\n(37% BONUS!)";
				atm_UI.ATM_RightColumnArrowText[3].enabled = true;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.Confirm:
				atm_UI.atmText.text = string.Concat(new string[]
				{
					"YOU HAVE CHOSEN TO PURCHASE ",
					this.numShinyRocksToBuy.ToString(),
					" SHINY ROCKS FOR $",
					this.shinyRocksCost.ToString(),
					". CONFIRM TO LAUNCH A STEAM WINDOW TO COMPLETE YOUR PURCHASE."
				});
				atm_UI.ATM_RightColumnButtonText[0].text = "CONFIRM";
				atm_UI.ATM_RightColumnArrowText[0].enabled = true;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "";
				atm_UI.ATM_RightColumnArrowText[3].enabled = false;
				atm_UI.creatorCodeObject.SetActive(true);
				break;
			case ATM_Manager.ATMStages.Purchasing:
				atm_UI.atmText.text = "PURCHASING IN STEAM...";
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.Success:
				atm_UI.atmText.text = "SUCCESS! NEW SHINY ROCKS BALANCE: " + (CosmeticsController.instance.CurrencyBalance + this.numShinyRocksToBuy).ToString();
				if (this.creatorCodeStatus == ATM_Manager.CreatorCodeStatus.Valid)
				{
					string name = this.supportedMember.name;
					if (!string.IsNullOrEmpty(name))
					{
						Text atmText = atm_UI.atmText;
						atmText.text = atmText.text + "\n\nTHIS PURCHASE SUPPORTED\n" + name + "!";
						foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
						{
							creatorCodeSmallDisplay.SuccessfulPurchase(name);
						}
					}
				}
				atm_UI.ATM_RightColumnButtonText[0].text = "";
				atm_UI.ATM_RightColumnArrowText[0].enabled = false;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "";
				atm_UI.ATM_RightColumnArrowText[3].enabled = false;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.Failure:
				atm_UI.atmText.text = "PURCHASE CANCELLED. NO FUNDS WERE SPENT.";
				atm_UI.ATM_RightColumnButtonText[0].text = "";
				atm_UI.ATM_RightColumnArrowText[0].enabled = false;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "";
				atm_UI.ATM_RightColumnArrowText[3].enabled = false;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			case ATM_Manager.ATMStages.SafeAccount:
				atm_UI.atmText.text = "Out Of Order.";
				atm_UI.ATM_RightColumnButtonText[0].text = "";
				atm_UI.ATM_RightColumnArrowText[0].enabled = false;
				atm_UI.ATM_RightColumnButtonText[1].text = "";
				atm_UI.ATM_RightColumnArrowText[1].enabled = false;
				atm_UI.ATM_RightColumnButtonText[2].text = "";
				atm_UI.ATM_RightColumnArrowText[2].enabled = false;
				atm_UI.ATM_RightColumnButtonText[3].text = "";
				atm_UI.ATM_RightColumnArrowText[3].enabled = false;
				atm_UI.creatorCodeObject.SetActive(false);
				break;
			}
		}
	}

	// Token: 0x06001AE7 RID: 6887 RVA: 0x0008428C File Offset: 0x0008248C
	public void SetATMText(string newText)
	{
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.atmText.text = newText;
		}
	}

	// Token: 0x06001AE8 RID: 6888 RVA: 0x000842E4 File Offset: 0x000824E4
	public void PressCurrencyPurchaseButton(string currencyPurchaseSize)
	{
		this.ProcessATMState(currencyPurchaseSize);
	}

	// Token: 0x06001AE9 RID: 6889 RVA: 0x000842ED File Offset: 0x000824ED
	public void VerifyCreatorCode()
	{
		this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Validating;
		NexusManager.instance.VerifyCreatorCode(this.currentCreatorCode, new Action<Member>(this.OnCreatorCodeSucess), new Action(this.OnCreatorCodeFailure));
	}

	// Token: 0x06001AEA RID: 6890 RVA: 0x00084320 File Offset: 0x00082520
	private void OnCreatorCodeSucess(Member member)
	{
		this.creatorCodeStatus = ATM_Manager.CreatorCodeStatus.Valid;
		this.supportedMember = member;
		this.ValidatedCreatorCode = this.currentCreatorCode;
		foreach (CreatorCodeSmallDisplay creatorCodeSmallDisplay in this.smallDisplays)
		{
			creatorCodeSmallDisplay.SetCode(this.ValidatedCreatorCode);
		}
		PlayerPrefs.SetString("CreatorCode", this.ValidatedCreatorCode);
		if (this.initialCode != this.ValidatedCreatorCode)
		{
			PlayerPrefs.SetString("CodeUsedTime", DateTime.Now.ToString());
		}
		PlayerPrefs.Save();
		Debug.Log("ATM CODE SUCCESS: " + this.supportedMember.name);
	}

	// Token: 0x06001AEB RID: 6891 RVA: 0x000843EC File Offset: 0x000825EC
	private void OnCreatorCodeFailure()
	{
		this.supportedMember = default(Member);
		this.ResetCreatorCode();
		foreach (ATM_UI atm_UI in this.atmUIs)
		{
			atm_UI.creatorCodeTitle.text = "CREATOR CODE: INVALID";
		}
		Debug.Log("ATM CODE FAILURE");
	}

	// Token: 0x06001AEC RID: 6892 RVA: 0x000023F4 File Offset: 0x000005F4
	public void LeaveSystemMenu()
	{
	}

	// Token: 0x04001DD7 RID: 7639
	[OnEnterPlay_SetNull]
	public static volatile ATM_Manager instance;

	// Token: 0x04001DD8 RID: 7640
	private const int MAX_CODE_LENGTH = 10;

	// Token: 0x04001DD9 RID: 7641
	public List<ATM_UI> atmUIs = new List<ATM_UI>();

	// Token: 0x04001DDA RID: 7642
	[HideInInspector]
	public List<CreatorCodeSmallDisplay> smallDisplays;

	// Token: 0x04001DDB RID: 7643
	private string currentCreatorCode;

	// Token: 0x04001DDC RID: 7644
	private string codeFirstUsedTime;

	// Token: 0x04001DDD RID: 7645
	private string initialCode;

	// Token: 0x04001DDE RID: 7646
	private string temporaryOverrideCode;

	// Token: 0x04001DE0 RID: 7648
	private ATM_Manager.CreatorCodeStatus creatorCodeStatus;

	// Token: 0x04001DE1 RID: 7649
	private ATM_Manager.ATMStages currentATMStage;

	// Token: 0x04001DE2 RID: 7650
	public int numShinyRocksToBuy;

	// Token: 0x04001DE3 RID: 7651
	public float shinyRocksCost;

	// Token: 0x04001DE4 RID: 7652
	private Member supportedMember;

	// Token: 0x04001DE5 RID: 7653
	public bool alreadyBegan;

	// Token: 0x02000442 RID: 1090
	public enum CreatorCodeStatus
	{
		// Token: 0x04001DE7 RID: 7655
		Empty,
		// Token: 0x04001DE8 RID: 7656
		Unchecked,
		// Token: 0x04001DE9 RID: 7657
		Validating,
		// Token: 0x04001DEA RID: 7658
		Valid
	}

	// Token: 0x02000443 RID: 1091
	public enum ATMStages
	{
		// Token: 0x04001DEC RID: 7660
		Unavailable,
		// Token: 0x04001DED RID: 7661
		Begin,
		// Token: 0x04001DEE RID: 7662
		Menu,
		// Token: 0x04001DEF RID: 7663
		Balance,
		// Token: 0x04001DF0 RID: 7664
		Choose,
		// Token: 0x04001DF1 RID: 7665
		Confirm,
		// Token: 0x04001DF2 RID: 7666
		Purchasing,
		// Token: 0x04001DF3 RID: 7667
		Success,
		// Token: 0x04001DF4 RID: 7668
		Failure,
		// Token: 0x04001DF5 RID: 7669
		SafeAccount
	}
}
