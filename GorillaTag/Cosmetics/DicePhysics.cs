using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000DB2 RID: 3506
	public class DicePhysics : MonoBehaviour
	{
		// Token: 0x060056D7 RID: 22231 RVA: 0x001A795C File Offset: 0x001A5B5C
		public int GetRandomSide()
		{
			DicePhysics.DiceType diceType = this.diceType;
			if (diceType != DicePhysics.DiceType.D6)
			{
				if (this.forceLandingSide)
				{
					return Mathf.Clamp(this.forcedLandingSide, 1, 20);
				}
				int num;
				if (this.CheckCosmeticRollOverride(out num))
				{
					return Mathf.Clamp(num, 1, 20);
				}
				return Random.Range(1, 21);
			}
			else
			{
				if (this.forceLandingSide)
				{
					return Mathf.Clamp(this.forcedLandingSide, 1, 6);
				}
				int num2;
				if (this.CheckCosmeticRollOverride(out num2))
				{
					return Mathf.Clamp(num2, 1, 6);
				}
				return Random.Range(1, 7);
			}
		}

		// Token: 0x060056D8 RID: 22232 RVA: 0x001A79DC File Offset: 0x001A5BDC
		public Vector3 GetSideDirection(int side)
		{
			DicePhysics.DiceType diceType = this.diceType;
			if (diceType != DicePhysics.DiceType.D6)
			{
				int num = Mathf.Clamp(side - 1, 0, 19);
				return this.d20SideDirections[num];
			}
			int num2 = Mathf.Clamp(side - 1, 0, 5);
			return this.d6SideDirections[num2];
		}

		// Token: 0x060056D9 RID: 22233 RVA: 0x001A7A28 File Offset: 0x001A5C28
		public void StartThrow(DiceHoldable holdable, Vector3 startPosition, Vector3 velocity, float playerScale, int side, double startTime)
		{
			this.holdableParent = holdable;
			base.transform.parent = null;
			base.transform.position = startPosition;
			base.transform.localScale = Vector3.one * playerScale;
			this.rb.isKinematic = false;
			this.rb.useGravity = true;
			this.rb.velocity = velocity;
			if (!this.allowPickupFromGround && this.interactionPoint != null)
			{
				this.interactionPoint.enabled = false;
			}
			this.throwStartTime = ((startTime > 0.0) ? startTime : ((double)Time.time));
			this.throwSettledTime = -1.0;
			this.scale = playerScale;
			this.landingSide = Mathf.Clamp(side, 1, 20);
			this.prevVelocity = Vector3.zero;
			velocity = Vector3.zero;
			base.enabled = true;
		}

		// Token: 0x060056DA RID: 22234 RVA: 0x001A7B10 File Offset: 0x001A5D10
		public void EndThrow()
		{
			this.rb.isKinematic = true;
			this.rb.velocity = Vector3.zero;
			if (this.holdableParent != null)
			{
				base.transform.parent = this.holdableParent.transform;
			}
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
			this.scale = 1f;
			this.throwStartTime = -1.0;
			if (this.interactionPoint != null)
			{
				this.interactionPoint.enabled = true;
			}
			this.onRollFinished.Invoke();
			base.enabled = false;
		}

		// Token: 0x060056DB RID: 22235 RVA: 0x001A7BD4 File Offset: 0x001A5DD4
		private void FixedUpdate()
		{
			double num = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
			float num2 = (float)(num - this.throwStartTime);
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 0.1f * this.scale, this.surfaceLayers.value, QueryTriggerInteraction.Ignore))
			{
				Vector3 normal = raycastHit.normal;
				Vector3 sideDirection = this.GetSideDirection(this.landingSide);
				Vector3 vector = base.transform.rotation * sideDirection;
				Vector3 normalized = Vector3.Cross(vector, normal).normalized;
				float num3 = Vector3.SignedAngle(vector, normal, normalized);
				float num4 = Mathf.Sign(num3) * this.angleDeltaVsStrengthCurve.Evaluate(Mathf.Clamp01(Mathf.Abs(num3) / 180f));
				float num5 = this.landingTimeVsStrengthCurve.Evaluate(Mathf.Clamp01(num2 / this.landingTime));
				float magnitude = this.rb.velocity.magnitude;
				float num6 = Mathf.Clamp01(1f - Mathf.Min(magnitude, 1f));
				float num7 = Mathf.Max(num5, num6);
				Vector3 vector2 = this.strength * (num7 * num4 * normalized) - this.damping * this.rb.angularVelocity;
				this.rb.AddTorque(vector2, ForceMode.Acceleration);
				if (!this.rb.isKinematic && magnitude < 0.01f && num3 < 2f)
				{
					this.rb.isKinematic = true;
					this.throwSettledTime = num;
					this.InvokeLandingEffects(this.landingSide);
				}
				else if (!this.rb.isKinematic && num2 > this.landingTime)
				{
					this.rb.isKinematic = true;
					this.throwSettledTime = num;
					base.transform.rotation = Quaternion.FromToRotation(vector, normal) * base.transform.rotation;
					this.InvokeLandingEffects(this.landingSide);
				}
			}
			if (num2 > this.landingTime + this.postLandingTime || (this.rb.isKinematic && (float)(num - this.throwSettledTime) > this.postLandingTime))
			{
				this.EndThrow();
			}
			this.prevVelocity = this.velocity;
			this.velocity = this.rb.velocity;
		}

		// Token: 0x060056DC RID: 22236 RVA: 0x001A7E20 File Offset: 0x001A6020
		private void OnCollisionEnter(Collision collision)
		{
			float magnitude = collision.impulse.magnitude;
			if (magnitude > 0.001f)
			{
				Vector3 vector = Vector3.Reflect(this.prevVelocity, collision.impulse / magnitude);
				this.rb.velocity = vector * this.bounceAmplification;
			}
		}

		// Token: 0x060056DD RID: 22237 RVA: 0x001A7E74 File Offset: 0x001A6074
		private void InvokeLandingEffects(int side)
		{
			DicePhysics.DiceType diceType = this.diceType;
			if (diceType != DicePhysics.DiceType.D6)
			{
				if (side == 20)
				{
					this.onBestRoll.Invoke();
					return;
				}
				if (side == 1)
				{
					this.onWorstRoll.Invoke();
					return;
				}
			}
			else
			{
				if (side == 6)
				{
					this.onBestRoll.Invoke();
					return;
				}
				if (side == 1)
				{
					this.onWorstRoll.Invoke();
				}
			}
		}

		// Token: 0x060056DE RID: 22238 RVA: 0x001A7ED0 File Offset: 0x001A60D0
		private bool CheckCosmeticRollOverride(out int rollSide)
		{
			if (this.cosmeticRollOverrides.Length != 0)
			{
				if (this.cachedLocalRig == null)
				{
					RigContainer rigContainer;
					if (PhotonNetwork.InRoom && VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer) && rigContainer.Rig != null)
					{
						this.cachedLocalRig = rigContainer.Rig;
					}
					else
					{
						this.cachedLocalRig = GorillaTagger.Instance.offlineVRRig;
					}
				}
				if (this.cachedLocalRig != null)
				{
					int num = -1;
					for (int i = 0; i < this.cosmeticRollOverrides.Length; i++)
					{
						if (this.cosmeticRollOverrides[i].cosmeticName != null && this.cachedLocalRig.cosmeticSet != null && this.cachedLocalRig.cosmeticSet.HasItem(this.cosmeticRollOverrides[i].cosmeticName) && (!this.cosmeticRollOverrides[i].requireHolding || (EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment.name == this.cosmeticRollOverrides[i].cosmeticName) || (EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment.name == this.cosmeticRollOverrides[i].cosmeticName)) && this.cosmeticRollOverrides[i].landingSide > num)
						{
							num = this.cosmeticRollOverrides[i].landingSide;
						}
					}
					if (num > 0)
					{
						rollSide = num;
						return true;
					}
				}
			}
			rollSide = 0;
			return false;
		}

		// Token: 0x04005ACD RID: 23245
		[SerializeField]
		private DicePhysics.DiceType diceType = DicePhysics.DiceType.D20;

		// Token: 0x04005ACE RID: 23246
		[SerializeField]
		private float landingTime = 5f;

		// Token: 0x04005ACF RID: 23247
		[SerializeField]
		private float postLandingTime = 2f;

		// Token: 0x04005AD0 RID: 23248
		[SerializeField]
		private LayerMask surfaceLayers;

		// Token: 0x04005AD1 RID: 23249
		[SerializeField]
		private AnimationCurve angleDeltaVsStrengthCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04005AD2 RID: 23250
		[SerializeField]
		private AnimationCurve landingTimeVsStrengthCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04005AD3 RID: 23251
		[SerializeField]
		private float strength = 1f;

		// Token: 0x04005AD4 RID: 23252
		[SerializeField]
		private float damping = 0.5f;

		// Token: 0x04005AD5 RID: 23253
		[SerializeField]
		private bool forceLandingSide;

		// Token: 0x04005AD6 RID: 23254
		[SerializeField]
		private int forcedLandingSide = 20;

		// Token: 0x04005AD7 RID: 23255
		[SerializeField]
		private bool allowPickupFromGround = true;

		// Token: 0x04005AD8 RID: 23256
		[SerializeField]
		private float bounceAmplification = 1f;

		// Token: 0x04005AD9 RID: 23257
		[SerializeField]
		private DicePhysics.CosmeticRollOverride[] cosmeticRollOverrides;

		// Token: 0x04005ADA RID: 23258
		[SerializeField]
		private UnityEvent onBestRoll;

		// Token: 0x04005ADB RID: 23259
		[SerializeField]
		private UnityEvent onWorstRoll;

		// Token: 0x04005ADC RID: 23260
		[SerializeField]
		private UnityEvent onRollFinished;

		// Token: 0x04005ADD RID: 23261
		[SerializeField]
		private Rigidbody rb;

		// Token: 0x04005ADE RID: 23262
		[SerializeField]
		private InteractionPoint interactionPoint;

		// Token: 0x04005ADF RID: 23263
		private VRRig cachedLocalRig;

		// Token: 0x04005AE0 RID: 23264
		private DiceHoldable holdableParent;

		// Token: 0x04005AE1 RID: 23265
		private double throwStartTime = -1.0;

		// Token: 0x04005AE2 RID: 23266
		private double throwSettledTime = -1.0;

		// Token: 0x04005AE3 RID: 23267
		private int landingSide;

		// Token: 0x04005AE4 RID: 23268
		private float scale;

		// Token: 0x04005AE5 RID: 23269
		private Vector3 prevVelocity = Vector3.zero;

		// Token: 0x04005AE6 RID: 23270
		private Vector3 velocity = Vector3.zero;

		// Token: 0x04005AE7 RID: 23271
		private const float a = 38.833332f;

		// Token: 0x04005AE8 RID: 23272
		private const float b = 77.66666f;

		// Token: 0x04005AE9 RID: 23273
		private Vector3[] d20SideDirections = new Vector3[]
		{
			Quaternion.AngleAxis(144f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(324f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(288f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(180f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(252f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(108f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(72f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(36f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(216f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(0f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(180f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(324f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(144f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(108f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(72f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(288f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(0f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(252f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up,
			Quaternion.AngleAxis(216f, Vector3.up) * Quaternion.AngleAxis(38.833332f, Vector3.forward) * -Vector3.up,
			Quaternion.AngleAxis(36f, -Vector3.up) * Quaternion.AngleAxis(38.833332f, -Vector3.forward) * Vector3.up
		};

		// Token: 0x04005AEA RID: 23274
		private Vector3[] d6SideDirections = new Vector3[]
		{
			new Vector3(0f, -1f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 1f, 0f)
		};

		// Token: 0x02000DB3 RID: 3507
		private enum DiceType
		{
			// Token: 0x04005AEC RID: 23276
			D6,
			// Token: 0x04005AED RID: 23277
			D20
		}

		// Token: 0x02000DB4 RID: 3508
		[Serializable]
		private struct CosmeticRollOverride
		{
			// Token: 0x04005AEE RID: 23278
			public string cosmeticName;

			// Token: 0x04005AEF RID: 23279
			public int landingSide;

			// Token: 0x04005AF0 RID: 23280
			public bool requireHolding;
		}
	}
}
