using System;
using UnityEngine;

// Token: 0x020004D1 RID: 1233
[CreateAssetMenu(fileName = "BuilderPieceEffectInfo", menuName = "Gorilla Tag/Builder/EffectInfo", order = 0)]
public class BuilderPieceEffectInfo : ScriptableObject
{
	// Token: 0x0400211B RID: 8475
	public GameObject placeVFX;

	// Token: 0x0400211C RID: 8476
	public GameObject disconnectVFX;

	// Token: 0x0400211D RID: 8477
	public GameObject grabbedVFX;

	// Token: 0x0400211E RID: 8478
	public GameObject locationLockVFX;

	// Token: 0x0400211F RID: 8479
	public GameObject recycleVFX;

	// Token: 0x04002120 RID: 8480
	public GameObject tooHeavyVFX;
}
