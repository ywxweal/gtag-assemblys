using System;
using GorillaExtensions;
using GorillaLocomotion;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x020008B4 RID: 2228
public class ForceVolume : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060035E3 RID: 13795 RVA: 0x00104E34 File Offset: 0x00103034
	private void Awake()
	{
		this.volume = base.GetComponent<Collider>();
		this.audioState = ForceVolume.AudioState.None;
	}

	// Token: 0x060035E4 RID: 13796 RVA: 0x00010F2B File Offset: 0x0000F12B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060035E5 RID: 13797 RVA: 0x00010F34 File Offset: 0x0000F134
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060035E6 RID: 13798 RVA: 0x00104E4C File Offset: 0x0010304C
	public void SliceUpdate()
	{
		if (this.audioSource && this.audioSource != null && !this.audioSource.isPlaying && this.audioSource.enabled)
		{
			this.audioSource.enabled = false;
		}
	}

	// Token: 0x060035E7 RID: 13799 RVA: 0x00104E9C File Offset: 0x0010309C
	private bool TriggerFilter(Collider other, out Rigidbody rb, out Transform xf)
	{
		rb = null;
		xf = null;
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
			xf = GorillaTagger.Instance.headCollider.GetComponent<Transform>();
		}
		return rb != null && xf != null;
	}

	// Token: 0x060035E8 RID: 13800 RVA: 0x00104EFC File Offset: 0x001030FC
	public void OnTriggerEnter(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.enterClip == null)
		{
			return;
		}
		if (this.audioSource)
		{
			this.audioSource.enabled = true;
			this.audioSource.GTPlayOneShot(this.enterClip, 1f);
			this.audioState = ForceVolume.AudioState.Enter;
		}
		this.enterPos = transform.position;
	}

	// Token: 0x060035E9 RID: 13801 RVA: 0x00104F6C File Offset: 0x0010316C
	public void OnTriggerExit(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.audioSource)
		{
			this.audioSource.enabled = true;
			this.audioSource.GTPlayOneShot(this.exitClip, 1f);
			this.audioState = ForceVolume.AudioState.None;
		}
	}

	// Token: 0x060035EA RID: 13802 RVA: 0x00104FC4 File Offset: 0x001031C4
	public void OnTriggerStay(Collider other)
	{
		Rigidbody rigidbody = null;
		Transform transform = null;
		if (!this.TriggerFilter(other, out rigidbody, out transform))
		{
			return;
		}
		if (this.audioSource && !this.audioSource.isPlaying)
		{
			ForceVolume.AudioState audioState = this.audioState;
			if (audioState != ForceVolume.AudioState.Enter)
			{
				if (audioState == ForceVolume.AudioState.Loop)
				{
					if (this.loopClip != null)
					{
						this.audioSource.enabled = true;
						this.audioSource.GTPlayOneShot(this.loopClip, 1f);
					}
					this.audioState = ForceVolume.AudioState.Loop;
				}
			}
			else
			{
				if (this.loopCresendoClip != null)
				{
					this.audioSource.enabled = true;
					this.audioSource.GTPlayOneShot(this.loopCresendoClip, 1f);
				}
				this.audioState = ForceVolume.AudioState.Crescendo;
			}
		}
		if (this.disableGrip)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
		}
		SizeManager sizeManager = null;
		if (this.scaleWithSize)
		{
			sizeManager = rigidbody.GetComponent<SizeManager>();
		}
		Vector3 vector = rigidbody.velocity;
		if (this.scaleWithSize && sizeManager)
		{
			vector /= sizeManager.currentScale;
		}
		Vector3 vector2 = Vector3.Dot(transform.position - base.transform.position, base.transform.up) * base.transform.up;
		Vector3 vector3 = base.transform.position + vector2 - transform.position;
		float num = vector3.magnitude + 0.0001f;
		Vector3 vector4 = vector3 / num;
		float num2 = Vector3.Dot(vector, vector4);
		float num3 = this.accel;
		if (this.maxDepth > -1f)
		{
			float num4 = Vector3.Dot(transform.position - this.enterPos, vector4);
			float num5 = this.maxDepth - num4;
			float num6 = 0f;
			if (num5 > 0.0001f)
			{
				num6 = num2 * num2 / num5;
			}
			num3 = Mathf.Max(this.accel, num6);
		}
		float deltaTime = Time.deltaTime;
		Vector3 vector5 = base.transform.up * num3 * deltaTime;
		vector += vector5;
		Vector3 vector6 = Mathf.Min(Vector3.Dot(vector, base.transform.up), this.maxSpeed) * base.transform.up;
		Vector3 vector7 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
		Vector3 vector8 = Vector3.Dot(vector, base.transform.forward) * base.transform.forward;
		float num7 = 1f;
		float num8 = 1f;
		if (this.dampenLateralVelocity)
		{
			num7 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
			num8 = 1f - this.dampenZVelPerc * 0.01f * deltaTime;
		}
		vector = vector6 + num7 * vector7 + num8 * vector8;
		if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
		{
			vector -= num2 * vector4;
			if (num > this.pullTOCenterMinDistance)
			{
				num2 += this.pullToCenterAccel * deltaTime;
				float num9 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
				num2 = Mathf.Min(num2, num9);
			}
			else
			{
				num2 = 0f;
			}
			vector += num2 * vector4;
			if (vector.magnitude > 0.0001f)
			{
				Vector3 vector9 = Vector3.Cross(base.transform.up, vector4);
				float magnitude = vector9.magnitude;
				if (magnitude > 0.0001f)
				{
					vector9 /= magnitude;
					num2 = Vector3.Dot(vector, vector9);
					vector -= num2 * vector9;
					num2 -= this.pullToCenterAccel * deltaTime;
					num2 = Mathf.Max(0f, num2);
					vector += num2 * vector9;
				}
			}
		}
		if (this.scaleWithSize && sizeManager)
		{
			vector *= sizeManager.currentScale;
		}
		rigidbody.velocity = vector;
	}

	// Token: 0x060035EB RID: 13803 RVA: 0x001053DC File Offset: 0x001035DC
	public void OnDrawGizmosSelected()
	{
		base.GetComponents<Collider>();
		Gizmos.color = Color.magenta;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.pullTOCenterMinDistance / base.transform.lossyScale.x, 1f, this.pullTOCenterMinDistance / base.transform.lossyScale.z));
	}

	// Token: 0x060035EC RID: 13804 RVA: 0x0010544C File Offset: 0x0010364C
	public void SetPropertiesFromPlaceholder(ForceVolumeProperties properties, AudioSource volumeAudioSource, Collider colliderVolume)
	{
		this.accel = properties.accel;
		this.maxDepth = properties.maxDepth;
		this.maxSpeed = properties.maxSpeed;
		this.disableGrip = properties.disableGrip;
		this.dampenLateralVelocity = properties.dampenLateralVelocity;
		this.dampenXVelPerc = properties.dampenXVel;
		this.dampenZVelPerc = properties.dampenZVel;
		this.applyPullToCenterAcceleration = properties.applyPullToCenterAcceleration;
		this.pullToCenterAccel = properties.pullToCenterAccel;
		this.pullToCenterMaxSpeed = properties.pullToCenterMaxSpeed;
		this.pullTOCenterMinDistance = properties.pullToCenterMinDistance;
		this.enterClip = properties.enterClip;
		this.exitClip = properties.exitClip;
		this.loopClip = properties.loopClip;
		this.loopCresendoClip = properties.loopCrescendoClip;
		if (volumeAudioSource.IsNotNull())
		{
			this.audioSource = volumeAudioSource;
		}
		if (colliderVolume.IsNotNull())
		{
			this.volume = colliderVolume;
		}
	}

	// Token: 0x060035EE RID: 13806 RVA: 0x00011040 File Offset: 0x0000F240
	bool IGorillaSliceableSimple.get_isActiveAndEnabled()
	{
		return base.isActiveAndEnabled;
	}

	// Token: 0x04003BA8 RID: 15272
	[SerializeField]
	public bool scaleWithSize = true;

	// Token: 0x04003BA9 RID: 15273
	[SerializeField]
	private float accel;

	// Token: 0x04003BAA RID: 15274
	[SerializeField]
	private float maxDepth = -1f;

	// Token: 0x04003BAB RID: 15275
	[SerializeField]
	private float maxSpeed;

	// Token: 0x04003BAC RID: 15276
	[SerializeField]
	private bool disableGrip;

	// Token: 0x04003BAD RID: 15277
	[SerializeField]
	private bool dampenLateralVelocity = true;

	// Token: 0x04003BAE RID: 15278
	[SerializeField]
	private float dampenXVelPerc;

	// Token: 0x04003BAF RID: 15279
	[SerializeField]
	private float dampenZVelPerc;

	// Token: 0x04003BB0 RID: 15280
	[SerializeField]
	private bool applyPullToCenterAcceleration = true;

	// Token: 0x04003BB1 RID: 15281
	[SerializeField]
	private float pullToCenterAccel;

	// Token: 0x04003BB2 RID: 15282
	[SerializeField]
	private float pullToCenterMaxSpeed;

	// Token: 0x04003BB3 RID: 15283
	[SerializeField]
	private float pullTOCenterMinDistance = 0.1f;

	// Token: 0x04003BB4 RID: 15284
	private Collider volume;

	// Token: 0x04003BB5 RID: 15285
	public AudioClip enterClip;

	// Token: 0x04003BB6 RID: 15286
	public AudioClip exitClip;

	// Token: 0x04003BB7 RID: 15287
	public AudioClip loopClip;

	// Token: 0x04003BB8 RID: 15288
	public AudioClip loopCresendoClip;

	// Token: 0x04003BB9 RID: 15289
	public AudioSource audioSource;

	// Token: 0x04003BBA RID: 15290
	private Vector3 enterPos;

	// Token: 0x04003BBB RID: 15291
	private ForceVolume.AudioState audioState;

	// Token: 0x020008B5 RID: 2229
	private enum AudioState
	{
		// Token: 0x04003BBD RID: 15293
		None,
		// Token: 0x04003BBE RID: 15294
		Enter,
		// Token: 0x04003BBF RID: 15295
		Crescendo,
		// Token: 0x04003BC0 RID: 15296
		Loop,
		// Token: 0x04003BC1 RID: 15297
		Exit
	}
}
