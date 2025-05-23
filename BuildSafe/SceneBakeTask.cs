using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BuildSafe
{
	// Token: 0x02000BB7 RID: 2999
	public abstract class SceneBakeTask : MonoBehaviour
	{
		// Token: 0x17000729 RID: 1833
		// (get) Token: 0x06004A3A RID: 19002 RVA: 0x00161D27 File Offset: 0x0015FF27
		// (set) Token: 0x06004A3B RID: 19003 RVA: 0x00161D2F File Offset: 0x0015FF2F
		public SceneBakeMode bakeMode
		{
			get
			{
				return this.m_bakeMode;
			}
			set
			{
				this.m_bakeMode = value;
			}
		}

		// Token: 0x1700072A RID: 1834
		// (get) Token: 0x06004A3C RID: 19004 RVA: 0x00161D38 File Offset: 0x0015FF38
		// (set) Token: 0x06004A3D RID: 19005 RVA: 0x00161D40 File Offset: 0x0015FF40
		public virtual int callbackOrder
		{
			get
			{
				return this.m_callbackOrder;
			}
			set
			{
				this.m_callbackOrder = value;
			}
		}

		// Token: 0x1700072B RID: 1835
		// (get) Token: 0x06004A3E RID: 19006 RVA: 0x00161D49 File Offset: 0x0015FF49
		// (set) Token: 0x06004A3F RID: 19007 RVA: 0x00161D51 File Offset: 0x0015FF51
		public bool runIfInactive
		{
			get
			{
				return this.m_runIfInactive;
			}
			set
			{
				this.m_runIfInactive = value;
			}
		}

		// Token: 0x06004A40 RID: 19008
		[Conditional("UNITY_EDITOR")]
		public abstract void OnSceneBake(Scene scene, SceneBakeMode mode);

		// Token: 0x06004A41 RID: 19009 RVA: 0x000023F4 File Offset: 0x000005F4
		[Conditional("UNITY_EDITOR")]
		private void ForceRun()
		{
		}

		// Token: 0x04004D12 RID: 19730
		[SerializeField]
		private SceneBakeMode m_bakeMode;

		// Token: 0x04004D13 RID: 19731
		[SerializeField]
		private int m_callbackOrder;

		// Token: 0x04004D14 RID: 19732
		[Space]
		[SerializeField]
		private bool m_runIfInactive = true;
	}
}
