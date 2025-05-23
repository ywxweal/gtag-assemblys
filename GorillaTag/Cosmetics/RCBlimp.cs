using System;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DB5 RID: 3509
	public class RCBlimp : RCVehicle
	{
		// Token: 0x060056DF RID: 22239 RVA: 0x001A85D4 File Offset: 0x001A67D4
		protected override void AuthorityBeginDocked()
		{
			base.AuthorityBeginDocked();
			this.turnRate = 0f;
			this.turnAngle = Vector3.SignedAngle(Vector3.forward, Vector3.ProjectOnPlane(base.transform.forward, Vector3.up), Vector3.up);
			this.motorLevel = 0f;
			if (this.connectedRemote == null)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060056E0 RID: 22240 RVA: 0x001A8644 File Offset: 0x001A6844
		protected override void Awake()
		{
			base.Awake();
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
			this.tiltAccel = this.maxHorizontalTiltAngle / this.horizontalTiltTime;
		}

		// Token: 0x060056E1 RID: 22241 RVA: 0x001A86A3 File Offset: 0x001A68A3
		protected override void OnDisable()
		{
			base.OnDisable();
			this.audioSource.GTStop();
		}

		// Token: 0x060056E2 RID: 22242 RVA: 0x001A86B8 File Offset: 0x001A68B8
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = Mathf.Max(Mathf.Max(Mathf.Abs(this.activeInput.joystick.y), Mathf.Abs(this.activeInput.joystick.x)), this.activeInput.trigger);
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
			}
		}

		// Token: 0x060056E3 RID: 22243 RVA: 0x001A8760 File Offset: 0x001A6960
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.localState == RCVehicle.State.Mobilized && this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
			}
		}

		// Token: 0x060056E4 RID: 22244 RVA: 0x001A87B0 File Offset: 0x001A69B0
		protected override void SharedUpdate(float dt)
		{
			base.SharedUpdate(dt);
			switch (this.localState)
			{
			case RCVehicle.State.Disabled:
				break;
			case RCVehicle.State.DockedLeft:
			case RCVehicle.State.DockedRight:
				if (this.localStatePrev != RCVehicle.State.DockedLeft && this.localStatePrev != RCVehicle.State.DockedRight)
				{
					this.audioSource.GTStop();
					this.blimpDeflateBlendWeight = 0f;
					this.blimpMesh.SetBlendShapeWeight(0, 0f);
					this.crashCollider.enabled = false;
				}
				this.leftPropellerSpinRate = Mathf.MoveTowards(this.leftPropellerSpinRate, 0.6f, 6.6666665f * dt);
				this.rightPropellerSpinRate = Mathf.MoveTowards(this.rightPropellerSpinRate, 0.6f, 6.6666665f * dt);
				this.leftPropellerAngle += this.leftPropellerSpinRate * 360f * dt;
				this.rightPropellerAngle += this.rightPropellerSpinRate * 360f * dt;
				this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.leftPropellerAngle, 0f, -90f));
				this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.rightPropellerAngle, 0f, 90f));
				return;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.audioSource.loop = true;
					this.audioSource.clip = this.motorSound;
					this.audioSource.volume = 0f;
					this.audioSource.GTPlay();
					this.blimpDeflateBlendWeight = 0f;
					this.blimpMesh.SetBlendShapeWeight(0, 0f);
					this.crashCollider.enabled = false;
				}
				float num = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, num, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				this.blimpDeflateBlendWeight = 0f;
				float num2 = this.activeInput.joystick.y * 5f;
				float num3 = this.activeInput.joystick.x * 5f;
				float num4 = Mathf.Clamp(num3 + num2 + 0.6f, -5f, 5f);
				float num5 = Mathf.Clamp(-num3 + num2 + 0.6f, -5f, 5f);
				this.leftPropellerSpinRate = Mathf.MoveTowards(this.leftPropellerSpinRate, num4, 6.6666665f * dt);
				this.rightPropellerSpinRate = Mathf.MoveTowards(this.rightPropellerSpinRate, num5, 6.6666665f * dt);
				this.leftPropellerAngle += this.leftPropellerSpinRate * 360f * dt;
				this.rightPropellerAngle += this.rightPropellerSpinRate * 360f * dt;
				this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.leftPropellerAngle, 0f, -90f));
				this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.rightPropellerAngle, 0f, 90f));
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.audioSource.GTStop();
					this.audioSource.clip = null;
					this.audioSource.loop = false;
					this.audioSource.volume = this.deflateSoundVolume;
					if (this.deflateSound != null)
					{
						this.audioSource.GTPlayOneShot(this.deflateSound, 1f);
					}
					this.leftPropellerSpinRate = 0f;
					this.rightPropellerSpinRate = 0f;
					this.leftPropellerAngle = 0f;
					this.rightPropellerAngle = 0f;
					this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
					this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
					this.crashCollider.enabled = true;
				}
				this.blimpDeflateBlendWeight = Mathf.Lerp(1f, this.blimpDeflateBlendWeight, Mathf.Exp(-this.deflateRate * dt));
				this.blimpMesh.SetBlendShapeWeight(0, this.blimpDeflateBlendWeight * 100f);
				return;
			default:
				return;
			}
		}

		// Token: 0x060056E5 RID: 22245 RVA: 0x001A8C00 File Offset: 0x001A6E00
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority)
			{
				return;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			float x = base.transform.lossyScale.x;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = this.maxAscendSpeed * x;
				float num2 = this.maxHorizontalSpeed * x;
				float num3 = this.ascendAccel * x;
				Vector3 velocity = this.rb.velocity;
				Vector3 normalized = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z).normalized;
				this.turnAngle = Vector3.SignedAngle(Vector3.forward, normalized, Vector3.up);
				this.tiltAngle = Vector3.SignedAngle(normalized, base.transform.forward, base.transform.right);
				float num4 = this.activeInput.joystick.x * this.maxTurnRate;
				this.turnRate = Mathf.MoveTowards(this.turnRate, num4, this.turnAccel * fixedDeltaTime);
				this.turnAngle += this.turnRate * fixedDeltaTime;
				float num5 = Vector3.Dot(normalized, velocity);
				float num6 = Mathf.InverseLerp(-num2, num2, num5);
				float num7 = Mathf.Lerp(-this.maxHorizontalTiltAngle, this.maxHorizontalTiltAngle, num6);
				this.tiltAngle = Mathf.MoveTowards(this.tiltAngle, num7, this.tiltAccel * fixedDeltaTime);
				base.transform.rotation = Quaternion.Euler(new Vector3(this.tiltAngle, this.turnAngle, 0f));
				Vector3 vector = new Vector3(velocity.x, 0f, velocity.z);
				Vector3 vector2 = Vector3.Lerp(normalized * this.activeInput.joystick.y * num2, vector, Mathf.Exp(-this.horizontalAccelTime * fixedDeltaTime));
				this.rb.AddForce(vector2 - vector, ForceMode.VelocityChange);
				float num8 = this.activeInput.trigger * num;
				if (num8 > 0.01f && velocity.y < num8)
				{
					this.rb.AddForce(Vector3.up * num3, ForceMode.Acceleration);
				}
				if (this.rb.useGravity)
				{
					RCVehicle.AddScaledGravityCompensationForce(this.rb, x, this.gravityCompensation);
					return;
				}
			}
			else if (this.localState == RCVehicle.State.Crashed && this.rb.useGravity)
			{
				RCVehicle.AddScaledGravityCompensationForce(this.rb, x, this.crashedGravityCompensation);
			}
		}

		// Token: 0x060056E6 RID: 22246 RVA: 0x001A8E70 File Offset: 0x001A7070
		private void OnTriggerEnter(Collider other)
		{
			bool flag = other.gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
			bool flag2 = other.gameObject.IsOnLayer(UnityLayer.GorillaHand);
			if (!other.isTrigger && base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginCrash();
				return;
			}
			if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
			{
				Vector3 vector = Vector3.zero;
				if (flag2)
				{
					GorillaHandClimber component = other.gameObject.GetComponent<GorillaHandClimber>();
					if (component != null)
					{
						vector = ((component.xrNode == XRNode.LeftHand) ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false);
					}
				}
				else if (other.attachedRigidbody != null)
				{
					vector = other.attachedRigidbody.velocity;
				}
				if (flag || vector.sqrMagnitude > 0.01f)
				{
					if (base.HasLocalAuthority)
					{
						this.AuthorityApplyImpact(vector, flag);
						return;
					}
					if (this.networkSync != null)
					{
						this.networkSync.photonView.RPC("HitRCVehicleRPC", RpcTarget.Others, new object[] { vector, flag });
					}
				}
			}
		}

		// Token: 0x04005AF0 RID: 23280
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x04005AF1 RID: 23281
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x04005AF2 RID: 23282
		[SerializeField]
		private float gravityCompensation = 0.9f;

		// Token: 0x04005AF3 RID: 23283
		[SerializeField]
		private float crashedGravityCompensation = 0.5f;

		// Token: 0x04005AF4 RID: 23284
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x04005AF5 RID: 23285
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x04005AF6 RID: 23286
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x04005AF7 RID: 23287
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x04005AF8 RID: 23288
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x04005AF9 RID: 23289
		[SerializeField]
		private float horizontalTiltTime = 2f;

		// Token: 0x04005AFA RID: 23290
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.1f, 0.8f);

		// Token: 0x04005AFB RID: 23291
		[SerializeField]
		private float deflateSoundVolume = 0.1f;

		// Token: 0x04005AFC RID: 23292
		[SerializeField]
		private Collider crashCollider;

		// Token: 0x04005AFD RID: 23293
		[SerializeField]
		private Transform leftPropeller;

		// Token: 0x04005AFE RID: 23294
		[SerializeField]
		private Transform rightPropeller;

		// Token: 0x04005AFF RID: 23295
		[SerializeField]
		private SkinnedMeshRenderer blimpMesh;

		// Token: 0x04005B00 RID: 23296
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005B01 RID: 23297
		[SerializeField]
		private AudioClip motorSound;

		// Token: 0x04005B02 RID: 23298
		[SerializeField]
		private AudioClip deflateSound;

		// Token: 0x04005B03 RID: 23299
		private float turnRate;

		// Token: 0x04005B04 RID: 23300
		private float turnAngle;

		// Token: 0x04005B05 RID: 23301
		private float tiltAngle;

		// Token: 0x04005B06 RID: 23302
		private float ascendAccel;

		// Token: 0x04005B07 RID: 23303
		private float turnAccel;

		// Token: 0x04005B08 RID: 23304
		private float tiltAccel;

		// Token: 0x04005B09 RID: 23305
		private float horizontalAccel;

		// Token: 0x04005B0A RID: 23306
		private float leftPropellerAngle;

		// Token: 0x04005B0B RID: 23307
		private float rightPropellerAngle;

		// Token: 0x04005B0C RID: 23308
		private float leftPropellerSpinRate;

		// Token: 0x04005B0D RID: 23309
		private float rightPropellerSpinRate;

		// Token: 0x04005B0E RID: 23310
		private float blimpDeflateBlendWeight;

		// Token: 0x04005B0F RID: 23311
		private float deflateRate = Mathf.Exp(1f);

		// Token: 0x04005B10 RID: 23312
		private const float propellerIdleAcc = 1f;

		// Token: 0x04005B11 RID: 23313
		private const float propellerIdleSpinRate = 0.6f;

		// Token: 0x04005B12 RID: 23314
		private const float propellerMaxAcc = 6.6666665f;

		// Token: 0x04005B13 RID: 23315
		private const float propellerMaxSpinRate = 5f;

		// Token: 0x04005B14 RID: 23316
		private float motorVolumeRampTime = 1f;

		// Token: 0x04005B15 RID: 23317
		private float motorLevel;
	}
}
