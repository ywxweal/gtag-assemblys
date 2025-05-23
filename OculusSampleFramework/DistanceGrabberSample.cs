using System;
using UnityEngine;
using UnityEngine.UI;

namespace OculusSampleFramework
{
	// Token: 0x02000BEB RID: 3051
	public class DistanceGrabberSample : MonoBehaviour
	{
		// Token: 0x17000771 RID: 1905
		// (get) Token: 0x06004B5B RID: 19291 RVA: 0x00165B0B File Offset: 0x00163D0B
		// (set) Token: 0x06004B5C RID: 19292 RVA: 0x00165B14 File Offset: 0x00163D14
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
		// (get) Token: 0x06004B5D RID: 19293 RVA: 0x00165B4E File Offset: 0x00163D4E
		// (set) Token: 0x06004B5E RID: 19294 RVA: 0x00165B58 File Offset: 0x00163D58
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

		// Token: 0x06004B5F RID: 19295 RVA: 0x00165B98 File Offset: 0x00163D98
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

		// Token: 0x06004B60 RID: 19296 RVA: 0x00165C43 File Offset: 0x00163E43
		public void ToggleSphereCasting(Toggle t)
		{
			this.UseSpherecast = !this.UseSpherecast;
		}

		// Token: 0x06004B61 RID: 19297 RVA: 0x00165C54 File Offset: 0x00163E54
		public void ToggleGrabThroughWalls(Toggle t)
		{
			this.AllowGrabThroughWalls = !this.AllowGrabThroughWalls;
		}

		// Token: 0x04004DF7 RID: 19959
		private bool useSpherecast;

		// Token: 0x04004DF8 RID: 19960
		private bool allowGrabThroughWalls;

		// Token: 0x04004DF9 RID: 19961
		[SerializeField]
		private DistanceGrabber[] m_grabbers;
	}
}
