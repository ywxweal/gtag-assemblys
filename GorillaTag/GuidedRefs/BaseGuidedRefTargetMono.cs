using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D59 RID: 3417
	public abstract class BaseGuidedRefTargetMono : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x0600556D RID: 21869 RVA: 0x000BC84C File Offset: 0x000BAA4C
		protected virtual void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x0600556E RID: 21870 RVA: 0x001A037E File Offset: 0x0019E57E
		protected virtual void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<BaseGuidedRefTargetMono>(this, true);
		}

		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x0600556F RID: 21871 RVA: 0x001A0387 File Offset: 0x0019E587
		// (set) Token: 0x06005570 RID: 21872 RVA: 0x001A038F File Offset: 0x0019E58F
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
		// (get) Token: 0x06005571 RID: 21873 RVA: 0x000430AE File Offset: 0x000412AE
		Object IGuidedRefTargetMono.GuidedRefTargetObject
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06005572 RID: 21874 RVA: 0x001A0398 File Offset: 0x0019E598
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<BaseGuidedRefTargetMono>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x06005574 RID: 21876 RVA: 0x00045F89 File Offset: 0x00044189
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06005575 RID: 21877 RVA: 0x00017401 File Offset: 0x00015601
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040058DC RID: 22748
		public GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
