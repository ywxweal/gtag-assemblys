using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000686 RID: 1670
public class SizeChanger : GorillaTriggerBox
{
	// Token: 0x17000405 RID: 1029
	// (get) Token: 0x060029B9 RID: 10681 RVA: 0x000CEE78 File Offset: 0x000CD078
	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x17000406 RID: 1030
	// (get) Token: 0x060029BA RID: 10682 RVA: 0x000CEEB8 File Offset: 0x000CD0B8
	public SizeChanger.ChangerType MyType
	{
		get
		{
			return this.myType;
		}
	}

	// Token: 0x17000407 RID: 1031
	// (get) Token: 0x060029BB RID: 10683 RVA: 0x000CEEC0 File Offset: 0x000CD0C0
	public float MaxScale
	{
		get
		{
			return this.maxScale;
		}
	}

	// Token: 0x17000408 RID: 1032
	// (get) Token: 0x060029BC RID: 10684 RVA: 0x000CEEC8 File Offset: 0x000CD0C8
	public float MinScale
	{
		get
		{
			return this.minScale;
		}
	}

	// Token: 0x17000409 RID: 1033
	// (get) Token: 0x060029BD RID: 10685 RVA: 0x000CEED0 File Offset: 0x000CD0D0
	public Transform StartPos
	{
		get
		{
			return this.startPos;
		}
	}

	// Token: 0x1700040A RID: 1034
	// (get) Token: 0x060029BE RID: 10686 RVA: 0x000CEED8 File Offset: 0x000CD0D8
	public Transform EndPos
	{
		get
		{
			return this.endPos;
		}
	}

	// Token: 0x1700040B RID: 1035
	// (get) Token: 0x060029BF RID: 10687 RVA: 0x000CEEE0 File Offset: 0x000CD0E0
	public float StaticEasing
	{
		get
		{
			return this.staticEasing;
		}
	}

	// Token: 0x060029C0 RID: 10688 RVA: 0x000CEEE8 File Offset: 0x000CD0E8
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
		this.myCollider = base.GetComponent<Collider>();
	}

	// Token: 0x060029C1 RID: 10689 RVA: 0x000CEF0C File Offset: 0x000CD10C
	public void OnEnable()
	{
		if (this.enterTrigger)
		{
			this.enterTrigger.OnEnter += this.OnTriggerEnter;
		}
		if (this.exitTrigger)
		{
			this.exitTrigger.OnExit += this.OnTriggerExit;
		}
		if (this.exitOnEnterTrigger)
		{
			this.exitOnEnterTrigger.OnEnter += this.OnTriggerExit;
		}
	}

	// Token: 0x060029C2 RID: 10690 RVA: 0x000CEF88 File Offset: 0x000CD188
	public void OnDisable()
	{
		if (this.enterTrigger)
		{
			this.enterTrigger.OnEnter -= this.OnTriggerEnter;
		}
		if (this.exitTrigger)
		{
			this.exitTrigger.OnExit -= this.OnTriggerExit;
		}
		if (this.exitOnEnterTrigger)
		{
			this.exitOnEnterTrigger.OnEnter -= this.OnTriggerExit;
		}
	}

	// Token: 0x060029C3 RID: 10691 RVA: 0x000CF001 File Offset: 0x000CD201
	public void AddEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter += this.OnTriggerEnter;
		}
	}

	// Token: 0x060029C4 RID: 10692 RVA: 0x000CF01D File Offset: 0x000CD21D
	public void RemoveEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter -= this.OnTriggerEnter;
		}
	}

	// Token: 0x060029C5 RID: 10693 RVA: 0x000CF039 File Offset: 0x000CD239
	public void AddExitOnEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter += this.OnTriggerExit;
		}
	}

	// Token: 0x060029C6 RID: 10694 RVA: 0x000CF055 File Offset: 0x000CD255
	public void RemoveExitOnEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter -= this.OnTriggerExit;
		}
	}

	// Token: 0x060029C7 RID: 10695 RVA: 0x000CF074 File Offset: 0x000CD274
	public void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		this.acceptRig(component);
	}

	// Token: 0x060029C8 RID: 10696 RVA: 0x000CF0B1 File Offset: 0x000CD2B1
	public void acceptRig(VRRig rig)
	{
		if (!rig.sizeManager.touchingChangers.Contains(this))
		{
			rig.sizeManager.touchingChangers.Add(this);
		}
		UnityAction onEnter = this.OnEnter;
		if (onEnter == null)
		{
			return;
		}
		onEnter();
	}

	// Token: 0x060029C9 RID: 10697 RVA: 0x000CF0E8 File Offset: 0x000CD2E8
	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		this.unacceptRig(component);
	}

	// Token: 0x060029CA RID: 10698 RVA: 0x000CF125 File Offset: 0x000CD325
	public void unacceptRig(VRRig rig)
	{
		rig.sizeManager.touchingChangers.Remove(this);
		UnityAction onExit = this.OnExit;
		if (onExit == null)
		{
			return;
		}
		onExit();
	}

	// Token: 0x060029CB RID: 10699 RVA: 0x000CF14C File Offset: 0x000CD34C
	public Vector3 ClosestPoint(Vector3 position)
	{
		if (this.enterTrigger && this.exitTrigger)
		{
			Vector3 vector = this.enterTrigger.ClosestPoint(position);
			Vector3 vector2 = this.exitTrigger.ClosestPoint(position);
			if (Vector3.Distance(position, vector) >= Vector3.Distance(position, vector2))
			{
				return vector2;
			}
			return vector;
		}
		else
		{
			if (this.myCollider)
			{
				return this.myCollider.ClosestPoint(position);
			}
			return position;
		}
	}

	// Token: 0x060029CC RID: 10700 RVA: 0x000CF1BC File Offset: 0x000CD3BC
	public void SetScaleCenterPoint(Transform centerPoint)
	{
		this.scaleAwayFromPoint = centerPoint;
	}

	// Token: 0x060029CD RID: 10701 RVA: 0x000CF1C5 File Offset: 0x000CD3C5
	public bool TryGetScaleCenterPoint(out Vector3 centerPoint)
	{
		if (this.scaleAwayFromPoint != null)
		{
			centerPoint = this.scaleAwayFromPoint.position;
			return true;
		}
		centerPoint = Vector3.zero;
		return false;
	}

	// Token: 0x04002ED9 RID: 11993
	[SerializeField]
	private SizeChanger.ChangerType myType;

	// Token: 0x04002EDA RID: 11994
	[SerializeField]
	private float staticEasing;

	// Token: 0x04002EDB RID: 11995
	[SerializeField]
	private float maxScale;

	// Token: 0x04002EDC RID: 11996
	[SerializeField]
	private float minScale;

	// Token: 0x04002EDD RID: 11997
	private Collider myCollider;

	// Token: 0x04002EDE RID: 11998
	[SerializeField]
	private Transform startPos;

	// Token: 0x04002EDF RID: 11999
	[SerializeField]
	private Transform endPos;

	// Token: 0x04002EE0 RID: 12000
	[SerializeField]
	private SizeChangerTrigger enterTrigger;

	// Token: 0x04002EE1 RID: 12001
	[SerializeField]
	private SizeChangerTrigger exitTrigger;

	// Token: 0x04002EE2 RID: 12002
	[SerializeField]
	private Transform scaleAwayFromPoint;

	// Token: 0x04002EE3 RID: 12003
	[SerializeField]
	private SizeChangerTrigger exitOnEnterTrigger;

	// Token: 0x04002EE4 RID: 12004
	public bool alwaysControlWhenEntered;

	// Token: 0x04002EE5 RID: 12005
	public int priority;

	// Token: 0x04002EE6 RID: 12006
	public bool aprilFoolsEnabled;

	// Token: 0x04002EE7 RID: 12007
	public float startRadius;

	// Token: 0x04002EE8 RID: 12008
	public float endRadius;

	// Token: 0x04002EE9 RID: 12009
	public bool affectLayerA = true;

	// Token: 0x04002EEA RID: 12010
	public bool affectLayerB = true;

	// Token: 0x04002EEB RID: 12011
	public bool affectLayerC = true;

	// Token: 0x04002EEC RID: 12012
	public bool affectLayerD = true;

	// Token: 0x04002EED RID: 12013
	public UnityAction OnExit;

	// Token: 0x04002EEE RID: 12014
	public UnityAction OnEnter;

	// Token: 0x04002EEF RID: 12015
	private HashSet<VRRig> unregisteredPresentRigs;

	// Token: 0x02000687 RID: 1671
	public enum ChangerType
	{
		// Token: 0x04002EF1 RID: 12017
		Static,
		// Token: 0x04002EF2 RID: 12018
		Continuous,
		// Token: 0x04002EF3 RID: 12019
		Radius
	}
}
