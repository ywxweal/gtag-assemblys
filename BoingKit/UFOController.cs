using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E4B RID: 3659
	public class UFOController : MonoBehaviour
	{
		// Token: 0x06005B9A RID: 23450 RVA: 0x001C1D14 File Offset: 0x001BFF14
		private void Start()
		{
			this.m_linearVelocity = Vector3.zero;
			this.m_angularVelocity = 0f;
			this.m_yawAngle = base.transform.rotation.eulerAngles.y * MathUtil.Deg2Rad;
			this.m_hoverCenter = base.transform.position;
			this.m_hoverPhase = 0f;
			this.m_motorAngle = 0f;
			if (this.Eyes != null)
			{
				this.m_eyeInitScale = this.Eyes.localScale;
				this.m_eyeInitPositionLs = this.Eyes.localPosition;
				this.m_blinkTimer = this.BlinkInterval + Random.Range(1f, 2f);
				this.m_lastBlinkWasDouble = false;
				this.m_eyeScaleSpring.Reset(this.m_eyeInitScale);
				this.m_eyePositionLsSpring.Reset(this.m_eyeInitPositionLs);
			}
		}

		// Token: 0x06005B9B RID: 23451 RVA: 0x001C1DF7 File Offset: 0x001BFFF7
		private void OnEnable()
		{
			this.Start();
		}

		// Token: 0x06005B9C RID: 23452 RVA: 0x001C1E00 File Offset: 0x001C0000
		private void FixedUpdate()
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			Vector3 vector = Vector3.zero;
			if (Input.GetKey(KeyCode.W))
			{
				vector += Vector3.forward;
			}
			if (Input.GetKey(KeyCode.S))
			{
				vector += Vector3.back;
			}
			if (Input.GetKey(KeyCode.A))
			{
				vector += Vector3.left;
			}
			if (Input.GetKey(KeyCode.D))
			{
				vector += Vector3.right;
			}
			if (Input.GetKey(KeyCode.R))
			{
				vector += Vector3.up;
			}
			if (Input.GetKey(KeyCode.F))
			{
				vector += Vector3.down;
			}
			if (vector.sqrMagnitude > MathUtil.Epsilon)
			{
				vector = vector.normalized * this.LinearThrust;
				this.m_linearVelocity += vector * fixedDeltaTime;
				this.m_linearVelocity = VectorUtil.ClampLength(this.m_linearVelocity, 0f, this.MaxLinearSpeed);
			}
			else
			{
				this.m_linearVelocity = VectorUtil.ClampLength(this.m_linearVelocity, 0f, Mathf.Max(0f, this.m_linearVelocity.magnitude - this.LinearDrag * fixedDeltaTime));
			}
			float magnitude = this.m_linearVelocity.magnitude;
			float num = magnitude * MathUtil.InvSafe(this.MaxLinearSpeed);
			Quaternion quaternion = Quaternion.identity;
			float num2 = 0f;
			if (magnitude > MathUtil.Epsilon)
			{
				Vector3 linearVelocity = this.m_linearVelocity;
				linearVelocity.y = 0f;
				float num3 = ((this.m_linearVelocity.magnitude > 0.01f) ? (1f - Mathf.Clamp01(Mathf.Abs(this.m_linearVelocity.y) / this.m_linearVelocity.magnitude)) : 0f);
				num2 = Mathf.Min(1f, magnitude / Mathf.Max(MathUtil.Epsilon, this.MaxLinearSpeed)) * num3;
				Vector3 normalized = Vector3.Cross(Vector3.up, linearVelocity).normalized;
				float num4 = this.Tilt * MathUtil.Deg2Rad * num2;
				quaternion = QuaternionUtil.AxisAngle(normalized, num4);
			}
			float num5 = 0f;
			if (Input.GetKey(KeyCode.Q))
			{
				num5 -= 1f;
			}
			if (Input.GetKey(KeyCode.E))
			{
				num5 += 1f;
			}
			bool key = Input.GetKey(KeyCode.LeftControl);
			if (Mathf.Abs(num5) > MathUtil.Epsilon)
			{
				float num6 = this.MaxAngularSpeed * (key ? 2.5f : 1f);
				num5 *= this.AngularThrust * MathUtil.Deg2Rad;
				this.m_angularVelocity += num5 * fixedDeltaTime;
				this.m_angularVelocity = Mathf.Clamp(this.m_angularVelocity, -num6 * MathUtil.Deg2Rad, num6 * MathUtil.Deg2Rad);
			}
			else
			{
				this.m_angularVelocity -= Mathf.Sign(this.m_angularVelocity) * Mathf.Min(Mathf.Abs(this.m_angularVelocity), this.AngularDrag * MathUtil.Deg2Rad * fixedDeltaTime);
			}
			this.m_yawAngle += this.m_angularVelocity * fixedDeltaTime;
			Quaternion quaternion2 = QuaternionUtil.AxisAngle(Vector3.up, this.m_yawAngle);
			this.m_hoverCenter += this.m_linearVelocity * fixedDeltaTime;
			this.m_hoverPhase += Time.deltaTime;
			Vector3 vector2 = 0.05f * Mathf.Sin(1.37f * this.m_hoverPhase) * Vector3.right + 0.05f * Mathf.Sin(1.93f * this.m_hoverPhase + 1.234f) * Vector3.forward + 0.04f * Mathf.Sin(0.97f * this.m_hoverPhase + 4.321f) * Vector3.up;
			vector2 *= this.Hover;
			Quaternion quaternion3 = Quaternion.FromToRotation(Vector3.up, vector2 + Vector3.up);
			base.transform.position = this.m_hoverCenter + vector2;
			base.transform.rotation = quaternion * quaternion2 * quaternion3;
			if (this.Motor != null)
			{
				float num7 = Mathf.Lerp(this.MotorBaseAngularSpeed, this.MotorMaxAngularSpeed, num2);
				this.m_motorAngle += num7 * MathUtil.Deg2Rad * fixedDeltaTime;
				this.Motor.localRotation = QuaternionUtil.AxisAngle(Vector3.up, this.m_motorAngle - this.m_yawAngle);
			}
			if (this.BubbleEmitter != null)
			{
				this.BubbleEmitter.emission.rateOverTime = Mathf.Lerp(this.BubbleBaseEmissionRate, this.BubbleMaxEmissionRate, num);
			}
			if (this.Eyes != null)
			{
				this.m_blinkTimer -= fixedDeltaTime;
				if (this.m_blinkTimer <= 0f)
				{
					bool flag = !this.m_lastBlinkWasDouble && Random.Range(0f, 1f) > 0.75f;
					this.m_blinkTimer = (flag ? 0.2f : (this.BlinkInterval + Random.Range(1f, 2f)));
					this.m_lastBlinkWasDouble = flag;
					this.m_eyeScaleSpring.Value.y = 0f;
					this.m_eyePositionLsSpring.Value.y = this.m_eyePositionLsSpring.Value.y - 0.025f;
				}
				this.Eyes.localScale = this.m_eyeScaleSpring.TrackDampingRatio(this.m_eyeInitScale, 30f, 0.8f, fixedDeltaTime);
				this.Eyes.localPosition = this.m_eyePositionLsSpring.TrackDampingRatio(this.m_eyeInitPositionLs, 30f, 0.8f, fixedDeltaTime);
			}
		}

		// Token: 0x04005F63 RID: 24419
		public float LinearThrust = 3f;

		// Token: 0x04005F64 RID: 24420
		public float MaxLinearSpeed = 2.5f;

		// Token: 0x04005F65 RID: 24421
		public float LinearDrag = 4f;

		// Token: 0x04005F66 RID: 24422
		public float Tilt = 15f;

		// Token: 0x04005F67 RID: 24423
		public float AngularThrust = 30f;

		// Token: 0x04005F68 RID: 24424
		public float MaxAngularSpeed = 30f;

		// Token: 0x04005F69 RID: 24425
		public float AngularDrag = 30f;

		// Token: 0x04005F6A RID: 24426
		[Range(0f, 1f)]
		public float Hover = 1f;

		// Token: 0x04005F6B RID: 24427
		public Transform Eyes;

		// Token: 0x04005F6C RID: 24428
		public float BlinkInterval = 5f;

		// Token: 0x04005F6D RID: 24429
		private float m_blinkTimer;

		// Token: 0x04005F6E RID: 24430
		private bool m_lastBlinkWasDouble;

		// Token: 0x04005F6F RID: 24431
		private Vector3 m_eyeInitScale;

		// Token: 0x04005F70 RID: 24432
		private Vector3 m_eyeInitPositionLs;

		// Token: 0x04005F71 RID: 24433
		private Vector3Spring m_eyeScaleSpring;

		// Token: 0x04005F72 RID: 24434
		private Vector3Spring m_eyePositionLsSpring;

		// Token: 0x04005F73 RID: 24435
		public Transform Motor;

		// Token: 0x04005F74 RID: 24436
		public float MotorBaseAngularSpeed = 10f;

		// Token: 0x04005F75 RID: 24437
		public float MotorMaxAngularSpeed = 10f;

		// Token: 0x04005F76 RID: 24438
		public ParticleSystem BubbleEmitter;

		// Token: 0x04005F77 RID: 24439
		public float BubbleBaseEmissionRate = 10f;

		// Token: 0x04005F78 RID: 24440
		public float BubbleMaxEmissionRate = 10f;

		// Token: 0x04005F79 RID: 24441
		private Vector3 m_linearVelocity;

		// Token: 0x04005F7A RID: 24442
		private float m_angularVelocity;

		// Token: 0x04005F7B RID: 24443
		private float m_yawAngle;

		// Token: 0x04005F7C RID: 24444
		private Vector3 m_hoverCenter;

		// Token: 0x04005F7D RID: 24445
		private float m_hoverPhase;

		// Token: 0x04005F7E RID: 24446
		private float m_motorAngle;
	}
}
