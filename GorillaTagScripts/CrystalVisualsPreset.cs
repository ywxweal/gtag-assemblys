using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000AFE RID: 2814
	[CreateAssetMenu(fileName = "CrystalVisualsPreset", menuName = "ScriptableObjects/CrystalVisualsPreset", order = 0)]
	public class CrystalVisualsPreset : ScriptableObject
	{
		// Token: 0x060044DE RID: 17630 RVA: 0x001464D0 File Offset: 0x001446D0
		public override int GetHashCode()
		{
			return new ValueTuple<CrystalVisualsPreset.VisualState, CrystalVisualsPreset.VisualState>(this.stateA, this.stateB).GetHashCode();
		}

		// Token: 0x060044DF RID: 17631 RVA: 0x000023F4 File Offset: 0x000005F4
		[Conditional("UNITY_EDITOR")]
		private void Save()
		{
		}

		// Token: 0x0400479A RID: 18330
		public CrystalVisualsPreset.VisualState stateA;

		// Token: 0x0400479B RID: 18331
		public CrystalVisualsPreset.VisualState stateB;

		// Token: 0x02000AFF RID: 2815
		[Serializable]
		public struct VisualState
		{
			// Token: 0x060044E1 RID: 17633 RVA: 0x001464FC File Offset: 0x001446FC
			public override int GetHashCode()
			{
				int num = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.albedo);
				int num2 = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.emission);
				return new ValueTuple<int, int>(num, num2).GetHashCode();
			}

			// Token: 0x060044E2 RID: 17634 RVA: 0x00146534 File Offset: 0x00144734
			[CompilerGenerated]
			internal static int <GetHashCode>g__GetColorHash|2_0(Color c)
			{
				return new ValueTuple<float, float, float>(c.r, c.g, c.b).GetHashCode();
			}

			// Token: 0x0400479C RID: 18332
			[ColorUsage(false, false)]
			public Color albedo;

			// Token: 0x0400479D RID: 18333
			[ColorUsage(false, false)]
			public Color emission;
		}
	}
}
