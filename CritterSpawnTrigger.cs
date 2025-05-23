using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000067 RID: 103
public class CritterSpawnTrigger : MonoBehaviour
{
	// Token: 0x06000287 RID: 647 RVA: 0x000101D4 File Offset: 0x0000E3D4
	private ValueDropdownList<int> GetCritterTypeList()
	{
		return new ValueDropdownList<int>();
	}

	// Token: 0x06000288 RID: 648 RVA: 0x000101DC File Offset: 0x0000E3DC
	private void OnTriggerEnter(Collider other)
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (Time.realtimeSinceStartup < this._nextSpawnTime)
		{
			return;
		}
		CrittersActor componentInParent = other.GetComponentInParent<CrittersActor>();
		if (!componentInParent)
		{
			return;
		}
		if (componentInParent.crittersActorType != this.triggerActorType)
		{
			return;
		}
		if (this.requiredSubObjectIndex >= 0 && componentInParent.subObjectIndex != this.requiredSubObjectIndex)
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.triggerActorName) && !componentInParent.GetActorSubtype().Contains(this.triggerActorName))
		{
			return;
		}
		CrittersManager.instance.DespawnActor(componentInParent);
		CrittersManager.instance.SpawnCritter(this.critterType, this.spawnPoint.position, this.spawnPoint.rotation);
		this._nextSpawnTime = Time.realtimeSinceStartup + this.triggerCooldown;
	}

	// Token: 0x06000289 RID: 649 RVA: 0x000102A6 File Offset: 0x0000E4A6
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(base.transform.position, this.spawnPoint.position);
		Gizmos.DrawWireSphere(this.spawnPoint.position, 0.1f);
	}

	// Token: 0x04000306 RID: 774
	[Header("Trigger Settings")]
	[SerializeField]
	private CrittersActor.CrittersActorType triggerActorType;

	// Token: 0x04000307 RID: 775
	[SerializeField]
	private int requiredSubObjectIndex = -1;

	// Token: 0x04000308 RID: 776
	[SerializeField]
	private string triggerActorName;

	// Token: 0x04000309 RID: 777
	[SerializeField]
	private float triggerCooldown = 1f;

	// Token: 0x0400030A RID: 778
	[Header("Spawn Settings")]
	[SerializeField]
	private Transform spawnPoint;

	// Token: 0x0400030B RID: 779
	[SerializeField]
	private int critterType;

	// Token: 0x0400030C RID: 780
	private float _nextSpawnTime;
}
