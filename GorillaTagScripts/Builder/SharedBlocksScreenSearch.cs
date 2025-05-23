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
		// Token: 0x0600494B RID: 18763 RVA: 0x0015E1FC File Offset: 0x0015C3FC
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

		// Token: 0x0600494C RID: 18764 RVA: 0x0015E263 File Offset: 0x0015C463
		public override void OnDeletePressed()
		{
			if (this.currentMapCode.Length > 0)
			{
				this.currentMapCode = this.currentMapCode.Substring(0, this.currentMapCode.Length - 1);
				this.UpdateInput();
			}
		}

		// Token: 0x0600494D RID: 18765 RVA: 0x0015E298 File Offset: 0x0015C498
		public override void OnNumberPressed(int number)
		{
			if (this.currentMapCode.Length < 8)
			{
				this.currentMapCode += number.ToString();
				this.UpdateInput();
			}
		}

		// Token: 0x0600494E RID: 18766 RVA: 0x0015E2C6 File Offset: 0x0015C4C6
		public override void OnLetterPressed(string letter)
		{
			if (this.currentMapCode.Length < 8)
			{
				this.currentMapCode += letter;
				this.UpdateInput();
			}
		}

		// Token: 0x0600494F RID: 18767 RVA: 0x0015E2F0 File Offset: 0x0015C4F0
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

		// Token: 0x06004950 RID: 18768 RVA: 0x0015E37C File Offset: 0x0015C57C
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

		// Token: 0x06004951 RID: 18769 RVA: 0x0015E404 File Offset: 0x0015C604
		private void OnMapLoaded(string mapID)
		{
			this.loadedMap.text = "LOADED MAP : " + (SharedBlocksManager.IsMapIDValid(mapID) ? SharedBlocksTerminal.MapIDToDisplayedString(mapID) : "NONE");
		}

		// Token: 0x06004952 RID: 18770 RVA: 0x0015E430 File Offset: 0x0015C630
		private void OnMapCleared()
		{
			this.loadedMap.text = "LOADED MAP : NONE";
		}

		// Token: 0x06004953 RID: 18771 RVA: 0x0015E442 File Offset: 0x0015C642
		private void UpdateInput()
		{
			this.inputText.text = "MAP SEARCH : " + SharedBlocksTerminal.MapIDToDisplayedString(this.currentMapCode);
		}

		// Token: 0x06004954 RID: 18772 RVA: 0x0015E464 File Offset: 0x0015C664
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

		// Token: 0x06004955 RID: 18773 RVA: 0x0015E483 File Offset: 0x0015C683
		public void SetInputTextEnabled(bool enabled)
		{
			if (enabled)
			{
				this.inputText.color = Color.white;
				return;
			}
			this.inputText.color = Color.gray;
		}

		// Token: 0x06004956 RID: 18774 RVA: 0x0015E4AC File Offset: 0x0015C6AC
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

		// Token: 0x06004957 RID: 18775 RVA: 0x0015E5F4 File Offset: 0x0015C7F4
		private void RefreshPlayerCounter()
		{
			this.terminal.RefreshLobbyCount();
			this.playerCountText.text = this.terminal.GetLobbyText();
			this.playersInLobbyWarning.gameObject.SetActive(!this.terminal.AreAllPlayersInLobby());
		}

		// Token: 0x06004958 RID: 18776 RVA: 0x0015E640 File Offset: 0x0015C840
		public void SliceUpdate()
		{
			this.RefreshPlayerCounter();
		}

		// Token: 0x06004959 RID: 18777 RVA: 0x0015E648 File Offset: 0x0015C848
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

		// Token: 0x0600495A RID: 18778 RVA: 0x0015E640 File Offset: 0x0015C840
		private void PlayersChangedEvent()
		{
			this.RefreshPlayerCounter();
		}

		// Token: 0x0600495B RID: 18779 RVA: 0x0015E686 File Offset: 0x0015C886
		public void OnDisable()
		{
			if (this.updating)
			{
				GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
				this.updating = false;
			}
			RoomSystem.PlayersChangedEvent = (Action)Delegate.Remove(RoomSystem.PlayersChangedEvent, new Action(this.PlayersChangedEvent));
		}

		// Token: 0x0600495D RID: 18781 RVA: 0x00011040 File Offset: 0x0000F240
		bool IGorillaSliceableSimple.get_isActiveAndEnabled()
		{
			return base.isActiveAndEnabled;
		}

		// Token: 0x04004C24 RID: 19492
		[SerializeField]
		private TMP_Text loadedMap;

		// Token: 0x04004C25 RID: 19493
		[SerializeField]
		private TMP_Text inputText;

		// Token: 0x04004C26 RID: 19494
		[SerializeField]
		private TMP_Text statusText;

		// Token: 0x04004C27 RID: 19495
		[SerializeField]
		private TMP_Text recentList;

		// Token: 0x04004C28 RID: 19496
		[SerializeField]
		private TMP_Text myScanList;

		// Token: 0x04004C29 RID: 19497
		[SerializeField]
		private TMP_Text playerCountText;

		// Token: 0x04004C2A RID: 19498
		[SerializeField]
		private TMP_Text playersInLobbyWarning;

		// Token: 0x04004C2B RID: 19499
		private string currentMapCode;

		// Token: 0x04004C2C RID: 19500
		private string savedMapCode;

		// Token: 0x04004C2D RID: 19501
		private StringBuilder sb = new StringBuilder();

		// Token: 0x04004C2E RID: 19502
		private bool updating;
	}
}
