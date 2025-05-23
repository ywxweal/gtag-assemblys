using System;

namespace TagEffects
{
	// Token: 0x02000CBB RID: 3259
	[Serializable]
	public class TagEffectsCombo : IEquatable<TagEffectsCombo>
	{
		// Token: 0x06005092 RID: 20626 RVA: 0x00180D80 File Offset: 0x0017EF80
		bool IEquatable<TagEffectsCombo>.Equals(TagEffectsCombo other)
		{
			return (other.inputA == this.inputA && other.inputB == this.inputB) || (other.inputA == this.inputB && other.inputB == this.inputA);
		}

		// Token: 0x06005093 RID: 20627 RVA: 0x00180DDB File Offset: 0x0017EFDB
		public override bool Equals(object obj)
		{
			return this.Equals((TagEffectsCombo)obj);
		}

		// Token: 0x06005094 RID: 20628 RVA: 0x00180DE9 File Offset: 0x0017EFE9
		public override int GetHashCode()
		{
			return this.inputA.GetHashCode() * this.inputB.GetHashCode();
		}

		// Token: 0x040053B6 RID: 21430
		public TagEffectPack inputA;

		// Token: 0x040053B7 RID: 21431
		public TagEffectPack inputB;
	}
}
