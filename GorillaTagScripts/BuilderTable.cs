using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BoingKit;
using CjLib;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTagScripts.Builder;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using Unity.Collections;
using Unity.Jobs;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTagScripts
{
	// Token: 0x02000AE4 RID: 2788
	public class BuilderTable : MonoBehaviour
	{
		// Token: 0x06004384 RID: 17284 RVA: 0x00138A50 File Offset: 0x00136C50
		private void ExecuteAction(BuilderAction action)
		{
			if (!this.isTableMutable)
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(action.pieceId);
			BuilderPiece piece2 = this.GetPiece(action.parentPieceId);
			int playerActorNumber = action.playerActorNumber;
			bool flag = PhotonNetwork.LocalPlayer.ActorNumber == action.playerActorNumber;
			switch (action.type)
			{
			case BuilderActionType.AttachToPlayer:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				RigContainer rigContainer;
				if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(playerActorNumber), out rigContainer))
				{
					string.Format("Execute Builder Action {0} {1} {2} {3} {4}", new object[] { action.localCommandId, action.type, action.pieceId, action.playerActorNumber, action.isLeftHand });
					return;
				}
				BodyDockPositions myBodyDockPositions = rigContainer.Rig.myBodyDockPositions;
				Transform transform = (action.isLeftHand ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform);
				piece.SetParentHeld(transform, playerActorNumber, action.isLeftHand);
				piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				BuilderPiece.State state = (flag ? BuilderPiece.State.GrabbedLocal : BuilderPiece.State.Grabbed);
				piece.SetState(state, false);
				if (!flag)
				{
					BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
				}
				if (flag)
				{
					BuilderPieceInteractor.instance.AddPieceToHeld(piece, action.isLeftHand, action.localPosition, action.localRotation);
					return;
				}
				break;
			}
			case BuilderActionType.DetachFromPlayer:
				if (flag)
				{
					BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
				}
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				return;
			case BuilderActionType.AttachToPiece:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				Quaternion identity = Quaternion.identity;
				Vector3 zero = Vector3.zero;
				Vector3 position = piece.transform.position;
				Quaternion rotation = piece.transform.rotation;
				if (piece2 != null)
				{
					piece.BumpTwistToPositionRotation(action.twist, action.bumpOffsetx, action.bumpOffsetz, action.attachIndex, piece2.gridPlanes[action.parentAttachIndex], out zero, out identity, out position, out rotation);
				}
				piece.transform.SetPositionAndRotation(position, rotation);
				BuilderPiece.State state2;
				if (piece2 == null)
				{
					state2 = BuilderPiece.State.AttachedAndPlaced;
				}
				else if (piece2.isArmShelf || piece2.state == BuilderPiece.State.AttachedToArm)
				{
					state2 = BuilderPiece.State.AttachedToArm;
				}
				else if (piece2.isBuiltIntoTable || piece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					state2 = BuilderPiece.State.AttachedAndPlaced;
				}
				else if (piece2.state == BuilderPiece.State.Grabbed)
				{
					state2 = BuilderPiece.State.Grabbed;
				}
				else if (piece2.state == BuilderPiece.State.GrabbedLocal)
				{
					state2 = BuilderPiece.State.GrabbedLocal;
				}
				else
				{
					state2 = BuilderPiece.State.AttachedToDropped;
				}
				BuilderPiece rootPiece = piece2.GetRootPiece();
				this.gridPlaneData.Clear();
				this.checkGridPlaneData.Clear();
				this.allPotentialPlacements.Clear();
				BuilderTable.tempPieceSet.Clear();
				QueryParameters queryParameters = new QueryParameters
				{
					layerMask = this.allPiecesMask
				};
				OverlapSphereCommand overlapSphereCommand = new OverlapSphereCommand(position, 1f, queryParameters);
				this.nearbyPiecesCommands[0] = overlapSphereCommand;
				OverlapSphereCommand.ScheduleBatch(this.nearbyPiecesCommands, this.nearbyPiecesResults, 1, 1024, default(JobHandle)).Complete();
				int num = 0;
				while (num < 1024 && this.nearbyPiecesResults[num].instanceID != 0)
				{
					BuilderPiece builderPiece = piece;
					BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.nearbyPiecesResults[num].collider);
					if (builderPieceFromCollider != null && !BuilderTable.tempPieceSet.Contains(builderPieceFromCollider))
					{
						BuilderTable.tempPieceSet.Add(builderPieceFromCollider);
						if (this.CanPiecesPotentiallyOverlap(builderPiece, rootPiece, state2, builderPieceFromCollider))
						{
							for (int i = 0; i < builderPieceFromCollider.gridPlanes.Count; i++)
							{
								BuilderGridPlaneData builderGridPlaneData = new BuilderGridPlaneData(builderPieceFromCollider.gridPlanes[i], -1);
								this.checkGridPlaneData.Add(in builderGridPlaneData);
							}
						}
					}
					num++;
				}
				BuilderTableJobs.BuildTestPieceListForJob(piece, this.gridPlaneData);
				BuilderPotentialPlacement builderPotentialPlacement = new BuilderPotentialPlacement
				{
					localPosition = zero,
					localRotation = identity,
					attachIndex = action.attachIndex,
					parentAttachIndex = action.parentAttachIndex,
					attachPiece = piece,
					parentPiece = piece2
				};
				this.CalcAllPotentialPlacements(this.gridPlaneData, this.checkGridPlaneData, builderPotentialPlacement, this.allPotentialPlacements);
				piece.SetParentPiece(action.attachIndex, piece2, action.parentAttachIndex);
				for (int j = 0; j < this.allPotentialPlacements.Count; j++)
				{
					BuilderPotentialPlacement builderPotentialPlacement2 = this.allPotentialPlacements[j];
					BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement2.attachPiece.gridPlanes[builderPotentialPlacement2.attachIndex];
					BuilderAttachGridPlane builderAttachGridPlane2 = builderPotentialPlacement2.parentPiece.gridPlanes[builderPotentialPlacement2.parentAttachIndex];
					BuilderAttachGridPlane movingParentGrid = builderAttachGridPlane.GetMovingParentGrid();
					bool flag2 = movingParentGrid != null;
					BuilderAttachGridPlane movingParentGrid2 = builderAttachGridPlane2.GetMovingParentGrid();
					bool flag3 = movingParentGrid2 != null;
					if (flag2 == flag3 && (!flag2 || !(movingParentGrid != movingParentGrid2)))
					{
						SnapOverlap snapOverlap = this.builderPool.CreateSnapOverlap(builderAttachGridPlane2, builderPotentialPlacement2.attachBounds);
						builderAttachGridPlane.AddSnapOverlap(snapOverlap);
						SnapOverlap snapOverlap2 = this.builderPool.CreateSnapOverlap(builderAttachGridPlane, builderPotentialPlacement2.parentAttachBounds);
						builderAttachGridPlane2.AddSnapOverlap(snapOverlap2);
					}
				}
				piece.transform.SetLocalPositionAndRotation(zero, identity);
				if (piece2 != null && piece2.state == BuilderPiece.State.GrabbedLocal)
				{
					BuilderPiece rootPiece2 = piece2.GetRootPiece();
					BuilderPieceInteractor.instance.OnCountChangedForRoot(rootPiece2);
				}
				if (piece2 == null)
				{
					piece.SetActivateTimeStamp(action.timeStamp);
					piece.SetState(BuilderPiece.State.AttachedAndPlaced, false);
					this.SetIsDirty(true);
					if (flag)
					{
						BuilderPieceInteractor.instance.DisableCollisionsWithHands();
						return;
					}
				}
				else
				{
					if (piece2.isArmShelf || piece2.state == BuilderPiece.State.AttachedToArm)
					{
						piece.SetState(BuilderPiece.State.AttachedToArm, false);
						return;
					}
					if (piece2.isBuiltIntoTable || piece2.state == BuilderPiece.State.AttachedAndPlaced)
					{
						piece.SetActivateTimeStamp(action.timeStamp);
						piece.SetState(BuilderPiece.State.AttachedAndPlaced, false);
						if (piece2 != null)
						{
							BuilderPiece attachedBuiltInPiece = piece2.GetAttachedBuiltInPiece();
							BuilderPiecePrivatePlot builderPiecePrivatePlot;
							if (attachedBuiltInPiece != null && attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot))
							{
								builderPiecePrivatePlot.OnPieceAttachedToPlot(piece);
							}
						}
						this.SetIsDirty(true);
						if (flag)
						{
							BuilderPieceInteractor.instance.DisableCollisionsWithHands();
							return;
						}
					}
					else
					{
						if (piece2.state == BuilderPiece.State.Grabbed)
						{
							piece.SetState(BuilderPiece.State.Grabbed, false);
							return;
						}
						if (piece2.state == BuilderPiece.State.GrabbedLocal)
						{
							piece.SetState(BuilderPiece.State.GrabbedLocal, false);
							return;
						}
						piece.SetState(BuilderPiece.State.AttachedToDropped, false);
						return;
					}
				}
				break;
			}
			case BuilderActionType.DetachFromPiece:
			{
				BuilderPiece builderPiece2 = piece;
				bool flag4 = piece.state == BuilderPiece.State.GrabbedLocal;
				if (flag4)
				{
					builderPiece2 = piece.GetRootPiece();
				}
				if (piece.state == BuilderPiece.State.AttachedAndPlaced)
				{
					this.SetIsDirty(true);
					BuilderPiece attachedBuiltInPiece2 = piece.GetAttachedBuiltInPiece();
					BuilderPiecePrivatePlot builderPiecePrivatePlot2;
					if (attachedBuiltInPiece2 != null && attachedBuiltInPiece2.TryGetPlotComponent(out builderPiecePrivatePlot2))
					{
						builderPiecePrivatePlot2.OnPieceDetachedFromPlot(piece);
					}
				}
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				if (flag4)
				{
					BuilderPieceInteractor.instance.OnCountChangedForRoot(builderPiece2);
					return;
				}
				break;
			}
			case BuilderActionType.MakePieceRoot:
				BuilderPiece.MakePieceRoot(piece);
				return;
			case BuilderActionType.DropPiece:
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				piece.transform.localScale = Vector3.one;
				piece.SetState(BuilderPiece.State.Dropped, false);
				piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				if (piece.rigidBody != null)
				{
					piece.rigidBody.position = action.localPosition;
					piece.rigidBody.rotation = action.localRotation;
					piece.rigidBody.velocity = action.velocity;
					piece.rigidBody.angularVelocity = action.angVelocity;
					return;
				}
				break;
			case BuilderActionType.AttachToShelf:
			{
				piece.ClearParentHeld();
				piece.ClearParentPiece(false);
				int attachIndex = action.attachIndex;
				bool isLeftHand = action.isLeftHand;
				int parentAttachIndex = action.parentAttachIndex;
				float x = action.velocity.x;
				piece.transform.localScale = Vector3.one;
				piece.SetState(isLeftHand ? BuilderPiece.State.OnConveyor : BuilderPiece.State.OnShelf, false);
				if (isLeftHand)
				{
					if (attachIndex >= 0 && attachIndex < this.conveyors.Count)
					{
						BuilderConveyor builderConveyor = this.conveyors[attachIndex];
						float num2 = x / builderConveyor.GetFrameMovement();
						if (PhotonNetwork.ServerTimestamp >= parentAttachIndex)
						{
							uint num3 = (uint)(PhotonNetwork.ServerTimestamp - parentAttachIndex);
							num2 += num3 / 1000f;
						}
						piece.shelfOwner = attachIndex;
						builderConveyor.OnShelfPieceCreated(piece, num2);
						return;
					}
				}
				else
				{
					if (attachIndex >= 0 && attachIndex < this.dispenserShelves.Count)
					{
						BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[attachIndex];
						piece.shelfOwner = attachIndex;
						builderDispenserShelf.OnShelfPieceCreated(piece, false);
						return;
					}
					piece.transform.SetLocalPositionAndRotation(action.localPosition, action.localRotation);
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06004385 RID: 17285 RVA: 0x00139304 File Offset: 0x00137504
		public static bool AreStatesCompatibleForOverlap(BuilderPiece.State stateA, BuilderPiece.State stateB, BuilderPiece rootA, BuilderPiece rootB)
		{
			switch (stateA)
			{
			case BuilderPiece.State.None:
				return false;
			case BuilderPiece.State.AttachedAndPlaced:
				return stateB == BuilderPiece.State.AttachedAndPlaced;
			case BuilderPiece.State.AttachedToDropped:
			case BuilderPiece.State.Dropped:
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.OnConveyor:
				return (stateB == BuilderPiece.State.AttachedToDropped || stateB == BuilderPiece.State.Dropped || stateB == BuilderPiece.State.OnShelf || stateB == BuilderPiece.State.OnConveyor) && rootA.Equals(rootB);
			case BuilderPiece.State.Grabbed:
				return stateB == BuilderPiece.State.Grabbed && rootA.Equals(rootB);
			case BuilderPiece.State.Displayed:
				return false;
			case BuilderPiece.State.GrabbedLocal:
				return stateB == BuilderPiece.State.GrabbedLocal && rootA.heldInLeftHand == rootB.heldInLeftHand;
			case BuilderPiece.State.AttachedToArm:
			{
				if (stateB != BuilderPiece.State.AttachedToArm)
				{
					return false;
				}
				object obj = ((rootA.parentPiece != null) ? rootA.parentPiece : rootA);
				BuilderPiece builderPiece = ((rootB.parentPiece != null) ? rootB.parentPiece : rootB);
				return obj.Equals(builderPiece);
			}
			default:
				return false;
			}
		}

		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x06004386 RID: 17286 RVA: 0x001393C9 File Offset: 0x001375C9
		// (set) Token: 0x06004387 RID: 17287 RVA: 0x001393D1 File Offset: 0x001375D1
		public int CurrentSaveSlot
		{
			get
			{
				return this.currentSaveSlot;
			}
			set
			{
				if (this.saveInProgress)
				{
					return;
				}
				if (value < 0 || value > BuilderScanKiosk.NUM_SAVE_SLOTS)
				{
					this.currentSaveSlot = -1;
				}
				if (this.currentSaveSlot != value)
				{
					this.SetIsDirty(true);
				}
				this.currentSaveSlot = value;
			}
		}

		// Token: 0x06004388 RID: 17288 RVA: 0x00139408 File Offset: 0x00137608
		private void Awake()
		{
			if (BuilderTable.zoneToInstance == null)
			{
				BuilderTable.zoneToInstance = new Dictionary<GTZone, BuilderTable>(2);
			}
			if (!BuilderTable.zoneToInstance.TryAdd(this.tableZone, this))
			{
				Object.Destroy(this);
			}
			this.acceptableSqrDistFromCenter = Mathf.Pow(217f * this.pieceScale, 2f);
			if (this.buttonSnapRotation != null)
			{
				this.buttonSnapRotation.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonFreeRotation));
				this.buttonSnapRotation.SetPressed(this.useSnapRotation);
			}
			if (this.buttonSnapPosition != null)
			{
				this.buttonSnapPosition.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonFreePosition));
				this.buttonSnapPosition.SetPressed(this.usePlacementStyle > BuilderPlacementStyle.Float);
			}
			if (this.buttonSaveLayout != null)
			{
				this.buttonSaveLayout.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonSaveLayout));
			}
			if (this.buttonClearLayout != null)
			{
				this.buttonClearLayout.Setup(new Action<BuilderOptionButton, bool>(this.OnButtonClearLayout));
			}
			this.isSetup = false;
			this.nextPieceId = 10000;
			BuilderTable.placedLayer = LayerMask.NameToLayer("Gorilla Object");
			BuilderTable.heldLayerLocal = LayerMask.NameToLayer("Prop");
			BuilderTable.heldLayer = LayerMask.NameToLayer("BuilderProp");
			BuilderTable.droppedLayer = LayerMask.NameToLayer("BuilderProp");
			this.currSnapParams = this.pushAndEaseParams;
			this.tableState = BuilderTable.TableState.WaitingForZoneAndRoom;
			this.inRoom = false;
			this.inBuilderZone = false;
			this.builderNetworking.SetTable(this);
			this.plotOwners = new Dictionary<int, int>(10);
			this.doesLocalPlayerOwnPlot = false;
			this.queuedBuildCommands = new List<BuilderTable.BuilderCommand>(1028);
			if (this.isTableMutable)
			{
				this.playerToArmShelfLeft = new Dictionary<int, int>(10);
				this.playerToArmShelfRight = new Dictionary<int, int>(10);
				this.rollBackBufferedCommands = new List<BuilderTable.BuilderCommand>(1028);
				this.rollBackActions = new List<BuilderAction>(1028);
				this.rollForwardCommands = new List<BuilderTable.BuilderCommand>(1028);
				this.droppedPieces = new List<BuilderPiece>(BuilderTable.DROPPED_PIECE_LIMIT + 50);
				this.droppedPieceData = new List<BuilderTable.DroppedPieceData>(BuilderTable.DROPPED_PIECE_LIMIT + 50);
				this.SetupMonkeBlocksRoom();
				this.gridPlaneData = new NativeList<BuilderGridPlaneData>(1024, Allocator.Persistent);
				this.checkGridPlaneData = new NativeList<BuilderGridPlaneData>(1024, Allocator.Persistent);
				this.nearbyPiecesResults = new NativeArray<ColliderHit>(1024, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.nearbyPiecesCommands = new NativeArray<OverlapSphereCommand>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.allPotentialPlacements = new List<BuilderPotentialPlacement>(1024);
			}
			else
			{
				this.rollBackBufferedCommands = new List<BuilderTable.BuilderCommand>(128);
				this.rollBackActions = new List<BuilderAction>(128);
				this.rollForwardCommands = new List<BuilderTable.BuilderCommand>(128);
			}
			this.SetupResources();
			if (!this.isTableMutable && this.linkedTerminal != null)
			{
				this.linkedTerminal.Init(this);
			}
		}

		// Token: 0x06004389 RID: 17289 RVA: 0x001396EB File Offset: 0x001378EB
		public static bool TryGetBuilderTableForZone(GTZone zone, out BuilderTable table)
		{
			if (BuilderTable.zoneToInstance == null)
			{
				table = null;
				return false;
			}
			return BuilderTable.zoneToInstance.TryGetValue(zone, out table);
		}

		// Token: 0x0600438A RID: 17290 RVA: 0x00139708 File Offset: 0x00137908
		private void SetupMonkeBlocksRoom()
		{
			if (this.shelves == null)
			{
				this.shelves = new List<BuilderShelf>(64);
			}
			if (this.shelvesRoot != null)
			{
				this.shelvesRoot.GetComponentsInChildren<BuilderShelf>(this.shelves);
			}
			this.conveyors = new List<BuilderConveyor>(32);
			this.dispenserShelves = new List<BuilderDispenserShelf>(32);
			if (this.allShelvesRoot != null)
			{
				for (int i = 0; i < this.allShelvesRoot.Count; i++)
				{
					this.allShelvesRoot[i].GetComponentsInChildren<BuilderConveyor>(BuilderTable.tempConveyors);
					this.conveyors.AddRange(BuilderTable.tempConveyors);
					BuilderTable.tempConveyors.Clear();
					this.allShelvesRoot[i].GetComponentsInChildren<BuilderDispenserShelf>(BuilderTable.tempDispensers);
					this.dispenserShelves.AddRange(BuilderTable.tempDispensers);
					BuilderTable.tempDispensers.Clear();
				}
			}
			this.recyclers = new List<BuilderRecycler>(5);
			if (this.recyclerRoot != null)
			{
				for (int j = 0; j < this.recyclerRoot.Count; j++)
				{
					this.recyclerRoot[j].GetComponentsInChildren<BuilderRecycler>(BuilderTable.tempRecyclers);
					this.recyclers.AddRange(BuilderTable.tempRecyclers);
					BuilderTable.tempRecyclers.Clear();
				}
			}
			for (int k = 0; k < this.recyclers.Count; k++)
			{
				this.recyclers[k].recyclerID = k;
				this.recyclers[k].table = this;
			}
			this.dropZones = new List<BuilderDropZone>(6);
			this.dropZoneRoot.GetComponentsInChildren<BuilderDropZone>(this.dropZones);
			for (int l = 0; l < this.dropZones.Count; l++)
			{
				this.dropZones[l].dropZoneID = l;
				this.dropZones[l].table = this;
			}
			foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
			{
				builderResourceMeter.table = this;
			}
		}

		// Token: 0x0600438B RID: 17291 RVA: 0x00139914 File Offset: 0x00137B14
		private void SetupResources()
		{
			this.maxResources = new int[3];
			if (this.totalResources != null && this.totalResources.quantities != null)
			{
				for (int i = 0; i < this.totalResources.quantities.Count; i++)
				{
					if (this.totalResources.quantities[i].type >= BuilderResourceType.Basic && this.totalResources.quantities[i].type < BuilderResourceType.Count)
					{
						this.maxResources[(int)this.totalResources.quantities[i].type] += this.totalResources.quantities[i].count;
					}
				}
			}
			this.usedResources = new int[3];
			this.reservedResources = new int[3];
			if (this.totalReservedResources != null && this.totalReservedResources.quantities != null)
			{
				for (int j = 0; j < this.totalReservedResources.quantities.Count; j++)
				{
					if (this.totalReservedResources.quantities[j].type >= BuilderResourceType.Basic && this.totalReservedResources.quantities[j].type < BuilderResourceType.Count)
					{
						this.reservedResources[(int)this.totalReservedResources.quantities[j].type] += this.totalReservedResources.quantities[j].count;
					}
				}
			}
			this.plotMaxResources = new int[3];
			if (this.resourcesPerPrivatePlot != null && this.resourcesPerPrivatePlot.quantities != null)
			{
				for (int k = 0; k < this.resourcesPerPrivatePlot.quantities.Count; k++)
				{
					if (this.resourcesPerPrivatePlot.quantities[k].type >= BuilderResourceType.Basic && this.resourcesPerPrivatePlot.quantities[k].type < BuilderResourceType.Count)
					{
						this.plotMaxResources[(int)this.resourcesPerPrivatePlot.quantities[k].type] += this.resourcesPerPrivatePlot.quantities[k].count;
					}
				}
			}
			this.OnAvailableResourcesChange();
		}

		// Token: 0x0600438C RID: 17292 RVA: 0x00139B5C File Offset: 0x00137D5C
		private void Start()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.InRoom != this.inRoom)
			{
				this.SetInRoom(NetworkSystem.Instance.InRoom);
			}
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
			this.HandleOnZoneChanged();
			this.RequestTableConfiguration();
		}

		// Token: 0x0600438D RID: 17293 RVA: 0x00139BCA File Offset: 0x00137DCA
		private void OnApplicationQuit()
		{
			this.ClearTable();
			this.tableState = BuilderTable.TableState.WaitingForZoneAndRoom;
		}

		// Token: 0x0600438E RID: 17294 RVA: 0x00139BDC File Offset: 0x00137DDC
		private void OnDestroy()
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
			if (this.isTableMutable)
			{
				if (this.gridPlaneData.IsCreated)
				{
					this.gridPlaneData.Dispose();
				}
				if (this.checkGridPlaneData.IsCreated)
				{
					this.checkGridPlaneData.Dispose();
				}
				if (this.nearbyPiecesResults.IsCreated)
				{
					this.nearbyPiecesResults.Dispose();
				}
				if (this.nearbyPiecesCommands.IsCreated)
				{
					this.nearbyPiecesCommands.Dispose();
				}
			}
			this.DestroyData();
		}

		// Token: 0x0600438F RID: 17295 RVA: 0x00139C80 File Offset: 0x00137E80
		private void HandleOnZoneChanged()
		{
			bool flag = ZoneManagement.instance.IsZoneActive(this.tableZone);
			this.SetInBuilderZone(flag);
		}

		// Token: 0x06004390 RID: 17296 RVA: 0x00139CA8 File Offset: 0x00137EA8
		public void InitIfNeeded()
		{
			if (!this.isSetup)
			{
				for (int i = 0; i < this.factories.Count; i++)
				{
					this.factories[i].Setup(this);
				}
				this.builderRenderer.BuildRenderer(this.factories[0].pieceList);
				this.baseGridPlanes.Clear();
				this.basePieces = new List<BuilderPiece>(1024);
				for (int j = 0; j < this.builtInPieceRoots.Count; j++)
				{
					this.builtInPieceRoots[j].SetActive(true);
					this.builtInPieceRoots[j].GetComponentsInChildren<BuilderPiece>(false, BuilderTable.tempPieces);
					this.basePieces.AddRange(BuilderTable.tempPieces);
				}
				this.allPrivatePlots = new List<BuilderPiecePrivatePlot>(20);
				this.CreateData();
				for (int k = 0; k < this.basePieces.Count; k++)
				{
					BuilderPiece builderPiece = this.basePieces[k];
					builderPiece.SetTable(this);
					builderPiece.pieceId = 5 + k;
					builderPiece.SetScale(this.pieceScale);
					builderPiece.SetupPiece(this.gridSize);
					builderPiece.OnCreate();
					builderPiece.SetState(BuilderPiece.State.OnShelf, true);
					this.baseGridPlanes.AddRange(builderPiece.gridPlanes);
					BuilderPiecePrivatePlot builderPiecePrivatePlot;
					if (builderPiece.IsPrivatePlot() && builderPiece.TryGetPlotComponent(out builderPiecePrivatePlot))
					{
						this.allPrivatePlots.Add(builderPiecePrivatePlot);
					}
					this.AddPieceData(builderPiece);
				}
				this.builderPool.Setup(this.factories[0]);
				this.builderPool.BuildFromPieceSets();
				for (int l = 0; l < this.factories.Count; l++)
				{
					this.factories[l].Show();
				}
				if (this.isTableMutable)
				{
					for (int m = 0; m < this.conveyors.Count; m++)
					{
						this.conveyors[m].table = this;
						this.conveyors[m].shelfID = m;
						this.conveyors[m].Setup();
					}
					for (int n = 0; n < this.dispenserShelves.Count; n++)
					{
						this.dispenserShelves[n].table = this;
						this.dispenserShelves[n].shelfID = n;
						this.dispenserShelves[n].Setup();
					}
					this.conveyorManager.Setup(this);
					this.repelledPieceRoots = new HashSet<int>[this.repelHistoryLength];
					for (int num = 0; num < this.repelHistoryLength; num++)
					{
						this.repelledPieceRoots[num] = new HashSet<int>(10);
					}
					this.sharedBuildAreas = this.sharedBuildArea.GetComponents<BoxCollider>();
					BoxCollider[] array = this.sharedBuildAreas;
					for (int num2 = 0; num2 < array.Length; num2++)
					{
						array[num2].enabled = false;
					}
					this.sharedBuildArea.SetActive(false);
				}
				BoxCollider[] components = this.noBlocksArea.GetComponents<BoxCollider>();
				this.noBlocksAreas = new List<BuilderTable.BoxCheckParams>(components.Length);
				foreach (BoxCollider boxCollider in components)
				{
					boxCollider.enabled = true;
					BuilderTable.BoxCheckParams boxCheckParams = new BuilderTable.BoxCheckParams
					{
						center = boxCollider.transform.TransformPoint(boxCollider.center),
						halfExtents = Vector3.Scale(boxCollider.transform.lossyScale, boxCollider.size) / 2f,
						rotation = boxCollider.transform.rotation
					};
					this.noBlocksAreas.Add(boxCheckParams);
					boxCollider.enabled = false;
				}
				this.noBlocksArea.SetActive(false);
				this.isSetup = true;
			}
		}

		// Token: 0x06004391 RID: 17297 RVA: 0x0013A076 File Offset: 0x00138276
		private void SetIsDirty(bool dirty)
		{
			if (this.isDirty != dirty)
			{
				UnityEvent<bool> onSaveDirtyChanged = this.OnSaveDirtyChanged;
				if (onSaveDirtyChanged != null)
				{
					onSaveDirtyChanged.Invoke(dirty);
				}
			}
			this.isDirty = dirty;
		}

		// Token: 0x06004392 RID: 17298 RVA: 0x0013A09C File Offset: 0x0013829C
		private void FixedUpdate()
		{
			if (this.tableState != BuilderTable.TableState.Ready && this.tableState != BuilderTable.TableState.WaitForMasterResync)
			{
				return;
			}
			foreach (IBuilderPieceFunctional builderPieceFunctional in this.funcComponentsToRegisterFixed)
			{
				if (builderPieceFunctional != null)
				{
					this.fixedUpdateFunctionalComponents.Add(builderPieceFunctional);
				}
			}
			foreach (IBuilderPieceFunctional builderPieceFunctional2 in this.funcComponentsToUnregisterFixed)
			{
				this.fixedUpdateFunctionalComponents.Remove(builderPieceFunctional2);
			}
			this.funcComponentsToRegisterFixed.Clear();
			this.funcComponentsToUnregisterFixed.Clear();
			foreach (IBuilderPieceFunctional builderPieceFunctional3 in this.fixedUpdateFunctionalComponents)
			{
				builderPieceFunctional3.FunctionalPieceFixedUpdate();
			}
		}

		// Token: 0x06004393 RID: 17299 RVA: 0x0013A1A8 File Offset: 0x001383A8
		private void Update()
		{
			this.InitIfNeeded();
			this.UpdateTableState();
			if (this.isTableMutable)
			{
				this.UpdateDroppedPieces(Time.deltaTime);
				this.repelHistoryIndex = (this.repelHistoryIndex + 1) % this.repelHistoryLength;
				int num = (this.repelHistoryIndex + 1) % this.repelHistoryLength;
				this.repelledPieceRoots[num].Clear();
			}
		}

		// Token: 0x06004394 RID: 17300 RVA: 0x0013A206 File Offset: 0x00138406
		public void AddQueuedCommand(BuilderTable.BuilderCommand cmd)
		{
			this.queuedBuildCommands.Add(cmd);
		}

		// Token: 0x06004395 RID: 17301 RVA: 0x0013A214 File Offset: 0x00138414
		public void ClearQueuedCommands()
		{
			if (this.queuedBuildCommands != null)
			{
				this.queuedBuildCommands.Clear();
			}
			this.RemoveRollBackActions();
			if (this.rollBackBufferedCommands != null)
			{
				this.rollBackBufferedCommands.Clear();
			}
			this.RemoveRollForwardCommands();
		}

		// Token: 0x06004396 RID: 17302 RVA: 0x0013A248 File Offset: 0x00138448
		public int GetNumQueuedCommands()
		{
			if (this.queuedBuildCommands != null)
			{
				return this.queuedBuildCommands.Count;
			}
			return 0;
		}

		// Token: 0x06004397 RID: 17303 RVA: 0x0013A25F File Offset: 0x0013845F
		public void AddRollbackAction(BuilderAction action)
		{
			this.rollBackActions.Add(action);
		}

		// Token: 0x06004398 RID: 17304 RVA: 0x0013A26D File Offset: 0x0013846D
		public void RemoveRollBackActions()
		{
			this.rollBackActions.Clear();
		}

		// Token: 0x06004399 RID: 17305 RVA: 0x0013A27C File Offset: 0x0013847C
		public void RemoveRollBackActions(int localCommandId)
		{
			for (int i = this.rollBackActions.Count - 1; i >= 0; i--)
			{
				if (localCommandId == -1 || this.rollBackActions[i].localCommandId == localCommandId)
				{
					this.rollBackActions.RemoveAt(i);
				}
			}
		}

		// Token: 0x0600439A RID: 17306 RVA: 0x0013A2C8 File Offset: 0x001384C8
		public bool HasRollBackActionsForCommand(int localCommandId)
		{
			for (int i = 0; i < this.rollBackActions.Count; i++)
			{
				if (this.rollBackActions[i].localCommandId == localCommandId)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600439B RID: 17307 RVA: 0x0013A302 File Offset: 0x00138502
		public void AddRollForwardCommand(BuilderTable.BuilderCommand command)
		{
			this.rollForwardCommands.Add(command);
		}

		// Token: 0x0600439C RID: 17308 RVA: 0x0013A310 File Offset: 0x00138510
		public void RemoveRollForwardCommands()
		{
			this.rollForwardCommands.Clear();
		}

		// Token: 0x0600439D RID: 17309 RVA: 0x0013A320 File Offset: 0x00138520
		public void RemoveRollForwardCommands(int localCommandId)
		{
			for (int i = this.rollForwardCommands.Count - 1; i >= 0; i--)
			{
				if (localCommandId == -1 || this.rollForwardCommands[i].localCommandId == localCommandId)
				{
					this.rollForwardCommands.RemoveAt(i);
				}
			}
		}

		// Token: 0x0600439E RID: 17310 RVA: 0x0013A36C File Offset: 0x0013856C
		public bool HasRollForwardCommand(int localCommandId)
		{
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				if (this.rollForwardCommands[i].localCommandId == localCommandId)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600439F RID: 17311 RVA: 0x0013A3A8 File Offset: 0x001385A8
		public bool ShouldRollbackBufferCommand(BuilderTable.BuilderCommand cmd)
		{
			return cmd.type != BuilderTable.BuilderCommandType.Create && cmd.type != BuilderTable.BuilderCommandType.CreateArmShelf && this.rollBackActions.Count > 0 && (cmd.player == null || !cmd.player.IsLocal || !this.HasRollForwardCommand(cmd.localCommandId));
		}

		// Token: 0x060043A0 RID: 17312 RVA: 0x0013A3FF File Offset: 0x001385FF
		public void AddRollbackBufferedCommand(BuilderTable.BuilderCommand bufferedCmd)
		{
			this.rollBackBufferedCommands.Add(bufferedCmd);
		}

		// Token: 0x060043A1 RID: 17313 RVA: 0x0013A410 File Offset: 0x00138610
		private void ExecuteRollBackActions()
		{
			for (int i = this.rollBackActions.Count - 1; i >= 0; i--)
			{
				this.ExecuteAction(this.rollBackActions[i]);
			}
			this.rollBackActions.Clear();
		}

		// Token: 0x060043A2 RID: 17314 RVA: 0x0013A454 File Offset: 0x00138654
		private void ExecuteRollbackBufferedCommands()
		{
			for (int i = 0; i < this.rollBackBufferedCommands.Count; i++)
			{
				BuilderTable.BuilderCommand builderCommand = this.rollBackBufferedCommands[i];
				builderCommand.isQueued = false;
				builderCommand.canRollback = false;
				this.ExecuteBuildCommand(builderCommand);
			}
			this.rollBackBufferedCommands.Clear();
		}

		// Token: 0x060043A3 RID: 17315 RVA: 0x0013A4A8 File Offset: 0x001386A8
		private void ExecuteRollForwardCommands()
		{
			BuilderTable.tempRollForwardCommands.Clear();
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				BuilderTable.tempRollForwardCommands.Add(this.rollForwardCommands[i]);
			}
			this.rollForwardCommands.Clear();
			for (int j = 0; j < BuilderTable.tempRollForwardCommands.Count; j++)
			{
				BuilderTable.BuilderCommand builderCommand = BuilderTable.tempRollForwardCommands[j];
				builderCommand.isQueued = true;
				builderCommand.canRollback = true;
				this.ExecuteBuildCommand(builderCommand);
			}
			BuilderTable.tempRollForwardCommands.Clear();
		}

		// Token: 0x060043A4 RID: 17316 RVA: 0x0013A538 File Offset: 0x00138738
		private void UpdateRollForwardCommandData()
		{
			for (int i = 0; i < this.rollForwardCommands.Count; i++)
			{
				BuilderTable.BuilderCommand builderCommand = this.rollForwardCommands[i];
				if (builderCommand.type == BuilderTable.BuilderCommandType.Drop)
				{
					BuilderPiece piece = this.GetPiece(builderCommand.pieceId);
					if (piece != null && piece.rigidBody != null)
					{
						builderCommand.localPosition = piece.rigidBody.position;
						builderCommand.localRotation = piece.rigidBody.rotation;
						builderCommand.velocity = piece.rigidBody.velocity;
						builderCommand.angVelocity = piece.rigidBody.angularVelocity;
						this.rollForwardCommands[i] = builderCommand;
					}
				}
			}
		}

		// Token: 0x060043A5 RID: 17317 RVA: 0x0013A5F0 File Offset: 0x001387F0
		public bool TryRollbackAndReExecute(int localCommandId)
		{
			if (this.HasRollBackActionsForCommand(localCommandId))
			{
				if (this.rollBackBufferedCommands.Count > 0)
				{
					this.UpdateRollForwardCommandData();
					this.ExecuteRollBackActions();
					this.ExecuteRollbackBufferedCommands();
					this.ExecuteRollForwardCommands();
					this.RemoveRollBackActions(localCommandId);
					this.RemoveRollForwardCommands(localCommandId);
				}
				else
				{
					this.RemoveRollBackActions(localCommandId);
					this.RemoveRollForwardCommands(localCommandId);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060043A6 RID: 17318 RVA: 0x0013A64F File Offset: 0x0013884F
		public void RollbackFailedCommand(int localCommandId)
		{
			if (this.HasRollBackActionsForCommand(localCommandId))
			{
				this.UpdateRollForwardCommandData();
				this.ExecuteRollBackActions();
				this.ExecuteRollbackBufferedCommands();
				this.RemoveRollForwardCommands(-1);
				this.ExecuteRollForwardCommands();
			}
		}

		// Token: 0x060043A7 RID: 17319 RVA: 0x0013A679 File Offset: 0x00138879
		public BuilderTable.TableState GetTableState()
		{
			return this.tableState;
		}

		// Token: 0x060043A8 RID: 17320 RVA: 0x0013A684 File Offset: 0x00138884
		public void SetTableState(BuilderTable.TableState newState)
		{
			this.InitIfNeeded();
			if (newState == this.tableState)
			{
				return;
			}
			BuilderTable.TableState tableState = this.tableState;
			this.tableState = newState;
			switch (this.tableState)
			{
			case BuilderTable.TableState.WaitingForInitalBuild:
				if (!this.isTableMutable && !NetworkSystem.Instance.IsMasterClient)
				{
					this.sharedBlocksMap = null;
					UnityEvent onMapCleared = this.OnMapCleared;
					if (onMapCleared == null)
					{
						return;
					}
					onMapCleared.Invoke();
					return;
				}
				break;
			case BuilderTable.TableState.ReceivingInitialBuild:
			case BuilderTable.TableState.ReceivingMasterResync:
			case BuilderTable.TableState.InitialBuild:
			case BuilderTable.TableState.ExecuteQueuedCommands:
				break;
			case BuilderTable.TableState.WaitForInitialBuildMaster:
				this.nextPieceId = 10000;
				if (this.isTableMutable)
				{
					this.BuildInitialTableForPlayer();
					return;
				}
				this.BuildSelectedSharedMap();
				return;
			case BuilderTable.TableState.WaitForMasterResync:
				this.ClearQueuedCommands();
				this.ResetConveyors();
				return;
			case BuilderTable.TableState.Ready:
				this.OnAvailableResourcesChange();
				if (!this.isTableMutable)
				{
					string text = ((this.sharedBlocksMap == null) ? "" : this.sharedBlocksMap.MapID);
					UnityEvent<string> onMapLoaded = this.OnMapLoaded;
					if (onMapLoaded != null)
					{
						onMapLoaded.Invoke(text);
					}
					this.SetPendingMap(null);
					return;
				}
				break;
			case BuilderTable.TableState.BadData:
				this.ClearTable();
				this.ClearQueuedCommands();
				break;
			case BuilderTable.TableState.WaitingForSharedMapLoad:
				this.ClearTable();
				this.ClearQueuedCommands();
				this.builderNetworking.ResetSerializedTableForAllPlayers();
				return;
			default:
				return;
			}
		}

		// Token: 0x060043A9 RID: 17321 RVA: 0x0013A7B0 File Offset: 0x001389B0
		public void SetPendingMap(string mapID)
		{
			this.pendingMapID = mapID;
		}

		// Token: 0x060043AA RID: 17322 RVA: 0x0013A7B9 File Offset: 0x001389B9
		public string GetPendingMap()
		{
			return this.pendingMapID;
		}

		// Token: 0x060043AB RID: 17323 RVA: 0x0013A7C1 File Offset: 0x001389C1
		public string GetCurrentMapID()
		{
			SharedBlocksManager.SharedBlocksMap sharedBlocksMap = this.sharedBlocksMap;
			if (sharedBlocksMap == null)
			{
				return null;
			}
			return sharedBlocksMap.MapID;
		}

		// Token: 0x060043AC RID: 17324 RVA: 0x0013A7D4 File Offset: 0x001389D4
		public void LoadSharedMap(SharedBlocksManager.SharedBlocksMap map)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				if (map.MapID.IsNullOrEmpty())
				{
					GTDev.LogWarning<string>("Invalid map to load", null);
					UnityEvent<string> onMapLoadFailed = this.OnMapLoadFailed;
					if (onMapLoadFailed == null)
					{
						return;
					}
					onMapLoadFailed.Invoke("Invalid Map ID");
					return;
				}
				else
				{
					if (this.tableState == BuilderTable.TableState.Ready || this.tableState == BuilderTable.TableState.BadData)
					{
						this.builderNetworking.RequestLoadSharedBlocksMap(map.MapID);
						return;
					}
					UnityEvent<string> onMapLoadFailed2 = this.OnMapLoadFailed;
					if (onMapLoadFailed2 == null)
					{
						return;
					}
					onMapLoadFailed2.Invoke("WAIT FOR LOAD IN PROGRESS");
					return;
				}
			}
			else
			{
				UnityEvent<string> onMapLoadFailed3 = this.OnMapLoadFailed;
				if (onMapLoadFailed3 == null)
				{
					return;
				}
				onMapLoadFailed3.Invoke("Not In Room");
				return;
			}
		}

		// Token: 0x060043AD RID: 17325 RVA: 0x0013A86C File Offset: 0x00138A6C
		public void SetInRoom(bool inRoom)
		{
			this.inRoom = inRoom;
			bool flag = inRoom && this.inBuilderZone;
			if (!inRoom)
			{
				this.pendingMapID = null;
				this.sharedBlocksMap = null;
				UnityEvent onMapCleared = this.OnMapCleared;
				if (onMapCleared != null)
				{
					onMapCleared.Invoke();
				}
			}
			if (flag && this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				this.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
				this.builderNetworking.PlayerEnterBuilder();
				return;
			}
			if (!flag && this.tableState != BuilderTable.TableState.WaitingForZoneAndRoom && !this.builderNetworking.IsPrivateMasterClient())
			{
				this.SetTableState(BuilderTable.TableState.WaitingForZoneAndRoom);
				this.builderNetworking.PlayerExitBuilder();
				return;
			}
			if (flag && PhotonNetwork.IsMasterClient && this.isTableMutable)
			{
				this.builderNetworking.RequestCreateArmShelfForPlayer(PhotonNetwork.LocalPlayer);
				return;
			}
			if (!flag && this.builderNetworking.IsPrivateMasterClient() && this.isTableMutable)
			{
				this.RemoveArmShelfForPlayer(PhotonNetwork.LocalPlayer);
			}
		}

		// Token: 0x060043AE RID: 17326 RVA: 0x0013A940 File Offset: 0x00138B40
		public static bool IsLocalPlayerInBuilderZone()
		{
			GorillaTagger instance = GorillaTagger.Instance;
			ZoneEntity zoneEntity;
			if (instance == null)
			{
				zoneEntity = null;
			}
			else
			{
				VRRig offlineVRRig = instance.offlineVRRig;
				zoneEntity = ((offlineVRRig != null) ? offlineVRRig.zoneEntity : null);
			}
			ZoneEntity zoneEntity2 = zoneEntity;
			BuilderTable builderTable;
			return !(zoneEntity2 == null) && BuilderTable.TryGetBuilderTableForZone(zoneEntity2.currentZone, out builderTable) && builderTable.IsInBuilderZone();
		}

		// Token: 0x060043AF RID: 17327 RVA: 0x0013A98D File Offset: 0x00138B8D
		public bool IsInBuilderZone()
		{
			return this.inBuilderZone;
		}

		// Token: 0x060043B0 RID: 17328 RVA: 0x0013A998 File Offset: 0x00138B98
		public void SetInBuilderZone(bool inBuilderZone)
		{
			this.inBuilderZone = inBuilderZone;
			if (this.builderRenderer != null)
			{
				this.builderRenderer.Show(inBuilderZone);
			}
			bool flag = this.inRoom && inBuilderZone;
			if (flag && this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom)
			{
				this.SetTableState(BuilderTable.TableState.WaitingForInitalBuild);
				this.builderNetworking.PlayerEnterBuilder();
				return;
			}
			if (!flag && this.tableState != BuilderTable.TableState.WaitingForZoneAndRoom && !this.builderNetworking.IsPrivateMasterClient())
			{
				this.SetTableState(BuilderTable.TableState.WaitingForZoneAndRoom);
				this.builderNetworking.PlayerExitBuilder();
				return;
			}
			if (flag && PhotonNetwork.IsMasterClient)
			{
				this.builderNetworking.RequestCreateArmShelfForPlayer(PhotonNetwork.LocalPlayer);
				return;
			}
			if (!flag && this.builderNetworking.IsPrivateMasterClient())
			{
				this.RemoveArmShelfForPlayer(PhotonNetwork.LocalPlayer);
			}
		}

		// Token: 0x060043B1 RID: 17329 RVA: 0x0013AA50 File Offset: 0x00138C50
		private void UpdateTableState()
		{
			switch (this.tableState)
			{
			case BuilderTable.TableState.InitialBuild:
			{
				BuilderTableNetworking.PlayerTableInitState localTableInit = this.builderNetworking.GetLocalTableInit();
				try
				{
					this.ClearTable();
					this.ClearQueuedCommands();
					byte[] array = GZipStream.UncompressBuffer(localTableInit.serializedTableState);
					localTableInit.totalSerializedBytes = array.Length;
					Array.Copy(array, 0, localTableInit.serializedTableState, 0, localTableInit.totalSerializedBytes);
					this.DeserializeTableState(localTableInit.serializedTableState, localTableInit.numSerializedBytes);
					if (this.tableState == BuilderTable.TableState.BadData)
					{
						return;
					}
					this.SetTableState(BuilderTable.TableState.ExecuteQueuedCommands);
					this.SetIsDirty(true);
					return;
				}
				catch (Exception)
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				break;
			}
			case BuilderTable.TableState.ExecuteQueuedCommands:
				break;
			case BuilderTable.TableState.Ready:
			{
				JobHandle jobHandle = default(JobHandle);
				if (this.isTableMutable)
				{
					this.conveyorManager.UpdateManager();
					jobHandle = this.conveyorManager.ConstructJobHandle();
					JobHandle.ScheduleBatchedJobs();
					foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
					{
						builderDispenserShelf.UpdateShelf();
					}
					foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
					{
						builderPiecePrivatePlot.UpdatePlot();
					}
					foreach (BuilderRecycler builderRecycler in this.recyclers)
					{
						builderRecycler.UpdateRecycler();
					}
					for (int i = this.shelfSliceUpdateIndex; i < this.dispenserShelves.Count; i += BuilderTable.SHELF_SLICE_BUCKETS)
					{
						this.dispenserShelves[i].UpdateShelfSliced();
					}
					this.shelfSliceUpdateIndex = (this.shelfSliceUpdateIndex + 1) % BuilderTable.SHELF_SLICE_BUCKETS;
				}
				foreach (IBuilderPieceFunctional builderPieceFunctional in this.funcComponentsToRegister)
				{
					if (builderPieceFunctional != null)
					{
						this.activeFunctionalComponents.Add(builderPieceFunctional);
					}
				}
				foreach (IBuilderPieceFunctional builderPieceFunctional2 in this.funcComponentsToUnregister)
				{
					this.activeFunctionalComponents.Remove(builderPieceFunctional2);
				}
				this.funcComponentsToRegister.Clear();
				this.funcComponentsToUnregister.Clear();
				foreach (IBuilderPieceFunctional builderPieceFunctional3 in this.activeFunctionalComponents)
				{
					if (builderPieceFunctional3 != null)
					{
						builderPieceFunctional3.FunctionalPieceUpdate();
					}
				}
				if (this.isTableMutable)
				{
					foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
					{
						builderResourceMeter.UpdateMeterFill();
					}
					this.CleanUpDroppedPiece();
					jobHandle.Complete();
					return;
				}
				return;
			}
			default:
				return;
			}
			for (int j = 0; j < this.queuedBuildCommands.Count; j++)
			{
				BuilderTable.BuilderCommand builderCommand = this.queuedBuildCommands[j];
				builderCommand.isQueued = true;
				this.ExecuteBuildCommand(builderCommand);
			}
			this.queuedBuildCommands.Clear();
			this.SetTableState(BuilderTable.TableState.Ready);
		}

		// Token: 0x060043B2 RID: 17330 RVA: 0x0013ADDC File Offset: 0x00138FDC
		private void RouteNewCommand(BuilderTable.BuilderCommand cmd, bool force)
		{
			bool flag = this.ShouldExecuteCommand();
			if (force)
			{
				this.ExecuteBuildCommand(cmd);
				return;
			}
			if (flag && this.ShouldRollbackBufferCommand(cmd))
			{
				this.AddRollbackBufferedCommand(cmd);
				return;
			}
			if (flag)
			{
				this.ExecuteBuildCommand(cmd);
				return;
			}
			if (this.ShouldQueueCommand())
			{
				this.AddQueuedCommand(cmd);
				return;
			}
			this.ShouldDiscardCommand();
		}

		// Token: 0x060043B3 RID: 17331 RVA: 0x0013AE34 File Offset: 0x00139034
		private void ExecuteBuildCommand(BuilderTable.BuilderCommand cmd)
		{
			if (!this.isTableMutable && cmd.type != BuilderTable.BuilderCommandType.FunctionalStateChange)
			{
				return;
			}
			switch (cmd.type)
			{
			case BuilderTable.BuilderCommandType.Create:
				this.ExecutePieceCreated(cmd);
				return;
			case BuilderTable.BuilderCommandType.Place:
				this.ExecutePiecePlacedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Grab:
				this.ExecutePieceGrabbedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Drop:
				this.ExecutePieceDroppedWithActions(cmd);
				return;
			case BuilderTable.BuilderCommandType.Remove:
				break;
			case BuilderTable.BuilderCommandType.Paint:
				this.ExecutePiecePainted(cmd);
				return;
			case BuilderTable.BuilderCommandType.Recycle:
				this.ExecutePieceRecycled(cmd);
				return;
			case BuilderTable.BuilderCommandType.ClaimPlot:
				this.ExecuteClaimPlot(cmd);
				return;
			case BuilderTable.BuilderCommandType.FreePlot:
				this.ExecuteFreePlot(cmd);
				return;
			case BuilderTable.BuilderCommandType.CreateArmShelf:
				this.ExecuteArmShelfCreated(cmd);
				return;
			case BuilderTable.BuilderCommandType.PlayerLeftRoom:
				this.ExecutePlayerLeftRoom(cmd);
				return;
			case BuilderTable.BuilderCommandType.FunctionalStateChange:
				this.ExecuteSetFunctionalPieceState(cmd);
				return;
			case BuilderTable.BuilderCommandType.SetSelection:
				this.ExecuteSetSelection(cmd);
				return;
			case BuilderTable.BuilderCommandType.Repel:
				this.ExecutePieceRepelled(cmd);
				break;
			default:
				return;
			}
		}

		// Token: 0x060043B4 RID: 17332 RVA: 0x0013AF01 File Offset: 0x00139101
		public void ClearTable()
		{
			this.ClearTableInternal();
		}

		// Token: 0x060043B5 RID: 17333 RVA: 0x0013AF0C File Offset: 0x0013910C
		private void ClearTableInternal()
		{
			BuilderTable.tempDeletePieces.Clear();
			for (int i = 0; i < this.pieces.Count; i++)
			{
				BuilderTable.tempDeletePieces.Add(this.pieces[i]);
			}
			if (this.isTableMutable)
			{
				this.droppedPieces.Clear();
				this.droppedPieceData.Clear();
			}
			for (int j = 0; j < BuilderTable.tempDeletePieces.Count; j++)
			{
				BuilderTable.tempDeletePieces[j].ClearParentPiece(false);
				BuilderTable.tempDeletePieces[j].ClearParentHeld();
				BuilderTable.tempDeletePieces[j].SetState(BuilderPiece.State.None, false);
				this.RemovePiece(BuilderTable.tempDeletePieces[j]);
			}
			for (int k = 0; k < BuilderTable.tempDeletePieces.Count; k++)
			{
				this.builderPool.DestroyPiece(BuilderTable.tempDeletePieces[k]);
			}
			BuilderTable.tempDeletePieces.Clear();
			this.pieces.Clear();
			this.pieceIDToIndexCache.Clear();
			this.nextPieceId = 10000;
			if (this.isTableMutable)
			{
				this.conveyorManager.OnClearTable();
				foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
				{
					builderDispenserShelf.OnClearTable();
				}
				for (int l = 0; l < this.repelHistoryLength; l++)
				{
					this.repelledPieceRoots[l].Clear();
				}
			}
			this.funcComponentsToRegister.Clear();
			this.funcComponentsToUnregister.Clear();
			this.activeFunctionalComponents.Clear();
			foreach (BuilderPiece builderPiece in this.basePieces)
			{
				foreach (BuilderAttachGridPlane builderAttachGridPlane in builderPiece.gridPlanes)
				{
					builderAttachGridPlane.OnReturnToPool(this.builderPool);
				}
			}
			if (this.isTableMutable)
			{
				this.ClearBuiltInPlots();
				this.playerToArmShelfLeft.Clear();
				this.playerToArmShelfRight.Clear();
				if (BuilderPieceInteractor.instance != null)
				{
					BuilderPieceInteractor.instance.RemovePiecesFromHands();
				}
			}
		}

		// Token: 0x060043B6 RID: 17334 RVA: 0x0013B178 File Offset: 0x00139378
		private void ClearBuiltInPlots()
		{
			foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
			{
				builderPiecePrivatePlot.ClearPlot();
			}
			this.plotOwners.Clear();
			this.SetLocalPlayerOwnsPlot(false);
		}

		// Token: 0x060043B7 RID: 17335 RVA: 0x0013B1DC File Offset: 0x001393DC
		private void OnDeserializeUpdatePlots()
		{
			foreach (BuilderPiecePrivatePlot builderPiecePrivatePlot in this.allPrivatePlots)
			{
				builderPiecePrivatePlot.RecountPlotCost();
			}
		}

		// Token: 0x060043B8 RID: 17336 RVA: 0x0013B22C File Offset: 0x0013942C
		public void BuildPiecesOnShelves()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (this.shelves == null)
			{
				return;
			}
			for (int i = 0; i < this.shelves.Count; i++)
			{
				if (this.shelves[i] != null)
				{
					this.shelves[i].Init();
				}
			}
			bool flag = true;
			while (flag)
			{
				flag = false;
				for (int j = 0; j < this.shelves.Count; j++)
				{
					if (this.shelves[j].HasOpenSlot())
					{
						this.shelves[j].BuildNextPiece(this);
						if (this.shelves[j].HasOpenSlot())
						{
							flag = true;
						}
					}
				}
			}
		}

		// Token: 0x060043B9 RID: 17337 RVA: 0x0013B2DF File Offset: 0x001394DF
		private void OnFinishedInitialTableBuild()
		{
			this.BuildPiecesOnShelves();
			this.SetTableState(BuilderTable.TableState.Ready);
			this.CreateArmShelvesForPlayersInBuilder();
		}

		// Token: 0x060043BA RID: 17338 RVA: 0x0013B2F4 File Offset: 0x001394F4
		public int CreatePieceId()
		{
			int num = this.nextPieceId;
			if (this.nextPieceId == 2147483647)
			{
				this.nextPieceId = 20000;
			}
			this.nextPieceId++;
			return num;
		}

		// Token: 0x060043BB RID: 17339 RVA: 0x0013B324 File Offset: 0x00139524
		public void ResetConveyors()
		{
			if (this.isTableMutable)
			{
				foreach (BuilderConveyor builderConveyor in this.conveyors)
				{
					builderConveyor.ResetConveyorState();
				}
			}
		}

		// Token: 0x060043BC RID: 17340 RVA: 0x0013B37C File Offset: 0x0013957C
		public void RequestCreateConveyorPiece(int newPieceType, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.conveyors.Count)
			{
				return;
			}
			BuilderConveyor builderConveyor = this.conveyors[shelfID];
			if (builderConveyor == null)
			{
				return;
			}
			Transform spawnTransform = builderConveyor.GetSpawnTransform();
			this.builderNetworking.CreateShelfPiece(newPieceType, spawnTransform.position, spawnTransform.rotation, materialType, BuilderPiece.State.OnConveyor, shelfID);
		}

		// Token: 0x060043BD RID: 17341 RVA: 0x0013B3D5 File Offset: 0x001395D5
		public void RequestCreateDispenserShelfPiece(int pieceType, Vector3 position, Quaternion rotation, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.dispenserShelves.Count)
			{
				return;
			}
			if (this.dispenserShelves[shelfID] == null)
			{
				return;
			}
			this.builderNetworking.CreateShelfPiece(pieceType, position, rotation, materialType, BuilderPiece.State.OnShelf, shelfID);
		}

		// Token: 0x060043BE RID: 17342 RVA: 0x0013B418 File Offset: 0x00139618
		public void CreateConveyorPiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, int shelfID, int sendTimestamp)
		{
			if (shelfID < 0 || shelfID >= this.conveyors.Count)
			{
				return;
			}
			if (this.conveyors[shelfID] == null)
			{
				return;
			}
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = BuilderPiece.State.OnConveyor,
				parentPieceId = shelfID,
				parentAttachIndex = sendTimestamp,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x060043BF RID: 17343 RVA: 0x0013B4C0 File Offset: 0x001396C0
		public void CreateDispenserShelfPiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, int shelfID)
		{
			if (shelfID < 0 || shelfID >= this.dispenserShelves.Count)
			{
				return;
			}
			if (this.dispenserShelves[shelfID] == null)
			{
				return;
			}
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = BuilderPiece.State.OnShelf,
				parentPieceId = shelfID,
				isLeft = true,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x060043C0 RID: 17344 RVA: 0x0013B566 File Offset: 0x00139766
		public void RequestShelfSelection(int shelfId, int setId, bool isConveyor)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestShelfSelection(shelfId, setId, isConveyor);
		}

		// Token: 0x060043C1 RID: 17345 RVA: 0x0013B580 File Offset: 0x00139780
		public void VerifySetSelections()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			foreach (BuilderConveyor builderConveyor in this.conveyors)
			{
				builderConveyor.VerifySetSelection();
			}
			foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
			{
				builderDispenserShelf.VerifySetSelection();
			}
		}

		// Token: 0x060043C2 RID: 17346 RVA: 0x0013B618 File Offset: 0x00139818
		public bool ValidateShelfSelectionParams(int shelfId, int setId, bool isConveyor, Player player)
		{
			bool flag = shelfId >= 0 && ((isConveyor && shelfId < this.conveyors.Count) || (!isConveyor && shelfId < this.dispenserShelves.Count)) && BuilderSetManager.instance.DoesPlayerOwnPieceSet(player, setId);
			if (PhotonNetwork.IsMasterClient)
			{
				if (isConveyor)
				{
					BuilderConveyor builderConveyor = this.conveyors[shelfId];
					bool flag2 = this.IsPlayerHandNearAction(NetPlayer.Get(player), builderConveyor.transform.position, false, true, 4f);
					flag = flag && flag2;
				}
				else
				{
					BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[shelfId];
					bool flag3 = this.IsPlayerHandNearAction(NetPlayer.Get(player), builderDispenserShelf.transform.position, false, true, 4f);
					flag = flag && flag3;
				}
			}
			return flag;
		}

		// Token: 0x060043C3 RID: 17347 RVA: 0x0013B6D4 File Offset: 0x001398D4
		private void SetConveyorSelection(int conveyorId, int setId)
		{
			BuilderConveyor builderConveyor = this.conveyors[conveyorId];
			if (builderConveyor == null)
			{
				return;
			}
			builderConveyor.SetSelection(setId);
		}

		// Token: 0x060043C4 RID: 17348 RVA: 0x0013B700 File Offset: 0x00139900
		private void SetDispenserSelection(int conveyorId, int setId)
		{
			BuilderDispenserShelf builderDispenserShelf = this.dispenserShelves[conveyorId];
			if (builderDispenserShelf == null)
			{
				return;
			}
			builderDispenserShelf.SetSelection(setId);
		}

		// Token: 0x060043C5 RID: 17349 RVA: 0x0013B72C File Offset: 0x0013992C
		public void ChangeSetSelection(int shelfID, int setID, bool isConveyor)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.SetSelection,
				parentPieceId = shelfID,
				pieceType = setID,
				isLeft = isConveyor,
				player = NetworkSystem.Instance.MasterClient
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x060043C6 RID: 17350 RVA: 0x0013B780 File Offset: 0x00139980
		public void ExecuteSetSelection(BuilderTable.BuilderCommand cmd)
		{
			bool isLeft = cmd.isLeft;
			int parentPieceId = cmd.parentPieceId;
			int pieceType = cmd.pieceType;
			if (isLeft)
			{
				this.SetConveyorSelection(parentPieceId, pieceType);
				return;
			}
			this.SetDispenserSelection(parentPieceId, pieceType);
		}

		// Token: 0x060043C7 RID: 17351 RVA: 0x0013B7B4 File Offset: 0x001399B4
		public bool ValidateFunctionalPieceState(int pieceID, byte state, NetPlayer player)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			return !(piece == null) && piece.functionalPieceComponent != null && (!NetworkSystem.Instance.IsMasterClient || player.IsMasterClient || this.IsPlayerHandNearAction(player, piece.transform.position, true, false, piece.functionalPieceComponent.GetInteractionDistace())) && piece.functionalPieceComponent.IsStateValid(state);
		}

		// Token: 0x060043C8 RID: 17352 RVA: 0x0013B824 File Offset: 0x00139A24
		public void OnFunctionalStateRequest(int pieceID, byte state, NetPlayer player, int timeStamp)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			if (piece == null)
			{
				return;
			}
			if (piece.functionalPieceComponent == null)
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			piece.functionalPieceComponent.OnStateRequest(state, player, timeStamp);
		}

		// Token: 0x060043C9 RID: 17353 RVA: 0x0013B860 File Offset: 0x00139A60
		public void SetFunctionalPieceState(int pieceID, byte state, NetPlayer player, int timeStamp)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.FunctionalStateChange,
				pieceId = pieceID,
				twist = state,
				player = player,
				serverTimeStamp = timeStamp
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x060043CA RID: 17354 RVA: 0x0013B8AC File Offset: 0x00139AAC
		public void ExecuteSetFunctionalPieceState(BuilderTable.BuilderCommand cmd)
		{
			BuilderPiece piece = this.GetPiece(cmd.pieceId);
			if (piece == null)
			{
				return;
			}
			piece.SetFunctionalPieceState(cmd.twist, cmd.player, cmd.serverTimeStamp);
		}

		// Token: 0x060043CB RID: 17355 RVA: 0x0013B8E8 File Offset: 0x00139AE8
		public void RegisterFunctionalPiece(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegister.Add(component);
			}
		}

		// Token: 0x060043CC RID: 17356 RVA: 0x0013B8F9 File Offset: 0x00139AF9
		public void UnregisterFunctionalPiece(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToUnregister.Add(component);
			}
		}

		// Token: 0x060043CD RID: 17357 RVA: 0x0013B90A File Offset: 0x00139B0A
		public void RegisterFunctionalPieceFixedUpdate(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegisterFixed.Add(component);
			}
		}

		// Token: 0x060043CE RID: 17358 RVA: 0x0013B91B File Offset: 0x00139B1B
		public void UnregisterFunctionalPieceFixedUpdate(IBuilderPieceFunctional component)
		{
			if (component != null)
			{
				this.funcComponentsToRegisterFixed.Remove(component);
			}
		}

		// Token: 0x060043CF RID: 17359 RVA: 0x000023F4 File Offset: 0x000005F4
		public void RequestCreatePiece(int newPieceType, Vector3 position, Quaternion rotation, int materialType)
		{
		}

		// Token: 0x060043D0 RID: 17360 RVA: 0x0013B930 File Offset: 0x00139B30
		public void CreatePiece(int pieceType, int pieceId, Vector3 position, Quaternion rotation, int materialType, BuilderPiece.State state, Player player)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Create,
				pieceType = pieceType,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				materialType = materialType,
				state = state,
				player = NetPlayer.Get(player)
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x060043D1 RID: 17361 RVA: 0x0013B998 File Offset: 0x00139B98
		public void RequestRecyclePiece(BuilderPiece piece, bool playFX, int recyclerID)
		{
			this.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, playFX, recyclerID);
		}

		// Token: 0x060043D2 RID: 17362 RVA: 0x0013B9C4 File Offset: 0x00139BC4
		public void RecyclePiece(int pieceId, Vector3 position, Quaternion rotation, bool playFX, int recyclerID, Player player)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Recycle,
				pieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				player = NetPlayer.Get(player),
				isLeft = playFX,
				parentPieceId = recyclerID
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x060043D3 RID: 17363 RVA: 0x0013BA23 File Offset: 0x00139C23
		private bool ShouldExecuteCommand()
		{
			return this.tableState == BuilderTable.TableState.Ready || this.tableState == BuilderTable.TableState.WaitForInitialBuildMaster;
		}

		// Token: 0x060043D4 RID: 17364 RVA: 0x0013BA39 File Offset: 0x00139C39
		private bool ShouldQueueCommand()
		{
			return this.tableState == BuilderTable.TableState.ReceivingInitialBuild || this.tableState == BuilderTable.TableState.ReceivingMasterResync || this.tableState == BuilderTable.TableState.InitialBuild || this.tableState == BuilderTable.TableState.ExecuteQueuedCommands;
		}

		// Token: 0x060043D5 RID: 17365 RVA: 0x0013BA61 File Offset: 0x00139C61
		private bool ShouldDiscardCommand()
		{
			return this.tableState == BuilderTable.TableState.WaitingForInitalBuild || this.tableState == BuilderTable.TableState.WaitForInitialBuildMaster || this.tableState == BuilderTable.TableState.WaitingForZoneAndRoom;
		}

		// Token: 0x060043D6 RID: 17366 RVA: 0x0013BA80 File Offset: 0x00139C80
		public bool DoesChainContainPiece(BuilderPiece targetPiece, BuilderPiece firstInChain, BuilderPiece nextInChain)
		{
			return !(targetPiece == null) && !(firstInChain == null) && (targetPiece.Equals(firstInChain) || (!(nextInChain == null) && (targetPiece.Equals(nextInChain) || (!(firstInChain == nextInChain) && this.DoesChainContainPiece(targetPiece, firstInChain, nextInChain.parentPiece)))));
		}

		// Token: 0x060043D7 RID: 17367 RVA: 0x0013BADC File Offset: 0x00139CDC
		public bool DoesChainContainChain(BuilderPiece chainARoot, BuilderPiece chainBAttachPiece)
		{
			if (chainARoot == null || chainBAttachPiece == null)
			{
				return false;
			}
			if (this.DoesChainContainPiece(chainARoot, chainBAttachPiece, chainBAttachPiece.parentPiece))
			{
				return true;
			}
			BuilderPiece builderPiece = chainARoot.firstChildPiece;
			while (builderPiece != null)
			{
				if (this.DoesChainContainChain(builderPiece, chainBAttachPiece))
				{
					return true;
				}
				builderPiece = builderPiece.nextSiblingPiece;
			}
			return false;
		}

		// Token: 0x060043D8 RID: 17368 RVA: 0x0013BB38 File Offset: 0x00139D38
		private bool IsPlayerHandNearAction(NetPlayer player, Vector3 worldPosition, bool isLeftHand, bool checkBothHands, float acceptableRadius = 2.5f)
		{
			bool flag = true;
			RigContainer rigContainer;
			if (player != null && VRRigCache.Instance != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				if (isLeftHand || checkBothHands)
				{
					flag = (worldPosition - rigContainer.Rig.leftHandTransform.position).sqrMagnitude < acceptableRadius * acceptableRadius;
				}
				if (!isLeftHand || checkBothHands)
				{
					float sqrMagnitude = (worldPosition - rigContainer.Rig.rightHandTransform.position).sqrMagnitude;
					flag = flag && sqrMagnitude < acceptableRadius * acceptableRadius;
				}
			}
			return flag;
		}

		// Token: 0x060043D9 RID: 17369 RVA: 0x0013BBCC File Offset: 0x00139DCC
		public bool ValidatePlacePieceParams(int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			if (piece2 == null)
			{
				return false;
			}
			if (piece.heldByPlayerActorNumber != placedByPlayer.ActorNumber)
			{
				return false;
			}
			if (piece.isBuiltIntoTable || piece2.isBuiltIntoTable)
			{
				return false;
			}
			if (twist > 3)
			{
				return false;
			}
			BuilderPiece piece3 = this.GetPiece(parentPieceId);
			if (!(piece3 != null))
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(placedByPlayer.ActorNumber, piece2, piece3))
			{
				return false;
			}
			if (this.DoesChainContainChain(piece2, piece3))
			{
				return false;
			}
			if (attachIndex < 0 || attachIndex >= piece2.gridPlanes.Count)
			{
				return false;
			}
			if (piece3 != null && (parentAttachIndex < 0 || parentAttachIndex >= piece3.gridPlanes.Count))
			{
				return false;
			}
			if (piece3 != null)
			{
				bool flag = (long)(twist % 2) == 1L;
				BuilderAttachGridPlane builderAttachGridPlane = piece2.gridPlanes[attachIndex];
				int num = (flag ? builderAttachGridPlane.length : builderAttachGridPlane.width);
				int num2 = (flag ? builderAttachGridPlane.width : builderAttachGridPlane.length);
				BuilderAttachGridPlane builderAttachGridPlane2 = piece3.gridPlanes[parentAttachIndex];
				int num3 = Mathf.FloorToInt((float)builderAttachGridPlane2.width / 2f);
				int num4 = num3 - (builderAttachGridPlane2.width - 1);
				if ((int)bumpOffsetX < num4 - num || (int)bumpOffsetX > num3 + num)
				{
					return false;
				}
				int num5 = Mathf.FloorToInt((float)builderAttachGridPlane2.length / 2f);
				int num6 = num5 - (builderAttachGridPlane2.length - 1);
				if ((int)bumpOffsetZ < num6 - num2 || (int)bumpOffsetZ > num5 + num2)
				{
					return false;
				}
			}
			if (placedByPlayer == null)
			{
				return false;
			}
			if (PhotonNetwork.IsMasterClient && piece3 != null)
			{
				Vector3 vector;
				Quaternion quaternion;
				Vector3 vector2;
				Quaternion quaternion2;
				piece2.BumpTwistToPositionRotation(twist, bumpOffsetX, bumpOffsetZ, attachIndex, piece3.gridPlanes[parentAttachIndex], out vector, out quaternion, out vector2, out quaternion2);
				Vector3 vector3 = piece2.transform.InverseTransformPoint(piece.transform.position);
				Vector3 vector4 = vector2 + quaternion2 * vector3;
				if (!this.IsPlayerHandNearAction(placedByPlayer, vector4, piece.heldInLeftHand, false, 2.5f))
				{
					return false;
				}
				if (!this.ValidatePieceWorldTransform(vector2, quaternion2))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060043DA RID: 17370 RVA: 0x0013BDE0 File Offset: 0x00139FE0
		public bool ValidatePlacePieceState(int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, Player placedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			return !(piece2 == null) && !(this.GetPiece(parentPieceId) == null) && placedByPlayer != null && !piece2.GetRootPiece() != piece;
		}

		// Token: 0x060043DB RID: 17371 RVA: 0x0013BE44 File Offset: 0x0013A044
		public void ExecutePieceCreated(BuilderTable.BuilderCommand cmd)
		{
			if ((cmd.player == null || !cmd.player.IsLocal) && !this.ValidateCreatePieceParams(cmd.pieceType, cmd.pieceId, cmd.state, cmd.materialType))
			{
				return;
			}
			BuilderPiece builderPiece = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, cmd.localPosition, cmd.localRotation, cmd.state, cmd.materialType, 0, this);
			if (!(builderPiece != null) || cmd.state != BuilderPiece.State.OnConveyor)
			{
				if (builderPiece != null && cmd.isLeft && cmd.state == BuilderPiece.State.OnShelf)
				{
					if (cmd.parentPieceId < 0 || cmd.parentPieceId >= this.dispenserShelves.Count)
					{
						return;
					}
					builderPiece.shelfOwner = cmd.parentPieceId;
					this.dispenserShelves[builderPiece.shelfOwner].OnShelfPieceCreated(builderPiece, true);
				}
				return;
			}
			if (cmd.parentPieceId < 0 || cmd.parentPieceId >= this.conveyors.Count)
			{
				return;
			}
			builderPiece.shelfOwner = cmd.parentPieceId;
			BuilderConveyor builderConveyor = this.conveyors[builderPiece.shelfOwner];
			int parentAttachIndex = cmd.parentAttachIndex;
			float num = 0f;
			if (PhotonNetwork.ServerTimestamp > parentAttachIndex)
			{
				num = (PhotonNetwork.ServerTimestamp - parentAttachIndex) / 1000f;
			}
			builderConveyor.OnShelfPieceCreated(builderPiece, num);
		}

		// Token: 0x060043DC RID: 17372 RVA: 0x0013BF8B File Offset: 0x0013A18B
		public void ExecutePieceRecycled(BuilderTable.BuilderCommand cmd)
		{
			this.RecyclePieceInternal(cmd.pieceId, false, cmd.isLeft, cmd.parentPieceId);
		}

		// Token: 0x060043DD RID: 17373 RVA: 0x0013BFA6 File Offset: 0x0013A1A6
		private bool ValidateCreatePieceParams(int newPieceType, int newPieceId, BuilderPiece.State state, int materialType)
		{
			return !(this.GetPiecePrefab(newPieceType) == null) && !(this.GetPiece(newPieceId) != null);
		}

		// Token: 0x060043DE RID: 17374 RVA: 0x0013BFCC File Offset: 0x0013A1CC
		private bool ValidateDeserializedRootPieceState(int pieceId, BuilderPiece.State state, int shelfOwner, int heldByActor, Vector3 localPosition, Quaternion localRotation)
		{
			switch (state)
			{
			case BuilderPiece.State.Grabbed:
			case BuilderPiece.State.GrabbedLocal:
				if (heldByActor == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. held piece in immutable table {0}", pieceId), null);
				}
				else if (localPosition.sqrMagnitude > 6.25f)
				{
					return false;
				}
				break;
			case BuilderPiece.State.Dropped:
				if (!this.ValidatePieceWorldTransform(localPosition, localRotation))
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. dropped piece in immutable table {0}", pieceId), null);
					return false;
				}
				break;
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.Displayed:
				if (shelfOwner == -1 && !this.ValidatePieceWorldTransform(localPosition, localRotation))
				{
					return false;
				}
				break;
			case BuilderPiece.State.OnConveyor:
				if (shelfOwner == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. OnConveyor piece in immutable table {0}", pieceId), null);
					return false;
				}
				break;
			case BuilderPiece.State.AttachedToArm:
				if (heldByActor == -1)
				{
					return false;
				}
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. AttachedToArm piece in immutable table {0}", pieceId), null);
					return false;
				}
				if (localPosition.sqrMagnitude > 6.25f)
				{
					return false;
				}
				break;
			default:
				return false;
			}
			return true;
		}

		// Token: 0x060043DF RID: 17375 RVA: 0x0013C0E4 File Offset: 0x0013A2E4
		private bool ValidateDeserializedChildPieceState(int pieceId, BuilderPiece.State state)
		{
			switch (state)
			{
			case BuilderPiece.State.AttachedAndPlaced:
			case BuilderPiece.State.OnShelf:
			case BuilderPiece.State.Displayed:
				return true;
			case BuilderPiece.State.AttachedToDropped:
			case BuilderPiece.State.Grabbed:
			case BuilderPiece.State.GrabbedLocal:
			case BuilderPiece.State.AttachedToArm:
				if (!this.isTableMutable)
				{
					GTDev.LogError<string>(string.Format("Deserialized bad CreatePiece parameters. Invalid state {0} of child piece {1} in Immutable table", state, pieceId), null);
					return false;
				}
				return true;
			}
			return false;
		}

		// Token: 0x060043E0 RID: 17376 RVA: 0x0013C148 File Offset: 0x0013A348
		public bool ValidatePieceWorldTransform(Vector3 position, Quaternion rotation)
		{
			float num = 10000f;
			return (in position).IsValid(in num) && (in rotation).IsValid() && (this.roomCenter.position - position).sqrMagnitude <= this.acceptableSqrDistFromCenter;
		}

		// Token: 0x060043E1 RID: 17377 RVA: 0x0013C198 File Offset: 0x0013A398
		private BuilderPiece CreatePieceInternal(int newPieceType, int newPieceId, Vector3 position, Quaternion rotation, BuilderPiece.State state, int materialType, int activateTimeStamp, BuilderTable table)
		{
			if (this.GetPiecePrefab(newPieceType) == null)
			{
				return null;
			}
			if (!PhotonNetwork.IsMasterClient)
			{
				this.nextPieceId = newPieceId + 1;
			}
			BuilderPiece builderPiece = this.builderPool.CreatePiece(newPieceType, false);
			builderPiece.SetScale(table.pieceScale);
			builderPiece.transform.SetPositionAndRotation(position, rotation);
			builderPiece.pieceType = newPieceType;
			builderPiece.pieceId = newPieceId;
			builderPiece.SetTable(table);
			builderPiece.gameObject.SetActive(true);
			builderPiece.SetupPiece(this.gridSize);
			builderPiece.OnCreate();
			builderPiece.activatedTimeStamp = ((state == BuilderPiece.State.AttachedAndPlaced) ? activateTimeStamp : 0);
			builderPiece.SetMaterial(materialType, true);
			builderPiece.SetState(state, true);
			this.AddPiece(builderPiece);
			return builderPiece;
		}

		// Token: 0x060043E2 RID: 17378 RVA: 0x0013C24C File Offset: 0x0013A44C
		private void RecyclePieceInternal(int pieceId, bool ignoreHaptics, bool playFX, int recyclerId)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			if (playFX)
			{
				try
				{
					piece.PlayRecycleFx();
				}
				catch (Exception)
				{
				}
			}
			if (!ignoreHaptics)
			{
				BuilderPiece rootPiece = piece.GetRootPiece();
				if (rootPiece != null && rootPiece.IsHeldLocal())
				{
					GorillaTagger.Instance.StartVibration(piece.IsHeldInLeftHand(), GorillaTagger.Instance.tapHapticStrength, this.pushAndEaseParams.snapDelayTime * 2f);
				}
			}
			BuilderPiece builderPiece = piece.firstChildPiece;
			while (builderPiece != null)
			{
				int pieceId2 = builderPiece.pieceId;
				builderPiece = builderPiece.nextSiblingPiece;
				this.RecyclePieceInternal(pieceId2, true, playFX, recyclerId);
			}
			if (this.isTableMutable && recyclerId >= 0 && recyclerId < this.recyclers.Count)
			{
				this.recyclers[recyclerId].OnRecycleRequestedAtRecycler(piece);
			}
			if (piece.state == BuilderPiece.State.OnConveyor && piece.shelfOwner >= 0 && piece.shelfOwner < this.conveyors.Count)
			{
				this.conveyors[piece.shelfOwner].OnShelfPieceRecycled(piece);
			}
			else if ((piece.state == BuilderPiece.State.OnShelf || piece.state == BuilderPiece.State.Displayed) && piece.shelfOwner >= 0 && piece.shelfOwner < this.dispenserShelves.Count)
			{
				this.dispenserShelves[piece.shelfOwner].OnShelfPieceRecycled(piece);
			}
			if (piece.isArmShelf && this.isTableMutable)
			{
				if (piece.armShelf != null)
				{
					piece.armShelf.piece = null;
					piece.armShelf = null;
				}
				int num;
				if (piece.heldInLeftHand && this.playerToArmShelfLeft.TryGetValue(piece.heldByPlayerActorNumber, out num) && num == piece.pieceId)
				{
					this.playerToArmShelfLeft.Remove(piece.heldByPlayerActorNumber);
				}
				int num2;
				if (!piece.heldInLeftHand && this.playerToArmShelfRight.TryGetValue(piece.heldByPlayerActorNumber, out num2) && num2 == piece.pieceId)
				{
					this.playerToArmShelfRight.Remove(piece.heldByPlayerActorNumber);
				}
			}
			else if (PhotonNetwork.LocalPlayer.ActorNumber == piece.heldByPlayerActorNumber)
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			int pieceId3 = piece.pieceId;
			piece.ClearParentPiece(false);
			piece.ClearParentHeld();
			piece.SetState(BuilderPiece.State.None, false);
			this.RemovePiece(piece);
			this.builderPool.DestroyPiece(piece);
		}

		// Token: 0x060043E3 RID: 17379 RVA: 0x0013C4A8 File Offset: 0x0013A6A8
		public BuilderPiece GetPiecePrefab(int pieceType)
		{
			return this.factories[0].GetPiecePrefab(pieceType);
		}

		// Token: 0x060043E4 RID: 17380 RVA: 0x0013C4BC File Offset: 0x0013A6BC
		private bool ValidateAttachPieceParams(int pieceId, int attachIndex, int parentId, int parentAttachIndex, int piecePlacement)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			BuilderPiece piece2 = this.GetPiece(parentId);
			if (piece2 == null)
			{
				return false;
			}
			if ((piecePlacement & 262143) != piecePlacement)
			{
				return false;
			}
			if (piece.isBuiltIntoTable)
			{
				return false;
			}
			if (this.DoesChainContainChain(piece, piece2))
			{
				return false;
			}
			if (attachIndex < 0 || attachIndex >= piece.gridPlanes.Count)
			{
				return false;
			}
			if (parentAttachIndex < 0 || parentAttachIndex >= piece2.gridPlanes.Count)
			{
				return false;
			}
			byte b;
			sbyte b2;
			sbyte b3;
			BuilderTable.UnpackPiecePlacement(piecePlacement, out b, out b2, out b3);
			bool flag = (long)(b % 2) == 1L;
			BuilderAttachGridPlane builderAttachGridPlane = piece.gridPlanes[attachIndex];
			int num = (flag ? builderAttachGridPlane.length : builderAttachGridPlane.width);
			int num2 = (flag ? builderAttachGridPlane.width : builderAttachGridPlane.length);
			BuilderAttachGridPlane builderAttachGridPlane2 = piece2.gridPlanes[parentAttachIndex];
			int num3 = Mathf.FloorToInt((float)builderAttachGridPlane2.width / 2f);
			int num4 = num3 - (builderAttachGridPlane2.width - 1);
			if ((int)b2 < num4 - num || (int)b2 > num3 + num)
			{
				return false;
			}
			int num5 = Mathf.FloorToInt((float)builderAttachGridPlane2.length / 2f);
			int num6 = num5 - (builderAttachGridPlane2.length - 1);
			return (int)b3 >= num6 - num2 && (int)b3 <= num5 + num2;
		}

		// Token: 0x060043E5 RID: 17381 RVA: 0x0013C608 File Offset: 0x0013A808
		private void AttachPieceInternal(int pieceId, int attachIndex, int parentId, int parentAttachIndex, int placement)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			BuilderPiece piece2 = this.GetPiece(parentId);
			if (piece == null)
			{
				return;
			}
			byte b;
			sbyte b2;
			sbyte b3;
			BuilderTable.UnpackPiecePlacement(placement, out b, out b2, out b3);
			Vector3 zero = Vector3.zero;
			Quaternion quaternion;
			if (piece2 != null && parentAttachIndex >= 0 && parentAttachIndex < piece2.gridPlanes.Count)
			{
				Vector3 vector;
				Quaternion quaternion2;
				piece.BumpTwistToPositionRotation(b, b2, b3, attachIndex, piece2.gridPlanes[parentAttachIndex], out zero, out quaternion, out vector, out quaternion2);
			}
			else
			{
				quaternion = Quaternion.Euler(0f, (float)b * 90f, 0f);
			}
			piece.SetParentPiece(attachIndex, piece2, parentAttachIndex);
			piece.transform.SetLocalPositionAndRotation(zero, quaternion);
		}

		// Token: 0x060043E6 RID: 17382 RVA: 0x0013C6B4 File Offset: 0x0013A8B4
		private void AttachPieceToActorInternal(int pieceId, int actorNumber, bool isLeftHand)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				return;
			}
			VRRig rig = rigContainer.Rig;
			BodyDockPositions myBodyDockPositions = rig.myBodyDockPositions;
			Transform transform = (isLeftHand ? myBodyDockPositions.leftHandTransform : myBodyDockPositions.rightHandTransform);
			if (piece.isArmShelf)
			{
				if (!this.isTableMutable)
				{
					return;
				}
				transform = (isLeftHand ? rig.builderArmShelfLeft.pieceAnchor : rig.builderArmShelfRight.pieceAnchor);
				if (isLeftHand)
				{
					rig.builderArmShelfLeft.piece = piece;
					piece.armShelf = rig.builderArmShelfLeft;
					int num;
					if (this.playerToArmShelfLeft.TryGetValue(actorNumber, out num) && num != pieceId)
					{
						BuilderPiece piece2 = this.GetPiece(num);
						if (piece2 != null && piece2.isArmShelf)
						{
							piece2.ClearParentHeld();
							this.playerToArmShelfLeft.Remove(actorNumber);
						}
					}
					this.playerToArmShelfLeft.TryAdd(actorNumber, pieceId);
				}
				else
				{
					rig.builderArmShelfRight.piece = piece;
					piece.armShelf = rig.builderArmShelfRight;
					int num2;
					if (this.playerToArmShelfRight.TryGetValue(actorNumber, out num2) && num2 != pieceId)
					{
						BuilderPiece piece3 = this.GetPiece(num2);
						if (piece3 != null && piece3.isArmShelf)
						{
							piece3.ClearParentHeld();
							this.playerToArmShelfRight.Remove(actorNumber);
						}
					}
					this.playerToArmShelfRight.TryAdd(actorNumber, pieceId);
				}
			}
			Vector3 localPosition = piece.transform.localPosition;
			Quaternion localRotation = piece.transform.localRotation;
			piece.ClearParentHeld();
			piece.ClearParentPiece(false);
			piece.SetParentHeld(transform, actorNumber, isLeftHand);
			piece.transform.SetLocalPositionAndRotation(localPosition, localRotation);
			BuilderPiece.State state = (player.IsLocal ? BuilderPiece.State.GrabbedLocal : BuilderPiece.State.Grabbed);
			if (piece.isArmShelf)
			{
				state = BuilderPiece.State.AttachedToArm;
				piece.transform.localScale = Vector3.one;
			}
			piece.SetState(state, false);
			if (!player.IsLocal)
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			if (player.IsLocal && !piece.isArmShelf)
			{
				BuilderPieceInteractor.instance.AddPieceToHeld(piece, isLeftHand, localPosition, localRotation);
			}
		}

		// Token: 0x060043E7 RID: 17383 RVA: 0x0013C8CC File Offset: 0x0013AACC
		public void RequestPlacePiece(BuilderPiece piece, BuilderPiece attachPiece, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, BuilderPiece parentPiece, int attachIndex, int parentAttachIndex)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestPlacePiece(piece, attachPiece, bumpOffsetX, bumpOffsetZ, twist, parentPiece, attachIndex, parentAttachIndex);
		}

		// Token: 0x060043E8 RID: 17384 RVA: 0x0013C8FC File Offset: 0x0013AAFC
		public void PlacePiece(int localCommandId, int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer, int timeStamp, bool force)
		{
			this.PiecePlacedInternal(localCommandId, pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, placedByPlayer, timeStamp, force);
		}

		// Token: 0x060043E9 RID: 17385 RVA: 0x0013C924 File Offset: 0x0013AB24
		public void PiecePlacedInternal(int localCommandId, int pieceId, int attachPieceId, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, int parentPieceId, int attachIndex, int parentAttachIndex, NetPlayer placedByPlayer, int timeStamp, bool force)
		{
			if (!force && placedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Place,
				pieceId = pieceId,
				bumpOffsetX = bumpOffsetX,
				bumpOffsetZ = bumpOffsetZ,
				twist = twist,
				attachPieceId = attachPieceId,
				parentPieceId = parentPieceId,
				attachIndex = attachIndex,
				parentAttachIndex = parentAttachIndex,
				player = placedByPlayer,
				canRollback = force,
				localCommandId = localCommandId,
				serverTimeStamp = timeStamp
			};
			this.RouteNewCommand(builderCommand, force);
		}

		// Token: 0x060043EA RID: 17386 RVA: 0x0013C9DC File Offset: 0x0013ABDC
		public void ExecutePiecePlacedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int attachPieceId = cmd.attachPieceId;
			int parentPieceId = cmd.parentPieceId;
			int parentAttachIndex = cmd.parentAttachIndex;
			int attachIndex = cmd.attachIndex;
			NetPlayer player = cmd.player;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			byte twist = cmd.twist;
			sbyte bumpOffsetX = cmd.bumpOffsetX;
			sbyte bumpOffsetZ = cmd.bumpOffsetZ;
			if ((player == null || !player.IsLocal) && !this.ValidatePlacePieceParams(pieceId, attachPieceId, bumpOffsetX, bumpOffsetZ, twist, parentPieceId, attachIndex, parentAttachIndex, player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			BuilderPiece piece2 = this.GetPiece(attachPieceId);
			if (piece2 == null)
			{
				return;
			}
			BuilderAction builderAction = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			BuilderAction builderAction2 = BuilderActions.CreateMakeRoot(localCommandId, attachPieceId);
			BuilderAction builderAction3 = BuilderActions.CreateAttachToPiece(localCommandId, attachPieceId, cmd.parentPieceId, cmd.attachIndex, cmd.parentAttachIndex, bumpOffsetX, bumpOffsetZ, twist, actorNumber, cmd.serverTimeStamp);
			if (cmd.canRollback)
			{
				BuilderAction builderAction4 = BuilderActions.CreateDetachFromPiece(localCommandId, attachPieceId, actorNumber);
				BuilderAction builderAction5 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderAction builderAction6 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
				this.AddRollbackAction(builderAction6);
				this.AddRollbackAction(builderAction5);
				this.AddRollbackAction(builderAction4);
				this.AddRollForwardCommand(cmd);
			}
			this.ExecuteAction(builderAction);
			this.ExecuteAction(builderAction2);
			this.ExecuteAction(builderAction3);
			if (!cmd.isQueued)
			{
				piece2.PlayPlacementFx();
			}
		}

		// Token: 0x060043EB RID: 17387 RVA: 0x0013CB40 File Offset: 0x0013AD40
		public bool ValidateGrabPieceParams(int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer)
		{
			float num = 10000f;
			if (!(in localPosition).IsValid(in num) || !(in localRotation).IsValid())
			{
				return false;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			if (piece.isBuiltIntoTable)
			{
				return false;
			}
			if (grabbedByPlayer == null)
			{
				return false;
			}
			if (!piece.CanPlayerGrabPiece(grabbedByPlayer.ActorNumber, piece.transform.position))
			{
				return false;
			}
			if (localPosition.sqrMagnitude > 6400f)
			{
				return false;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				Vector3 position = piece.transform.position;
				if (!this.IsPlayerHandNearAction(grabbedByPlayer, position, isLeftHand, false, 2.5f))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060043EC RID: 17388 RVA: 0x0013CBE0 File Offset: 0x0013ADE0
		public bool ValidateGrabPieceState(int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, Player grabbedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			return !(piece == null) && piece.state != BuilderPiece.State.Displayed && piece.state != BuilderPiece.State.None;
		}

		// Token: 0x060043ED RID: 17389 RVA: 0x0013CC18 File Offset: 0x0013AE18
		public bool IsLocationWithinSharedBuildArea(Vector3 worldPosition)
		{
			Vector3 vector = this.sharedBuildArea.transform.InverseTransformPoint(worldPosition);
			foreach (BoxCollider boxCollider in this.sharedBuildAreas)
			{
				Vector3 vector2 = boxCollider.center + boxCollider.size / 2f;
				Vector3 vector3 = boxCollider.center - boxCollider.size / 2f;
				if (vector.x >= vector3.x && vector.x <= vector2.x && vector.y >= vector3.y && vector.y <= vector2.y && vector.z >= vector3.z && vector.z <= vector2.z)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060043EE RID: 17390 RVA: 0x0013CCF8 File Offset: 0x0013AEF8
		private bool NoBlocksCheck()
		{
			foreach (BuilderTable.BoxCheckParams boxCheckParams in this.noBlocksAreas)
			{
				DebugUtil.DrawBox(boxCheckParams.center, boxCheckParams.rotation, boxCheckParams.halfExtents * 2f, Color.magenta, true, DebugUtil.Style.Wireframe);
				int num = 0;
				num |= 1 << BuilderTable.placedLayer;
				int num2 = Physics.OverlapBoxNonAlloc(boxCheckParams.center, boxCheckParams.halfExtents, this.noBlocksCheckResults, boxCheckParams.rotation, num);
				for (int i = 0; i < num2; i++)
				{
					BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(this.noBlocksCheckResults[i]);
					if (builderPieceFromCollider != null && builderPieceFromCollider.GetTable() == this && builderPieceFromCollider.state == BuilderPiece.State.AttachedAndPlaced && !builderPieceFromCollider.isBuiltIntoTable)
					{
						GTDev.LogError<string>(string.Format("Builder Table found piece {0} {1} in NO BLOCK AREA {2}", builderPieceFromCollider.pieceId, builderPieceFromCollider.displayName, builderPieceFromCollider.transform.position), null);
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060043EF RID: 17391 RVA: 0x0013CE28 File Offset: 0x0013B028
		public void RequestGrabPiece(BuilderPiece piece, bool isLefHand, Vector3 localPosition, Quaternion localRotation)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestGrabPiece(piece, isLefHand, localPosition, localRotation);
		}

		// Token: 0x060043F0 RID: 17392 RVA: 0x0013CE44 File Offset: 0x0013B044
		public void GrabPiece(int localCommandId, int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer, bool force)
		{
			this.PieceGrabbedInternal(localCommandId, pieceId, isLeftHand, localPosition, localRotation, grabbedByPlayer, force);
		}

		// Token: 0x060043F1 RID: 17393 RVA: 0x0013CE58 File Offset: 0x0013B058
		public void PieceGrabbedInternal(int localCommandId, int pieceId, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, NetPlayer grabbedByPlayer, bool force)
		{
			if (!force && grabbedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Grab,
				pieceId = pieceId,
				attachPieceId = -1,
				isLeft = isLeftHand,
				localPosition = localPosition,
				localRotation = localRotation,
				player = grabbedByPlayer,
				canRollback = force,
				localCommandId = localCommandId
			};
			this.RouteNewCommand(builderCommand, force);
		}

		// Token: 0x060043F2 RID: 17394 RVA: 0x0013CEEC File Offset: 0x0013B0EC
		public void ExecutePieceGrabbedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			bool isLeft = cmd.isLeft;
			NetPlayer player = cmd.player;
			Vector3 localPosition = cmd.localPosition;
			Quaternion localRotation = cmd.localRotation;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			if ((player == null || !player.Equals(NetworkSystem.Instance.LocalPlayer)) && !this.ValidateGrabPieceParams(pieceId, isLeft, localPosition, localRotation, player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			bool flag = PhotonNetwork.CurrentRoom.GetPlayer(piece.heldByPlayerActorNumber, false) != null;
			bool flag2 = BuilderPiece.IsDroppedState(piece.state);
			bool flag3 = piece.state == BuilderPiece.State.OnConveyor || piece.state == BuilderPiece.State.OnShelf || piece.state == BuilderPiece.State.Displayed;
			BuilderAction builderAction = BuilderActions.CreateAttachToPlayer(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, actorNumber, cmd.isLeft);
			BuilderAction builderAction2 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			if (flag)
			{
				BuilderAction builderAction3 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				if (cmd.canRollback)
				{
					BuilderAction builderAction4 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
					this.AddRollbackAction(builderAction4);
					this.AddRollbackAction(builderAction2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(builderAction3);
				this.ExecuteAction(builderAction);
				return;
			}
			if (flag3)
			{
				BuilderAction builderAction5;
				if (piece.state == BuilderPiece.State.OnConveyor)
				{
					int serverTimestamp = PhotonNetwork.ServerTimestamp;
					float splineProgressForPiece = this.conveyorManager.GetSplineProgressForPiece(piece);
					builderAction5 = BuilderActions.CreateAttachToShelfRollback(localCommandId, piece, piece.shelfOwner, true, serverTimestamp, splineProgressForPiece);
				}
				else
				{
					if (piece.state == BuilderPiece.State.Displayed)
					{
						int actorNumber2 = NetworkSystem.Instance.LocalPlayer.ActorNumber;
					}
					builderAction5 = BuilderActions.CreateAttachToShelfRollback(localCommandId, piece, piece.shelfOwner, false, 0, 0f);
				}
				BuilderAction builderAction6 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderPiece rootPiece = piece.GetRootPiece();
				BuilderAction builderAction7 = BuilderActions.CreateMakeRoot(localCommandId, rootPiece.pieceId);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(builderAction5);
					this.AddRollbackAction(builderAction7);
					this.AddRollbackAction(builderAction2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(builderAction6);
				this.ExecuteAction(builderAction);
				return;
			}
			if (flag2)
			{
				BuilderAction builderAction8 = BuilderActions.CreateMakeRoot(localCommandId, pieceId);
				BuilderPiece rootPiece2 = piece.GetRootPiece();
				BuilderAction builderAction9 = BuilderActions.CreateDropPieceRollback(localCommandId, rootPiece2, actorNumber);
				BuilderAction builderAction10 = BuilderActions.CreateMakeRoot(localCommandId, rootPiece2.pieceId);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(builderAction9);
					this.AddRollbackAction(builderAction10);
					this.AddRollbackAction(builderAction2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(builderAction8);
				this.ExecuteAction(builderAction);
				return;
			}
			if (piece.parentPiece != null)
			{
				BuilderAction builderAction11 = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, actorNumber);
				BuilderAction builderAction12 = BuilderActions.CreateAttachToPieceRollback(localCommandId, piece, actorNumber);
				if (cmd.canRollback)
				{
					this.AddRollbackAction(builderAction12);
					this.AddRollbackAction(builderAction2);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(builderAction11);
				this.ExecuteAction(builderAction);
			}
		}

		// Token: 0x060043F3 RID: 17395 RVA: 0x0013D1C0 File Offset: 0x0013B3C0
		public bool ValidateDropPieceParams(int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer)
		{
			float num = 10000f;
			if ((in position).IsValid(in num) && (in rotation).IsValid())
			{
				float num2 = 10000f;
				if ((in velocity).IsValid(in num2))
				{
					float num3 = 10000f;
					if ((in angVelocity).IsValid(in num3))
					{
						BuilderPiece piece = this.GetPiece(pieceId);
						if (piece == null)
						{
							return false;
						}
						if (piece.isBuiltIntoTable)
						{
							return false;
						}
						if (droppedByPlayer == null)
						{
							return false;
						}
						if (velocity.sqrMagnitude > BuilderTable.MAX_DROP_VELOCITY * BuilderTable.MAX_DROP_VELOCITY)
						{
							return false;
						}
						if (angVelocity.sqrMagnitude > BuilderTable.MAX_DROP_ANG_VELOCITY * BuilderTable.MAX_DROP_ANG_VELOCITY)
						{
							return false;
						}
						if ((this.roomCenter.position - position).sqrMagnitude > this.acceptableSqrDistFromCenter)
						{
							return false;
						}
						if (piece.state == BuilderPiece.State.AttachedToArm)
						{
							if (piece.parentPiece == null)
							{
								return false;
							}
							if (piece.parentPiece.heldByPlayerActorNumber != droppedByPlayer.ActorNumber)
							{
								return false;
							}
						}
						else if (piece.heldByPlayerActorNumber != droppedByPlayer.ActorNumber)
						{
							return false;
						}
						return !PhotonNetwork.IsMasterClient || this.IsPlayerHandNearAction(droppedByPlayer, position, piece.heldInLeftHand, false, 2.5f);
					}
				}
			}
			return false;
		}

		// Token: 0x060043F4 RID: 17396 RVA: 0x0013D2E0 File Offset: 0x0013B4E0
		public bool ValidateDropPieceState(int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, Player droppedByPlayer)
		{
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return false;
			}
			bool flag = piece.state == BuilderPiece.State.AttachedToArm;
			return (flag || piece.heldByPlayerActorNumber == droppedByPlayer.ActorNumber) && (!flag || piece.parentPiece.heldByPlayerActorNumber == droppedByPlayer.ActorNumber);
		}

		// Token: 0x060043F5 RID: 17397 RVA: 0x0013D336 File Offset: 0x0013B536
		public void RequestDropPiece(BuilderPiece piece, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity)
		{
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				return;
			}
			this.builderNetworking.RequestDropPiece(piece, position, rotation, velocity, angVelocity);
		}

		// Token: 0x060043F6 RID: 17398 RVA: 0x0013D354 File Offset: 0x0013B554
		public void DropPiece(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer, bool force)
		{
			this.PieceDroppedInternal(localCommandId, pieceId, position, rotation, velocity, angVelocity, droppedByPlayer, force);
		}

		// Token: 0x060043F7 RID: 17399 RVA: 0x0013D374 File Offset: 0x0013B574
		public void PieceDroppedInternal(int localCommandId, int pieceId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, NetPlayer droppedByPlayer, bool force)
		{
			if (!force && droppedByPlayer == NetworkSystem.Instance.LocalPlayer && this.HasRollForwardCommand(localCommandId) && this.TryRollbackAndReExecute(localCommandId))
			{
				return;
			}
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Drop,
				pieceId = pieceId,
				parentPieceId = pieceId,
				localPosition = position,
				localRotation = rotation,
				velocity = velocity,
				angVelocity = angVelocity,
				player = droppedByPlayer,
				canRollback = force,
				localCommandId = localCommandId
			};
			this.RouteNewCommand(builderCommand, force);
		}

		// Token: 0x060043F8 RID: 17400 RVA: 0x0013D410 File Offset: 0x0013B610
		public void ExecutePieceDroppedWithActions(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			if ((cmd.player == null || !cmd.player.IsLocal) && !this.ValidateDropPieceParams(pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, cmd.player))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null)
			{
				return;
			}
			if (piece.state == BuilderPiece.State.AttachedToArm)
			{
				BuilderPiece parentPiece = piece.parentPiece;
				BuilderAction builderAction = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, actorNumber);
				BuilderAction builderAction2 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, actorNumber);
				if (cmd.canRollback)
				{
					BuilderAction builderAction3 = BuilderActions.CreateAttachToPieceRollback(localCommandId, piece, actorNumber);
					this.AddRollbackAction(builderAction3);
					this.AddRollForwardCommand(cmd);
				}
				this.ExecuteAction(builderAction);
				this.ExecuteAction(builderAction2);
				return;
			}
			BuilderAction builderAction4 = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, actorNumber);
			BuilderAction builderAction5 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, cmd.velocity, cmd.angVelocity, actorNumber);
			if (cmd.canRollback)
			{
				BuilderAction builderAction6 = BuilderActions.CreateAttachToPlayerRollback(localCommandId, piece);
				this.AddRollbackAction(builderAction6);
				this.AddRollForwardCommand(cmd);
			}
			this.ExecuteAction(builderAction4);
			this.ExecuteAction(builderAction5);
		}

		// Token: 0x060043F9 RID: 17401 RVA: 0x0013D554 File Offset: 0x0013B754
		public void ExecutePieceRepelled(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int localCommandId = cmd.localCommandId;
			int actorNumber = cmd.player.ActorNumber;
			int attachPieceId = cmd.attachPieceId;
			BuilderPiece piece = this.GetPiece(pieceId);
			Vector3 vector = cmd.velocity;
			if (piece == null)
			{
				return;
			}
			if (piece.isBuiltIntoTable || piece.isArmShelf)
			{
				return;
			}
			if (piece.state != BuilderPiece.State.Grabbed && piece.state != BuilderPiece.State.GrabbedLocal && piece.state != BuilderPiece.State.Dropped && piece.state != BuilderPiece.State.AttachedToDropped && piece.state != BuilderPiece.State.AttachedToArm)
			{
				return;
			}
			if (attachPieceId >= 0 && attachPieceId < this.dropZones.Count)
			{
				BuilderDropZone builderDropZone = this.dropZones[attachPieceId];
				builderDropZone.PlayEffect();
				if (builderDropZone.overrideDirection)
				{
					vector = builderDropZone.GetRepelDirectionWorld() * BuilderTable.DROP_ZONE_REPEL;
				}
			}
			if (piece.heldByPlayerActorNumber >= 0)
			{
				BuilderAction builderAction = BuilderActions.CreateDetachFromPlayer(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				BuilderAction builderAction2 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, vector, cmd.angVelocity, actorNumber);
				this.ExecuteAction(builderAction);
				this.ExecuteAction(builderAction2);
				return;
			}
			if (piece.state == BuilderPiece.State.AttachedToArm && piece.parentPiece != null)
			{
				BuilderAction builderAction3 = BuilderActions.CreateDetachFromPiece(localCommandId, pieceId, piece.heldByPlayerActorNumber);
				BuilderAction builderAction4 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, vector, cmd.angVelocity, actorNumber);
				this.ExecuteAction(builderAction3);
				this.ExecuteAction(builderAction4);
				return;
			}
			BuilderAction builderAction5 = BuilderActions.CreateDropPiece(localCommandId, pieceId, cmd.localPosition, cmd.localRotation, vector, cmd.angVelocity, actorNumber);
			this.ExecuteAction(builderAction5);
		}

		// Token: 0x060043FA RID: 17402 RVA: 0x0013D6F4 File Offset: 0x0013B8F4
		private void CleanUpDroppedPiece()
		{
			if (!PhotonNetwork.IsMasterClient || this.droppedPieces.Count <= BuilderTable.DROPPED_PIECE_LIMIT)
			{
				return;
			}
			BuilderPiece builderPiece = this.FindFirstSleepingPiece();
			if (builderPiece != null && builderPiece.state == BuilderPiece.State.Dropped)
			{
				this.RequestRecyclePiece(builderPiece, false, -1);
				return;
			}
			Debug.LogErrorFormat("Piece {0} in Dropped List is {1}", new object[] { builderPiece.pieceId, builderPiece.state });
		}

		// Token: 0x060043FB RID: 17403 RVA: 0x0013D76C File Offset: 0x0013B96C
		public void AddPieceToDropList(BuilderPiece piece)
		{
			this.droppedPieces.Add(piece);
			this.droppedPieceData.Add(new BuilderTable.DroppedPieceData
			{
				speedThreshCrossedTime = 0f,
				droppedState = BuilderTable.DroppedPieceState.Light,
				filteredSpeed = 0f
			});
		}

		// Token: 0x060043FC RID: 17404 RVA: 0x0013D7BC File Offset: 0x0013B9BC
		private BuilderPiece FindFirstSleepingPiece()
		{
			if (this.droppedPieces.Count < 1)
			{
				return null;
			}
			BuilderPiece builderPiece = this.droppedPieces[0];
			for (int i = 0; i < this.droppedPieces.Count; i++)
			{
				if (this.droppedPieces[i].rigidBody != null && this.droppedPieces[i].rigidBody.IsSleeping())
				{
					BuilderPiece builderPiece2 = this.droppedPieces[i];
					this.droppedPieces.RemoveAt(i);
					this.droppedPieceData.RemoveAt(i);
					return builderPiece2;
				}
			}
			BuilderPiece builderPiece3 = this.droppedPieces[0];
			this.droppedPieces.RemoveAt(0);
			this.droppedPieceData.RemoveAt(0);
			return builderPiece3;
		}

		// Token: 0x060043FD RID: 17405 RVA: 0x0013D876 File Offset: 0x0013BA76
		public void RemovePieceFromDropList(BuilderPiece piece)
		{
			if (piece.state == BuilderPiece.State.Dropped)
			{
				this.droppedPieces.Remove(piece);
			}
		}

		// Token: 0x060043FE RID: 17406 RVA: 0x0013D890 File Offset: 0x0013BA90
		private void UpdateDroppedPieces(float dt)
		{
			for (int i = 0; i < this.droppedPieces.Count; i++)
			{
				Rigidbody rigidBody = this.droppedPieces[i].rigidBody;
				if (rigidBody != null)
				{
					BuilderTable.DroppedPieceData droppedPieceData = this.droppedPieceData[i];
					float magnitude = rigidBody.velocity.magnitude;
					droppedPieceData.filteredSpeed = droppedPieceData.filteredSpeed * 0.95f + magnitude * 0.05f;
					BuilderTable.DroppedPieceState droppedState = droppedPieceData.droppedState;
					if (droppedState != BuilderTable.DroppedPieceState.Light)
					{
						if (droppedState == BuilderTable.DroppedPieceState.Heavy)
						{
							droppedPieceData.speedThreshCrossedTime += dt;
							droppedPieceData.speedThreshCrossedTime = ((droppedPieceData.filteredSpeed > 0.075f) ? (droppedPieceData.speedThreshCrossedTime + dt) : 0f);
							if (droppedPieceData.speedThreshCrossedTime > 0.5f)
							{
								rigidBody.mass = 1f;
								droppedPieceData.droppedState = BuilderTable.DroppedPieceState.Light;
								droppedPieceData.speedThreshCrossedTime = 0f;
							}
						}
					}
					else
					{
						droppedPieceData.speedThreshCrossedTime = ((droppedPieceData.filteredSpeed < 0.05f) ? (droppedPieceData.speedThreshCrossedTime + dt) : 0f);
						if (droppedPieceData.speedThreshCrossedTime > 0f)
						{
							rigidBody.mass = 10000f;
							droppedPieceData.droppedState = BuilderTable.DroppedPieceState.Heavy;
							droppedPieceData.speedThreshCrossedTime = 0f;
						}
					}
					this.droppedPieceData[i] = droppedPieceData;
				}
			}
		}

		// Token: 0x060043FF RID: 17407 RVA: 0x0013D9ED File Offset: 0x0013BBED
		private void SetLocalPlayerOwnsPlot(bool ownsPlot)
		{
			this.doesLocalPlayerOwnPlot = ownsPlot;
			UnityEvent<bool> onLocalPlayerClaimedPlot = this.OnLocalPlayerClaimedPlot;
			if (onLocalPlayerClaimedPlot == null)
			{
				return;
			}
			onLocalPlayerClaimedPlot.Invoke(this.doesLocalPlayerOwnPlot);
		}

		// Token: 0x06004400 RID: 17408 RVA: 0x0013DA0C File Offset: 0x0013BC0C
		public void PlotClaimed(int plotPieceId, Player claimingPlayer)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.ClaimPlot,
				pieceId = plotPieceId,
				player = NetPlayer.Get(claimingPlayer)
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x06004401 RID: 17409 RVA: 0x0013DA48 File Offset: 0x0013BC48
		public void ExecuteClaimPlot(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			NetPlayer player = cmd.player;
			if (pieceId == -1)
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece == null || !piece.IsPrivatePlot())
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (this.plotOwners.TryAdd(player.ActorNumber, pieceId) && piece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				builderPiecePrivatePlot.ClaimPlotForPlayerNumber(player.ActorNumber);
				if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					this.SetLocalPlayerOwnsPlot(true);
				}
			}
		}

		// Token: 0x06004402 RID: 17410 RVA: 0x0013DACC File Offset: 0x0013BCCC
		public void PlayerLeftRoom(int playerActorNumber)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.PlayerLeftRoom,
				pieceId = playerActorNumber,
				player = null
			};
			bool flag = this.tableState == BuilderTable.TableState.WaitForMasterResync;
			this.RouteNewCommand(builderCommand, flag);
		}

		// Token: 0x06004403 RID: 17411 RVA: 0x0013DB10 File Offset: 0x0013BD10
		public void ExecutePlayerLeftRoom(BuilderTable.BuilderCommand cmd)
		{
			NetPlayer player = cmd.player;
			int num = ((player != null) ? player.ActorNumber : cmd.pieceId);
			this.FreePlotInternal(-1, num);
			int num2;
			if (this.playerToArmShelfLeft.TryGetValue(num, out num2))
			{
				this.RecyclePieceInternal(num2, true, false, -1);
			}
			this.playerToArmShelfLeft.Remove(num);
			int num3;
			if (this.playerToArmShelfRight.TryGetValue(num, out num3))
			{
				this.RecyclePieceInternal(num3, true, false, -1);
			}
			this.playerToArmShelfRight.Remove(num);
		}

		// Token: 0x06004404 RID: 17412 RVA: 0x0013DB8C File Offset: 0x0013BD8C
		public void PlotFreed(int plotPieceId, Player claimingPlayer)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.FreePlot,
				pieceId = plotPieceId,
				player = NetPlayer.Get(claimingPlayer)
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x06004405 RID: 17413 RVA: 0x0013DBC8 File Offset: 0x0013BDC8
		public void ExecuteFreePlot(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			NetPlayer player = cmd.player;
			if (player == null)
			{
				return;
			}
			this.FreePlotInternal(pieceId, player.ActorNumber);
		}

		// Token: 0x06004406 RID: 17414 RVA: 0x0013DBF4 File Offset: 0x0013BDF4
		private void FreePlotInternal(int plotPieceId, int requestingPlayer)
		{
			if (plotPieceId == -1 && !this.plotOwners.TryGetValue(requestingPlayer, out plotPieceId))
			{
				return;
			}
			BuilderPiece piece = this.GetPiece(plotPieceId);
			if (piece == null || !piece.IsPrivatePlot())
			{
				return;
			}
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (piece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				int ownerActorNumber = builderPiecePrivatePlot.GetOwnerActorNumber();
				this.plotOwners.Remove(ownerActorNumber);
				builderPiecePrivatePlot.FreePlot();
				if (ownerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					this.SetLocalPlayerOwnsPlot(false);
				}
			}
		}

		// Token: 0x06004407 RID: 17415 RVA: 0x0013DC68 File Offset: 0x0013BE68
		public bool DoesPlayerOwnPlot(int actorNum)
		{
			return this.plotOwners.ContainsKey(actorNum);
		}

		// Token: 0x06004408 RID: 17416 RVA: 0x0013DC76 File Offset: 0x0013BE76
		public void RequestPaintPiece(int pieceId, int materialType)
		{
			this.builderNetworking.RequestPaintPiece(pieceId, materialType);
		}

		// Token: 0x06004409 RID: 17417 RVA: 0x0013DC85 File Offset: 0x0013BE85
		public void PaintPiece(int pieceId, int materialType, Player paintingPlayer, bool force)
		{
			this.PaintPieceInternal(pieceId, materialType, paintingPlayer, force);
		}

		// Token: 0x0600440A RID: 17418 RVA: 0x0013DC94 File Offset: 0x0013BE94
		private void PaintPieceInternal(int pieceId, int materialType, Player paintingPlayer, bool force)
		{
			if (!force && paintingPlayer == PhotonNetwork.LocalPlayer)
			{
				return;
			}
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Paint,
				pieceId = pieceId,
				materialType = materialType,
				player = NetPlayer.Get(paintingPlayer)
			};
			this.RouteNewCommand(builderCommand, force);
		}

		// Token: 0x0600440B RID: 17419 RVA: 0x0013DCE8 File Offset: 0x0013BEE8
		public void ExecutePiecePainted(BuilderTable.BuilderCommand cmd)
		{
			int pieceId = cmd.pieceId;
			int materialType = cmd.materialType;
			BuilderPiece piece = this.GetPiece(pieceId);
			if (piece != null && !piece.isBuiltIntoTable)
			{
				piece.SetMaterial(materialType, false);
			}
		}

		// Token: 0x0600440C RID: 17420 RVA: 0x0013DD24 File Offset: 0x0013BF24
		public void CreateArmShelvesForPlayersInBuilder()
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
			{
				foreach (Player player in this.builderNetworking.armShelfRequests)
				{
					if (player != null)
					{
						this.builderNetworking.RequestCreateArmShelfForPlayer(player);
					}
				}
				this.builderNetworking.armShelfRequests.Clear();
			}
		}

		// Token: 0x0600440D RID: 17421 RVA: 0x0013DDAC File Offset: 0x0013BFAC
		public void RemoveArmShelfForPlayer(Player player)
		{
			if (!this.isTableMutable)
			{
				return;
			}
			if (player == null)
			{
				return;
			}
			if (this.tableState != BuilderTable.TableState.Ready)
			{
				this.builderNetworking.armShelfRequests.Remove(player);
				return;
			}
			int num;
			if (this.playerToArmShelfLeft.TryGetValue(player.ActorNumber, out num))
			{
				BuilderPiece piece = this.GetPiece(num);
				this.playerToArmShelfLeft.Remove(player.ActorNumber);
				if (piece.armShelf != null)
				{
					piece.armShelf.piece = null;
					piece.armShelf = null;
				}
				if (PhotonNetwork.IsMasterClient)
				{
					this.builderNetworking.RequestRecyclePiece(num, piece.transform.position, piece.transform.rotation, false, -1);
				}
				else
				{
					this.DropPieceForPlayerLeavingInternal(piece, player.ActorNumber);
				}
			}
			int num2;
			if (this.playerToArmShelfRight.TryGetValue(player.ActorNumber, out num2))
			{
				BuilderPiece piece2 = this.GetPiece(num2);
				this.playerToArmShelfRight.Remove(player.ActorNumber);
				if (piece2.armShelf != null)
				{
					piece2.armShelf.piece = null;
					piece2.armShelf = null;
				}
				if (PhotonNetwork.IsMasterClient)
				{
					this.builderNetworking.RequestRecyclePiece(num2, piece2.transform.position, piece2.transform.rotation, false, -1);
					return;
				}
				this.DropPieceForPlayerLeavingInternal(piece2, player.ActorNumber);
			}
		}

		// Token: 0x0600440E RID: 17422 RVA: 0x0013DEF8 File Offset: 0x0013C0F8
		public void DropAllPiecesForPlayerLeaving(int playerActorNumber)
		{
			List<BuilderPiece> list = this.pieces;
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				BuilderPiece builderPiece = list[i];
				if (builderPiece != null && builderPiece.heldByPlayerActorNumber == playerActorNumber && (builderPiece.state == BuilderPiece.State.Grabbed || builderPiece.state == BuilderPiece.State.GrabbedLocal))
				{
					this.DropPieceForPlayerLeavingInternal(builderPiece, playerActorNumber);
				}
			}
		}

		// Token: 0x0600440F RID: 17423 RVA: 0x0013DF58 File Offset: 0x0013C158
		public void RecycleAllPiecesForPlayerLeaving(int playerActorNumber)
		{
			List<BuilderPiece> list = this.pieces;
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				BuilderPiece builderPiece = list[i];
				if (builderPiece != null && builderPiece.heldByPlayerActorNumber == playerActorNumber && (builderPiece.state == BuilderPiece.State.Grabbed || builderPiece.state == BuilderPiece.State.GrabbedLocal))
				{
					this.RecyclePieceForPlayerLeavingInternal(builderPiece, playerActorNumber);
				}
			}
		}

		// Token: 0x06004410 RID: 17424 RVA: 0x0013DFB8 File Offset: 0x0013C1B8
		private void DropPieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			BuilderAction builderAction = BuilderActions.CreateDetachFromPlayer(-1, piece.pieceId, playerActorNumber);
			BuilderAction builderAction2 = BuilderActions.CreateDropPiece(-1, piece.pieceId, piece.transform.position, piece.transform.rotation, Vector3.zero, Vector3.zero, playerActorNumber);
			this.ExecuteAction(builderAction);
			this.ExecuteAction(builderAction2);
		}

		// Token: 0x06004411 RID: 17425 RVA: 0x0013E00F File Offset: 0x0013C20F
		private void RecyclePieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			this.builderNetworking.RequestRecyclePiece(piece.pieceId, piece.transform.position, piece.transform.rotation, false, -1);
		}

		// Token: 0x06004412 RID: 17426 RVA: 0x0013E03C File Offset: 0x0013C23C
		private void DetachPieceForPlayerLeavingInternal(BuilderPiece piece, int playerActorNumber)
		{
			BuilderAction builderAction = BuilderActions.CreateDetachFromPiece(-1, piece.pieceId, playerActorNumber);
			BuilderAction builderAction2 = BuilderActions.CreateDropPiece(-1, piece.pieceId, piece.transform.position, piece.transform.rotation, Vector3.zero, Vector3.zero, playerActorNumber);
			this.ExecuteAction(builderAction);
			this.ExecuteAction(builderAction2);
		}

		// Token: 0x06004413 RID: 17427 RVA: 0x0013E094 File Offset: 0x0013C294
		public void CreateArmShelf(int pieceIdLeft, int pieceIdRight, int pieceType, Player player)
		{
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.CreateArmShelf,
				pieceId = pieceIdLeft,
				pieceType = pieceType,
				player = NetPlayer.Get(player),
				isLeft = true
			};
			this.RouteNewCommand(builderCommand, false);
			BuilderTable.BuilderCommand builderCommand2 = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.CreateArmShelf,
				pieceId = pieceIdRight,
				pieceType = pieceType,
				player = NetPlayer.Get(player),
				isLeft = false
			};
			this.RouteNewCommand(builderCommand2, false);
		}

		// Token: 0x06004414 RID: 17428 RVA: 0x0013E124 File Offset: 0x0013C324
		public void ExecuteArmShelfCreated(BuilderTable.BuilderCommand cmd)
		{
			NetPlayer player = cmd.player;
			if (player == null)
			{
				return;
			}
			bool isLeft = cmd.isLeft;
			if (this.GetPiece(cmd.pieceId) != null)
			{
				return;
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
			{
				BuilderArmShelf builderArmShelf = (isLeft ? rigContainer.Rig.builderArmShelfLeft : rigContainer.Rig.builderArmShelfRight);
				if (builderArmShelf != null)
				{
					if (builderArmShelf.piece != null)
					{
						if (builderArmShelf.piece.isArmShelf && builderArmShelf.piece.isActiveAndEnabled)
						{
							builderArmShelf.piece.armShelf = null;
							this.RecyclePiece(builderArmShelf.piece.pieceId, builderArmShelf.piece.transform.position, builderArmShelf.piece.transform.rotation, false, -1, PhotonNetwork.LocalPlayer);
						}
						else
						{
							builderArmShelf.piece = null;
						}
						BuilderPiece builderPiece = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, builderArmShelf.pieceAnchor.position, builderArmShelf.pieceAnchor.rotation, BuilderPiece.State.AttachedToArm, -1, 0, this);
						builderArmShelf.piece = builderPiece;
						builderPiece.armShelf = builderArmShelf;
						builderPiece.SetParentHeld(builderArmShelf.pieceAnchor, cmd.player.ActorNumber, isLeft);
						builderPiece.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
						builderPiece.transform.localScale = Vector3.one;
						if (isLeft)
						{
							this.playerToArmShelfLeft.AddOrUpdate(player.ActorNumber, cmd.pieceId);
							return;
						}
						this.playerToArmShelfRight.AddOrUpdate(player.ActorNumber, cmd.pieceId);
						return;
					}
					else
					{
						BuilderPiece builderPiece2 = this.CreatePieceInternal(cmd.pieceType, cmd.pieceId, builderArmShelf.pieceAnchor.position, builderArmShelf.pieceAnchor.rotation, BuilderPiece.State.AttachedToArm, -1, 0, this);
						builderArmShelf.piece = builderPiece2;
						builderPiece2.armShelf = builderArmShelf;
						builderPiece2.SetParentHeld(builderArmShelf.pieceAnchor, cmd.player.ActorNumber, isLeft);
						builderPiece2.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
						builderPiece2.transform.localScale = Vector3.one;
						if (isLeft)
						{
							this.playerToArmShelfLeft.TryAdd(player.ActorNumber, cmd.pieceId);
							return;
						}
						this.playerToArmShelfRight.TryAdd(player.ActorNumber, cmd.pieceId);
					}
				}
			}
		}

		// Token: 0x06004415 RID: 17429 RVA: 0x0013E370 File Offset: 0x0013C570
		public void ClearLocalArmShelf()
		{
			VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
			if (offlineVRRig != null)
			{
				BuilderArmShelf builderArmShelf = offlineVRRig.builderArmShelfLeft;
				if (builderArmShelf != null)
				{
					BuilderPiece piece = builderArmShelf.piece;
					builderArmShelf.piece = null;
					if (piece != null)
					{
						piece.transform.SetParent(null);
					}
				}
				builderArmShelf = offlineVRRig.builderArmShelfRight;
				if (builderArmShelf != null)
				{
					BuilderPiece piece2 = builderArmShelf.piece;
					builderArmShelf.piece = null;
					if (piece2 != null)
					{
						piece2.transform.SetParent(null);
					}
				}
			}
		}

		// Token: 0x06004416 RID: 17430 RVA: 0x0013E3F8 File Offset: 0x0013C5F8
		public void PieceEnteredDropZone(int pieceId, Vector3 worldPos, Quaternion worldRot, int dropZoneId)
		{
			Vector3 vector = (this.roomCenter.position - worldPos).normalized * BuilderTable.DROP_ZONE_REPEL;
			BuilderTable.BuilderCommand builderCommand = new BuilderTable.BuilderCommand
			{
				type = BuilderTable.BuilderCommandType.Repel,
				pieceId = pieceId,
				parentPieceId = pieceId,
				attachPieceId = dropZoneId,
				localPosition = worldPos,
				localRotation = worldRot,
				velocity = vector,
				angVelocity = Vector3.zero,
				player = NetworkSystem.Instance.MasterClient,
				canRollback = false
			};
			this.RouteNewCommand(builderCommand, false);
		}

		// Token: 0x06004417 RID: 17431 RVA: 0x0013E49C File Offset: 0x0013C69C
		public bool ValidateRepelPiece(BuilderPiece piece)
		{
			if (!this.isSetup)
			{
				return false;
			}
			if (piece.isBuiltIntoTable || piece.isArmShelf)
			{
				return false;
			}
			if (piece.state == BuilderPiece.State.Grabbed || piece.state == BuilderPiece.State.GrabbedLocal || piece.state == BuilderPiece.State.Dropped || piece.state == BuilderPiece.State.AttachedToDropped || piece.state == BuilderPiece.State.AttachedToArm)
			{
				bool flag = false;
				for (int i = 0; i < this.repelHistoryLength; i++)
				{
					flag = flag || this.repelledPieceRoots[i].Contains(piece.pieceId);
					if (flag)
					{
						return false;
					}
				}
				this.repelledPieceRoots[this.repelHistoryIndex].Add(piece.pieceId);
				return true;
			}
			return false;
		}

		// Token: 0x06004418 RID: 17432 RVA: 0x0013E540 File Offset: 0x0013C740
		public void RepelPieceTowardTable(int pieceID)
		{
			BuilderPiece piece = this.GetPiece(pieceID);
			if (piece == null)
			{
				return;
			}
			Vector3 position = piece.transform.position;
			Quaternion rotation = piece.transform.rotation;
			if (position.y < this.tableCenter.position.y)
			{
				position.y = this.tableCenter.position.y;
			}
			Vector3 vector = (this.tableCenter.position - position).normalized * BuilderTable.DROP_ZONE_REPEL;
			if (piece.IsHeldLocal())
			{
				BuilderPieceInteractor.instance.RemovePieceFromHeld(piece);
			}
			piece.ClearParentHeld();
			piece.ClearParentPiece(false);
			piece.transform.localScale = Vector3.one;
			piece.SetState(BuilderPiece.State.Dropped, false);
			piece.transform.SetLocalPositionAndRotation(position, rotation);
			if (piece.rigidBody != null)
			{
				piece.rigidBody.position = position;
				piece.rigidBody.rotation = rotation;
				piece.rigidBody.velocity = vector;
				piece.rigidBody.AddForce(Vector3.up * (BuilderTable.DROP_ZONE_REPEL / 2f), ForceMode.VelocityChange);
				piece.rigidBody.angularVelocity = Vector3.zero;
			}
		}

		// Token: 0x06004419 RID: 17433 RVA: 0x0013E678 File Offset: 0x0013C878
		public BuilderPiece GetPiece(int pieceId)
		{
			int num;
			if (this.pieceIDToIndexCache.TryGetValue(pieceId, out num))
			{
				if (num >= 0 && num < this.pieces.Count)
				{
					return this.pieces[num];
				}
				this.pieceIDToIndexCache.Remove(pieceId);
			}
			for (int i = 0; i < this.pieces.Count; i++)
			{
				if (this.pieces[i].pieceId == pieceId)
				{
					this.pieceIDToIndexCache.Add(pieceId, i);
					return this.pieces[i];
				}
			}
			for (int j = 0; j < this.basePieces.Count; j++)
			{
				if (this.basePieces[j].pieceId == pieceId)
				{
					return this.basePieces[j];
				}
			}
			return null;
		}

		// Token: 0x0600441A RID: 17434 RVA: 0x0013E73D File Offset: 0x0013C93D
		public void AddPiece(BuilderPiece piece)
		{
			this.pieces.Add(piece);
			this.UseResources(piece);
			this.AddPieceData(piece);
		}

		// Token: 0x0600441B RID: 17435 RVA: 0x0013E75A File Offset: 0x0013C95A
		public void RemovePiece(BuilderPiece piece)
		{
			this.pieces.Remove(piece);
			this.AddResources(piece);
			this.RemovePieceData(piece);
			this.pieceIDToIndexCache.Clear();
		}

		// Token: 0x0600441C RID: 17436 RVA: 0x000023F4 File Offset: 0x000005F4
		private void CreateData()
		{
		}

		// Token: 0x0600441D RID: 17437 RVA: 0x000023F4 File Offset: 0x000005F4
		private void DestroyData()
		{
		}

		// Token: 0x0600441E RID: 17438 RVA: 0x000BC497 File Offset: 0x000BA697
		private int AddPieceData(BuilderPiece piece)
		{
			return -1;
		}

		// Token: 0x0600441F RID: 17439 RVA: 0x000023F4 File Offset: 0x000005F4
		public void UpdatePieceData(BuilderPiece piece)
		{
		}

		// Token: 0x06004420 RID: 17440 RVA: 0x000023F4 File Offset: 0x000005F4
		private void RemovePieceData(BuilderPiece piece)
		{
		}

		// Token: 0x06004421 RID: 17441 RVA: 0x000BC497 File Offset: 0x000BA697
		private int AddGridPlaneData(BuilderAttachGridPlane gridPlane)
		{
			return -1;
		}

		// Token: 0x06004422 RID: 17442 RVA: 0x000023F4 File Offset: 0x000005F4
		private void RemoveGridPlaneData(BuilderAttachGridPlane gridPlane)
		{
		}

		// Token: 0x06004423 RID: 17443 RVA: 0x000BC497 File Offset: 0x000BA697
		private int AddPrivatePlotData(BuilderPiecePrivatePlot plot)
		{
			return -1;
		}

		// Token: 0x06004424 RID: 17444 RVA: 0x000023F4 File Offset: 0x000005F4
		private void RemovePrivatePlotData(BuilderPiecePrivatePlot plot)
		{
		}

		// Token: 0x06004425 RID: 17445 RVA: 0x0013E782 File Offset: 0x0013C982
		public void OnButtonFreeRotation(BuilderOptionButton button, bool isLeftHand)
		{
			this.useSnapRotation = !this.useSnapRotation;
			button.SetPressed(this.useSnapRotation);
		}

		// Token: 0x06004426 RID: 17446 RVA: 0x0013E79F File Offset: 0x0013C99F
		public void OnButtonFreePosition(BuilderOptionButton button, bool isLeftHand)
		{
			if (this.usePlacementStyle == BuilderPlacementStyle.Float)
			{
				this.usePlacementStyle = BuilderPlacementStyle.SnapDown;
			}
			else if (this.usePlacementStyle == BuilderPlacementStyle.SnapDown)
			{
				this.usePlacementStyle = BuilderPlacementStyle.Float;
			}
			button.SetPressed(this.usePlacementStyle > BuilderPlacementStyle.Float);
		}

		// Token: 0x06004427 RID: 17447 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnButtonSaveLayout(BuilderOptionButton button, bool isLeftHand)
		{
		}

		// Token: 0x06004428 RID: 17448 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnButtonClearLayout(BuilderOptionButton button, bool isLeftHand)
		{
		}

		// Token: 0x06004429 RID: 17449 RVA: 0x0013E7D4 File Offset: 0x0013C9D4
		public bool TryPlaceGridPlane(BuilderPiece piece, BuilderAttachGridPlane gridPlane, List<BuilderAttachGridPlane> checkGridPlanes, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			Vector3 position = gridPlane.transform.position;
			Quaternion rotation = gridPlane.transform.rotation;
			if (this.gridSize <= 0f)
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < checkGridPlanes.Count; i++)
			{
				BuilderAttachGridPlane builderAttachGridPlane = checkGridPlanes[i];
				this.TryPlaceGridPlaneOnGridPlane(piece, gridPlane, position, rotation, builderAttachGridPlane, ref potentialPlacement, ref flag);
			}
			return flag;
		}

		// Token: 0x0600442A RID: 17450 RVA: 0x0013E848 File Offset: 0x0013CA48
		public bool TryPlaceGridPlaneOnGridPlane(BuilderPiece piece, BuilderAttachGridPlane gridPlane, Vector3 gridPlanePos, Quaternion gridPlaneRot, BuilderAttachGridPlane checkGridPlane, ref BuilderPotentialPlacement potentialPlacement, ref bool success)
		{
			if (checkGridPlane.male == gridPlane.male)
			{
				return false;
			}
			if (checkGridPlane.piece == gridPlane.piece)
			{
				return false;
			}
			Transform center = checkGridPlane.center;
			Vector3 position = center.position;
			float sqrMagnitude = (position - gridPlanePos).sqrMagnitude;
			float num = checkGridPlane.boundingRadius + gridPlane.boundingRadius;
			if (sqrMagnitude > num * num)
			{
				return false;
			}
			Quaternion rotation = center.rotation;
			Quaternion quaternion = Quaternion.Inverse(rotation);
			Quaternion quaternion2 = quaternion * gridPlaneRot;
			if (Vector3.Dot(Vector3.up, quaternion2 * Vector3.up) < this.currSnapParams.maxUpDotProduct)
			{
				return false;
			}
			Vector3 vector = quaternion * (gridPlanePos - position);
			float y = vector.y;
			float num2 = -Mathf.Abs(y);
			if (success && num2 < potentialPlacement.score)
			{
				return false;
			}
			if (Mathf.Abs(y) > 1f)
			{
				return false;
			}
			if ((gridPlane.male && y > this.currSnapParams.minOffsetY) || (!gridPlane.male && y < -this.currSnapParams.minOffsetY))
			{
				return false;
			}
			if (Mathf.Abs(y) > this.currSnapParams.maxOffsetY)
			{
				return false;
			}
			Quaternion quaternion3;
			Quaternion quaternion4;
			global::BoingKit.QuaternionUtil.DecomposeSwingTwist(quaternion2, Vector3.up, out quaternion3, out quaternion4);
			float maxTwistDotProduct = this.currSnapParams.maxTwistDotProduct;
			Vector3 vector2 = quaternion4 * Vector3.forward;
			float num3 = Vector3.Dot(vector2, Vector3.forward);
			float num4 = Vector3.Dot(vector2, Vector3.right);
			bool flag = Mathf.Abs(num3) > maxTwistDotProduct;
			bool flag2 = Mathf.Abs(num4) > maxTwistDotProduct;
			if (!flag && !flag2)
			{
				return false;
			}
			float num5;
			uint num6;
			if (flag)
			{
				num5 = ((num3 > 0f) ? 0f : 180f);
				num6 = ((num3 > 0f) ? 0U : 2U);
			}
			else
			{
				num5 = ((num4 > 0f) ? 90f : 270f);
				num6 = ((num4 > 0f) ? 1U : 3U);
			}
			int num7 = (flag2 ? gridPlane.width : gridPlane.length);
			int num8 = (flag2 ? gridPlane.length : gridPlane.width);
			float num9 = ((num8 % 2 == 0) ? (this.gridSize / 2f) : 0f);
			float num10 = ((num7 % 2 == 0) ? (this.gridSize / 2f) : 0f);
			float num11 = ((checkGridPlane.width % 2 == 0) ? (this.gridSize / 2f) : 0f);
			float num12 = ((checkGridPlane.length % 2 == 0) ? (this.gridSize / 2f) : 0f);
			float num13 = num9 - num11;
			float num14 = num10 - num12;
			int num15 = Mathf.RoundToInt((vector.x - num13) / this.gridSize);
			int num16 = Mathf.RoundToInt((vector.z - num14) / this.gridSize);
			int num17 = num15 + Mathf.FloorToInt((float)num8 / 2f);
			int num18 = num16 + Mathf.FloorToInt((float)num7 / 2f);
			int num19 = num17 - (num8 - 1);
			int num20 = num18 - (num7 - 1);
			int num21 = Mathf.FloorToInt((float)checkGridPlane.width / 2f);
			int num22 = Mathf.FloorToInt((float)checkGridPlane.length / 2f);
			int num23 = num21 - (checkGridPlane.width - 1);
			int num24 = num22 - (checkGridPlane.length - 1);
			if (num19 > num21 || num17 < num23 || num20 > num22 || num18 < num24)
			{
				return false;
			}
			BuilderPiece rootPiece = checkGridPlane.piece.GetRootPiece();
			if (BuilderTable.ShareSameRoot(gridPlane.piece, rootPiece))
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, gridPlane.piece, rootPiece))
			{
				return false;
			}
			BuilderPiece piece2 = checkGridPlane.piece;
			if (piece2 != null)
			{
				if (piece2.preventSnapUntilMoved > 0)
				{
					return false;
				}
				if (piece2.requestedParentPiece != null && BuilderTable.ShareSameRoot(piece, piece2.requestedParentPiece))
				{
					return false;
				}
			}
			Quaternion quaternion5 = Quaternion.Euler(0f, num5, 0f);
			Quaternion quaternion6 = rotation * quaternion5;
			float num25 = (float)num15 * this.gridSize + num13;
			float num26 = (float)num16 * this.gridSize + num14;
			Vector3 vector3 = new Vector3(num25, 0f, num26);
			Vector3 vector4 = position + rotation * vector3;
			Transform center2 = gridPlane.center;
			Quaternion quaternion7 = quaternion6 * Quaternion.Inverse(center2.localRotation);
			Vector3 vector5 = piece.transform.InverseTransformPoint(center2.position);
			Vector3 vector6 = vector4 - quaternion7 * vector5;
			potentialPlacement.localPosition = vector6;
			potentialPlacement.localRotation = quaternion7;
			potentialPlacement.score = num2;
			success = true;
			potentialPlacement.parentPiece = piece2;
			potentialPlacement.parentAttachIndex = checkGridPlane.attachIndex;
			potentialPlacement.attachDistance = Mathf.Abs(y);
			potentialPlacement.attachPlaneNormal = Vector3.up;
			if (!checkGridPlane.male)
			{
				potentialPlacement.attachPlaneNormal *= -1f;
			}
			if (potentialPlacement.parentPiece != null)
			{
				BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
				potentialPlacement.localPosition = builderAttachGridPlane.transform.InverseTransformPoint(potentialPlacement.localPosition);
				potentialPlacement.localRotation = Quaternion.Inverse(builderAttachGridPlane.transform.rotation) * potentialPlacement.localRotation;
			}
			potentialPlacement.parentAttachBounds.min.x = Mathf.Max(num23, num19);
			potentialPlacement.parentAttachBounds.min.y = Mathf.Max(num24, num20);
			potentialPlacement.parentAttachBounds.max.x = Mathf.Min(num21, num17);
			potentialPlacement.parentAttachBounds.max.y = Mathf.Min(num22, num18);
			Vector2Int vector2Int = Vector2Int.zero;
			Vector2Int vector2Int2 = Vector2Int.zero;
			vector2Int.x = potentialPlacement.parentAttachBounds.min.x - num15;
			vector2Int2.x = potentialPlacement.parentAttachBounds.max.x - num15;
			vector2Int.y = potentialPlacement.parentAttachBounds.min.y - num16;
			vector2Int2.y = potentialPlacement.parentAttachBounds.max.y - num16;
			potentialPlacement.twist = (byte)num6;
			potentialPlacement.bumpOffsetX = (sbyte)num15;
			potentialPlacement.bumpOffsetZ = (sbyte)num16;
			int num27 = ((num8 % 2 == 0) ? 1 : 0);
			int num28 = ((num7 % 2 == 0) ? 1 : 0);
			if (flag && num3 < 0f)
			{
				vector2Int = this.Rotate180(vector2Int, num27, num28);
				vector2Int2 = this.Rotate180(vector2Int2, num27, num28);
			}
			else if (flag2 && num4 < 0f)
			{
				vector2Int = this.Rotate270(vector2Int, num27, num28);
				vector2Int2 = this.Rotate270(vector2Int2, num27, num28);
			}
			else if (flag2 && num4 > 0f)
			{
				vector2Int = this.Rotate90(vector2Int, num27, num28);
				vector2Int2 = this.Rotate90(vector2Int2, num27, num28);
			}
			potentialPlacement.attachBounds.min.x = Mathf.Min(vector2Int.x, vector2Int2.x);
			potentialPlacement.attachBounds.min.y = Mathf.Min(vector2Int.y, vector2Int2.y);
			potentialPlacement.attachBounds.max.x = Mathf.Max(vector2Int.x, vector2Int2.x);
			potentialPlacement.attachBounds.max.y = Mathf.Max(vector2Int.y, vector2Int2.y);
			return true;
		}

		// Token: 0x0600442B RID: 17451 RVA: 0x0013EFC2 File Offset: 0x0013D1C2
		private Vector2Int Rotate90(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.y * -1 + offsetY, v.x);
		}

		// Token: 0x0600442C RID: 17452 RVA: 0x0013EFDB File Offset: 0x0013D1DB
		private Vector2Int Rotate270(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.y, v.x * -1 + offsetX);
		}

		// Token: 0x0600442D RID: 17453 RVA: 0x0013EFF4 File Offset: 0x0013D1F4
		private Vector2Int Rotate180(Vector2Int v, int offsetX, int offsetY)
		{
			return new Vector2Int(v.x * -1 + offsetX, v.y * -1 + offsetY);
		}

		// Token: 0x0600442E RID: 17454 RVA: 0x0013F011 File Offset: 0x0013D211
		public bool ShareSameRoot(BuilderAttachGridPlane plane, BuilderAttachGridPlane otherPlane)
		{
			return !(plane == null) && !(otherPlane == null) && !(otherPlane.piece == null) && BuilderTable.ShareSameRoot(plane.piece, otherPlane.piece);
		}

		// Token: 0x0600442F RID: 17455 RVA: 0x0013F048 File Offset: 0x0013D248
		public static bool ShareSameRoot(BuilderPiece piece, BuilderPiece otherPiece)
		{
			if (otherPiece == null || piece == null)
			{
				return false;
			}
			if (piece == otherPiece)
			{
				return true;
			}
			BuilderPiece builderPiece = piece;
			int num = 2048;
			while (builderPiece.parentPiece != null && !builderPiece.parentPiece.isBuiltIntoTable)
			{
				builderPiece = builderPiece.parentPiece;
				num--;
				if (num <= 0)
				{
					return true;
				}
			}
			num = 2048;
			BuilderPiece builderPiece2 = otherPiece;
			while (builderPiece2.parentPiece != null && !builderPiece2.parentPiece.isBuiltIntoTable)
			{
				builderPiece2 = builderPiece2.parentPiece;
				num--;
				if (num <= 0)
				{
					return true;
				}
			}
			return builderPiece == builderPiece2;
		}

		// Token: 0x06004430 RID: 17456 RVA: 0x0013F0E8 File Offset: 0x0013D2E8
		public bool TryPlacePieceOnTableNoDrop(bool leftHand, BuilderPiece testPiece, List<BuilderAttachGridPlane> checkGridPlanesMale, List<BuilderAttachGridPlane> checkGridPlanesFemale, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			if (this == null)
			{
				return false;
			}
			if (testPiece == null)
			{
				return false;
			}
			this.currSnapParams = this.pushAndEaseParams;
			return this.TryPlacePieceGridPlanesOnTableInternal(testPiece, this.maxPlacementChildDepth, checkGridPlanesMale, checkGridPlanesFemale, out potentialPlacement);
		}

		// Token: 0x06004431 RID: 17457 RVA: 0x0013F138 File Offset: 0x0013D338
		public bool TryPlacePieceOnTableNoDropJobs(NativeList<BuilderGridPlaneData> gridPlaneData, NativeList<BuilderPieceData> pieceData, NativeList<BuilderGridPlaneData> checkGridPlaneData, NativeList<BuilderPieceData> checkPieceData, out BuilderPotentialPlacement potentialPlacement, List<BuilderPotentialPlacement> allPlacements)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			if (this == null)
			{
				return false;
			}
			this.currSnapParams = this.pushAndEaseParams;
			NativeQueue<BuilderPotentialPlacementData> nativeQueue = new NativeQueue<BuilderPotentialPlacementData>(Allocator.TempJob);
			new BuilderFindPotentialSnaps
			{
				gridSize = this.gridSize,
				currSnapParams = this.currSnapParams,
				gridPlanes = gridPlaneData,
				checkGridPlanes = checkGridPlaneData,
				worldToLocalPos = Vector3.zero,
				worldToLocalRot = Quaternion.identity,
				localToWorldPos = Vector3.zero,
				localToWorldRot = Quaternion.identity,
				potentialPlacements = nativeQueue.AsParallelWriter()
			}.Schedule(gridPlaneData.Length, 32, default(JobHandle)).Complete();
			BuilderPotentialPlacementData builderPotentialPlacementData = default(BuilderPotentialPlacementData);
			bool flag = false;
			while (!nativeQueue.IsEmpty())
			{
				BuilderPotentialPlacementData builderPotentialPlacementData2 = nativeQueue.Dequeue();
				if (!flag || builderPotentialPlacementData2.score > builderPotentialPlacementData.score)
				{
					builderPotentialPlacementData = builderPotentialPlacementData2;
					flag = true;
				}
			}
			if (flag)
			{
				potentialPlacement = builderPotentialPlacementData.ToPotentialPlacement(this);
			}
			if (flag)
			{
				nativeQueue.Clear();
				this.currSnapParams = this.overlapParams;
				Vector3 vector = -potentialPlacement.attachPiece.transform.position;
				Quaternion quaternion = Quaternion.Inverse(potentialPlacement.attachPiece.transform.rotation);
				BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
				Quaternion quaternion2 = builderAttachGridPlane.transform.rotation * potentialPlacement.localRotation;
				Vector3 vector2 = builderAttachGridPlane.transform.TransformPoint(potentialPlacement.localPosition);
				new BuilderFindPotentialSnaps
				{
					gridSize = this.gridSize,
					currSnapParams = this.currSnapParams,
					gridPlanes = gridPlaneData,
					checkGridPlanes = checkGridPlaneData,
					worldToLocalPos = vector,
					worldToLocalRot = quaternion,
					localToWorldPos = vector2,
					localToWorldRot = quaternion2,
					potentialPlacements = nativeQueue.AsParallelWriter()
				}.Schedule(gridPlaneData.Length, 32, default(JobHandle)).Complete();
				while (!nativeQueue.IsEmpty())
				{
					BuilderPotentialPlacementData builderPotentialPlacementData3 = nativeQueue.Dequeue();
					if (builderPotentialPlacementData3.attachDistance < this.currSnapParams.maxBlockSnapDist)
					{
						allPlacements.Add(builderPotentialPlacementData3.ToPotentialPlacement(this));
					}
				}
			}
			nativeQueue.Dispose();
			return flag;
		}

		// Token: 0x06004432 RID: 17458 RVA: 0x0013F3A4 File Offset: 0x0013D5A4
		public bool CalcAllPotentialPlacements(NativeList<BuilderGridPlaneData> gridPlaneData, NativeList<BuilderGridPlaneData> checkGridPlaneData, BuilderPotentialPlacement potentialPlacement, List<BuilderPotentialPlacement> allPlacements)
		{
			if (this == null)
			{
				return false;
			}
			bool flag = false;
			this.currSnapParams = this.overlapParams;
			NativeQueue<BuilderPotentialPlacementData> nativeQueue = new NativeQueue<BuilderPotentialPlacementData>(Allocator.TempJob);
			nativeQueue.Clear();
			Vector3 vector = -potentialPlacement.attachPiece.transform.position;
			Quaternion quaternion = Quaternion.Inverse(potentialPlacement.attachPiece.transform.rotation);
			BuilderAttachGridPlane builderAttachGridPlane = potentialPlacement.parentPiece.gridPlanes[potentialPlacement.parentAttachIndex];
			Quaternion quaternion2 = builderAttachGridPlane.transform.rotation * potentialPlacement.localRotation;
			Vector3 vector2 = builderAttachGridPlane.transform.TransformPoint(potentialPlacement.localPosition);
			new BuilderFindPotentialSnaps
			{
				gridSize = this.gridSize,
				currSnapParams = this.currSnapParams,
				gridPlanes = gridPlaneData,
				checkGridPlanes = checkGridPlaneData,
				worldToLocalPos = vector,
				worldToLocalRot = quaternion,
				localToWorldPos = vector2,
				localToWorldRot = quaternion2,
				potentialPlacements = nativeQueue.AsParallelWriter()
			}.Schedule(gridPlaneData.Length, 32, default(JobHandle)).Complete();
			while (!nativeQueue.IsEmpty())
			{
				BuilderPotentialPlacementData builderPotentialPlacementData = nativeQueue.Dequeue();
				if (builderPotentialPlacementData.attachDistance < this.currSnapParams.maxBlockSnapDist)
				{
					allPlacements.Add(builderPotentialPlacementData.ToPotentialPlacement(this));
				}
			}
			nativeQueue.Dispose();
			return flag;
		}

		// Token: 0x06004433 RID: 17459 RVA: 0x0013F510 File Offset: 0x0013D710
		public bool CanPiecesPotentiallySnap(BuilderPiece pieceInHand, BuilderPiece piece)
		{
			BuilderPiece rootPiece = piece.GetRootPiece();
			return !(rootPiece == pieceInHand) && BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, pieceInHand, rootPiece) && (!(piece.requestedParentPiece != null) || !BuilderTable.ShareSameRoot(pieceInHand, piece.requestedParentPiece)) && piece.preventSnapUntilMoved <= 0;
		}

		// Token: 0x06004434 RID: 17460 RVA: 0x0013F574 File Offset: 0x0013D774
		public bool CanPiecesPotentiallyOverlap(BuilderPiece pieceInHand, BuilderPiece rootWhenPlaced, BuilderPiece.State stateWhenPlaced, BuilderPiece otherPiece)
		{
			BuilderPiece rootPiece = otherPiece.GetRootPiece();
			if (rootPiece == pieceInHand)
			{
				return false;
			}
			if (!BuilderPiece.CanPlayerAttachPieceToPiece(PhotonNetwork.LocalPlayer.ActorNumber, pieceInHand, rootPiece))
			{
				return false;
			}
			if (otherPiece.requestedParentPiece != null && BuilderTable.ShareSameRoot(pieceInHand, otherPiece.requestedParentPiece))
			{
				return false;
			}
			if (otherPiece.preventSnapUntilMoved > 0)
			{
				return false;
			}
			BuilderPiece.State state = otherPiece.state;
			if (otherPiece.isBuiltIntoTable && !otherPiece.isArmShelf)
			{
				state = BuilderPiece.State.AttachedAndPlaced;
			}
			return BuilderTable.AreStatesCompatibleForOverlap(stateWhenPlaced, state, rootWhenPlaced, rootPiece);
		}

		// Token: 0x06004435 RID: 17461 RVA: 0x0013F5FD File Offset: 0x0013D7FD
		public void TryDropPiece(bool leftHand, BuilderPiece testPiece, Vector3 velocity, Vector3 angVelocity)
		{
			if (this == null)
			{
				return;
			}
			if (testPiece == null)
			{
				return;
			}
			this.RequestDropPiece(testPiece, testPiece.transform.position, testPiece.transform.rotation, velocity, angVelocity);
		}

		// Token: 0x06004436 RID: 17462 RVA: 0x0013F634 File Offset: 0x0013D834
		public bool TryPlacePieceGridPlanesOnTableInternal(BuilderPiece testPiece, int recurse, List<BuilderAttachGridPlane> checkGridPlanesMale, List<BuilderAttachGridPlane> checkGridPlanesFemale, out BuilderPotentialPlacement potentialPlacement)
		{
			potentialPlacement = default(BuilderPotentialPlacement);
			potentialPlacement.Reset();
			bool flag = false;
			bool flag2 = false;
			if (testPiece != null && testPiece.gridPlanes != null && testPiece.gridPlanes.Count > 0 && testPiece.gridPlanes != null)
			{
				for (int i = 0; i < testPiece.gridPlanes.Count; i++)
				{
					List<BuilderAttachGridPlane> list = (testPiece.gridPlanes[i].male ? checkGridPlanesFemale : checkGridPlanesMale);
					BuilderPotentialPlacement builderPotentialPlacement;
					if (this.TryPlaceGridPlane(testPiece, testPiece.gridPlanes[i], list, out builderPotentialPlacement))
					{
						if (builderPotentialPlacement.attachDistance < this.currSnapParams.snapAttachDistance * 1.1f)
						{
							flag2 = true;
						}
						if (builderPotentialPlacement.score > potentialPlacement.score && testPiece.preventSnapUntilMoved <= 0)
						{
							potentialPlacement = builderPotentialPlacement;
							potentialPlacement.attachIndex = i;
							potentialPlacement.attachPiece = testPiece;
							flag = true;
						}
					}
				}
			}
			if (recurse > 0)
			{
				BuilderPiece builderPiece = testPiece.firstChildPiece;
				while (builderPiece != null)
				{
					BuilderPotentialPlacement builderPotentialPlacement2;
					if (this.TryPlacePieceGridPlanesOnTableInternal(builderPiece, recurse - 1, checkGridPlanesMale, checkGridPlanesFemale, out builderPotentialPlacement2))
					{
						if (builderPotentialPlacement2.attachDistance < this.currSnapParams.snapAttachDistance * 1.1f)
						{
							flag2 = true;
						}
						if (builderPotentialPlacement2.score > potentialPlacement.score && testPiece.preventSnapUntilMoved <= 0)
						{
							potentialPlacement = builderPotentialPlacement2;
							flag = true;
						}
					}
					builderPiece = builderPiece.nextSiblingPiece;
				}
			}
			if (testPiece.preventSnapUntilMoved > 0 && !flag2)
			{
				testPiece.preventSnapUntilMoved--;
				this.UpdatePieceData(testPiece);
			}
			return flag;
		}

		// Token: 0x06004437 RID: 17463 RVA: 0x0013F7B8 File Offset: 0x0013D9B8
		public void TryPlaceRandomlyOnTable(BuilderPiece piece)
		{
			BuilderAttachGridPlane builderAttachGridPlane = piece.gridPlanes[Random.Range(0, piece.gridPlanes.Count)];
			List<BuilderAttachGridPlane> list = this.baseGridPlanes;
			int num = Random.Range(0, list.Count);
			int i = 0;
			while (i < list.Count)
			{
				int num2 = (i + num) % list.Count;
				BuilderAttachGridPlane builderAttachGridPlane2 = list[num2];
				if (builderAttachGridPlane2.male != builderAttachGridPlane.male && !(builderAttachGridPlane2.piece == builderAttachGridPlane.piece) && !this.ShareSameRoot(builderAttachGridPlane, builderAttachGridPlane2))
				{
					Vector3 zero = Vector3.zero;
					Quaternion identity = Quaternion.identity;
					BuilderPiece piece2 = builderAttachGridPlane2.piece;
					int attachIndex = builderAttachGridPlane2.attachIndex;
					Transform center = builderAttachGridPlane.center;
					Quaternion quaternion = builderAttachGridPlane2.transform.rotation * Quaternion.Inverse(center.localRotation);
					Vector3 vector = piece.transform.InverseTransformPoint(center.position);
					Vector3 vector2 = builderAttachGridPlane2.transform.position - quaternion * vector;
					if (piece2 != null)
					{
						BuilderAttachGridPlane builderAttachGridPlane3 = piece2.gridPlanes[attachIndex];
						Vector3 lossyScale = builderAttachGridPlane3.transform.lossyScale;
						Vector3 vector3 = new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
						Quaternion.Inverse(builderAttachGridPlane3.transform.rotation) * Vector3.Scale(vector2 - builderAttachGridPlane3.transform.position, vector3);
						Quaternion.Inverse(builderAttachGridPlane3.transform.rotation) * quaternion;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x06004438 RID: 17464 RVA: 0x0013F974 File Offset: 0x0013DB74
		public void UseResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				this.UseResource(cost.quantities[i]);
			}
		}

		// Token: 0x06004439 RID: 17465 RVA: 0x0013F9BA File Offset: 0x0013DBBA
		private void UseResource(BuilderResourceQuantity quantity)
		{
			if (quantity.type < BuilderResourceType.Basic || quantity.type >= BuilderResourceType.Count)
			{
				return;
			}
			this.usedResources[(int)quantity.type] += quantity.count;
			if (this.tableState == BuilderTable.TableState.Ready)
			{
				this.OnAvailableResourcesChange();
			}
		}

		// Token: 0x0600443A RID: 17466 RVA: 0x0013F9FC File Offset: 0x0013DBFC
		public void AddResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				this.AddResource(cost.quantities[i]);
			}
		}

		// Token: 0x0600443B RID: 17467 RVA: 0x0013FA42 File Offset: 0x0013DC42
		private void AddResource(BuilderResourceQuantity quantity)
		{
			if (quantity.type < BuilderResourceType.Basic || quantity.type >= BuilderResourceType.Count)
			{
				return;
			}
			this.usedResources[(int)quantity.type] -= quantity.count;
			if (this.tableState == BuilderTable.TableState.Ready)
			{
				this.OnAvailableResourcesChange();
			}
		}

		// Token: 0x0600443C RID: 17468 RVA: 0x0013FA84 File Offset: 0x0013DC84
		public bool HasEnoughUnreservedResources(BuilderResources resources)
		{
			if (resources == null)
			{
				return false;
			}
			for (int i = 0; i < resources.quantities.Count; i++)
			{
				if (!this.HasEnoughUnreservedResource(resources.quantities[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600443D RID: 17469 RVA: 0x0013FACC File Offset: 0x0013DCCC
		public bool HasEnoughUnreservedResource(BuilderResourceQuantity quantity)
		{
			return quantity.type >= BuilderResourceType.Basic && quantity.type < BuilderResourceType.Count && this.usedResources[(int)quantity.type] + this.reservedResources[(int)quantity.type] + quantity.count <= this.maxResources[(int)quantity.type];
		}

		// Token: 0x0600443E RID: 17470 RVA: 0x0013FB24 File Offset: 0x0013DD24
		public bool HasEnoughResources(BuilderPiece piece)
		{
			BuilderResources cost = piece.cost;
			if (cost == null)
			{
				return false;
			}
			for (int i = 0; i < cost.quantities.Count; i++)
			{
				if (!this.HasEnoughResource(cost.quantities[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600443F RID: 17471 RVA: 0x0013FB70 File Offset: 0x0013DD70
		public bool HasEnoughResource(BuilderResourceQuantity quantity)
		{
			return quantity.type >= BuilderResourceType.Basic && quantity.type < BuilderResourceType.Count && this.usedResources[(int)quantity.type] + quantity.count <= this.maxResources[(int)quantity.type];
		}

		// Token: 0x06004440 RID: 17472 RVA: 0x0013FBAC File Offset: 0x0013DDAC
		public int GetAvailableResources(BuilderResourceType type)
		{
			if (type < BuilderResourceType.Basic || type >= BuilderResourceType.Count)
			{
				return 0;
			}
			return this.maxResources[(int)type] - this.usedResources[(int)type];
		}

		// Token: 0x06004441 RID: 17473 RVA: 0x0013FBCC File Offset: 0x0013DDCC
		private void OnAvailableResourcesChange()
		{
			for (int i = 0; i < this.factories.Count; i++)
			{
				this.factories[i].OnAvailableResourcesChange();
			}
			if (this.isSetup && this.isTableMutable)
			{
				for (int j = 0; j < this.conveyors.Count; j++)
				{
					this.conveyors[j].OnAvailableResourcesChange();
				}
				foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
				{
					builderResourceMeter.OnAvailableResourcesChange();
				}
			}
		}

		// Token: 0x06004442 RID: 17474 RVA: 0x0013FC7C File Offset: 0x0013DE7C
		public int GetPrivateResourceLimitForType(int type)
		{
			if (this.plotMaxResources == null)
			{
				return 0;
			}
			return this.plotMaxResources[type];
		}

		// Token: 0x06004443 RID: 17475 RVA: 0x0013FC90 File Offset: 0x0013DE90
		private void WriteVector3(BinaryWriter writer, Vector3 data)
		{
			writer.Write(data.x);
			writer.Write(data.y);
			writer.Write(data.z);
		}

		// Token: 0x06004444 RID: 17476 RVA: 0x0013FCB6 File Offset: 0x0013DEB6
		private void WriteQuaternion(BinaryWriter writer, Quaternion data)
		{
			writer.Write(data.x);
			writer.Write(data.y);
			writer.Write(data.z);
			writer.Write(data.w);
		}

		// Token: 0x06004445 RID: 17477 RVA: 0x0013FCE8 File Offset: 0x0013DEE8
		private Vector3 ReadVector3(BinaryReader reader)
		{
			Vector3 vector;
			vector.x = reader.ReadSingle();
			vector.y = reader.ReadSingle();
			vector.z = reader.ReadSingle();
			return vector;
		}

		// Token: 0x06004446 RID: 17478 RVA: 0x0013FD20 File Offset: 0x0013DF20
		private Quaternion ReadQuaternion(BinaryReader reader)
		{
			Quaternion quaternion;
			quaternion.x = reader.ReadSingle();
			quaternion.y = reader.ReadSingle();
			quaternion.z = reader.ReadSingle();
			quaternion.w = reader.ReadSingle();
			return quaternion;
		}

		// Token: 0x06004447 RID: 17479 RVA: 0x0013FD64 File Offset: 0x0013DF64
		public static int PackPiecePlacement(byte twist, sbyte xOffset, sbyte zOffset)
		{
			int num = (int)(twist & 3);
			int num2 = (int)xOffset + 128;
			int num3 = (int)zOffset + 128;
			return num2 + (num3 << 8) + (num << 16);
		}

		// Token: 0x06004448 RID: 17480 RVA: 0x0013FD90 File Offset: 0x0013DF90
		public static void UnpackPiecePlacement(int packed, out byte twist, out sbyte xOffset, out sbyte zOffset)
		{
			int num = packed & 255;
			int num2 = (packed >> 8) & 255;
			int num3 = (packed >> 16) & 3;
			twist = (byte)num3;
			xOffset = (sbyte)(num - 128);
			zOffset = (sbyte)(num2 - 128);
		}

		// Token: 0x06004449 RID: 17481 RVA: 0x0013FDD0 File Offset: 0x0013DFD0
		private long PackSnapInfo(int attachGridIndex, int otherAttachGridIndex, Vector2Int min, Vector2Int max)
		{
			long num = (long)Mathf.Clamp(attachGridIndex, 0, 31);
			long num2 = (long)Mathf.Clamp(otherAttachGridIndex, 0, 31);
			long num3 = (long)Mathf.Clamp(min.x + 1024, 0, 2047);
			long num4 = (long)Mathf.Clamp(min.y + 1024, 0, 2047);
			long num5 = (long)Mathf.Clamp(max.x + 1024, 0, 2047);
			long num6 = (long)Mathf.Clamp(max.y + 1024, 0, 2047);
			return num + (num2 << 5) + (num3 << 10) + (num4 << 21) + (num5 << 32) + (num6 << 43);
		}

		// Token: 0x0600444A RID: 17482 RVA: 0x0013FE74 File Offset: 0x0013E074
		private void UnpackSnapInfo(long packed, out int attachGridIndex, out int otherAttachGridIndex, out Vector2Int min, out Vector2Int max)
		{
			long num = packed & 31L;
			attachGridIndex = (int)num;
			num = (packed >> 5) & 31L;
			otherAttachGridIndex = (int)num;
			int num2 = (int)((packed >> 10) & 2047L) - 1024;
			int num3 = (int)((packed >> 21) & 2047L) - 1024;
			min = new Vector2Int(num2, num3);
			int num4 = (int)((packed >> 32) & 2047L) - 1024;
			int num5 = (int)((packed >> 43) & 2047L) - 1024;
			max = new Vector2Int(num4, num5);
		}

		// Token: 0x0600444B RID: 17483 RVA: 0x0013FF01 File Offset: 0x0013E101
		private void RequestTableConfiguration()
		{
			SharedBlocksManager.instance.OnGetTableConfiguration += this.OnGetTableConfiguration;
			SharedBlocksManager.instance.RequestTableConfiguration();
		}

		// Token: 0x0600444C RID: 17484 RVA: 0x0013FF23 File Offset: 0x0013E123
		private void OnGetTableConfiguration(string configString)
		{
			SharedBlocksManager.instance.OnGetTableConfiguration -= this.OnGetTableConfiguration;
			if (!configString.IsNullOrEmpty())
			{
				this.ParseTableConfiguration(configString);
			}
		}

		// Token: 0x0600444D RID: 17485 RVA: 0x0013FF4C File Offset: 0x0013E14C
		private void ParseTableConfiguration(string dataRecord)
		{
			if (string.IsNullOrEmpty(dataRecord))
			{
				return;
			}
			BuilderTableConfiguration builderTableConfiguration = JsonUtility.FromJson<BuilderTableConfiguration>(dataRecord);
			if (builderTableConfiguration != null)
			{
				if (builderTableConfiguration.TableResourceLimits != null)
				{
					for (int i = 0; i < builderTableConfiguration.TableResourceLimits.Length; i++)
					{
						int num = builderTableConfiguration.TableResourceLimits[i];
						if (num >= 0)
						{
							this.maxResources[i] = num;
						}
					}
				}
				if (builderTableConfiguration.PlotResourceLimits != null)
				{
					for (int j = 0; j < builderTableConfiguration.PlotResourceLimits.Length; j++)
					{
						int num2 = builderTableConfiguration.PlotResourceLimits[j];
						if (num2 >= 0)
						{
							this.plotMaxResources[j] = num2;
						}
					}
				}
				int droppedPieceLimit = builderTableConfiguration.DroppedPieceLimit;
				if (droppedPieceLimit >= 0)
				{
					BuilderTable.DROPPED_PIECE_LIMIT = droppedPieceLimit;
				}
				if (builderTableConfiguration.updateCountdownDate != null && !string.IsNullOrEmpty(builderTableConfiguration.updateCountdownDate))
				{
					try
					{
						DateTime.Parse(builderTableConfiguration.updateCountdownDate, CultureInfo.InvariantCulture);
						BuilderTable.nextUpdateOverride = builderTableConfiguration.updateCountdownDate;
						goto IL_00DC;
					}
					catch
					{
						BuilderTable.nextUpdateOverride = string.Empty;
						goto IL_00DC;
					}
				}
				BuilderTable.nextUpdateOverride = string.Empty;
				IL_00DC:
				this.OnAvailableResourcesChange();
				UnityEvent onTableConfigurationUpdated = this.OnTableConfigurationUpdated;
				if (onTableConfigurationUpdated == null)
				{
					return;
				}
				onTableConfigurationUpdated.Invoke();
			}
		}

		// Token: 0x0600444E RID: 17486 RVA: 0x0014005C File Offset: 0x0013E25C
		private void DumpTableConfig()
		{
			BuilderTableConfiguration builderTableConfiguration = new BuilderTableConfiguration();
			Array.Clear(builderTableConfiguration.TableResourceLimits, 0, builderTableConfiguration.TableResourceLimits.Length);
			Array.Clear(builderTableConfiguration.PlotResourceLimits, 0, builderTableConfiguration.PlotResourceLimits.Length);
			foreach (BuilderResourceQuantity builderResourceQuantity in this.totalResources.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < (BuilderResourceType)builderTableConfiguration.TableResourceLimits.Length)
				{
					builderTableConfiguration.TableResourceLimits[(int)builderResourceQuantity.type] = builderResourceQuantity.count;
				}
			}
			foreach (BuilderResourceQuantity builderResourceQuantity2 in this.resourcesPerPrivatePlot.quantities)
			{
				if (builderResourceQuantity2.type >= BuilderResourceType.Basic && builderResourceQuantity2.type < (BuilderResourceType)builderTableConfiguration.PlotResourceLimits.Length)
				{
					builderTableConfiguration.PlotResourceLimits[(int)builderResourceQuantity2.type] = builderResourceQuantity2.count;
				}
			}
			builderTableConfiguration.DroppedPieceLimit = BuilderTable.DROPPED_PIECE_LIMIT;
			builderTableConfiguration.updateCountdownDate = "1/10/2025 16:00:00";
			string text = JsonUtility.ToJson(builderTableConfiguration);
			Debug.Log("Configuration Dump \n" + text);
		}

		// Token: 0x0600444F RID: 17487 RVA: 0x001401A8 File Offset: 0x0013E3A8
		private string GetSaveDataTimeKey(int slot)
		{
			return BuilderTable.personalBuildKey + slot.ToString("D2") + "Time";
		}

		// Token: 0x06004450 RID: 17488 RVA: 0x001401C5 File Offset: 0x0013E3C5
		private string GetSaveDataKey(int slot)
		{
			return BuilderTable.personalBuildKey + slot.ToString("D2");
		}

		// Token: 0x06004451 RID: 17489 RVA: 0x001401DD File Offset: 0x0013E3DD
		public void FindAndLoadSharedBlocksMap(string mapID)
		{
			SharedBlocksManager.instance.RequestMapDataFromID(mapID, new SharedBlocksManager.BlocksMapRequestCallback(this.FoundSharedBlocksMap));
		}

		// Token: 0x06004452 RID: 17490 RVA: 0x001401F6 File Offset: 0x0013E3F6
		public string GetSharedBlocksMapID()
		{
			if (this.sharedBlocksMap != null)
			{
				return this.sharedBlocksMap.MapID;
			}
			return string.Empty;
		}

		// Token: 0x06004453 RID: 17491 RVA: 0x00140214 File Offset: 0x0013E414
		private void FoundSharedBlocksMap(SharedBlocksManager.SharedBlocksMap map)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			if (map == null || map.MapData.IsNullOrEmpty())
			{
				this.builderNetworking.LoadSharedBlocksFailedMaster((map == null) ? string.Empty : map.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				return;
			}
			this.sharedBlocksMap = map;
			this.SetTableState(BuilderTable.TableState.WaitForInitialBuildMaster);
		}

		// Token: 0x06004454 RID: 17492 RVA: 0x00140290 File Offset: 0x0013E490
		private void BuildInitialTableForPlayer()
		{
			if (NetworkSystem.Instance.IsNull() || !NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.SessionIsPrivate || NetworkSystem.Instance.GetLocalPlayer() == null || !NetworkSystem.Instance.IsMasterClient)
			{
				this.TryBuildingFromTitleData();
				return;
			}
			if (this.currentSaveSlot < 0 || this.currentSaveSlot >= BuilderScanKiosk.NUM_SAVE_SLOTS)
			{
				this.TryBuildingFromTitleData();
				return;
			}
			SharedBlocksManager.instance.OnFetchPrivateScanComplete += this.OnFetchPrivateScanComplete;
			SharedBlocksManager.instance.RequestFetchPrivateScan(this.currentSaveSlot);
		}

		// Token: 0x06004455 RID: 17493 RVA: 0x00140324 File Offset: 0x0013E524
		private void OnFetchPrivateScanComplete(int slot, bool success)
		{
			SharedBlocksManager.instance.OnFetchPrivateScanComplete -= this.OnFetchPrivateScanComplete;
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			string text;
			if (!success || !SharedBlocksManager.instance.TryGetPrivateScanResponse(slot, out text))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			if (!this.BuildTableFromJson(text, false))
			{
				this.TryBuildingFromTitleData();
				return;
			}
			this.SetIsDirty(false);
			this.OnFinishedInitialTableBuild();
		}

		// Token: 0x06004456 RID: 17494 RVA: 0x00140388 File Offset: 0x0013E588
		private void BuildSelectedSharedMap()
		{
			if (!NetworkSystem.Instance.IsNull() && NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient)
			{
				if (this.sharedBlocksMap != null && !this.sharedBlocksMap.MapData.IsNullOrEmpty())
				{
					this.TryBuildingSharedBlocksMap(this.sharedBlocksMap.MapData);
					return;
				}
				if (SharedBlocksManager.IsMapIDValid(this.pendingMapID))
				{
					SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
					{
						MapID = this.pendingMapID
					};
					this.LoadSharedMap(sharedBlocksMap);
					return;
				}
				SharedBlocksManager.instance.OnFoundDefaultSharedBlocksMap += this.FoundDefaultSharedBlocksMap;
				SharedBlocksManager.instance.RequestDefaultSharedMap();
			}
		}

		// Token: 0x06004457 RID: 17495 RVA: 0x00140430 File Offset: 0x0013E630
		private void FoundDefaultSharedBlocksMap(bool success, SharedBlocksManager.SharedBlocksMap map)
		{
			SharedBlocksManager.instance.OnFoundDefaultSharedBlocksMap -= this.FoundDefaultSharedBlocksMap;
			if (success && !map.MapData.IsNullOrEmpty())
			{
				this.sharedBlocksMap = map;
				this.TryBuildingSharedBlocksMap(this.sharedBlocksMap.MapData);
				return;
			}
			this.TryBuildingFromTitleData();
		}

		// Token: 0x06004458 RID: 17496 RVA: 0x00140484 File Offset: 0x0013E684
		private void TryBuildingSharedBlocksMap(string mapData)
		{
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			if (!this.BuildTableFromJson(mapData, true))
			{
				GTDev.LogWarning<string>("Unable to build shared blocks map", null);
				this.builderNetworking.LoadSharedBlocksFailedMaster(this.sharedBlocksMap.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				return;
			}
			base.StartCoroutine(this.CheckForNoBlocks());
		}

		// Token: 0x06004459 RID: 17497 RVA: 0x001404F9 File Offset: 0x0013E6F9
		private IEnumerator CheckForNoBlocks()
		{
			yield return null;
			if (!this.NoBlocksCheck())
			{
				GTDev.LogError<string>("Failed No Blocks Check", null);
				this.builderNetworking.SharedBlocksOutOfBoundsMaster(this.sharedBlocksMap.MapID);
				this.sharedBlocksMap = null;
				this.tableData = new BuilderTableData();
				this.ClearTable();
				this.ClearQueuedCommands();
				this.SetTableState(BuilderTable.TableState.Ready);
				yield break;
			}
			this.OnFinishedInitialTableBuild();
			yield break;
		}

		// Token: 0x0600445A RID: 17498 RVA: 0x00140508 File Offset: 0x0013E708
		private void TryBuildingFromTitleData()
		{
			SharedBlocksManager.instance.OnGetTitleDataBuildComplete += this.OnGetTitleDataBuildComplete;
			SharedBlocksManager.instance.FetchTitleDataBuild();
		}

		// Token: 0x0600445B RID: 17499 RVA: 0x0014052C File Offset: 0x0013E72C
		private void OnGetTitleDataBuildComplete(string titleDataBuild)
		{
			SharedBlocksManager.instance.OnGetTitleDataBuildComplete -= this.OnGetTitleDataBuildComplete;
			if (this.tableState != BuilderTable.TableState.WaitForInitialBuildMaster)
			{
				return;
			}
			if (!titleDataBuild.IsNullOrEmpty())
			{
				if (!this.BuildTableFromJson(titleDataBuild, true))
				{
					this.tableData = new BuilderTableData();
				}
			}
			else
			{
				this.tableData = new BuilderTableData();
			}
			this.OnFinishedInitialTableBuild();
		}

		// Token: 0x0600445C RID: 17500 RVA: 0x0014058C File Offset: 0x0013E78C
		public void SaveTableForPlayer()
		{
			if (SharedBlocksManager.instance.IsWaitingOnRequest())
			{
				this.SetIsDirty(true);
				UnityEvent<string> onSaveFailure = this.OnSaveFailure;
				if (onSaveFailure == null)
				{
					return;
				}
				onSaveFailure.Invoke("Busy");
				return;
			}
			else
			{
				this.saveInProgress = true;
				if (this.currentSaveSlot < 0 || this.currentSaveSlot >= BuilderScanKiosk.NUM_SAVE_SLOTS)
				{
					this.saveInProgress = false;
					return;
				}
				if (!this.isDirty)
				{
					this.saveInProgress = false;
					UnityEvent onSaveTimeUpdated = this.OnSaveTimeUpdated;
					if (onSaveTimeUpdated == null)
					{
						return;
					}
					onSaveTimeUpdated.Invoke();
					return;
				}
				else
				{
					if (this.NoBlocksCheck())
					{
						if (this.tableData == null)
						{
							this.tableData = new BuilderTableData();
						}
						this.SetIsDirty(false);
						this.tableData.numEdits++;
						string text = this.WriteTableToJson();
						text = Convert.ToBase64String(GZipStream.CompressString(text));
						SharedBlocksManager.instance.OnSavePrivateScanSuccess += this.OnSaveScanSuccess;
						SharedBlocksManager.instance.OnSavePrivateScanFailed += this.OnSaveScanFailure;
						SharedBlocksManager.instance.RequestSavePrivateScan(this.currentSaveSlot, text);
						return;
					}
					this.saveInProgress = false;
					this.SetIsDirty(true);
					UnityEvent<string> onSaveFailure2 = this.OnSaveFailure;
					if (onSaveFailure2 == null)
					{
						return;
					}
					onSaveFailure2.Invoke("PLEASE REMOVE BLOCKS CONNECTED OUTSIDE OF TABLE PLATFORM");
					return;
				}
			}
		}

		// Token: 0x0600445D RID: 17501 RVA: 0x001406B0 File Offset: 0x0013E8B0
		private void OnSaveScanSuccess(int scan)
		{
			SharedBlocksManager.instance.OnSavePrivateScanSuccess -= this.OnSaveScanSuccess;
			SharedBlocksManager.instance.OnSavePrivateScanFailed -= this.OnSaveScanFailure;
			this.saveInProgress = false;
			UnityEvent onSaveSuccess = this.OnSaveSuccess;
			if (onSaveSuccess == null)
			{
				return;
			}
			onSaveSuccess.Invoke();
		}

		// Token: 0x0600445E RID: 17502 RVA: 0x00140700 File Offset: 0x0013E900
		private void OnSaveScanFailure(int scan, string message)
		{
			SharedBlocksManager.instance.OnSavePrivateScanSuccess -= this.OnSaveScanSuccess;
			SharedBlocksManager.instance.OnSavePrivateScanFailed -= this.OnSaveScanFailure;
			this.saveInProgress = false;
			this.SetIsDirty(true);
			UnityEvent<string> onSaveFailure = this.OnSaveFailure;
			if (onSaveFailure == null)
			{
				return;
			}
			onSaveFailure.Invoke(message);
		}

		// Token: 0x0600445F RID: 17503 RVA: 0x00140758 File Offset: 0x0013E958
		private string WriteTableToJson()
		{
			this.tableData.Clear();
			BuilderTable.tempDuplicateOverlaps.Clear();
			for (int i = 0; i < this.pieces.Count; i++)
			{
				if (this.pieces[i].state == BuilderPiece.State.AttachedAndPlaced)
				{
					this.tableData.pieceType.Add(this.pieces[i].overrideSavedPiece ? this.pieces[i].savedPieceType : this.pieces[i].pieceType);
					this.tableData.pieceId.Add(this.pieces[i].pieceId);
					this.tableData.parentId.Add((this.pieces[i].parentPiece == null) ? (-1) : this.pieces[i].parentPiece.pieceId);
					this.tableData.attachIndex.Add(this.pieces[i].attachIndex);
					this.tableData.parentAttachIndex.Add((this.pieces[i].parentPiece == null) ? (-1) : this.pieces[i].parentAttachIndex);
					this.tableData.placement.Add(this.pieces[i].GetPiecePlacement());
					this.tableData.materialType.Add(this.pieces[i].overrideSavedPiece ? this.pieces[i].savedMaterialType : this.pieces[i].materialType);
					BuilderMovingSnapPiece component = this.pieces[i].GetComponent<BuilderMovingSnapPiece>();
					int num = ((component == null) ? 0 : component.GetTimeOffset());
					this.tableData.timeOffset.Add(num);
					for (int j = 0; j < this.pieces[i].gridPlanes.Count; j++)
					{
						if (!(this.pieces[i].gridPlanes[j] == null))
						{
							for (SnapOverlap snapOverlap = this.pieces[i].gridPlanes[j].firstOverlap; snapOverlap != null; snapOverlap = snapOverlap.nextOverlap)
							{
								if (snapOverlap.otherPlane.piece.state == BuilderPiece.State.AttachedAndPlaced || snapOverlap.otherPlane.piece.isBuiltIntoTable)
								{
									BuilderTable.SnapOverlapKey snapOverlapKey = BuilderTable.BuildOverlapKey(this.pieces[i].pieceId, snapOverlap.otherPlane.piece.pieceId, j, snapOverlap.otherPlane.attachIndex);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey))
									{
										BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey);
										long num2 = this.PackSnapInfo(j, snapOverlap.otherPlane.attachIndex, snapOverlap.bounds.min, snapOverlap.bounds.max);
										this.tableData.overlapingPieces.Add(this.pieces[i].pieceId);
										this.tableData.overlappedPieces.Add(snapOverlap.otherPlane.piece.pieceId);
										this.tableData.overlapInfo.Add(num2);
									}
								}
							}
						}
					}
				}
			}
			foreach (BuilderPiece builderPiece in this.basePieces)
			{
				if (!(builderPiece == null))
				{
					for (int k = 0; k < builderPiece.gridPlanes.Count; k++)
					{
						if (!(builderPiece.gridPlanes[k] == null))
						{
							for (SnapOverlap snapOverlap2 = builderPiece.gridPlanes[k].firstOverlap; snapOverlap2 != null; snapOverlap2 = snapOverlap2.nextOverlap)
							{
								if (snapOverlap2.otherPlane.piece.state == BuilderPiece.State.AttachedAndPlaced || snapOverlap2.otherPlane.piece.isBuiltIntoTable)
								{
									BuilderTable.SnapOverlapKey snapOverlapKey2 = BuilderTable.BuildOverlapKey(builderPiece.pieceId, snapOverlap2.otherPlane.piece.pieceId, k, snapOverlap2.otherPlane.attachIndex);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey2))
									{
										BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey2);
										long num3 = this.PackSnapInfo(k, snapOverlap2.otherPlane.attachIndex, snapOverlap2.bounds.min, snapOverlap2.bounds.max);
										this.tableData.overlapingPieces.Add(builderPiece.pieceId);
										this.tableData.overlappedPieces.Add(snapOverlap2.otherPlane.piece.pieceId);
										this.tableData.overlapInfo.Add(num3);
									}
								}
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			this.tableData.numPieces = this.tableData.pieceType.Count;
			return JsonUtility.ToJson(this.tableData);
		}

		// Token: 0x06004460 RID: 17504 RVA: 0x00140CAC File Offset: 0x0013EEAC
		private static BuilderTable.SnapOverlapKey BuildOverlapKey(int pieceId, int otherPieceId, int attachGridIndex, int otherAttachGridIndex)
		{
			BuilderTable.SnapOverlapKey snapOverlapKey = default(BuilderTable.SnapOverlapKey);
			snapOverlapKey.piece = (long)pieceId;
			snapOverlapKey.piece <<= 32;
			snapOverlapKey.piece |= (long)attachGridIndex;
			snapOverlapKey.otherPiece = (long)otherPieceId;
			snapOverlapKey.otherPiece <<= 32;
			snapOverlapKey.otherPiece |= (long)otherAttachGridIndex;
			return snapOverlapKey;
		}

		// Token: 0x06004461 RID: 17505 RVA: 0x00140D08 File Offset: 0x0013EF08
		private bool BuildTableFromJson(string tableJson, bool fromTitleData)
		{
			if (string.IsNullOrEmpty(tableJson))
			{
				return false;
			}
			this.tableData = null;
			try
			{
				this.tableData = JsonUtility.FromJson<BuilderTableData>(tableJson);
			}
			catch
			{
			}
			try
			{
				if (this.tableData == null)
				{
					tableJson = GZipStream.UncompressString(Convert.FromBase64String(tableJson));
					this.tableData = JsonUtility.FromJson<BuilderTableData>(tableJson);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				return false;
			}
			if (this.tableData == null)
			{
				return false;
			}
			if (this.tableData.version < 4)
			{
				return false;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>((this.tableData.pieceType == null) ? 0 : this.tableData.pieceType.Count);
			bool flag = this.tableData.timeOffset != null && this.tableData.timeOffset.Count > 0;
			int i = 0;
			while (i < this.tableData.pieceType.Count)
			{
				int num = this.CreatePieceId();
				dictionary.TryAdd(this.tableData.pieceId[i], num);
				int num2 = ((this.tableData.materialType != null && this.tableData.materialType.Count > i) ? this.tableData.materialType[i] : (-1));
				int num3 = this.tableData.pieceType[i];
				int num4 = num2;
				bool flag2 = true;
				if (fromTitleData)
				{
					goto IL_01F7;
				}
				BuilderPiece piecePrefab = this.GetPiecePrefab(this.tableData.pieceType[i]);
				if (piecePrefab == null)
				{
					this.ClearTable();
					return false;
				}
				if (num4 == -1 && piecePrefab.materialOptions != null)
				{
					int num5;
					Material material;
					int num6;
					piecePrefab.materialOptions.GetDefaultMaterial(out num5, out material, out num6);
					num4 = num5;
				}
				flag2 = BuilderSetManager.instance.IsPieceOwnedLocally(this.tableData.pieceType[i], num4);
				if (!fromTitleData && !flag2)
				{
					if (!piecePrefab.fallbackInfo.materialSwapThisPrefab)
					{
						if (piecePrefab.fallbackInfo.prefab == null)
						{
							goto IL_0267;
						}
						num3 = piecePrefab.fallbackInfo.prefab.name.GetStaticHash();
					}
					num4 = -1;
				}
				goto IL_01F7;
				IL_0267:
				i++;
				continue;
				IL_01F7:
				int num7 = (flag ? this.tableData.timeOffset[i] : 0);
				BuilderPiece builderPiece = this.CreatePieceInternal(num3, num, Vector3.zero, Quaternion.identity, BuilderPiece.State.AttachedAndPlaced, num4, NetworkSystem.Instance.ServerTimestamp - num7, this);
				if (!fromTitleData && !flag2)
				{
					builderPiece.overrideSavedPiece = true;
					builderPiece.savedPieceType = this.tableData.pieceType[i];
					builderPiece.savedMaterialType = num2;
				}
				goto IL_0267;
			}
			for (int j = 0; j < this.tableData.pieceType.Count; j++)
			{
				int num8 = ((this.tableData.parentAttachIndex == null || this.tableData.parentAttachIndex.Count <= j) ? (-1) : this.tableData.parentAttachIndex[j]);
				int num9 = ((this.tableData.attachIndex == null || this.tableData.attachIndex.Count <= j) ? (-1) : this.tableData.attachIndex[j]);
				int valueOrDefault = dictionary.GetValueOrDefault(this.tableData.pieceId[j], -1);
				int num10 = -1;
				int num11;
				if (dictionary.TryGetValue(this.tableData.parentId[j], out num11))
				{
					num10 = num11;
				}
				else if (this.tableData.parentId[j] < 10000 && this.tableData.parentId[j] >= 5)
				{
					num10 = this.tableData.parentId[j];
				}
				this.AttachPieceInternal(valueOrDefault, num9, num10, num8, this.tableData.placement[j]);
			}
			foreach (BuilderPiece builderPiece2 in this.pieces)
			{
				if (builderPiece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					builderPiece2.OnPlacementDeserialized();
				}
			}
			this.OnDeserializeUpdatePlots();
			BuilderTable.tempDuplicateOverlaps.Clear();
			if (this.tableData.overlapingPieces != null)
			{
				int num12 = 0;
				while (num12 < this.tableData.overlapingPieces.Count && num12 < this.tableData.overlappedPieces.Count && num12 < this.tableData.overlapInfo.Count)
				{
					int num13 = -1;
					int num14;
					if (dictionary.TryGetValue(this.tableData.overlapingPieces[num12], out num14))
					{
						num13 = num14;
					}
					else if (this.tableData.overlapingPieces[num12] < 10000 && this.tableData.overlapingPieces[num12] >= 5)
					{
						num13 = this.tableData.overlapingPieces[num12];
					}
					int num15 = -1;
					int num16;
					if (dictionary.TryGetValue(this.tableData.overlappedPieces[num12], out num16))
					{
						num15 = num16;
					}
					else if (this.tableData.overlappedPieces[num12] < 10000 && this.tableData.overlappedPieces[num12] >= 5)
					{
						num15 = this.tableData.overlappedPieces[num12];
					}
					if (num13 != -1 && num15 != -1)
					{
						long num17 = this.tableData.overlapInfo[num12];
						BuilderPiece piece = this.GetPiece(num13);
						if (!(piece == null))
						{
							BuilderPiece piece2 = this.GetPiece(num15);
							if (!(piece2 == null))
							{
								int num18;
								int num19;
								Vector2Int vector2Int;
								Vector2Int vector2Int2;
								this.UnpackSnapInfo(num17, out num18, out num19, out vector2Int, out vector2Int2);
								if (num18 >= 0 && num18 < piece.gridPlanes.Count && num19 >= 0 && num19 < piece2.gridPlanes.Count)
								{
									BuilderTable.SnapOverlapKey snapOverlapKey = BuilderTable.BuildOverlapKey(num13, num15, num18, num19);
									if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey))
									{
										BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey);
										piece.gridPlanes[num18].AddSnapOverlap(this.builderPool.CreateSnapOverlap(piece2.gridPlanes[num19], new SnapBounds(vector2Int, vector2Int2)));
									}
								}
							}
						}
					}
					num12++;
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			return true;
		}

		// Token: 0x06004462 RID: 17506 RVA: 0x00141374 File Offset: 0x0013F574
		public int SerializeTableState(byte[] bytes, int maxBytes)
		{
			MemoryStream memoryStream = new MemoryStream(bytes);
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			if (this.conveyors == null)
			{
				binaryWriter.Write(0);
			}
			else
			{
				binaryWriter.Write(this.conveyors.Count);
				foreach (BuilderConveyor builderConveyor in this.conveyors)
				{
					int selectedSetID = builderConveyor.GetSelectedSetID();
					binaryWriter.Write(selectedSetID);
				}
			}
			if (this.dispenserShelves == null)
			{
				binaryWriter.Write(0);
			}
			else
			{
				binaryWriter.Write(this.dispenserShelves.Count);
				foreach (BuilderDispenserShelf builderDispenserShelf in this.dispenserShelves)
				{
					int selectedSetID2 = builderDispenserShelf.GetSelectedSetID();
					binaryWriter.Write(selectedSetID2);
				}
			}
			BuilderTable.childPieces.Clear();
			BuilderTable.rootPieces.Clear();
			BuilderTable.childPieces.EnsureCapacity(this.pieces.Count);
			BuilderTable.rootPieces.EnsureCapacity(this.pieces.Count);
			foreach (BuilderPiece builderPiece in this.pieces)
			{
				if (builderPiece.parentPiece == null)
				{
					BuilderTable.rootPieces.Add(builderPiece);
				}
				else
				{
					BuilderTable.childPieces.Add(builderPiece);
				}
			}
			binaryWriter.Write(BuilderTable.rootPieces.Count);
			for (int i = 0; i < BuilderTable.rootPieces.Count; i++)
			{
				BuilderPiece builderPiece2 = BuilderTable.rootPieces[i];
				binaryWriter.Write(builderPiece2.pieceType);
				binaryWriter.Write(builderPiece2.pieceId);
				binaryWriter.Write((byte)builderPiece2.state);
				if (builderPiece2.state == BuilderPiece.State.OnConveyor || builderPiece2.state == BuilderPiece.State.OnShelf || builderPiece2.state == BuilderPiece.State.Displayed)
				{
					binaryWriter.Write(builderPiece2.shelfOwner);
				}
				else
				{
					binaryWriter.Write(builderPiece2.heldByPlayerActorNumber);
				}
				binaryWriter.Write(builderPiece2.heldInLeftHand ? 1 : 0);
				binaryWriter.Write(builderPiece2.materialType);
				long num = BitPackUtils.PackWorldPosForNetwork(builderPiece2.transform.localPosition);
				int num2 = BitPackUtils.PackQuaternionForNetwork(builderPiece2.transform.localRotation);
				binaryWriter.Write(num);
				binaryWriter.Write(num2);
				if (builderPiece2.state == BuilderPiece.State.AttachedAndPlaced)
				{
					binaryWriter.Write(builderPiece2.functionalPieceState);
					binaryWriter.Write(builderPiece2.activatedTimeStamp);
				}
				if (builderPiece2.state == BuilderPiece.State.OnConveyor)
				{
					binaryWriter.Write((this.conveyorManager == null) ? 0 : this.conveyorManager.GetPieceCreateTimestamp(builderPiece2));
				}
			}
			binaryWriter.Write(BuilderTable.childPieces.Count);
			for (int j = 0; j < BuilderTable.childPieces.Count; j++)
			{
				BuilderPiece builderPiece3 = BuilderTable.childPieces[j];
				binaryWriter.Write(builderPiece3.pieceType);
				binaryWriter.Write(builderPiece3.pieceId);
				int num3 = ((builderPiece3.parentPiece == null) ? (-1) : builderPiece3.parentPiece.pieceId);
				binaryWriter.Write(num3);
				binaryWriter.Write(builderPiece3.attachIndex);
				binaryWriter.Write(builderPiece3.parentAttachIndex);
				binaryWriter.Write((byte)builderPiece3.state);
				if (builderPiece3.state == BuilderPiece.State.OnConveyor || builderPiece3.state == BuilderPiece.State.OnShelf || builderPiece3.state == BuilderPiece.State.Displayed)
				{
					binaryWriter.Write(builderPiece3.shelfOwner);
				}
				else
				{
					binaryWriter.Write(builderPiece3.heldByPlayerActorNumber);
				}
				binaryWriter.Write(builderPiece3.heldInLeftHand ? 1 : 0);
				binaryWriter.Write(builderPiece3.materialType);
				int piecePlacement = builderPiece3.GetPiecePlacement();
				binaryWriter.Write(piecePlacement);
				if (builderPiece3.state == BuilderPiece.State.AttachedAndPlaced)
				{
					binaryWriter.Write(builderPiece3.functionalPieceState);
					binaryWriter.Write(builderPiece3.activatedTimeStamp);
				}
				if (builderPiece3.state == BuilderPiece.State.OnConveyor)
				{
					binaryWriter.Write((this.conveyorManager == null) ? 0 : this.conveyorManager.GetPieceCreateTimestamp(builderPiece3));
				}
			}
			if (this.isTableMutable)
			{
				binaryWriter.Write(this.plotOwners.Count);
				using (Dictionary<int, int>.Enumerator enumerator4 = this.plotOwners.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						KeyValuePair<int, int> keyValuePair = enumerator4.Current;
						binaryWriter.Write(keyValuePair.Key);
						binaryWriter.Write(keyValuePair.Value);
					}
					goto IL_04F9;
				}
			}
			if (this.sharedBlocksMap == null || this.sharedBlocksMap.MapID == null || !SharedBlocksManager.IsMapIDValid(this.sharedBlocksMap.MapID))
			{
				for (int k = 0; k < BuilderTable.mapIDBuffer.Length; k++)
				{
					BuilderTable.mapIDBuffer[k] = 'a';
				}
			}
			else
			{
				for (int l = 0; l < BuilderTable.mapIDBuffer.Length; l++)
				{
					BuilderTable.mapIDBuffer[l] = this.sharedBlocksMap.MapID[l];
				}
			}
			binaryWriter.Write(BuilderTable.mapIDBuffer);
			IL_04F9:
			long position = memoryStream.Position;
			BuilderTable.overlapPieces.Clear();
			BuilderTable.overlapOtherPieces.Clear();
			BuilderTable.overlapPacked.Clear();
			BuilderTable.tempDuplicateOverlaps.Clear();
			foreach (BuilderPiece builderPiece4 in this.pieces)
			{
				if (!(builderPiece4 == null))
				{
					for (int m = 0; m < builderPiece4.gridPlanes.Count; m++)
					{
						if (!(builderPiece4.gridPlanes[m] == null))
						{
							for (SnapOverlap snapOverlap = builderPiece4.gridPlanes[m].firstOverlap; snapOverlap != null; snapOverlap = snapOverlap.nextOverlap)
							{
								BuilderTable.SnapOverlapKey snapOverlapKey = BuilderTable.BuildOverlapKey(builderPiece4.pieceId, snapOverlap.otherPlane.piece.pieceId, m, snapOverlap.otherPlane.attachIndex);
								if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey))
								{
									BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey);
									long num4 = this.PackSnapInfo(m, snapOverlap.otherPlane.attachIndex, snapOverlap.bounds.min, snapOverlap.bounds.max);
									BuilderTable.overlapPieces.Add(builderPiece4.pieceId);
									BuilderTable.overlapOtherPieces.Add(snapOverlap.otherPlane.piece.pieceId);
									BuilderTable.overlapPacked.Add(num4);
								}
							}
						}
					}
				}
			}
			foreach (BuilderPiece builderPiece5 in this.basePieces)
			{
				if (!(builderPiece5 == null))
				{
					for (int n = 0; n < builderPiece5.gridPlanes.Count; n++)
					{
						if (!(builderPiece5.gridPlanes[n] == null))
						{
							for (SnapOverlap snapOverlap2 = builderPiece5.gridPlanes[n].firstOverlap; snapOverlap2 != null; snapOverlap2 = snapOverlap2.nextOverlap)
							{
								BuilderTable.SnapOverlapKey snapOverlapKey2 = BuilderTable.BuildOverlapKey(builderPiece5.pieceId, snapOverlap2.otherPlane.piece.pieceId, n, snapOverlap2.otherPlane.attachIndex);
								if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey2))
								{
									BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey2);
									long num5 = this.PackSnapInfo(n, snapOverlap2.otherPlane.attachIndex, snapOverlap2.bounds.min, snapOverlap2.bounds.max);
									BuilderTable.overlapPieces.Add(builderPiece5.pieceId);
									BuilderTable.overlapOtherPieces.Add(snapOverlap2.otherPlane.piece.pieceId);
									BuilderTable.overlapPacked.Add(num5);
								}
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			binaryWriter.Write(BuilderTable.overlapPieces.Count);
			for (int num6 = 0; num6 < BuilderTable.overlapPieces.Count; num6++)
			{
				binaryWriter.Write(BuilderTable.overlapPieces[num6]);
				binaryWriter.Write(BuilderTable.overlapOtherPieces[num6]);
				binaryWriter.Write(BuilderTable.overlapPacked[num6]);
			}
			return (int)memoryStream.Position;
		}

		// Token: 0x06004463 RID: 17507 RVA: 0x00141C5C File Offset: 0x0013FE5C
		public void DeserializeTableState(byte[] bytes, int numBytes)
		{
			if (numBytes <= 0)
			{
				return;
			}
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(bytes));
			BuilderTable.tempPeiceIds.Clear();
			BuilderTable.tempParentPeiceIds.Clear();
			BuilderTable.tempAttachIndexes.Clear();
			BuilderTable.tempParentAttachIndexes.Clear();
			BuilderTable.tempParentActorNumbers.Clear();
			BuilderTable.tempInLeftHand.Clear();
			BuilderTable.tempPiecePlacement.Clear();
			int num = binaryReader.ReadInt32();
			bool flag = this.conveyors != null;
			for (int i = 0; i < num; i++)
			{
				int num2 = binaryReader.ReadInt32();
				if (flag && i < this.conveyors.Count)
				{
					this.conveyors[i].SetSelection(num2);
				}
			}
			int num3 = binaryReader.ReadInt32();
			bool flag2 = this.dispenserShelves != null;
			for (int j = 0; j < num3; j++)
			{
				int num4 = binaryReader.ReadInt32();
				if (flag2 && j < this.dispenserShelves.Count)
				{
					this.dispenserShelves[j].SetSelection(num4);
				}
			}
			int num5 = binaryReader.ReadInt32();
			for (int k = 0; k < num5; k++)
			{
				int num6 = binaryReader.ReadInt32();
				int num7 = binaryReader.ReadInt32();
				BuilderPiece.State state = (BuilderPiece.State)binaryReader.ReadByte();
				int num8 = binaryReader.ReadInt32();
				bool flag3 = binaryReader.ReadByte() > 0;
				int num9 = binaryReader.ReadInt32();
				long num10 = binaryReader.ReadInt64();
				int num11 = binaryReader.ReadInt32();
				Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(num10);
				Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(num11);
				byte b = ((state == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadByte() : 0);
				int num12 = ((state == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadInt32() : 0);
				int num13 = ((state == BuilderPiece.State.OnConveyor) ? binaryReader.ReadInt32() : 0);
				float num14 = 10000f;
				if (!(in vector).IsValid(in num14) || !(in quaternion).IsValid() || !this.ValidateCreatePieceParams(num6, num7, state, num9))
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				int num15 = -1;
				if (state == BuilderPiece.State.OnConveyor || state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.Displayed)
				{
					num15 = num8;
					num8 = -1;
				}
				if (this.ValidateDeserializedRootPieceState(num7, state, num15, num8, vector, quaternion))
				{
					BuilderPiece builderPiece = this.CreatePieceInternal(num6, num7, vector, quaternion, state, num9, num12, this);
					BuilderTable.tempPeiceIds.Add(num7);
					BuilderTable.tempParentActorNumbers.Add(num8);
					BuilderTable.tempInLeftHand.Add(flag3);
					builderPiece.SetFunctionalPieceState(b, NetPlayer.Get(PhotonNetwork.MasterClient), PhotonNetwork.ServerTimestamp);
					if (num15 >= 0 && this.isTableMutable)
					{
						builderPiece.shelfOwner = num15;
						if (state == BuilderPiece.State.OnConveyor)
						{
							BuilderConveyor builderConveyor = this.conveyors[num15];
							float num16 = 0f;
							if (PhotonNetwork.ServerTimestamp > num13)
							{
								num16 = (PhotonNetwork.ServerTimestamp - num13) / 1000f;
							}
							builderConveyor.OnShelfPieceCreated(builderPiece, num16);
						}
						else if (state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.Displayed)
						{
							this.dispenserShelves[num15].OnShelfPieceCreated(builderPiece, false);
						}
					}
				}
			}
			for (int l = 0; l < BuilderTable.tempPeiceIds.Count; l++)
			{
				if (BuilderTable.tempParentActorNumbers[l] >= 0)
				{
					this.AttachPieceToActorInternal(BuilderTable.tempPeiceIds[l], BuilderTable.tempParentActorNumbers[l], BuilderTable.tempInLeftHand[l]);
				}
			}
			BuilderTable.tempPeiceIds.Clear();
			BuilderTable.tempParentActorNumbers.Clear();
			BuilderTable.tempInLeftHand.Clear();
			int num17 = binaryReader.ReadInt32();
			for (int m = 0; m < num17; m++)
			{
				int num18 = binaryReader.ReadInt32();
				int num19 = binaryReader.ReadInt32();
				int num20 = binaryReader.ReadInt32();
				int num21 = binaryReader.ReadInt32();
				int num22 = binaryReader.ReadInt32();
				BuilderPiece.State state2 = (BuilderPiece.State)binaryReader.ReadByte();
				int num23 = binaryReader.ReadInt32();
				bool flag4 = binaryReader.ReadByte() > 0;
				int num24 = binaryReader.ReadInt32();
				int num25 = binaryReader.ReadInt32();
				byte b2 = ((state2 == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadByte() : 0);
				int num26 = ((state2 == BuilderPiece.State.AttachedAndPlaced) ? binaryReader.ReadInt32() : 0);
				int num27 = ((state2 == BuilderPiece.State.OnConveyor) ? binaryReader.ReadInt32() : 0);
				if (!this.ValidateCreatePieceParams(num18, num19, state2, num24))
				{
					this.SetTableState(BuilderTable.TableState.BadData);
					return;
				}
				int num28 = -1;
				if (state2 == BuilderPiece.State.OnConveyor || state2 == BuilderPiece.State.OnShelf || state2 == BuilderPiece.State.Displayed)
				{
					num28 = num23;
					num23 = -1;
				}
				if (this.ValidateDeserializedChildPieceState(num19, state2))
				{
					BuilderPiece builderPiece2 = this.CreatePieceInternal(num18, num19, this.roomCenter.position, Quaternion.identity, state2, num24, num26, this);
					builderPiece2.SetFunctionalPieceState(b2, NetPlayer.Get(PhotonNetwork.MasterClient), PhotonNetwork.ServerTimestamp);
					BuilderTable.tempPeiceIds.Add(num19);
					BuilderTable.tempParentPeiceIds.Add(num20);
					BuilderTable.tempAttachIndexes.Add(num21);
					BuilderTable.tempParentAttachIndexes.Add(num22);
					BuilderTable.tempParentActorNumbers.Add(num23);
					BuilderTable.tempInLeftHand.Add(flag4);
					BuilderTable.tempPiecePlacement.Add(num25);
					if (num28 >= 0 && this.isTableMutable)
					{
						builderPiece2.shelfOwner = num28;
						if (state2 == BuilderPiece.State.OnConveyor)
						{
							BuilderConveyor builderConveyor2 = this.conveyors[num28];
							float num29 = 0f;
							if (PhotonNetwork.ServerTimestamp > num27)
							{
								num29 = (PhotonNetwork.ServerTimestamp - num27) / 1000f;
							}
							builderConveyor2.OnShelfPieceCreated(builderPiece2, num29);
						}
						else if (state2 == BuilderPiece.State.OnShelf || state2 == BuilderPiece.State.Displayed)
						{
							this.dispenserShelves[num28].OnShelfPieceCreated(builderPiece2, false);
						}
					}
				}
			}
			for (int n = 0; n < BuilderTable.tempPeiceIds.Count; n++)
			{
				if (!this.ValidateAttachPieceParams(BuilderTable.tempPeiceIds[n], BuilderTable.tempAttachIndexes[n], BuilderTable.tempParentPeiceIds[n], BuilderTable.tempParentAttachIndexes[n], BuilderTable.tempPiecePlacement[n]))
				{
					this.RecyclePieceInternal(BuilderTable.tempPeiceIds[n], true, false, -1);
				}
				else
				{
					this.AttachPieceInternal(BuilderTable.tempPeiceIds[n], BuilderTable.tempAttachIndexes[n], BuilderTable.tempParentPeiceIds[n], BuilderTable.tempParentAttachIndexes[n], BuilderTable.tempPiecePlacement[n]);
				}
			}
			for (int num30 = 0; num30 < BuilderTable.tempPeiceIds.Count; num30++)
			{
				if (BuilderTable.tempParentActorNumbers[num30] >= 0)
				{
					this.AttachPieceToActorInternal(BuilderTable.tempPeiceIds[num30], BuilderTable.tempParentActorNumbers[num30], BuilderTable.tempInLeftHand[num30]);
				}
			}
			foreach (BuilderPiece builderPiece3 in this.pieces)
			{
				if (builderPiece3.state == BuilderPiece.State.AttachedAndPlaced)
				{
					builderPiece3.OnPlacementDeserialized();
				}
			}
			if (this.isTableMutable)
			{
				this.plotOwners.Clear();
				this.doesLocalPlayerOwnPlot = false;
				int num31 = binaryReader.ReadInt32();
				for (int num32 = 0; num32 < num31; num32++)
				{
					int num33 = binaryReader.ReadInt32();
					int num34 = binaryReader.ReadInt32();
					BuilderPiecePrivatePlot builderPiecePrivatePlot;
					if (this.plotOwners.TryAdd(num33, num34) && this.GetPiece(num34).TryGetPlotComponent(out builderPiecePrivatePlot))
					{
						builderPiecePrivatePlot.ClaimPlotForPlayerNumber(num33);
						if (num33 == PhotonNetwork.LocalPlayer.ActorNumber)
						{
							this.doesLocalPlayerOwnPlot = true;
						}
					}
				}
				UnityEvent<bool> onLocalPlayerClaimedPlot = this.OnLocalPlayerClaimedPlot;
				if (onLocalPlayerClaimedPlot != null)
				{
					onLocalPlayerClaimedPlot.Invoke(this.doesLocalPlayerOwnPlot);
				}
				this.OnDeserializeUpdatePlots();
			}
			else
			{
				BuilderTable.mapIDBuffer = binaryReader.ReadChars(BuilderTable.mapIDBuffer.Length);
				string text = new string(BuilderTable.mapIDBuffer);
				if (SharedBlocksManager.IsMapIDValid(text))
				{
					this.sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
					{
						MapID = text
					};
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
			int num35 = binaryReader.ReadInt32();
			for (int num36 = 0; num36 < num35; num36++)
			{
				int num37 = binaryReader.ReadInt32();
				int num38 = binaryReader.ReadInt32();
				long num39 = binaryReader.ReadInt64();
				BuilderPiece piece = this.GetPiece(num37);
				if (!(piece == null))
				{
					BuilderPiece piece2 = this.GetPiece(num38);
					if (!(piece2 == null))
					{
						int num40;
						int num41;
						Vector2Int vector2Int;
						Vector2Int vector2Int2;
						this.UnpackSnapInfo(num39, out num40, out num41, out vector2Int, out vector2Int2);
						if (num40 >= 0 && num40 < piece.gridPlanes.Count && num41 >= 0 && num41 < piece2.gridPlanes.Count)
						{
							BuilderTable.SnapOverlapKey snapOverlapKey = BuilderTable.BuildOverlapKey(num37, num38, num40, num41);
							if (!BuilderTable.tempDuplicateOverlaps.Contains(snapOverlapKey))
							{
								BuilderTable.tempDuplicateOverlaps.Add(snapOverlapKey);
								piece.gridPlanes[num40].AddSnapOverlap(this.builderPool.CreateSnapOverlap(piece2.gridPlanes[num41], new SnapBounds(vector2Int, vector2Int2)));
							}
						}
					}
				}
			}
			BuilderTable.tempDuplicateOverlaps.Clear();
		}

		// Token: 0x0400461E RID: 17950
		public const GTZone BUILDER_ZONE = GTZone.monkeBlocks;

		// Token: 0x0400461F RID: 17951
		private const int INITIAL_BUILTIN_PIECE_ID = 5;

		// Token: 0x04004620 RID: 17952
		private const int INITIAL_CREATED_PIECE_ID = 10000;

		// Token: 0x04004621 RID: 17953
		public static float MAX_DROP_VELOCITY = 20f;

		// Token: 0x04004622 RID: 17954
		public static float MAX_DROP_ANG_VELOCITY = 50f;

		// Token: 0x04004623 RID: 17955
		private const float MAX_DISTANCE_FROM_CENTER = 217f;

		// Token: 0x04004624 RID: 17956
		private const float MAX_LOCAL_MAGNITUDE = 80f;

		// Token: 0x04004625 RID: 17957
		public const float MAX_DISTANCE_FROM_HAND = 2.5f;

		// Token: 0x04004626 RID: 17958
		public static float DROP_ZONE_REPEL = 2.25f;

		// Token: 0x04004627 RID: 17959
		public static int placedLayer;

		// Token: 0x04004628 RID: 17960
		public static int heldLayer;

		// Token: 0x04004629 RID: 17961
		public static int heldLayerLocal;

		// Token: 0x0400462A RID: 17962
		public static int droppedLayer;

		// Token: 0x0400462B RID: 17963
		private float acceptableSqrDistFromCenter = 47089f;

		// Token: 0x0400462C RID: 17964
		public List<BuilderPiece> builderPieces;

		// Token: 0x0400462D RID: 17965
		[FormerlySerializedAs("isTableMutible")]
		[Header("Scene References")]
		public bool isTableMutable = true;

		// Token: 0x0400462E RID: 17966
		public GTZone tableZone = GTZone.monkeBlocks;

		// Token: 0x0400462F RID: 17967
		public BuilderTableNetworking builderNetworking;

		// Token: 0x04004630 RID: 17968
		public BuilderRenderer builderRenderer;

		// Token: 0x04004631 RID: 17969
		public BuilderPool builderPool;

		// Token: 0x04004632 RID: 17970
		public SizeChanger sizeChanger;

		// Token: 0x04004633 RID: 17971
		public GameObject shelvesRoot;

		// Token: 0x04004634 RID: 17972
		public GameObject dropZoneRoot;

		// Token: 0x04004635 RID: 17973
		public List<GameObject> recyclerRoot;

		// Token: 0x04004636 RID: 17974
		public List<GameObject> allShelvesRoot;

		// Token: 0x04004637 RID: 17975
		public List<BuilderFactory> factories;

		// Token: 0x04004638 RID: 17976
		[NonSerialized]
		public List<BuilderConveyor> conveyors = new List<BuilderConveyor>();

		// Token: 0x04004639 RID: 17977
		[NonSerialized]
		public List<BuilderDispenserShelf> dispenserShelves = new List<BuilderDispenserShelf>();

		// Token: 0x0400463A RID: 17978
		public BuilderConveyorManager conveyorManager;

		// Token: 0x0400463B RID: 17979
		public List<BuilderResourceMeter> resourceMeters;

		// Token: 0x0400463C RID: 17980
		public SharedBlocksTerminal linkedTerminal;

		// Token: 0x0400463D RID: 17981
		[NonSerialized]
		public List<BuilderRecycler> recyclers;

		// Token: 0x0400463E RID: 17982
		[NonSerialized]
		public List<BuilderDropZone> dropZones;

		// Token: 0x0400463F RID: 17983
		private int shelfSliceUpdateIndex;

		// Token: 0x04004640 RID: 17984
		public static int SHELF_SLICE_BUCKETS = 6;

		// Token: 0x04004641 RID: 17985
		[Header("Tint Settings")]
		public float defaultTint = 1f;

		// Token: 0x04004642 RID: 17986
		public float droppedTint = 0.75f;

		// Token: 0x04004643 RID: 17987
		public float grabbedTint = 0.75f;

		// Token: 0x04004644 RID: 17988
		public float shelfTint = 1f;

		// Token: 0x04004645 RID: 17989
		public float potentialGrabTint = 0.75f;

		// Token: 0x04004646 RID: 17990
		public float paintingTint = 0.6f;

		// Token: 0x04004647 RID: 17991
		[Header("Table Transform")]
		public Transform tableCenter;

		// Token: 0x04004648 RID: 17992
		public Transform roomCenter;

		// Token: 0x04004649 RID: 17993
		public Transform worldCenter;

		// Token: 0x0400464A RID: 17994
		public GameObject sharedBuildArea;

		// Token: 0x0400464B RID: 17995
		private BoxCollider[] sharedBuildAreas;

		// Token: 0x0400464C RID: 17996
		public GameObject noBlocksArea;

		// Token: 0x0400464D RID: 17997
		private List<BuilderTable.BoxCheckParams> noBlocksAreas;

		// Token: 0x0400464E RID: 17998
		private Collider[] noBlocksCheckResults = new Collider[64];

		// Token: 0x0400464F RID: 17999
		[Header("Table Scale")]
		public float tableToWorldScale = 50f;

		// Token: 0x04004650 RID: 18000
		public float pieceScale = 0.04f;

		// Token: 0x04004651 RID: 18001
		public float gridSize = 0.02f;

		// Token: 0x04004652 RID: 18002
		[Header("Layers")]
		public LayerMask allPiecesMask;

		// Token: 0x04004653 RID: 18003
		[Header("Builder Options")]
		public bool useSnapRotation;

		// Token: 0x04004654 RID: 18004
		public BuilderPlacementStyle usePlacementStyle;

		// Token: 0x04004655 RID: 18005
		public bool buildInPlace;

		// Token: 0x04004656 RID: 18006
		public BuilderOptionButton buttonSnapRotation;

		// Token: 0x04004657 RID: 18007
		public BuilderOptionButton buttonSnapPosition;

		// Token: 0x04004658 RID: 18008
		public BuilderOptionButton buttonSaveLayout;

		// Token: 0x04004659 RID: 18009
		public BuilderOptionButton buttonClearLayout;

		// Token: 0x0400465A RID: 18010
		[HideInInspector]
		public List<BuilderAttachGridPlane> baseGridPlanes;

		// Token: 0x0400465B RID: 18011
		[Header("Piece Fabrication")]
		public List<GameObject> builtInPieceRoots;

		// Token: 0x0400465C RID: 18012
		private List<BuilderPiece> basePieces;

		// Token: 0x0400465D RID: 18013
		public List<BuilderPiecePrivatePlot> allPrivatePlots;

		// Token: 0x0400465E RID: 18014
		public BuilderPiece armShelfPieceType;

		// Token: 0x0400465F RID: 18015
		private int nextPieceId;

		// Token: 0x04004660 RID: 18016
		[HideInInspector]
		public List<BuilderTable.BuildPieceSpawn> buildPieceSpawns;

		// Token: 0x04004661 RID: 18017
		[HideInInspector]
		public List<BuilderShelf> shelves;

		// Token: 0x04004662 RID: 18018
		[NonSerialized]
		public List<BuilderPiece> pieces = new List<BuilderPiece>(1024);

		// Token: 0x04004663 RID: 18019
		private Dictionary<int, int> pieceIDToIndexCache = new Dictionary<int, int>(1024);

		// Token: 0x04004664 RID: 18020
		[HideInInspector]
		public Dictionary<int, int> plotOwners;

		// Token: 0x04004665 RID: 18021
		private bool doesLocalPlayerOwnPlot;

		// Token: 0x04004666 RID: 18022
		public Dictionary<int, int> playerToArmShelfLeft;

		// Token: 0x04004667 RID: 18023
		public Dictionary<int, int> playerToArmShelfRight;

		// Token: 0x04004668 RID: 18024
		private HashSet<int> builderPiecesVisited = new HashSet<int>(128);

		// Token: 0x04004669 RID: 18025
		public BuilderResources totalResources;

		// Token: 0x0400466A RID: 18026
		[Tooltip("Resources reserved for conveyors and dispensers")]
		public BuilderResources totalReservedResources;

		// Token: 0x0400466B RID: 18027
		public BuilderResources resourcesPerPrivatePlot;

		// Token: 0x0400466C RID: 18028
		[NonSerialized]
		public int[] maxResources;

		// Token: 0x0400466D RID: 18029
		private int[] plotMaxResources;

		// Token: 0x0400466E RID: 18030
		[NonSerialized]
		public int[] usedResources;

		// Token: 0x0400466F RID: 18031
		[NonSerialized]
		public int[] reservedResources;

		// Token: 0x04004670 RID: 18032
		private List<int> playersInBuilder;

		// Token: 0x04004671 RID: 18033
		private List<IBuilderPieceFunctional> activeFunctionalComponents = new List<IBuilderPieceFunctional>(128);

		// Token: 0x04004672 RID: 18034
		private List<IBuilderPieceFunctional> funcComponentsToRegister = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04004673 RID: 18035
		private List<IBuilderPieceFunctional> funcComponentsToUnregister = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04004674 RID: 18036
		private List<IBuilderPieceFunctional> fixedUpdateFunctionalComponents = new List<IBuilderPieceFunctional>(128);

		// Token: 0x04004675 RID: 18037
		private List<IBuilderPieceFunctional> funcComponentsToRegisterFixed = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04004676 RID: 18038
		private List<IBuilderPieceFunctional> funcComponentsToUnregisterFixed = new List<IBuilderPieceFunctional>(10);

		// Token: 0x04004677 RID: 18039
		private const int MAX_SPHERE_CHECK_RESULTS = 1024;

		// Token: 0x04004678 RID: 18040
		private NativeList<BuilderGridPlaneData> gridPlaneData;

		// Token: 0x04004679 RID: 18041
		private NativeList<BuilderGridPlaneData> checkGridPlaneData;

		// Token: 0x0400467A RID: 18042
		private NativeArray<ColliderHit> nearbyPiecesResults;

		// Token: 0x0400467B RID: 18043
		private NativeArray<OverlapSphereCommand> nearbyPiecesCommands;

		// Token: 0x0400467C RID: 18044
		private List<BuilderPotentialPlacement> allPotentialPlacements;

		// Token: 0x0400467D RID: 18045
		private static HashSet<BuilderPiece> tempPieceSet = new HashSet<BuilderPiece>(512);

		// Token: 0x0400467E RID: 18046
		private BuilderTable.TableState tableState;

		// Token: 0x0400467F RID: 18047
		private bool inRoom;

		// Token: 0x04004680 RID: 18048
		private bool inBuilderZone;

		// Token: 0x04004681 RID: 18049
		private static int DROPPED_PIECE_LIMIT = 100;

		// Token: 0x04004682 RID: 18050
		public static string nextUpdateOverride = string.Empty;

		// Token: 0x04004683 RID: 18051
		private List<BuilderPiece> droppedPieces;

		// Token: 0x04004684 RID: 18052
		private List<BuilderTable.DroppedPieceData> droppedPieceData;

		// Token: 0x04004685 RID: 18053
		private HashSet<int>[] repelledPieceRoots;

		// Token: 0x04004686 RID: 18054
		private int repelHistoryLength = 3;

		// Token: 0x04004687 RID: 18055
		private int repelHistoryIndex;

		// Token: 0x04004688 RID: 18056
		private bool hasRequestedConfig;

		// Token: 0x04004689 RID: 18057
		private bool isDirty;

		// Token: 0x0400468A RID: 18058
		private bool saveInProgress;

		// Token: 0x0400468B RID: 18059
		private int currentSaveSlot = -1;

		// Token: 0x0400468C RID: 18060
		[HideInInspector]
		public UnityEvent OnSaveTimeUpdated;

		// Token: 0x0400468D RID: 18061
		[HideInInspector]
		public UnityEvent<bool> OnSaveDirtyChanged;

		// Token: 0x0400468E RID: 18062
		[HideInInspector]
		public UnityEvent OnSaveSuccess;

		// Token: 0x0400468F RID: 18063
		[HideInInspector]
		public UnityEvent<string> OnSaveFailure;

		// Token: 0x04004690 RID: 18064
		[HideInInspector]
		public UnityEvent OnTableConfigurationUpdated;

		// Token: 0x04004691 RID: 18065
		[HideInInspector]
		public UnityEvent<bool> OnLocalPlayerClaimedPlot;

		// Token: 0x04004692 RID: 18066
		[HideInInspector]
		public UnityEvent OnMapCleared;

		// Token: 0x04004693 RID: 18067
		[HideInInspector]
		public UnityEvent<string> OnMapLoaded;

		// Token: 0x04004694 RID: 18068
		[HideInInspector]
		public UnityEvent<string> OnMapLoadFailed;

		// Token: 0x04004695 RID: 18069
		private List<BuilderTable.BuilderCommand> queuedBuildCommands;

		// Token: 0x04004696 RID: 18070
		private List<BuilderAction> rollBackActions;

		// Token: 0x04004697 RID: 18071
		private List<BuilderTable.BuilderCommand> rollBackBufferedCommands;

		// Token: 0x04004698 RID: 18072
		private List<BuilderTable.BuilderCommand> rollForwardCommands;

		// Token: 0x04004699 RID: 18073
		private static Dictionary<GTZone, BuilderTable> zoneToInstance;

		// Token: 0x0400469A RID: 18074
		private bool isSetup;

		// Token: 0x0400469B RID: 18075
		[Header("Snap Params")]
		public BuilderTable.SnapParams pushAndEaseParams;

		// Token: 0x0400469C RID: 18076
		public BuilderTable.SnapParams overlapParams;

		// Token: 0x0400469D RID: 18077
		private BuilderTable.SnapParams currSnapParams;

		// Token: 0x0400469E RID: 18078
		public int maxPlacementChildDepth = 5;

		// Token: 0x0400469F RID: 18079
		private static List<BuilderPiece> tempPieces = new List<BuilderPiece>(256);

		// Token: 0x040046A0 RID: 18080
		private static List<BuilderConveyor> tempConveyors = new List<BuilderConveyor>(256);

		// Token: 0x040046A1 RID: 18081
		private static List<BuilderDispenserShelf> tempDispensers = new List<BuilderDispenserShelf>(256);

		// Token: 0x040046A2 RID: 18082
		private static List<BuilderRecycler> tempRecyclers = new List<BuilderRecycler>(5);

		// Token: 0x040046A3 RID: 18083
		private static List<BuilderTable.BuilderCommand> tempRollForwardCommands = new List<BuilderTable.BuilderCommand>(128);

		// Token: 0x040046A4 RID: 18084
		private static List<BuilderPiece> tempDeletePieces = new List<BuilderPiece>(1024);

		// Token: 0x040046A5 RID: 18085
		public const int MAX_PIECE_DATA = 2560;

		// Token: 0x040046A6 RID: 18086
		public const int MAX_GRID_PLANE_DATA = 10240;

		// Token: 0x040046A7 RID: 18087
		public const int MAX_PRIVATE_PLOT_DATA = 64;

		// Token: 0x040046A8 RID: 18088
		public const int MAX_PLAYER_DATA = 64;

		// Token: 0x040046A9 RID: 18089
		private BuilderTableData tableData;

		// Token: 0x040046AA RID: 18090
		private int fetchConfigurationAttempts;

		// Token: 0x040046AB RID: 18091
		private int maxRetries = 3;

		// Token: 0x040046AC RID: 18092
		private SharedBlocksManager.SharedBlocksMap sharedBlocksMap;

		// Token: 0x040046AD RID: 18093
		private string pendingMapID;

		// Token: 0x040046AE RID: 18094
		private static string personalBuildKey = "MyBuild";

		// Token: 0x040046AF RID: 18095
		private static HashSet<BuilderTable.SnapOverlapKey> tempDuplicateOverlaps = new HashSet<BuilderTable.SnapOverlapKey>(16384);

		// Token: 0x040046B0 RID: 18096
		private static List<BuilderPiece> childPieces = new List<BuilderPiece>(4096);

		// Token: 0x040046B1 RID: 18097
		private static List<BuilderPiece> rootPieces = new List<BuilderPiece>(4096);

		// Token: 0x040046B2 RID: 18098
		private static List<int> overlapPieces = new List<int>(4096);

		// Token: 0x040046B3 RID: 18099
		private static List<int> overlapOtherPieces = new List<int>(4096);

		// Token: 0x040046B4 RID: 18100
		private static List<long> overlapPacked = new List<long>(4096);

		// Token: 0x040046B5 RID: 18101
		private static char[] mapIDBuffer = new char[8];

		// Token: 0x040046B6 RID: 18102
		private static Dictionary<long, int> snapOverlapSanity = new Dictionary<long, int>(16384);

		// Token: 0x040046B7 RID: 18103
		private static List<int> tempPeiceIds = new List<int>(4096);

		// Token: 0x040046B8 RID: 18104
		private static List<int> tempParentPeiceIds = new List<int>(4096);

		// Token: 0x040046B9 RID: 18105
		private static List<int> tempAttachIndexes = new List<int>(4096);

		// Token: 0x040046BA RID: 18106
		private static List<int> tempParentAttachIndexes = new List<int>(4096);

		// Token: 0x040046BB RID: 18107
		private static List<int> tempParentActorNumbers = new List<int>(4096);

		// Token: 0x040046BC RID: 18108
		private static List<bool> tempInLeftHand = new List<bool>(4096);

		// Token: 0x040046BD RID: 18109
		private static List<int> tempPiecePlacement = new List<int>(4096);

		// Token: 0x02000AE5 RID: 2789
		private struct BoxCheckParams
		{
			// Token: 0x040046BE RID: 18110
			public Vector3 center;

			// Token: 0x040046BF RID: 18111
			public Vector3 halfExtents;

			// Token: 0x040046C0 RID: 18112
			public Quaternion rotation;
		}

		// Token: 0x02000AE6 RID: 2790
		[Serializable]
		public class BuildPieceSpawn
		{
			// Token: 0x040046C1 RID: 18113
			public GameObject buildPiecePrefab;

			// Token: 0x040046C2 RID: 18114
			public int count = 1;
		}

		// Token: 0x02000AE7 RID: 2791
		public enum BuilderCommandType
		{
			// Token: 0x040046C4 RID: 18116
			Create,
			// Token: 0x040046C5 RID: 18117
			Place,
			// Token: 0x040046C6 RID: 18118
			Grab,
			// Token: 0x040046C7 RID: 18119
			Drop,
			// Token: 0x040046C8 RID: 18120
			Remove,
			// Token: 0x040046C9 RID: 18121
			Paint,
			// Token: 0x040046CA RID: 18122
			Recycle,
			// Token: 0x040046CB RID: 18123
			ClaimPlot,
			// Token: 0x040046CC RID: 18124
			FreePlot,
			// Token: 0x040046CD RID: 18125
			CreateArmShelf,
			// Token: 0x040046CE RID: 18126
			PlayerLeftRoom,
			// Token: 0x040046CF RID: 18127
			FunctionalStateChange,
			// Token: 0x040046D0 RID: 18128
			SetSelection,
			// Token: 0x040046D1 RID: 18129
			Repel
		}

		// Token: 0x02000AE8 RID: 2792
		public enum TableState
		{
			// Token: 0x040046D3 RID: 18131
			WaitingForZoneAndRoom,
			// Token: 0x040046D4 RID: 18132
			WaitingForInitalBuild,
			// Token: 0x040046D5 RID: 18133
			ReceivingInitialBuild,
			// Token: 0x040046D6 RID: 18134
			WaitForInitialBuildMaster,
			// Token: 0x040046D7 RID: 18135
			WaitForMasterResync,
			// Token: 0x040046D8 RID: 18136
			ReceivingMasterResync,
			// Token: 0x040046D9 RID: 18137
			InitialBuild,
			// Token: 0x040046DA RID: 18138
			ExecuteQueuedCommands,
			// Token: 0x040046DB RID: 18139
			Ready,
			// Token: 0x040046DC RID: 18140
			BadData,
			// Token: 0x040046DD RID: 18141
			WaitingForSharedMapLoad
		}

		// Token: 0x02000AE9 RID: 2793
		public enum DroppedPieceState
		{
			// Token: 0x040046DF RID: 18143
			None = -1,
			// Token: 0x040046E0 RID: 18144
			Light,
			// Token: 0x040046E1 RID: 18145
			Heavy
		}

		// Token: 0x02000AEA RID: 2794
		private struct DroppedPieceData
		{
			// Token: 0x040046E2 RID: 18146
			public BuilderTable.DroppedPieceState droppedState;

			// Token: 0x040046E3 RID: 18147
			public float speedThreshCrossedTime;

			// Token: 0x040046E4 RID: 18148
			public float filteredSpeed;
		}

		// Token: 0x02000AEB RID: 2795
		public struct BuilderCommand
		{
			// Token: 0x040046E5 RID: 18149
			public BuilderTable.BuilderCommandType type;

			// Token: 0x040046E6 RID: 18150
			public int pieceType;

			// Token: 0x040046E7 RID: 18151
			public int pieceId;

			// Token: 0x040046E8 RID: 18152
			public int attachPieceId;

			// Token: 0x040046E9 RID: 18153
			public int parentPieceId;

			// Token: 0x040046EA RID: 18154
			public int parentAttachIndex;

			// Token: 0x040046EB RID: 18155
			public int attachIndex;

			// Token: 0x040046EC RID: 18156
			public Vector3 localPosition;

			// Token: 0x040046ED RID: 18157
			public Quaternion localRotation;

			// Token: 0x040046EE RID: 18158
			public byte twist;

			// Token: 0x040046EF RID: 18159
			public sbyte bumpOffsetX;

			// Token: 0x040046F0 RID: 18160
			public sbyte bumpOffsetZ;

			// Token: 0x040046F1 RID: 18161
			public Vector3 velocity;

			// Token: 0x040046F2 RID: 18162
			public Vector3 angVelocity;

			// Token: 0x040046F3 RID: 18163
			public bool isLeft;

			// Token: 0x040046F4 RID: 18164
			public int materialType;

			// Token: 0x040046F5 RID: 18165
			public NetPlayer player;

			// Token: 0x040046F6 RID: 18166
			public BuilderPiece.State state;

			// Token: 0x040046F7 RID: 18167
			public bool isQueued;

			// Token: 0x040046F8 RID: 18168
			public bool canRollback;

			// Token: 0x040046F9 RID: 18169
			public int localCommandId;

			// Token: 0x040046FA RID: 18170
			public int serverTimeStamp;
		}

		// Token: 0x02000AEC RID: 2796
		[Serializable]
		public struct SnapParams
		{
			// Token: 0x040046FB RID: 18171
			public float minOffsetY;

			// Token: 0x040046FC RID: 18172
			public float maxOffsetY;

			// Token: 0x040046FD RID: 18173
			public float maxUpDotProduct;

			// Token: 0x040046FE RID: 18174
			public float maxTwistDotProduct;

			// Token: 0x040046FF RID: 18175
			public float snapAttachDistance;

			// Token: 0x04004700 RID: 18176
			public float snapDelayTime;

			// Token: 0x04004701 RID: 18177
			public float snapDelayOffsetDist;

			// Token: 0x04004702 RID: 18178
			public float unSnapDelayTime;

			// Token: 0x04004703 RID: 18179
			public float unSnapDelayDist;

			// Token: 0x04004704 RID: 18180
			public float maxBlockSnapDist;
		}

		// Token: 0x02000AED RID: 2797
		private struct SnapOverlapKey
		{
			// Token: 0x06004467 RID: 17511 RVA: 0x001427C1 File Offset: 0x001409C1
			public override int GetHashCode()
			{
				return HashCode.Combine<int, int>(this.piece.GetHashCode(), this.otherPiece.GetHashCode());
			}

			// Token: 0x06004468 RID: 17512 RVA: 0x001427DE File Offset: 0x001409DE
			public bool Equals(BuilderTable.SnapOverlapKey other)
			{
				return this.piece == other.piece && this.otherPiece == other.otherPiece;
			}

			// Token: 0x06004469 RID: 17513 RVA: 0x001427FE File Offset: 0x001409FE
			public override bool Equals(object o)
			{
				return o is BuilderTable.SnapOverlapKey && this.Equals((BuilderTable.SnapOverlapKey)o);
			}

			// Token: 0x04004705 RID: 18181
			public long piece;

			// Token: 0x04004706 RID: 18182
			public long otherPiece;
		}
	}
}
