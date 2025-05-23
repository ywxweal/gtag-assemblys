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
		// (get) Token: 0x06004A3B RID: 19003 RVA: 0x00161DFF File Offset: 0x0015FFFF
		// (set) Token: 0x06004A3C RID: 19004 RVA: 0x00161E07 File Offset: 0x00160007
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
		// (get) Token: 0x06004A3D RID: 19005 RVA: 0x00161E10 File Offset: 0x00160010
		// (set) Token: 0x06004A3E RID: 19006 RVA: 0x00161E18 File Offset: 0x00160018
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
		// (get) Token: 0x06004A3F RID: 19007 RVA: 0x00161E21 File Offset: 0x00160021
		// (set) Token: 0x06004A40 RID: 19008 RVA: 0x00161E29 File Offset: 0x00160029
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

		// Token: 0x06004A41 RID: 19009
		[Conditional("UNITY_EDITOR")]
		public abstract void OnSceneBake(Scene scene, SceneBakeMode mode);

		// Token: 0x06004A42 RID: 19010 RVA: 0x000023F4 File Offset: 0x000005F4
		[Conditional("UNITY_EDITOR")]
		private void ForceRun()
		{
		}

		// Token: 0x04004D13 RID: 19731
		[SerializeField]
		private SceneBakeMode m_bakeMode;

		// Token: 0x04004D14 RID: 19732
		[SerializeField]
		private int m_callbackOrder;

		// Token: 0x04004D15 RID: 19733
		[Space]
		[SerializeField]
		private bool m_runIfInactive = true;
	}
}
