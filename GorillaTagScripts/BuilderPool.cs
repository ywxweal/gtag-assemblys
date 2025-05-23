using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ADF RID: 2783
	public class BuilderPool : MonoBehaviour
	{
		// Token: 0x06004368 RID: 17256 RVA: 0x00137B4C File Offset: 0x00135D4C
		public void Setup(BuilderFactory factory)
		{
			this.factory = factory;
			this.piecePools = new List<List<BuilderPiece>>(512);
			this.piecePoolLookup = new Dictionary<int, int>(512);
			this.bumpGlowPool = new List<BuilderBumpGlow>(256);
			this.AddToGlowBumpPool(256);
			this.snapOverlapPool = new List<SnapOverlap>(4096);
			this.AddToSnapOverlapPool(4096);
		}

		// Token: 0x06004369 RID: 17257 RVA: 0x00137BB8 File Offset: 0x00135DB8
		public void BuildFromShelves(List<BuilderShelf> shelves)
		{
			for (int i = 0; i < shelves.Count; i++)
			{
				BuilderShelf builderShelf = shelves[i];
				for (int j = 0; j < builderShelf.buildPieceSpawns.Count; j++)
				{
					BuilderShelf.BuildPieceSpawn buildPieceSpawn = builderShelf.buildPieceSpawns[j];
					this.AddToPool(buildPieceSpawn.buildPiecePrefab.name.GetStaticHash(), buildPieceSpawn.count);
				}
			}
		}

		// Token: 0x0600436A RID: 17258 RVA: 0x00137C20 File Offset: 0x00135E20
		public void BuildFromPieceSets()
		{
			foreach (BuilderPieceSet builderPieceSet in BuilderSetManager.instance.GetAllPieceSets())
			{
				foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in builderPieceSet.subsets)
				{
					int num = (builderPieceSet.setName.Equals("HIDDEN") ? 8 : 32);
					foreach (BuilderPieceSet.PieceInfo pieceInfo in builderPieceSubset.pieceInfos)
					{
						int staticHash = pieceInfo.piecePrefab.name.GetStaticHash();
						int count;
						if (!this.piecePoolLookup.TryGetValue(staticHash, out count))
						{
							count = this.piecePools.Count;
							this.piecePools.Add(new List<BuilderPiece>(num));
							this.piecePoolLookup.Add(staticHash, count);
							this.AddToPool(staticHash, num);
						}
					}
				}
			}
		}

		// Token: 0x0600436B RID: 17259 RVA: 0x00137D64 File Offset: 0x00135F64
		private void AddToPool(int pieceType, int count)
		{
			int count2;
			if (!this.piecePoolLookup.TryGetValue(pieceType, out count2))
			{
				count2 = this.piecePools.Count;
				this.piecePools.Add(new List<BuilderPiece>(count * 8));
				this.piecePoolLookup.Add(pieceType, count2);
				Debug.LogWarningFormat("Creating Pool for piece {0} of size {1}. Is this piece not in a piece set?", new object[]
				{
					pieceType,
					count * 8
				});
			}
			BuilderPiece piecePrefab = this.factory.GetPiecePrefab(pieceType);
			if (piecePrefab == null)
			{
				return;
			}
			List<BuilderPiece> list = this.piecePools[count2];
			for (int i = 0; i < count; i++)
			{
				BuilderPiece builderPiece = Object.Instantiate<BuilderPiece>(piecePrefab);
				builderPiece.OnCreatedByPool();
				builderPiece.gameObject.SetActive(false);
				list.Add(builderPiece);
			}
		}

		// Token: 0x0600436C RID: 17260 RVA: 0x00137E28 File Offset: 0x00136028
		public BuilderPiece CreatePiece(int pieceType, bool assertNotEmpty)
		{
			int count;
			if (!this.piecePoolLookup.TryGetValue(pieceType, out count))
			{
				if (assertNotEmpty)
				{
					Debug.LogErrorFormat("No Pool Found for {0} Adding 4", new object[] { pieceType });
				}
				count = this.piecePools.Count;
				this.AddToPool(pieceType, 4);
			}
			List<BuilderPiece> list = this.piecePools[count];
			if (list.Count == 0)
			{
				if (assertNotEmpty)
				{
					Debug.LogErrorFormat("Pool for {0} is Empty Adding 4", new object[] { pieceType });
				}
				this.AddToPool(pieceType, 4);
			}
			BuilderPiece builderPiece = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			return builderPiece;
		}

		// Token: 0x0600436D RID: 17261 RVA: 0x00137ECC File Offset: 0x001360CC
		public void DestroyPiece(BuilderPiece piece)
		{
			if (piece == null)
			{
				Debug.LogError("Why is a null piece being destroyed");
				return;
			}
			int num;
			if (!this.piecePoolLookup.TryGetValue(piece.pieceType, out num))
			{
				Debug.LogErrorFormat("No Pool Found for {0} Cannot return to pool", new object[] { piece.pieceType });
				return;
			}
			List<BuilderPiece> list = this.piecePools[num];
			if (list.Count == list.Capacity)
			{
				piece.OnReturnToPool();
				Object.Destroy(piece.gameObject);
				return;
			}
			piece.gameObject.SetActive(false);
			piece.transform.SetParent(null);
			piece.transform.SetPositionAndRotation(Vector3.up * 10000f, Quaternion.identity);
			piece.OnReturnToPool();
			list.Add(piece);
		}

		// Token: 0x0600436E RID: 17262 RVA: 0x00137F94 File Offset: 0x00136194
		private void AddToGlowBumpPool(int count)
		{
			if (this.bumpGlowPrefab == null)
			{
				Debug.LogError("Builderpool missing bump glow prefab");
				return;
			}
			for (int i = 0; i < count; i++)
			{
				BuilderBumpGlow builderBumpGlow = Object.Instantiate<BuilderBumpGlow>(this.bumpGlowPrefab);
				builderBumpGlow.gameObject.SetActive(false);
				this.bumpGlowPool.Add(builderBumpGlow);
			}
		}

		// Token: 0x0600436F RID: 17263 RVA: 0x00137FEC File Offset: 0x001361EC
		public BuilderBumpGlow CreateGlowBump()
		{
			if (this.bumpGlowPool.Count == 0)
			{
				Debug.LogError(" Glow bump Pool is Empty Adding 4");
				this.AddToGlowBumpPool(4);
			}
			BuilderBumpGlow builderBumpGlow = this.bumpGlowPool[this.bumpGlowPool.Count - 1];
			this.bumpGlowPool.RemoveAt(this.bumpGlowPool.Count - 1);
			return builderBumpGlow;
		}

		// Token: 0x06004370 RID: 17264 RVA: 0x00138048 File Offset: 0x00136248
		public void DestroyBumpGlow(BuilderBumpGlow bump)
		{
			if (bump == null)
			{
				Debug.LogError("Returning null glow bump to pool");
				return;
			}
			bump.gameObject.SetActive(false);
			bump.transform.SetPositionAndRotation(Vector3.up * 10000f, Quaternion.identity);
			this.bumpGlowPool.Add(bump);
		}

		// Token: 0x06004371 RID: 17265 RVA: 0x001380A0 File Offset: 0x001362A0
		private void AddToSnapOverlapPool(int count)
		{
			this.snapOverlapPool.Capacity = this.snapOverlapPool.Capacity + count;
			for (int i = 0; i < count; i++)
			{
				this.snapOverlapPool.Add(new SnapOverlap());
			}
		}

		// Token: 0x06004372 RID: 17266 RVA: 0x001380E4 File Offset: 0x001362E4
		public SnapOverlap CreateSnapOverlap(BuilderAttachGridPlane otherPlane, SnapBounds bounds)
		{
			if (this.snapOverlapPool.Count == 0)
			{
				Debug.LogError("Snap Overlap Pool is Empty Adding 1024");
				this.AddToSnapOverlapPool(1024);
			}
			SnapOverlap snapOverlap = this.snapOverlapPool[this.snapOverlapPool.Count - 1];
			this.snapOverlapPool.RemoveAt(this.snapOverlapPool.Count - 1);
			snapOverlap.otherPlane = otherPlane;
			snapOverlap.bounds = bounds;
			snapOverlap.nextOverlap = null;
			return snapOverlap;
		}

		// Token: 0x06004373 RID: 17267 RVA: 0x00138158 File Offset: 0x00136358
		public void DestroySnapOverlap(SnapOverlap snapOverlap)
		{
			snapOverlap.otherPlane = null;
			snapOverlap.nextOverlap = null;
			this.snapOverlapPool.Add(snapOverlap);
		}

		// Token: 0x06004374 RID: 17268 RVA: 0x00138174 File Offset: 0x00136374
		private void OnDestroy()
		{
			for (int i = 0; i < this.piecePools.Count; i++)
			{
				if (this.piecePools[i] != null)
				{
					foreach (BuilderPiece builderPiece in this.piecePools[i])
					{
						if (builderPiece != null)
						{
							Object.Destroy(builderPiece);
						}
					}
					this.piecePools[i].Clear();
				}
			}
			this.piecePoolLookup.Clear();
			foreach (BuilderBumpGlow builderBumpGlow in this.bumpGlowPool)
			{
				Object.Destroy(builderBumpGlow);
			}
			this.bumpGlowPool.Clear();
		}

		// Token: 0x040045EB RID: 17899
		public List<List<BuilderPiece>> piecePools;

		// Token: 0x040045EC RID: 17900
		public Dictionary<int, int> piecePoolLookup;

		// Token: 0x040045ED RID: 17901
		[HideInInspector]
		public List<BuilderBumpGlow> bumpGlowPool;

		// Token: 0x040045EE RID: 17902
		public BuilderBumpGlow bumpGlowPrefab;

		// Token: 0x040045EF RID: 17903
		[HideInInspector]
		public List<SnapOverlap> snapOverlapPool;

		// Token: 0x040045F0 RID: 17904
		private const int INITIAL_POOL_SIZE = 32;

		// Token: 0x040045F1 RID: 17905
		private const int HIDDEN_PIECE_POOL_SIZE = 8;

		// Token: 0x040045F2 RID: 17906
		private BuilderFactory factory;
	}
}
