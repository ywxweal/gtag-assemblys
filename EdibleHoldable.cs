using System;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000400 RID: 1024
public class EdibleHoldable : TransferrableObject
{
	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x060018C9 RID: 6345 RVA: 0x00078352 File Offset: 0x00076552
	// (set) Token: 0x060018CA RID: 6346 RVA: 0x0007835A File Offset: 0x0007655A
	public int lastBiterActorID { get; private set; } = -1;

	// Token: 0x060018CB RID: 6347 RVA: 0x00078363 File Offset: 0x00076563
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.previousEdibleState = (EdibleHoldable.EdibleHoldableStates)this.itemState;
		this.lastFullyEatenTime = -this.respawnTime;
		this.iResettableItems = base.GetComponentsInChildren<IResettableItem>(true);
	}

	// Token: 0x060018CC RID: 6348 RVA: 0x00078398 File Offset: 0x00076598
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.lastEatTime = Time.time - this.eatMinimumCooldown;
	}

	// Token: 0x060018CD RID: 6349 RVA: 0x000783B4 File Offset: 0x000765B4
	public override void OnActivate()
	{
		base.OnActivate();
	}

	// Token: 0x060018CE RID: 6350 RVA: 0x000783BC File Offset: 0x000765BC
	internal override void OnEnable()
	{
		base.OnEnable();
	}

	// Token: 0x060018CF RID: 6351 RVA: 0x00022B8F File Offset: 0x00020D8F
	internal override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x060018D0 RID: 6352 RVA: 0x000783C4 File Offset: 0x000765C4
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
	}

	// Token: 0x060018D1 RID: 6353 RVA: 0x000783CC File Offset: 0x000765CC
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return base.OnRelease(zoneReleased, releasingHand) && !base.InHand();
	}

	// Token: 0x060018D2 RID: 6354 RVA: 0x000783E8 File Offset: 0x000765E8
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.itemState == TransferrableObject.ItemStates.State3)
		{
			if (Time.time > this.lastFullyEatenTime + this.respawnTime)
			{
				this.itemState = TransferrableObject.ItemStates.State0;
				return;
			}
		}
		else if (Time.time > this.lastEatTime + this.eatMinimumCooldown)
		{
			bool flag = false;
			bool flag2 = false;
			float num = this.biteDistance * this.biteDistance;
			if (!GorillaParent.hasInstance)
			{
				return;
			}
			VRRig vrrig = null;
			VRRig vrrig2 = null;
			for (int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
			{
				VRRig vrrig3 = GorillaParent.instance.vrrigs[i];
				if (!vrrig3.isOfflineVRRig)
				{
					if (vrrig3.head == null || vrrig3.head.rigTarget == null)
					{
						break;
					}
					Transform transform = vrrig3.head.rigTarget.transform;
					if ((transform.position + transform.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude < num)
					{
						flag = true;
						vrrig2 = vrrig3;
					}
				}
			}
			Transform transform2 = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform;
			if ((transform2.position + transform2.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude < num)
			{
				flag = true;
				flag2 = true;
				vrrig = GorillaTagger.Instance.offlineVRRig;
			}
			if (flag && !this.inBiteZone && (!flag2 || base.InHand()) && this.itemState != TransferrableObject.ItemStates.State3)
			{
				if (this.itemState == TransferrableObject.ItemStates.State0)
				{
					this.itemState = TransferrableObject.ItemStates.State1;
				}
				else if (this.itemState == TransferrableObject.ItemStates.State1)
				{
					this.itemState = TransferrableObject.ItemStates.State2;
				}
				else if (this.itemState == TransferrableObject.ItemStates.State2)
				{
					this.itemState = TransferrableObject.ItemStates.State3;
				}
				this.lastEatTime = Time.time;
				this.lastFullyEatenTime = Time.time;
			}
			if (flag)
			{
				if (flag2)
				{
					int num2;
					if (!vrrig)
					{
						num2 = -1;
					}
					else
					{
						NetPlayer owningNetPlayer = vrrig.OwningNetPlayer;
						num2 = ((owningNetPlayer != null) ? owningNetPlayer.ActorNumber : (-1));
					}
					this.lastBiterActorID = num2;
					EdibleHoldable.BiteEvent biteEvent = this.onBiteView;
					if (biteEvent != null)
					{
						biteEvent.Invoke(vrrig, (int)this.itemState);
					}
				}
				else
				{
					int num3;
					if (!vrrig2)
					{
						num3 = -1;
					}
					else
					{
						NetPlayer owningNetPlayer2 = vrrig2.OwningNetPlayer;
						num3 = ((owningNetPlayer2 != null) ? owningNetPlayer2.ActorNumber : (-1));
					}
					this.lastBiterActorID = num3;
					EdibleHoldable.BiteEvent biteEvent2 = this.onBiteWorld;
					if (biteEvent2 != null)
					{
						biteEvent2.Invoke(vrrig2, (int)this.itemState);
					}
				}
			}
			this.inBiteZone = flag;
		}
	}

	// Token: 0x060018D3 RID: 6355 RVA: 0x00078668 File Offset: 0x00076868
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		EdibleHoldable.EdibleHoldableStates itemState = (EdibleHoldable.EdibleHoldableStates)this.itemState;
		if (itemState != this.previousEdibleState)
		{
			this.OnEdibleHoldableStateChange();
		}
		this.previousEdibleState = itemState;
	}

	// Token: 0x060018D4 RID: 6356 RVA: 0x00078698 File Offset: 0x00076898
	protected virtual void OnEdibleHoldableStateChange()
	{
		float num = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float num2 = 0.08f;
		int num3 = 0;
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			num3 = 0;
			if (this.iResettableItems != null)
			{
				foreach (IResettableItem resettableItem in this.iResettableItems)
				{
					if (resettableItem != null)
					{
						resettableItem.ResetToDefaultState();
					}
				}
			}
		}
		else if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			num3 = 1;
		}
		else if (this.itemState == TransferrableObject.ItemStates.State2)
		{
			num3 = 2;
		}
		else if (this.itemState == TransferrableObject.ItemStates.State3)
		{
			num3 = 3;
		}
		int num4 = num3 - 1;
		if (num4 < 0)
		{
			num4 = this.edibleMeshObjects.Length - 1;
		}
		this.edibleMeshObjects[num4].SetActive(false);
		this.edibleMeshObjects[num3].SetActive(true);
		if ((this.itemState != TransferrableObject.ItemStates.State0 && this.onBiteView != null) || this.onBiteWorld != null)
		{
			VRRig vrrig = null;
			float num5 = float.PositiveInfinity;
			for (int j = 0; j < GorillaParent.instance.vrrigs.Count; j++)
			{
				VRRig vrrig2 = GorillaParent.instance.vrrigs[j];
				if (vrrig2.head == null || vrrig2.head.rigTarget == null)
				{
					break;
				}
				Transform transform = vrrig2.head.rigTarget.transform;
				float sqrMagnitude = (transform.position + transform.rotation * this.biteOffset - this.biteSpot.position).sqrMagnitude;
				if (sqrMagnitude < num5)
				{
					num5 = sqrMagnitude;
					vrrig = vrrig2;
				}
			}
			if (vrrig != null)
			{
				EdibleHoldable.BiteEvent biteEvent = (vrrig.isOfflineVRRig ? this.onBiteView : this.onBiteWorld);
				if (biteEvent != null)
				{
					biteEvent.Invoke(vrrig, (int)this.itemState);
				}
				if (vrrig.isOfflineVRRig && this.itemState != TransferrableObject.ItemStates.State0)
				{
					PlayerGameEvents.EatObject(this.interactEventName);
				}
			}
		}
		this.eatSoundSource.GTPlayOneShot(this.eatSounds[num3], num2);
		if (this.IsMyItem())
		{
			if (base.InHand())
			{
				GorillaTagger.Instance.StartVibration(base.InLeftHand(), num, fixedDeltaTime);
				return;
			}
			GorillaTagger.Instance.StartVibration(false, num, fixedDeltaTime);
			GorillaTagger.Instance.StartVibration(true, num, fixedDeltaTime);
		}
	}

	// Token: 0x060018D5 RID: 6357 RVA: 0x00047642 File Offset: 0x00045842
	public override bool CanActivate()
	{
		return true;
	}

	// Token: 0x060018D6 RID: 6358 RVA: 0x00047642 File Offset: 0x00045842
	public override bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x04001B9D RID: 7069
	public AudioClip[] eatSounds;

	// Token: 0x04001B9E RID: 7070
	public GameObject[] edibleMeshObjects;

	// Token: 0x04001BA0 RID: 7072
	public EdibleHoldable.BiteEvent onBiteView;

	// Token: 0x04001BA1 RID: 7073
	public EdibleHoldable.BiteEvent onBiteWorld;

	// Token: 0x04001BA2 RID: 7074
	[DebugReadout]
	public float lastEatTime;

	// Token: 0x04001BA3 RID: 7075
	[DebugReadout]
	public float lastFullyEatenTime;

	// Token: 0x04001BA4 RID: 7076
	public float eatMinimumCooldown = 1f;

	// Token: 0x04001BA5 RID: 7077
	public float respawnTime = 7f;

	// Token: 0x04001BA6 RID: 7078
	public float biteDistance = 0.1666667f;

	// Token: 0x04001BA7 RID: 7079
	public Vector3 biteOffset = new Vector3(0f, 0.0208f, 0.171f);

	// Token: 0x04001BA8 RID: 7080
	public Transform biteSpot;

	// Token: 0x04001BA9 RID: 7081
	public bool inBiteZone;

	// Token: 0x04001BAA RID: 7082
	public AudioSource eatSoundSource;

	// Token: 0x04001BAB RID: 7083
	private EdibleHoldable.EdibleHoldableStates previousEdibleState;

	// Token: 0x04001BAC RID: 7084
	private IResettableItem[] iResettableItems;

	// Token: 0x02000401 RID: 1025
	private enum EdibleHoldableStates
	{
		// Token: 0x04001BAE RID: 7086
		EatingState0 = 1,
		// Token: 0x04001BAF RID: 7087
		EatingState1,
		// Token: 0x04001BB0 RID: 7088
		EatingState2 = 4,
		// Token: 0x04001BB1 RID: 7089
		EatingState3 = 8
	}

	// Token: 0x02000402 RID: 1026
	[Serializable]
	public class BiteEvent : UnityEvent<VRRig, int>
	{
	}
}
