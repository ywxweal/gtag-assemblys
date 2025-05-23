using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000596 RID: 1430
public class GRBreakable : MonoBehaviour
{
	// Token: 0x17000361 RID: 865
	// (get) Token: 0x060022FF RID: 8959 RVA: 0x000AF2E2 File Offset: 0x000AD4E2
	public bool BrokenLocal
	{
		get
		{
			return this.brokenLocal;
		}
	}

	// Token: 0x06002300 RID: 8960 RVA: 0x000AF2EA File Offset: 0x000AD4EA
	private void OnEnable()
	{
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06002301 RID: 8961 RVA: 0x000AF303 File Offset: 0x000AD503
	private void OnDisable()
	{
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged -= this.OnEntityStateChanged;
		}
	}

	// Token: 0x06002302 RID: 8962 RVA: 0x000AF32C File Offset: 0x000AD52C
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		GRBreakable.BreakableState breakableState = (GRBreakable.BreakableState)nextState;
		if (breakableState == GRBreakable.BreakableState.Broken)
		{
			this.BreakLocal();
			return;
		}
		if (breakableState == GRBreakable.BreakableState.Unbroken)
		{
			this.RestoreLocal();
		}
	}

	// Token: 0x06002303 RID: 8963 RVA: 0x000AF350 File Offset: 0x000AD550
	public void TryHit(GameEntity toolEntity = null)
	{
		if (!this.brokenLocal)
		{
			GhostReactorManager.instance.ReportBreakableBroken(this.gameEntity, toolEntity);
			this.BreakLocal();
		}
	}

	// Token: 0x06002304 RID: 8964 RVA: 0x000AF374 File Offset: 0x000AD574
	public void BreakLocal()
	{
		if (!this.brokenLocal)
		{
			this.brokenLocal = true;
			if (this.breakableCollider != null)
			{
				this.breakableCollider.enabled = false;
			}
			for (int i = 0; i < this.disableWhenBroken.Count; i++)
			{
				this.disableWhenBroken[i].gameObject.SetActive(false);
			}
			for (int j = 0; j < this.enableWhenBroken.Count; j++)
			{
				this.enableWhenBroken[j].gameObject.SetActive(true);
			}
			if (this.audioSource != null)
			{
				this.audioSource.PlayOneShot(this.breakSound, this.breakSoundVolume);
			}
		}
	}

	// Token: 0x06002305 RID: 8965 RVA: 0x000AF42C File Offset: 0x000AD62C
	public void RestoreLocal()
	{
		if (this.brokenLocal)
		{
			this.brokenLocal = false;
			if (this.breakableCollider != null)
			{
				this.breakableCollider.enabled = true;
			}
			for (int i = 0; i < this.disableWhenBroken.Count; i++)
			{
				this.disableWhenBroken[i].gameObject.SetActive(true);
			}
			for (int j = 0; j < this.enableWhenBroken.Count; j++)
			{
				this.enableWhenBroken[j].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x04002723 RID: 10019
	public GameEntity gameEntity;

	// Token: 0x04002724 RID: 10020
	public List<Transform> enableWhenBroken;

	// Token: 0x04002725 RID: 10021
	public List<Transform> disableWhenBroken;

	// Token: 0x04002726 RID: 10022
	public Collider breakableCollider;

	// Token: 0x04002727 RID: 10023
	public bool holdsRandomItem = true;

	// Token: 0x04002728 RID: 10024
	public Transform itemSpawnLocation;

	// Token: 0x04002729 RID: 10025
	public GRBreakableItemSpawnConfig itemSpawnProbability;

	// Token: 0x0400272A RID: 10026
	public AudioSource audioSource;

	// Token: 0x0400272B RID: 10027
	public AudioClip breakSound;

	// Token: 0x0400272C RID: 10028
	public float breakSoundVolume;

	// Token: 0x0400272D RID: 10029
	private bool brokenLocal;

	// Token: 0x02000597 RID: 1431
	public enum BreakableState
	{
		// Token: 0x0400272F RID: 10031
		Unbroken,
		// Token: 0x04002730 RID: 10032
		Broken
	}
}
