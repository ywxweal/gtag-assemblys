using System;
using System.Collections.Generic;
using System.Text;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200048E RID: 1166
public class GameModePages : BasePageHandler
{
	// Token: 0x1700031C RID: 796
	// (get) Token: 0x06001C7C RID: 7292 RVA: 0x0008B1E5 File Offset: 0x000893E5
	protected override int pageSize
	{
		get
		{
			return this.buttons.Length;
		}
	}

	// Token: 0x1700031D RID: 797
	// (get) Token: 0x06001C7D RID: 7293 RVA: 0x0008B1EF File Offset: 0x000893EF
	protected override int entriesCount
	{
		get
		{
			return GameMode.gameModeNames.Count;
		}
	}

	// Token: 0x06001C7E RID: 7294 RVA: 0x0008B1FC File Offset: 0x000893FC
	private void Awake()
	{
		GameModePages.gameModeSelectorInstances.Add(this);
		this.buttons = base.GetComponentsInChildren<GameModeSelectButton>();
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].buttonIndex = i;
			this.buttons[i].selector = this;
		}
	}

	// Token: 0x06001C7F RID: 7295 RVA: 0x0008B24F File Offset: 0x0008944F
	protected override void Start()
	{
		base.Start();
		base.SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		this.initialized = true;
	}

	// Token: 0x06001C80 RID: 7296 RVA: 0x0008B269 File Offset: 0x00089469
	private void OnEnable()
	{
		if (this.initialized)
		{
			base.SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		}
	}

	// Token: 0x06001C81 RID: 7297 RVA: 0x0008B27E File Offset: 0x0008947E
	private void OnDestroy()
	{
		GameModePages.gameModeSelectorInstances.Remove(this);
	}

	// Token: 0x06001C82 RID: 7298 RVA: 0x0008B28C File Offset: 0x0008948C
	protected override void ShowPage(int selectedPage, int startIndex, int endIndex)
	{
		GameModePages.textBuilder.Clear();
		for (int i = startIndex; i < endIndex; i++)
		{
			GameModePages.textBuilder.AppendLine(GameMode.gameModeNames[i]);
		}
		this.gameModeText.text = GameModePages.textBuilder.ToString();
		if (base.selectedIndex >= startIndex && base.selectedIndex <= endIndex)
		{
			this.UpdateAllButtons(this.currentButtonIndex);
		}
		else
		{
			this.UpdateAllButtons(-1);
		}
		int num = ((selectedPage == base.pages - 1 && base.maxEntires > endIndex) ? (base.maxEntires - endIndex) : 0);
		this.EnableEntryButtons(num);
	}

	// Token: 0x06001C83 RID: 7299 RVA: 0x0008B329 File Offset: 0x00089529
	protected override void PageEntrySelected(int pageEntry, int selectionIndex)
	{
		if (selectionIndex >= this.entriesCount)
		{
			return;
		}
		GameModePages.sharedSelectedIndex = selectionIndex;
		this.UpdateAllButtons(pageEntry);
		this.currentButtonIndex = pageEntry;
		GorillaComputer.instance.OnModeSelectButtonPress(GameMode.gameModeNames[selectionIndex], false);
	}

	// Token: 0x06001C84 RID: 7300 RVA: 0x0008B364 File Offset: 0x00089564
	private void UpdateAllButtons(int onButton)
	{
		for (int i = 0; i < this.buttons.Length; i++)
		{
			if (i == onButton)
			{
				this.buttons[onButton].isOn = true;
				this.buttons[onButton].UpdateColor();
			}
			else if (this.buttons[i].isOn)
			{
				this.buttons[i].isOn = false;
				this.buttons[i].UpdateColor();
			}
		}
	}

	// Token: 0x06001C85 RID: 7301 RVA: 0x0008B3D0 File Offset: 0x000895D0
	private void EnableEntryButtons(int buttonsMissing)
	{
		int num = this.buttons.Length - buttonsMissing;
		int i;
		for (i = 0; i < num; i++)
		{
			this.buttons[i].gameObject.SetActive(true);
		}
		while (i < this.buttons.Length)
		{
			this.buttons[i].gameObject.SetActive(false);
			i++;
		}
	}

	// Token: 0x06001C86 RID: 7302 RVA: 0x0008B42C File Offset: 0x0008962C
	public static void SetSelectedGameModeShared(string gameMode)
	{
		GameModePages.sharedSelectedIndex = GameMode.gameModeNames.IndexOf(gameMode);
		if (GameModePages.sharedSelectedIndex < 0)
		{
			return;
		}
		for (int i = 0; i < GameModePages.gameModeSelectorInstances.Count; i++)
		{
			GameModePages.gameModeSelectorInstances[i].SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		}
	}

	// Token: 0x04001F8E RID: 8078
	private int currentButtonIndex;

	// Token: 0x04001F8F RID: 8079
	[SerializeField]
	private Text gameModeText;

	// Token: 0x04001F90 RID: 8080
	[SerializeField]
	private GameModeSelectButton[] buttons;

	// Token: 0x04001F91 RID: 8081
	private bool initialized;

	// Token: 0x04001F92 RID: 8082
	private static int sharedSelectedIndex = 0;

	// Token: 0x04001F93 RID: 8083
	private static StringBuilder textBuilder = new StringBuilder(50);

	// Token: 0x04001F94 RID: 8084
	[OnEnterPlay_Clear]
	private static List<GameModePages> gameModeSelectorInstances = new List<GameModePages>(7);
}
