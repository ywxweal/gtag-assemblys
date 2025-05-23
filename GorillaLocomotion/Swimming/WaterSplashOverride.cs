using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02000CD3 RID: 3283
	public class WaterSplashOverride : MonoBehaviour
	{
		// Token: 0x0400558A RID: 21898
		public bool suppressWaterEffects;

		// Token: 0x0400558B RID: 21899
		public bool playBigSplash;

		// Token: 0x0400558C RID: 21900
		public bool playDrippingEffect = true;

		// Token: 0x0400558D RID: 21901
		public bool scaleByPlayersScale;

		// Token: 0x0400558E RID: 21902
		public bool overrideBoundingRadius;

		// Token: 0x0400558F RID: 21903
		public float boundingRadiusOverride = 1f;
	}
}
