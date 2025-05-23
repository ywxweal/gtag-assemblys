using System;
using Photon.Pun;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Splines;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000B51 RID: 2897
	public class BuilderConveyorManager : MonoBehaviour
	{
		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x0600476A RID: 18282 RVA: 0x00153501 File Offset: 0x00151701
		// (set) Token: 0x0600476B RID: 18283 RVA: 0x00153508 File Offset: 0x00151708
		public static BuilderConveyorManager instance { get; private set; }

		// Token: 0x0600476C RID: 18284 RVA: 0x00153510 File Offset: 0x00151710
		private void Awake()
		{
			if (BuilderConveyorManager.instance != null && BuilderConveyorManager.instance != this)
			{
				Object.Destroy(this);
			}
			if (BuilderConveyorManager.instance == null)
			{
				BuilderConveyorManager.instance = this;
			}
		}

		// Token: 0x0600476D RID: 18285 RVA: 0x00153548 File Offset: 0x00151748
		public void UpdateManager()
		{
			foreach (BuilderConveyor builderConveyor in this.table.conveyors)
			{
				builderConveyor.UpdateConveyor();
			}
			bool flag = false;
			bool flag2 = this.pieceTransforms.length >= this.pieceTransforms.capacity - 5;
			for (int i = this.jobSplineTimes.Length - 1; i >= 0; i--)
			{
				BuilderConveyor builderConveyor2 = this.table.conveyors[this.conveyorIndices[i]];
				float num = Time.deltaTime * builderConveyor2.GetFrameMovement();
				float num2 = this.jobSplineTimes[i] + num;
				this.jobSplineTimes[i] = Mathf.Clamp(num2, 0f, 1f);
				if (PhotonNetwork.IsMasterClient && (!flag || flag2) && (double)num2 > 0.999)
				{
					builderConveyor2.RemovePieceFromConveyor(this.pieceTransforms[i]);
					this.RemovePieceFromJobAtIndex(i);
					flag = true;
				}
			}
			for (int j = this.shelfSlice; j < this.table.conveyors.Count; j += BuilderTable.SHELF_SLICE_BUCKETS)
			{
				this.table.conveyors[j].UpdateShelfSliced();
			}
			this.shelfSlice = (this.shelfSlice + 1) % BuilderTable.SHELF_SLICE_BUCKETS;
		}

		// Token: 0x0600476E RID: 18286 RVA: 0x001536C4 File Offset: 0x001518C4
		public void Setup(BuilderTable mytable)
		{
			if (this.isSetup)
			{
				return;
			}
			this.table = mytable;
			this.conveyorSplines = new NativeArray<NativeSpline>(this.table.conveyors.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.conveyorRotations = new NativeArray<Quaternion>(this.table.conveyors.Count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			int num = 0;
			for (int i = 0; i < this.table.conveyors.Count; i++)
			{
				this.conveyorSplines[i] = this.table.conveyors[i].nativeSpline;
				this.conveyorRotations[i] = this.table.conveyors[i].GetSpawnTransform().rotation;
				num += this.table.conveyors[i].GetMaxItemsOnConveyor();
			}
			this.maxItemCount = num;
			this.conveyorIndices = new NativeList<int>(this.maxItemCount, Allocator.Persistent);
			this.jobSplineTimes = new NativeList<float>(this.maxItemCount, Allocator.Persistent);
			this.jobShelfOffsets = new NativeList<Vector3>(this.maxItemCount, Allocator.Persistent);
			this.pieceTransforms = new TransformAccessArray(this.maxItemCount, 3);
			this.isSetup = true;
		}

		// Token: 0x0600476F RID: 18287 RVA: 0x00153800 File Offset: 0x00151A00
		public float GetSplineProgressForPiece(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					return this.jobSplineTimes[i];
				}
			}
			return 0f;
		}

		// Token: 0x06004770 RID: 18288 RVA: 0x00153850 File Offset: 0x00151A50
		public int GetPieceCreateTimestamp(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					BuilderConveyor builderConveyor = this.table.conveyors[this.conveyorIndices[i]];
					int num = Mathf.RoundToInt(this.jobSplineTimes[i] / builderConveyor.GetFrameMovement() * 1000f);
					return PhotonNetwork.ServerTimestamp - num;
				}
			}
			return 0;
		}

		// Token: 0x06004771 RID: 18289 RVA: 0x001538D4 File Offset: 0x00151AD4
		public void OnClearTable()
		{
			if (!this.isSetup)
			{
				return;
			}
			foreach (BuilderConveyor builderConveyor in this.table.conveyors)
			{
				builderConveyor.OnClearTable();
			}
			for (int i = this.pieceTransforms.length - 1; i >= 0; i--)
			{
				this.pieceTransforms.RemoveAtSwapBack(i);
			}
			this.jobSplineTimes.Clear();
			this.jobShelfOffsets.Clear();
			this.conveyorIndices.Clear();
		}

		// Token: 0x06004772 RID: 18290 RVA: 0x00153978 File Offset: 0x00151B78
		private void OnDestroy()
		{
			this.conveyorSplines.Dispose();
			this.conveyorRotations.Dispose();
			this.conveyorIndices.Dispose();
			this.jobSplineTimes.Dispose();
			this.jobShelfOffsets.Dispose();
			this.pieceTransforms.Dispose();
		}

		// Token: 0x06004773 RID: 18291 RVA: 0x001539C8 File Offset: 0x00151BC8
		public JobHandle ConstructJobHandle()
		{
			BuilderConveyorManager.EvaluateSplineJob evaluateSplineJob = new BuilderConveyorManager.EvaluateSplineJob
			{
				conveyorRotations = this.conveyorRotations,
				conveyorIndices = this.conveyorIndices,
				shelfOffsets = this.jobShelfOffsets,
				splineTimes = this.jobSplineTimes
			};
			for (int i = 0; i < this.conveyorSplines.Length; i++)
			{
				evaluateSplineJob.SetSplineAt(i, this.conveyorSplines[i]);
			}
			return evaluateSplineJob.Schedule(this.pieceTransforms, default(JobHandle));
		}

		// Token: 0x06004774 RID: 18292 RVA: 0x00153A54 File Offset: 0x00151C54
		public void AddPieceToJob(BuilderPiece piece, float splineTime, int conveyorID)
		{
			if (this.pieceTransforms.length >= this.pieceTransforms.capacity)
			{
				Debug.LogError("Too many pieces on conveyor!");
			}
			this.pieceTransforms.Add(piece.transform);
			this.conveyorIndices.Add(in conveyorID);
			this.jobShelfOffsets.Add(in piece.desiredShelfOffset);
			this.jobSplineTimes.Add(in splineTime);
		}

		// Token: 0x06004775 RID: 18293 RVA: 0x00153ABF File Offset: 0x00151CBF
		public void RemovePieceFromJobAtIndex(int index)
		{
			BuilderRenderer.RemoveAt(this.pieceTransforms, index);
			this.jobShelfOffsets.RemoveAt(index);
			this.jobSplineTimes.RemoveAt(index);
			this.conveyorIndices.RemoveAt(index);
		}

		// Token: 0x06004776 RID: 18294 RVA: 0x00153AF4 File Offset: 0x00151CF4
		public void RemovePieceFromJob(BuilderPiece piece)
		{
			for (int i = 0; i < this.pieceTransforms.length; i++)
			{
				if (this.pieceTransforms[i] == piece.transform)
				{
					BuilderRenderer.RemoveAt(this.pieceTransforms, i);
					this.jobShelfOffsets.RemoveAt(i);
					this.jobSplineTimes.RemoveAt(i);
					this.conveyorIndices.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x040049B9 RID: 18873
		private NativeArray<NativeSpline> conveyorSplines;

		// Token: 0x040049BA RID: 18874
		private NativeArray<Quaternion> conveyorRotations;

		// Token: 0x040049BB RID: 18875
		private NativeList<int> conveyorIndices;

		// Token: 0x040049BC RID: 18876
		private NativeList<float> jobSplineTimes;

		// Token: 0x040049BD RID: 18877
		private NativeList<Vector3> jobShelfOffsets;

		// Token: 0x040049BE RID: 18878
		private TransformAccessArray pieceTransforms;

		// Token: 0x040049BF RID: 18879
		private BuilderTable table;

		// Token: 0x040049C0 RID: 18880
		private bool isSetup;

		// Token: 0x040049C1 RID: 18881
		private int maxItemCount;

		// Token: 0x040049C2 RID: 18882
		private int shelfSlice;

		// Token: 0x02000B52 RID: 2898
		[BurstCompile]
		public struct EvaluateSplineJob : IJobParallelForTransform
		{
			// Token: 0x06004778 RID: 18296 RVA: 0x00153B61 File Offset: 0x00151D61
			public NativeSpline GetSplineAt(int index)
			{
				switch (index)
				{
				case 0:
					return this.conveyorSpline0;
				case 1:
					return this.conveyorSpline1;
				case 2:
					return this.conveyorSpline2;
				case 3:
					return this.conveyorSpline3;
				default:
					return this.conveyorSpline0;
				}
			}

			// Token: 0x06004779 RID: 18297 RVA: 0x00153B9D File Offset: 0x00151D9D
			public void SetSplineAt(int index, NativeSpline s)
			{
				switch (index)
				{
				case 0:
					this.conveyorSpline0 = s;
					return;
				case 1:
					this.conveyorSpline1 = s;
					return;
				case 2:
					this.conveyorSpline2 = s;
					return;
				case 3:
					this.conveyorSpline3 = s;
					return;
				default:
					return;
				}
			}

			// Token: 0x0600477A RID: 18298 RVA: 0x00153BD8 File Offset: 0x00151DD8
			public void Execute(int index, TransformAccess transform)
			{
				float num = this.splineTimes[index];
				Vector3 vector = this.shelfOffsets[index];
				int num2 = this.conveyorIndices[index];
				NativeSpline splineAt = this.GetSplineAt(num2);
				Quaternion quaternion = this.conveyorRotations[num2];
				float num3;
				Vector3 vector2 = CurveUtility.EvaluatePosition(splineAt.GetCurve(splineAt.SplineToCurveT(num, out num3)), num3) + quaternion * vector;
				transform.position = vector2;
			}

			// Token: 0x040049C4 RID: 18884
			public NativeSpline conveyorSpline0;

			// Token: 0x040049C5 RID: 18885
			public NativeSpline conveyorSpline1;

			// Token: 0x040049C6 RID: 18886
			public NativeSpline conveyorSpline2;

			// Token: 0x040049C7 RID: 18887
			public NativeSpline conveyorSpline3;

			// Token: 0x040049C8 RID: 18888
			[ReadOnly]
			public NativeArray<Quaternion> conveyorRotations;

			// Token: 0x040049C9 RID: 18889
			[ReadOnly]
			public NativeList<int> conveyorIndices;

			// Token: 0x040049CA RID: 18890
			[ReadOnly]
			public NativeList<float> splineTimes;

			// Token: 0x040049CB RID: 18891
			[ReadOnly]
			public NativeList<Vector3> shelfOffsets;
		}
	}
}
