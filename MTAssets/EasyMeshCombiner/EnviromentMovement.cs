using System;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000C9F RID: 3231
	public class EnviromentMovement : MonoBehaviour
	{
		// Token: 0x0600501B RID: 20507 RVA: 0x0017D8CB File Offset: 0x0017BACB
		private void Start()
		{
			this.thisTransform = base.gameObject.GetComponent<Transform>();
			this.nextPosition = this.pos1;
		}

		// Token: 0x0600501C RID: 20508 RVA: 0x0017D8EC File Offset: 0x0017BAEC
		private void Update()
		{
			if (Vector3.Distance(this.thisTransform.position, this.nextPosition) > 0.5f)
			{
				base.transform.position = Vector3.Lerp(this.thisTransform.position, this.nextPosition, 2f * Time.deltaTime);
				return;
			}
			if (this.nextPosition == this.pos1)
			{
				this.nextPosition = this.pos2;
				return;
			}
			if (this.nextPosition == this.pos2)
			{
				this.nextPosition = this.pos1;
				return;
			}
		}

		// Token: 0x0400530D RID: 21261
		private Vector3 nextPosition = Vector3.zero;

		// Token: 0x0400530E RID: 21262
		private Transform thisTransform;

		// Token: 0x0400530F RID: 21263
		public Vector3 pos1;

		// Token: 0x04005310 RID: 21264
		public Vector3 pos2;
	}
}
