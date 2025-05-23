using System;
using UnityEngine;

namespace OVRTouchSample
{
	// Token: 0x02000C09 RID: 3081
	public class HandPose : MonoBehaviour
	{
		// Token: 0x1700078E RID: 1934
		// (get) Token: 0x06004C22 RID: 19490 RVA: 0x00168BAE File Offset: 0x00166DAE
		public bool AllowPointing
		{
			get
			{
				return this.m_allowPointing;
			}
		}

		// Token: 0x1700078F RID: 1935
		// (get) Token: 0x06004C23 RID: 19491 RVA: 0x00168BB6 File Offset: 0x00166DB6
		public bool AllowThumbsUp
		{
			get
			{
				return this.m_allowThumbsUp;
			}
		}

		// Token: 0x17000790 RID: 1936
		// (get) Token: 0x06004C24 RID: 19492 RVA: 0x00168BBE File Offset: 0x00166DBE
		public HandPoseId PoseId
		{
			get
			{
				return this.m_poseId;
			}
		}

		// Token: 0x04004EF5 RID: 20213
		[SerializeField]
		private bool m_allowPointing;

		// Token: 0x04004EF6 RID: 20214
		[SerializeField]
		private bool m_allowThumbsUp;

		// Token: 0x04004EF7 RID: 20215
		[SerializeField]
		private HandPoseId m_poseId;
	}
}
