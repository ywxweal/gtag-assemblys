using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B56 RID: 2902
	public class BuilderParticleSpawner : MonoBehaviour
	{
		// Token: 0x060047A0 RID: 18336 RVA: 0x001549EC File Offset: 0x00152BEC
		private void Start()
		{
			this.spawnTrigger.onTriggerFirstEntered += this.OnEnter;
			this.spawnTrigger.onTriggerLastExited += this.OnExit;
		}

		// Token: 0x060047A1 RID: 18337 RVA: 0x00154A1C File Offset: 0x00152C1C
		private void OnDestroy()
		{
			if (this.spawnTrigger != null)
			{
				this.spawnTrigger.onTriggerFirstEntered -= this.OnEnter;
				this.spawnTrigger.onTriggerLastExited -= this.OnExit;
			}
		}

		// Token: 0x060047A2 RID: 18338 RVA: 0x00154A5C File Offset: 0x00152C5C
		public void TrySpawning()
		{
			if (Time.time > this.lastSpawnTime + this.cooldown)
			{
				this.lastSpawnTime = Time.time;
				ObjectPools.instance.Instantiate(this.prefab, this.spawnLocation.position, this.spawnLocation.rotation, this.myPiece.GetScale(), true);
			}
		}

		// Token: 0x060047A3 RID: 18339 RVA: 0x00154ABB File Offset: 0x00152CBB
		private void OnEnter()
		{
			if (this.spawnOnEnter)
			{
				this.TrySpawning();
			}
		}

		// Token: 0x060047A4 RID: 18340 RVA: 0x00154ACB File Offset: 0x00152CCB
		private void OnExit()
		{
			if (this.spawnOnExit)
			{
				this.TrySpawning();
			}
		}

		// Token: 0x040049F1 RID: 18929
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x040049F2 RID: 18930
		public GameObject prefab;

		// Token: 0x040049F3 RID: 18931
		public float cooldown = 0.1f;

		// Token: 0x040049F4 RID: 18932
		private float lastSpawnTime;

		// Token: 0x040049F5 RID: 18933
		[SerializeField]
		private BuilderSmallMonkeTrigger spawnTrigger;

		// Token: 0x040049F6 RID: 18934
		[SerializeField]
		private bool spawnOnEnter = true;

		// Token: 0x040049F7 RID: 18935
		[SerializeField]
		private bool spawnOnExit;

		// Token: 0x040049F8 RID: 18936
		[SerializeField]
		private Transform spawnLocation;
	}
}
