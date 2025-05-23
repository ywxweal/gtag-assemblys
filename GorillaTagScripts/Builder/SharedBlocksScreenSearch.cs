using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B91 RID: 2961
	public class SharedBlocksScreenSearch : SharedBlocksScreen, IGorillaSliceableSimple
	{
		// Token: 0x0600494C RID: 18764 RVA: 0x0015E2D4 File Offset: 0x0015C4D4
		public override void OnSelectPressed()
		{
			if (SharedBlocksManager.IsMapIDValid(this.currentMapCode))
			{
				this.savedMapCode = this.currentMapCode;
				this.terminal.SelectMapIDAndOpenInfo(this.savedMapCode);
				return;
			}
			if (this.currentMapCode.Length < 8)
			{
				this.terminal.SetStatusText("INVALID MAP ID LENGTH");
				return;
			}
			this.terminal.SetStatusText("INVALID MAP ID");
		}

		// Token: 0x0600494D RID: 18765 RVA: 0x0015E33B File Offset: 0x0015C53B
		public override void OnDeletePressed()
		{
			if (this.currentMapCode.Length > 0)
			{
				this.currentMapCode = this.currentMapCode.Substring(0, this.currentMapCode.Length - 1);
				this.UpdateInput();
			}
		}

		// Token: 0x0600494E RID: 18766 RVA: 0x0015E370 File Offset: 0x0015C570
		public override void OnNumberPressed(int number)
		{
			if (this.currentMapCode.Length < 8)
			{
				this.currentMapCode += number.ToString();
				this.UpdateInput();
			}
		}

		// Token: 0x0600494F RID: 18767 RVA: 0x0015E39E File Offset: 0x0015C59E
		public override void OnLetterPressed(string letter)
		{
			if (this.currentMapCode.Length < 8)
			{
				this.currentMapCode += letter;
				this.UpdateInput();
			}
		}

		// Token: 0x06004950 RID: 18768 RVA: 0x0015E3C8 File Offset: 0x0015C5C8
		public override void Show()
		{
			SharedBlocksManager.OnRecentMapIdsUpdated += this.DrawScreen;
			this.currentMapCode = string.Empty;
			this.DrawScreen();
			base.Show();
			this.RefreshPlayerCounter();
			BuilderTable table = this.terminal.GetTable();
			if (table != null)
			{
				table.OnMapLoaded.AddListener(new UnityAction<string>(this.OnMapLoaded));
				table.OnMapCleared.AddListener(new UnityAction(this.OnMapCleared));
				this.OnMapLoaded(table.GetCurrentMapID());
			}
		}

		// Token: 0x06004951 RID: 18769 RVA: 0x0015E454 File Offset: 0x0015C654
		public override void Hide()
		{
			BuilderTable table = this.terminal.GetTable();
			if (table != null)
			{
				table.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnMapLoaded));
				table.OnMapCleared.RemoveListener(new UnityAction(this.OnMapCleared));
			}
			this.statusText.text = "";
			this.statusText.gameObject.SetActive(false);
			SharedBlocksManager.OnRecentMapIdsUpdated -= this.DrawScreen;
			base.Hide();
		}

		// Token: 0x06004952 RID: 18770 RVA: 0x0015E4DC File Offset: 0x0015C6DC
		private void OnMapLoaded(string mapID)
		{
			this.loadedMap.text = "LOADED MAP : " + (SharedBlocksManager.IsMapIDValid(mapID) ? SharedBlocksTerminal.MapIDToDisplayedString(mapID) : "NONE");
		}

		// Token: 0x06004953 RID: 18771 RVA: 0x0015E508 File Offset: 0x0015C708
		private void OnMapCleared()
		{
			this.loadedMap.text = "LOADED MAP : NONE";
		}

		// Token: 0x06004954 RID: 18772 RVA: 0x0015E51A File Offset: 0x0015C71A
		private void UpdateInput()
		{
			this.inputText.text = "MAP SEARCH : " + SharedBlocksTerminal.MapIDToDisplayedString(this.currentMapCode);
		}

		// Token: 0x06004955 RID: 18773 RVA: 0x0015E53C File Offset: 0x0015C73C
		public void SetMapCode(string mapCode)
		{
			if (mapCode == null)
			{
				this.currentMapCode = string.Empty;
			}
			else
			{
				this.currentMapCode = mapCode;
			}
			this.UpdateInput();
		}

		// Token: 0x06004956 RID: 18774 RVA: 0x0015E55B File Offset: 0x0015C75B
		public void SetInputTextEnabled(bool enabled)
		{
			if (enabled)
			{
				this.inputText.color = Color.white;
				return;
			}
			this.inputText.color = Color.gray;
		}

		// Token: 0x06004957 RID: 18775 RVA: 0x0015E584 File Offset: 0x0015C784
		private void DrawScreen()
		{
			this.UpdateInput();
			this.sb.Clear();
			this.sb.Append("RECENT VOTES\n");
			foreach (string text in SharedBlocksManager.GetRecentUpVotes())
			{
				if (SharedBlocksManager.IsMapIDValid(text))
				{
					this.sb.Append(SharedBlocksTerminal.MapIDToDisplayedString(text));
					this.sb.Append("\n");
				}
			}
			this.recentList.text = this.sb.ToString();
			this.sb.Clear();
			this.sb.Append("MY MAPS\n");
			foreach (string text2 in SharedBlocksManager.GetLocalMapIDs())
			{
				if (SharedBlocksManager.IsMapIDValid(text2))
				{
					this.sb.Append(SharedBlocksTerminal.MapIDToDisplayedString(text2));
					this.sb.Append("\n");
				}
			}
			this.myScanList.text = this.sb.ToString();
		}

		// Token: 0x06004958 RID: 18776 RVA: 0x0015E6CC File Offset: 0x0015C8CC
		private void RefreshPlayerCounter()
		{
			this.terminal.RefreshLobbyCount();
			this.playerCountText.text = this.terminal.GetLobbyText();
			this.playersInLobbyWarning.gameObject.SetActive(!this.terminal.AreAllPlayersInLobby());
		}

		// Token: 0x06004959 RID: 18777 RVA: 0x0015E718 File Offset: 0x0015C918
		public void SliceUpdate()
		{
			this.RefreshPlayerCounter();
		}

		// Token: 0x0600495A RID: 18778 RVA: 0x0015E720 File Offset: 0x0015C920
		public void OnEnable()
		{
			if (!this.updating)
			{
				GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
				this.updating = true;
			}
			this.RefreshPlayerCounter();
			RoomSystem.PlayersChangedEvent = (Action)Delegate.Combine(RoomSystem.PlayersChangedEvent, new Action(this.PlayersChangedEvent));
		}

		// Token: 0x0600495B RID: 18779 RVA: 0x0015E718 File Offset: 0x0015C918
		private void PlayersChangedEvent()
		{
			this.RefreshPlayerCounter();
		}

		// Token: 0x0600495C RID: 18780 RVA: 0x0015E75E File Offset: 0x0015C95E
		public void OnDisable()
		{
			if (this.updating)
			{
				GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
				this.updating = false;
			}
			RoomSystem.PlayersChangedEvent = (Action)Delegate.Remove(RoomSystem.PlayersChangedEvent, new Action(this.PlayersChangedEvent));
		}

		// Token: 0x0600495E RID: 18782 RVA: 0x00011040 File Offset: 0x0000F240
		bool IGorillaSliceableSimple.get_isActiveAndEnabled()
		{
			return base.isActiveAndEnabled;
		}

		// Token: 0x04004C25 RID: 19493
		[SerializeField]
		private TMP_Text loadedMap;

		// Token: 0x04004C26 RID: 19494
		[SerializeField]
		private TMP_Text inputText;

		// Token: 0x04004C27 RID: 19495
		[SerializeField]
		private TMP_Text statusText;

		// Token: 0x04004C28 RID: 19496
		[SerializeField]
		private TMP_Text recentList;

		// Token: 0x04004C29 RID: 19497
		[SerializeField]
		private TMP_Text myScanList;

		// Token: 0x04004C2A RID: 19498
		[SerializeField]
		private TMP_Text playerCountText;

		// Token: 0x04004C2B RID: 19499
		[SerializeField]
		private TMP_Text playersInLobbyWarning;

		// Token: 0x04004C2C RID: 19500
		private string currentMapCode;

		// Token: 0x04004C2D RID: 19501
		private string savedMapCode;

		// Token: 0x04004C2E RID: 19502
		private StringBuilder sb = new StringBuilder();

		// Token: 0x04004C2F RID: 19503
		private bool updating;
	}
}
