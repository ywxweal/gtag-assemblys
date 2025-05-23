using System;
using TMPro;
using UnityEngine;

// Token: 0x020005CE RID: 1486
public class GRUIEmployeeBadgeDispenser : MonoBehaviour
{
	// Token: 0x0600243A RID: 9274 RVA: 0x000B6494 File Offset: 0x000B4694
	public void Refresh()
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.actorNr);
		if (player != null && player.InRoom)
		{
			this.playerName.text = player.SanitizedNickName;
			if (this.idBadge != null)
			{
				this.idBadge.playerName.text = player.SanitizedNickName;
				return;
			}
		}
		else
		{
			this.playerName.text = "";
		}
	}

	// Token: 0x0600243B RID: 9275 RVA: 0x000B6504 File Offset: 0x000B4704
	public void CreateBadge(NetPlayer player)
	{
		if (GameEntityManager.instance.IsAuthority())
		{
			GameEntityManager.instance.RequestCreateItem(this.idBadgePrefab.name.GetStaticHash(), this.spawnLocation.position, this.spawnLocation.rotation, (long)(player.ActorNumber * 100 + this.index));
		}
	}

	// Token: 0x0600243C RID: 9276 RVA: 0x000B6562 File Offset: 0x000B4762
	public Transform GetSpawnMarker()
	{
		return this.spawnLocation;
	}

	// Token: 0x0600243D RID: 9277 RVA: 0x000B656A File Offset: 0x000B476A
	public bool IsDispenserForBadge(GRBadge badge)
	{
		return badge == this.idBadge;
	}

	// Token: 0x0600243E RID: 9278 RVA: 0x000B6578 File Offset: 0x000B4778
	public Vector3 GetSpawnPosition()
	{
		return this.spawnLocation.position;
	}

	// Token: 0x0600243F RID: 9279 RVA: 0x000B6585 File Offset: 0x000B4785
	public Quaternion GetSpawnRotation()
	{
		return this.spawnLocation.rotation;
	}

	// Token: 0x06002440 RID: 9280 RVA: 0x000B6592 File Offset: 0x000B4792
	public void ClearBadge()
	{
		this.actorNr = -1;
		this.idBadge = null;
	}

	// Token: 0x06002441 RID: 9281 RVA: 0x000B65A4 File Offset: 0x000B47A4
	public void AttachIDBadge(GRBadge linkedBadge, NetPlayer _player)
	{
		this.actorNr = ((_player == null) ? (-1) : _player.ActorNumber);
		this.idBadge = linkedBadge;
		this.playerName.text = ((_player == null) ? null : _player.SanitizedNickName);
		this.idBadge.Setup(_player, this.index);
	}

	// Token: 0x04002953 RID: 10579
	[SerializeField]
	private TMP_Text msg;

	// Token: 0x04002954 RID: 10580
	[SerializeField]
	private TMP_Text playerName;

	// Token: 0x04002955 RID: 10581
	[SerializeField]
	private Transform spawnLocation;

	// Token: 0x04002956 RID: 10582
	[SerializeField]
	private GameEntity idBadgePrefab;

	// Token: 0x04002957 RID: 10583
	[SerializeField]
	private LayerMask badgeLayerMask;

	// Token: 0x04002958 RID: 10584
	public int index;

	// Token: 0x04002959 RID: 10585
	public int actorNr;

	// Token: 0x0400295A RID: 10586
	public GRBadge idBadge;

	// Token: 0x0400295B RID: 10587
	private Coroutine getSpawnedBadgeCoroutine;

	// Token: 0x0400295C RID: 10588
	private static Collider[] overlapColliders = new Collider[10];

	// Token: 0x0400295D RID: 10589
	private bool isEmployee;

	// Token: 0x0400295E RID: 10590
	private const string GR_DATA_KEY = "GRData";
}
