using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020006B1 RID: 1713
public class SpawnPooledObject : MonoBehaviour
{
	// Token: 0x06002AC9 RID: 10953 RVA: 0x000D22C2 File Offset: 0x000D04C2
	private void Awake()
	{
		if (this._pooledObject == null)
		{
			return;
		}
		this._pooledObjectHash = PoolUtils.GameObjHashCode(this._pooledObject);
	}

	// Token: 0x06002ACA RID: 10954 RVA: 0x000D22E4 File Offset: 0x000D04E4
	public void SpawnObject()
	{
		if (!this.ShouldSpawn())
		{
			return;
		}
		if (this._pooledObject == null || this._spawnLocation == null)
		{
			return;
		}
		GameObject gameObject = ObjectPools.instance.Instantiate(this._pooledObjectHash, true);
		gameObject.transform.position = this.SpawnLocation();
		gameObject.transform.rotation = this.SpawnRotation();
		gameObject.transform.localScale = base.transform.lossyScale;
	}

	// Token: 0x06002ACB RID: 10955 RVA: 0x000D235F File Offset: 0x000D055F
	private Vector3 SpawnLocation()
	{
		return this._spawnLocation.transform.position + this.offset;
	}

	// Token: 0x06002ACC RID: 10956 RVA: 0x000D237C File Offset: 0x000D057C
	private Quaternion SpawnRotation()
	{
		Quaternion quaternion = this._spawnLocation.transform.rotation;
		if (this.facePlayer)
		{
			quaternion = Quaternion.LookRotation(GTPlayer.Instance.headCollider.transform.position - this._spawnLocation.transform.position);
		}
		if (this.upright)
		{
			quaternion.eulerAngles = new Vector3(0f, quaternion.eulerAngles.y, 0f);
		}
		return quaternion;
	}

	// Token: 0x06002ACD RID: 10957 RVA: 0x000D23FC File Offset: 0x000D05FC
	private bool ShouldSpawn()
	{
		return Random.Range(0, 100) < this.chanceToSpawn;
	}

	// Token: 0x04002FAB RID: 12203
	[SerializeField]
	private Transform _spawnLocation;

	// Token: 0x04002FAC RID: 12204
	[SerializeField]
	private GameObject _pooledObject;

	// Token: 0x04002FAD RID: 12205
	[FormerlySerializedAs("_offset")]
	public Vector3 offset;

	// Token: 0x04002FAE RID: 12206
	[FormerlySerializedAs("_upright")]
	public bool upright;

	// Token: 0x04002FAF RID: 12207
	[FormerlySerializedAs("_facePlayer")]
	public bool facePlayer;

	// Token: 0x04002FB0 RID: 12208
	[FormerlySerializedAs("_chanceToSpawn")]
	[Range(0f, 100f)]
	public int chanceToSpawn = 100;

	// Token: 0x04002FB1 RID: 12209
	private int _pooledObjectHash;
}
