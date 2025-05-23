using System;
using UnityEngine;

// Token: 0x020000A8 RID: 168
public class CosmeticCritterSpawnerShadeHidden : CosmeticCritterSpawnerTimed
{
	// Token: 0x0600042F RID: 1071 RVA: 0x00018620 File Offset: 0x00016820
	public override void SetRandomVariables(CosmeticCritter critter)
	{
		float num = Random.Range(this.orbitHeightOffsetMinMax.x, this.orbitHeightOffsetMinMax.y);
		float num2 = Random.Range(this.orbitRadiusMinMax.x, this.orbitRadiusMinMax.y);
		(critter as CosmeticCritterShadeHidden).SetCenterAndRadius(base.transform.position + new Vector3(0f, num, 0f), num2);
	}

	// Token: 0x040004A3 RID: 1187
	[Tooltip("Add between X and Y extra height to the base orbit height.")]
	[SerializeField]
	private Vector2 orbitHeightOffsetMinMax = new Vector2(0f, 2f);

	// Token: 0x040004A4 RID: 1188
	[Tooltip("Orbit between X (green sphere) and Y (red sphere) units away from this spawner's position when first spawned.")]
	[SerializeField]
	private Vector2 orbitRadiusMinMax = new Vector2(5f, 10f);
}
