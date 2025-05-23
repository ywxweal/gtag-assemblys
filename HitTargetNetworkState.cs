using System;
using System.Collections;
using Fusion;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200039A RID: 922
[NetworkBehaviourWeaved(1)]
public class HitTargetNetworkState : NetworkComponent
{
	// Token: 0x06001576 RID: 5494 RVA: 0x00068FCC File Offset: 0x000671CC
	protected override void Awake()
	{
		base.Awake();
		this.audioPlayer = base.GetComponent<AudioSource>();
		SlingshotProjectileHitNotifier component = base.GetComponent<SlingshotProjectileHitNotifier>();
		if (component != null)
		{
			component.OnProjectileHit += this.ProjectileHitReciever;
			component.OnProjectileCollisionStay += this.ProjectileHitReciever;
			return;
		}
		Debug.LogError("Needs SlingshotProjectileHitNotifier added to this GameObject to increment score");
	}

	// Token: 0x06001577 RID: 5495 RVA: 0x0006902A File Offset: 0x0006722A
	protected override void Start()
	{
		base.Start();
		RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(this.OnLeftRoom));
	}

	// Token: 0x06001578 RID: 5496 RVA: 0x00069052 File Offset: 0x00067252
	private void SetInitialState()
	{
		this.networkedScore.Value = 0;
		this.nextHittableTimestamp = 0f;
		this.audioPlayer.GTStop();
	}

	// Token: 0x06001579 RID: 5497 RVA: 0x00069076 File Offset: 0x00067276
	public void OnLeftRoom()
	{
		this.SetInitialState();
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x0006907E File Offset: 0x0006727E
	internal override void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		if (Application.isEditor)
		{
			base.StartCoroutine(this.TestPressCheck());
		}
		this.SetInitialState();
	}

	// Token: 0x0600157B RID: 5499 RVA: 0x000690A6 File Offset: 0x000672A6
	private IEnumerator TestPressCheck()
	{
		for (;;)
		{
			if (this.testPress)
			{
				this.testPress = false;
				this.TargetHit(Vector3.zero, Vector3.one);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x000690B5 File Offset: 0x000672B5
	private void ProjectileHitReciever(SlingshotProjectile projectile, Collision collision)
	{
		this.TargetHit(projectile.launchPosition, collision.contacts[0].point);
	}

	// Token: 0x0600157D RID: 5501 RVA: 0x000690D4 File Offset: 0x000672D4
	public void TargetHit(Vector3 launchPoint, Vector3 impactPoint)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (Time.time <= this.nextHittableTimestamp)
		{
			return;
		}
		int num = this.networkedScore.Value;
		if (this.scoreIsDistance)
		{
			int num2 = Mathf.RoundToInt((launchPoint - impactPoint).magnitude * 3.28f);
			if (num2 <= num)
			{
				return;
			}
			num = num2;
		}
		else
		{
			num++;
			if (num >= 1000)
			{
				num = 0;
			}
		}
		if (this.resetAfterDuration > 0f && this.resetCoroutine == null)
		{
			this.resetAtTimestamp = Time.time + this.resetAfterDuration;
			this.resetCoroutine = base.StartCoroutine(this.ResetCo());
		}
		this.PlayAudio(this.networkedScore.Value, num);
		this.networkedScore.Value = num;
		this.nextHittableTimestamp = Time.time + (float)this.hitCooldownTime;
	}

	// Token: 0x1700025B RID: 603
	// (get) Token: 0x0600157E RID: 5502 RVA: 0x000691AC File Offset: 0x000673AC
	// (set) Token: 0x0600157F RID: 5503 RVA: 0x000691D2 File Offset: 0x000673D2
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe int Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HitTargetNetworkState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HitTargetNetworkState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = value;
		}
	}

	// Token: 0x06001580 RID: 5504 RVA: 0x000691F9 File Offset: 0x000673F9
	public override void WriteDataFusion()
	{
		this.Data = this.networkedScore.Value;
	}

	// Token: 0x06001581 RID: 5505 RVA: 0x0006920C File Offset: 0x0006740C
	public override void ReadDataFusion()
	{
		int data = this.Data;
		if (data != this.networkedScore.Value)
		{
			this.PlayAudio(this.networkedScore.Value, data);
		}
		this.networkedScore.Value = data;
	}

	// Token: 0x06001582 RID: 5506 RVA: 0x0006924C File Offset: 0x0006744C
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.networkedScore.Value);
	}

	// Token: 0x06001583 RID: 5507 RVA: 0x00069274 File Offset: 0x00067474
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		if (num != this.networkedScore.Value)
		{
			this.PlayAudio(this.networkedScore.Value, num);
		}
		this.networkedScore.Value = num;
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x000692C7 File Offset: 0x000674C7
	public void PlayAudio(int oldScore, int newScore)
	{
		if (oldScore > newScore && !this.scoreIsDistance)
		{
			this.audioPlayer.GTPlayOneShot(this.audioClips[1], 1f);
			return;
		}
		this.audioPlayer.GTPlayOneShot(this.audioClips[0], 1f);
	}

	// Token: 0x06001585 RID: 5509 RVA: 0x00069306 File Offset: 0x00067506
	private IEnumerator ResetCo()
	{
		while (Time.time < this.resetAtTimestamp)
		{
			yield return new WaitForSeconds(this.resetAtTimestamp - Time.time);
		}
		this.networkedScore.Value = 0;
		this.PlayAudio(this.networkedScore.Value, 0);
		this.resetCoroutine = null;
		yield break;
	}

	// Token: 0x06001587 RID: 5511 RVA: 0x00069324 File Offset: 0x00067524
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06001588 RID: 5512 RVA: 0x0006933C File Offset: 0x0006753C
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x040017F1 RID: 6129
	[SerializeField]
	private WatchableIntSO networkedScore;

	// Token: 0x040017F2 RID: 6130
	[SerializeField]
	private int hitCooldownTime = 1;

	// Token: 0x040017F3 RID: 6131
	[SerializeField]
	private bool testPress;

	// Token: 0x040017F4 RID: 6132
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x040017F5 RID: 6133
	[SerializeField]
	private bool scoreIsDistance;

	// Token: 0x040017F6 RID: 6134
	[SerializeField]
	private float resetAfterDuration;

	// Token: 0x040017F7 RID: 6135
	private AudioSource audioPlayer;

	// Token: 0x040017F8 RID: 6136
	private float nextHittableTimestamp;

	// Token: 0x040017F9 RID: 6137
	private float resetAtTimestamp;

	// Token: 0x040017FA RID: 6138
	private Coroutine resetCoroutine;

	// Token: 0x040017FB RID: 6139
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private int _Data;
}
