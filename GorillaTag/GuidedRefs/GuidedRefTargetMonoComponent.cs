using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D65 RID: 3429
	public class GuidedRefTargetMonoComponent : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x1700088E RID: 2190
		// (get) Token: 0x0600559D RID: 21917 RVA: 0x001A1650 File Offset: 0x0019F850
		// (set) Token: 0x0600559E RID: 21918 RVA: 0x001A1658 File Offset: 0x0019F858
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
		// (get) Token: 0x0600559F RID: 21919 RVA: 0x001A1661 File Offset: 0x0019F861
		public Object GuidedRefTargetObject
		{
			get
			{
				return this.targetComponent;
			}
		}

		// Token: 0x060055A0 RID: 21920 RVA: 0x000BC86C File Offset: 0x000BAA6C
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x060055A1 RID: 21921 RVA: 0x001A1669 File Offset: 0x0019F869
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoComponent>(this, true);
		}

		// Token: 0x060055A2 RID: 21922 RVA: 0x001A1672 File Offset: 0x0019F872
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoComponent>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x060055A4 RID: 21924 RVA: 0x00045F89 File Offset: 0x00044189
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x060055A5 RID: 21925 RVA: 0x00017401 File Offset: 0x00015601
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040058F7 RID: 22775
		[SerializeField]
		private Component targetComponent;

		// Token: 0x040058F8 RID: 22776
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
