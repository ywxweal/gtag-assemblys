using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AD4 RID: 2772
	public class MovingSurfaceManager : MonoBehaviour
	{
		// Token: 0x06004308 RID: 17160 RVA: 0x00135A9C File Offset: 0x00133C9C
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

		// Token: 0x06004309 RID: 17161 RVA: 0x00135AE8 File Offset: 0x00133CE8
		public void RegisterMovingSurface(MovingSurface ms)
		{
			this.movingSurfaces.TryAdd(ms.GetID(), ms);
		}

		// Token: 0x0600430A RID: 17162 RVA: 0x00135AFD File Offset: 0x00133CFD
		public void UnregisterMovingSurface(MovingSurface ms)
		{
			this.movingSurfaces.Remove(ms.GetID());
		}

		// Token: 0x0600430B RID: 17163 RVA: 0x00135B11 File Offset: 0x00133D11
		public void RegisterSurfaceMover(SurfaceMover sm)
		{
			if (!this.surfaceMovers.Contains(sm))
			{
				this.surfaceMovers.Add(sm);
				sm.InitMovingSurface();
			}
		}

		// Token: 0x0600430C RID: 17164 RVA: 0x00135B33 File Offset: 0x00133D33
		public void UnregisterSurfaceMover(SurfaceMover sm)
		{
			this.surfaceMovers.Remove(sm);
		}

		// Token: 0x0600430D RID: 17165 RVA: 0x00135B42 File Offset: 0x00133D42
		public bool TryGetMovingSurface(int id, out MovingSurface result)
		{
			return this.movingSurfaces.TryGetValue(id, out result) && result != null;
		}

		// Token: 0x0600430E RID: 17166 RVA: 0x00135B60 File Offset: 0x00133D60
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

		// Token: 0x04004586 RID: 17798
		private List<SurfaceMover> surfaceMovers = new List<SurfaceMover>(5);

		// Token: 0x04004587 RID: 17799
		private Dictionary<int, MovingSurface> movingSurfaces = new Dictionary<int, MovingSurface>(10);

		// Token: 0x04004588 RID: 17800
		public static MovingSurfaceManager instance;
	}
}
