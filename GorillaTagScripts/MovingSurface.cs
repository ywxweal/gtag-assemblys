using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AD3 RID: 2771
	[RequireComponent(typeof(Collider))]
	public class MovingSurface : MonoBehaviour
	{
		// Token: 0x06004304 RID: 17156 RVA: 0x00135A4F File Offset: 0x00133C4F
		private void Start()
		{
			MovingSurfaceManager.instance == null;
			MovingSurfaceManager.instance.RegisterMovingSurface(this);
		}

		// Token: 0x06004305 RID: 17157 RVA: 0x00135A68 File Offset: 0x00133C68
		private void OnDestroy()
		{
			if (MovingSurfaceManager.instance != null)
			{
				MovingSurfaceManager.instance.UnregisterMovingSurface(this);
			}
		}

		// Token: 0x06004306 RID: 17158 RVA: 0x00135A82 File Offset: 0x00133C82
		public int GetID()
		{
			return this.uniqueId;
		}

		// Token: 0x04004585 RID: 17797
		[SerializeField]
		private int uniqueId = -1;
	}
}
