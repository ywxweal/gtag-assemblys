using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02000CC0 RID: 3264
	public class TagEffectTester : MonoBehaviour, IHandEffectsTrigger
	{
		// Token: 0x1700080E RID: 2062
		// (get) Token: 0x060050A0 RID: 20640 RVA: 0x00180DCF File Offset: 0x0017EFCF
		public bool Static
		{
			get
			{
				return this.isStatic;
			}
		}

		// Token: 0x1700080F RID: 2063
		// (get) Token: 0x060050A1 RID: 20641 RVA: 0x00180DD7 File Offset: 0x0017EFD7
		public IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x17000810 RID: 2064
		// (get) Token: 0x060050A2 RID: 20642 RVA: 0x00180DDF File Offset: 0x0017EFDF
		public Transform Transform { get; }

		// Token: 0x17000811 RID: 2065
		// (get) Token: 0x060050A3 RID: 20643 RVA: 0x00045F91 File Offset: 0x00044191
		public VRRig Rig
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000812 RID: 2066
		// (get) Token: 0x060050A4 RID: 20644 RVA: 0x00180DE7 File Offset: 0x0017EFE7
		public bool FingersDown { get; }

		// Token: 0x17000813 RID: 2067
		// (get) Token: 0x060050A5 RID: 20645 RVA: 0x00180DEF File Offset: 0x0017EFEF
		public bool FingersUp { get; }

		// Token: 0x17000814 RID: 2068
		// (get) Token: 0x060050A6 RID: 20646 RVA: 0x00180DF7 File Offset: 0x0017EFF7
		public Vector3 Velocity { get; }

		// Token: 0x17000815 RID: 2069
		// (get) Token: 0x060050A7 RID: 20647 RVA: 0x00180DFF File Offset: 0x0017EFFF
		public bool RightHand { get; }

		// Token: 0x17000816 RID: 2070
		// (get) Token: 0x060050A8 RID: 20648 RVA: 0x00180E07 File Offset: 0x0017F007
		public float Magnitude { get; }

		// Token: 0x17000817 RID: 2071
		// (get) Token: 0x060050A9 RID: 20649 RVA: 0x00180E0F File Offset: 0x0017F00F
		public TagEffectPack CosmeticEffectPack { get; }

		// Token: 0x060050AA RID: 20650 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnTriggerEntered(IHandEffectsTrigger other)
		{
		}

		// Token: 0x060050AB RID: 20651 RVA: 0x00002076 File Offset: 0x00000276
		public bool InTriggerZone(IHandEffectsTrigger t)
		{
			return false;
		}

		// Token: 0x040053C0 RID: 21440
		[SerializeField]
		private bool isStatic = true;
	}
}
