using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D60 RID: 3424
	public abstract class GuidedRefIdBaseSO : ScriptableObject, IGuidedRefObject
	{
		// Token: 0x06005595 RID: 21909 RVA: 0x000023F4 File Offset: 0x000005F4
		public virtual void GuidedRefInitialize()
		{
		}

		// Token: 0x06005597 RID: 21911 RVA: 0x00017401 File Offset: 0x00015601
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}
	}
}
