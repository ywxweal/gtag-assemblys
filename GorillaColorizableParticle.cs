using System;
using UnityEngine;

// Token: 0x020005FA RID: 1530
public class GorillaColorizableParticle : GorillaColorizableBase
{
	// Token: 0x060025BA RID: 9658 RVA: 0x000BB9BC File Offset: 0x000B9BBC
	public override void SetColor(Color color)
	{
		ParticleSystem.MainModule main = this.particleSystem.main;
		Color color2 = new Color(Mathf.Pow(color.r, this.gradientColorPower), Mathf.Pow(color.g, this.gradientColorPower), Mathf.Pow(color.b, this.gradientColorPower), color.a);
		main.startColor = new ParticleSystem.MinMaxGradient(this.useLinearColor ? color.linear : color, this.useLinearColor ? color2.linear : color2);
	}

	// Token: 0x04002A4D RID: 10829
	public ParticleSystem particleSystem;

	// Token: 0x04002A4E RID: 10830
	public float gradientColorPower = 2f;

	// Token: 0x04002A4F RID: 10831
	public bool useLinearColor = true;
}
