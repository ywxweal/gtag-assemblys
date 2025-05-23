using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x02000D66 RID: 3430
	public class GuidedRefTargetMonoGameObject : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000890 RID: 2192
		// (get) Token: 0x060055A5 RID: 21925 RVA: 0x001A15AE File Offset: 0x0019F7AE
		// (set) Token: 0x060055A6 RID: 21926 RVA: 0x001A15B6 File Offset: 0x0019F7B6
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
		// (get) Token: 0x060055A7 RID: 21927 RVA: 0x00013963 File Offset: 0x00011B63
		public Object GuidedRefTargetObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x060055A8 RID: 21928 RVA: 0x000BC84C File Offset: 0x000BAA4C
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x060055A9 RID: 21929 RVA: 0x001A15BF File Offset: 0x0019F7BF
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoGameObject>(this, true);
		}

		// Token: 0x060055AA RID: 21930 RVA: 0x001A15C8 File Offset: 0x0019F7C8
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoGameObject>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x060055AC RID: 21932 RVA: 0x00045F89 File Offset: 0x00044189
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x060055AD RID: 21933 RVA: 0x00017401 File Offset: 0x00015601
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040058F8 RID: 22776
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
