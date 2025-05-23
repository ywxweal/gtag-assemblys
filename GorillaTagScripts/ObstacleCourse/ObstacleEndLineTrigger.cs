using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000B49 RID: 2889
	public class ObstacleEndLineTrigger : MonoBehaviour
	{
		// Token: 0x1400007C RID: 124
		// (add) Token: 0x0600474C RID: 18252 RVA: 0x001531BC File Offset: 0x001513BC
		// (remove) Token: 0x0600474D RID: 18253 RVA: 0x001531F4 File Offset: 0x001513F4
		public event ObstacleEndLineTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		// Token: 0x0600474E RID: 18254 RVA: 0x0015322C File Offset: 0x0015142C
		private void OnTriggerEnter(Collider other)
		{
			VRRig vrrig;
			if (other.attachedRigidbody.gameObject.TryGetComponent<VRRig>(out vrrig))
			{
				ObstacleEndLineTrigger.ObstacleCourseTriggerEvent onPlayerTriggerEnter = this.OnPlayerTriggerEnter;
				if (onPlayerTriggerEnter == null)
				{
					return;
				}
				onPlayerTriggerEnter(vrrig);
			}
		}

		// Token: 0x02000B4A RID: 2890
		// (Invoke) Token: 0x06004751 RID: 18257
		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
