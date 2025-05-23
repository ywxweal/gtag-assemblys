using System;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace GorillaTagScripts
{
	// Token: 0x02000AF6 RID: 2806
	[BurstCompile]
	internal struct FindNearbyPiecesJob : IJobParallelForTransform
	{
		// Token: 0x0600447B RID: 17531 RVA: 0x001432EC File Offset: 0x001414EC
		public void Execute(int index, TransformAccess transform)
		{
			if (!transform.isValid)
			{
				return;
			}
			this.CheckGridPlane(index, this.leftPieceInHandIndex, transform, this.leftHandPos, true, this.leftHandGridPlanes);
			this.CheckGridPlane(index, this.rightPieceInHandIndex, transform, this.rightHandPos, false, this.rightHandGridPlanes);
		}

		// Token: 0x0600447C RID: 17532 RVA: 0x0014333C File Offset: 0x0014153C
		private void CheckGridPlane(int gridPlaneIndex, int handPieceIndex, TransformAccess transform, Vector3 handPos, bool isLeft, NativeList<BuilderGridPlaneData>.ParallelWriter checkGridPlanes)
		{
			if (handPieceIndex < 0)
			{
				return;
			}
			if ((transform.position - handPos).sqrMagnitude > this.distanceThreshSq)
			{
				return;
			}
			BuilderGridPlaneData builderGridPlaneData = this.gridPlaneData[gridPlaneIndex];
			int pieceIndex = builderGridPlaneData.pieceIndex;
			int rootPieceIndex = this.GetRootPieceIndex(pieceIndex);
			if (rootPieceIndex == handPieceIndex)
			{
				return;
			}
			if (!this.CanPiecesPotentiallySnap(this.localPlayerActorNumber, handPieceIndex, pieceIndex, rootPieceIndex, this.pieceData[pieceIndex].requestedParentPieceIndex, isLeft))
			{
				return;
			}
			transform.GetPositionAndRotation(out builderGridPlaneData.position, out builderGridPlaneData.rotation);
			checkGridPlanes.AddNoResize(builderGridPlaneData);
		}

		// Token: 0x0600447D RID: 17533 RVA: 0x001433D0 File Offset: 0x001415D0
		public bool CanPiecesPotentiallySnap(int localActorNumber, int pieceInHandIndex, int attachToPieceIndex, int attachToPieceRootIndex, int requestedParentPieceIndex, bool isLeft)
		{
			return this.CanPlayerAttachToRootPiece(localActorNumber, attachToPieceRootIndex, isLeft) && (requestedParentPieceIndex == -1 || pieceInHandIndex != this.GetRootPieceIndex(requestedParentPieceIndex));
		}

		// Token: 0x0600447E RID: 17534 RVA: 0x001433FC File Offset: 0x001415FC
		public bool CanPlayerAttachToRootPiece(int playerActorNumber, int attachToPieceRootIndex, bool isLeft)
		{
			BuilderPieceData builderPieceData = this.pieceData[attachToPieceRootIndex];
			if (builderPieceData.state != BuilderPiece.State.AttachedAndPlaced && builderPieceData.privatePlotIndex < 0 && builderPieceData.state != BuilderPiece.State.AttachedToArm)
			{
				return true;
			}
			int attachedBuiltInPiece = this.GetAttachedBuiltInPiece(attachToPieceRootIndex);
			if (attachedBuiltInPiece == -1)
			{
				return true;
			}
			BuilderPieceData builderPieceData2 = this.pieceData[attachedBuiltInPiece];
			if (builderPieceData2.privatePlotIndex < 0 && !builderPieceData2.isArmPiece)
			{
				return true;
			}
			if (builderPieceData2.isArmPiece)
			{
				if (builderPieceData2.heldByActorNumber == playerActorNumber)
				{
					int playerIndex = this.GetPlayerIndex(playerActorNumber);
					return playerIndex >= 0 && this.playerData[playerIndex].scale >= 1f;
				}
				return false;
			}
			else
			{
				if (builderPieceData2.privatePlotIndex < 0)
				{
					return true;
				}
				if (!this.CanPlayerAttachToPlot(builderPieceData2.privatePlotIndex, playerActorNumber))
				{
					return false;
				}
				if (!isLeft)
				{
					return this.privatePlotData[builderPieceData2.privatePlotIndex].isUnderCapacityRight;
				}
				return this.privatePlotData[builderPieceData2.privatePlotIndex].isUnderCapacityLeft;
			}
		}

		// Token: 0x0600447F RID: 17535 RVA: 0x001434EC File Offset: 0x001416EC
		public bool CanPlayerAttachToPlot(int privatePlotIndex, int actorNumber)
		{
			BuilderPrivatePlotData builderPrivatePlotData = this.privatePlotData[privatePlotIndex];
			return (builderPrivatePlotData.plotState == BuilderPiecePrivatePlot.PlotState.Occupied && builderPrivatePlotData.ownerActorNumber == actorNumber) || (builderPrivatePlotData.plotState == BuilderPiecePrivatePlot.PlotState.Vacant && this.localPlayerPlotIndex < 0);
		}

		// Token: 0x06004480 RID: 17536 RVA: 0x00143530 File Offset: 0x00141730
		private int GetPlayerIndex(int playerActorNumber)
		{
			for (int i = 0; i < this.playerData.Length; i++)
			{
				if (this.playerData[i].playerActorNumber == playerActorNumber)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06004481 RID: 17537 RVA: 0x0014356C File Offset: 0x0014176C
		public int GetAttachedBuiltInPiece(int pieceIndex)
		{
			BuilderPieceData builderPieceData = this.pieceData[pieceIndex];
			if (builderPieceData.isBuiltIntoTable)
			{
				return pieceIndex;
			}
			if (builderPieceData.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return -1;
			}
			int num = this.GetRootPieceIndex(pieceIndex);
			int parentPieceIndex = this.pieceData[num].parentPieceIndex;
			if (parentPieceIndex != -1)
			{
				num = parentPieceIndex;
			}
			if (this.pieceData[num].isBuiltIntoTable)
			{
				return num;
			}
			return -1;
		}

		// Token: 0x06004482 RID: 17538 RVA: 0x001435D0 File Offset: 0x001417D0
		private int GetRootPieceIndex(int pieceIndex)
		{
			int num = pieceIndex;
			while (num != -1 && this.pieceData[num].parentPieceIndex != -1 && !this.pieceData[this.pieceData[num].parentPieceIndex].isBuiltIntoTable)
			{
				num = this.pieceData[num].parentPieceIndex;
			}
			return num;
		}

		// Token: 0x0400473B RID: 18235
		[ReadOnly]
		public float distanceThreshSq;

		// Token: 0x0400473C RID: 18236
		[ReadOnly]
		public Vector3 leftHandPos;

		// Token: 0x0400473D RID: 18237
		[ReadOnly]
		public int leftPieceInHandIndex;

		// Token: 0x0400473E RID: 18238
		[ReadOnly]
		public Vector3 rightHandPos;

		// Token: 0x0400473F RID: 18239
		[ReadOnly]
		public int rightPieceInHandIndex;

		// Token: 0x04004740 RID: 18240
		[ReadOnly]
		public int localPlayerPlotIndex;

		// Token: 0x04004741 RID: 18241
		[ReadOnly]
		public int localPlayerActorNumber;

		// Token: 0x04004742 RID: 18242
		[ReadOnly]
		public NativeArray<BuilderPieceData> pieceData;

		// Token: 0x04004743 RID: 18243
		[ReadOnly]
		public NativeArray<BuilderGridPlaneData> gridPlaneData;

		// Token: 0x04004744 RID: 18244
		[ReadOnly]
		public NativeArray<BuilderPrivatePlotData> privatePlotData;

		// Token: 0x04004745 RID: 18245
		[ReadOnly]
		public NativeArray<BuilderPlayerData> playerData;

		// Token: 0x04004746 RID: 18246
		public NativeList<BuilderGridPlaneData>.ParallelWriter leftHandGridPlanes;

		// Token: 0x04004747 RID: 18247
		public NativeList<BuilderGridPlaneData>.ParallelWriter rightHandGridPlanes;
	}
}
