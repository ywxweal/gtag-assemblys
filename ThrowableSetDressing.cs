using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000A14 RID: 2580
[RequireComponent(typeof(NetworkView))]
public class ThrowableSetDressing : TransferrableObject
{
	// Token: 0x17000605 RID: 1541
	// (get) Token: 0x06003D9F RID: 15775 RVA: 0x00124367 File Offset: 0x00122567
	// (set) Token: 0x06003DA0 RID: 15776 RVA: 0x0012436F File Offset: 0x0012256F
	public bool inInitialPose { get; private set; } = true;

	// Token: 0x06003DA1 RID: 15777 RVA: 0x00124378 File Offset: 0x00122578
	public override bool ShouldBeKinematic()
	{
		return this.inInitialPose || base.ShouldBeKinematic();
	}

	// Token: 0x06003DA2 RID: 15778 RVA: 0x0012438A File Offset: 0x0012258A
	protected override void Awake()
	{
		base.Awake();
		this.netView = base.GetComponent<NetworkView>();
	}

	// Token: 0x06003DA3 RID: 15779 RVA: 0x0012439E File Offset: 0x0012259E
	protected override void Start()
	{
		base.Start();
		this.respawnAtPos = base.transform.position;
		this.respawnAtRot = base.transform.rotation;
		this.currentState = TransferrableObject.PositionState.Dropped;
	}

	// Token: 0x06003DA4 RID: 15780 RVA: 0x001243D3 File Offset: 0x001225D3
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		this.inInitialPose = false;
		this.StopRespawnTimer();
	}

	// Token: 0x06003DA5 RID: 15781 RVA: 0x001243EA File Offset: 0x001225EA
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.StartRespawnTimer(-1f);
		return true;
	}

	// Token: 0x06003DA6 RID: 15782 RVA: 0x00124404 File Offset: 0x00122604
	public override void DropItem()
	{
		base.DropItem();
		this.StartRespawnTimer(-1f);
	}

	// Token: 0x06003DA7 RID: 15783 RVA: 0x00124417 File Offset: 0x00122617
	private void StopRespawnTimer()
	{
		if (this.respawnTimer != null)
		{
			base.StopCoroutine(this.respawnTimer);
			this.respawnTimer = null;
		}
	}

	// Token: 0x06003DA8 RID: 15784 RVA: 0x00124434 File Offset: 0x00122634
	public void SetWillTeleport()
	{
		this.worldShareableInstance.SetWillTeleport();
	}

	// Token: 0x06003DA9 RID: 15785 RVA: 0x00124444 File Offset: 0x00122644
	public void StartRespawnTimer(float overrideTimer = -1f)
	{
		float num = ((overrideTimer != -1f) ? overrideTimer : this.respawnTimerDuration);
		this.StopRespawnTimer();
		if (this.respawnTimerDuration != 0f && (!this.netView.IsValid || this.netView.IsMine))
		{
			this.respawnTimer = base.StartCoroutine(this.RespawnTimerCoroutine(num));
		}
	}

	// Token: 0x06003DAA RID: 15786 RVA: 0x001244A3 File Offset: 0x001226A3
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

	// Token: 0x0400416B RID: 16747
	public float respawnTimerDuration;

	// Token: 0x0400416D RID: 16749
	[Tooltip("set this only if this set dressing is using as an ingredient for the magic cauldron - Halloween")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x0400416E RID: 16750
	private float _respawnTimestamp;

	// Token: 0x0400416F RID: 16751
	[SerializeField]
	private CapsuleCollider capsuleCollider;

	// Token: 0x04004170 RID: 16752
	private NetworkView netView;

	// Token: 0x04004171 RID: 16753
	private Vector3 respawnAtPos;

	// Token: 0x04004172 RID: 16754
	private Quaternion respawnAtRot;

	// Token: 0x04004173 RID: 16755
	private Coroutine respawnTimer;
}
