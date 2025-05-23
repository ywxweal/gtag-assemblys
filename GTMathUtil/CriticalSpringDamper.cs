using System;

namespace GTMathUtil
{
	// Token: 0x02000AC2 RID: 2754
	internal class CriticalSpringDamper
	{
		// Token: 0x06004278 RID: 17016 RVA: 0x00131162 File Offset: 0x0012F362
		private static float halflife_to_damping(float halflife, float eps = 1E-05f)
		{
			return 2.7725887f / (halflife + eps);
		}

		// Token: 0x06004279 RID: 17017 RVA: 0x00130E89 File Offset: 0x0012F089
		private static float fast_negexp(float x)
		{
			return 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
		}

		// Token: 0x0600427A RID: 17018 RVA: 0x001330E0 File Offset: 0x001312E0
		public float Update(float dt)
		{
			float num = CriticalSpringDamper.halflife_to_damping(this.halfLife, 1E-05f) / 2f;
			float num2 = this.x - this.xGoal;
			float num3 = this.curVel + num2 * num;
			float num4 = CriticalSpringDamper.fast_negexp(num * dt);
			this.x = num4 * (num2 + num3 * dt) + this.xGoal;
			this.curVel = num4 * (this.curVel - num3 * num * dt);
			return this.x;
		}

		// Token: 0x040044D5 RID: 17621
		public float x;

		// Token: 0x040044D6 RID: 17622
		public float xGoal;

		// Token: 0x040044D7 RID: 17623
		public float halfLife = 0.1f;

		// Token: 0x040044D8 RID: 17624
		private float curVel;
	}
}
