using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ACF RID: 2767
	public class WhackAMoleManager : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x060042E0 RID: 17120 RVA: 0x0013551A File Offset: 0x0013371A
		private void Awake()
		{
			WhackAMoleManager.instance = this;
			this.allGames.Clear();
		}

		// Token: 0x060042E1 RID: 17121 RVA: 0x00017251 File Offset: 0x00015451
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x060042E2 RID: 17122 RVA: 0x0001725A File Offset: 0x0001545A
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x060042E3 RID: 17123 RVA: 0x00135530 File Offset: 0x00133730
		public void SliceUpdate()
		{
			foreach (WhackAMole whackAMole in this.allGames)
			{
				whackAMole.InvokeUpdate();
			}
		}

		// Token: 0x060042E4 RID: 17124 RVA: 0x00135580 File Offset: 0x00133780
		private void OnDestroy()
		{
			WhackAMoleManager.instance = null;
		}

		// Token: 0x060042E5 RID: 17125 RVA: 0x00135588 File Offset: 0x00133788
		public void Register(WhackAMole whackAMole)
		{
			this.allGames.Add(whackAMole);
		}

		// Token: 0x060042E6 RID: 17126 RVA: 0x00135597 File Offset: 0x00133797
		public void Unregister(WhackAMole whackAMole)
		{
			this.allGames.Remove(whackAMole);
		}

		// Token: 0x060042E8 RID: 17128 RVA: 0x00011040 File Offset: 0x0000F240
		bool IGorillaSliceableSimple.get_isActiveAndEnabled()
		{
			return base.isActiveAndEnabled;
		}

		// Token: 0x0400456F RID: 17775
		public static WhackAMoleManager instance;

		// Token: 0x04004570 RID: 17776
		public HashSet<WhackAMole> allGames = new HashSet<WhackAMole>();
	}
}
