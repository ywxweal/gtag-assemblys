using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020004BD RID: 1213
public class MonkeBall : MonoBehaviour
{
	// Token: 0x06001D50 RID: 7504 RVA: 0x0008E548 File Offset: 0x0008C748
	private void Start()
	{
		this.Refresh();
	}

	// Token: 0x06001D51 RID: 7505 RVA: 0x0008E550 File Offset: 0x0008C750
	private void Update()
	{
		this.UpdateVisualOffset();
		if (!PhotonNetwork.IsMasterClient)
		{
			if (this._resyncPosition)
			{
				this._resyncDelay -= Time.deltaTime;
				if (this._resyncDelay <= 0f)
				{
					this._resyncPosition = false;
					GameBallManager.Instance.RequestSetBallPosition(this.gameBall.id);
				}
			}
			if (this._positionFailsafe)
			{
				if (base.transform.position.y < -500f || (GameBallManager.Instance.transform.position - base.transform.position).sqrMagnitude > 6400f)
				{
					if (PhotonNetwork.IsConnected)
					{
						GameBallManager.Instance.RequestSetBallPosition(this.gameBall.id);
					}
					else
					{
						base.transform.position = GameBallManager.Instance.transform.position;
					}
					this._positionFailsafe = false;
					this._positionFailsafeTimer = 3f;
					return;
				}
			}
			else
			{
				this._positionFailsafeTimer -= Time.deltaTime;
				if (this._positionFailsafeTimer <= 0f)
				{
					this._positionFailsafe = true;
				}
			}
			return;
		}
		if (this.gameBall.onlyGrabTeamId != -1 && Time.timeAsDouble >= this.restrictTeamGrabEndTime)
		{
			MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, -1);
		}
		if (this.AlreadyDropped())
		{
			this._droppedTimer += Time.deltaTime;
			if (this._droppedTimer >= 7.5f)
			{
				this._droppedTimer = 0f;
				GameBallManager.Instance.RequestTeleportBall(this.gameBall.id, base.transform.position, base.transform.rotation, this._rigidBody.velocity, this._rigidBody.angularVelocity);
			}
		}
		if (this._resyncPosition)
		{
			this._resyncDelay -= Time.deltaTime;
			if (this._resyncDelay <= 0f)
			{
				this._resyncPosition = false;
				GameBallManager.Instance.RequestTeleportBall(this.gameBall.id, base.transform.position, base.transform.rotation, this._rigidBody.velocity, this._rigidBody.angularVelocity);
			}
		}
		if (this._positionFailsafe)
		{
			if (base.transform.position.y < -250f || (GameBallManager.Instance.transform.position - base.transform.position).sqrMagnitude > 6400f)
			{
				MonkeBallGame.Instance.LaunchBallNeutral(this.gameBall.id);
				this._positionFailsafe = false;
				this._positionFailsafeTimer = 3f;
				return;
			}
		}
		else
		{
			this._positionFailsafeTimer -= Time.deltaTime;
			if (this._positionFailsafeTimer <= 0f)
			{
				this._positionFailsafe = true;
			}
		}
	}

	// Token: 0x06001D52 RID: 7506 RVA: 0x0008E830 File Offset: 0x0008CA30
	public void OnCollisionEnter(Collision collision)
	{
		if (this.AlreadyDropped())
		{
			return;
		}
		if (MonkeBall.IsGamePlayer(collision.collider))
		{
			return;
		}
		this.alreadyDropped = true;
		this._droppedTimer = 0f;
		this.gameBall.PlayBounceFX();
		if (!PhotonNetwork.IsMasterClient)
		{
			if (this._rigidBody.velocity.sqrMagnitude > 1f)
			{
				this._resyncPosition = true;
				this._resyncDelay = 1.5f;
			}
			int lastHeldByActorNumber = this.gameBall.lastHeldByActorNumber;
			int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
			return;
		}
		if (this._rigidBody.velocity.sqrMagnitude > 1f)
		{
			this._resyncPosition = true;
			this._resyncDelay = 0.5f;
		}
		if (this._launchAfterScore)
		{
			this._launchAfterScore = false;
			MonkeBallGame.Instance.RequestRestrictBallToTeamOnScore(this.gameBall.id, MonkeBallGame.Instance.GetOtherTeam(this.gameBall.lastHeldByTeamId));
			return;
		}
		MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, MonkeBallGame.Instance.GetOtherTeam(this.gameBall.lastHeldByTeamId));
	}

	// Token: 0x06001D53 RID: 7507 RVA: 0x0008E94F File Offset: 0x0008CB4F
	public void TriggerDelayedResync()
	{
		this._resyncPosition = true;
		if (PhotonNetwork.IsMasterClient)
		{
			this._resyncDelay = 0.5f;
			return;
		}
		this._resyncDelay = 1.5f;
	}

	// Token: 0x06001D54 RID: 7508 RVA: 0x0008E976 File Offset: 0x0008CB76
	public void SetRigidbodyDiscrete()
	{
		this._rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
	}

	// Token: 0x06001D55 RID: 7509 RVA: 0x0008E984 File Offset: 0x0008CB84
	public void SetRigidbodyContinuous()
	{
		this._rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
	}

	// Token: 0x06001D56 RID: 7510 RVA: 0x0008E992 File Offset: 0x0008CB92
	public static MonkeBall Get(GameBall ball)
	{
		if (ball == null)
		{
			return null;
		}
		return ball.GetComponent<MonkeBall>();
	}

	// Token: 0x06001D57 RID: 7511 RVA: 0x0008E9A5 File Offset: 0x0008CBA5
	public bool AlreadyDropped()
	{
		return this.alreadyDropped;
	}

	// Token: 0x06001D58 RID: 7512 RVA: 0x0008E9AD File Offset: 0x0008CBAD
	public void OnGrabbed()
	{
		this.alreadyDropped = false;
		this._resyncPosition = false;
	}

	// Token: 0x06001D59 RID: 7513 RVA: 0x0008E9BD File Offset: 0x0008CBBD
	public void OnSwitchHeldByTeam(int teamId)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			MonkeBallGame.Instance.RequestRestrictBallToTeam(this.gameBall.id, teamId);
		}
	}

	// Token: 0x06001D5A RID: 7514 RVA: 0x0008E9DC File Offset: 0x0008CBDC
	public void ClearCannotGrabTeamId()
	{
		this.gameBall.onlyGrabTeamId = -1;
		this.restrictTeamGrabEndTime = -1.0;
		this.Refresh();
	}

	// Token: 0x06001D5B RID: 7515 RVA: 0x0008EA00 File Offset: 0x0008CC00
	public bool RestrictBallToTeam(int teamId, float duration)
	{
		if (teamId == this.gameBall.onlyGrabTeamId && Time.timeAsDouble + (double)duration < this.restrictTeamGrabEndTime)
		{
			return false;
		}
		this.gameBall.onlyGrabTeamId = teamId;
		this.restrictTeamGrabEndTime = Time.timeAsDouble + (double)duration;
		this.Refresh();
		return true;
	}

	// Token: 0x06001D5C RID: 7516 RVA: 0x0008EA4E File Offset: 0x0008CC4E
	private void Refresh()
	{
		if (this.gameBall.onlyGrabTeamId == -1)
		{
			this.mainRenderer.material = this.defaultMaterial;
			return;
		}
		this.mainRenderer.material = this.teamMaterial[this.gameBall.onlyGrabTeamId];
	}

	// Token: 0x06001D5D RID: 7517 RVA: 0x0008EA8D File Offset: 0x0008CC8D
	private static bool IsGamePlayer(Collider collider)
	{
		return GameBallPlayer.GetGamePlayer(collider, false) != null;
	}

	// Token: 0x06001D5E RID: 7518 RVA: 0x0008EA9C File Offset: 0x0008CC9C
	public void SetVisualOffset(bool detach)
	{
		if (detach)
		{
			this.lastVisiblePosition = this.mainRenderer.transform.position;
			this._visualOffset = true;
			this._timeOffset = Time.time;
			this.mainRenderer.transform.SetParent(null, true);
			return;
		}
		this.ReattachVisuals();
	}

	// Token: 0x06001D5F RID: 7519 RVA: 0x0008EAF0 File Offset: 0x0008CCF0
	private void ReattachVisuals()
	{
		if (!this._visualOffset)
		{
			return;
		}
		this.mainRenderer.transform.SetParent(base.transform);
		this.mainRenderer.transform.localPosition = Vector3.zero;
		this.mainRenderer.transform.localRotation = Quaternion.identity;
		this._visualOffset = false;
	}

	// Token: 0x06001D60 RID: 7520 RVA: 0x0008EB50 File Offset: 0x0008CD50
	private void UpdateVisualOffset()
	{
		if (this._visualOffset)
		{
			this.mainRenderer.transform.position = Vector3.Lerp(this.mainRenderer.transform.position, this._rigidBody.position, Mathf.Clamp((Time.time - this._timeOffset) / this.maxLerpTime, this.offsetLerp, 1f));
			if ((this.mainRenderer.transform.position - this._rigidBody.position).sqrMagnitude < this._offsetThreshold)
			{
				this.ReattachVisuals();
			}
		}
	}

	// Token: 0x04002076 RID: 8310
	public GameBall gameBall;

	// Token: 0x04002077 RID: 8311
	public MeshRenderer mainRenderer;

	// Token: 0x04002078 RID: 8312
	public Material defaultMaterial;

	// Token: 0x04002079 RID: 8313
	public Material[] teamMaterial;

	// Token: 0x0400207A RID: 8314
	public double restrictTeamGrabEndTime;

	// Token: 0x0400207B RID: 8315
	public bool alreadyDropped;

	// Token: 0x0400207C RID: 8316
	private bool _launchAfterScore;

	// Token: 0x0400207D RID: 8317
	private float _droppedTimer;

	// Token: 0x0400207E RID: 8318
	private bool _resyncPosition;

	// Token: 0x0400207F RID: 8319
	private float _resyncDelay;

	// Token: 0x04002080 RID: 8320
	private bool _visualOffset;

	// Token: 0x04002081 RID: 8321
	private float _offsetThreshold = 0.05f;

	// Token: 0x04002082 RID: 8322
	private float _timeOffset;

	// Token: 0x04002083 RID: 8323
	public float maxLerpTime = 0.5f;

	// Token: 0x04002084 RID: 8324
	public float offsetLerp = 0.2f;

	// Token: 0x04002085 RID: 8325
	private bool _positionFailsafe = true;

	// Token: 0x04002086 RID: 8326
	private float _positionFailsafeTimer;

	// Token: 0x04002087 RID: 8327
	public Vector3 lastVisiblePosition;

	// Token: 0x04002088 RID: 8328
	[SerializeField]
	private Rigidbody _rigidBody;
}
