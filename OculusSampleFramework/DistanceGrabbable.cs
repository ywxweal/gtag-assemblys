using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BC6 RID: 3014
	public class DistanceGrabbable : OVRGrabbable
	{
		// Token: 0x17000731 RID: 1841
		// (get) Token: 0x06004A74 RID: 19060 RVA: 0x001628FB File Offset: 0x00160AFB
		// (set) Token: 0x06004A75 RID: 19061 RVA: 0x00162903 File Offset: 0x00160B03
		public bool InRange
		{
			get
			{
				return this.m_inRange;
			}
			set
			{
				this.m_inRange = value;
				this.RefreshCrosshair();
			}
		}

		// Token: 0x17000732 RID: 1842
		// (get) Token: 0x06004A76 RID: 19062 RVA: 0x00162912 File Offset: 0x00160B12
		// (set) Token: 0x06004A77 RID: 19063 RVA: 0x0016291A File Offset: 0x00160B1A
		public bool Targeted
		{
			get
			{
				return this.m_targeted;
			}
			set
			{
				this.m_targeted = value;
				this.RefreshCrosshair();
			}
		}

		// Token: 0x06004A78 RID: 19064 RVA: 0x0016292C File Offset: 0x00160B2C
		protected override void Start()
		{
			base.Start();
			this.m_crosshair = base.gameObject.GetComponentInChildren<GrabbableCrosshair>();
			this.m_renderer = base.gameObject.GetComponent<Renderer>();
			this.m_crosshairManager = Object.FindObjectOfType<GrabManager>();
			this.m_mpb = new MaterialPropertyBlock();
			this.RefreshCrosshair();
			this.m_renderer.SetPropertyBlock(this.m_mpb);
		}

		// Token: 0x06004A79 RID: 19065 RVA: 0x00162990 File Offset: 0x00160B90
		private void RefreshCrosshair()
		{
			if (this.m_crosshair)
			{
				if (base.isGrabbed)
				{
					this.m_crosshair.SetState(GrabbableCrosshair.CrosshairState.Disabled);
				}
				else if (!this.InRange)
				{
					this.m_crosshair.SetState(GrabbableCrosshair.CrosshairState.Disabled);
				}
				else
				{
					this.m_crosshair.SetState(this.Targeted ? GrabbableCrosshair.CrosshairState.Targeted : GrabbableCrosshair.CrosshairState.Enabled);
				}
			}
			if (this.m_materialColorField != null)
			{
				this.m_renderer.GetPropertyBlock(this.m_mpb);
				if (base.isGrabbed || !this.InRange)
				{
					this.m_mpb.SetColor(this.m_materialColorField, this.m_crosshairManager.OutlineColorOutOfRange);
				}
				else if (this.Targeted)
				{
					this.m_mpb.SetColor(this.m_materialColorField, this.m_crosshairManager.OutlineColorHighlighted);
				}
				else
				{
					this.m_mpb.SetColor(this.m_materialColorField, this.m_crosshairManager.OutlineColorInRange);
				}
				this.m_renderer.SetPropertyBlock(this.m_mpb);
			}
		}

		// Token: 0x04004D2F RID: 19759
		public string m_materialColorField;

		// Token: 0x04004D30 RID: 19760
		private GrabbableCrosshair m_crosshair;

		// Token: 0x04004D31 RID: 19761
		private GrabManager m_crosshairManager;

		// Token: 0x04004D32 RID: 19762
		private Renderer m_renderer;

		// Token: 0x04004D33 RID: 19763
		private MaterialPropertyBlock m_mpb;

		// Token: 0x04004D34 RID: 19764
		private bool m_inRange;

		// Token: 0x04004D35 RID: 19765
		private bool m_targeted;
	}
}
