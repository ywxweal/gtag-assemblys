using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B96 RID: 2966
	public class SharedBlocksVotingStation : MonoBehaviour
	{
		// Token: 0x0600498E RID: 18830 RVA: 0x0015F644 File Offset: 0x0015D844
		private void Start()
		{
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(this.tableZone, out builderTable))
			{
				this.table = builderTable;
				this.table.OnMapLoaded.AddListener(new UnityAction<string>(this.OnLoadedMapChanged));
				this.table.OnMapCleared.AddListener(new UnityAction(this.OnMapCleared));
				this.OnLoadedMapChanged(this.table.GetCurrentMapID());
			}
			else
			{
				GTDev.LogWarning<string>("No Builder Table found for Voting Station", null);
			}
			this.upVoteButton.onPressButton.AddListener(new UnityAction(this.OnUpVotePressed));
			this.downVoteButton.onPressButton.AddListener(new UnityAction(this.OnDownVotePressed));
		}

		// Token: 0x0600498F RID: 18831 RVA: 0x0015F6F8 File Offset: 0x0015D8F8
		private void OnDestroy()
		{
			this.upVoteButton.onPressButton.RemoveListener(new UnityAction(this.OnUpVotePressed));
			this.downVoteButton.onPressButton.RemoveListener(new UnityAction(this.OnDownVotePressed));
			if (this.table != null)
			{
				this.table.OnMapLoaded.RemoveListener(new UnityAction<string>(this.OnLoadedMapChanged));
				this.table.OnMapCleared.RemoveListener(new UnityAction(this.OnMapCleared));
			}
		}

		// Token: 0x06004990 RID: 18832 RVA: 0x0015F784 File Offset: 0x0015D984
		private void OnUpVotePressed()
		{
			if (this.voteInProgress)
			{
				return;
			}
			this.voteInProgress = true;
			this.statusText.text = "";
			this.statusText.gameObject.SetActive(false);
			if (SharedBlocksManager.IsMapIDValid(this.loadedMapID) && this.upVoteButton.enabled)
			{
				SharedBlocksManager.instance.RequestVote(this.loadedMapID, true, new Action<bool, string>(this.OnVoteResponse));
				this.upVoteButton.buttonRenderer.material = this.upVoteButton.pressedMaterial;
				this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.upVoteButton.enabled = false;
				this.downVoteButton.enabled = true;
			}
		}

		// Token: 0x06004991 RID: 18833 RVA: 0x0015F844 File Offset: 0x0015DA44
		private void OnDownVotePressed()
		{
			if (this.voteInProgress)
			{
				return;
			}
			this.voteInProgress = true;
			this.statusText.text = "";
			this.statusText.gameObject.SetActive(false);
			if (SharedBlocksManager.IsMapIDValid(this.loadedMapID) && this.downVoteButton.enabled)
			{
				SharedBlocksManager.instance.RequestVote(this.loadedMapID, false, new Action<bool, string>(this.OnVoteResponse));
				this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.downVoteButton.buttonRenderer.material = this.downVoteButton.pressedMaterial;
				this.upVoteButton.enabled = true;
				this.downVoteButton.enabled = false;
			}
		}

		// Token: 0x06004992 RID: 18834 RVA: 0x0015F904 File Offset: 0x0015DB04
		private void OnVoteResponse(bool success, string message)
		{
			this.voteInProgress = false;
			if (success)
			{
				this.statusText.text = "Successfully submitted vote";
				this.statusText.gameObject.SetActive(true);
			}
			else
			{
				this.statusText.text = message;
				this.statusText.gameObject.SetActive(true);
				if (!this.loadedMapID.IsNullOrEmpty())
				{
					this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
					this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
					this.upVoteButton.enabled = true;
					this.downVoteButton.enabled = true;
				}
			}
			this.clearStatusTime = Time.time + this.clearStatusDelay;
			this.waitingToClearStatus = true;
		}

		// Token: 0x06004993 RID: 18835 RVA: 0x0015F9C5 File Offset: 0x0015DBC5
		private void LateUpdate()
		{
			if (this.waitingToClearStatus && Time.time > this.clearStatusTime)
			{
				this.waitingToClearStatus = false;
				this.statusText.text = "";
				this.statusText.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004994 RID: 18836 RVA: 0x0015FA04 File Offset: 0x0015DC04
		private void OnLoadedMapChanged(string mapID)
		{
			this.loadedMapID = mapID;
			this.statusText.gameObject.SetActive(false);
			this.UpdateScreen();
		}

		// Token: 0x06004995 RID: 18837 RVA: 0x0015FA24 File Offset: 0x0015DC24
		private void OnMapCleared()
		{
			this.loadedMapID = null;
			this.statusText.gameObject.SetActive(false);
			this.UpdateScreen();
		}

		// Token: 0x06004996 RID: 18838 RVA: 0x0015FA44 File Offset: 0x0015DC44
		private void UpdateScreen()
		{
			if (!this.loadedMapID.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(this.loadedMapID))
			{
				this.screenText.text = "MAP: " + SharedBlocksTerminal.MapIDToDisplayedString(this.loadedMapID);
				this.upVoteButton.enabled = true;
				this.downVoteButton.enabled = true;
				this.upVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				this.downVoteButton.buttonRenderer.material = this.buttonDefaultMaterial;
				return;
			}
			this.screenText.text = "MAP: NONE";
			this.upVoteButton.enabled = false;
			this.downVoteButton.enabled = false;
			this.upVoteButton.buttonRenderer.material = this.buttonDisabledMaterial;
			this.downVoteButton.buttonRenderer.material = this.buttonDisabledMaterial;
		}

		// Token: 0x04004C5F RID: 19551
		[SerializeField]
		private TMP_Text screenText;

		// Token: 0x04004C60 RID: 19552
		[SerializeField]
		private TMP_Text statusText;

		// Token: 0x04004C61 RID: 19553
		[SerializeField]
		private GorillaPressableButton upVoteButton;

		// Token: 0x04004C62 RID: 19554
		[SerializeField]
		private GorillaPressableButton downVoteButton;

		// Token: 0x04004C63 RID: 19555
		[SerializeField]
		private GTZone tableZone = GTZone.monkeBlocksShared;

		// Token: 0x04004C64 RID: 19556
		[SerializeField]
		private Material buttonDefaultMaterial;

		// Token: 0x04004C65 RID: 19557
		[SerializeField]
		private Material buttonDisabledMaterial;

		// Token: 0x04004C66 RID: 19558
		private BuilderTable table;

		// Token: 0x04004C67 RID: 19559
		private string loadedMapID = string.Empty;

		// Token: 0x04004C68 RID: 19560
		private bool voteInProgress;

		// Token: 0x04004C69 RID: 19561
		private bool waitingToClearStatus;

		// Token: 0x04004C6A RID: 19562
		private float clearStatusTime;

		// Token: 0x04004C6B RID: 19563
		private float clearStatusDelay = 2f;
	}
}
