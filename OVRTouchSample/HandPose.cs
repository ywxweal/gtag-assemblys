using System;
using UnityEngine;

namespace OVRTouchSample
{
	// Token: 0x02000C09 RID: 3081
	public class HandPose : MonoBehaviour
	{
		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x06004C23 RID: 19491 RVA: 0x00168C86 File Offset: 0x00166E86
		public bool AllowPointing
		{
			get
			{
				return this.m_allowPointing;
			}
		}

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x06004C24 RID: 19492 RVA: 0x00168C8E File Offset: 0x00166E8E
		public bool AllowThumbsUp
		{
			get
			{
				return this.m_allowThumbsUp;
			}
		}

		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x06004C25 RID: 19493 RVA: 0x00168C96 File Offset: 0x00166E96
		public HandPoseId PoseId
		{
			get
			{
				return this.m_poseId;
			}
		}

		// Token: 0x04004EF6 RID: 20214
		[SerializeField]
		private bool m_allowPointing;

		// Token: 0x04004EF7 RID: 20215
		[SerializeField]
		private bool m_allowThumbsUp;

		// Token: 0x04004EF8 RID: 20216
		[SerializeField]
		private HandPoseId m_poseId;
	}
}
