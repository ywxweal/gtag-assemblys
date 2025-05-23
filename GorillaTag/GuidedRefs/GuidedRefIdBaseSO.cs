using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D60 RID: 3424
	public abstract class GuidedRefIdBaseSO : ScriptableObject, IGuidedRefObject
	{
		// Token: 0x06005594 RID: 21908 RVA: 0x000023F4 File Offset: 0x000005F4
		public virtual void GuidedRefInitialize()
		{
		}

		// Token: 0x06005596 RID: 21910 RVA: 0x00017401 File Offset: 0x00015601
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}
	}
}
