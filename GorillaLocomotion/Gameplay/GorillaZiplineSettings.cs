using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000CE1 RID: 3297
	[CreateAssetMenu(fileName = "GorillaZiplineSettings", menuName = "ScriptableObjects/GorillaZiplineSettings", order = 0)]
	public class GorillaZiplineSettings : ScriptableObject
	{
		// Token: 0x040055F0 RID: 22000
		public float minSlidePitch = 0.5f;

		// Token: 0x040055F1 RID: 22001
		public float maxSlidePitch = 1f;

		// Token: 0x040055F2 RID: 22002
		public float minSlideVolume;

		// Token: 0x040055F3 RID: 22003
		public float maxSlideVolume = 0.2f;

		// Token: 0x040055F4 RID: 22004
		public float maxSpeed = 10f;

		// Token: 0x040055F5 RID: 22005
		public float gravityMulti = 1.1f;

		// Token: 0x040055F6 RID: 22006
		[Header("Friction")]
		public float friction = 0.25f;

		// Token: 0x040055F7 RID: 22007
		public float maxFriction = 1f;

		// Token: 0x040055F8 RID: 22008
		public float maxFrictionSpeed = 15f;
	}
}
