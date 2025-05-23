using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AD9 RID: 2777
	public class BuilderAttachPoint : MonoBehaviour
	{
		// Token: 0x06004331 RID: 17201 RVA: 0x001368D1 File Offset: 0x00134AD1
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x040045B3 RID: 17843
		public Transform center;
	}
}
