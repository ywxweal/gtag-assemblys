using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AD6 RID: 2774
	public class BuilderAttachEdge : MonoBehaviour
	{
		// Token: 0x0600431D RID: 17181 RVA: 0x00135E86 File Offset: 0x00134086
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x0600431E RID: 17182 RVA: 0x00135EA4 File Offset: 0x001340A4
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Transform transform = this.center;
			if (transform == null)
			{
				transform = base.transform;
			}
			Vector3 vector = transform.rotation * Vector3.right;
			Gizmos.DrawLine(transform.position - vector * this.length * 0.5f, transform.position + vector * this.length * 0.5f);
		}

		// Token: 0x0400459B RID: 17819
		public Transform center;

		// Token: 0x0400459C RID: 17820
		public float length;
	}
}
