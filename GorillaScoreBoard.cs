using System;
using System.Collections.Generic;
using System.Text;
using GorillaGameModes;
using TMPro;
using UnityEngine;

// Token: 0x02000634 RID: 1588
public class GorillaScoreBoard : MonoBehaviour
{
	// Token: 0x170003C4 RID: 964
	// (get) Token: 0x06002790 RID: 10128 RVA: 0x000C3FE4 File Offset: 0x000C21E4
	// (set) Token: 0x06002791 RID: 10129 RVA: 0x000C3FFB File Offset: 0x000C21FB
	public bool IsDirty
	{
		get
		{
			return this._isDirty || string.IsNullOrEmpty(this.initialGameMode);
		}
		set
		{
			this._isDirty = value;
		}
	}

	// Token: 0x06002792 RID: 10130 RVA: 0x000C4004 File Offset: 0x000C2204
	public void SetSleepState(bool awake)
	{
		this.boardText.enabled = awake;
		this.buttonText.enabled = awake;
		if (this.linesParent != null)
		{
			this.linesParent.SetActive(awake);
		}
	}

	// Token: 0x06002793 RID: 10131 RVA: 0x000023F4 File Offset: 0x000005F4
	private void OnDestroy()
	{
	}

	// Token: 0x06002794 RID: 10132 RVA: 0x000C4038 File Offset: 0x000C2238
	public string GetBeginningString()
	{
		return "ROOM ID: " + (NetworkSystem.Instance.SessionIsPrivate ? "-PRIVATE- GAME: " : (NetworkSystem.Instance.RoomName + "   GAME: ")) + this.RoomType() + "\n  PLAYER     COLOR  MUTE   REPORT";
	}

	// Token: 0x06002795 RID: 10133 RVA: 0x000C4078 File Offset: 0x000C2278
	public string RoomType()
	{
		this.initialGameMode = RoomSystem.RoomGameMode;
		this.gmNames = GameMode.gameModeNames;
		this.gmName = "ERROR";
		int count = this.gmNames.Count;
		for (int i = 0; i < count; i++)
		{
			this.tempGmName = this.gmNames[i];
			if (this.initialGameMode.Contains(this.tempGmName))
			{
				this.gmName = this.tempGmName;
				break;
			}
		}
		return this.gmName;
	}

	// Token: 0x06002796 RID: 10134 RVA: 0x000C40F8 File Offset: 0x000C22F8
	public void RedrawPlayerLines()
	{
		this.stringBuilder.Clear();
		this.stringBuilder.Append(this.GetBeginningString());
		this.buttonStringBuilder.Clear();
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		for (int i = 0; i < this.lines.Count; i++)
		{
			try
			{
				if (this.lines[i].gameObject.activeInHierarchy)
				{
					this.lines[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)(this.startingYValue - this.lineHeight * i), 0f);
					if (this.lines[i].linePlayer != null && this.lines[i].linePlayer.InRoom)
					{
						this.stringBuilder.Append("\n ");
						this.stringBuilder.Append(flag ? this.lines[i].playerNameVisible : this.lines[i].linePlayer.DefaultName);
						if (this.lines[i].linePlayer != NetworkSystem.Instance.LocalPlayer)
						{
							if (this.lines[i].reportButton.isActiveAndEnabled)
							{
								this.buttonStringBuilder.Append("MUTE                                REPORT\n");
							}
							else
							{
								this.buttonStringBuilder.Append("MUTE                HATE SPEECH    TOXICITY     CHEATING       CANCEL\n");
							}
						}
						else
						{
							this.buttonStringBuilder.Append("\n");
						}
					}
				}
			}
			catch
			{
			}
		}
		this.boardText.text = this.stringBuilder.ToString();
		this.buttonText.text = this.buttonStringBuilder.ToString();
		this._isDirty = false;
	}

	// Token: 0x06002797 RID: 10135 RVA: 0x000C42E4 File Offset: 0x000C24E4
	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
			if (text.Length > 12)
			{
				text = text.Substring(0, 10);
			}
			text = text.ToUpper();
		}
		return text;
	}

	// Token: 0x06002798 RID: 10136 RVA: 0x000C4343 File Offset: 0x000C2543
	private void Start()
	{
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
	}

	// Token: 0x06002799 RID: 10137 RVA: 0x000C434B File Offset: 0x000C254B
	private void OnEnable()
	{
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
		this._isDirty = true;
	}

	// Token: 0x0600279A RID: 10138 RVA: 0x000C435A File Offset: 0x000C255A
	private void OnDisable()
	{
		GorillaScoreboardTotalUpdater.UnregisterScoreboard(this);
	}

	// Token: 0x04002C09 RID: 11273
	public GameObject scoreBoardLinePrefab;

	// Token: 0x04002C0A RID: 11274
	public int startingYValue;

	// Token: 0x04002C0B RID: 11275
	public int lineHeight;

	// Token: 0x04002C0C RID: 11276
	public bool includeMMR;

	// Token: 0x04002C0D RID: 11277
	public bool isActive;

	// Token: 0x04002C0E RID: 11278
	public GameObject linesParent;

	// Token: 0x04002C0F RID: 11279
	[SerializeField]
	public List<GorillaPlayerScoreboardLine> lines;

	// Token: 0x04002C10 RID: 11280
	public TextMeshPro boardText;

	// Token: 0x04002C11 RID: 11281
	public TextMeshPro buttonText;

	// Token: 0x04002C12 RID: 11282
	public bool needsUpdate;

	// Token: 0x04002C13 RID: 11283
	public TextMeshPro notInRoomText;

	// Token: 0x04002C14 RID: 11284
	public string initialGameMode;

	// Token: 0x04002C15 RID: 11285
	private string tempGmName;

	// Token: 0x04002C16 RID: 11286
	private string gmName;

	// Token: 0x04002C17 RID: 11287
	private const string error = "ERROR";

	// Token: 0x04002C18 RID: 11288
	private List<string> gmNames;

	// Token: 0x04002C19 RID: 11289
	private bool _isDirty = true;

	// Token: 0x04002C1A RID: 11290
	private StringBuilder stringBuilder = new StringBuilder(220);

	// Token: 0x04002C1B RID: 11291
	private StringBuilder buttonStringBuilder = new StringBuilder(720);
}
