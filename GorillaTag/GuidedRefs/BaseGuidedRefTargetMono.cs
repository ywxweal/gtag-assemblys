using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D59 RID: 3417
	public abstract class BaseGuidedRefTargetMono : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x0600556E RID: 21870 RVA: 0x000BC86C File Offset: 0x000BAA6C
		protected virtual void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x0600556F RID: 21871 RVA: 0x001A0456 File Offset: 0x0019E656
		protected virtual void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<BaseGuidedRefTargetMono>(this, true);
		}

		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x06005570 RID: 21872 RVA: 0x001A045F File Offset: 0x0019E65F
		// (set) Token: 0x06005571 RID: 21873 RVA: 0x001A0467 File Offset: 0x0019E667
		GuidedRefBasicTargetInfo IGuidedRefTargetMono.GRefTargetInfo
		{
			get
			{
				return this.guidedRefTargetInfo;
			}
			set
			{
				this.guidedRefTargetInfo = value;
			}
		}

		// Token: 0x1700088C RID: 2188
		// (get) Token: 0x06005572 RID: 21874 RVA: 0x000430AE File Offset: 0x000412AE
		Object IGuidedRefTargetMono.GuidedRefTargetObject
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06005573 RID: 21875 RVA: 0x001A0470 File Offset: 0x0019E670
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<BaseGuidedRefTargetMono>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x06005575 RID: 21877 RVA: 0x00045F89 File Offset: 0x00044189
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06005576 RID: 21878 RVA: 0x00017401 File Offset: 0x00015601
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040058DD RID: 22749
		public GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
