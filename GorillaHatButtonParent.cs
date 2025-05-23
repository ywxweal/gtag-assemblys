using System;
using UnityEngine;

// Token: 0x02000610 RID: 1552
public class GorillaHatButtonParent : MonoBehaviour
{
	// Token: 0x06002689 RID: 9865 RVA: 0x000BE784 File Offset: 0x000BC984
	public void Start()
	{
		this.hat = PlayerPrefs.GetString("hatCosmetic", "none");
		this.face = PlayerPrefs.GetString("faceCosmetic", "none");
		this.badge = PlayerPrefs.GetString("badgeCosmetic", "none");
		this.leftHandHold = PlayerPrefs.GetString("leftHandHoldCosmetic", "none");
		this.rightHandHold = PlayerPrefs.GetString("rightHandHoldCosmetic", "none");
	}

	// Token: 0x0600268A RID: 9866 RVA: 0x000BE7FC File Offset: 0x000BC9FC
	public void LateUpdate()
	{
		if (!this.initialized && GorillaTagger.Instance.offlineVRRig.InitializedCosmetics)
		{
			this.initialized = true;
			if (GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed.Contains("AdministratorBadge"))
			{
				foreach (GameObject gameObject in this.adminObjects)
				{
					Debug.Log("doing this?");
					gameObject.SetActive(true);
				}
			}
			if (GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed.Contains("earlyaccess"))
			{
				this.UpdateButtonState();
				this.screen.UpdateText("WELCOME TO THE HAT ROOM!\nTHANK YOU FOR PURCHASING THE EARLY ACCESS SUPPORTER PACK! PLEASE ENJOY THESE VARIOUS HATS AND NOT-HATS!", true);
			}
		}
	}

	// Token: 0x0600268B RID: 9867 RVA: 0x000BE8A4 File Offset: 0x000BCAA4
	public void PressButton(bool isOn, GorillaHatButton.HatButtonType buttonType, string buttonValue)
	{
		if (this.initialized && GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed.Contains("earlyaccess"))
		{
			switch (buttonType)
			{
			case GorillaHatButton.HatButtonType.Hat:
				if (this.hat != buttonValue)
				{
					this.hat = buttonValue;
					PlayerPrefs.SetString("hatCosmetic", buttonValue);
				}
				else
				{
					this.hat = "none";
					PlayerPrefs.SetString("hatCosmetic", "none");
				}
				break;
			case GorillaHatButton.HatButtonType.Face:
				if (this.face != buttonValue)
				{
					this.face = buttonValue;
					PlayerPrefs.SetString("faceCosmetic", buttonValue);
				}
				else
				{
					this.face = "none";
					PlayerPrefs.SetString("faceCosmetic", "none");
				}
				break;
			case GorillaHatButton.HatButtonType.Badge:
				if (this.badge != buttonValue)
				{
					this.badge = buttonValue;
					PlayerPrefs.SetString("badgeCosmetic", buttonValue);
				}
				else
				{
					this.badge = "none";
					PlayerPrefs.SetString("badgeCosmetic", "none");
				}
				break;
			}
			PlayerPrefs.Save();
			this.UpdateButtonState();
		}
	}

	// Token: 0x0600268C RID: 9868 RVA: 0x000BE9B8 File Offset: 0x000BCBB8
	private void UpdateButtonState()
	{
		foreach (GorillaHatButton gorillaHatButton in this.hatButtons)
		{
			switch (gorillaHatButton.buttonType)
			{
			case GorillaHatButton.HatButtonType.Hat:
				gorillaHatButton.isOn = gorillaHatButton.cosmeticName == this.hat;
				break;
			case GorillaHatButton.HatButtonType.Face:
				gorillaHatButton.isOn = gorillaHatButton.cosmeticName == this.face;
				break;
			case GorillaHatButton.HatButtonType.Badge:
				gorillaHatButton.isOn = gorillaHatButton.cosmeticName == this.badge;
				break;
			}
			gorillaHatButton.UpdateColor();
		}
	}

	// Token: 0x04002AF3 RID: 10995
	public GorillaHatButton[] hatButtons;

	// Token: 0x04002AF4 RID: 10996
	public GameObject[] adminObjects;

	// Token: 0x04002AF5 RID: 10997
	public string hat;

	// Token: 0x04002AF6 RID: 10998
	public string face;

	// Token: 0x04002AF7 RID: 10999
	public string badge;

	// Token: 0x04002AF8 RID: 11000
	public string leftHandHold;

	// Token: 0x04002AF9 RID: 11001
	public string rightHandHold;

	// Token: 0x04002AFA RID: 11002
	public bool initialized;

	// Token: 0x04002AFB RID: 11003
	public GorillaLevelScreen screen;
}
