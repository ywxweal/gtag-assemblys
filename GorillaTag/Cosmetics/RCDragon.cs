using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DB8 RID: 3512
	public class RCDragon : RCVehicle
	{
		// Token: 0x060056EE RID: 22254 RVA: 0x001A94B8 File Offset: 0x001A76B8
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

		// Token: 0x060056EF RID: 22255 RVA: 0x001A9528 File Offset: 0x001A7728
		protected override void Awake()
		{
			base.Awake();
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
			this.tiltAccel = this.maxHorizontalTiltAngle / this.horizontalTiltTime;
			this.shouldFlap = false;
			this.isFlapping = false;
			this.StopBreathFire();
			if (this.animation != null)
			{
				this.animation[this.wingFlapAnimName].speed = this.wingFlapAnimSpeed;
				this.animation[this.crashAnimName].speed = this.crashAnimSpeed;
				this.animation[this.mouthClosedAnimName].layer = 1;
				this.animation[this.mouthBreathFireAnimName].layer = 1;
			}
			this.nextFlapEventAnimTime = this.flapAnimEventTime;
		}

		// Token: 0x060056F0 RID: 22256 RVA: 0x001A961B File Offset: 0x001A781B
		protected override void OnDisable()
		{
			base.OnDisable();
			this.audioSource.GTStop();
		}

		// Token: 0x060056F1 RID: 22257 RVA: 0x001A9630 File Offset: 0x001A7830
		public void StartBreathFire()
		{
			if (!string.IsNullOrEmpty(this.mouthBreathFireAnimName))
			{
				this.animation.CrossFade(this.mouthBreathFireAnimName, 0.1f);
			}
			if (this.fireBreath != null)
			{
				this.fireBreath.SetActive(true);
			}
			this.PlayRandomSound(this.breathFireSound, this.breathFireVolume);
			this.fireBreathTimeRemaining = this.fireBreathDuration;
		}

		// Token: 0x060056F2 RID: 22258 RVA: 0x001A9698 File Offset: 0x001A7898
		public void StopBreathFire()
		{
			if (!string.IsNullOrEmpty(this.mouthClosedAnimName))
			{
				this.animation.CrossFade(this.mouthClosedAnimName, 0.1f);
			}
			if (this.fireBreath != null)
			{
				this.fireBreath.SetActive(false);
			}
			this.fireBreathTimeRemaining = -1f;
		}

		// Token: 0x060056F3 RID: 22259 RVA: 0x001A96ED File Offset: 0x001A78ED
		public bool IsBreathingFire()
		{
			return this.fireBreathTimeRemaining >= 0f;
		}

		// Token: 0x060056F4 RID: 22260 RVA: 0x001A96FF File Offset: 0x001A78FF
		private void PlayRandomSound(List<AudioClip> clips, float volume)
		{
			if (clips == null || clips.Count == 0)
			{
				return;
			}
			this.PlaySound(clips[Random.Range(0, clips.Count)], volume);
		}

		// Token: 0x060056F5 RID: 22261 RVA: 0x001A9728 File Offset: 0x001A7928
		private void PlaySound(AudioClip clip, float volume)
		{
			if (this.audioSource == null || clip == null)
			{
				return;
			}
			this.audioSource.GTStop();
			this.audioSource.clip = null;
			this.audioSource.loop = false;
			this.audioSource.volume = volume;
			this.audioSource.GTPlayOneShot(clip, 1f);
		}

		// Token: 0x060056F6 RID: 22262 RVA: 0x001A9790 File Offset: 0x001A7990
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = Mathf.Max(Mathf.Max(Mathf.Abs(this.activeInput.joystick.y), Mathf.Abs(this.activeInput.joystick.x)), this.activeInput.trigger);
				if (!this.IsBreathingFire() && this.activeInput.buttons > 0)
				{
					this.StartBreathFire();
				}
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
				this.networkSync.syncedState.dataB = this.activeInput.buttons;
				this.networkSync.syncedState.dataC = (this.shouldFlap ? 1 : 0);
			}
		}

		// Token: 0x060056F7 RID: 22263 RVA: 0x001A988C File Offset: 0x001A7A8C
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.localState == RCVehicle.State.Mobilized && this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
				if (!this.IsBreathingFire() && this.networkSync.syncedState.dataB > 0)
				{
					this.StartBreathFire();
				}
				this.shouldFlap = this.networkSync.syncedState.dataC > 0;
			}
		}

		// Token: 0x060056F8 RID: 22264 RVA: 0x001A9914 File Offset: 0x001A7B14
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
					if (this.crashCollider != null)
					{
						this.crashCollider.enabled = false;
					}
					if (this.animation != null)
					{
						this.animation.Play(this.dockedAnimName);
					}
					if (this.IsBreathingFire())
					{
						this.StopBreathFire();
						return;
					}
				}
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized && this.crashCollider != null)
				{
					this.crashCollider.enabled = false;
				}
				if (this.animation != null)
				{
					if (!this.isFlapping && this.shouldFlap)
					{
						this.animation.CrossFade(this.wingFlapAnimName, 0.1f);
						this.nextFlapEventAnimTime = this.flapAnimEventTime;
					}
					else if (this.isFlapping && !this.shouldFlap)
					{
						this.animation.CrossFade(this.idleAnimName, 0.15f);
					}
					this.isFlapping = this.shouldFlap;
					if (this.isFlapping && !this.IsBreathingFire())
					{
						AnimationState animationState = this.animation[this.wingFlapAnimName];
						if (animationState.normalizedTime * animationState.length > this.nextFlapEventAnimTime)
						{
							this.PlayRandomSound(this.wingFlapSound, this.wingFlapVolume);
							this.nextFlapEventAnimTime = (Mathf.Floor(animationState.normalizedTime) + 1f) * animationState.length + this.flapAnimEventTime;
						}
					}
				}
				GTTime.TimeAsDouble();
				if (this.IsBreathingFire())
				{
					this.fireBreathTimeRemaining -= dt;
					if (this.fireBreathTimeRemaining <= 0f)
					{
						this.StopBreathFire();
					}
				}
				float num = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, num, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.PlaySound(this.crashSound, this.crashSoundVolume);
					if (this.crashCollider != null)
					{
						this.crashCollider.enabled = true;
					}
					if (this.animation != null)
					{
						this.animation.CrossFade(this.crashAnimName, 0.05f);
					}
					if (this.IsBreathingFire())
					{
						this.StopBreathFire();
						return;
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060056F9 RID: 22265 RVA: 0x001A9BB0 File Offset: 0x001A7DB0
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority)
			{
				return;
			}
			float x = base.transform.lossyScale.x;
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.shouldFlap = false;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = this.maxAscendSpeed * x;
				float num2 = this.maxHorizontalSpeed * x;
				float num3 = this.ascendAccel * x;
				float num4 = this.ascendWhileFlyingAccelBoost * x;
				float num5 = 0.5f * x;
				float num6 = 45f;
				Vector3 velocity = this.rb.velocity;
				Vector3 normalized = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z).normalized;
				this.turnAngle = Vector3.SignedAngle(Vector3.forward, normalized, Vector3.up);
				this.tiltAngle = Vector3.SignedAngle(normalized, base.transform.forward, base.transform.right);
				float num7 = this.activeInput.joystick.x * this.maxTurnRate;
				this.turnRate = Mathf.MoveTowards(this.turnRate, num7, this.turnAccel * fixedDeltaTime);
				this.turnAngle += this.turnRate * fixedDeltaTime;
				float num8 = Vector3.Dot(normalized, velocity);
				float num9 = Mathf.InverseLerp(-num2, num2, num8);
				float num10 = Mathf.Lerp(-this.maxHorizontalTiltAngle, this.maxHorizontalTiltAngle, num9);
				this.tiltAngle = Mathf.MoveTowards(this.tiltAngle, num10, this.tiltAccel * fixedDeltaTime);
				base.transform.rotation = Quaternion.Euler(new Vector3(this.tiltAngle, this.turnAngle, 0f));
				Vector3 vector = new Vector3(velocity.x, 0f, velocity.z);
				Vector3 vector2 = Vector3.Lerp(normalized * this.activeInput.joystick.y * num2, vector, Mathf.Exp(-this.horizontalAccelTime * fixedDeltaTime));
				this.rb.AddForce(vector2 - vector, ForceMode.VelocityChange);
				float num11 = this.activeInput.trigger * num;
				if (num11 > 0.01f && velocity.y < num11)
				{
					this.rb.AddForce(Vector3.up * num3, ForceMode.Acceleration);
				}
				bool flag = Mathf.Abs(num8) > num5;
				bool flag2 = Mathf.Abs(this.turnRate) > num6;
				if (flag || flag2)
				{
					this.rb.AddForce(Vector3.up * num4, ForceMode.Acceleration);
				}
				this.shouldFlap = num11 > 0.01f || flag || flag2;
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

		// Token: 0x060056FA RID: 22266 RVA: 0x001A9E94 File Offset: 0x001A8094
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

		// Token: 0x04005B1F RID: 23327
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x04005B20 RID: 23328
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x04005B21 RID: 23329
		[SerializeField]
		private float ascendWhileFlyingAccelBoost;

		// Token: 0x04005B22 RID: 23330
		[SerializeField]
		private float gravityCompensation = 0.9f;

		// Token: 0x04005B23 RID: 23331
		[SerializeField]
		private float crashedGravityCompensation = 0.5f;

		// Token: 0x04005B24 RID: 23332
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x04005B25 RID: 23333
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x04005B26 RID: 23334
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x04005B27 RID: 23335
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x04005B28 RID: 23336
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x04005B29 RID: 23337
		[SerializeField]
		private float horizontalTiltTime = 2f;

		// Token: 0x04005B2A RID: 23338
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.1f, 0.8f);

		// Token: 0x04005B2B RID: 23339
		[SerializeField]
		private float crashSoundVolume = 0.1f;

		// Token: 0x04005B2C RID: 23340
		[SerializeField]
		private float breathFireVolume = 0.5f;

		// Token: 0x04005B2D RID: 23341
		[SerializeField]
		private float wingFlapVolume = 0.1f;

		// Token: 0x04005B2E RID: 23342
		[SerializeField]
		private Animation animation;

		// Token: 0x04005B2F RID: 23343
		[SerializeField]
		private string wingFlapAnimName;

		// Token: 0x04005B30 RID: 23344
		[SerializeField]
		private float wingFlapAnimSpeed = 1f;

		// Token: 0x04005B31 RID: 23345
		[SerializeField]
		private string dockedAnimName;

		// Token: 0x04005B32 RID: 23346
		[SerializeField]
		private string idleAnimName;

		// Token: 0x04005B33 RID: 23347
		[SerializeField]
		private string crashAnimName;

		// Token: 0x04005B34 RID: 23348
		[SerializeField]
		private float crashAnimSpeed = 1f;

		// Token: 0x04005B35 RID: 23349
		[SerializeField]
		private string mouthClosedAnimName;

		// Token: 0x04005B36 RID: 23350
		[SerializeField]
		private string mouthBreathFireAnimName;

		// Token: 0x04005B37 RID: 23351
		private bool shouldFlap;

		// Token: 0x04005B38 RID: 23352
		private bool isFlapping;

		// Token: 0x04005B39 RID: 23353
		private float nextFlapEventAnimTime;

		// Token: 0x04005B3A RID: 23354
		[SerializeField]
		private float flapAnimEventTime = 0.25f;

		// Token: 0x04005B3B RID: 23355
		[SerializeField]
		private GameObject fireBreath;

		// Token: 0x04005B3C RID: 23356
		[SerializeField]
		private float fireBreathDuration;

		// Token: 0x04005B3D RID: 23357
		private float fireBreathTimeRemaining;

		// Token: 0x04005B3E RID: 23358
		[SerializeField]
		private Collider crashCollider;

		// Token: 0x04005B3F RID: 23359
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005B40 RID: 23360
		[SerializeField]
		private List<AudioClip> breathFireSound;

		// Token: 0x04005B41 RID: 23361
		[SerializeField]
		private List<AudioClip> wingFlapSound;

		// Token: 0x04005B42 RID: 23362
		[SerializeField]
		private AudioClip crashSound;

		// Token: 0x04005B43 RID: 23363
		private float turnRate;

		// Token: 0x04005B44 RID: 23364
		private float turnAngle;

		// Token: 0x04005B45 RID: 23365
		private float tiltAngle;

		// Token: 0x04005B46 RID: 23366
		private float ascendAccel;

		// Token: 0x04005B47 RID: 23367
		private float turnAccel;

		// Token: 0x04005B48 RID: 23368
		private float tiltAccel;

		// Token: 0x04005B49 RID: 23369
		private float horizontalAccel;

		// Token: 0x04005B4A RID: 23370
		private float motorVolumeRampTime = 1f;

		// Token: 0x04005B4B RID: 23371
		private float motorLevel;
	}
}
