using System;
using GorillaTag.Reactions;
using UnityEngine;

// Token: 0x020009E6 RID: 2534
[RequireComponent(typeof(SpawnWorldEffects))]
public class SpawnWorldEffectsTrigger : MonoBehaviour
{
	// Token: 0x06003C93 RID: 15507 RVA: 0x001210B6 File Offset: 0x0011F2B6
	private void OnEnable()
	{
		if (this.swe == null)
		{
			this.swe = base.GetComponent<SpawnWorldEffects>();
		}
	}

	// Token: 0x06003C94 RID: 15508 RVA: 0x001210D2 File Offset: 0x0011F2D2
	private void OnTriggerEnter(Collider other)
	{
		this.spawnTime = Time.time;
		this.swe.RequestSpawn(base.transform.position);
	}

	// Token: 0x06003C95 RID: 15509 RVA: 0x001210F5 File Offset: 0x0011F2F5
	private void OnTriggerStay(Collider other)
	{
		if (Time.time - this.spawnTime < this.spawnCooldown)
		{
			return;
		}
		this.swe.RequestSpawn(base.transform.position);
		this.spawnTime = Time.time;
	}

	// Token: 0x0400407C RID: 16508
	private SpawnWorldEffects swe;

	// Token: 0x0400407D RID: 16509
	private float spawnTime;

	// Token: 0x0400407E RID: 16510
	[SerializeField]
	private float spawnCooldown = 1f;
}
