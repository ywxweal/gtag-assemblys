using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020005CD RID: 1485
public class GRUIBuyItem : MonoBehaviour
{
	// Token: 0x06002436 RID: 9270 RVA: 0x000B641B File Offset: 0x000B461B
	public void Setup(int standId)
	{
		this.standId = standId;
		this.buyItemButton.onPressButton.AddListener(new UnityAction(this.OnBuyItem));
		this.entityTypeId = this.entityPrefab.gameObject.name.GetStaticHash();
	}

	// Token: 0x06002437 RID: 9271 RVA: 0x000B645C File Offset: 0x000B465C
	public void OnBuyItem()
	{
		int itemCost = GhostReactor.instance.GetItemCost(this.entityTypeId);
		int currency = VRRig.LocalRig.GetComponent<GRPlayer>().currency;
	}

	// Token: 0x06002438 RID: 9272 RVA: 0x000B648C File Offset: 0x000B468C
	public Transform GetSpawnMarker()
	{
		return this.spawnMarker;
	}

	// Token: 0x0400294D RID: 10573
	[SerializeField]
	private GorillaPressableButton buyItemButton;

	// Token: 0x0400294E RID: 10574
	[SerializeField]
	private Text itemInfoLabel;

	// Token: 0x0400294F RID: 10575
	[SerializeField]
	private Transform spawnMarker;

	// Token: 0x04002950 RID: 10576
	[SerializeField]
	private GameEntity entityPrefab;

	// Token: 0x04002951 RID: 10577
	private int entityTypeId;

	// Token: 0x04002952 RID: 10578
	private int standId;
}
