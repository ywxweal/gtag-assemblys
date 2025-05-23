using System;

namespace TagEffects
{
	// Token: 0x02000CBB RID: 3259
	[Serializable]
	public class TagEffectsCombo : IEquatable<TagEffectsCombo>
	{
		// Token: 0x06005091 RID: 20625 RVA: 0x00180CA8 File Offset: 0x0017EEA8
		bool IEquatable<TagEffectsCombo>.Equals(TagEffectsCombo other)
		{
			return (other.inputA == this.inputA && other.inputB == this.inputB) || (other.inputA == this.inputB && other.inputB == this.inputA);
		}

		// Token: 0x06005092 RID: 20626 RVA: 0x00180D03 File Offset: 0x0017EF03
		public override bool Equals(object obj)
		{
			return this.Equals((TagEffectsCombo)obj);
		}

		// Token: 0x06005093 RID: 20627 RVA: 0x00180D11 File Offset: 0x0017EF11
		public override int GetHashCode()
		{
			return this.inputA.GetHashCode() * this.inputB.GetHashCode();
		}

		// Token: 0x040053B5 RID: 21429
		public TagEffectPack inputA;

		// Token: 0x040053B6 RID: 21430
		public TagEffectPack inputB;
	}
}
