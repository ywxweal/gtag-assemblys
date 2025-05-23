using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

// Token: 0x020004E1 RID: 1249
public class BuilderConveyor : MonoBehaviour
{
	// Token: 0x06001E20 RID: 7712 RVA: 0x0009259C File Offset: 0x0009079C
	private void Start()
	{
		this.InitIfNeeded();
	}

	// Token: 0x06001E21 RID: 7713 RVA: 0x0009259C File Offset: 0x0009079C
	public void Setup()
	{
		this.InitIfNeeded();
	}

	// Token: 0x06001E22 RID: 7714 RVA: 0x000925A4 File Offset: 0x000907A4
	private void InitIfNeeded()
	{
		if (this.initialized)
		{
			return;
		}
		this.nextPieceToSpawn = 0;
		this.grabbedPieceTypes = new Queue<int>(10);
		this.grabbedPieceMaterials = new Queue<int>(10);
		this.setSelector.Setup(this._includeCategories);
		this.currentSet = this.setSelector.GetSelectedSet();
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentSet.subsets)
		{
			if (this._includeCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		double timeAsDouble = Time.timeAsDouble;
		this.nextSpawnTime = timeAsDouble + (double)this.spawnDelay;
		this.setSelector.OnSelectedSet.AddListener(new UnityAction<int>(this.OnSelectedSetChange));
		this.initialized = true;
		this.splineLength = this.spline.Splines[0].GetLength();
		this.maxItemsOnSpline = Mathf.RoundToInt(this.splineLength / (this.conveyorMoveSpeed * this.spawnDelay)) + 5;
		this.nativeSpline = new NativeSpline(this.spline.Splines[0], this.spline.transform.localToWorldMatrix, Allocator.Persistent);
	}

	// Token: 0x06001E23 RID: 7715 RVA: 0x00092718 File Offset: 0x00090918
	public int GetMaxItemsOnConveyor()
	{
		return Mathf.RoundToInt(this.splineLength / (this.conveyorMoveSpeed * this.spawnDelay)) + 5;
	}

	// Token: 0x06001E24 RID: 7716 RVA: 0x00092735 File Offset: 0x00090935
	public float GetFrameMovement()
	{
		return this.conveyorMoveSpeed / this.splineLength;
	}

	// Token: 0x06001E25 RID: 7717 RVA: 0x00092744 File Offset: 0x00090944
	private void OnDestroy()
	{
		if (this.setSelector != null)
		{
			this.setSelector.OnSelectedSet.RemoveListener(new UnityAction<int>(this.OnSelectedSetChange));
		}
		this.nativeSpline.Dispose();
	}

	// Token: 0x06001E26 RID: 7718 RVA: 0x0009277B File Offset: 0x0009097B
	public void OnSelectedSetChange(int setId)
	{
		if (this.table.GetTableState() != BuilderTable.TableState.Ready)
		{
			return;
		}
		this.table.RequestShelfSelection(this.shelfID, setId, true);
	}

	// Token: 0x06001E27 RID: 7719 RVA: 0x000927A0 File Offset: 0x000909A0
	public void SetSelection(int setId)
	{
		this.setSelector.SetSelection(setId);
		this.currentSet = this.setSelector.GetSelectedSet();
		this.piecesInSet.Clear();
		foreach (BuilderPieceSet.BuilderPieceSubset builderPieceSubset in this.currentSet.subsets)
		{
			if (this._includeCategories.Contains(builderPieceSubset.pieceCategory))
			{
				this.piecesInSet.AddRange(builderPieceSubset.pieceInfos);
			}
		}
		this.nextPieceToSpawn = 0;
		this.loopCount = 0;
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x0009284C File Offset: 0x00090A4C
	public int GetSelectedSetID()
	{
		return this.setSelector.GetSelectedSet().GetIntIdentifier();
	}

	// Token: 0x06001E29 RID: 7721 RVA: 0x00092860 File Offset: 0x00090A60
	public void UpdateConveyor()
	{
		if (!this.initialized)
		{
			this.Setup();
		}
		for (int i = this.piecesOnConveyor.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.piecesOnConveyor[i];
			if (builderPiece.state != BuilderPiece.State.OnConveyor)
			{
				if (PhotonNetwork.LocalPlayer.IsMasterClient && builderPiece.state != BuilderPiece.State.None)
				{
					this.grabbedPieceTypes.Enqueue(builderPiece.pieceType);
					this.grabbedPieceMaterials.Enqueue(builderPiece.materialType);
				}
				builderPiece.shelfOwner = -1;
				this.piecesOnConveyor.RemoveAt(i);
				this.table.conveyorManager.RemovePieceFromJob(builderPiece);
			}
		}
	}

	// Token: 0x06001E2A RID: 7722 RVA: 0x00092904 File Offset: 0x00090B04
	public void RemovePieceFromConveyor(Transform pieceTransform)
	{
		foreach (BuilderPiece builderPiece in this.piecesOnConveyor)
		{
			if (builderPiece.transform == pieceTransform)
			{
				this.piecesOnConveyor.Remove(builderPiece);
				builderPiece.shelfOwner = -1;
				this.table.RequestRecyclePiece(builderPiece, false, -1);
				break;
			}
		}
	}

	// Token: 0x06001E2B RID: 7723 RVA: 0x00092984 File Offset: 0x00090B84
	private Vector3 EvaluateSpline(float t)
	{
		float num;
		this._evaluateCurve = this.nativeSpline.GetCurve(this.nativeSpline.SplineToCurveT(t, out num));
		return CurveUtility.EvaluatePosition(this._evaluateCurve, num);
	}

	// Token: 0x06001E2C RID: 7724 RVA: 0x000929C4 File Offset: 0x00090BC4
	public void UpdateShelfSliced()
	{
		if (!PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			return;
		}
		if (this.shouldVerifySetSelection)
		{
			BuilderPieceSet selectedSet = this.setSelector.GetSelectedSet();
			if (selectedSet == null || !BuilderSetManager.instance.DoesAnyPlayerInRoomOwnPieceSet(selectedSet.GetIntIdentifier()))
			{
				int defaultSetID = this.setSelector.GetDefaultSetID();
				if (defaultSetID != -1)
				{
					this.OnSelectedSetChange(defaultSetID);
				}
			}
			this.shouldVerifySetSelection = false;
		}
		if (this.waitForResourceChange)
		{
			return;
		}
		double timeAsDouble = Time.timeAsDouble;
		if (timeAsDouble >= this.nextSpawnTime)
		{
			this.SpawnNextPiece();
			this.nextSpawnTime = timeAsDouble + (double)this.spawnDelay;
		}
	}

	// Token: 0x06001E2D RID: 7725 RVA: 0x00092A5A File Offset: 0x00090C5A
	public void VerifySetSelection()
	{
		this.shouldVerifySetSelection = true;
	}

	// Token: 0x06001E2E RID: 7726 RVA: 0x00092A63 File Offset: 0x00090C63
	public void OnAvailableResourcesChange()
	{
		this.waitForResourceChange = false;
	}

	// Token: 0x06001E2F RID: 7727 RVA: 0x00092A6C File Offset: 0x00090C6C
	public Transform GetSpawnTransform()
	{
		return this.spawnTransform;
	}

	// Token: 0x06001E30 RID: 7728 RVA: 0x00092A74 File Offset: 0x00090C74
	public void OnShelfPieceCreated(BuilderPiece piece, float timeOffset)
	{
		float num = timeOffset * this.conveyorMoveSpeed / this.splineLength;
		if (num > 1f)
		{
			Debug.LogWarningFormat("Piece {0} add to shelf time {1}", new object[] { piece.pieceId, num });
		}
		int count = this.piecesOnConveyor.Count;
		this.piecesOnConveyor.Add(piece);
		float num2 = Mathf.Clamp(num, 0f, 1f);
		Vector3 vector = this.EvaluateSpline(num2);
		Quaternion quaternion = this.spawnTransform.rotation * Quaternion.Euler(piece.desiredShelfRotationOffset);
		Vector3 vector2 = vector + this.spawnTransform.rotation * piece.desiredShelfOffset;
		piece.transform.SetPositionAndRotation(vector2, quaternion);
		this.table.conveyorManager.AddPieceToJob(piece, num2, this.shelfID);
	}

	// Token: 0x06001E31 RID: 7729 RVA: 0x00092B4D File Offset: 0x00090D4D
	public void OnShelfPieceRecycled(BuilderPiece piece)
	{
		this.piecesOnConveyor.Remove(piece);
		if (piece != null)
		{
			this.table.conveyorManager.RemovePieceFromJob(piece);
		}
	}

	// Token: 0x06001E32 RID: 7730 RVA: 0x00092B76 File Offset: 0x00090D76
	public void OnClearTable()
	{
		this.piecesOnConveyor.Clear();
		this.grabbedPieceTypes.Clear();
		this.grabbedPieceMaterials.Clear();
	}

	// Token: 0x06001E33 RID: 7731 RVA: 0x00092B9C File Offset: 0x00090D9C
	public void ResetConveyorState()
	{
		for (int i = this.piecesOnConveyor.Count - 1; i >= 0; i--)
		{
			BuilderPiece builderPiece = this.piecesOnConveyor[i];
			if (!(builderPiece == null))
			{
				BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
				{
					type = BuilderTable.BuilderCommandType.Recycle,
					pieceId = builderPiece.pieceId,
					localPosition = builderPiece.transform.position,
					localRotation = builderPiece.transform.rotation,
					player = NetworkSystem.Instance.LocalPlayer,
					isLeft = false,
					parentPieceId = -1
				};
				this.table.ExecutePieceRecycled(builderCommand);
			}
		}
		this.OnClearTable();
	}

	// Token: 0x06001E34 RID: 7732 RVA: 0x00092C54 File Offset: 0x00090E54
	private void SpawnNextPiece()
	{
		int num;
		int num2;
		this.FindNextAffordablePieceType(out num, out num2);
		if (num == -1)
		{
			return;
		}
		this.table.RequestCreateConveyorPiece(num, num2, this.shelfID);
	}

	// Token: 0x06001E35 RID: 7733 RVA: 0x00092C84 File Offset: 0x00090E84
	private void FindNextAffordablePieceType(out int pieceType, out int materialType)
	{
		if (this.grabbedPieceTypes.Count > 0)
		{
			pieceType = this.grabbedPieceTypes.Dequeue();
			materialType = this.grabbedPieceMaterials.Dequeue();
			return;
		}
		pieceType = -1;
		materialType = -1;
		if (this.piecesInSet.Count <= 0)
		{
			return;
		}
		for (int i = this.nextPieceToSpawn; i < this.piecesInSet.Count; i++)
		{
			BuilderPiece piecePrefab = this.piecesInSet[i].piecePrefab;
			if (this.table.HasEnoughResources(piecePrefab))
			{
				if (i + 1 >= this.piecesInSet.Count)
				{
					this.loopCount++;
					this.loopCount = Mathf.Max(0, this.loopCount);
				}
				this.nextPieceToSpawn = (i + 1) % this.piecesInSet.Count;
				pieceType = piecePrefab.name.GetStaticHash();
				materialType = this.GetMaterialType(this.piecesInSet[i]);
				return;
			}
		}
		this.loopCount++;
		this.loopCount = Mathf.Max(0, this.loopCount);
		for (int j = 0; j < this.nextPieceToSpawn; j++)
		{
			BuilderPiece piecePrefab2 = this.piecesInSet[j].piecePrefab;
			if (this.table.HasEnoughResources(piecePrefab2))
			{
				this.nextPieceToSpawn = (j + 1) % this.piecesInSet.Count;
				pieceType = piecePrefab2.name.GetStaticHash();
				materialType = this.GetMaterialType(this.piecesInSet[j]);
				return;
			}
		}
		this.waitForResourceChange = true;
	}

	// Token: 0x06001E36 RID: 7734 RVA: 0x00092E08 File Offset: 0x00091008
	private int GetMaterialType(BuilderPieceSet.PieceInfo info)
	{
		if (info.piecePrefab.materialOptions != null && info.overrideSetMaterial && info.pieceMaterialTypes.Length != 0)
		{
			int num = this.loopCount % info.pieceMaterialTypes.Length;
			string text = info.pieceMaterialTypes[num];
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogErrorFormat("Empty Material Override for piece {0} in set {1}", new object[]
				{
					info.piecePrefab.name,
					this.currentSet.name
				});
				return -1;
			}
			return text.GetHashCode();
		}
		else
		{
			if (string.IsNullOrEmpty(this.currentSet.materialId))
			{
				return -1;
			}
			return this.currentSet.materialId.GetHashCode();
		}
	}

	// Token: 0x04002167 RID: 8551
	[Header("Set Selection")]
	[SerializeField]
	private BuilderSetSelector setSelector;

	// Token: 0x04002168 RID: 8552
	public List<BuilderPieceSet.BuilderPieceCategory> _includeCategories;

	// Token: 0x04002169 RID: 8553
	[HideInInspector]
	public BuilderTable table;

	// Token: 0x0400216A RID: 8554
	public int shelfID = -1;

	// Token: 0x0400216B RID: 8555
	[Header("Conveyor Properties")]
	[SerializeField]
	private Transform spawnTransform;

	// Token: 0x0400216C RID: 8556
	[SerializeField]
	private SplineContainer spline;

	// Token: 0x0400216D RID: 8557
	private float conveyorMoveSpeed = 0.2f;

	// Token: 0x0400216E RID: 8558
	private float spawnDelay = 1.5f;

	// Token: 0x0400216F RID: 8559
	private double nextSpawnTime;

	// Token: 0x04002170 RID: 8560
	private int nextPieceToSpawn;

	// Token: 0x04002171 RID: 8561
	private BuilderPieceSet currentSet;

	// Token: 0x04002172 RID: 8562
	private int loopCount;

	// Token: 0x04002173 RID: 8563
	private List<BuilderPieceSet.PieceInfo> piecesInSet = new List<BuilderPieceSet.PieceInfo>(10);

	// Token: 0x04002174 RID: 8564
	private Queue<int> grabbedPieceTypes;

	// Token: 0x04002175 RID: 8565
	private Queue<int> grabbedPieceMaterials;

	// Token: 0x04002176 RID: 8566
	private List<BuilderPiece> piecesOnConveyor = new List<BuilderPiece>(10);

	// Token: 0x04002177 RID: 8567
	private Vector3 moveDirection;

	// Token: 0x04002178 RID: 8568
	private bool waitForResourceChange;

	// Token: 0x04002179 RID: 8569
	private bool initialized;

	// Token: 0x0400217A RID: 8570
	private float splineLength = 1f;

	// Token: 0x0400217B RID: 8571
	private int maxItemsOnSpline;

	// Token: 0x0400217C RID: 8572
	private global::UnityEngine.Splines.BezierCurve _evaluateCurve;

	// Token: 0x0400217D RID: 8573
	public NativeSpline nativeSpline;

	// Token: 0x0400217E RID: 8574
	private bool shouldVerifySetSelection;
}
