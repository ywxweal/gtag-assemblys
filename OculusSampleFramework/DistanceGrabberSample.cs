using System;
using UnityEngine;
using UnityEngine.UI;

namespace OculusSampleFramework
{
	// Token: 0x02000BEB RID: 3051
	public class DistanceGrabberSample : MonoBehaviour
	{
		// Token: 0x17000771 RID: 1905
		// (get) Token: 0x06004B5C RID: 19292 RVA: 0x00165BE3 File Offset: 0x00163DE3
		// (set) Token: 0x06004B5D RID: 19293 RVA: 0x00165BEC File Offset: 0x00163DEC
		public bool UseSpherecast
		{
			get
			{
				return this.useSpherecast;
			}
			set
			{
				this.useSpherecast = value;
				for (int i = 0; i < this.m_grabbers.Length; i++)
				{
					this.m_grabbers[i].UseSpherecast = this.useSpherecast;
				}
			}
		}

		// Token: 0x17000772 RID: 1906
		// (get) Token: 0x06004B5E RID: 19294 RVA: 0x00165C26 File Offset: 0x00163E26
		// (set) Token: 0x06004B5F RID: 19295 RVA: 0x00165C30 File Offset: 0x00163E30
		public bool AllowGrabThroughWalls
		{
			get
			{
				return this.allowGrabThroughWalls;
			}
			set
			{
				this.allowGrabThroughWalls = value;
				for (int i = 0; i < this.m_grabbers.Length; i++)
				{
					this.m_grabbers[i].m_preventGrabThroughWalls = !this.allowGrabThroughWalls;
				}
			}
		}

		// Token: 0x06004B60 RID: 19296 RVA: 0x00165C70 File Offset: 0x00163E70
		private void Start()
		{
			DebugUIBuilder.instance.AddLabel("Distance Grab Sample", 0);
			DebugUIBuilder.instance.AddToggle("Use Spherecasting", new DebugUIBuilder.OnToggleValueChange(this.ToggleSphereCasting), this.useSpherecast, 0);
			DebugUIBuilder.instance.AddToggle("Grab Through Walls", new DebugUIBuilder.OnToggleValueChange(this.ToggleGrabThroughWalls), this.allowGrabThroughWalls, 0);
			DebugUIBuilder.instance.Show();
			float displayFrequency = OVRManager.display.displayFrequency;
			if (displayFrequency > 0.1f)
			{
				Debug.Log("Setting Time.fixedDeltaTime to: " + (1f / displayFrequency).ToString());
				Time.fixedDeltaTime = 1f / displayFrequency;
			}
		}

		// Token: 0x06004B61 RID: 19297 RVA: 0x00165D1B File Offset: 0x00163F1B
		public void ToggleSphereCasting(Toggle t)
		{
			this.UseSpherecast = !this.UseSpherecast;
		}

		// Token: 0x06004B62 RID: 19298 RVA: 0x00165D2C File Offset: 0x00163F2C
		public void ToggleGrabThroughWalls(Toggle t)
		{
			this.AllowGrabThroughWalls = !this.AllowGrabThroughWalls;
		}

		// Token: 0x04004DF8 RID: 19960
		private bool useSpherecast;

		// Token: 0x04004DF9 RID: 19961
		private bool allowGrabThroughWalls;

		// Token: 0x04004DFA RID: 19962
		[SerializeField]
		private DistanceGrabber[] m_grabbers;
	}
}
