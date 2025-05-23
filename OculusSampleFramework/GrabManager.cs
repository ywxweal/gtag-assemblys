using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BCA RID: 3018
	public class GrabManager : MonoBehaviour
	{
		// Token: 0x06004A8B RID: 19083 RVA: 0x00163384 File Offset: 0x00161584
		private void OnTriggerEnter(Collider otherCollider)
		{
			DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
			if (componentInChildren)
			{
				componentInChildren.InRange = true;
			}
		}

		// Token: 0x06004A8C RID: 19084 RVA: 0x001633A8 File Offset: 0x001615A8
		private void OnTriggerExit(Collider otherCollider)
		{
			DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
			if (componentInChildren)
			{
				componentInChildren.InRange = false;
			}
		}

		// Token: 0x04004D4B RID: 19787
		private Collider m_grabVolume;

		// Token: 0x04004D4C RID: 19788
		public Color OutlineColorInRange;

		// Token: 0x04004D4D RID: 19789
		public Color OutlineColorHighlighted;

		// Token: 0x04004D4E RID: 19790
		public Color OutlineColorOutOfRange;
	}
}
