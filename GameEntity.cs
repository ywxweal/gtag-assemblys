using System;
using UnityEngine;

// Token: 0x02000560 RID: 1376
public class GameEntity : MonoBehaviour
{
	// Token: 0x1400004E RID: 78
	// (add) Token: 0x0600216B RID: 8555 RVA: 0x000A7394 File Offset: 0x000A5594
	// (remove) Token: 0x0600216C RID: 8556 RVA: 0x000A73CC File Offset: 0x000A55CC
	public event GameEntity.StateChangedEvent OnStateChanged;

	// Token: 0x0600216D RID: 8557 RVA: 0x000A7404 File Offset: 0x000A5604
	private void Awake()
	{
		this.id = GameEntityId.Invalid;
		this.rigidBody = base.GetComponent<Rigidbody>();
		if (this.disc && this.rigidBody != null)
		{
			this.rigidBody.maxAngularVelocity = 28f;
		}
		this.heldByActorNumber = -1;
		this.heldByHandIndex = -1;
		this.onlyGrabActorNumber = -1;
	}

	// Token: 0x0600216E RID: 8558 RVA: 0x000A7463 File Offset: 0x000A5663
	public Vector3 GetVelocity()
	{
		if (this.rigidBody == null)
		{
			return Vector3.zero;
		}
		return this.rigidBody.velocity;
	}

	// Token: 0x0600216F RID: 8559 RVA: 0x000A7484 File Offset: 0x000A5684
	public void PlayCatchFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.volume = this.catchSoundVolume;
			this.audioSource.PlayOneShot(this.catchSound);
		}
	}

	// Token: 0x06002170 RID: 8560 RVA: 0x000A74B6 File Offset: 0x000A56B6
	public void PlayThrowFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.volume = this.throwSoundVolume;
			this.audioSource.PlayOneShot(this.throwSound);
		}
	}

	// Token: 0x06002171 RID: 8561 RVA: 0x000A74E8 File Offset: 0x000A56E8
	private bool IsGamePlayer(Collider collider)
	{
		return GamePlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x06002172 RID: 8562 RVA: 0x000A74F7 File Offset: 0x000A56F7
	public long GetState()
	{
		return this.state;
	}

	// Token: 0x06002173 RID: 8563 RVA: 0x000A7500 File Offset: 0x000A5700
	public void SetState(long newState)
	{
		if (this.state != newState)
		{
			long num = this.state;
			this.state = newState;
			GameEntity.StateChangedEvent onStateChanged = this.OnStateChanged;
			if (onStateChanged == null)
			{
				return;
			}
			onStateChanged(num, newState);
		}
	}

	// Token: 0x06002174 RID: 8564 RVA: 0x000A7536 File Offset: 0x000A5736
	public static int GetNetId(GameEntityId gameEntityId)
	{
		return GameEntityManager.instance.GetNetIdFromEntityId(gameEntityId);
	}

	// Token: 0x06002175 RID: 8565 RVA: 0x000A7545 File Offset: 0x000A5745
	public static int GetNetId(GameEntity gameEntity)
	{
		if (gameEntity == null)
		{
			return -1;
		}
		return GameEntityManager.instance.GetNetIdFromEntityId(gameEntity.id);
	}

	// Token: 0x06002176 RID: 8566 RVA: 0x000A7564 File Offset: 0x000A5764
	public static GameEntityId GetIdFromNetId(int netId)
	{
		return GameEntityManager.instance.GetEntityIdFromNetId(netId);
	}

	// Token: 0x06002177 RID: 8567 RVA: 0x000A7574 File Offset: 0x000A5774
	public static GameEntity Get(Collider collider)
	{
		if (collider == null)
		{
			return null;
		}
		Transform transform = collider.transform;
		while (transform != null)
		{
			GameEntity component = transform.GetComponent<GameEntity>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x0400259B RID: 9627
	public const int Invalid = -1;

	// Token: 0x0400259C RID: 9628
	public GameEntityId id;

	// Token: 0x0400259D RID: 9629
	public int typeId;

	// Token: 0x0400259E RID: 9630
	public long createData;

	// Token: 0x0400259F RID: 9631
	public bool pickupable = true;

	// Token: 0x040025A0 RID: 9632
	public float gravityMult = 1f;

	// Token: 0x040025A1 RID: 9633
	public bool disc;

	// Token: 0x040025A2 RID: 9634
	public Vector3 localDiscUp;

	// Token: 0x040025A3 RID: 9635
	public AudioSource audioSource;

	// Token: 0x040025A4 RID: 9636
	public AudioClip catchSound;

	// Token: 0x040025A5 RID: 9637
	public float catchSoundVolume;

	// Token: 0x040025A6 RID: 9638
	public AudioClip throwSound;

	// Token: 0x040025A7 RID: 9639
	public float throwSoundVolume;

	// Token: 0x040025A8 RID: 9640
	private Rigidbody rigidBody;

	// Token: 0x040025A9 RID: 9641
	public int heldByActorNumber;

	// Token: 0x040025AA RID: 9642
	public int heldByHandIndex;

	// Token: 0x040025AB RID: 9643
	public int lastHeldByActorNumber;

	// Token: 0x040025AC RID: 9644
	public int onlyGrabActorNumber;

	// Token: 0x040025AD RID: 9645
	public Action OnGrabbed;

	// Token: 0x040025AE RID: 9646
	public Action OnReleased;

	// Token: 0x040025B0 RID: 9648
	private long state;

	// Token: 0x02000561 RID: 1377
	// (Invoke) Token: 0x0600217A RID: 8570
	public delegate void StateChangedEvent(long prevState, long nextState);
}
