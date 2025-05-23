using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GorillaTagScripts;
using GorillaTagScripts.Builder;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000510 RID: 1296
public class BuilderScanKiosk : MonoBehaviour
{
	// Token: 0x06001F5F RID: 8031 RVA: 0x0009D75C File Offset: 0x0009B95C
	private void Start()
	{
		if (this.saveButton != null)
		{
			this.saveButton.onPressButton.AddListener(new UnityAction(this.OnSavePressed));
		}
		if (this.targetTable != null)
		{
			this.targetTable.OnSaveDirtyChanged.AddListener(new UnityAction<bool>(this.OnSaveDirtyChanged));
			this.targetTable.OnSaveSuccess.AddListener(new UnityAction(this.OnSaveSuccess));
			this.targetTable.OnSaveFailure.AddListener(new UnityAction<string>(this.OnSaveFail));
			SharedBlocksManager.OnSaveTimeUpdated += this.OnSaveTimeUpdated;
		}
		if (this.noneButton != null)
		{
			this.noneButton.onPressButton.AddListener(new UnityAction(this.OnNoneButtonPressed));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.scanButtons)
		{
			gorillaPressableButton.onPressed += this.OnScanButtonPressed;
		}
		this.scanTriangle = this.scanAnimation.GetComponent<MeshRenderer>();
		this.scanTriangle.enabled = false;
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.LoadPlayerPrefs();
	}

	// Token: 0x06001F60 RID: 8032 RVA: 0x0009D8AC File Offset: 0x0009BAAC
	private void OnDestroy()
	{
		if (this.saveButton != null)
		{
			this.saveButton.onPressButton.RemoveListener(new UnityAction(this.OnSavePressed));
		}
		SharedBlocksManager.OnSaveTimeUpdated -= this.OnSaveTimeUpdated;
		if (this.targetTable != null)
		{
			this.targetTable.OnSaveDirtyChanged.RemoveListener(new UnityAction<bool>(this.OnSaveDirtyChanged));
			this.targetTable.OnSaveFailure.RemoveListener(new UnityAction<string>(this.OnSaveFail));
		}
		if (this.noneButton != null)
		{
			this.noneButton.onPressButton.RemoveListener(new UnityAction(this.OnNoneButtonPressed));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.scanButtons)
		{
			if (!(gorillaPressableButton == null))
			{
				gorillaPressableButton.onPressed -= this.OnScanButtonPressed;
			}
		}
	}

	// Token: 0x06001F61 RID: 8033 RVA: 0x0009D9C0 File Offset: 0x0009BBC0
	private void OnNoneButtonPressed()
	{
		if (this.targetTable == null)
		{
			return;
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		}
		if (this.targetTable.CurrentSaveSlot != -1)
		{
			this.targetTable.CurrentSaveSlot = -1;
			this.SavePlayerPrefs();
			this.UpdateUI();
		}
	}

	// Token: 0x06001F62 RID: 8034 RVA: 0x0009DA14 File Offset: 0x0009BC14
	private void OnScanButtonPressed(GorillaPressableButton button, bool isLeft)
	{
		if (this.targetTable == null)
		{
			return;
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		}
		int i = 0;
		while (i < this.scanButtons.Count)
		{
			if (button.Equals(this.scanButtons[i]))
			{
				if (i != this.targetTable.CurrentSaveSlot)
				{
					this.targetTable.CurrentSaveSlot = i;
					this.SavePlayerPrefs();
					this.UpdateUI();
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x06001F63 RID: 8035 RVA: 0x0009DA94 File Offset: 0x0009BC94
	private void LoadPlayerPrefs()
	{
		int @int = PlayerPrefs.GetInt(BuilderScanKiosk.playerPrefKey, -1);
		this.targetTable.CurrentSaveSlot = @int;
		this.UpdateUI();
	}

	// Token: 0x06001F64 RID: 8036 RVA: 0x0009DABF File Offset: 0x0009BCBF
	private void SavePlayerPrefs()
	{
		PlayerPrefs.SetInt(BuilderScanKiosk.playerPrefKey, this.targetTable.CurrentSaveSlot);
		PlayerPrefs.Save();
	}

	// Token: 0x06001F65 RID: 8037 RVA: 0x0009DADC File Offset: 0x0009BCDC
	private void ToggleSaveButton(bool enabled)
	{
		if (enabled)
		{
			this.saveButton.enabled = true;
			this.saveButton.buttonRenderer.material = this.saveButton.unpressedMaterial;
			return;
		}
		this.saveButton.enabled = false;
		this.saveButton.buttonRenderer.material = this.saveButton.pressedMaterial;
	}

	// Token: 0x06001F66 RID: 8038 RVA: 0x0009DB3C File Offset: 0x0009BD3C
	private void Update()
	{
		if (this.isAnimating)
		{
			if (this.scanAnimation == null)
			{
				this.isAnimating = false;
			}
			else if ((double)Time.time > this.scanCompleteTime)
			{
				this.scanTriangle.enabled = false;
				this.isAnimating = false;
			}
		}
		if (this.coolingDown && (double)Time.time > this.coolDownCompleteTime)
		{
			this.coolingDown = false;
			this.UpdateUI();
		}
	}

	// Token: 0x06001F67 RID: 8039 RVA: 0x0009DBAC File Offset: 0x0009BDAC
	private void OnSavePressed()
	{
		if (this.targetTable == null || !this.isDirty || this.coolingDown)
		{
			return;
		}
		BuilderScanKiosk.ScannerState scannerState = this.scannerState;
		if (scannerState == BuilderScanKiosk.ScannerState.IDLE)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.CONFIRMATION;
			this.UpdateUI();
			return;
		}
		if (scannerState != BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			return;
		}
		this.scannerState = BuilderScanKiosk.ScannerState.SAVING;
		if (this.scanAnimation != null)
		{
			this.scanCompleteTime = (double)(Time.time + this.scanAnimation.clip.length);
			this.scanTriangle.enabled = true;
			this.scanAnimation.Rewind();
			this.scanAnimation.Play();
		}
		if (this.soundBank != null)
		{
			this.soundBank.Play();
		}
		this.isAnimating = true;
		this.saveError = false;
		this.errorMsg = string.Empty;
		this.coolDownCompleteTime = (double)(Time.time + this.saveCooldownSeconds);
		this.coolingDown = true;
		this.UpdateUI();
		this.targetTable.SaveTableForPlayer();
	}

	// Token: 0x06001F68 RID: 8040 RVA: 0x0009DCA8 File Offset: 0x0009BEA8
	private string GetSavePath()
	{
		return string.Concat(new string[]
		{
			this.GetSaveFolder(),
			Path.DirectorySeparatorChar.ToString(),
			BuilderScanKiosk.SAVE_FILE,
			"_",
			this.targetTable.CurrentSaveSlot.ToString(),
			".png"
		});
	}

	// Token: 0x06001F69 RID: 8041 RVA: 0x0009DD04 File Offset: 0x0009BF04
	private string GetSaveFolder()
	{
		return Application.persistentDataPath + Path.DirectorySeparatorChar.ToString() + BuilderScanKiosk.SAVE_FOLDER;
	}

	// Token: 0x06001F6A RID: 8042 RVA: 0x0009DD1F File Offset: 0x0009BF1F
	private void OnSaveDirtyChanged(bool dirty)
	{
		this.isDirty = dirty;
		this.UpdateUI();
	}

	// Token: 0x06001F6B RID: 8043 RVA: 0x0009DD2E File Offset: 0x0009BF2E
	private void OnSaveTimeUpdated()
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = false;
		this.UpdateUI();
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x0009DD2E File Offset: 0x0009BF2E
	private void OnSaveSuccess()
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = false;
		this.UpdateUI();
	}

	// Token: 0x06001F6D RID: 8045 RVA: 0x0009DD44 File Offset: 0x0009BF44
	private void OnSaveFail(string errorMsg)
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = true;
		this.errorMsg = errorMsg;
		this.UpdateUI();
	}

	// Token: 0x06001F6E RID: 8046 RVA: 0x0009DD64 File Offset: 0x0009BF64
	private void UpdateUI()
	{
		this.screenText.text = this.GetTextForScreen();
		this.ToggleSaveButton(this.targetTable.CurrentSaveSlot >= 0 && !this.coolingDown);
		this.noneButton.buttonRenderer.material = ((this.targetTable.CurrentSaveSlot < 0) ? this.noneButton.pressedMaterial : this.noneButton.unpressedMaterial);
		for (int i = 0; i < this.scanButtons.Count; i++)
		{
			this.scanButtons[i].buttonRenderer.material = ((this.targetTable.CurrentSaveSlot == i) ? this.scanButtons[i].pressedMaterial : this.scanButtons[i].unpressedMaterial);
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			this.saveButton.myTmpText.text = "YES UPDATE SCAN";
			return;
		}
		this.saveButton.myTmpText.text = "UPDATE SCAN";
	}

	// Token: 0x06001F6F RID: 8047 RVA: 0x0009DE6C File Offset: 0x0009C06C
	private string GetTextForScreen()
	{
		if (this.targetTable == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (this.targetTable.CurrentSaveSlot < 0)
		{
			stringBuilder.Append("<b><color=red>NONE</color></b>");
		}
		else
		{
			int currentSaveSlot = this.targetTable.CurrentSaveSlot;
			stringBuilder.Append("<b><color=red>");
			stringBuilder.Append("SCAN ");
			stringBuilder.Append(currentSaveSlot + 1);
			stringBuilder.Append("</color></b>");
			SharedBlocksManager.LocalPublishInfo publishInfoForSlot = SharedBlocksManager.GetPublishInfoForSlot(currentSaveSlot);
			DateTime dateTime = DateTime.FromBinary(publishInfoForSlot.publishTime);
			if (dateTime > DateTime.MinValue)
			{
				stringBuilder.Append(": ");
				stringBuilder.Append("UPDATED ");
				stringBuilder.Append(dateTime.ToString());
				stringBuilder.Append("\n");
			}
			if (SharedBlocksManager.IsMapIDValid(publishInfoForSlot.mapID))
			{
				stringBuilder.Append("MAP ID: ");
				stringBuilder.Append(publishInfoForSlot.mapID.Substring(0, 4));
				stringBuilder.Append("-");
				stringBuilder.Append(publishInfoForSlot.mapID.Substring(4));
				stringBuilder.Append("\nUSE THIS CODE IN THE SHARE MY BLOCKS ROOM");
			}
		}
		stringBuilder.Append("\n");
		switch (this.scannerState)
		{
		case BuilderScanKiosk.ScannerState.IDLE:
			if (this.saveError)
			{
				stringBuilder.Append("ERROR WHILE SCANNING: ");
				stringBuilder.Append(this.errorMsg);
			}
			else if (this.coolingDown)
			{
				stringBuilder.Append("COOLING DOWN...");
			}
			else if (!this.isDirty)
			{
				stringBuilder.Append("NO UNSAVED CHANGES");
			}
			break;
		case BuilderScanKiosk.ScannerState.CONFIRMATION:
			stringBuilder.Append("YOU ARE ABOUT TO REPLACE ");
			stringBuilder.Append("<b><color=red>SCAN ");
			stringBuilder.Append(this.targetTable.CurrentSaveSlot + 1);
			stringBuilder.Append("</color></b>");
			stringBuilder.Append(" ARE YOU SURE YOU WANT TO SCAN?");
			break;
		case BuilderScanKiosk.ScannerState.SAVING:
			stringBuilder.Append("SCANNING BUILD...");
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		stringBuilder.Append("\n\n\n");
		stringBuilder.Append("CREATE A <b><color=red>NEW</color></b> PRIVATE ROOM TO LOAD ");
		if (this.targetTable.CurrentSaveSlot < 0)
		{
			stringBuilder.Append("<b><color=red>AN EMPTY TABLE</color></b>");
		}
		else
		{
			stringBuilder.Append("<b><color=red>");
			stringBuilder.Append("SCAN ");
			stringBuilder.Append(this.targetTable.CurrentSaveSlot + 1);
			stringBuilder.Append("</color></b>");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04002337 RID: 9015
	[SerializeField]
	private GorillaPressableButton saveButton;

	// Token: 0x04002338 RID: 9016
	[SerializeField]
	private GorillaPressableButton noneButton;

	// Token: 0x04002339 RID: 9017
	[SerializeField]
	private List<GorillaPressableButton> scanButtons;

	// Token: 0x0400233A RID: 9018
	[SerializeField]
	private BuilderTable targetTable;

	// Token: 0x0400233B RID: 9019
	[SerializeField]
	private float saveCooldownSeconds = 5f;

	// Token: 0x0400233C RID: 9020
	[SerializeField]
	private TMP_Text screenText;

	// Token: 0x0400233D RID: 9021
	[SerializeField]
	private SoundBankPlayer soundBank;

	// Token: 0x0400233E RID: 9022
	[SerializeField]
	private Animation scanAnimation;

	// Token: 0x0400233F RID: 9023
	private MeshRenderer scanTriangle;

	// Token: 0x04002340 RID: 9024
	private bool isAnimating;

	// Token: 0x04002341 RID: 9025
	private static string playerPrefKey = "BuilderSaveSlot";

	// Token: 0x04002342 RID: 9026
	private static string SAVE_FOLDER = "MonkeBlocks";

	// Token: 0x04002343 RID: 9027
	private static string SAVE_FILE = "MyBuild";

	// Token: 0x04002344 RID: 9028
	public static int NUM_SAVE_SLOTS = 3;

	// Token: 0x04002345 RID: 9029
	private Texture2D buildCaptureTexture;

	// Token: 0x04002346 RID: 9030
	private bool isDirty;

	// Token: 0x04002347 RID: 9031
	private bool saveError;

	// Token: 0x04002348 RID: 9032
	private string errorMsg = string.Empty;

	// Token: 0x04002349 RID: 9033
	private bool coolingDown;

	// Token: 0x0400234A RID: 9034
	private double coolDownCompleteTime;

	// Token: 0x0400234B RID: 9035
	private double scanCompleteTime;

	// Token: 0x0400234C RID: 9036
	private BuilderScanKiosk.ScannerState scannerState;

	// Token: 0x02000511 RID: 1297
	private enum ScannerState
	{
		// Token: 0x0400234E RID: 9038
		IDLE,
		// Token: 0x0400234F RID: 9039
		CONFIRMATION,
		// Token: 0x04002350 RID: 9040
		SAVING
	}
}
