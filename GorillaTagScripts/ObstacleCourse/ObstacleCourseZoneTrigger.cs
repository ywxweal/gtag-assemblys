using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000B47 RID: 2887
	public class ObstacleCourseZoneTrigger : MonoBehaviour
	{
		// Token: 0x1400007A RID: 122
		// (add) Token: 0x06004741 RID: 18241 RVA: 0x0015306C File Offset: 0x0015126C
		// (remove) Token: 0x06004742 RID: 18242 RVA: 0x001530A4 File Offset: 0x001512A4
		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		// Token: 0x1400007B RID: 123
		// (add) Token: 0x06004743 RID: 18243 RVA: 0x001530DC File Offset: 0x001512DC
		// (remove) Token: 0x06004744 RID: 18244 RVA: 0x00153114 File Offset: 0x00151314
		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerExit;

		// Token: 0x06004745 RID: 18245 RVA: 0x00153149 File Offset: 0x00151349
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

		// Token: 0x06004746 RID: 18246 RVA: 0x00153181 File Offset: 0x00151381
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

		// Token: 0x040049A6 RID: 18854
		public LayerMask bodyLayer;

		// Token: 0x02000B48 RID: 2888
		// (Invoke) Token: 0x06004749 RID: 18249
		public delegate void ObstacleCourseTriggerEvent(Collider collider);
	}
}
