using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AD3 RID: 2771
	[RequireComponent(typeof(Collider))]
	public class MovingSurface : MonoBehaviour
	{
		// Token: 0x06004303 RID: 17155 RVA: 0x00135977 File Offset: 0x00133B77
		private void Start()
		{
			MovingSurfaceManager.instance == null;
			MovingSurfaceManager.instance.RegisterMovingSurface(this);
		}

		// Token: 0x06004304 RID: 17156 RVA: 0x00135990 File Offset: 0x00133B90
		private void OnDestroy()
		{
			if (MovingSurfaceManager.instance != null)
			{
				MovingSurfaceManager.instance.UnregisterMovingSurface(this);
			}
		}

		// Token: 0x06004305 RID: 17157 RVA: 0x001359AA File Offset: 0x00133BAA
		public int GetID()
		{
			return this.uniqueId;
		}

		// Token: 0x04004584 RID: 17796
		[SerializeField]
		private int uniqueId = -1;
	}
}
