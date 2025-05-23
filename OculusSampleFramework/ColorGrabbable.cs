using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BC5 RID: 3013
	public class ColorGrabbable : OVRGrabbable
	{
		// Token: 0x17000730 RID: 1840
		// (get) Token: 0x06004A6B RID: 19051 RVA: 0x0016273E File Offset: 0x0016093E
		// (set) Token: 0x06004A6C RID: 19052 RVA: 0x00162746 File Offset: 0x00160946
		public bool Highlight
		{
			get
			{
				return this.m_highlight;
			}
			set
			{
				this.m_highlight = value;
				this.UpdateColor();
			}
		}

		// Token: 0x06004A6D RID: 19053 RVA: 0x00162755 File Offset: 0x00160955
		protected void UpdateColor()
		{
			if (base.isGrabbed)
			{
				this.SetColor(ColorGrabbable.COLOR_GRAB);
				return;
			}
			if (this.Highlight)
			{
				this.SetColor(ColorGrabbable.COLOR_HIGHLIGHT);
				return;
			}
			this.SetColor(this.m_color);
		}

		// Token: 0x06004A6E RID: 19054 RVA: 0x0016278B File Offset: 0x0016098B
		public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
		{
			base.GrabBegin(hand, grabPoint);
			this.UpdateColor();
		}

		// Token: 0x06004A6F RID: 19055 RVA: 0x0016279B File Offset: 0x0016099B
		public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
		{
			base.GrabEnd(linearVelocity, angularVelocity);
			this.UpdateColor();
		}

		// Token: 0x06004A70 RID: 19056 RVA: 0x001627AC File Offset: 0x001609AC
		private void Awake()
		{
			if (this.m_grabPoints.Length == 0)
			{
				Collider component = base.GetComponent<Collider>();
				if (component == null)
				{
					throw new ArgumentException("Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
				}
				this.m_grabPoints = new Collider[] { component };
				this.m_meshRenderers = new MeshRenderer[1];
				this.m_meshRenderers[0] = base.GetComponent<MeshRenderer>();
			}
			else
			{
				this.m_meshRenderers = base.GetComponentsInChildren<MeshRenderer>();
			}
			this.m_color = new Color(Random.Range(0.1f, 0.95f), Random.Range(0.1f, 0.95f), Random.Range(0.1f, 0.95f), 1f);
			this.SetColor(this.m_color);
		}

		// Token: 0x06004A71 RID: 19057 RVA: 0x00162860 File Offset: 0x00160A60
		private void SetColor(Color color)
		{
			for (int i = 0; i < this.m_meshRenderers.Length; i++)
			{
				MeshRenderer meshRenderer = this.m_meshRenderers[i];
				for (int j = 0; j < meshRenderer.materials.Length; j++)
				{
					meshRenderer.materials[j].color = color;
				}
			}
		}

		// Token: 0x04004D2A RID: 19754
		public static readonly Color COLOR_GRAB = new Color(1f, 0.5f, 0f, 1f);

		// Token: 0x04004D2B RID: 19755
		public static readonly Color COLOR_HIGHLIGHT = new Color(1f, 0f, 1f, 1f);

		// Token: 0x04004D2C RID: 19756
		private Color m_color = Color.black;

		// Token: 0x04004D2D RID: 19757
		private MeshRenderer[] m_meshRenderers;

		// Token: 0x04004D2E RID: 19758
		private bool m_highlight;
	}
}
