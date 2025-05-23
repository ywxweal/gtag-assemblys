using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020003A9 RID: 937
public class SlingshotLifeIndicator : MonoBehaviour, IGorillaSliceableSimple, ISpawnable
{
	// Token: 0x17000268 RID: 616
	// (get) Token: 0x060015E3 RID: 5603 RVA: 0x0006AAE8 File Offset: 0x00068CE8
	// (set) Token: 0x060015E4 RID: 5604 RVA: 0x0006AAF0 File Offset: 0x00068CF0
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000269 RID: 617
	// (get) Token: 0x060015E5 RID: 5605 RVA: 0x0006AAF9 File Offset: 0x00068CF9
	// (set) Token: 0x060015E6 RID: 5606 RVA: 0x0006AB01 File Offset: 0x00068D01
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060015E7 RID: 5607 RVA: 0x0006AB0A File Offset: 0x00068D0A
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x060015E8 RID: 5608 RVA: 0x000023F4 File Offset: 0x000005F4
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060015E9 RID: 5609 RVA: 0x0006AB13 File Offset: 0x00068D13
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(this.OnLeftRoom));
	}

	// Token: 0x060015EA RID: 5610 RVA: 0x0006AB3C File Offset: 0x00068D3C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.Reset();
		RoomSystem.LeftRoomEvent = (Action)Delegate.Remove(RoomSystem.LeftRoomEvent, new Action(this.OnLeftRoom));
	}

	// Token: 0x060015EB RID: 5611 RVA: 0x0006AB6B File Offset: 0x00068D6B
	private void SetActive(GameObject obj, bool active)
	{
		if (!obj.activeSelf && active)
		{
			obj.SetActive(true);
		}
		if (obj.activeSelf && !active)
		{
			obj.SetActive(false);
		}
	}

	// Token: 0x060015EC RID: 5612 RVA: 0x0006AB94 File Offset: 0x00068D94
	public void SliceUpdate()
	{
		if (!NetworkSystem.Instance.InRoom || (this.checkedBattle && !this.inBattle))
		{
			if (this.indicator1.activeSelf)
			{
				this.indicator1.SetActive(false);
			}
			if (this.indicator2.activeSelf)
			{
				this.indicator2.SetActive(false);
			}
			if (this.indicator3.activeSelf)
			{
				this.indicator3.SetActive(false);
			}
			return;
		}
		if (this.bMgr == null)
		{
			this.checkedBattle = true;
			this.inBattle = true;
			if (GorillaGameManager.instance == null)
			{
				return;
			}
			this.bMgr = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
			if (this.bMgr == null)
			{
				this.inBattle = false;
				return;
			}
		}
		VRRig vrrig = this.myRig;
		if (((vrrig != null) ? vrrig.creator : null) == null)
		{
			return;
		}
		int playerLives = this.bMgr.GetPlayerLives(this.myRig.creator);
		this.SetActive(this.indicator1, playerLives >= 1);
		this.SetActive(this.indicator2, playerLives >= 2);
		this.SetActive(this.indicator3, playerLives >= 3);
	}

	// Token: 0x060015ED RID: 5613 RVA: 0x0006ACC9 File Offset: 0x00068EC9
	public void OnLeftRoom()
	{
		this.Reset();
	}

	// Token: 0x060015EE RID: 5614 RVA: 0x0006ACD1 File Offset: 0x00068ED1
	public void Reset()
	{
		this.bMgr = null;
		this.inBattle = false;
		this.checkedBattle = false;
	}

	// Token: 0x060015F0 RID: 5616 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x0400186B RID: 6251
	private VRRig myRig;

	// Token: 0x0400186C RID: 6252
	public GorillaPaintbrawlManager bMgr;

	// Token: 0x0400186D RID: 6253
	public bool checkedBattle;

	// Token: 0x0400186E RID: 6254
	public bool inBattle;

	// Token: 0x0400186F RID: 6255
	public GameObject indicator1;

	// Token: 0x04001870 RID: 6256
	public GameObject indicator2;

	// Token: 0x04001871 RID: 6257
	public GameObject indicator3;
}
