using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AD4 RID: 2772
	public class MovingSurfaceManager : MonoBehaviour
	{
		// Token: 0x06004307 RID: 17159 RVA: 0x001359C4 File Offset: 0x00133BC4
		private void Awake()
		{
			if (MovingSurfaceManager.instance != null && MovingSurfaceManager.instance != this)
			{
				GTDev.LogWarning<string>("Instance of MovingSurfaceManager already exists. Destroying.", null);
				Object.Destroy(this);
				return;
			}
			if (MovingSurfaceManager.instance == null)
			{
				MovingSurfaceManager.instance = this;
			}
		}

		// Token: 0x06004308 RID: 17160 RVA: 0x00135A10 File Offset: 0x00133C10
		public void RegisterMovingSurface(MovingSurface ms)
		{
			this.movingSurfaces.TryAdd(ms.GetID(), ms);
		}

		// Token: 0x06004309 RID: 17161 RVA: 0x00135A25 File Offset: 0x00133C25
		public void UnregisterMovingSurface(MovingSurface ms)
		{
			this.movingSurfaces.Remove(ms.GetID());
		}

		// Token: 0x0600430A RID: 17162 RVA: 0x00135A39 File Offset: 0x00133C39
		public void RegisterSurfaceMover(SurfaceMover sm)
		{
			if (!this.surfaceMovers.Contains(sm))
			{
				this.surfaceMovers.Add(sm);
				sm.InitMovingSurface();
			}
		}

		// Token: 0x0600430B RID: 17163 RVA: 0x00135A5B File Offset: 0x00133C5B
		public void UnregisterSurfaceMover(SurfaceMover sm)
		{
			this.surfaceMovers.Remove(sm);
		}

		// Token: 0x0600430C RID: 17164 RVA: 0x00135A6A File Offset: 0x00133C6A
		public bool TryGetMovingSurface(int id, out MovingSurface result)
		{
			return this.movingSurfaces.TryGetValue(id, out result) && result != null;
		}

		// Token: 0x0600430D RID: 17165 RVA: 0x00135A88 File Offset: 0x00133C88
		private void FixedUpdate()
		{
			foreach (SurfaceMover surfaceMover in this.surfaceMovers)
			{
				if (surfaceMover.isActiveAndEnabled)
				{
					surfaceMover.Move();
				}
			}
		}

		// Token: 0x04004585 RID: 17797
		private List<SurfaceMover> surfaceMovers = new List<SurfaceMover>(5);

		// Token: 0x04004586 RID: 17798
		private Dictionary<int, MovingSurface> movingSurfaces = new Dictionary<int, MovingSurface>(10);

		// Token: 0x04004587 RID: 17799
		public static MovingSurfaceManager instance;
	}
}
