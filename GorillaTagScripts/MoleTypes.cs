using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AC6 RID: 2758
	public class MoleTypes : MonoBehaviour
	{
		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x06004290 RID: 17040 RVA: 0x0013373E File Offset: 0x0013193E
		// (set) Token: 0x06004291 RID: 17041 RVA: 0x00133746 File Offset: 0x00131946
		public bool IsLeftSideMoleType { get; set; }

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x06004292 RID: 17042 RVA: 0x0013374F File Offset: 0x0013194F
		// (set) Token: 0x06004293 RID: 17043 RVA: 0x00133757 File Offset: 0x00131957
		public Mole MoleContainerParent { get; set; }

		// Token: 0x06004294 RID: 17044 RVA: 0x00133760 File Offset: 0x00131960
		private void Start()
		{
			this.MoleContainerParent = base.GetComponentInParent<Mole>();
			if (this.MoleContainerParent)
			{
				this.IsLeftSideMoleType = this.MoleContainerParent.IsLeftSideMole;
			}
		}

		// Token: 0x040044F8 RID: 17656
		public bool isHazard;

		// Token: 0x040044F9 RID: 17657
		public int scorePoint = 1;

		// Token: 0x040044FA RID: 17658
		public MeshRenderer MeshRenderer;

		// Token: 0x040044FB RID: 17659
		public Material monkeMoleDefaultMaterial;

		// Token: 0x040044FC RID: 17660
		public Material monkeMoleHitMaterial;
	}
}
