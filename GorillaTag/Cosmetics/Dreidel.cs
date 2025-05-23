using System;
using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DDB RID: 3547
	public class Dreidel : MonoBehaviour
	{
		// Token: 0x060057EB RID: 22507 RVA: 0x001B03B0 File Offset: 0x001AE5B0
		public bool TrySetIdle()
		{
			if (this.state == Dreidel.State.Idle || this.state == Dreidel.State.FindingSurface || this.state == Dreidel.State.Fallen)
			{
				this.StartIdle();
				return true;
			}
			return false;
		}

		// Token: 0x060057EC RID: 22508 RVA: 0x001B03D5 File Offset: 0x001AE5D5
		public bool TryCheckForSurfaces()
		{
			if (this.state == Dreidel.State.Idle || this.state == Dreidel.State.FindingSurface)
			{
				this.StartFindingSurfaces();
				return true;
			}
			return false;
		}

		// Token: 0x060057ED RID: 22509 RVA: 0x001B03F1 File Offset: 0x001AE5F1
		public void Spin()
		{
			this.StartSpin();
		}

		// Token: 0x060057EE RID: 22510 RVA: 0x001B03FC File Offset: 0x001AE5FC
		public bool TryGetSpinStartData(out Vector3 surfacePoint, out Vector3 surfaceNormal, out float randomDuration, out Dreidel.Side randomSide, out Dreidel.Variation randomVariation, out double startTime)
		{
			if (this.canStartSpin)
			{
				surfacePoint = this.surfacePlanePoint;
				surfaceNormal = this.surfacePlaneNormal;
				randomDuration = Random.Range(this.spinTimeRange.x, this.spinTimeRange.y);
				randomSide = (Dreidel.Side)Random.Range(0, 4);
				randomVariation = (Dreidel.Variation)Random.Range(0, 5);
				startTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : (-1.0));
				return true;
			}
			surfacePoint = Vector3.zero;
			surfaceNormal = Vector3.zero;
			randomDuration = 0f;
			randomSide = Dreidel.Side.Shin;
			randomVariation = Dreidel.Variation.Tumble;
			startTime = -1.0;
			return false;
		}

		// Token: 0x060057EF RID: 22511 RVA: 0x001B04A8 File Offset: 0x001AE6A8
		public void SetSpinStartData(Vector3 surfacePoint, Vector3 surfaceNormal, float duration, bool counterClockwise, Dreidel.Side side, Dreidel.Variation variation, double startTime)
		{
			this.surfacePlanePoint = surfacePoint;
			this.surfacePlaneNormal = surfaceNormal;
			this.spinTime = duration;
			this.spinStartTime = startTime;
			this.spinCounterClockwise = counterClockwise;
			this.landingSide = side;
			this.landingVariation = variation;
		}

		// Token: 0x060057F0 RID: 22512 RVA: 0x001B04E0 File Offset: 0x001AE6E0
		private void LateUpdate()
		{
			float deltaTime = Time.deltaTime;
			double num = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			switch (this.state)
			{
			default:
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				this.spinTransform.localRotation = Quaternion.identity;
				this.spinTransform.localPosition = Vector3.zero;
				return;
			case Dreidel.State.FindingSurface:
			{
				float num2 = ((GTPlayer.Instance != null) ? GTPlayer.Instance.scale : 1f);
				Vector3 down = Vector3.down;
				Vector3 vector = base.transform.parent.position - down * 2f * this.surfaceCheckDistance * num2;
				float num3 = (3f * this.surfaceCheckDistance + -this.bottomPointOffset.y) * num2;
				RaycastHit raycastHit;
				if (Physics.Raycast(vector, down, out raycastHit, num3, this.surfaceLayers.value, QueryTriggerInteraction.Ignore) && Vector3.Dot(raycastHit.normal, Vector3.up) > this.surfaceUprightThreshold && Vector3.Dot(raycastHit.normal, this.spinTransform.up) > this.surfaceDreidelAngleThreshold)
				{
					this.canStartSpin = true;
					this.surfacePlanePoint = raycastHit.point;
					this.surfacePlaneNormal = raycastHit.normal;
					this.AlignToSurfacePlane();
					this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
					this.UpdateSpinTransform();
					return;
				}
				this.canStartSpin = false;
				base.transform.localPosition = Vector3.zero;
				base.transform.localRotation = Quaternion.identity;
				this.spinTransform.localRotation = Quaternion.identity;
				this.spinTransform.localPosition = Vector3.zero;
				return;
			}
			case Dreidel.State.Spinning:
			{
				float num4 = Mathf.Clamp01((float)(num - this.stateStartTime) / this.spinTime);
				this.spinSpeed = Mathf.Lerp(this.spinSpeedStart, this.spinSpeedEnd, num4);
				float num5 = (this.spinCounterClockwise ? (-1f) : 1f);
				this.spinAngle += num5 * this.spinSpeed * 360f * deltaTime;
				float num6 = this.tiltWobble;
				float num7 = Mathf.Sin(this.spinWobbleFrequency * 2f * 3.1415927f * (float)(num - this.stateStartTime));
				float num8 = 0.5f * num7 + 0.5f;
				this.tiltWobble = Mathf.Lerp(this.spinWobbleAmplitudeEndMin * num4, this.spinWobbleAmplitude * num4, num8);
				if (this.landingTiltTarget.y == 0f)
				{
					if (this.landingVariation == Dreidel.Variation.Tumble || this.landingVariation == Dreidel.Variation.Smooth)
					{
						this.tiltFrontBack = Mathf.Sign(this.landingTiltTarget.x) * this.tiltWobble;
					}
					else
					{
						this.tiltFrontBack = Mathf.Sign(this.landingTiltLeadingTarget.x) * this.tiltWobble;
					}
				}
				else if (this.landingVariation == Dreidel.Variation.Tumble || this.landingVariation == Dreidel.Variation.Smooth)
				{
					this.tiltLeftRight = Mathf.Sign(this.landingTiltTarget.y) * this.tiltWobble;
				}
				else
				{
					this.tiltLeftRight = Mathf.Sign(this.landingTiltLeadingTarget.y) * this.tiltWobble;
				}
				float num9 = Mathf.Lerp(this.pathStartTurnRate, this.pathEndTurnRate, num4) + num7 * this.pathTurnRateSinOffset;
				if (this.spinCounterClockwise)
				{
					this.pathDir = Vector3.ProjectOnPlane(Quaternion.AngleAxis(-num9 * deltaTime, Vector3.up) * this.pathDir, Vector3.up);
					this.pathDir.Normalize();
				}
				else
				{
					this.pathDir = Vector3.ProjectOnPlane(Quaternion.AngleAxis(-num9 * deltaTime, Vector3.up) * this.pathDir, Vector3.up);
					this.pathDir.Normalize();
				}
				this.pathOffset += this.pathDir * this.pathMoveSpeed * deltaTime;
				this.AlignToSurfacePlane();
				this.UpdateSpinTransform();
				if (num4 - Mathf.Epsilon >= 1f && this.tiltWobble > 0.9f * this.spinWobbleAmplitude && num6 < this.tiltWobble)
				{
					this.StartFall();
					return;
				}
				break;
			}
			case Dreidel.State.Falling:
			{
				float num10 = this.fallTimeTumble;
				Dreidel.Variation variation = this.landingVariation;
				if (variation <= Dreidel.Variation.Smooth || variation - Dreidel.Variation.Bounce > 2)
				{
					this.spinSpeed = Mathf.MoveTowards(this.spinSpeed, 0f, this.spinSpeedStopRate * deltaTime);
					float num11 = (this.spinCounterClockwise ? (-1f) : 1f);
					this.spinAngle += num11 * this.spinSpeed * 360f * deltaTime;
					float num12 = ((this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallFrequency : this.tumbleFallFrontBackFrequency);
					float num13 = ((this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallDampingRatio : this.tumbleFallFrontBackDampingRatio);
					float num14 = ((this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallFrequency : this.tumbleFallFrequency);
					float num15 = ((this.landingVariation == Dreidel.Variation.Smooth) ? this.smoothFallDampingRatio : this.tumbleFallDampingRatio);
					this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltTarget.x, num12, num13, deltaTime);
					this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, num14, num15, deltaTime);
				}
				else
				{
					bool flag = this.landingVariation != Dreidel.Variation.Bounce;
					bool flag2 = this.landingVariation == Dreidel.Variation.FalseSlowTurn;
					float num16 = (flag ? this.slowTurnSwitchTime : this.bounceFallSwitchTime);
					if (flag)
					{
						num10 = this.fallTimeSlowTurn;
					}
					if (num - this.stateStartTime < (double)num16)
					{
						this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltLeadingTarget.x, this.tumbleFallFrontBackFrequency, this.tumbleFallFrontBackDampingRatio, deltaTime);
						this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltLeadingTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
					}
					else
					{
						this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltTarget.x, this.tumbleFallFrontBackFrequency, this.tumbleFallFrontBackDampingRatio, deltaTime);
						if (flag2)
						{
							if (!this.falseTargetReached && Mathf.Abs(this.landingTiltTarget.y - this.tiltLeftRight) > 0.49f)
							{
								this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.slowTurnFrequency, this.slowTurnDampingRatio, deltaTime);
							}
							else
							{
								this.falseTargetReached = true;
								this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltLeadingTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
							}
						}
						else if (flag && Mathf.Abs(this.landingTiltTarget.y - this.tiltLeftRight) > 0.45f)
						{
							this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.slowTurnFrequency, this.slowTurnDampingRatio, deltaTime);
						}
						else
						{
							this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
						}
					}
					this.spinSpeed = Mathf.MoveTowards(this.spinSpeed, 0f, this.spinSpeedStopRate * deltaTime);
					float num17 = (this.spinCounterClockwise ? (-1f) : 1f);
					this.spinAngle += num17 * this.spinSpeed * 360f * deltaTime;
				}
				this.AlignToSurfacePlane();
				this.UpdateSpinTransform();
				float num18 = (float)(num - this.stateStartTime);
				if (num18 > num10)
				{
					if (!this.hasLanded)
					{
						this.hasLanded = true;
						if (this.landingSide == Dreidel.Side.Gimel)
						{
							this.gimelConfetti.transform.position = this.spinTransform.position + Vector3.up * this.confettiHeight;
							this.gimelConfetti.gameObject.SetActive(true);
							this.audioSource.GTPlayOneShot(this.gimelConfettiSound, 1f);
						}
					}
					if (num18 > num10 + this.respawnTimeAfterLanding)
					{
						this.StartIdle();
					}
				}
				break;
			}
			case Dreidel.State.Fallen:
				break;
			}
		}

		// Token: 0x060057F1 RID: 22513 RVA: 0x001B0D0C File Offset: 0x001AEF0C
		private void StartIdle()
		{
			this.state = Dreidel.State.Idle;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.spinAngle = 0f;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			this.spinTransform.localRotation = Quaternion.identity;
			this.spinTransform.localPosition = Vector3.zero;
			this.tiltFrontBack = 0f;
			this.tiltLeftRight = 0f;
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
			this.gimelConfetti.gameObject.SetActive(false);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
		}

		// Token: 0x060057F2 RID: 22514 RVA: 0x001B0DE8 File Offset: 0x001AEFE8
		private void StartFindingSurfaces()
		{
			this.state = Dreidel.State.FindingSurface;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.spinAngle = 0f;
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			this.spinTransform.localRotation = Quaternion.identity;
			this.spinTransform.localPosition = Vector3.zero;
			this.tiltFrontBack = 0f;
			this.tiltLeftRight = 0f;
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
			this.gimelConfetti.gameObject.SetActive(false);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
		}

		// Token: 0x060057F3 RID: 22515 RVA: 0x001B0EC4 File Offset: 0x001AF0C4
		private void StartSpin()
		{
			this.state = Dreidel.State.Spinning;
			this.stateStartTime = ((this.spinStartTime > 0.0) ? this.spinStartTime : ((double)Time.time));
			this.canStartSpin = false;
			this.spinSpeed = this.spinSpeedStart;
			this.tiltWobble = 0f;
			this.audioSource.loop = true;
			this.audioSource.clip = this.spinLoopAudio;
			this.audioSource.GTPlay();
			this.gimelConfetti.gameObject.SetActive(false);
			this.AlignToSurfacePlane();
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.UpdateSpinTransform();
			this.pathOffset = Vector3.zero;
			this.pathDir = Vector3.forward;
		}

		// Token: 0x060057F4 RID: 22516 RVA: 0x001B0F8C File Offset: 0x001AF18C
		private void StartFall()
		{
			this.state = Dreidel.State.Falling;
			this.stateStartTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			this.canStartSpin = false;
			this.falseTargetReached = false;
			this.hasLanded = false;
			if (this.landingVariation == Dreidel.Variation.FalseSlowTurn)
			{
				if (this.spinCounterClockwise)
				{
					this.GetTiltVectorsForSideWithPrev(this.landingSide, out this.landingTiltLeadingTarget, out this.landingTiltTarget);
				}
				else
				{
					this.GetTiltVectorsForSideWithNext(this.landingSide, out this.landingTiltLeadingTarget, out this.landingTiltTarget);
				}
			}
			else if (this.spinCounterClockwise)
			{
				this.GetTiltVectorsForSideWithNext(this.landingSide, out this.landingTiltTarget, out this.landingTiltLeadingTarget);
			}
			else
			{
				this.GetTiltVectorsForSideWithPrev(this.landingSide, out this.landingTiltTarget, out this.landingTiltLeadingTarget);
			}
			this.spinSpeedSpring.Reset(this.spinSpeed, 0f);
			this.tiltFrontBackSpring.Reset(this.tiltFrontBack, 0f);
			this.tiltLeftRightSpring.Reset(this.tiltLeftRight, 0f);
			this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
			this.audioSource.loop = false;
			this.audioSource.GTPlayOneShot(this.fallSound, 1f);
			this.gimelConfetti.gameObject.SetActive(false);
		}

		// Token: 0x060057F5 RID: 22517 RVA: 0x001B10DC File Offset: 0x001AF2DC
		private Vector3 GetGroundContactPoint()
		{
			Vector3 position = this.spinTransform.position;
			this.dreidelCollider.enabled = true;
			Vector3 vector = this.dreidelCollider.ClosestPoint(position - base.transform.up);
			this.dreidelCollider.enabled = false;
			float num = Vector3.Dot(vector - position, this.spinTransform.up);
			if (num > 0f)
			{
				vector -= num * this.spinTransform.up;
			}
			return this.spinTransform.InverseTransformPoint(vector);
		}

		// Token: 0x060057F6 RID: 22518 RVA: 0x001B1170 File Offset: 0x001AF370
		private void GetTiltVectorsForSideWithPrev(Dreidel.Side side, out Vector2 sideTilt, out Vector2 prevSideTilt)
		{
			int num = ((side <= Dreidel.Side.Shin) ? 3 : (side - Dreidel.Side.Hey));
			if (side == Dreidel.Side.Hey || side == Dreidel.Side.Nun)
			{
				sideTilt = this.landingTiltValues[(int)side];
				prevSideTilt = this.landingTiltValues[num];
				prevSideTilt.x = sideTilt.x;
				return;
			}
			prevSideTilt = this.landingTiltValues[num];
			sideTilt = this.landingTiltValues[(int)side];
			sideTilt.x = prevSideTilt.x;
		}

		// Token: 0x060057F7 RID: 22519 RVA: 0x001B11F4 File Offset: 0x001AF3F4
		private void GetTiltVectorsForSideWithNext(Dreidel.Side side, out Vector2 sideTilt, out Vector2 nextSideTilt)
		{
			int num = (int)((side + 1) % Dreidel.Side.Count);
			if (side == Dreidel.Side.Hey || side == Dreidel.Side.Nun)
			{
				sideTilt = this.landingTiltValues[(int)side];
				nextSideTilt = this.landingTiltValues[num];
				nextSideTilt.x = sideTilt.x;
				return;
			}
			nextSideTilt = this.landingTiltValues[num];
			sideTilt = this.landingTiltValues[(int)side];
			sideTilt.x = nextSideTilt.x;
		}

		// Token: 0x060057F8 RID: 22520 RVA: 0x001B1270 File Offset: 0x001AF470
		private void AlignToSurfacePlane()
		{
			Vector3 vector = Vector3.forward;
			if (Vector3.Dot(Vector3.up, this.surfacePlaneNormal) < 0.9999f)
			{
				Vector3 vector2 = Vector3.Cross(this.surfacePlaneNormal, Vector3.up);
				vector = Quaternion.AngleAxis(90f, vector2) * this.surfacePlaneNormal;
			}
			Quaternion quaternion = Quaternion.LookRotation(vector, this.surfacePlaneNormal);
			base.transform.position = this.surfacePlanePoint;
			base.transform.rotation = quaternion;
		}

		// Token: 0x060057F9 RID: 22521 RVA: 0x001B12EC File Offset: 0x001AF4EC
		private void UpdateSpinTransform()
		{
			Vector3 position = this.spinTransform.position;
			Vector3 groundContactPoint = this.GetGroundContactPoint();
			Vector3 vector = this.groundPointSpring.TrackDampingRatio(groundContactPoint, this.groundTrackingFrequency, this.groundTrackingDampingRatio, Time.deltaTime);
			Vector3 vector2 = this.spinTransform.TransformPoint(vector);
			Quaternion quaternion = Quaternion.AngleAxis(90f * this.tiltLeftRight, Vector3.forward) * Quaternion.AngleAxis(90f * this.tiltFrontBack, Vector3.right);
			this.spinAxis = base.transform.InverseTransformDirection(base.transform.up);
			Quaternion quaternion2 = Quaternion.AngleAxis(this.spinAngle, this.spinAxis);
			this.spinTransform.localRotation = quaternion2 * quaternion;
			Vector3 vector3 = base.transform.InverseTransformVector(Vector3.Dot(position - vector2, base.transform.up) * base.transform.up);
			this.spinTransform.localPosition = vector3 + this.pathOffset;
			this.spinTransform.TransformPoint(this.bottomPointOffset);
		}

		// Token: 0x04005CCC RID: 23756
		[Header("References")]
		[SerializeField]
		private Transform spinTransform;

		// Token: 0x04005CCD RID: 23757
		[SerializeField]
		private MeshCollider dreidelCollider;

		// Token: 0x04005CCE RID: 23758
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04005CCF RID: 23759
		[SerializeField]
		private AudioClip spinLoopAudio;

		// Token: 0x04005CD0 RID: 23760
		[SerializeField]
		private AudioClip fallSound;

		// Token: 0x04005CD1 RID: 23761
		[SerializeField]
		private AudioClip gimelConfettiSound;

		// Token: 0x04005CD2 RID: 23762
		[SerializeField]
		private ParticleSystem gimelConfetti;

		// Token: 0x04005CD3 RID: 23763
		[Header("Offsets")]
		[SerializeField]
		private Vector3 centerOfMassOffset = Vector3.zero;

		// Token: 0x04005CD4 RID: 23764
		[SerializeField]
		private Vector3 bottomPointOffset = Vector3.zero;

		// Token: 0x04005CD5 RID: 23765
		[SerializeField]
		private Vector2 bodyRect = Vector2.one;

		// Token: 0x04005CD6 RID: 23766
		[SerializeField]
		private float confettiHeight = 0.125f;

		// Token: 0x04005CD7 RID: 23767
		[Header("Surface Detection")]
		[SerializeField]
		private float surfaceCheckDistance = 0.15f;

		// Token: 0x04005CD8 RID: 23768
		[SerializeField]
		private float surfaceUprightThreshold = 0.5f;

		// Token: 0x04005CD9 RID: 23769
		[SerializeField]
		private float surfaceDreidelAngleThreshold = 0.9f;

		// Token: 0x04005CDA RID: 23770
		[SerializeField]
		private LayerMask surfaceLayers;

		// Token: 0x04005CDB RID: 23771
		[Header("Spin Paramss")]
		[SerializeField]
		private float spinSpeedStart = 2f;

		// Token: 0x04005CDC RID: 23772
		[SerializeField]
		private float spinSpeedEnd = 1f;

		// Token: 0x04005CDD RID: 23773
		[SerializeField]
		private float spinTime = 10f;

		// Token: 0x04005CDE RID: 23774
		[SerializeField]
		private Vector2 spinTimeRange = new Vector2(7f, 12f);

		// Token: 0x04005CDF RID: 23775
		[SerializeField]
		private float spinWobbleFrequency = 0.1f;

		// Token: 0x04005CE0 RID: 23776
		[SerializeField]
		private float spinWobbleAmplitude = 0.01f;

		// Token: 0x04005CE1 RID: 23777
		[SerializeField]
		private float spinWobbleAmplitudeEndMin = 0.01f;

		// Token: 0x04005CE2 RID: 23778
		[SerializeField]
		private float tiltFrontBack;

		// Token: 0x04005CE3 RID: 23779
		[SerializeField]
		private float tiltLeftRight;

		// Token: 0x04005CE4 RID: 23780
		[SerializeField]
		private float groundTrackingDampingRatio = 0.9f;

		// Token: 0x04005CE5 RID: 23781
		[SerializeField]
		private float groundTrackingFrequency = 1f;

		// Token: 0x04005CE6 RID: 23782
		[Header("Motion Path")]
		[SerializeField]
		private float pathMoveSpeed = 0.1f;

		// Token: 0x04005CE7 RID: 23783
		[SerializeField]
		private float pathStartTurnRate = 360f;

		// Token: 0x04005CE8 RID: 23784
		[SerializeField]
		private float pathEndTurnRate = 90f;

		// Token: 0x04005CE9 RID: 23785
		[SerializeField]
		private float pathTurnRateSinOffset = 180f;

		// Token: 0x04005CEA RID: 23786
		[Header("Falling Params")]
		[SerializeField]
		private float spinSpeedStopRate = 1f;

		// Token: 0x04005CEB RID: 23787
		[SerializeField]
		private float tumbleFallDampingRatio = 0.4f;

		// Token: 0x04005CEC RID: 23788
		[SerializeField]
		private float tumbleFallFrequency = 6f;

		// Token: 0x04005CED RID: 23789
		[SerializeField]
		private float tumbleFallFrontBackDampingRatio = 0.4f;

		// Token: 0x04005CEE RID: 23790
		[SerializeField]
		private float tumbleFallFrontBackFrequency = 6f;

		// Token: 0x04005CEF RID: 23791
		[SerializeField]
		private float smoothFallDampingRatio = 0.9f;

		// Token: 0x04005CF0 RID: 23792
		[SerializeField]
		private float smoothFallFrequency = 2f;

		// Token: 0x04005CF1 RID: 23793
		[SerializeField]
		private float slowTurnDampingRatio = 0.9f;

		// Token: 0x04005CF2 RID: 23794
		[SerializeField]
		private float slowTurnFrequency = 2f;

		// Token: 0x04005CF3 RID: 23795
		[SerializeField]
		private float bounceFallSwitchTime = 0.5f;

		// Token: 0x04005CF4 RID: 23796
		[SerializeField]
		private float slowTurnSwitchTime = 0.5f;

		// Token: 0x04005CF5 RID: 23797
		[SerializeField]
		private float respawnTimeAfterLanding = 3f;

		// Token: 0x04005CF6 RID: 23798
		[SerializeField]
		private float fallTimeTumble = 3f;

		// Token: 0x04005CF7 RID: 23799
		[SerializeField]
		private float fallTimeSlowTurn = 5f;

		// Token: 0x04005CF8 RID: 23800
		private Dreidel.State state;

		// Token: 0x04005CF9 RID: 23801
		private double stateStartTime;

		// Token: 0x04005CFA RID: 23802
		private float spinSpeed;

		// Token: 0x04005CFB RID: 23803
		private float spinAngle;

		// Token: 0x04005CFC RID: 23804
		private Vector3 spinAxis = Vector3.up;

		// Token: 0x04005CFD RID: 23805
		private bool canStartSpin;

		// Token: 0x04005CFE RID: 23806
		private double spinStartTime = -1.0;

		// Token: 0x04005CFF RID: 23807
		private float tiltWobble;

		// Token: 0x04005D00 RID: 23808
		private bool falseTargetReached;

		// Token: 0x04005D01 RID: 23809
		private bool hasLanded;

		// Token: 0x04005D02 RID: 23810
		private Vector3 pathOffset = Vector3.zero;

		// Token: 0x04005D03 RID: 23811
		private Vector3 pathDir = Vector3.forward;

		// Token: 0x04005D04 RID: 23812
		private Vector3 surfacePlanePoint;

		// Token: 0x04005D05 RID: 23813
		private Vector3 surfacePlaneNormal;

		// Token: 0x04005D06 RID: 23814
		private FloatSpring tiltFrontBackSpring;

		// Token: 0x04005D07 RID: 23815
		private FloatSpring tiltLeftRightSpring;

		// Token: 0x04005D08 RID: 23816
		private FloatSpring spinSpeedSpring;

		// Token: 0x04005D09 RID: 23817
		private Vector3Spring groundPointSpring;

		// Token: 0x04005D0A RID: 23818
		private Vector2[] landingTiltValues = new Vector2[]
		{
			new Vector2(1f, -1f),
			new Vector2(1f, 0f),
			new Vector2(-1f, 1f),
			new Vector2(-1f, 0f)
		};

		// Token: 0x04005D0B RID: 23819
		private Vector2 landingTiltLeadingTarget = Vector2.zero;

		// Token: 0x04005D0C RID: 23820
		private Vector2 landingTiltTarget = Vector2.zero;

		// Token: 0x04005D0D RID: 23821
		[Header("Debug Params")]
		[SerializeField]
		private Dreidel.Side landingSide;

		// Token: 0x04005D0E RID: 23822
		[SerializeField]
		private Dreidel.Variation landingVariation;

		// Token: 0x04005D0F RID: 23823
		[SerializeField]
		private bool spinCounterClockwise;

		// Token: 0x04005D10 RID: 23824
		[SerializeField]
		private bool debugDraw;

		// Token: 0x02000DDC RID: 3548
		private enum State
		{
			// Token: 0x04005D12 RID: 23826
			Idle,
			// Token: 0x04005D13 RID: 23827
			FindingSurface,
			// Token: 0x04005D14 RID: 23828
			Spinning,
			// Token: 0x04005D15 RID: 23829
			Falling,
			// Token: 0x04005D16 RID: 23830
			Fallen
		}

		// Token: 0x02000DDD RID: 3549
		public enum Side
		{
			// Token: 0x04005D18 RID: 23832
			Shin,
			// Token: 0x04005D19 RID: 23833
			Hey,
			// Token: 0x04005D1A RID: 23834
			Gimel,
			// Token: 0x04005D1B RID: 23835
			Nun,
			// Token: 0x04005D1C RID: 23836
			Count
		}

		// Token: 0x02000DDE RID: 3550
		public enum Variation
		{
			// Token: 0x04005D1E RID: 23838
			Tumble,
			// Token: 0x04005D1F RID: 23839
			Smooth,
			// Token: 0x04005D20 RID: 23840
			Bounce,
			// Token: 0x04005D21 RID: 23841
			SlowTurn,
			// Token: 0x04005D22 RID: 23842
			FalseSlowTurn,
			// Token: 0x04005D23 RID: 23843
			Count
		}
	}
}
