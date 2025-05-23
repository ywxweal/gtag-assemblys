using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ACF RID: 2767
	public class WhackAMoleManager : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x060042E1 RID: 17121 RVA: 0x001355F2 File Offset: 0x001337F2
		private void Awake()
		{
			WhackAMoleManager.instance = this;
			this.allGames.Clear();
		}

		// Token: 0x060042E2 RID: 17122 RVA: 0x00017251 File Offset: 0x00015451
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x060042E3 RID: 17123 RVA: 0x0001725A File Offset: 0x0001545A
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x060042E4 RID: 17124 RVA: 0x00135608 File Offset: 0x00133808
		public void SliceUpdate()
		{
			foreach (WhackAMole whackAMole in this.allGames)
			{
				whackAMole.InvokeUpdate();
			}
		}

		// Token: 0x060042E5 RID: 17125 RVA: 0x00135658 File Offset: 0x00133858
		private void OnDestroy()
		{
			WhackAMoleManager.instance = null;
		}

		// Token: 0x060042E6 RID: 17126 RVA: 0x00135660 File Offset: 0x00133860
		public void Register(WhackAMole whackAMole)
		{
			this.allGames.Add(whackAMole);
		}

		// Token: 0x060042E7 RID: 17127 RVA: 0x0013566F File Offset: 0x0013386F
		public void Unregister(WhackAMole whackAMole)
		{
			this.allGames.Remove(whackAMole);
		}

		// Token: 0x060042E9 RID: 17129 RVA: 0x00011040 File Offset: 0x0000F240
		bool IGorillaSliceableSimple.get_isActiveAndEnabled()
		{
			return base.isActiveAndEnabled;
		}

		// Token: 0x04004570 RID: 17776
		public static WhackAMoleManager instance;

		// Token: 0x04004571 RID: 17777
		public HashSet<WhackAMole> allGames = new HashSet<WhackAMole>();
	}
}
