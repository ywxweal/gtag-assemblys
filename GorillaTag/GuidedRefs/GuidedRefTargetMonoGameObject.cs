using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D66 RID: 3430
	public class GuidedRefTargetMonoGameObject : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000890 RID: 2192
		// (get) Token: 0x060055A6 RID: 21926 RVA: 0x001A1686 File Offset: 0x0019F886
		// (set) Token: 0x060055A7 RID: 21927 RVA: 0x001A168E File Offset: 0x0019F88E
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

		// Token: 0x17000891 RID: 2193
		// (get) Token: 0x060055A8 RID: 21928 RVA: 0x00013963 File Offset: 0x00011B63
		public Object GuidedRefTargetObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x060055A9 RID: 21929 RVA: 0x000BC86C File Offset: 0x000BAA6C
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x060055AA RID: 21930 RVA: 0x001A1697 File Offset: 0x0019F897
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoGameObject>(this, true);
		}

		// Token: 0x060055AB RID: 21931 RVA: 0x001A16A0 File Offset: 0x0019F8A0
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoGameObject>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x060055AD RID: 21933 RVA: 0x00045F89 File Offset: 0x00044189
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x060055AE RID: 21934 RVA: 0x00017401 File Offset: 0x00015601
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040058F9 RID: 22777
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
