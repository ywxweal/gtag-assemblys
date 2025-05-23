using System;
using UnityEngine;

// Token: 0x020004B1 RID: 1201
public class GameBall : MonoBehaviour
{
	// Token: 0x17000322 RID: 802
	// (get) Token: 0x06001CF3 RID: 7411 RVA: 0x0008C4D6 File Offset: 0x0008A6D6
	public bool IsLaunched
	{
		get
		{
			return this._launched;
		}
	}

	// Token: 0x06001CF4 RID: 7412 RVA: 0x0008C4E0 File Offset: 0x0008A6E0
	private void Awake()
	{
		this.id = GameBallId.Invalid;
		if (this.rigidBody == null)
		{
			this.rigidBody = base.GetComponent<Rigidbody>();
		}
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		if (this.disc && this.rigidBody != null)
		{
			this.rigidBody.maxAngularVelocity = 28f;
		}
		this.heldByActorNumber = -1;
		this.lastHeldByTeamId = -1;
		this.onlyGrabTeamId = -1;
		this._monkeBall = base.GetComponent<MonkeBall>();
	}

	// Token: 0x06001CF5 RID: 7413 RVA: 0x0008C574 File Offset: 0x0008A774
	private void FixedUpdate()
	{
		if (this.rigidBody == null)
		{
			return;
		}
		if (this._launched)
		{
			this._launchedTimer += Time.fixedDeltaTime;
			if (this.collider.isTrigger && this._launchedTimer > 1f && this.rigidBody.velocity.y <= 0f)
			{
				this._launched = false;
				this.collider.isTrigger = false;
			}
		}
		Vector3 vector = -Physics.gravity * (1f - this.gravityMult);
		this.rigidBody.AddForce(vector, ForceMode.Acceleration);
		this._catchSoundDecay -= Time.deltaTime;
	}

	// Token: 0x06001CF6 RID: 7414 RVA: 0x0008C629 File Offset: 0x0008A829
	public void WasLaunched()
	{
		this._launched = true;
		this.collider.isTrigger = true;
		this._launchedTimer = 0f;
	}

	// Token: 0x06001CF7 RID: 7415 RVA: 0x0008C649 File Offset: 0x0008A849
	public Vector3 GetVelocity()
	{
		if (this.rigidBody == null)
		{
			return Vector3.zero;
		}
		return this.rigidBody.velocity;
	}

	// Token: 0x06001CF8 RID: 7416 RVA: 0x0008C66A File Offset: 0x0008A86A
	public void SetVelocity(Vector3 velocity)
	{
		this.rigidBody.velocity = velocity;
	}

	// Token: 0x06001CF9 RID: 7417 RVA: 0x0008C678 File Offset: 0x0008A878
	public void PlayCatchFx()
	{
		if (this.audioSource != null && this._catchSoundDecay <= 0f)
		{
			this.audioSource.clip = this.catchSound;
			this.audioSource.volume = this.catchSoundVolume;
			this.audioSource.Play();
			this._catchSoundDecay = 0.1f;
		}
	}

	// Token: 0x06001CFA RID: 7418 RVA: 0x0008C6D8 File Offset: 0x0008A8D8
	public void PlayThrowFx()
	{
		if (this.audioSource != null)
		{
			this.audioSource.clip = this.throwSound;
			this.audioSource.volume = this.throwSoundVolume;
			this.audioSource.Play();
		}
	}

	// Token: 0x06001CFB RID: 7419 RVA: 0x0008C715 File Offset: 0x0008A915
	public void PlayBounceFX()
	{
		if (this.audioSource != null)
		{
			this.audioSource.clip = this.groundSound;
			this.audioSource.volume = this.groundSoundVolume;
			this.audioSource.Play();
		}
	}

	// Token: 0x06001CFC RID: 7420 RVA: 0x0008C752 File Offset: 0x0008A952
	public void SetHeldByTeamId(int teamId)
	{
		this.lastHeldByTeamId = teamId;
	}

	// Token: 0x06001CFD RID: 7421 RVA: 0x0008C75B File Offset: 0x0008A95B
	private bool IsGamePlayer(Collider collider)
	{
		return GameBallPlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x06001CFE RID: 7422 RVA: 0x0008C76A File Offset: 0x0008A96A
	public void SetVisualOffset(bool detach)
	{
		if (this._monkeBall != null)
		{
			this._monkeBall.SetVisualOffset(detach);
		}
	}

	// Token: 0x0400202B RID: 8235
	public GameBallId id;

	// Token: 0x0400202C RID: 8236
	public float gravityMult = 1f;

	// Token: 0x0400202D RID: 8237
	public bool disc;

	// Token: 0x0400202E RID: 8238
	public Vector3 localDiscUp;

	// Token: 0x0400202F RID: 8239
	public AudioSource audioSource;

	// Token: 0x04002030 RID: 8240
	public AudioClip catchSound;

	// Token: 0x04002031 RID: 8241
	public float catchSoundVolume;

	// Token: 0x04002032 RID: 8242
	private float _catchSoundDecay;

	// Token: 0x04002033 RID: 8243
	public AudioClip throwSound;

	// Token: 0x04002034 RID: 8244
	public float throwSoundVolume;

	// Token: 0x04002035 RID: 8245
	public AudioClip groundSound;

	// Token: 0x04002036 RID: 8246
	public float groundSoundVolume;

	// Token: 0x04002037 RID: 8247
	[SerializeField]
	private Rigidbody rigidBody;

	// Token: 0x04002038 RID: 8248
	[SerializeField]
	private Collider collider;

	// Token: 0x04002039 RID: 8249
	public int heldByActorNumber;

	// Token: 0x0400203A RID: 8250
	public int lastHeldByActorNumber;

	// Token: 0x0400203B RID: 8251
	public int lastHeldByTeamId;

	// Token: 0x0400203C RID: 8252
	public int onlyGrabTeamId;

	// Token: 0x0400203D RID: 8253
	private bool _launched;

	// Token: 0x0400203E RID: 8254
	private float _launchedTimer;

	// Token: 0x0400203F RID: 8255
	public MonkeBall _monkeBall;
}
