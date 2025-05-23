using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DBA RID: 3514
	public class RCPlane : RCVehicle
	{
		// Token: 0x06005701 RID: 22273 RVA: 0x001AA39C File Offset: 0x001A859C
		protected override void Awake()
		{
			base.Awake();
			this.pitchAccelMinMax.x = this.pitchVelocityTargetMinMax.x / this.pitchVelocityRampTimeMinMax.x;
			this.pitchAccelMinMax.y = this.pitchVelocityTargetMinMax.y / this.pitchVelocityRampTimeMinMax.y;
			this.rollAccel = this.rollVelocityTarget / this.rollVelocityRampTime;
			this.thrustAccel = this.thrustVelocityTarget / this.thrustAccelTime;
		}

		// Token: 0x06005702 RID: 22274 RVA: 0x001AA41C File Offset: 0x001A861C
		protected override void AuthorityBeginMobilization()
		{
			base.AuthorityBeginMobilization();
			float x = base.transform.lossyScale.x;
			this.rb.velocity = base.transform.forward * this.initialSpeed * x;
		}

		// Token: 0x06005703 RID: 22275 RVA: 0x001AA468 File Offset: 0x001A8668
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = this.activeInput.trigger;
			}
			this.leftAileronLevel = 0f;
			this.rightAileronLevel = 0f;
			float magnitude = this.activeInput.joystick.magnitude;
			if (magnitude > 0.01f)
			{
				float num = Mathf.Abs(this.activeInput.joystick.x) / magnitude;
				float num2 = Mathf.Abs(this.activeInput.joystick.y) / magnitude;
				this.leftAileronLevel = Mathf.Clamp(num * this.activeInput.joystick.x + num2 * -this.activeInput.joystick.y, -1f, 1f);
				this.rightAileronLevel = Mathf.Clamp(num * this.activeInput.joystick.x + num2 * this.activeInput.joystick.y, -1f, 1f);
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
				this.networkSync.syncedState.dataB = (byte)Mathf.Clamp(Mathf.FloorToInt(this.leftAileronLevel * 126f), -126, 126);
				this.networkSync.syncedState.dataC = (byte)Mathf.Clamp(Mathf.FloorToInt(this.rightAileronLevel * 126f), -126, 126);
			}
		}

		// Token: 0x06005704 RID: 22276 RVA: 0x001AA60C File Offset: 0x001A880C
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
				this.leftAileronLevel = Mathf.Clamp((float)this.networkSync.syncedState.dataB / 126f, -1f, 1f);
				this.rightAileronLevel = Mathf.Clamp((float)this.networkSync.syncedState.dataC / 126f, -1f, 1f);
			}
		}

		// Token: 0x06005705 RID: 22277 RVA: 0x001AA6A8 File Offset: 0x001A88A8
		protected override void SharedUpdate(float dt)
		{
			base.SharedUpdate(dt);
			switch (this.localState)
			{
			case RCVehicle.State.DockedLeft:
			case RCVehicle.State.DockedRight:
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 0.6f, 6.6666665f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.audioSource.loop = true;
					this.audioSource.clip = this.motorSound;
					this.audioSource.volume = 0f;
					this.audioSource.GTPlay();
				}
				float num = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, num, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 5f, 6.6666665f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.audioSource.GTStop();
					this.audioSource.clip = null;
					this.audioSource.loop = false;
					this.audioSource.volume = this.crashSoundVolume;
					if (this.crashSound != null)
					{
						this.audioSource.GTPlayOneShot(this.crashSound, 1f);
					}
				}
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 0f, 13.333333f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			}
			float num2 = Mathf.Lerp(this.aileronAngularRange.x, this.aileronAngularRange.y, Mathf.InverseLerp(-1f, 1f, this.leftAileronLevel));
			float num3 = Mathf.Lerp(this.aileronAngularRange.x, this.aileronAngularRange.y, Mathf.InverseLerp(-1f, 1f, this.rightAileronLevel));
			this.leftAileronAngle = Mathf.MoveTowards(this.leftAileronAngle, num2, this.aileronAngularAcc * Time.deltaTime);
			this.rightAileronAngle = Mathf.MoveTowards(this.rightAileronAngle, num3, this.aileronAngularAcc * Time.deltaTime);
			Quaternion quaternion = Quaternion.Euler(0f, -90f, 90f + this.leftAileronAngle);
			Quaternion quaternion2 = Quaternion.Euler(0f, 90f, -90f + this.rightAileronAngle);
			this.leftAileronLower.localRotation = quaternion;
			this.leftAileronUpper.localRotation = quaternion;
			this.rightAileronLower.localRotation = quaternion2;
			this.rightAileronUpper.localRotation = quaternion2;
		}

		// Token: 0x06005706 RID: 22278 RVA: 0x001AAA10 File Offset: 0x001A8C10
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
			{
				return;
			}
			float x = base.transform.lossyScale.x;
			float num = this.thrustVelocityTarget * x;
			float num2 = this.thrustAccel * x;
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.pitch = base.NormalizeAngle180(this.pitch);
			this.roll = base.NormalizeAngle180(this.roll);
			float num3 = this.pitch;
			float num4 = this.roll;
			if (this.activeInput.joystick.y >= 0f)
			{
				float num5 = this.activeInput.joystick.y * this.pitchVelocityTargetMinMax.y;
				this.pitchVel = Mathf.MoveTowards(this.pitchVel, num5, this.pitchAccelMinMax.y * fixedDeltaTime);
				this.pitch += this.pitchVel * fixedDeltaTime;
			}
			else
			{
				float num6 = -this.activeInput.joystick.y * this.pitchVelocityTargetMinMax.x;
				this.pitchVel = Mathf.MoveTowards(this.pitchVel, num6, this.pitchAccelMinMax.x * fixedDeltaTime);
				this.pitch += this.pitchVel * fixedDeltaTime;
			}
			float num7 = -this.activeInput.joystick.x * this.rollVelocityTarget;
			this.rollVel = Mathf.MoveTowards(this.rollVel, num7, this.rollAccel * fixedDeltaTime);
			this.roll += this.rollVel * fixedDeltaTime;
			Quaternion quaternion = Quaternion.Euler(new Vector3(this.pitch - num3, 0f, this.roll - num4));
			base.transform.rotation = base.transform.rotation * quaternion;
			this.rb.angularVelocity = Vector3.zero;
			Vector3 velocity = this.rb.velocity;
			float magnitude = velocity.magnitude;
			float num8 = Mathf.Max(Vector3.Dot(base.transform.forward, velocity), 0f);
			float num9 = this.activeInput.trigger * num;
			float num10 = 0.1f * x;
			if (num9 > num10 && num9 > num8)
			{
				float num11 = Mathf.MoveTowards(num8, num9, num2 * fixedDeltaTime);
				this.rb.AddForce(base.transform.forward * (num11 - num8), ForceMode.VelocityChange);
			}
			float num12 = 0.01f * x;
			float num13 = Vector3.Dot(velocity / Mathf.Max(magnitude, num12), base.transform.forward);
			float num14 = this.liftVsAttackCurve.Evaluate(num13);
			float num15 = Mathf.Lerp(this.liftVsSpeedOutput.x, this.liftVsSpeedOutput.y, Mathf.InverseLerp(this.liftVsSpeedInput.x, this.liftVsSpeedInput.y, magnitude / x));
			float num16 = num14 * num15;
			Vector3 vector = Vector3.RotateTowards(velocity, base.transform.forward * magnitude, this.pitchVelocityFollowRateAngle * 0.017453292f * fixedDeltaTime, this.pitchVelocityFollowRateMagnitude * fixedDeltaTime) - velocity;
			this.rb.AddForce(vector * num16, ForceMode.VelocityChange);
			float num17 = Vector3.Dot(velocity.normalized, base.transform.up);
			float num18 = this.dragVsAttackCurve.Evaluate(num17);
			this.rb.AddForce(-velocity * this.maxDrag * num18, ForceMode.Acceleration);
			if (this.rb.useGravity)
			{
				float num19 = Mathf.Lerp(this.gravityCompensationRange.x, this.gravityCompensationRange.y, Mathf.InverseLerp(0f, num, num8 / x));
				RCVehicle.AddScaledGravityCompensationForce(this.rb, x, num19);
			}
		}

		// Token: 0x06005707 RID: 22279 RVA: 0x001AADC8 File Offset: 0x001A8FC8
		private void OnCollisionEnter(Collision collision)
		{
			if (base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				for (int i = 0; i < collision.contactCount; i++)
				{
					ContactPoint contact = collision.GetContact(i);
					if (!this.nonCrashColliders.Contains(contact.thisCollider))
					{
						this.AuthorityBeginCrash();
					}
				}
				return;
			}
			bool flag = collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
			bool flag2 = collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaHand);
			if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
			{
				Vector3 vector = Vector3.zero;
				if (flag2)
				{
					GorillaHandClimber component = collision.collider.gameObject.GetComponent<GorillaHandClimber>();
					if (component != null)
					{
						vector = ((component.xrNode == XRNode.LeftHand) ? GTPlayer.Instance.leftHandCenterVelocityTracker : GTPlayer.Instance.rightHandCenterVelocityTracker).GetAverageVelocity(true, 0.15f, false);
					}
				}
				else if (collision.rigidbody != null)
				{
					vector = collision.rigidbody.velocity;
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

		// Token: 0x04005B5D RID: 23389
		public Vector2 pitchVelocityTargetMinMax = new Vector2(-180f, 180f);

		// Token: 0x04005B5E RID: 23390
		public Vector2 pitchVelocityRampTimeMinMax = new Vector2(-0.75f, 0.75f);

		// Token: 0x04005B5F RID: 23391
		public float rollVelocityTarget = 180f;

		// Token: 0x04005B60 RID: 23392
		public float rollVelocityRampTime = 0.75f;

		// Token: 0x04005B61 RID: 23393
		public float thrustVelocityTarget = 15f;

		// Token: 0x04005B62 RID: 23394
		public float thrustAccelTime = 2f;

		// Token: 0x04005B63 RID: 23395
		[SerializeField]
		private float pitchVelocityFollowRateAngle = 60f;

		// Token: 0x04005B64 RID: 23396
		[SerializeField]
		private float pitchVelocityFollowRateMagnitude = 5f;

		// Token: 0x04005B65 RID: 23397
		[SerializeField]
		private float maxDrag = 0.1f;

		// Token: 0x04005B66 RID: 23398
		[SerializeField]
		private Vector2 liftVsSpeedInput = new Vector2(0f, 4f);

		// Token: 0x04005B67 RID: 23399
		[SerializeField]
		private Vector2 liftVsSpeedOutput = new Vector2(0.5f, 1f);

		// Token: 0x04005B68 RID: 23400
		[SerializeField]
		private AnimationCurve liftVsAttackCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04005B69 RID: 23401
		[SerializeField]
		private AnimationCurve dragVsAttackCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04005B6A RID: 23402
		[SerializeField]
		private Vector2 gravityCompensationRange = new Vector2(0.5f, 1f);

		// Token: 0x04005B6B RID: 23403
		[SerializeField]
		private List<Collider> nonCrashColliders = new List<Collider>();

		// Token: 0x04005B6C RID: 23404
		[SerializeField]
		private Transform propeller;

		// Token: 0x04005B6D RID: 23405
		[SerializeField]
		private Transform leftAileronUpper;

		// Token: 0x04005B6E RID: 23406
		[SerializeField]
		private Transform leftAileronLower;

		// Token: 0x04005B6F RID: 23407
		[SerializeField]
		private Transform rightAileronUpper;

		// Token: 0x04005B70 RID: 23408
		[SerializeField]
		private Transform rightAileronLower;

		// Token: 0x04005B71 RID: 23409
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005B72 RID: 23410
		[SerializeField]
		private AudioClip motorSound;

		// Token: 0x04005B73 RID: 23411
		[SerializeField]
		private AudioClip crashSound;

		// Token: 0x04005B74 RID: 23412
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.02f, 0.1f);

		// Token: 0x04005B75 RID: 23413
		[SerializeField]
		private float crashSoundVolume = 0.12f;

		// Token: 0x04005B76 RID: 23414
		private float motorVolumeRampTime = 1f;

		// Token: 0x04005B77 RID: 23415
		private float propellerAngle;

		// Token: 0x04005B78 RID: 23416
		private float propellerSpinRate;

		// Token: 0x04005B79 RID: 23417
		private const float propellerIdleAcc = 1f;

		// Token: 0x04005B7A RID: 23418
		private const float propellerIdleSpinRate = 0.6f;

		// Token: 0x04005B7B RID: 23419
		private const float propellerMaxAcc = 6.6666665f;

		// Token: 0x04005B7C RID: 23420
		private const float propellerMaxSpinRate = 5f;

		// Token: 0x04005B7D RID: 23421
		public float initialSpeed = 3f;

		// Token: 0x04005B7E RID: 23422
		private float pitch;

		// Token: 0x04005B7F RID: 23423
		private float pitchVel;

		// Token: 0x04005B80 RID: 23424
		private Vector2 pitchAccelMinMax;

		// Token: 0x04005B81 RID: 23425
		private float roll;

		// Token: 0x04005B82 RID: 23426
		private float rollVel;

		// Token: 0x04005B83 RID: 23427
		private float rollAccel;

		// Token: 0x04005B84 RID: 23428
		private float thrustAccel;

		// Token: 0x04005B85 RID: 23429
		private float motorLevel;

		// Token: 0x04005B86 RID: 23430
		private float leftAileronLevel;

		// Token: 0x04005B87 RID: 23431
		private float rightAileronLevel;

		// Token: 0x04005B88 RID: 23432
		private Vector2 aileronAngularRange = new Vector2(-30f, 45f);

		// Token: 0x04005B89 RID: 23433
		private float aileronAngularAcc = 120f;

		// Token: 0x04005B8A RID: 23434
		private float leftAileronAngle;

		// Token: 0x04005B8B RID: 23435
		private float rightAileronAngle;
	}
}
