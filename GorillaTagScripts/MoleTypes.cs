using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AC6 RID: 2758
	public class MoleTypes : MonoBehaviour
	{
		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x0600428F RID: 17039 RVA: 0x00133666 File Offset: 0x00131866
		// (set) Token: 0x06004290 RID: 17040 RVA: 0x0013366E File Offset: 0x0013186E
		public bool IsLeftSideMoleType { get; set; }

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x06004291 RID: 17041 RVA: 0x00133677 File Offset: 0x00131877
		// (set) Token: 0x06004292 RID: 17042 RVA: 0x0013367F File Offset: 0x0013187F
		public Mole MoleContainerParent { get; set; }

		// Token: 0x06004293 RID: 17043 RVA: 0x00133688 File Offset: 0x00131888
		private void Start()
		{
			this.MoleContainerParent = base.GetComponentInParent<Mole>();
			if (this.MoleContainerParent)
			{
				this.IsLeftSideMoleType = this.MoleContainerParent.IsLeftSideMole;
			}
		}

		// Token: 0x040044F7 RID: 17655
		public bool isHazard;

		// Token: 0x040044F8 RID: 17656
		public int scorePoint = 1;

		// Token: 0x040044F9 RID: 17657
		public MeshRenderer MeshRenderer;

		// Token: 0x040044FA RID: 17658
		public Material monkeMoleDefaultMaterial;

		// Token: 0x040044FB RID: 17659
		public Material monkeMoleHitMaterial;
	}
}
