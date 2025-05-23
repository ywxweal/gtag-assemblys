using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000B49 RID: 2889
	public class ObstacleEndLineTrigger : MonoBehaviour
	{
		// Token: 0x1400007C RID: 124
		// (add) Token: 0x0600474B RID: 18251 RVA: 0x001530E4 File Offset: 0x001512E4
		// (remove) Token: 0x0600474C RID: 18252 RVA: 0x0015311C File Offset: 0x0015131C
		public event ObstacleEndLineTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		// Token: 0x0600474D RID: 18253 RVA: 0x00153154 File Offset: 0x00151354
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
		// (Invoke) Token: 0x06004750 RID: 18256
		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
