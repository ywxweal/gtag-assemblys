using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BCA RID: 3018
	public class GrabManager : MonoBehaviour
	{
		// Token: 0x06004A8A RID: 19082 RVA: 0x001632AC File Offset: 0x001614AC
		private void OnTriggerEnter(Collider otherCollider)
		{
			DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
			if (componentInChildren)
			{
				componentInChildren.InRange = true;
			}
		}

		// Token: 0x06004A8B RID: 19083 RVA: 0x001632D0 File Offset: 0x001614D0
		private void OnTriggerExit(Collider otherCollider)
		{
			DistanceGrabbable componentInChildren = otherCollider.GetComponentInChildren<DistanceGrabbable>();
			if (componentInChildren)
			{
				componentInChildren.InRange = false;
			}
		}

		// Token: 0x04004D4A RID: 19786
		private Collider m_grabVolume;

		// Token: 0x04004D4B RID: 19787
		public Color OutlineColorInRange;

		// Token: 0x04004D4C RID: 19788
		public Color OutlineColorHighlighted;

		// Token: 0x04004D4D RID: 19789
		public Color OutlineColorOutOfRange;
	}
}
