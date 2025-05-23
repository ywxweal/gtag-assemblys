using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000A14 RID: 2580
[RequireComponent(typeof(NetworkView))]
public class ThrowableSetDressing : TransferrableObject
{
	// Token: 0x17000605 RID: 1541
	// (get) Token: 0x06003DA0 RID: 15776 RVA: 0x0012443F File Offset: 0x0012263F
	// (set) Token: 0x06003DA1 RID: 15777 RVA: 0x00124447 File Offset: 0x00122647
	public bool inInitialPose { get; private set; } = true;

	// Token: 0x06003DA2 RID: 15778 RVA: 0x00124450 File Offset: 0x00122650
	public override bool ShouldBeKinematic()
	{
		return this.inInitialPose || base.ShouldBeKinematic();
	}

	// Token: 0x06003DA3 RID: 15779 RVA: 0x00124462 File Offset: 0x00122662
	protected override void Awake()
	{
		base.Awake();
		this.netView = base.GetComponent<NetworkView>();
	}

	// Token: 0x06003DA4 RID: 15780 RVA: 0x00124476 File Offset: 0x00122676
	protected override void Start()
	{
		base.Start();
		this.respawnAtPos = base.transform.position;
		this.respawnAtRot = base.transform.rotation;
		this.currentState = TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06003DA5 RID: 15781 RVA: 0x001244AB File Offset: 0x001226AB
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.inInitialPose = false;
		this.StopRespawnTimer();
	}

	// Token: 0x06003DA6 RID: 15782 RVA: 0x001244C2 File Offset: 0x001226C2
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.StartRespawnTimer(-1f);
		return true;
	}

	// Token: 0x06003DA7 RID: 15783 RVA: 0x001244DC File Offset: 0x001226DC
	public override void DropItem()
	{
		base.DropItem();
		this.StartRespawnTimer(-1f);
	}

	// Token: 0x06003DA8 RID: 15784 RVA: 0x001244EF File Offset: 0x001226EF
	private void StopRespawnTimer()
	{
		if (this.respawnTimer != null)
		{
			base.StopCoroutine(this.respawnTimer);
			this.respawnTimer = null;
		}
	}

	// Token: 0x06003DA9 RID: 15785 RVA: 0x0012450C File Offset: 0x0012270C
	public void SetWillTeleport()
	{
		this.worldShareableInstance.SetWillTeleport();
	}

	// Token: 0x06003DAA RID: 15786 RVA: 0x0012451C File Offset: 0x0012271C
	public void StartRespawnTimer(float overrideTimer = -1f)
	{
		float num = ((overrideTimer != -1f) ? overrideTimer : this.respawnTimerDuration);
		this.StopRespawnTimer();
		if (this.respawnTimerDuration != 0f && (!this.netView.IsValid || this.netView.IsMine))
		{
			this.respawnTimer = base.StartCoroutine(this.RespawnTimerCoroutine(num));
		}
	}

	// Token: 0x06003DAB RID: 15787 RVA: 0x0012457B File Offset: 0x0012277B
	private IEnumerator RespawnTimerCoroutine(float timerDuration)
	{
		yield return new WaitForSeconds(timerDuration);
		if (base.InHand())
		{
			yield break;
		}
		this.SetWillTeleport();
		base.transform.position = this.respawnAtPos;
		base.transform.rotation = this.respawnAtRot;
		this.inInitialPose = true;
		this.rigidbodyInstance.isKinematic = true;
		yield break;
	}

	// Token: 0x0400416C RID: 16748
	public float respawnTimerDuration;

	// Token: 0x0400416E RID: 16750
	[Tooltip("set this only if this set dressing is using as an ingredient for the magic cauldron - Halloween")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x0400416F RID: 16751
	private float _respawnTimestamp;

	// Token: 0x04004170 RID: 16752
	[SerializeField]
	private CapsuleCollider capsuleCollider;

	// Token: 0x04004171 RID: 16753
	private NetworkView netView;

	// Token: 0x04004172 RID: 16754
	private Vector3 respawnAtPos;

	// Token: 0x04004173 RID: 16755
	private Quaternion respawnAtRot;

	// Token: 0x04004174 RID: 16756
	private Coroutine respawnTimer;
}
