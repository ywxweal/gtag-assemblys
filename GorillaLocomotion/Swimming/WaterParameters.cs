using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000CD2 RID: 3282
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WaterParameters", order = 1)]
	public class WaterParameters : ScriptableObject
	{
		// Token: 0x04005573 RID: 21875
		[Header("Splash Effect")]
		public bool playSplashEffect = true;

		// Token: 0x04005574 RID: 21876
		public GameObject splashEffect;

		// Token: 0x04005575 RID: 21877
		public float splashEffectScale = 1f;

		// Token: 0x04005576 RID: 21878
		public bool sendSplashEffectRPCs;

		// Token: 0x04005577 RID: 21879
		public float splashSpeedRequirement = 0.8f;

		// Token: 0x04005578 RID: 21880
		public float bigSplashSpeedRequirement = 1.9f;

		// Token: 0x04005579 RID: 21881
		public Gradient splashColorBySpeedGradient;

		// Token: 0x0400557A RID: 21882
		[Header("Ripple Effect")]
		public bool playRippleEffect = true;

		// Token: 0x0400557B RID: 21883
		public GameObject rippleEffect;

		// Token: 0x0400557C RID: 21884
		public float rippleEffectScale = 1f;

		// Token: 0x0400557D RID: 21885
		public float defaultDistanceBetweenRipples = 0.75f;

		// Token: 0x0400557E RID: 21886
		public float minDistanceBetweenRipples = 0.2f;

		// Token: 0x0400557F RID: 21887
		public float minTimeBetweenRipples = 0.75f;

		// Token: 0x04005580 RID: 21888
		public Color rippleSpriteColor = Color.white;

		// Token: 0x04005581 RID: 21889
		[Header("Drip Effect")]
		public bool playDripEffect = true;

		// Token: 0x04005582 RID: 21890
		public float postExitDripDuration = 1.5f;

		// Token: 0x04005583 RID: 21891
		public float perDripTimeDelay = 0.2f;

		// Token: 0x04005584 RID: 21892
		public float perDripTimeRandRange = 0.15f;

		// Token: 0x04005585 RID: 21893
		public float perDripDefaultRadius = 0.01f;

		// Token: 0x04005586 RID: 21894
		public float perDripRadiusRandRange = 0.01f;

		// Token: 0x04005587 RID: 21895
		[Header("Misc")]
		public float recomputeSurfaceForColliderDist = 0.2f;

		// Token: 0x04005588 RID: 21896
		public bool allowBubblesInVolume;
	}
}
