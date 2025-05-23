using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000E4C RID: 3660
	public class BoingBase : MonoBehaviour
	{
		// Token: 0x170008EB RID: 2283
		// (get) Token: 0x06005B9E RID: 23454 RVA: 0x001C2442 File Offset: 0x001C0642
		public Version CurrentVersion
		{
			get
			{
				return this.m_currentVersion;
			}
		}

		// Token: 0x170008EC RID: 2284
		// (get) Token: 0x06005B9F RID: 23455 RVA: 0x001C244A File Offset: 0x001C064A
		public Version PreviousVersion
		{
			get
			{
				return this.m_previousVersion;
			}
		}

		// Token: 0x170008ED RID: 2285
		// (get) Token: 0x06005BA0 RID: 23456 RVA: 0x001C2452 File Offset: 0x001C0652
		public Version InitialVersion
		{
			get
			{
				return this.m_initialVersion;
			}
		}

		// Token: 0x06005BA1 RID: 23457 RVA: 0x001C245A File Offset: 0x001C065A
		protected virtual void OnUpgrade(Version oldVersion, Version newVersion)
		{
			this.m_previousVersion = this.m_currentVersion;
			if (this.m_currentVersion.Revision < 33)
			{
				this.m_initialVersion = Version.Invalid;
				this.m_previousVersion = Version.Invalid;
			}
			this.m_currentVersion = newVersion;
		}

		// Token: 0x04005F7F RID: 24447
		[SerializeField]
		private Version m_currentVersion;

		// Token: 0x04005F80 RID: 24448
		[SerializeField]
		private Version m_previousVersion;

		// Token: 0x04005F81 RID: 24449
		[SerializeField]
		private Version m_initialVersion = BoingKit.Version;
	}
}
