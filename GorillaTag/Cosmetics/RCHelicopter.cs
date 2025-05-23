using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DB9 RID: 3513
	public class RCHelicopter : RCVehicle
	{
		// Token: 0x060056FB RID: 22267 RVA: 0x001A9FC4 File Offset: 0x001A81C4
		protected override void AuthorityBeginDocked()
		{
			base.AuthorityBeginDocked();
			this.turnRate = 0f;
			this.verticalPropeller.localRotation = this.verticalPropellerBaseRotation;
			this.turnPropeller.localRotation = this.turnPropellerBaseRotation;
			if (this.connectedRemote == null)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060056FC RID: 22268 RVA: 0x001AA020 File Offset: 0x001A8220
		protected override void Awake()
		{
			base.Awake();
			this.verticalPropellerBaseRotation = this.verticalPropeller.localRotation;
			this.turnPropellerBaseRotation = this.turnPropeller.localRotation;
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
		}

		// Token: 0x060056FD RID: 22269 RVA: 0x001AA090 File Offset: 0x001A8290
		protected override void SharedUpdate(float dt)
		{
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = Mathf.Lerp(this.mainPropellerSpinRateRange.x, this.mainPropellerSpinRateRange.y, this.activeInput.trigger);
				this.verticalPropeller.Rotate(new Vector3(0f, num * dt, 0f), Space.Self);
				this.turnPropeller.Rotate(new Vector3(this.activeInput.joystick.x * this.backPropellerSpinRate * dt, 0f, 0f), Space.Self);
			}
		}

		// Token: 0x060056FE RID: 22270 RVA: 0x001AA120 File Offset: 0x001A8320
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
			{
				return;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			Vector3 velocity = this.rb.velocity;
			float magnitude = velocity.magnitude;
			float num = this.activeInput.joystick.x * this.maxTurnRate;
			this.turnRate = Mathf.MoveTowards(this.turnRate, num, this.turnAccel * fixedDeltaTime);
			float num2 = this.activeInput.joystick.y * this.maxHorizontalSpeed;
			float num3 = Mathf.Sign(this.activeInput.joystick.y) * Mathf.Lerp(0f, this.maxHorizontalTiltAngle, Mathf.Abs(this.activeInput.joystick.y));
			base.transform.rotation = Quaternion.Euler(new Vector3(num3, this.turnAccel, 0f));
			float num4 = Mathf.Abs(num2);
			Vector3 normalized = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
			float num5 = Vector3.Dot(normalized, velocity);
			if (num4 > 0.01f && ((num2 > 0f && num2 > num5) || (num2 < 0f && num2 < num5)))
			{
				this.rb.AddForce(normalized * Mathf.Sign(num2) * this.horizontalAccel * fixedDeltaTime, ForceMode.Acceleration);
			}
			float num6 = this.activeInput.trigger * this.maxAscendSpeed;
			if (num6 > 0.01f && velocity.y < num6)
			{
				this.rb.AddForce(Vector3.up * this.ascendAccel, ForceMode.Acceleration);
			}
			if (this.rb.useGravity)
			{
				this.rb.AddForce(-Physics.gravity * this.gravityCompensation, ForceMode.Acceleration);
			}
		}

		// Token: 0x060056FF RID: 22271 RVA: 0x001AA2EE File Offset: 0x001A84EE
		private void OnTriggerEnter(Collider other)
		{
			if (!other.isTrigger && base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginCrash();
			}
		}

		// Token: 0x04005B4B RID: 23371
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x04005B4C RID: 23372
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x04005B4D RID: 23373
		[SerializeField]
		private float gravityCompensation = 0.5f;

		// Token: 0x04005B4E RID: 23374
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x04005B4F RID: 23375
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x04005B50 RID: 23376
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x04005B51 RID: 23377
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x04005B52 RID: 23378
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x04005B53 RID: 23379
		[SerializeField]
		private Vector2 mainPropellerSpinRateRange = new Vector2(3f, 15f);

		// Token: 0x04005B54 RID: 23380
		[SerializeField]
		private float backPropellerSpinRate = 5f;

		// Token: 0x04005B55 RID: 23381
		[SerializeField]
		private Transform verticalPropeller;

		// Token: 0x04005B56 RID: 23382
		[SerializeField]
		private Transform turnPropeller;

		// Token: 0x04005B57 RID: 23383
		private Quaternion verticalPropellerBaseRotation;

		// Token: 0x04005B58 RID: 23384
		private Quaternion turnPropellerBaseRotation;

		// Token: 0x04005B59 RID: 23385
		private float turnRate;

		// Token: 0x04005B5A RID: 23386
		private float ascendAccel;

		// Token: 0x04005B5B RID: 23387
		private float turnAccel;

		// Token: 0x04005B5C RID: 23388
		private float horizontalAccel;
	}
}
