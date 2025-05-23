using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000CC0 RID: 3264
	public class TagEffectTester : MonoBehaviour, IHandEffectsTrigger
	{
		// Token: 0x1700080E RID: 2062
		// (get) Token: 0x060050A1 RID: 20641 RVA: 0x00180EA7 File Offset: 0x0017F0A7
		public bool Static
		{
			get
			{
				return this.isStatic;
			}
		}

		// Token: 0x1700080F RID: 2063
		// (get) Token: 0x060050A2 RID: 20642 RVA: 0x00180EAF File Offset: 0x0017F0AF
		public IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x17000810 RID: 2064
		// (get) Token: 0x060050A3 RID: 20643 RVA: 0x00180EB7 File Offset: 0x0017F0B7
		public Transform Transform { get; }

		// Token: 0x17000811 RID: 2065
		// (get) Token: 0x060050A4 RID: 20644 RVA: 0x00045F91 File Offset: 0x00044191
		public VRRig Rig
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000812 RID: 2066
		// (get) Token: 0x060050A5 RID: 20645 RVA: 0x00180EBF File Offset: 0x0017F0BF
		public bool FingersDown { get; }

		// Token: 0x17000813 RID: 2067
		// (get) Token: 0x060050A6 RID: 20646 RVA: 0x00180EC7 File Offset: 0x0017F0C7
		public bool FingersUp { get; }

		// Token: 0x17000814 RID: 2068
		// (get) Token: 0x060050A7 RID: 20647 RVA: 0x00180ECF File Offset: 0x0017F0CF
		public Vector3 Velocity { get; }

		// Token: 0x17000815 RID: 2069
		// (get) Token: 0x060050A8 RID: 20648 RVA: 0x00180ED7 File Offset: 0x0017F0D7
		public bool RightHand { get; }

		// Token: 0x17000816 RID: 2070
		// (get) Token: 0x060050A9 RID: 20649 RVA: 0x00180EDF File Offset: 0x0017F0DF
		public float Magnitude { get; }

		// Token: 0x17000817 RID: 2071
		// (get) Token: 0x060050AA RID: 20650 RVA: 0x00180EE7 File Offset: 0x0017F0E7
		public TagEffectPack CosmeticEffectPack { get; }

		// Token: 0x060050AB RID: 20651 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnTriggerEntered(IHandEffectsTrigger other)
		{
		}

		// Token: 0x060050AC RID: 20652 RVA: 0x00002076 File Offset: 0x00000276
		public bool InTriggerZone(IHandEffectsTrigger t)
		{
			return false;
		}

		// Token: 0x040053C1 RID: 21441
		[SerializeField]
		private bool isStatic = true;
	}
}
