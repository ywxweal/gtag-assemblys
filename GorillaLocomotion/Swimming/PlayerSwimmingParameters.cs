using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000CCA RID: 3274
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PlayerSwimmingParameters", order = 1)]
	public class PlayerSwimmingParameters : ScriptableObject
	{
		// Token: 0x040054F9 RID: 21753
		[Header("Base Settings")]
		public float floatingWaterLevelBelowHead = 0.6f;

		// Token: 0x040054FA RID: 21754
		public float buoyancyFadeDist = 0.3f;

		// Token: 0x040054FB RID: 21755
		public bool extendBouyancyFromSpeed;

		// Token: 0x040054FC RID: 21756
		public float buoyancyExtensionDecayHalflife = 0.2f;

		// Token: 0x040054FD RID: 21757
		public float baseUnderWaterDampingHalfLife = 0.25f;

		// Token: 0x040054FE RID: 21758
		public float swimUnderWaterDampingHalfLife = 1.1f;

		// Token: 0x040054FF RID: 21759
		public AnimationCurve speedToBouyancyExtension = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04005500 RID: 21760
		public Vector2 speedToBouyancyExtensionMinMax = Vector2.zero;

		// Token: 0x04005501 RID: 21761
		public float swimmingVelocityOutOfWaterDrainRate = 3f;

		// Token: 0x04005502 RID: 21762
		[Range(0f, 1f)]
		public float underwaterJumpsAsSwimVelocityFactor = 1f;

		// Token: 0x04005503 RID: 21763
		[Range(0f, 1f)]
		public float swimmingHapticsStrength = 0.5f;

		// Token: 0x04005504 RID: 21764
		[Header("Surface Jumping")]
		public bool allowWaterSurfaceJumps;

		// Token: 0x04005505 RID: 21765
		public float waterSurfaceJumpHandSpeedThreshold = 1f;

		// Token: 0x04005506 RID: 21766
		public float waterSurfaceJumpAmount;

		// Token: 0x04005507 RID: 21767
		public float waterSurfaceJumpMaxSpeed = 1f;

		// Token: 0x04005508 RID: 21768
		public AnimationCurve waterSurfaceJumpPalmFacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04005509 RID: 21769
		public AnimationCurve waterSurfaceJumpHandVelocityFacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400550A RID: 21770
		[Header("Diving")]
		public bool applyDiveSteering;

		// Token: 0x0400550B RID: 21771
		public bool applyDiveDampingMultiplier;

		// Token: 0x0400550C RID: 21772
		public float diveDampingMultiplier = 1f;

		// Token: 0x0400550D RID: 21773
		[Tooltip("In degrees")]
		public float maxDiveSteerAnglePerStep = 1f;

		// Token: 0x0400550E RID: 21774
		public float diveVelocityAveragingWindow = 0.1f;

		// Token: 0x0400550F RID: 21775
		public bool applyDiveSwimVelocityConversion;

		// Token: 0x04005510 RID: 21776
		[Tooltip("In meters per second")]
		public float diveSwimVelocityConversionRate = 3f;

		// Token: 0x04005511 RID: 21777
		public float diveMaxSwimVelocityConversion = 3f;

		// Token: 0x04005512 RID: 21778
		public bool reduceDiveSteeringBelowVelocityPlane;

		// Token: 0x04005513 RID: 21779
		public float reduceDiveSteeringBelowPlaneFadeStartDist = 0.4f;

		// Token: 0x04005514 RID: 21780
		public float reduceDiveSteeringBelowPlaneFadeEndDist = 0.55f;

		// Token: 0x04005515 RID: 21781
		public AnimationCurve palmFacingToRedirectAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04005516 RID: 21782
		public Vector2 palmFacingToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x04005517 RID: 21783
		public AnimationCurve swimSpeedToRedirectAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04005518 RID: 21784
		public Vector2 swimSpeedToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x04005519 RID: 21785
		public AnimationCurve swimSpeedToMaxRedirectAngle = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x0400551A RID: 21786
		public Vector2 swimSpeedToMaxRedirectAngleMinMax = Vector2.zero;

		// Token: 0x0400551B RID: 21787
		public AnimationCurve handSpeedToRedirectAmount = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x0400551C RID: 21788
		public Vector2 handSpeedToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x0400551D RID: 21789
		public AnimationCurve handAccelToRedirectAmount = AnimationCurve.Linear(0f, 1f, 1f, 0f);

		// Token: 0x0400551E RID: 21790
		public Vector2 handAccelToRedirectAmountMinMax = Vector2.zero;

		// Token: 0x0400551F RID: 21791
		public AnimationCurve nonDiveDampingHapticsAmount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04005520 RID: 21792
		public Vector2 nonDiveDampingHapticsAmountMinMax = Vector2.zero;
	}
}
