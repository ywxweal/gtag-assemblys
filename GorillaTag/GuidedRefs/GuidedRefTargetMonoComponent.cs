using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D65 RID: 3429
	public class GuidedRefTargetMonoComponent : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x0600559C RID: 21916 RVA: 0x001A1578 File Offset: 0x0019F778
		// (set) Token: 0x0600559D RID: 21917 RVA: 0x001A1580 File Offset: 0x0019F780
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

		// Token: 0x1700088F RID: 2191
		// (get) Token: 0x0600559E RID: 21918 RVA: 0x001A1589 File Offset: 0x0019F789
		public Object GuidedRefTargetObject
		{
			get
			{
				return this.targetComponent;
			}
		}

		// Token: 0x0600559F RID: 21919 RVA: 0x000BC84C File Offset: 0x000BAA4C
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x060055A0 RID: 21920 RVA: 0x001A1591 File Offset: 0x0019F791
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoComponent>(this, true);
		}

		// Token: 0x060055A1 RID: 21921 RVA: 0x001A159A File Offset: 0x0019F79A
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoComponent>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x060055A3 RID: 21923 RVA: 0x00045F89 File Offset: 0x00044189
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x060055A4 RID: 21924 RVA: 0x00017401 File Offset: 0x00015601
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040058F6 RID: 22774
		[SerializeField]
		private Component targetComponent;

		// Token: 0x040058F7 RID: 22775
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
