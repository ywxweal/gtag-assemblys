using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B56 RID: 2902
	public class BuilderParticleSpawner : MonoBehaviour
	{
		// Token: 0x060047A1 RID: 18337 RVA: 0x00154AC4 File Offset: 0x00152CC4
		private void Start()
		{
			this.spawnTrigger.onTriggerFirstEntered += this.OnEnter;
			this.spawnTrigger.onTriggerLastExited += this.OnExit;
		}

		// Token: 0x060047A2 RID: 18338 RVA: 0x00154AF4 File Offset: 0x00152CF4
		private void OnDestroy()
		{
			if (this.spawnTrigger != null)
			{
				this.spawnTrigger.onTriggerFirstEntered -= this.OnEnter;
				this.spawnTrigger.onTriggerLastExited -= this.OnExit;
			}
		}

		// Token: 0x060047A3 RID: 18339 RVA: 0x00154B34 File Offset: 0x00152D34
		public void TrySpawning()
		{
			if (Time.time > this.lastSpawnTime + this.cooldown)
			{
				this.lastSpawnTime = Time.time;
				ObjectPools.instance.Instantiate(this.prefab, this.spawnLocation.position, this.spawnLocation.rotation, this.myPiece.GetScale(), true);
			}
		}

		// Token: 0x060047A4 RID: 18340 RVA: 0x00154B93 File Offset: 0x00152D93
		private void OnEnter()
		{
			if (this.spawnOnEnter)
			{
				this.TrySpawning();
			}
		}

		// Token: 0x060047A5 RID: 18341 RVA: 0x00154BA3 File Offset: 0x00152DA3
		private void OnExit()
		{
			if (this.spawnOnExit)
			{
				this.TrySpawning();
			}
		}

		// Token: 0x040049F2 RID: 18930
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x040049F3 RID: 18931
		public GameObject prefab;

		// Token: 0x040049F4 RID: 18932
		public float cooldown = 0.1f;

		// Token: 0x040049F5 RID: 18933
		private float lastSpawnTime;

		// Token: 0x040049F6 RID: 18934
		[SerializeField]
		private BuilderSmallMonkeTrigger spawnTrigger;

		// Token: 0x040049F7 RID: 18935
		[SerializeField]
		private bool spawnOnEnter = true;

		// Token: 0x040049F8 RID: 18936
		[SerializeField]
		private bool spawnOnExit;

		// Token: 0x040049F9 RID: 18937
		[SerializeField]
		private Transform spawnLocation;
	}
}
