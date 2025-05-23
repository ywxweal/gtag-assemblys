using System;
using System.Collections;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006E5 RID: 1765
public class GorillaScoreboardSpawner : MonoBehaviour
{
	// Token: 0x06002BEC RID: 11244 RVA: 0x000D8AD3 File Offset: 0x000D6CD3
	public void Awake()
	{
		base.StartCoroutine(this.UpdateBoard());
	}

	// Token: 0x06002BED RID: 11245 RVA: 0x000D8AE4 File Offset: 0x000D6CE4
	private void Start()
	{
		RoomSystem.JoinedRoomEvent = (Action)Delegate.Combine(RoomSystem.JoinedRoomEvent, new Action(this.OnJoinedRoom));
		RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(this.OnLeftRoom));
	}

	// Token: 0x06002BEE RID: 11246 RVA: 0x000D8B31 File Offset: 0x000D6D31
	public bool IsCurrentScoreboard()
	{
		return base.gameObject.activeInHierarchy;
	}

	// Token: 0x06002BEF RID: 11247 RVA: 0x000D8B40 File Offset: 0x000D6D40
	public void OnJoinedRoom()
	{
		Debug.Log("SCOREBOARD JOIN ROOM");
		if (this.IsCurrentScoreboard())
		{
			this.notInRoomText.SetActive(false);
			this.currentScoreboard = Object.Instantiate<GameObject>(this.scoreboardPrefab, base.transform).GetComponent<GorillaScoreBoard>();
			this.currentScoreboard.transform.rotation = base.transform.rotation;
			if (this.includeMMR)
			{
				this.currentScoreboard.GetComponent<GorillaScoreBoard>().includeMMR = true;
				this.currentScoreboard.GetComponent<Text>().text = "Player                     Color         Level        MMR";
			}
		}
	}

	// Token: 0x06002BF0 RID: 11248 RVA: 0x000D8BD0 File Offset: 0x000D6DD0
	public bool IsVisible()
	{
		if (!this.forOverlay)
		{
			return this.controllingParentGameObject.activeSelf;
		}
		return GTPlayer.Instance.inOverlay;
	}

	// Token: 0x06002BF1 RID: 11249 RVA: 0x000D8BF0 File Offset: 0x000D6DF0
	private IEnumerator UpdateBoard()
	{
		for (;;)
		{
			try
			{
				if (this.currentScoreboard != null)
				{
					bool flag = this.IsVisible();
					foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
					{
						if (flag != gorillaPlayerScoreboardLine.lastVisible)
						{
							gorillaPlayerScoreboardLine.lastVisible = flag;
						}
					}
					if (this.currentScoreboard.boardText.enabled != flag)
					{
						this.currentScoreboard.boardText.enabled = flag;
					}
					if (this.currentScoreboard.buttonText.enabled != flag)
					{
						this.currentScoreboard.buttonText.enabled = flag;
					}
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x06002BF2 RID: 11250 RVA: 0x000D8BFF File Offset: 0x000D6DFF
	public void OnLeftRoom()
	{
		this.Cleanup();
		if (this.notInRoomText)
		{
			this.notInRoomText.SetActive(true);
		}
	}

	// Token: 0x06002BF3 RID: 11251 RVA: 0x000D8C20 File Offset: 0x000D6E20
	public void Cleanup()
	{
		if (this.currentScoreboard != null)
		{
			Object.Destroy(this.currentScoreboard.gameObject);
			this.currentScoreboard = null;
		}
	}

	// Token: 0x0400321D RID: 12829
	public string gameType;

	// Token: 0x0400321E RID: 12830
	public bool includeMMR;

	// Token: 0x0400321F RID: 12831
	public GameObject scoreboardPrefab;

	// Token: 0x04003220 RID: 12832
	public GameObject notInRoomText;

	// Token: 0x04003221 RID: 12833
	public GameObject controllingParentGameObject;

	// Token: 0x04003222 RID: 12834
	public bool isActive = true;

	// Token: 0x04003223 RID: 12835
	public GorillaScoreBoard currentScoreboard;

	// Token: 0x04003224 RID: 12836
	public bool lastVisible;

	// Token: 0x04003225 RID: 12837
	public bool forOverlay;
}
