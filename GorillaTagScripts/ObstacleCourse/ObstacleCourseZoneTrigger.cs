using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000B47 RID: 2887
	public class ObstacleCourseZoneTrigger : MonoBehaviour
	{
		// Token: 0x1400007A RID: 122
		// (add) Token: 0x06004740 RID: 18240 RVA: 0x00152F94 File Offset: 0x00151194
		// (remove) Token: 0x06004741 RID: 18241 RVA: 0x00152FCC File Offset: 0x001511CC
		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		// Token: 0x1400007B RID: 123
		// (add) Token: 0x06004742 RID: 18242 RVA: 0x00153004 File Offset: 0x00151204
		// (remove) Token: 0x06004743 RID: 18243 RVA: 0x0015303C File Offset: 0x0015123C
		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerExit;

		// Token: 0x06004744 RID: 18244 RVA: 0x00153071 File Offset: 0x00151271
		private void OnTriggerEnter(Collider other)
		{
			if (!other.GetComponent<SphereCollider>())
			{
				return;
			}
			if (other.attachedRigidbody.gameObject.CompareTag("GorillaPlayer"))
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent onPlayerTriggerEnter = this.OnPlayerTriggerEnter;
				if (onPlayerTriggerEnter == null)
				{
					return;
				}
				onPlayerTriggerEnter(other);
			}
		}

		// Token: 0x06004745 RID: 18245 RVA: 0x001530A9 File Offset: 0x001512A9
		private void OnTriggerExit(Collider other)
		{
			if (!other.GetComponent<SphereCollider>())
			{
				return;
			}
			if (other.attachedRigidbody.gameObject.CompareTag("GorillaPlayer"))
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent onPlayerTriggerExit = this.OnPlayerTriggerExit;
				if (onPlayerTriggerExit == null)
				{
					return;
				}
				onPlayerTriggerExit(other);
			}
		}

		// Token: 0x040049A5 RID: 18853
		public LayerMask bodyLayer;

		// Token: 0x02000B48 RID: 2888
		// (Invoke) Token: 0x06004748 RID: 18248
		public delegate void ObstacleCourseTriggerEvent(Collider collider);
	}
}
