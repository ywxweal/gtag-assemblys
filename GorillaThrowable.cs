using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020006F2 RID: 1778
public class GorillaThrowable : MonoBehaviourPun, IPunObservable, IPhotonViewCallback
{
	// Token: 0x06002C4A RID: 11338 RVA: 0x000DA36C File Offset: 0x000D856C
	public virtual void Start()
	{
		this.offset = Vector3.zero;
		this.headsetTransform = GTPlayer.Instance.headCollider.transform;
		this.velocityHistory = new Vector3[this.trackingHistorySize];
		this.positionHistory = new Vector3[this.trackingHistorySize];
		this.headsetPositionHistory = new Vector3[this.trackingHistorySize];
		this.rotationHistory = new Vector3[this.trackingHistorySize];
		this.rotationalVelocityHistory = new Vector3[this.trackingHistorySize];
		for (int i = 0; i < this.trackingHistorySize; i++)
		{
			this.velocityHistory[i] = Vector3.zero;
			this.positionHistory[i] = base.transform.position - this.headsetTransform.position;
			this.headsetPositionHistory[i] = this.headsetTransform.position;
			this.rotationHistory[i] = base.transform.eulerAngles;
			this.rotationalVelocityHistory[i] = Vector3.zero;
		}
		this.currentIndex = 0;
		this.rigidbody = base.GetComponentInChildren<Rigidbody>();
	}

	// Token: 0x06002C4B RID: 11339 RVA: 0x000DA48C File Offset: 0x000D868C
	public virtual void LateUpdate()
	{
		if (this.isHeld && base.photonView.IsMine)
		{
			base.transform.rotation = this.transformToFollow.rotation * this.offsetRotation;
			if (!this.initialLerp && (base.transform.position - this.transformToFollow.position).magnitude > this.lerpDistanceLimit)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.transformToFollow.position + this.transformToFollow.rotation * this.offset, this.pickupLerp);
			}
			else
			{
				this.initialLerp = true;
				base.transform.position = this.transformToFollow.position + this.transformToFollow.rotation * this.offset;
			}
		}
		if (!base.photonView.IsMine)
		{
			this.rigidbody.isKinematic = true;
			base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, this.lerpValue);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetRotation, this.lerpValue);
		}
		this.StoreHistories();
	}

	// Token: 0x06002C4C RID: 11340 RVA: 0x000023F4 File Offset: 0x000005F4
	private void IsHandPushing(XRNode node)
	{
	}

	// Token: 0x06002C4D RID: 11341 RVA: 0x000DA5F8 File Offset: 0x000D87F8
	private void StoreHistories()
	{
		this.previousPosition = this.positionHistory[this.currentIndex];
		this.previousRotation = this.rotationHistory[this.currentIndex];
		this.previousHeadsetPosition = this.headsetPositionHistory[this.currentIndex];
		this.currentIndex = (this.currentIndex + 1) % this.trackingHistorySize;
		this.currentVelocity = (base.transform.position - this.headsetTransform.position - this.previousPosition) / Time.deltaTime;
		this.currentHeadsetVelocity = (this.headsetTransform.position - this.previousHeadsetPosition) / Time.deltaTime;
		this.currentRotationalVelocity = (base.transform.eulerAngles - this.previousRotation) / Time.deltaTime;
		this.denormalizedVelocityAverage = Vector3.zero;
		this.denormalizedRotationalVelocityAverage = Vector3.zero;
		this.loopIndex = 0;
		while (this.loopIndex < this.trackingHistorySize)
		{
			this.denormalizedVelocityAverage += this.velocityHistory[this.loopIndex];
			this.denormalizedRotationalVelocityAverage += this.rotationalVelocityHistory[this.loopIndex];
			this.loopIndex++;
		}
		this.denormalizedVelocityAverage /= (float)this.trackingHistorySize;
		this.denormalizedRotationalVelocityAverage /= (float)this.trackingHistorySize;
		this.velocityHistory[this.currentIndex] = this.currentVelocity;
		this.positionHistory[this.currentIndex] = base.transform.position - this.headsetTransform.position;
		this.headsetPositionHistory[this.currentIndex] = this.headsetTransform.position;
		this.rotationHistory[this.currentIndex] = base.transform.eulerAngles;
		this.rotationalVelocityHistory[this.currentIndex] = this.currentRotationalVelocity;
	}

	// Token: 0x06002C4E RID: 11342 RVA: 0x000DA824 File Offset: 0x000D8A24
	public virtual void Grabbed(Transform grabTransform)
	{
		this.grabbingTransform = grabTransform;
		this.isHeld = true;
		this.transformToFollow = this.grabbingTransform;
		this.offsetRotation = base.transform.rotation * Quaternion.Inverse(this.transformToFollow.rotation);
		this.initialLerp = false;
		this.rigidbody.isKinematic = true;
		this.rigidbody.useGravity = false;
		base.photonView.RequestOwnership();
	}

	// Token: 0x06002C4F RID: 11343 RVA: 0x000DA89C File Offset: 0x000D8A9C
	public virtual void ThrowThisThingo()
	{
		this.transformToFollow = null;
		this.isHeld = false;
		this.synchThrow = true;
		this.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		this.rigidbody.isKinematic = false;
		this.rigidbody.useGravity = true;
		if (this.isLinear || this.denormalizedVelocityAverage.magnitude < this.linearMax)
		{
			if (this.denormalizedVelocityAverage.magnitude * this.throwMultiplier < this.throwMagnitudeLimit)
			{
				this.rigidbody.velocity = this.denormalizedVelocityAverage * this.throwMultiplier + this.currentHeadsetVelocity;
			}
			else
			{
				this.rigidbody.velocity = this.denormalizedVelocityAverage.normalized * this.throwMagnitudeLimit + this.currentHeadsetVelocity;
			}
		}
		else
		{
			this.rigidbody.velocity = this.denormalizedVelocityAverage.normalized * Mathf.Max(Mathf.Min(Mathf.Pow(this.throwMultiplier * this.denormalizedVelocityAverage.magnitude / this.linearMax, this.exponThrowMultMax), 0.1f) * this.denormalizedHeadsetVelocityAverage.magnitude, this.throwMagnitudeLimit) + this.currentHeadsetVelocity;
		}
		this.rigidbody.angularVelocity = this.denormalizedRotationalVelocityAverage * 3.1415927f / 180f;
		this.rigidbody.MovePosition(this.rigidbody.transform.position + this.rigidbody.velocity * Time.deltaTime);
	}

	// Token: 0x06002C50 RID: 11344 RVA: 0x000DAA38 File Offset: 0x000D8C38
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);
			stream.SendNext(this.rigidbody.velocity);
			return;
		}
		this.targetPosition = (Vector3)stream.ReceiveNext();
		this.targetRotation = (Quaternion)stream.ReceiveNext();
		this.rigidbody.velocity = (Vector3)stream.ReceiveNext();
	}

	// Token: 0x06002C51 RID: 11345 RVA: 0x000DAAC8 File Offset: 0x000D8CC8
	public virtual void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.GetComponent<GorillaSurfaceOverride>() != null)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				base.photonView.RPC("PlaySurfaceHit", RpcTarget.Others, new object[]
				{
					this.bounceAudioClip,
					this.InterpolateVolume()
				});
			}
			this.PlaySurfaceHit(collision.collider.GetComponent<GorillaSurfaceOverride>().overrideIndex, this.InterpolateVolume());
		}
	}

	// Token: 0x06002C52 RID: 11346 RVA: 0x000DAB44 File Offset: 0x000D8D44
	[PunRPC]
	public void PlaySurfaceHit(int soundIndex, float tapVolume)
	{
		if (soundIndex > -1 && soundIndex < GTPlayer.Instance.materialData.Count)
		{
			this.audioSource.volume = tapVolume;
			this.audioSource.clip = (GTPlayer.Instance.materialData[soundIndex].overrideAudio ? GTPlayer.Instance.materialData[soundIndex].audio : GTPlayer.Instance.materialData[0].audio);
			this.audioSource.GTPlayOneShot(this.audioSource.clip, 1f);
		}
	}

	// Token: 0x06002C53 RID: 11347 RVA: 0x000DABE0 File Offset: 0x000D8DE0
	public float InterpolateVolume()
	{
		return (Mathf.Clamp(this.rigidbody.velocity.magnitude, this.minVelocity, this.maxVelocity) - this.minVelocity) / (this.maxVelocity - this.minVelocity) * (this.maxVolume - this.minVolume) + this.minVolume;
	}

	// Token: 0x04003272 RID: 12914
	public int trackingHistorySize;

	// Token: 0x04003273 RID: 12915
	public float throwMultiplier;

	// Token: 0x04003274 RID: 12916
	public float throwMagnitudeLimit;

	// Token: 0x04003275 RID: 12917
	private Vector3[] velocityHistory;

	// Token: 0x04003276 RID: 12918
	private Vector3[] headsetVelocityHistory;

	// Token: 0x04003277 RID: 12919
	private Vector3[] positionHistory;

	// Token: 0x04003278 RID: 12920
	private Vector3[] headsetPositionHistory;

	// Token: 0x04003279 RID: 12921
	private Vector3[] rotationHistory;

	// Token: 0x0400327A RID: 12922
	private Vector3[] rotationalVelocityHistory;

	// Token: 0x0400327B RID: 12923
	private Vector3 previousPosition;

	// Token: 0x0400327C RID: 12924
	private Vector3 previousRotation;

	// Token: 0x0400327D RID: 12925
	private Vector3 previousHeadsetPosition;

	// Token: 0x0400327E RID: 12926
	private int currentIndex;

	// Token: 0x0400327F RID: 12927
	private Vector3 currentVelocity;

	// Token: 0x04003280 RID: 12928
	private Vector3 currentHeadsetVelocity;

	// Token: 0x04003281 RID: 12929
	private Vector3 currentRotationalVelocity;

	// Token: 0x04003282 RID: 12930
	public Vector3 denormalizedVelocityAverage;

	// Token: 0x04003283 RID: 12931
	private Vector3 denormalizedHeadsetVelocityAverage;

	// Token: 0x04003284 RID: 12932
	private Vector3 denormalizedRotationalVelocityAverage;

	// Token: 0x04003285 RID: 12933
	private Transform headsetTransform;

	// Token: 0x04003286 RID: 12934
	private Vector3 targetPosition;

	// Token: 0x04003287 RID: 12935
	private Quaternion targetRotation;

	// Token: 0x04003288 RID: 12936
	public bool initialLerp;

	// Token: 0x04003289 RID: 12937
	public float lerpValue = 0.4f;

	// Token: 0x0400328A RID: 12938
	public float lerpDistanceLimit = 0.01f;

	// Token: 0x0400328B RID: 12939
	public bool isHeld;

	// Token: 0x0400328C RID: 12940
	public Rigidbody rigidbody;

	// Token: 0x0400328D RID: 12941
	private int loopIndex;

	// Token: 0x0400328E RID: 12942
	private Transform transformToFollow;

	// Token: 0x0400328F RID: 12943
	private Vector3 offset;

	// Token: 0x04003290 RID: 12944
	private Quaternion offsetRotation;

	// Token: 0x04003291 RID: 12945
	public AudioSource audioSource;

	// Token: 0x04003292 RID: 12946
	public int timeLastReceived;

	// Token: 0x04003293 RID: 12947
	public bool synchThrow;

	// Token: 0x04003294 RID: 12948
	public float tempFloat;

	// Token: 0x04003295 RID: 12949
	public Transform grabbingTransform;

	// Token: 0x04003296 RID: 12950
	public float pickupLerp;

	// Token: 0x04003297 RID: 12951
	public float minVelocity;

	// Token: 0x04003298 RID: 12952
	public float maxVelocity;

	// Token: 0x04003299 RID: 12953
	public float minVolume;

	// Token: 0x0400329A RID: 12954
	public float maxVolume;

	// Token: 0x0400329B RID: 12955
	public bool isLinear;

	// Token: 0x0400329C RID: 12956
	public float linearMax;

	// Token: 0x0400329D RID: 12957
	public float exponThrowMultMax;

	// Token: 0x0400329E RID: 12958
	public int bounceAudioClip;
}
