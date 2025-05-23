using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BC8 RID: 3016
	public class GrabbableCrosshair : MonoBehaviour
	{
		// Token: 0x06004A86 RID: 19078 RVA: 0x0016320F File Offset: 0x0016140F
		private void Start()
		{
			this.m_centerEyeAnchor = GameObject.Find("CenterEyeAnchor").transform;
		}

		// Token: 0x06004A87 RID: 19079 RVA: 0x00163228 File Offset: 0x00161428
		public void SetState(GrabbableCrosshair.CrosshairState cs)
		{
			this.m_state = cs;
			if (cs == GrabbableCrosshair.CrosshairState.Disabled)
			{
				this.m_targetedCrosshair.SetActive(false);
				this.m_enabledCrosshair.SetActive(false);
				return;
			}
			if (cs == GrabbableCrosshair.CrosshairState.Enabled)
			{
				this.m_targetedCrosshair.SetActive(false);
				this.m_enabledCrosshair.SetActive(true);
				return;
			}
			if (cs == GrabbableCrosshair.CrosshairState.Targeted)
			{
				this.m_targetedCrosshair.SetActive(true);
				this.m_enabledCrosshair.SetActive(false);
			}
		}

		// Token: 0x06004A88 RID: 19080 RVA: 0x00163291 File Offset: 0x00161491
		private void Update()
		{
			if (this.m_state != GrabbableCrosshair.CrosshairState.Disabled)
			{
				base.transform.LookAt(this.m_centerEyeAnchor);
			}
		}

		// Token: 0x04004D42 RID: 19778
		private GrabbableCrosshair.CrosshairState m_state;

		// Token: 0x04004D43 RID: 19779
		private Transform m_centerEyeAnchor;

		// Token: 0x04004D44 RID: 19780
		[SerializeField]
		private GameObject m_targetedCrosshair;

		// Token: 0x04004D45 RID: 19781
		[SerializeField]
		private GameObject m_enabledCrosshair;

		// Token: 0x02000BC9 RID: 3017
		public enum CrosshairState
		{
			// Token: 0x04004D47 RID: 19783
			Disabled,
			// Token: 0x04004D48 RID: 19784
			Enabled,
			// Token: 0x04004D49 RID: 19785
			Targeted
		}
	}
}
