﻿using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x020004F8 RID: 1272
public class BuilderPieceInteractor : MonoBehaviour
{
	// Token: 0x06001EEB RID: 7915 RVA: 0x00097754 File Offset: 0x00095954
	private void Awake()
	{
		if (BuilderPieceInteractor.instance == null)
		{
			BuilderPieceInteractor.instance = this;
			BuilderPieceInteractor.hasInstance = true;
		}
		else if (BuilderPieceInteractor.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		this.velocityEstimator = new List<GorillaVelocityEstimator>(2) { this.velocityEstimatorLeft, this.velocityEstimatorRight };
		this.laserSight = new List<BuilderLaserSight>(2) { this.laserSightLeft, this.laserSightRight };
		this.handState = new List<BuilderPieceInteractor.HandState>(2);
		this.heldPiece = new List<BuilderPiece>(2);
		this.potentialHeldPiece = new List<BuilderPiece>(2);
		this.potentialGrabbedOffsetDist = new List<float>(2);
		this.heldInitialRot = new List<Quaternion>(2);
		this.heldCurrentRot = new List<Quaternion>(2);
		this.heldInitialPos = new List<Vector3>(2);
		this.heldCurrentPos = new List<Vector3>(2);
		this.heldChainLength = new int[2];
		this.heldChainLength[0] = 0;
		this.heldChainLength[1] = 0;
		this.heldChainCost = new List<int[]>(2);
		for (int i = 0; i < 2; i++)
		{
			this.heldChainCost.Add(new int[3]);
		}
		BuilderPieceInteractor.allPotentialPlacements = new List<BuilderPotentialPlacement>[2];
		this.delayedPotentialPlacement = new List<BuilderPotentialPlacement>(2);
		this.delayedPlacementTime = new List<float>(2);
		this.prevPotentialPlacement = new List<BuilderPotentialPlacement>(2);
		this.glowBumps = new List<List<BuilderBumpGlow>>(2);
		for (int j = 0; j < 2; j++)
		{
			this.handState.Add(BuilderPieceInteractor.HandState.Empty);
			this.heldPiece.Add(null);
			this.potentialHeldPiece.Add(null);
			this.potentialGrabbedOffsetDist.Add(0f);
			this.heldInitialRot.Add(Quaternion.identity);
			this.heldCurrentRot.Add(Quaternion.identity);
			this.heldInitialPos.Add(Vector3.zero);
			this.heldCurrentPos.Add(Vector3.zero);
			this.delayedPotentialPlacement.Add(default(BuilderPotentialPlacement));
			this.delayedPlacementTime.Add(-1f);
			this.prevPotentialPlacement.Add(default(BuilderPotentialPlacement));
			BuilderPieceInteractor.allPotentialPlacements[j] = new List<BuilderPotentialPlacement>(128);
			this.glowBumps.Add(new List<BuilderBumpGlow>(1024));
		}
		this.checkPiecesInSphere = new NativeArray<OverlapSphereCommand>(2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.checkPiecesInSphereResults = new NativeArray<ColliderHit>(2048, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.grabSphereCast = new NativeArray<SpherecastCommand>(2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.grabSphereCastResults = new NativeArray<RaycastHit>(128, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		BuilderPieceInteractor.handGridPlaneData = new NativeList<BuilderGridPlaneData>[2];
		BuilderPieceInteractor.handPieceData = new NativeList<BuilderPieceData>[2];
		BuilderPieceInteractor.localAttachableGridPlaneData = new NativeList<BuilderGridPlaneData>[2];
		BuilderPieceInteractor.localAttachablePieceData = new NativeList<BuilderPieceData>[2];
		for (int k = 0; k < 2; k++)
		{
			BuilderPieceInteractor.handGridPlaneData[k] = new NativeList<BuilderGridPlaneData>(512, Allocator.Persistent);
			BuilderPieceInteractor.handPieceData[k] = new NativeList<BuilderPieceData>(512, Allocator.Persistent);
			BuilderPieceInteractor.localAttachableGridPlaneData[k] = new NativeList<BuilderGridPlaneData>(10240, Allocator.Persistent);
			BuilderPieceInteractor.localAttachablePieceData[k] = new NativeList<BuilderPieceData>(2560, Allocator.Persistent);
		}
	}

	// Token: 0x06001EEC RID: 7916 RVA: 0x000023F4 File Offset: 0x000005F4
	public void PreInteract()
	{
	}

	// Token: 0x06001EED RID: 7917 RVA: 0x00097A8C File Offset: 0x00095C8C
	public void StartFindNearbyPieces()
	{
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(offlineVRRig.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		if (!builderTable.isTableMutable)
		{
			return;
		}
		QueryParameters queryParameters = new QueryParameters
		{
			layerMask = builderTable.allPiecesMask
		};
		this.checkPiecesInSphere[0] = new OverlapSphereCommand(offlineVRRig.leftHand.overrideTarget.position, (this.handState[0] == BuilderPieceInteractor.HandState.Empty) ? 0.0375f : 1f, queryParameters);
		this.checkPiecesInSphere[1] = new OverlapSphereCommand(offlineVRRig.rightHand.overrideTarget.position, (this.handState[1] == BuilderPieceInteractor.HandState.Empty) ? 0.0375f : 1f, queryParameters);
		this.checkNearbyPiecesHandle = OverlapSphereCommand.ScheduleBatch(this.checkPiecesInSphere, this.checkPiecesInSphereResults, 1, 1024, default(JobHandle));
		for (int i = 0; i < 64; i++)
		{
			this.grabSphereCastResults[i] = this.emptyRaycastHit;
		}
		this.grabSphereCast[0] = new SpherecastCommand(offlineVRRig.leftHand.overrideTarget.position, 0.0375f, offlineVRRig.leftHand.overrideTarget.rotation * Vector3.right, queryParameters, 0.15f);
		this.grabSphereCast[1] = new SpherecastCommand(offlineVRRig.rightHand.overrideTarget.position, 0.0375f, offlineVRRig.rightHand.overrideTarget.rotation * -Vector3.right, queryParameters, 0.15f);
		this.findPiecesToGrab = SpherecastCommand.ScheduleBatch(this.grabSphereCast, this.grabSphereCastResults, 1, 64, default(JobHandle));
		JobHandle.ScheduleBatchedJobs();
	}

	// Token: 0x06001EEE RID: 7918 RVA: 0x00097C5C File Offset: 0x00095E5C
	private void CalcLocalGridPlanes()
	{
		this.checkNearbyPiecesHandle.Complete();
		for (int i = 0; i < 2; i++)
		{
			if (this.handState[i] == BuilderPieceInteractor.HandState.Grabbed)
			{
				BuilderPieceInteractor.localAttachableGridPlaneData[i].Clear();
				BuilderPieceInteractor.localAttachablePieceData[i].Clear();
				BuilderPieceInteractor.tempPieceSet.Clear();
				if (this.currentTable.IsInBuilderZone())
				{
					for (int j = 0; j < 1024; j++)
					{
						int num = i * 1024 + j;
						if (this.checkPiecesInSphereResults[num].instanceID == 0)
						{
							break;
						}
						BuilderPiece builderPiece = this.heldPiece[i];
						BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.checkPiecesInSphereResults[num].collider);
						if (builderPieceFromCollider != null && !BuilderPieceInteractor.tempPieceSet.Contains(builderPieceFromCollider))
						{
							BuilderPieceInteractor.tempPieceSet.Add(builderPieceFromCollider);
							if (this.currentTable.CanPiecesPotentiallySnap(builderPiece, builderPieceFromCollider))
							{
								int length = BuilderPieceInteractor.localAttachablePieceData[i].Length;
								NativeList<BuilderPieceData>[] array = BuilderPieceInteractor.localAttachablePieceData;
								int num2 = i;
								BuilderPieceData builderPieceData = new BuilderPieceData(builderPieceFromCollider);
								array[num2].Add(in builderPieceData);
								for (int k = 0; k < builderPieceFromCollider.gridPlanes.Count; k++)
								{
									NativeList<BuilderGridPlaneData>[] array2 = BuilderPieceInteractor.localAttachableGridPlaneData;
									int num3 = i;
									BuilderGridPlaneData builderGridPlaneData = new BuilderGridPlaneData(builderPieceFromCollider.gridPlanes[k], length);
									array2[num3].Add(in builderGridPlaneData);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001EEF RID: 7919 RVA: 0x00097DE4 File Offset: 0x00095FE4
	private void OnDestroy()
	{
		if (BuilderPieceInteractor.instance == this)
		{
			BuilderPieceInteractor.hasInstance = false;
			BuilderPieceInteractor.instance = null;
		}
		if (this.checkPiecesInSphere.IsCreated)
		{
			this.checkPiecesInSphere.Dispose();
		}
		if (this.checkPiecesInSphereResults.IsCreated)
		{
			this.checkPiecesInSphereResults.Dispose();
		}
		if (this.grabSphereCast.IsCreated)
		{
			this.grabSphereCast.Dispose();
		}
		if (this.grabSphereCastResults.IsCreated)
		{
			this.grabSphereCastResults.Dispose();
		}
		for (int i = 0; i < 2; i++)
		{
			if (BuilderPieceInteractor.handGridPlaneData[i].IsCreated)
			{
				BuilderPieceInteractor.handGridPlaneData[i].Dispose();
			}
			if (BuilderPieceInteractor.handPieceData[i].IsCreated)
			{
				BuilderPieceInteractor.handPieceData[i].Dispose();
			}
			if (BuilderPieceInteractor.localAttachableGridPlaneData[i].IsCreated)
			{
				BuilderPieceInteractor.localAttachableGridPlaneData[i].Dispose();
			}
			if (BuilderPieceInteractor.localAttachablePieceData[i].IsCreated)
			{
				BuilderPieceInteractor.localAttachablePieceData[i].Dispose();
			}
		}
	}

	// Token: 0x06001EF0 RID: 7920 RVA: 0x00097F08 File Offset: 0x00096108
	public bool BlockSnowballCreation()
	{
		BuilderTable builderTable;
		return !(GorillaTagger.Instance == null) && BuilderTable.TryGetBuilderTableForZone(GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone, out builderTable) && (builderTable.IsInBuilderZone() && builderTable.isTableMutable && GorillaTagger.Instance.offlineVRRig.scaleFactor >= 0.99f);
	}

	// Token: 0x06001EF1 RID: 7921 RVA: 0x00097F6C File Offset: 0x0009616C
	public void OnLateUpdate()
	{
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(offlineVRRig.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		if (!builderTable.isTableMutable)
		{
			return;
		}
		this.currentTable = builderTable;
		this.CalcLocalGridPlanes();
		BodyDockPositions myBodyDockPositions = offlineVRRig.myBodyDockPositions;
		this.findPiecesToGrab.Complete();
		this.UpdateHandState(BuilderPieceInteractor.HandType.Left, offlineVRRig.leftHand.overrideTarget, Vector3.right, myBodyDockPositions.leftHandTransform, this.equipmentInteractor.isLeftGrabbing, this.equipmentInteractor.wasLeftGrabPressed, this.equipmentInteractor.leftHandHeldEquipment, this.equipmentInteractor.disableLeftGrab);
		this.UpdateHandState(BuilderPieceInteractor.HandType.Right, offlineVRRig.rightHand.overrideTarget, -Vector3.right, myBodyDockPositions.rightHandTransform, this.equipmentInteractor.isRightGrabbing, this.equipmentInteractor.wasRightGrabPressed, this.equipmentInteractor.rightHandHeldEquipment, this.equipmentInteractor.disableRightGrab);
		this.UpdatePieceDisables();
		if (offlineVRRig != null)
		{
			bool flag = offlineVRRig.scaleFactor < 1f;
			if (flag && !this.isRigSmall)
			{
				if (offlineVRRig.builderArmShelfLeft != null)
				{
					offlineVRRig.builderArmShelfLeft.DropAttachedPieces();
					if (offlineVRRig.builderArmShelfLeft.piece != null)
					{
						foreach (Collider collider in offlineVRRig.builderArmShelfLeft.piece.colliders)
						{
							collider.enabled = false;
						}
					}
				}
				if (!(offlineVRRig.builderArmShelfRight != null))
				{
					goto IL_02C0;
				}
				offlineVRRig.builderArmShelfRight.DropAttachedPieces();
				if (!(offlineVRRig.builderArmShelfRight.piece != null))
				{
					goto IL_02C0;
				}
				using (List<Collider>.Enumerator enumerator = offlineVRRig.builderArmShelfRight.piece.colliders.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Collider collider2 = enumerator.Current;
						collider2.enabled = false;
					}
					goto IL_02C0;
				}
			}
			if (!flag && this.isRigSmall)
			{
				if (offlineVRRig.builderArmShelfLeft != null && offlineVRRig.builderArmShelfLeft.piece != null)
				{
					foreach (Collider collider3 in offlineVRRig.builderArmShelfLeft.piece.colliders)
					{
						collider3.enabled = true;
					}
				}
				if (offlineVRRig.builderArmShelfRight != null && offlineVRRig.builderArmShelfRight.piece != null)
				{
					foreach (Collider collider4 in offlineVRRig.builderArmShelfRight.piece.colliders)
					{
						collider4.enabled = true;
					}
				}
			}
			IL_02C0:
			this.isRigSmall = flag;
		}
	}

	// Token: 0x06001EF2 RID: 7922 RVA: 0x00098274 File Offset: 0x00096474
	private void SetHandState(int handIndex, BuilderPieceInteractor.HandState newState)
	{
		if (this.handState[handIndex] == BuilderPieceInteractor.HandState.Empty && this.potentialHeldPiece[handIndex] != null)
		{
			this.potentialHeldPiece[handIndex].PotentialGrab(false);
			this.potentialHeldPiece[handIndex] = null;
		}
		this.handState[handIndex] = newState;
		switch (this.handState[handIndex])
		{
		case BuilderPieceInteractor.HandState.Empty:
		{
			this.heldChainLength[handIndex] = 0;
			for (int i = 0; i < this.heldChainCost[handIndex].Length; i++)
			{
				this.heldChainCost[handIndex][i] = 0;
			}
			break;
		}
		case BuilderPieceInteractor.HandState.Grabbed:
			this.heldChainLength[handIndex] = this.heldPiece[handIndex].GetChildCount() + 1;
			this.heldPiece[handIndex].GetChainCost(this.heldChainCost[handIndex]);
			return;
		case BuilderPieceInteractor.HandState.PotentialGrabbed:
		{
			this.heldChainLength[handIndex] = 0;
			for (int j = 0; j < this.heldChainCost[handIndex].Length; j++)
			{
				this.heldChainCost[handIndex][j] = 0;
			}
			return;
		}
		case BuilderPieceInteractor.HandState.WaitForGrabbed:
			break;
		default:
			return;
		}
	}

	// Token: 0x06001EF3 RID: 7923 RVA: 0x00098390 File Offset: 0x00096590
	public void OnCountChangedForRoot(BuilderPiece piece)
	{
		if (piece == null)
		{
			return;
		}
		if (this.heldPiece[0] != null && this.heldPiece[0].Equals(piece))
		{
			this.heldChainLength[0] = this.heldPiece[0].GetChainCostAndCount(this.heldChainCost[0]);
			return;
		}
		if (this.heldPiece[1] != null && this.heldPiece[1].Equals(piece))
		{
			this.heldChainLength[1] = this.heldPiece[1].GetChainCostAndCount(this.heldChainCost[1]);
		}
	}

	// Token: 0x06001EF4 RID: 7924 RVA: 0x00098444 File Offset: 0x00096644
	private void UpdateHandState(BuilderPieceInteractor.HandType handType, Transform handTransform, Vector3 palmForwardLocal, Transform handAttachPoint, bool isGrabbing, bool wasGrabPressed, IHoldableObject heldEquipment, bool grabDisabled)
	{
		int num = (int)((handType + 1) % (BuilderPieceInteractor.HandType)2);
		bool flag = GorillaTagger.Instance.offlineVRRig.scaleFactor < 1f;
		bool flag2 = isGrabbing && !wasGrabPressed;
		bool flag3 = this.heldPiece[(int)handType] != null && (!isGrabbing || flag);
		bool flag4 = heldEquipment != null;
		bool flag5 = this.heldPiece[(int)handType] != null;
		bool flag6 = !flag4 && !flag5 && !grabDisabled && !flag && this.currentTable.IsInBuilderZone();
		BuilderPiece builderPiece = null;
		Vector3 position = handTransform.position;
		handTransform.rotation * palmForwardLocal;
		Vector3 zero = Vector3.zero;
		switch (this.handState[(int)handType])
		{
		case BuilderPieceInteractor.HandState.Empty:
			if (!flag)
			{
				float num2 = float.MaxValue;
				for (int i = 0; i < 1024; i++)
				{
					int num3 = (int)(handType * (BuilderPieceInteractor.HandType)1024 + i);
					ColliderHit colliderHit = this.checkPiecesInSphereResults[num3];
					if (colliderHit.instanceID == 0)
					{
						break;
					}
					BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(colliderHit.collider);
					if (builderPieceFromCollider != null && !builderPieceFromCollider.isBuiltIntoTable)
					{
						float num4 = Vector3.SqrMagnitude(colliderHit.collider.transform.position - handTransform.position);
						if ((builderPiece == null || num4 < num2) && builderPieceFromCollider.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, builderPieceFromCollider.transform.position))
						{
							builderPiece = builderPieceFromCollider;
							num2 = num4;
						}
					}
				}
				if (builderPiece == null)
				{
					for (int j = 0; j < 64; j++)
					{
						int num5 = (int)(handType * (BuilderPieceInteractor.HandType)64 + j);
						RaycastHit raycastHit = this.grabSphereCastResults[num5];
						if (raycastHit.colliderInstanceID == 0)
						{
							break;
						}
						BuilderPiece builderPieceFromCollider2 = BuilderPiece.GetBuilderPieceFromCollider(raycastHit.collider);
						if (builderPieceFromCollider2 != null && !builderPieceFromCollider2.isBuiltIntoTable && (builderPiece == null || raycastHit.distance < num2) && builderPieceFromCollider2.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, builderPieceFromCollider2.transform.position))
						{
							builderPiece = builderPieceFromCollider2;
							num2 = raycastHit.distance;
						}
					}
				}
			}
			if (this.potentialHeldPiece[(int)handType] != builderPiece)
			{
				if (this.potentialHeldPiece[(int)handType] != null)
				{
					this.potentialHeldPiece[(int)handType].PotentialGrab(false);
				}
				this.potentialHeldPiece[(int)handType] = builderPiece;
				if (this.potentialHeldPiece[(int)handType] != null)
				{
					this.potentialHeldPiece[(int)handType].PotentialGrab(true);
				}
			}
			if (flag6 && flag2 && builderPiece != null)
			{
				Vector3 vector;
				Quaternion quaternion;
				this.CalcPieceLocalPosAndRot(builderPiece.transform.position, builderPiece.transform.rotation, handAttachPoint, out vector, out quaternion);
				if (BuilderPiece.IsDroppedState(builderPiece.state) || this.heldPiece[num] == builderPiece)
				{
					builderPiece.PlayGrabbedFx();
					this.currentTable.RequestGrabPiece(builderPiece, handType == BuilderPieceInteractor.HandType.Left, vector, quaternion);
					return;
				}
				builderPiece.PlayGrabbedFx();
				this.SetHandState((int)handType, BuilderPieceInteractor.HandState.PotentialGrabbed);
				this.potentialGrabbedOffsetDist[(int)handType] = 0f;
				this.heldPiece[(int)handType] = builderPiece;
				this.heldInitialRot[(int)handType] = quaternion;
				this.heldCurrentRot[(int)handType] = quaternion;
				this.heldInitialPos[(int)handType] = vector;
				this.heldCurrentPos[(int)handType] = vector;
				return;
			}
			break;
		case BuilderPieceInteractor.HandState.Grabbed:
		{
			if (flag3)
			{
				Vector3 vector2 = this.velocityEstimator[(int)handType].linearVelocity;
				if (flag)
				{
					Vector3 vector3 = this.currentTable.roomCenter.position - this.velocityEstimator[(int)handType].handPos;
					vector3.Normalize();
					Vector3 vector4 = Quaternion.Euler(0f, 180f, 0f) * vector3;
					vector2 = BuilderTable.DROP_ZONE_REPEL * vector4;
				}
				else if (this.prevPotentialPlacement[(int)handType].attachPiece == this.heldPiece[(int)handType] && this.prevPotentialPlacement[(int)handType].parentPiece != null)
				{
					Vector3 vector5 = this.prevPotentialPlacement[(int)handType].parentPiece.gridPlanes[this.prevPotentialPlacement[(int)handType].parentAttachIndex].transform.TransformDirection(this.prevPotentialPlacement[(int)handType].attachPlaneNormal);
					vector2 += vector5 * 1f;
				}
				this.currentTable.TryDropPiece(handType == BuilderPieceInteractor.HandType.Left, this.heldPiece[(int)handType], vector2, this.velocityEstimator[(int)handType].angularVelocity);
				return;
			}
			BuilderPiece builderPiece2 = this.heldPiece[(int)handType];
			if (builderPiece2 != null)
			{
				builderPiece2.transform.localRotation = this.heldInitialRot[(int)handType];
				builderPiece2.transform.localPosition = this.heldInitialPos[(int)handType];
				Quaternion quaternion2 = this.heldCurrentRot[(int)handType];
				Vector3 vector6 = this.heldCurrentPos[(int)handType];
				NativeList<BuilderGridPlaneData>[] array = BuilderPieceInteractor.localAttachableGridPlaneData;
				BuilderPieceInteractor.handPieceData[(int)handType].Clear();
				BuilderPieceInteractor.handGridPlaneData[(int)handType].Clear();
				BuilderTableJobs.BuildTestPieceListForJob(builderPiece2, BuilderPieceInteractor.handPieceData[(int)handType], BuilderPieceInteractor.handGridPlaneData[(int)handType]);
				BuilderPieceInteractor.allPotentialPlacements[(int)handType].Clear();
				BuilderPotentialPlacement builderPotentialPlacement;
				bool flag7 = this.currentTable.TryPlacePieceOnTableNoDropJobs(BuilderPieceInteractor.handGridPlaneData[(int)handType], BuilderPieceInteractor.handPieceData[(int)handType], BuilderPieceInteractor.localAttachableGridPlaneData[(int)handType], BuilderPieceInteractor.localAttachablePieceData[(int)handType], out builderPotentialPlacement, BuilderPieceInteractor.allPotentialPlacements[(int)handType]);
				if (flag7)
				{
					BuilderPiece.State state = builderPotentialPlacement.attachPiece.state;
					BuilderPiece.State state2 = ((builderPotentialPlacement.parentPiece == null) ? BuilderPiece.State.None : builderPotentialPlacement.parentPiece.state);
					bool flag8 = state == BuilderPiece.State.Grabbed || state == BuilderPiece.State.GrabbedLocal;
					bool flag9 = state2 == BuilderPiece.State.Grabbed || state2 == BuilderPiece.State.GrabbedLocal;
					bool flag10 = flag8 && flag9;
					int num6 = (int)((handType + 1) % (BuilderPieceInteractor.HandType)2);
					BuilderPieceInteractor.HandState handState = this.handState[num6];
					if (flag10 && builderPotentialPlacement.attachPiece.gridPlanes[builderPotentialPlacement.attachIndex].male)
					{
						flag7 = false;
					}
					else if (flag10 && handState == BuilderPieceInteractor.HandState.WaitingForSnap)
					{
						flag7 = false;
					}
				}
				if (flag7)
				{
					for (int k = 0; k < BuilderPieceInteractor.allPotentialPlacements[(int)handType].Count; k++)
					{
						BuilderPotentialPlacement builderPotentialPlacement2 = BuilderPieceInteractor.allPotentialPlacements[(int)handType][k];
						BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement2.attachPiece.gridPlanes[builderPotentialPlacement2.attachIndex];
						BuilderAttachGridPlane builderAttachGridPlane2 = ((builderPotentialPlacement2.parentPiece == null) ? null : builderPotentialPlacement2.parentPiece.gridPlanes[builderPotentialPlacement2.parentAttachIndex]);
						bool flag11 = builderAttachGridPlane.IsConnected(builderPotentialPlacement2.attachBounds);
						if (!flag11)
						{
							flag11 = builderAttachGridPlane2.IsConnected(builderPotentialPlacement2.parentAttachBounds);
						}
						if (flag11)
						{
							flag7 = false;
							break;
						}
					}
				}
				if (flag7)
				{
					Vector3 vector7 = builderPotentialPlacement.localPosition;
					Quaternion quaternion3 = builderPotentialPlacement.localRotation;
					Vector3 vector8 = builderPotentialPlacement.attachPlaneNormal;
					if (builderPotentialPlacement.parentPiece != null)
					{
						BuilderAttachGridPlane builderAttachGridPlane3 = builderPotentialPlacement.parentPiece.gridPlanes[builderPotentialPlacement.parentAttachIndex];
						vector7 = builderAttachGridPlane3.transform.TransformPoint(builderPotentialPlacement.localPosition);
						quaternion3 = builderAttachGridPlane3.transform.rotation * builderPotentialPlacement.localRotation;
						vector8 = builderAttachGridPlane3.transform.TransformDirection(builderPotentialPlacement.attachPlaneNormal);
					}
					Vector3 vector9 = handAttachPoint.transform.InverseTransformPoint(vector7);
					Quaternion quaternion4 = Quaternion.Inverse(handAttachPoint.transform.rotation) * quaternion3;
					float attachDistance = builderPotentialPlacement.attachDistance;
					float num7 = Mathf.InverseLerp(this.currentTable.pushAndEaseParams.snapDelayOffsetDist, this.currentTable.pushAndEaseParams.maxOffsetY, attachDistance);
					num7 = Mathf.Clamp(num7, 0f, 1f);
					bool flag12 = builderPotentialPlacement.attachPiece == builderPiece2;
					bool flag13 = builderPotentialPlacement.attachPiece == builderPiece2;
					if (flag12)
					{
						Quaternion quaternion5 = this.heldInitialRot[(int)handType];
						Quaternion quaternion6 = Quaternion.Slerp(quaternion4, quaternion5, num7);
						quaternion2 = Quaternion.Slerp(quaternion2, quaternion6, 0.1f);
					}
					if (flag13)
					{
						Vector3 vector10 = this.heldInitialPos[(int)handType];
						Vector3 vector11 = handAttachPoint.transform.InverseTransformDirection(vector8);
						Vector3 vector12 = vector9 + vector11 * this.currentTable.pushAndEaseParams.snapDelayOffsetDist - vector10;
						float num8 = Vector3.Dot(vector12, vector11);
						num8 = Mathf.Min(0f, num8);
						Vector3 vector13 = vector11 * num8;
						Vector3 vector14 = vector12 - vector13;
						Vector3 vector15 = vector10 + Vector3.Lerp(vector14, Vector3.zero, num7);
						vector6 = Vector3.Lerp(vector6, vector15, 0.5f);
					}
					this.heldCurrentRot[(int)handType] = quaternion2;
					this.heldCurrentPos[(int)handType] = vector6;
					builderPiece2.transform.localRotation = quaternion2;
					builderPiece2.transform.localPosition = vector6;
					bool flag14 = Vector3.Dot(this.velocityEstimator[(int)handType].linearVelocity, vector8) > 0f;
					float snapAttachDistance = this.currentTable.pushAndEaseParams.snapAttachDistance;
					if (builderPotentialPlacement.attachDistance < snapAttachDistance && !flag14 && BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, builderPiece2, builderPotentialPlacement.parentPiece))
					{
						GorillaTagger.Instance.StartVibration(handType == BuilderPieceInteractor.HandType.Left, GorillaTagger.Instance.tapHapticStrength, this.currentTable.pushAndEaseParams.snapDelayTime * 2f);
						if (((builderPotentialPlacement.parentPiece == null) ? BuilderPiece.State.None : builderPotentialPlacement.parentPiece.state) == BuilderPiece.State.GrabbedLocal)
						{
							GorillaTagger.Instance.StartVibration(handType > BuilderPieceInteractor.HandType.Left, GorillaTagger.Instance.tapHapticStrength, this.currentTable.pushAndEaseParams.snapDelayTime * 2f);
						}
						this.delayedPotentialPlacement[(int)handType] = builderPotentialPlacement;
						this.delayedPlacementTime[(int)handType] = 0f;
						this.SetHandState((int)handType, BuilderPieceInteractor.HandState.WaitingForSnap);
					}
					else
					{
						float num9 = this.currentTable.gridSize * 0.5f * (this.currentTable.gridSize * 0.5f);
						if (this.prevPotentialPlacement[(int)handType].attachPiece != builderPotentialPlacement.attachPiece || this.prevPotentialPlacement[(int)handType].parentPiece != builderPotentialPlacement.parentPiece || this.prevPotentialPlacement[(int)handType].attachIndex != builderPotentialPlacement.attachIndex || this.prevPotentialPlacement[(int)handType].parentAttachIndex != builderPotentialPlacement.parentAttachIndex || Vector3.SqrMagnitude(this.prevPotentialPlacement[(int)handType].localPosition - builderPotentialPlacement.localPosition) > num9)
						{
							GorillaTagger.Instance.StartVibration(handType == BuilderPieceInteractor.HandType.Left, GorillaTagger.Instance.tapHapticStrength * 0.15f, this.currentTable.pushAndEaseParams.snapDelayTime);
							try
							{
								this.ClearGlowBumps((int)handType);
								this.AddGlowBumps((int)handType, BuilderPieceInteractor.allPotentialPlacements[(int)handType]);
							}
							catch (Exception ex)
							{
								Debug.LogErrorFormat("Error adding glow bumps {0}", new object[] { ex.ToString() });
							}
						}
					}
					this.UpdateGlowBumps((int)handType, 1f - num7);
					this.prevPotentialPlacement[(int)handType] = builderPotentialPlacement;
					return;
				}
				this.ClearGlowBumps((int)handType);
				Quaternion quaternion7 = this.heldInitialRot[(int)handType];
				quaternion2 = Quaternion.Slerp(quaternion2, quaternion7, 0.1f);
				Vector3 vector16 = this.heldInitialPos[(int)handType];
				vector6 = Vector3.Lerp(vector6, vector16, 0.1f);
				this.heldCurrentRot[(int)handType] = quaternion2;
				this.heldCurrentPos[(int)handType] = vector6;
				builderPiece2.transform.localRotation = quaternion2;
				builderPiece2.transform.localPosition = vector6;
				this.prevPotentialPlacement[(int)handType] = default(BuilderPotentialPlacement);
				return;
			}
			break;
		}
		case BuilderPieceInteractor.HandState.PotentialGrabbed:
		{
			if (flag3)
			{
				BuilderPiece builderPiece3 = this.heldPiece[(int)handType];
				this.ClearUnSnapOffset((int)handType, builderPiece3);
				this.RemovePieceFromHand(builderPiece3, (int)handType);
				this.heldPiece[(int)handType] = null;
				this.SetHandState((int)handType, BuilderPieceInteractor.HandState.Empty);
				return;
			}
			BuilderPiece builderPiece4 = this.heldPiece[(int)handType];
			Vector3 vector17;
			Quaternion quaternion8;
			this.CalcPieceLocalPosAndRot(builderPiece4.transform.position, builderPiece4.transform.rotation, handAttachPoint, out vector17, out quaternion8);
			if (BuilderPiece.IsDroppedState(builderPiece4.state))
			{
				this.currentTable.RequestGrabPiece(builderPiece4, handType == BuilderPieceInteractor.HandType.Left, this.heldInitialPos[(int)handType], this.heldInitialRot[(int)handType]);
				return;
			}
			Vector3 vector18 = this.heldInitialPos[(int)handType] - vector17;
			this.UpdatePullApartOffset((int)handType, builderPiece4, handAttachPoint.TransformVector(vector18));
			float num10 = this.currentTable.pushAndEaseParams.unSnapDelayDist * this.currentTable.pushAndEaseParams.unSnapDelayDist;
			if (vector18.sqrMagnitude > num10)
			{
				GorillaTagger.Instance.StartVibration(handType == BuilderPieceInteractor.HandType.Left, GorillaTagger.Instance.tapHapticStrength * 0.15f, this.currentTable.pushAndEaseParams.unSnapDelayTime * 2f);
				if (((builderPiece4 == null) ? BuilderPiece.State.None : builderPiece4.state) == BuilderPiece.State.GrabbedLocal)
				{
					GorillaTagger.Instance.StartVibration(handType > BuilderPieceInteractor.HandType.Left, GorillaTagger.Instance.tapHapticStrength * 0.15f, this.currentTable.pushAndEaseParams.unSnapDelayTime * 2f);
				}
				this.SetHandState((int)handType, BuilderPieceInteractor.HandState.WaitingForUnSnap);
				this.delayedPlacementTime[(int)handType] = 0f;
				return;
			}
			break;
		}
		case BuilderPieceInteractor.HandState.WaitForGrabbed:
			break;
		case BuilderPieceInteractor.HandState.WaitingForSnap:
		{
			BuilderPiece builderPiece5 = this.heldPiece[(int)handType];
			if (builderPiece5 != null)
			{
				builderPiece5.transform.localRotation = this.heldInitialRot[(int)handType];
				builderPiece5.transform.localPosition = this.heldInitialPos[(int)handType];
				Quaternion quaternion9 = this.heldCurrentRot[(int)handType];
				Vector3 vector19 = this.heldCurrentPos[(int)handType];
				if (this.delayedPlacementTime[(int)handType] >= 0f)
				{
					BuilderPotentialPlacement builderPotentialPlacement3 = this.delayedPotentialPlacement[(int)handType];
					if (this.delayedPlacementTime[(int)handType] > this.currentTable.pushAndEaseParams.snapDelayTime)
					{
						BuilderAttachGridPlane builderAttachGridPlane4 = builderPotentialPlacement3.attachPiece.gridPlanes[builderPotentialPlacement3.attachIndex];
						BuilderAttachGridPlane builderAttachGridPlane5 = ((builderPotentialPlacement3.parentPiece == null) ? null : builderPotentialPlacement3.parentPiece.gridPlanes[builderPotentialPlacement3.parentAttachIndex]);
						bool flag15 = builderAttachGridPlane4.IsConnected(builderPotentialPlacement3.attachBounds);
						if (!flag15)
						{
							flag15 = builderAttachGridPlane5.IsConnected(builderPotentialPlacement3.parentAttachBounds);
						}
						if (flag15)
						{
							Debug.LogError("Snap Overlapping Why are we doing this!!??");
						}
						if (!BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, builderPiece5, builderPotentialPlacement3.parentPiece))
						{
							this.SetHandState((int)handType, BuilderPieceInteractor.HandState.Grabbed);
						}
						this.currentTable.RequestPlacePiece(builderPiece5, builderPotentialPlacement3.attachPiece, builderPotentialPlacement3.bumpOffsetX, builderPotentialPlacement3.bumpOffsetZ, builderPotentialPlacement3.twist, builderPotentialPlacement3.parentPiece, builderPotentialPlacement3.attachIndex, builderPotentialPlacement3.parentAttachIndex);
						return;
					}
					this.delayedPlacementTime[(int)handType] = this.delayedPlacementTime[(int)handType] + Time.deltaTime;
					Transform parent = builderPiece5.transform.parent;
					Vector3 vector20 = builderPotentialPlacement3.localPosition;
					Quaternion quaternion10 = builderPotentialPlacement3.localRotation;
					Vector3 vector21 = builderPotentialPlacement3.attachPlaneNormal;
					if (builderPotentialPlacement3.parentPiece != null)
					{
						BuilderAttachGridPlane builderAttachGridPlane6 = builderPotentialPlacement3.parentPiece.gridPlanes[builderPotentialPlacement3.parentAttachIndex];
						vector20 = builderAttachGridPlane6.transform.TransformPoint(builderPotentialPlacement3.localPosition);
						quaternion10 = builderAttachGridPlane6.transform.rotation * builderPotentialPlacement3.localRotation;
						vector21 = builderAttachGridPlane6.transform.TransformDirection(builderPotentialPlacement3.attachPlaneNormal);
					}
					Vector3 vector22 = parent.transform.InverseTransformPoint(vector20);
					Quaternion quaternion11 = Quaternion.Inverse(parent.transform.rotation) * quaternion10;
					bool flag16 = builderPotentialPlacement3.attachPiece == builderPiece5;
					bool flag17 = builderPotentialPlacement3.attachPiece == builderPiece5;
					if (flag16)
					{
						quaternion9 = quaternion11;
					}
					if (flag17)
					{
						Vector3 vector23 = parent.transform.InverseTransformDirection(vector21);
						vector19 = vector22 + vector23 * this.currentTable.pushAndEaseParams.snapDelayOffsetDist;
					}
					this.heldCurrentRot[(int)handType] = quaternion9;
					this.heldCurrentPos[(int)handType] = vector19;
					builderPiece5.transform.localRotation = quaternion9;
					builderPiece5.transform.localPosition = vector19;
				}
			}
			break;
		}
		case BuilderPieceInteractor.HandState.WaitingForUnSnap:
		{
			BuilderPiece builderPiece6 = this.heldPiece[(int)handType];
			if (BuilderPiece.IsDroppedState(builderPiece6.state))
			{
				this.currentTable.RequestGrabPiece(builderPiece6, handType == BuilderPieceInteractor.HandType.Left, this.heldInitialPos[(int)handType], this.heldInitialRot[(int)handType]);
				return;
			}
			if (this.delayedPlacementTime[(int)handType] <= this.currentTable.pushAndEaseParams.unSnapDelayTime)
			{
				Vector3 vector24;
				Quaternion quaternion12;
				this.CalcPieceLocalPosAndRot(builderPiece6.transform.position, builderPiece6.transform.rotation, handAttachPoint, out vector24, out quaternion12);
				Vector3 vector25 = this.heldInitialPos[(int)handType] - vector24;
				this.UpdatePullApartOffset((int)handType, builderPiece6, handAttachPoint.TransformVector(vector25));
				this.delayedPlacementTime[(int)handType] = this.delayedPlacementTime[(int)handType] + Time.deltaTime;
				return;
			}
			if (builderPiece6.GetChildCount() > this.maxHoldablePieceStackCount)
			{
				builderPiece6.PlayTooHeavyFx();
				this.ClearUnSnapOffset((int)handType, builderPiece6);
				this.RemovePieceFromHand(builderPiece6, (int)handType);
				return;
			}
			this.currentTable.RequestGrabPiece(builderPiece6, handType == BuilderPieceInteractor.HandType.Left, this.heldInitialPos[(int)handType], this.heldInitialRot[(int)handType]);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06001EF5 RID: 7925 RVA: 0x0009961C File Offset: 0x0009781C
	private void ClearGlowBumps(int handIndex)
	{
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		BuilderPool builderPool = builderTable.builderPool;
		List<BuilderBumpGlow> list = this.glowBumps[handIndex];
		if (builderPool != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				builderPool.DestroyBumpGlow(list[i]);
			}
		}
		else
		{
			Debug.LogError("BuilderPieceInteractor could not find Builderpool");
		}
		list.Clear();
	}

	// Token: 0x06001EF6 RID: 7926 RVA: 0x00099694 File Offset: 0x00097894
	private void AddGlowBumps(int handIndex, List<BuilderPotentialPlacement> allPotentialPlacements)
	{
		this.ClearGlowBumps(handIndex);
		if (allPotentialPlacements == null)
		{
			Debug.LogError("How is allPotentialPlacements null");
			return;
		}
		BuilderPool builderPool = this.currentTable.builderPool;
		if (builderPool == null)
		{
			Debug.LogError("How is the pool null?");
			return;
		}
		float gridSize = this.currentTable.gridSize;
		for (int i = 0; i < allPotentialPlacements.Count; i++)
		{
			BuilderPotentialPlacement builderPotentialPlacement = allPotentialPlacements[i];
			if (builderPotentialPlacement.parentPiece != null && builderPotentialPlacement.attachPiece != null && builderPotentialPlacement.attachPiece.gridPlanes != null && builderPotentialPlacement.parentPiece.gridPlanes != null)
			{
				BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement.parentPiece.gridPlanes[builderPotentialPlacement.parentAttachIndex];
				if (builderAttachGridPlane != null)
				{
					Vector2Int min = builderPotentialPlacement.parentAttachBounds.min;
					Vector2Int max = builderPotentialPlacement.parentAttachBounds.max;
					for (int j = min.x; j <= max.x; j++)
					{
						for (int k = min.y; k <= max.y; k++)
						{
							Vector3 gridPosition = builderAttachGridPlane.GetGridPosition(j, k, gridSize);
							BuilderBumpGlow builderBumpGlow = builderPool.CreateGlowBump();
							if (builderBumpGlow != null)
							{
								builderBumpGlow.transform.SetPositionAndRotation(gridPosition, builderAttachGridPlane.transform.rotation);
								builderBumpGlow.transform.SetParent(builderAttachGridPlane.transform, true);
								builderBumpGlow.transform.localScale = Vector3.one;
								builderBumpGlow.gameObject.SetActive(true);
								this.glowBumps[handIndex].Add(builderBumpGlow);
							}
						}
					}
				}
				BuilderAttachGridPlane builderAttachGridPlane2 = builderPotentialPlacement.attachPiece.gridPlanes[builderPotentialPlacement.attachIndex];
				if (builderAttachGridPlane2 != null)
				{
					Vector2Int min2 = builderPotentialPlacement.attachBounds.min;
					Vector2Int max2 = builderPotentialPlacement.attachBounds.max;
					for (int l = min2.x; l <= max2.x; l++)
					{
						for (int m = min2.y; m <= max2.y; m++)
						{
							Vector3 gridPosition2 = builderAttachGridPlane2.GetGridPosition(l, m, gridSize);
							BuilderBumpGlow builderBumpGlow2 = builderPool.CreateGlowBump();
							if (builderBumpGlow2 != null)
							{
								Quaternion quaternion = Quaternion.Euler(180f, 0f, 0f);
								builderBumpGlow2.transform.SetPositionAndRotation(gridPosition2, builderAttachGridPlane2.transform.rotation * quaternion);
								builderBumpGlow2.transform.SetParent(builderAttachGridPlane2.transform, true);
								builderBumpGlow2.transform.localScale = Vector3.one;
								builderBumpGlow2.gameObject.SetActive(true);
								this.glowBumps[handIndex].Add(builderBumpGlow2);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001EF7 RID: 7927 RVA: 0x00099964 File Offset: 0x00097B64
	private void UpdateGlowBumps(int handIndex, float intensity)
	{
		List<BuilderBumpGlow> list = this.glowBumps[handIndex];
		for (int i = 0; i < list.Count; i++)
		{
			list[i].SetIntensity(intensity);
		}
	}

	// Token: 0x06001EF8 RID: 7928 RVA: 0x0009999C File Offset: 0x00097B9C
	private void UpdatePullApartOffset(int handIndex, BuilderPiece potentialGrabPiece, Vector3 pullApartDiff)
	{
		BuilderPiece parentPiece = potentialGrabPiece.parentPiece;
		BuilderAttachGridPlane builderAttachGridPlane = null;
		if (parentPiece != null)
		{
			builderAttachGridPlane = parentPiece.gridPlanes[potentialGrabPiece.parentAttachIndex];
		}
		Vector3 vector = Vector3.up;
		if (builderAttachGridPlane != null)
		{
			vector = builderAttachGridPlane.transform.TransformDirection(vector);
			if (!builderAttachGridPlane.male)
			{
				vector *= -1f;
			}
		}
		float num = Vector3.Dot(pullApartDiff, vector);
		num = Mathf.Max(num, 0f);
		float num2 = 0.0025f;
		float num3 = num / num2;
		num3 = 1f - 1f / (1f + num3);
		num = num3 * num2;
		Vector3 vector2 = vector * this.potentialGrabbedOffsetDist[handIndex];
		this.potentialGrabbedOffsetDist[handIndex] = num;
		Vector3 vector3 = vector * this.potentialGrabbedOffsetDist[handIndex];
		if (builderAttachGridPlane != null)
		{
			vector2 = builderAttachGridPlane.transform.InverseTransformVector(vector2);
			vector3 = builderAttachGridPlane.transform.InverseTransformVector(vector3);
		}
		Vector3 vector4 = potentialGrabPiece.transform.localPosition - vector2;
		potentialGrabPiece.transform.localPosition = vector4 + vector3;
	}

	// Token: 0x06001EF9 RID: 7929 RVA: 0x00099ABE File Offset: 0x00097CBE
	private void ClearUnSnapOffset(int handIndex, BuilderPiece potentialGrabPiece)
	{
		this.UpdatePullApartOffset(handIndex, potentialGrabPiece, Vector3.zero);
	}

	// Token: 0x06001EFA RID: 7930 RVA: 0x00099AD0 File Offset: 0x00097CD0
	public void AddPieceToHeld(BuilderPiece piece, bool isLeft, Vector3 localPosition, Quaternion localRotation)
	{
		int num = (isLeft ? 0 : 1);
		this.AddPieceToHand(piece, num, localPosition, localRotation);
		int num2 = (num + 1) % 2;
		if (this.heldPiece[num2] == piece)
		{
			this.RemovePieceFromHand(piece, num2);
		}
	}

	// Token: 0x06001EFB RID: 7931 RVA: 0x00099B14 File Offset: 0x00097D14
	public void RemovePieceFromHeld(BuilderPiece piece)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.heldPiece[i] == piece)
			{
				this.RemovePieceFromHand(piece, i);
			}
		}
	}

	// Token: 0x06001EFC RID: 7932 RVA: 0x00099B4C File Offset: 0x00097D4C
	private void AddPieceToHand(BuilderPiece piece, int handIndex, Vector3 localPosition, Quaternion localRotation)
	{
		this.heldPiece[handIndex] = piece;
		this.delayedPlacementTime[handIndex] = -1f;
		this.SetHandState(handIndex, BuilderPieceInteractor.HandState.Grabbed);
		this.heldInitialRot[handIndex] = localRotation;
		this.heldCurrentRot[handIndex] = localRotation;
		this.heldInitialPos[handIndex] = localPosition;
		this.heldCurrentPos[handIndex] = localPosition;
	}

	// Token: 0x06001EFD RID: 7933 RVA: 0x00099BB5 File Offset: 0x00097DB5
	private void RemovePieceFromHand(BuilderPiece piece, int handIndex)
	{
		this.heldPiece[handIndex] = null;
		this.delayedPlacementTime[handIndex] = -1f;
		this.SetHandState(handIndex, BuilderPieceInteractor.HandState.Empty);
		this.ClearGlowBumps(handIndex);
	}

	// Token: 0x06001EFE RID: 7934 RVA: 0x00099BE4 File Offset: 0x00097DE4
	public void RemovePiecesFromHands()
	{
		for (int i = 0; i < 2; i++)
		{
			this.heldPiece[i] = null;
			this.delayedPlacementTime[i] = -1f;
			this.SetHandState(i, BuilderPieceInteractor.HandState.Empty);
			this.ClearGlowBumps(i);
		}
	}

	// Token: 0x06001EFF RID: 7935 RVA: 0x00099C2C File Offset: 0x00097E2C
	private void CalcPieceLocalPosAndRot(Vector3 worldPosition, Quaternion worldRotation, Transform attachPoint, out Vector3 localPosition, out Quaternion localRotation)
	{
		Quaternion rotation = attachPoint.transform.rotation;
		Vector3 position = attachPoint.transform.position;
		localRotation = Quaternion.Inverse(rotation) * worldRotation;
		localPosition = Quaternion.Inverse(rotation) * (worldPosition - position);
	}

	// Token: 0x06001F00 RID: 7936 RVA: 0x00099C81 File Offset: 0x00097E81
	public void DisableCollisionsWithHands()
	{
		this.DisableCollisionsWithHand(true);
		this.DisableCollisionsWithHand(false);
	}

	// Token: 0x06001F01 RID: 7937 RVA: 0x00099C94 File Offset: 0x00097E94
	private void DisableCollisionsWithHand(bool leftHand)
	{
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		BuilderTable builderTable;
		if (!BuilderTable.TryGetBuilderTableForZone(offlineVRRig.zoneEntity.currentZone, out builderTable))
		{
			return;
		}
		Transform transform = (leftHand ? offlineVRRig.leftHand.overrideTarget : offlineVRRig.rightHand.overrideTarget);
		List<GameObject> list = (leftHand ? this.collisionDisabledPiecesLeft : this.collisionDisabledPiecesRight);
		int num = Physics.OverlapSphereNonAlloc(transform.position, 0.15f, BuilderPieceInteractor.tempDisableColliders, builderTable.allPiecesMask);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = BuilderPieceInteractor.tempDisableColliders[i].gameObject;
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(BuilderPieceInteractor.tempDisableColliders[i]);
			if (builderPieceFromCollider != null && builderPieceFromCollider.state == BuilderPiece.State.AttachedAndPlaced && !list.Contains(gameObject))
			{
				gameObject.layer = BuilderTable.heldLayer;
				list.Add(gameObject);
			}
		}
	}

	// Token: 0x06001F02 RID: 7938 RVA: 0x00099D6D File Offset: 0x00097F6D
	public void UpdatePieceDisables()
	{
		this.UpdatePieceDisablesForHand(true);
		this.UpdatePieceDisablesForHand(false);
	}

	// Token: 0x06001F03 RID: 7939 RVA: 0x00099D80 File Offset: 0x00097F80
	public void UpdatePieceDisablesForHand(bool leftHand)
	{
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		Transform transform = (leftHand ? offlineVRRig.leftHand.overrideTarget : offlineVRRig.rightHand.overrideTarget);
		List<GameObject> list = (leftHand ? this.collisionDisabledPiecesLeft : this.collisionDisabledPiecesRight);
		List<GameObject> list2 = ((!leftHand) ? this.collisionDisabledPiecesLeft : this.collisionDisabledPiecesRight);
		Vector3 position = transform.position;
		float num = 0.040000003f;
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = list[i];
			if (gameObject == null)
			{
				list.RemoveAt(i);
				i--;
			}
			else if ((gameObject.transform.position - position).sqrMagnitude > num)
			{
				BuilderPiece builderPieceFromTransform = BuilderPiece.GetBuilderPieceFromTransform(gameObject.transform);
				if (builderPieceFromTransform.state == BuilderPiece.State.AttachedAndPlaced && !list2.Contains(gameObject))
				{
					builderPieceFromTransform.SetColliderLayers<Collider>(builderPieceFromTransform.colliders, BuilderTable.placedLayer);
				}
				list.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x04002240 RID: 8768
	[OnEnterPlay_SetNull]
	public static volatile BuilderPieceInteractor instance;

	// Token: 0x04002241 RID: 8769
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x04002242 RID: 8770
	public EquipmentInteractor equipmentInteractor;

	// Token: 0x04002243 RID: 8771
	private const int NUM_HANDS = 2;

	// Token: 0x04002244 RID: 8772
	public GorillaVelocityEstimator velocityEstimatorLeft;

	// Token: 0x04002245 RID: 8773
	public GorillaVelocityEstimator velocityEstimatorRight;

	// Token: 0x04002246 RID: 8774
	public BuilderLaserSight laserSightLeft;

	// Token: 0x04002247 RID: 8775
	public BuilderLaserSight laserSightRight;

	// Token: 0x04002248 RID: 8776
	public int maxHoldablePieceStackCount = 50;

	// Token: 0x04002249 RID: 8777
	public List<GorillaVelocityEstimator> velocityEstimator;

	// Token: 0x0400224A RID: 8778
	public List<BuilderPieceInteractor.HandState> handState;

	// Token: 0x0400224B RID: 8779
	public List<BuilderPiece> heldPiece;

	// Token: 0x0400224C RID: 8780
	public List<BuilderPiece> potentialHeldPiece;

	// Token: 0x0400224D RID: 8781
	public List<float> potentialGrabbedOffsetDist;

	// Token: 0x0400224E RID: 8782
	public List<Quaternion> heldInitialRot;

	// Token: 0x0400224F RID: 8783
	public List<Quaternion> heldCurrentRot;

	// Token: 0x04002250 RID: 8784
	public List<Vector3> heldInitialPos;

	// Token: 0x04002251 RID: 8785
	public List<Vector3> heldCurrentPos;

	// Token: 0x04002252 RID: 8786
	public List<BuilderPotentialPlacement> delayedPotentialPlacement;

	// Token: 0x04002253 RID: 8787
	public List<float> delayedPlacementTime;

	// Token: 0x04002254 RID: 8788
	public List<BuilderPotentialPlacement> prevPotentialPlacement;

	// Token: 0x04002255 RID: 8789
	public List<BuilderLaserSight> laserSight;

	// Token: 0x04002256 RID: 8790
	public int[] heldChainLength;

	// Token: 0x04002257 RID: 8791
	public List<int[]> heldChainCost;

	// Token: 0x04002258 RID: 8792
	private static List<BuilderPotentialPlacement>[] allPotentialPlacements;

	// Token: 0x04002259 RID: 8793
	private static NativeList<BuilderGridPlaneData>[] handGridPlaneData;

	// Token: 0x0400225A RID: 8794
	private static NativeList<BuilderPieceData>[] handPieceData;

	// Token: 0x0400225B RID: 8795
	private static NativeList<BuilderGridPlaneData>[] localAttachableGridPlaneData;

	// Token: 0x0400225C RID: 8796
	private static NativeList<BuilderPieceData>[] localAttachablePieceData;

	// Token: 0x0400225D RID: 8797
	private JobHandle findNearbyJobHandle;

	// Token: 0x0400225E RID: 8798
	public List<GameObject> collisionDisabledPiecesLeft = new List<GameObject>();

	// Token: 0x0400225F RID: 8799
	public List<GameObject> collisionDisabledPiecesRight = new List<GameObject>();

	// Token: 0x04002260 RID: 8800
	public const int MAX_SPHERE_CHECK_RESULTS = 1024;

	// Token: 0x04002261 RID: 8801
	public NativeArray<OverlapSphereCommand> checkPiecesInSphere;

	// Token: 0x04002262 RID: 8802
	public NativeArray<ColliderHit> checkPiecesInSphereResults;

	// Token: 0x04002263 RID: 8803
	public JobHandle checkNearbyPiecesHandle;

	// Token: 0x04002264 RID: 8804
	public const float GRAB_CAST_RADIUS = 0.0375f;

	// Token: 0x04002265 RID: 8805
	public const int MAX_GRAB_CAST_RESULTS = 64;

	// Token: 0x04002266 RID: 8806
	public NativeArray<SpherecastCommand> grabSphereCast;

	// Token: 0x04002267 RID: 8807
	public NativeArray<RaycastHit> grabSphereCastResults;

	// Token: 0x04002268 RID: 8808
	public JobHandle findPiecesToGrab;

	// Token: 0x04002269 RID: 8809
	private RaycastHit emptyRaycastHit;

	// Token: 0x0400226A RID: 8810
	public BuilderBumpGlow glowBumpPrefab;

	// Token: 0x0400226B RID: 8811
	public List<List<BuilderBumpGlow>> glowBumps;

	// Token: 0x0400226C RID: 8812
	private const int MAX_GRID_PLANES = 8192;

	// Token: 0x0400226D RID: 8813
	private bool isRigSmall;

	// Token: 0x0400226E RID: 8814
	private BuilderTable currentTable;

	// Token: 0x0400226F RID: 8815
	private static HashSet<BuilderPiece> tempPieceSet = new HashSet<BuilderPiece>(512);

	// Token: 0x04002270 RID: 8816
	private static RaycastHit[] tempHitResults = new RaycastHit[64];

	// Token: 0x04002271 RID: 8817
	private const float PIECE_DISTANCE_DISABLE = 0.15f;

	// Token: 0x04002272 RID: 8818
	private const float PIECE_DISTANCE_ENABLE = 0.2f;

	// Token: 0x04002273 RID: 8819
	private static Collider[] tempDisableColliders = new Collider[128];

	// Token: 0x020004F9 RID: 1273
	public enum HandType
	{
		// Token: 0x04002275 RID: 8821
		Invalid = -1,
		// Token: 0x04002276 RID: 8822
		Left,
		// Token: 0x04002277 RID: 8823
		Right
	}

	// Token: 0x020004FA RID: 1274
	public enum HandState
	{
		// Token: 0x04002279 RID: 8825
		Invalid = -1,
		// Token: 0x0400227A RID: 8826
		Empty,
		// Token: 0x0400227B RID: 8827
		Grabbed,
		// Token: 0x0400227C RID: 8828
		PotentialGrabbed,
		// Token: 0x0400227D RID: 8829
		WaitForGrabbed,
		// Token: 0x0400227E RID: 8830
		WaitingForSnap,
		// Token: 0x0400227F RID: 8831
		WaitingForUnSnap
	}
}
