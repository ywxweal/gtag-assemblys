using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x0200064A RID: 1610
public class GorillaTurning : GorillaTriggerBox
{
	// Token: 0x06002848 RID: 10312 RVA: 0x000023F4 File Offset: 0x000005F4
	private void Awake()
	{
	}

	// Token: 0x04002D26 RID: 11558
	public Material redMaterial;

	// Token: 0x04002D27 RID: 11559
	public Material blueMaterial;

	// Token: 0x04002D28 RID: 11560
	public Material greenMaterial;

	// Token: 0x04002D29 RID: 11561
	public Material transparentBlueMaterial;

	// Token: 0x04002D2A RID: 11562
	public Material transparentRedMaterial;

	// Token: 0x04002D2B RID: 11563
	public Material transparentGreenMaterial;

	// Token: 0x04002D2C RID: 11564
	public MeshRenderer smoothTurnBox;

	// Token: 0x04002D2D RID: 11565
	public MeshRenderer snapTurnBox;

	// Token: 0x04002D2E RID: 11566
	public MeshRenderer noTurnBox;

	// Token: 0x04002D2F RID: 11567
	public GorillaSnapTurn snapTurn;

	// Token: 0x04002D30 RID: 11568
	public string currentChoice;

	// Token: 0x04002D31 RID: 11569
	public float currentSpeed;
}
