using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D67 RID: 3431
	public class GuidedRefTargetMonoTransform : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000892 RID: 2194
		// (get) Token: 0x060055AE RID: 21934 RVA: 0x001A15DC File Offset: 0x0019F7DC
		// (set) Token: 0x060055AF RID: 21935 RVA: 0x001A15E4 File Offset: 0x0019F7E4
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

		// Token: 0x17000893 RID: 2195
		// (get) Token: 0x060055B0 RID: 21936 RVA: 0x00045F89 File Offset: 0x00044189
		public Object GuidedRefTargetObject
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x060055B1 RID: 21937 RVA: 0x000BC84C File Offset: 0x000BAA4C
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x060055B2 RID: 21938 RVA: 0x001A15ED File Offset: 0x0019F7ED
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoTransform>(this, true);
		}

		// Token: 0x060055B3 RID: 21939 RVA: 0x001A15F6 File Offset: 0x0019F7F6
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoTransform>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x060055B5 RID: 21941 RVA: 0x00045F89 File Offset: 0x00044189
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x060055B6 RID: 21942 RVA: 0x00017401 File Offset: 0x00015601
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040058F9 RID: 22777
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
