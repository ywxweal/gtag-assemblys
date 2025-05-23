using System;
using GorillaTag.Reactions;
using UnityEngine;

// Token: 0x020009E6 RID: 2534
[RequireComponent(typeof(SpawnWorldEffects))]
public class SpawnWorldEffectsTrigger : MonoBehaviour
{
	// Token: 0x06003C94 RID: 15508 RVA: 0x0012118E File Offset: 0x0011F38E
	private void OnEnable()
	{
		if (this.swe == null)
		{
			this.swe = base.GetComponent<SpawnWorldEffects>();
		}
	}

	// Token: 0x06003C95 RID: 15509 RVA: 0x001211AA File Offset: 0x0011F3AA
	private void OnTriggerEnter(Collider other)
	{
		this.spawnTime = Time.time;
		this.swe.RequestSpawn(base.transform.position);
	}

	// Token: 0x06003C96 RID: 15510 RVA: 0x001211CD File Offset: 0x0011F3CD
	private void OnTriggerStay(Collider other)
	{
		if (Time.time - this.spawnTime < this.spawnCooldown)
		{
			return;
		}
		this.swe.RequestSpawn(base.transform.position);
		this.spawnTime = Time.time;
	}

	// Token: 0x0400407D RID: 16509
	private SpawnWorldEffects swe;

	// Token: 0x0400407E RID: 16510
	private float spawnTime;

	// Token: 0x0400407F RID: 16511
	[SerializeField]
	private float spawnCooldown = 1f;
}
