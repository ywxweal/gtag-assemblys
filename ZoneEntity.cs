using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000A37 RID: 2615
public class ZoneEntity : MonoBehaviour
{
	// Token: 0x17000614 RID: 1556
	// (get) Token: 0x06003E16 RID: 15894 RVA: 0x00127104 File Offset: 0x00125304
	public string entityTag
	{
		get
		{
			return this._entityTag;
		}
	}

	// Token: 0x17000615 RID: 1557
	// (get) Token: 0x06003E17 RID: 15895 RVA: 0x0012710C File Offset: 0x0012530C
	public int entityID
	{
		get
		{
			int num = this._entityID.GetValueOrDefault();
			if (this._entityID == null)
			{
				num = base.GetInstanceID();
				this._entityID = new int?(num);
			}
			return this._entityID.Value;
		}
	}

	// Token: 0x17000616 RID: 1558
	// (get) Token: 0x06003E18 RID: 15896 RVA: 0x00127150 File Offset: 0x00125350
	public VRRig entityRig
	{
		get
		{
			return this._entityRig;
		}
	}

	// Token: 0x17000617 RID: 1559
	// (get) Token: 0x06003E19 RID: 15897 RVA: 0x00127158 File Offset: 0x00125358
	public SphereCollider collider
	{
		get
		{
			return this._collider;
		}
	}

	// Token: 0x17000618 RID: 1560
	// (get) Token: 0x06003E1A RID: 15898 RVA: 0x00127160 File Offset: 0x00125360
	public GroupJoinZoneAB GroupZone
	{
		get
		{
			return (this.currentGroupZone & ~this.currentExcludeGroupZone) | this.previousGroupZone;
		}
	}

	// Token: 0x06003E1B RID: 15899 RVA: 0x00127183 File Offset: 0x00125383
	protected virtual void OnEnable()
	{
		this.insideBoxes.Clear();
		ZoneGraph.Register(this);
	}

	// Token: 0x06003E1C RID: 15900 RVA: 0x00127196 File Offset: 0x00125396
	protected virtual void OnDisable()
	{
		this.insideBoxes.Clear();
		ZoneGraph.Unregister(this);
	}

	// Token: 0x06003E1D RID: 15901 RVA: 0x001271A9 File Offset: 0x001253A9
	public void EnableZoneChanges()
	{
		this._collider.enabled = true;
		if (this.disabledZoneChangesOnTriggerStayCoroutine != null)
		{
			base.StopCoroutine(this.disabledZoneChangesOnTriggerStayCoroutine);
			this.disabledZoneChangesOnTriggerStayCoroutine = null;
		}
	}

	// Token: 0x06003E1E RID: 15902 RVA: 0x001271D2 File Offset: 0x001253D2
	public void DisableZoneChanges()
	{
		this._collider.enabled = false;
		if (this.insideBoxes.Count > 0 && this.disabledZoneChangesOnTriggerStayCoroutine == null)
		{
			this.disabledZoneChangesOnTriggerStayCoroutine = base.StartCoroutine(this.DisabledZoneCollider_OnTriggerStay());
		}
	}

	// Token: 0x06003E1F RID: 15903 RVA: 0x00127208 File Offset: 0x00125408
	private IEnumerator DisabledZoneCollider_OnTriggerStay()
	{
		for (;;)
		{
			foreach (BoxCollider boxCollider in this.insideBoxes)
			{
				this.OnTriggerStay(boxCollider);
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06003E20 RID: 15904 RVA: 0x00127217 File Offset: 0x00125417
	protected virtual void OnTriggerEnter(Collider c)
	{
		this.OnZoneTrigger(GTZoneEventType.zone_enter, c);
	}

	// Token: 0x06003E21 RID: 15905 RVA: 0x00127221 File Offset: 0x00125421
	protected virtual void OnTriggerExit(Collider c)
	{
		this.OnZoneTrigger(GTZoneEventType.zone_exit, c);
	}

	// Token: 0x06003E22 RID: 15906 RVA: 0x0012722C File Offset: 0x0012542C
	protected virtual void OnTriggerStay(Collider c)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		BoxCollider boxCollider = c as BoxCollider;
		if (boxCollider == null)
		{
			return;
		}
		ZoneDef zoneDef = ZoneGraph.ColliderToZoneDef(boxCollider);
		if (Time.time >= this.groupZoneClearAtTimestamp)
		{
			this.previousGroupZone = this.currentGroupZone & ~this.currentExcludeGroupZone;
			this.currentGroupZone = zoneDef.groupZoneAB;
			this.currentExcludeGroupZone = zoneDef.excludeGroupZoneAB;
			this.groupZoneClearAtTimestamp = Time.time + this.groupZoneClearInterval;
		}
		else
		{
			this.currentGroupZone |= zoneDef.groupZoneAB;
			this.currentExcludeGroupZone |= zoneDef.excludeGroupZoneAB;
		}
		if (!this.gLastStayPoll.HasElapsed(1f, true))
		{
			return;
		}
		this.OnZoneTrigger(GTZoneEventType.zone_stay, boxCollider);
	}

	// Token: 0x06003E23 RID: 15907 RVA: 0x001272F4 File Offset: 0x001254F4
	protected virtual void OnZoneTrigger(GTZoneEventType zoneEvent, Collider c)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		BoxCollider boxCollider = c as BoxCollider;
		if (boxCollider == null)
		{
			return;
		}
		ZoneDef zoneDef = ZoneGraph.ColliderToZoneDef(boxCollider);
		this.OnZoneTrigger(zoneEvent, zoneDef, boxCollider);
	}

	// Token: 0x06003E24 RID: 15908 RVA: 0x00127324 File Offset: 0x00125524
	private void OnZoneTrigger(GTZoneEventType zoneEvent, ZoneDef zone, BoxCollider box)
	{
		bool flag = false;
		switch (zoneEvent)
		{
		case GTZoneEventType.zone_enter:
		{
			if (zone.zoneId != this.lastEnteredNode.zoneId)
			{
				this.sinceZoneEntered = 0;
			}
			this.lastEnteredNode = ZoneGraph.ColliderToNode(box);
			ZoneDef zoneDef = ZoneGraph.ColliderToZoneDef(box);
			this.insideBoxes.Add(box);
			if (zoneDef.priority > this.currentZonePriority)
			{
				this.currentZone = zone.zoneId;
				this.currentSubZone = zone.subZoneId;
				this.currentZonePriority = zoneDef.priority;
			}
			if (zone.subZoneId == GTSubZone.store_register)
			{
				GorillaTelemetry.PostShopEvent(this._entityRig, GTShopEventType.register_visit, CosmeticsController.instance.currentCart);
			}
			flag = zone.trackEnter;
			break;
		}
		case GTZoneEventType.zone_exit:
			this.lastExitedNode = ZoneGraph.ColliderToNode(box);
			this.insideBoxes.Remove(box);
			if (this.currentZone == this.lastExitedNode.zoneId)
			{
				int num = 0;
				ZoneDef zoneDef2 = null;
				foreach (BoxCollider boxCollider in this.insideBoxes)
				{
					ZoneDef zoneDef3 = ZoneGraph.ColliderToZoneDef(boxCollider);
					if (zoneDef3.priority > num)
					{
						zoneDef2 = zoneDef3;
						num = zoneDef3.priority;
					}
				}
				if (zoneDef2 != null)
				{
					this.currentZone = zoneDef2.zoneId;
					this.currentSubZone = zoneDef2.subZoneId;
					this.currentZonePriority = zoneDef2.priority;
				}
				else
				{
					this.currentZone = GTZone.none;
					this.currentSubZone = GTSubZone.none;
					this.currentZonePriority = 0;
				}
			}
			if (this.currentZone == GTZone.forest && this.currentSubZone == GTSubZone.tree_room)
			{
				zone.subZoneId = GTSubZone.none;
			}
			flag = zone.trackExit;
			break;
		case GTZoneEventType.zone_stay:
		{
			bool flag2 = this.sinceZoneEntered.secondsElapsedInt >= this._zoneStayEventInterval;
			if (flag2)
			{
				this.sinceZoneEntered = 0;
			}
			flag = zone.trackStay && flag2;
			break;
		}
		}
		GorillaTelemetry.CurrentZone = zone.zoneId;
		GorillaTelemetry.CurrentSubZone = zone.subZoneId;
		if (!this._emitTelemetry)
		{
			return;
		}
		if (!flag)
		{
			return;
		}
		if (!this._entityRig.isOfflineVRRig)
		{
			return;
		}
		GorillaTelemetry.PostZoneEvent(zone.zoneId, zone.subZoneId, zoneEvent);
	}

	// Token: 0x06003E25 RID: 15909 RVA: 0x00127554 File Offset: 0x00125754
	public static int Compare<T>(T x, T y) where T : ZoneEntity
	{
		if (x == null && y == null)
		{
			return 0;
		}
		if (x == null)
		{
			return 1;
		}
		if (y == null)
		{
			return -1;
		}
		return x.entityID.CompareTo(y.entityID);
	}

	// Token: 0x040042B6 RID: 17078
	[Space]
	[NonSerialized]
	private int? _entityID;

	// Token: 0x040042B7 RID: 17079
	[SerializeField]
	private string _entityTag;

	// Token: 0x040042B8 RID: 17080
	[Space]
	[SerializeField]
	private bool _emitTelemetry = true;

	// Token: 0x040042B9 RID: 17081
	[SerializeField]
	private int _zoneStayEventInterval = 300;

	// Token: 0x040042BA RID: 17082
	[Space]
	[SerializeField]
	private VRRig _entityRig;

	// Token: 0x040042BB RID: 17083
	[SerializeField]
	private SphereCollider _collider;

	// Token: 0x040042BC RID: 17084
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x040042BD RID: 17085
	[Space]
	[NonSerialized]
	public GTZone currentZone = GTZone.none;

	// Token: 0x040042BE RID: 17086
	[NonSerialized]
	public GTSubZone currentSubZone;

	// Token: 0x040042BF RID: 17087
	[NonSerialized]
	private GroupJoinZoneAB currentGroupZone = 0;

	// Token: 0x040042C0 RID: 17088
	[NonSerialized]
	private GroupJoinZoneAB previousGroupZone = 0;

	// Token: 0x040042C1 RID: 17089
	[NonSerialized]
	private GroupJoinZoneAB currentExcludeGroupZone = 0;

	// Token: 0x040042C2 RID: 17090
	private HashSet<BoxCollider> insideBoxes = new HashSet<BoxCollider>();

	// Token: 0x040042C3 RID: 17091
	private int currentZonePriority;

	// Token: 0x040042C4 RID: 17092
	private float groupZoneClearAtTimestamp;

	// Token: 0x040042C5 RID: 17093
	private float groupZoneClearInterval = 0.1f;

	// Token: 0x040042C6 RID: 17094
	private Coroutine disabledZoneChangesOnTriggerStayCoroutine;

	// Token: 0x040042C7 RID: 17095
	[Space]
	[NonSerialized]
	public ZoneNode currentNode = ZoneNode.Null;

	// Token: 0x040042C8 RID: 17096
	[NonSerialized]
	public ZoneNode lastEnteredNode = ZoneNode.Null;

	// Token: 0x040042C9 RID: 17097
	[NonSerialized]
	public ZoneNode lastExitedNode = ZoneNode.Null;

	// Token: 0x040042CA RID: 17098
	[Space]
	[NonSerialized]
	private TimeSince sinceZoneEntered = 0;

	// Token: 0x040042CB RID: 17099
	private TimeSince gLastStayPoll = 0;
}
