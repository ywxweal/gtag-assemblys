using System;
using UnityEngine;

// Token: 0x020000E1 RID: 225
public class SpawnOnEnter : MonoBehaviour
{
	// Token: 0x060005A0 RID: 1440 RVA: 0x00020968 File Offset: 0x0001EB68
	public void OnTriggerEnter(Collider other)
	{
		if (Time.time > this.lastSpawnTime + this.cooldown)
		{
			this.lastSpawnTime = Time.time;
			ObjectPools.instance.Instantiate(this.prefab, other.transform.position, true);
		}
	}

	// Token: 0x040006AA RID: 1706
	public GameObject prefab;

	// Token: 0x040006AB RID: 1707
	public float cooldown = 0.1f;

	// Token: 0x040006AC RID: 1708
	private float lastSpawnTime;
}
