using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AD9 RID: 2777
	public class BuilderAttachPoint : MonoBehaviour
	{
		// Token: 0x06004332 RID: 17202 RVA: 0x001369A9 File Offset: 0x00134BA9
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x040045B4 RID: 17844
		public Transform center;
	}
}
