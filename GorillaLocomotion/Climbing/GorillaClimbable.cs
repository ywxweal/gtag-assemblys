using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02000CED RID: 3309
	public class GorillaClimbable : MonoBehaviour
	{
		// Token: 0x06005223 RID: 21027 RVA: 0x0018FA6F File Offset: 0x0018DC6F
		private void Awake()
		{
			this.colliderCache = base.GetComponent<Collider>();
		}

		// Token: 0x0400564E RID: 22094
		public bool snapX;

		// Token: 0x0400564F RID: 22095
		public bool snapY;

		// Token: 0x04005650 RID: 22096
		public bool snapZ;

		// Token: 0x04005651 RID: 22097
		public float maxDistanceSnap = 0.05f;

		// Token: 0x04005652 RID: 22098
		public AudioClip clip;

		// Token: 0x04005653 RID: 22099
		public AudioClip clipOnFullRelease;

		// Token: 0x04005654 RID: 22100
		public Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb;

		// Token: 0x04005655 RID: 22101
		public bool climbOnlyWhileSmall;

		// Token: 0x04005656 RID: 22102
		public bool IsPlayerAttached;

		// Token: 0x04005657 RID: 22103
		[NonSerialized]
		public bool isBeingClimbed;

		// Token: 0x04005658 RID: 22104
		[NonSerialized]
		public Collider colliderCache;
	}
}
