using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200051D RID: 1309
public class BuilderShelf : MonoBehaviour
{
	// Token: 0x06001FB8 RID: 8120 RVA: 0x000A017C File Offset: 0x0009E37C
	public void Init()
	{
		this.shelfSlot = 0;
		this.buildPieceSpawnIndex = 0;
		this.spawnCount = 0;
		this.count = 0;
		this.spawnCosts = new List<BuilderResources>(this.buildPieceSpawns.Count);
		for (int i = 0; i < this.buildPieceSpawns.Count; i++)
		{
			this.count += this.buildPieceSpawns[i].count;
			BuilderPiece component = this.buildPieceSpawns[i].buildPiecePrefab.GetComponent<BuilderPiece>();
			this.spawnCosts.Add(component.cost);
		}
	}

	// Token: 0x06001FB9 RID: 8121 RVA: 0x000A0217 File Offset: 0x0009E417
	public bool HasOpenSlot()
	{
		return this.shelfSlot < this.count;
	}

	// Token: 0x06001FBA RID: 8122 RVA: 0x000A0228 File Offset: 0x0009E428
	public void BuildNextPiece(BuilderTable table)
	{
		if (!this.HasOpenSlot())
		{
			return;
		}
		BuilderShelf.BuildPieceSpawn buildPieceSpawn = this.buildPieceSpawns[this.buildPieceSpawnIndex];
		BuilderResources builderResources = this.spawnCosts[this.buildPieceSpawnIndex];
		while (!table.HasEnoughUnreservedResources(builderResources) && this.buildPieceSpawnIndex < this.buildPieceSpawns.Count - 1)
		{
			int num = buildPieceSpawn.count - this.spawnCount;
			this.shelfSlot += num;
			this.spawnCount = 0;
			this.buildPieceSpawnIndex++;
			buildPieceSpawn = this.buildPieceSpawns[this.buildPieceSpawnIndex];
			builderResources = this.spawnCosts[this.buildPieceSpawnIndex];
		}
		if (!table.HasEnoughUnreservedResources(builderResources))
		{
			int num2 = buildPieceSpawn.count - this.spawnCount;
			this.shelfSlot += num2;
			this.spawnCount = 0;
			return;
		}
		int staticHash = buildPieceSpawn.buildPiecePrefab.name.GetStaticHash();
		int num3 = (string.IsNullOrEmpty(buildPieceSpawn.materialID) ? (-1) : buildPieceSpawn.materialID.GetHashCode());
		Vector3 vector;
		Quaternion quaternion;
		this.GetSpawnLocation(this.shelfSlot, buildPieceSpawn, out vector, out quaternion);
		int num4 = table.CreatePieceId();
		table.CreatePiece(staticHash, num4, vector, quaternion, num3, BuilderPiece.State.OnShelf, PhotonNetwork.LocalPlayer);
		this.spawnCount++;
		this.shelfSlot++;
		if (this.spawnCount >= buildPieceSpawn.count)
		{
			this.buildPieceSpawnIndex++;
			this.spawnCount = 0;
		}
	}

	// Token: 0x06001FBB RID: 8123 RVA: 0x000A03A4 File Offset: 0x0009E5A4
	public void InitCount()
	{
		this.count = 0;
		for (int i = 0; i < this.buildPieceSpawns.Count; i++)
		{
			this.count += this.buildPieceSpawns[i].count;
		}
	}

	// Token: 0x06001FBC RID: 8124 RVA: 0x000A03EC File Offset: 0x0009E5EC
	public void BuildItems(BuilderTable table)
	{
		int num = 0;
		this.InitCount();
		for (int i = 0; i < this.buildPieceSpawns.Count; i++)
		{
			BuilderShelf.BuildPieceSpawn buildPieceSpawn = this.buildPieceSpawns[i];
			if (buildPieceSpawn != null && buildPieceSpawn.count != 0)
			{
				int staticHash = buildPieceSpawn.buildPiecePrefab.name.GetStaticHash();
				int num2 = (string.IsNullOrEmpty(buildPieceSpawn.materialID) ? (-1) : buildPieceSpawn.materialID.GetHashCode());
				int num3 = 0;
				while (num3 < buildPieceSpawn.count && num < this.count)
				{
					Vector3 vector;
					Quaternion quaternion;
					this.GetSpawnLocation(num, buildPieceSpawn, out vector, out quaternion);
					int num4 = table.CreatePieceId();
					table.CreatePiece(staticHash, num4, vector, quaternion, num2, BuilderPiece.State.OnShelf, PhotonNetwork.LocalPlayer);
					num++;
					num3++;
				}
			}
		}
	}

	// Token: 0x06001FBD RID: 8125 RVA: 0x000A04B8 File Offset: 0x0009E6B8
	public void GetSpawnLocation(int slot, BuilderShelf.BuildPieceSpawn spawn, out Vector3 spawnPosition, out Quaternion spawnRotation)
	{
		if (this.center == null)
		{
			this.center = base.transform;
		}
		Vector3 vector = spawn.positionOffset;
		Vector3 vector2 = spawn.rotationOffset;
		BuilderPiece component = spawn.buildPiecePrefab.GetComponent<BuilderPiece>();
		if (component != null)
		{
			vector = component.desiredShelfOffset;
			vector2 = component.desiredShelfRotationOffset;
		}
		spawnRotation = this.center.rotation * Quaternion.Euler(vector2);
		float num = (float)slot * this.separation - (float)(this.count - 1) * this.separation / 2f;
		spawnPosition = this.center.position + this.center.rotation * (spawn.localAxis * num + vector);
	}

	// Token: 0x04002396 RID: 9110
	private int count;

	// Token: 0x04002397 RID: 9111
	public float separation;

	// Token: 0x04002398 RID: 9112
	public Transform center;

	// Token: 0x04002399 RID: 9113
	public Material overrideMaterial;

	// Token: 0x0400239A RID: 9114
	public List<BuilderShelf.BuildPieceSpawn> buildPieceSpawns;

	// Token: 0x0400239B RID: 9115
	private List<BuilderResources> spawnCosts;

	// Token: 0x0400239C RID: 9116
	private int shelfSlot;

	// Token: 0x0400239D RID: 9117
	private int buildPieceSpawnIndex;

	// Token: 0x0400239E RID: 9118
	private int spawnCount;

	// Token: 0x0200051E RID: 1310
	[Serializable]
	public class BuildPieceSpawn
	{
		// Token: 0x0400239F RID: 9119
		public GameObject buildPiecePrefab;

		// Token: 0x040023A0 RID: 9120
		public string materialID;

		// Token: 0x040023A1 RID: 9121
		public int count = 1;

		// Token: 0x040023A2 RID: 9122
		public Vector3 localAxis = Vector3.right;

		// Token: 0x040023A3 RID: 9123
		[Tooltip("Use BuilderPiece:desiredShelfOffset instead")]
		public Vector3 positionOffset;

		// Token: 0x040023A4 RID: 9124
		[Tooltip("Use BuilderPiece:desiredShelfRotationOffset instead")]
		public Vector3 rotationOffset;

		// Token: 0x040023A5 RID: 9125
		[Tooltip("Optional Editor Visual")]
		public Mesh previewMesh;
	}
}
