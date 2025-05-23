using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020003F6 RID: 1014
public class BodyDockPositions : MonoBehaviour
{
	// Token: 0x170002B6 RID: 694
	// (get) Token: 0x06001853 RID: 6227 RVA: 0x00076194 File Offset: 0x00074394
	// (set) Token: 0x06001854 RID: 6228 RVA: 0x0007619C File Offset: 0x0007439C
	public TransferrableObject[] allObjects
	{
		get
		{
			return this._allObjects;
		}
		set
		{
			this._allObjects = value;
		}
	}

	// Token: 0x06001855 RID: 6229 RVA: 0x000761A8 File Offset: 0x000743A8
	public void Awake()
	{
		RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(this.OnLeftRoom));
		RoomSystem.PlayerLeftEvent = (Action<NetPlayer>)Delegate.Combine(RoomSystem.PlayerLeftEvent, new Action<NetPlayer>(this.OnPlayerLeftRoom));
	}

	// Token: 0x06001856 RID: 6230 RVA: 0x000761F5 File Offset: 0x000743F5
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		if (object.Equals(this.myRig.creator, otherPlayer))
		{
			this.DeallocateSharableInstances();
		}
	}

	// Token: 0x06001857 RID: 6231 RVA: 0x00076210 File Offset: 0x00074410
	public void OnLeftRoom()
	{
		this.DeallocateSharableInstances();
	}

	// Token: 0x06001858 RID: 6232 RVA: 0x00076218 File Offset: 0x00074418
	public WorldShareableItem AllocateSharableInstance(BodyDockPositions.DropPositions position, NetPlayer owner)
	{
		switch (position)
		{
		case BodyDockPositions.DropPositions.None:
		case BodyDockPositions.DropPositions.LeftArm:
		case BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm:
		case BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.MaxDropPostions:
		case BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
		case BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.Chest:
			break;
		case BodyDockPositions.DropPositions.LeftBack:
			if (this.leftBackSharableItem == null)
			{
				this.leftBackSharableItem = ObjectPools.instance.Instantiate(this.SharableItemInstance, true).GetComponent<WorldShareableItem>();
				this.leftBackSharableItem.GetComponent<RequestableOwnershipGuard>().SetOwnership(owner, false, true);
				this.leftBackSharableItem.GetComponent<WorldShareableItem>().SetupSharableViewIDs(owner, 3);
			}
			return this.leftBackSharableItem;
		default:
			if (position == BodyDockPositions.DropPositions.RightBack)
			{
				if (this.rightBackShareableItem == null)
				{
					this.rightBackShareableItem = ObjectPools.instance.Instantiate(this.SharableItemInstance, true).GetComponent<WorldShareableItem>();
					this.rightBackShareableItem.GetComponent<RequestableOwnershipGuard>().SetOwnership(owner, false, true);
					this.rightBackShareableItem.GetComponent<WorldShareableItem>().SetupSharableViewIDs(owner, 4);
				}
				return this.rightBackShareableItem;
			}
			if (position != BodyDockPositions.DropPositions.All)
			{
			}
			break;
		}
		throw new ArgumentOutOfRangeException("position", position, null);
	}

	// Token: 0x06001859 RID: 6233 RVA: 0x00076314 File Offset: 0x00074514
	public void DeallocateSharableInstance(WorldShareableItem worldShareable)
	{
		if (worldShareable == null)
		{
			return;
		}
		if (worldShareable == this.leftBackSharableItem)
		{
			if (this.leftBackSharableItem == null)
			{
				return;
			}
			this.leftBackSharableItem.ResetViews();
			ObjectPools.instance.Destroy(this.leftBackSharableItem.gameObject);
			this.leftBackSharableItem = null;
		}
		if (worldShareable == this.rightBackShareableItem)
		{
			if (this.rightBackShareableItem == null)
			{
				return;
			}
			this.rightBackShareableItem.ResetViews();
			ObjectPools.instance.Destroy(this.rightBackShareableItem.gameObject);
			this.rightBackShareableItem = null;
		}
	}

	// Token: 0x0600185A RID: 6234 RVA: 0x000763A8 File Offset: 0x000745A8
	public void DeallocateSharableInstances()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.rightBackShareableItem != null)
		{
			this.rightBackShareableItem.ResetViews();
			ObjectPools.instance.Destroy(this.rightBackShareableItem.gameObject);
		}
		if (this.leftBackSharableItem != null)
		{
			this.leftBackSharableItem.ResetViews();
			ObjectPools.instance.Destroy(this.leftBackSharableItem.gameObject);
		}
		this.leftBackSharableItem = null;
		this.rightBackShareableItem = null;
	}

	// Token: 0x0600185B RID: 6235 RVA: 0x0007641B File Offset: 0x0007461B
	public static bool IsPositionLeft(BodyDockPositions.DropPositions pos)
	{
		return pos == BodyDockPositions.DropPositions.LeftArm || pos == BodyDockPositions.DropPositions.LeftBack;
	}

	// Token: 0x0600185C RID: 6236 RVA: 0x00076428 File Offset: 0x00074628
	public int DropZoneStorageUsed(BodyDockPositions.DropPositions dropPosition)
	{
		if (this.myRig == null)
		{
			Debug.Log("BodyDockPositions lost reference to VR Rig, resetting it now", this);
			this.myRig = base.GetComponent<VRRig>();
		}
		if (this.myRig == null)
		{
			Debug.Log("Unable to reset reference");
			return -1;
		}
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) >= 0 && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].gameObject.activeInHierarchy && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].storedZone == dropPosition)
			{
				return this.myRig.ActiveTransferrableObjectIndex(i);
			}
		}
		return -1;
	}

	// Token: 0x0600185D RID: 6237 RVA: 0x000764E4 File Offset: 0x000746E4
	public TransferrableObject ItemPositionInUse(BodyDockPositions.DropPositions dropPosition)
	{
		TransferrableObject.PositionState positionState = this.MapDropPositionToState(dropPosition);
		if (this.myRig == null)
		{
			Debug.Log("BodyDockPositions lost reference to VR Rig, resetting it now", this);
			this.myRig = base.GetComponent<VRRig>();
		}
		if (this.myRig == null)
		{
			Debug.Log("Unable to reset reference");
			return null;
		}
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) != -1 && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].gameObject.activeInHierarchy && this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)].currentState == positionState)
			{
				return this.allObjects[this.myRig.ActiveTransferrableObjectIndex(i)];
			}
		}
		return null;
	}

	// Token: 0x0600185E RID: 6238 RVA: 0x000765AC File Offset: 0x000747AC
	private int EnableTransferrableItem(int allItemsIndex, BodyDockPositions.DropPositions startingPosition, TransferrableObject.PositionState startingState)
	{
		if (allItemsIndex < 0 || allItemsIndex >= this.allObjects.Length)
		{
			return -1;
		}
		if (this.myRig != null && this.myRig.isOfflineVRRig)
		{
			for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(i) == allItemsIndex)
				{
					this.DisableTransferrableItem(allItemsIndex);
				}
			}
			for (int j = 0; j < this.myRig.ActiveTransferrableObjectIndexLength(); j++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(j) == -1)
				{
					string itemNameFromDisplayName = CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[allItemsIndex].gameObject.name);
					if (this.myRig.IsItemAllowed(itemNameFromDisplayName))
					{
						this.myRig.SetActiveTransferrableObjectIndex(j, allItemsIndex);
						this.myRig.SetTransferrablePosStates(j, startingState);
						this.myRig.SetTransferrableItemStates(j, (TransferrableObject.ItemStates)0);
						this.myRig.SetTransferrableDockPosition(j, startingPosition);
						this.EnableTransferrableGameObject(allItemsIndex, startingPosition, startingState);
						return j;
					}
				}
			}
		}
		return -1;
	}

	// Token: 0x0600185F RID: 6239 RVA: 0x000766AC File Offset: 0x000748AC
	public BodyDockPositions.DropPositions ItemActive(int allItemsIndex)
	{
		if (!this.allObjects[allItemsIndex].gameObject.activeSelf)
		{
			return BodyDockPositions.DropPositions.None;
		}
		return this.allObjects[allItemsIndex].storedZone;
	}

	// Token: 0x06001860 RID: 6240 RVA: 0x000766D4 File Offset: 0x000748D4
	public static BodyDockPositions.DropPositions OfflineItemActive(int allItemsIndex)
	{
		if (GorillaTagger.Instance == null || GorillaTagger.Instance.offlineVRRig == null)
		{
			return BodyDockPositions.DropPositions.None;
		}
		BodyDockPositions component = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>();
		if (component == null)
		{
			return BodyDockPositions.DropPositions.None;
		}
		if (!component.allObjects[allItemsIndex].gameObject.activeSelf)
		{
			return BodyDockPositions.DropPositions.None;
		}
		return component.allObjects[allItemsIndex].storedZone;
	}

	// Token: 0x06001861 RID: 6241 RVA: 0x00076744 File Offset: 0x00074944
	public void DisableTransferrableItem(int index)
	{
		TransferrableObject transferrableObject = this.allObjects[index];
		if (transferrableObject.gameObject.activeSelf)
		{
			transferrableObject.gameObject.Disable();
			transferrableObject.storedZone = BodyDockPositions.DropPositions.None;
		}
		if (this.myRig.isOfflineVRRig)
		{
			for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
			{
				if (this.myRig.ActiveTransferrableObjectIndex(i) == index)
				{
					this.myRig.SetActiveTransferrableObjectIndex(i, -1);
				}
			}
		}
	}

	// Token: 0x06001862 RID: 6242 RVA: 0x000767B8 File Offset: 0x000749B8
	public void DisableAllTransferableItems()
	{
		if (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			return;
		}
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			int num = this.myRig.ActiveTransferrableObjectIndex(i);
			if (num >= 0 && num < this.allObjects.Length)
			{
				TransferrableObject transferrableObject = this.allObjects[num];
				transferrableObject.gameObject.Disable();
				transferrableObject.storedZone = BodyDockPositions.DropPositions.None;
				this.myRig.SetActiveTransferrableObjectIndex(i, -1);
				this.myRig.SetTransferrableItemStates(i, (TransferrableObject.ItemStates)0);
				this.myRig.SetTransferrablePosStates(i, TransferrableObject.PositionState.None);
			}
		}
		this.DeallocateSharableInstances();
	}

	// Token: 0x06001863 RID: 6243 RVA: 0x00076845 File Offset: 0x00074A45
	private bool AllItemsIndexValid(int allItemsIndex)
	{
		return allItemsIndex != -1 && allItemsIndex < this.allObjects.Length;
	}

	// Token: 0x06001864 RID: 6244 RVA: 0x00076858 File Offset: 0x00074A58
	public bool PositionAvailable(int allItemIndex, BodyDockPositions.DropPositions startPos)
	{
		return (this.allObjects[allItemIndex].dockPositions & startPos) > BodyDockPositions.DropPositions.None;
	}

	// Token: 0x06001865 RID: 6245 RVA: 0x0007686C File Offset: 0x00074A6C
	public BodyDockPositions.DropPositions FirstAvailablePosition(int allItemIndex)
	{
		for (int i = 0; i < 5; i++)
		{
			BodyDockPositions.DropPositions dropPositions = (BodyDockPositions.DropPositions)(1 << i);
			if ((this.allObjects[allItemIndex].dockPositions & dropPositions) != BodyDockPositions.DropPositions.None)
			{
				return dropPositions;
			}
		}
		return BodyDockPositions.DropPositions.None;
	}

	// Token: 0x06001866 RID: 6246 RVA: 0x000768A0 File Offset: 0x00074AA0
	public int TransferrableItemDisable(int allItemsIndex)
	{
		if (BodyDockPositions.OfflineItemActive(allItemsIndex) != BodyDockPositions.DropPositions.None)
		{
			this.DisableTransferrableItem(allItemsIndex);
		}
		return 0;
	}

	// Token: 0x06001867 RID: 6247 RVA: 0x000768B4 File Offset: 0x00074AB4
	public void TransferrableItemDisableAtPosition(BodyDockPositions.DropPositions dropPositions)
	{
		int num = this.DropZoneStorageUsed(dropPositions);
		if (num >= 0)
		{
			this.TransferrableItemDisable(num);
		}
	}

	// Token: 0x06001868 RID: 6248 RVA: 0x000768D8 File Offset: 0x00074AD8
	public void TransferrableItemEnableAtPosition(string itemName, BodyDockPositions.DropPositions dropPosition)
	{
		if (this.DropZoneStorageUsed(dropPosition) >= 0)
		{
			return;
		}
		List<int> list = this.TransferrableObjectIndexFromName(itemName);
		if (list.Count == 0)
		{
			return;
		}
		TransferrableObject.PositionState positionState = this.MapDropPositionToState(dropPosition);
		if (list.Count == 1)
		{
			this.EnableTransferrableItem(list[0], dropPosition, positionState);
			return;
		}
		int num = (BodyDockPositions.IsPositionLeft(dropPosition) ? list[0] : list[1]);
		this.EnableTransferrableItem(num, dropPosition, positionState);
	}

	// Token: 0x06001869 RID: 6249 RVA: 0x00076948 File Offset: 0x00074B48
	public bool TransferrableItemActive(string transferrableItemName)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int num in list)
		{
			if (this.TransferrableItemActive(num))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600186A RID: 6250 RVA: 0x000769B4 File Offset: 0x00074BB4
	public bool TransferrableItemActiveAtPos(string transferrableItemName, BodyDockPositions.DropPositions dropPosition)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int num in list)
		{
			BodyDockPositions.DropPositions dropPositions = this.TransferrableItemPosition(num);
			if (dropPositions != BodyDockPositions.DropPositions.None && dropPositions == dropPosition)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600186B RID: 6251 RVA: 0x00076A28 File Offset: 0x00074C28
	public bool TransferrableItemActive(int allItemsIndex)
	{
		return this.ItemActive(allItemsIndex) > BodyDockPositions.DropPositions.None;
	}

	// Token: 0x0600186C RID: 6252 RVA: 0x00076A34 File Offset: 0x00074C34
	public TransferrableObject TransferrableItem(int allItemsIndex)
	{
		return this.allObjects[allItemsIndex];
	}

	// Token: 0x0600186D RID: 6253 RVA: 0x00076A3E File Offset: 0x00074C3E
	public BodyDockPositions.DropPositions TransferrableItemPosition(int allItemsIndex)
	{
		return this.ItemActive(allItemsIndex);
	}

	// Token: 0x0600186E RID: 6254 RVA: 0x00076A48 File Offset: 0x00074C48
	public bool DisableTransferrableItem(string transferrableItemName)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return false;
		}
		foreach (int num in list)
		{
			this.DisableTransferrableItem(num);
		}
		return true;
	}

	// Token: 0x0600186F RID: 6255 RVA: 0x00076AAC File Offset: 0x00074CAC
	public BodyDockPositions.DropPositions OppositePosition(BodyDockPositions.DropPositions pos)
	{
		if (pos == BodyDockPositions.DropPositions.LeftArm)
		{
			return BodyDockPositions.DropPositions.RightArm;
		}
		if (pos == BodyDockPositions.DropPositions.RightArm)
		{
			return BodyDockPositions.DropPositions.LeftArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftBack)
		{
			return BodyDockPositions.DropPositions.RightBack;
		}
		if (pos == BodyDockPositions.DropPositions.RightBack)
		{
			return BodyDockPositions.DropPositions.LeftBack;
		}
		return pos;
	}

	// Token: 0x06001870 RID: 6256 RVA: 0x00076ACC File Offset: 0x00074CCC
	public BodyDockPositions.DockingResult ToggleWithHandedness(string transferrableItemName, bool isLeftHand, bool bothHands)
	{
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return new BodyDockPositions.DockingResult();
		}
		if (!this.AllItemsIndexValid(list[0]))
		{
			return new BodyDockPositions.DockingResult();
		}
		BodyDockPositions.DropPositions dropPositions;
		if (isLeftHand)
		{
			dropPositions = (((this.allObjects[list[0]].dockPositions & BodyDockPositions.DropPositions.RightArm) != BodyDockPositions.DropPositions.None) ? BodyDockPositions.DropPositions.RightArm : BodyDockPositions.DropPositions.LeftBack);
		}
		else
		{
			dropPositions = (((this.allObjects[list[0]].dockPositions & BodyDockPositions.DropPositions.LeftArm) != BodyDockPositions.DropPositions.None) ? BodyDockPositions.DropPositions.LeftArm : BodyDockPositions.DropPositions.RightBack);
		}
		return this.ToggleTransferrableItem(transferrableItemName, dropPositions, bothHands);
	}

	// Token: 0x06001871 RID: 6257 RVA: 0x00076B4C File Offset: 0x00074D4C
	public BodyDockPositions.DockingResult ToggleTransferrableItem(string transferrableItemName, BodyDockPositions.DropPositions startingPos, bool bothHands)
	{
		BodyDockPositions.DockingResult dockingResult = new BodyDockPositions.DockingResult();
		List<int> list = this.TransferrableObjectIndexFromName(transferrableItemName);
		if (list.Count == 0)
		{
			return dockingResult;
		}
		if (bothHands && list.Count == 2)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int num = list[i];
				BodyDockPositions.DropPositions dropPositions = BodyDockPositions.OfflineItemActive(num);
				if (dropPositions != BodyDockPositions.DropPositions.None)
				{
					this.TransferrableItemDisable(num);
					dockingResult.positionsDisabled.Add(dropPositions);
				}
			}
			if (dockingResult.positionsDisabled.Count >= 1)
			{
				return dockingResult;
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			int num2 = list[j];
			BodyDockPositions.DropPositions dropPositions2 = startingPos;
			if (bothHands && j != 0)
			{
				dropPositions2 = this.OppositePosition(dropPositions2);
			}
			if (!this.PositionAvailable(num2, dropPositions2))
			{
				dropPositions2 = this.FirstAvailablePosition(num2);
				if (dropPositions2 == BodyDockPositions.DropPositions.None)
				{
					return dockingResult;
				}
			}
			if (BodyDockPositions.OfflineItemActive(num2) == dropPositions2)
			{
				this.TransferrableItemDisable(num2);
				dockingResult.positionsDisabled.Add(dropPositions2);
			}
			else
			{
				this.TransferrableItemDisableAtPosition(dropPositions2);
				dockingResult.dockedPosition.Add(dropPositions2);
				TransferrableObject.PositionState positionState = this.MapDropPositionToState(dropPositions2);
				if (this.TransferrableItemActive(num2))
				{
					BodyDockPositions.DropPositions dropPositions3 = this.TransferrableItemPosition(num2);
					dockingResult.positionsDisabled.Add(dropPositions3);
					this.MoveTransferableItem(num2, dropPositions2, positionState);
				}
				else
				{
					this.EnableTransferrableItem(num2, dropPositions2, positionState);
				}
			}
		}
		return dockingResult;
	}

	// Token: 0x06001872 RID: 6258 RVA: 0x00076C93 File Offset: 0x00074E93
	private void MoveTransferableItem(int allItemsIndex, BodyDockPositions.DropPositions newPosition, TransferrableObject.PositionState newPositionState)
	{
		this.allObjects[allItemsIndex].storedZone = newPosition;
		this.allObjects[allItemsIndex].currentState = newPositionState;
		this.allObjects[allItemsIndex].ResetToDefaultState();
	}

	// Token: 0x06001873 RID: 6259 RVA: 0x00076CC0 File Offset: 0x00074EC0
	public void EnableTransferrableGameObject(int allItemsIndex, BodyDockPositions.DropPositions dropZone, TransferrableObject.PositionState startingPosition)
	{
		GameObject gameObject = this.allObjects[allItemsIndex].gameObject;
		TransferrableObject component = gameObject.GetComponent<TransferrableObject>();
		if ((component.dockPositions & dropZone) == BodyDockPositions.DropPositions.None || !component.ValidateState(startingPosition))
		{
			gameObject.Disable();
			return;
		}
		this.MoveTransferableItem(allItemsIndex, dropZone, startingPosition);
		gameObject.SetActive(true);
		ProjectileWeapon component2;
		if ((component2 = gameObject.GetComponent<ProjectileWeapon>()) != null)
		{
			component2.enabled = true;
		}
	}

	// Token: 0x06001874 RID: 6260 RVA: 0x00076D24 File Offset: 0x00074F24
	public void RefreshTransferrableItems()
	{
		if (!this.myRig)
		{
			this.myRig = base.GetComponentInParent<VRRig>(true);
			if (!this.myRig)
			{
				Debug.LogError("BodyDockPositions.RefreshTransferrableItems: (should never happen) myRig is null and could not be found on same GameObject or parents. Path: " + base.transform.GetPathQ(), this);
			}
		}
		this.objectsToEnable.Clear();
		this.objectsToDisable.Clear();
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			bool flag = true;
			int num = this.myRig.ActiveTransferrableObjectIndex(i);
			if (num != -1)
			{
				if (num < 0 || num >= this.allObjects.Length)
				{
					Debug.LogError(string.Format("Transferrable object index {0} out of range, expected [0..{1})", num, this.allObjects.Length));
				}
				else if (this.myRig.IsItemAllowed(CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[num].gameObject.name)))
				{
					for (int j = 0; j < this.allObjects.Length; j++)
					{
						if (j == this.myRig.ActiveTransferrableObjectIndex(i) && this.allObjects[j].gameObject.activeSelf)
						{
							this.allObjects[j].objectIndex = i;
							flag = false;
						}
					}
					if (flag)
					{
						this.objectsToEnable.Add(i);
					}
				}
			}
		}
		for (int k = 0; k < this.allObjects.Length; k++)
		{
			if (this.allObjects[k] != null && this.allObjects[k].gameObject.activeSelf)
			{
				bool flag2 = true;
				for (int l = 0; l < this.myRig.ActiveTransferrableObjectIndexLength(); l++)
				{
					if (this.myRig.ActiveTransferrableObjectIndex(l) == k && this.myRig.IsItemAllowed(CosmeticsController.instance.GetItemNameFromDisplayName(this.allObjects[this.myRig.ActiveTransferrableObjectIndex(l)].gameObject.name)))
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					this.objectsToDisable.Add(k);
				}
			}
		}
		foreach (int num2 in this.objectsToDisable)
		{
			this.DisableTransferrableItem(num2);
		}
		foreach (int num3 in this.objectsToEnable)
		{
			this.EnableTransferrableGameObject(this.myRig.ActiveTransferrableObjectIndex(num3), this.myRig.TransferrableDockPosition(num3), this.myRig.TransferrablePosStates(num3));
		}
		this.UpdateHandState();
	}

	// Token: 0x06001875 RID: 6261 RVA: 0x00076FF0 File Offset: 0x000751F0
	public int ReturnTransferrableItemIndex(int allItemsIndex)
	{
		for (int i = 0; i < this.myRig.ActiveTransferrableObjectIndexLength(); i++)
		{
			if (this.myRig.ActiveTransferrableObjectIndex(i) == allItemsIndex)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06001876 RID: 6262 RVA: 0x00077028 File Offset: 0x00075228
	public List<int> TransferrableObjectIndexFromName(string transObjectName)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.allObjects.Length; i++)
		{
			if (!(this.allObjects[i] == null) && this.allObjects[i].gameObject.name == transObjectName)
			{
				list.Add(i);
			}
		}
		return list;
	}

	// Token: 0x06001877 RID: 6263 RVA: 0x00077080 File Offset: 0x00075280
	private TransferrableObject.PositionState MapDropPositionToState(BodyDockPositions.DropPositions pos)
	{
		if (pos == BodyDockPositions.DropPositions.RightArm)
		{
			return TransferrableObject.PositionState.OnRightArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftArm)
		{
			return TransferrableObject.PositionState.OnLeftArm;
		}
		if (pos == BodyDockPositions.DropPositions.LeftBack)
		{
			return TransferrableObject.PositionState.OnLeftShoulder;
		}
		if (pos == BodyDockPositions.DropPositions.RightBack)
		{
			return TransferrableObject.PositionState.OnRightShoulder;
		}
		return TransferrableObject.PositionState.OnChest;
	}

	// Token: 0x170002B7 RID: 695
	// (get) Token: 0x06001878 RID: 6264 RVA: 0x0007709F File Offset: 0x0007529F
	internal int PreviousLeftHandThrowableIndex
	{
		get
		{
			return this.throwableDisabledIndex[0];
		}
	}

	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x06001879 RID: 6265 RVA: 0x000770A9 File Offset: 0x000752A9
	internal int PreviousRightHandThrowableIndex
	{
		get
		{
			return this.throwableDisabledIndex[1];
		}
	}

	// Token: 0x170002B9 RID: 697
	// (get) Token: 0x0600187A RID: 6266 RVA: 0x000770B3 File Offset: 0x000752B3
	internal float PreviousLeftHandThrowableDisabledTime
	{
		get
		{
			return this.throwableDisabledTime[0];
		}
	}

	// Token: 0x170002BA RID: 698
	// (get) Token: 0x0600187B RID: 6267 RVA: 0x000770BD File Offset: 0x000752BD
	internal float PreviousRightHandThrowableDisabledTime
	{
		get
		{
			return this.throwableDisabledTime[1];
		}
	}

	// Token: 0x0600187C RID: 6268 RVA: 0x000770C8 File Offset: 0x000752C8
	private void UpdateHandState()
	{
		for (int i = 0; i < 2; i++)
		{
			GameObject[] array = ((i == 0) ? this.leftHandThrowables : this.rightHandThrowables);
			int num = ((i == 0) ? this.myRig.LeftThrowableProjectileIndex : this.myRig.RightThrowableProjectileIndex);
			for (int j = 0; j < array.Length; j++)
			{
				bool activeSelf = array[j].activeSelf;
				bool flag = j == num;
				array[j].SetActive(flag);
				if (activeSelf && !flag)
				{
					this.throwableDisabledIndex[i] = j;
					this.throwableDisabledTime[i] = Time.time + 0.02f;
				}
			}
		}
	}

	// Token: 0x0600187D RID: 6269 RVA: 0x00077157 File Offset: 0x00075357
	internal GameObject GetLeftHandThrowable()
	{
		return this.GetLeftHandThrowable(this.myRig.LeftThrowableProjectileIndex);
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x0007716A File Offset: 0x0007536A
	internal GameObject GetLeftHandThrowable(int throwableIndex)
	{
		if (throwableIndex < 0 || throwableIndex >= this.leftHandThrowables.Length)
		{
			throwableIndex = this.PreviousLeftHandThrowableIndex;
			if (throwableIndex < 0 || throwableIndex >= this.leftHandThrowables.Length || this.PreviousLeftHandThrowableDisabledTime < Time.time)
			{
				return null;
			}
		}
		return this.leftHandThrowables[throwableIndex];
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x000771A9 File Offset: 0x000753A9
	internal GameObject GetRightHandThrowable()
	{
		return this.GetRightHandThrowable(this.myRig.RightThrowableProjectileIndex);
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x000771BC File Offset: 0x000753BC
	internal GameObject GetRightHandThrowable(int throwableIndex)
	{
		if (throwableIndex < 0 || throwableIndex >= this.rightHandThrowables.Length)
		{
			throwableIndex = this.PreviousRightHandThrowableIndex;
			if (throwableIndex < 0 || throwableIndex >= this.rightHandThrowables.Length || this.PreviousRightHandThrowableDisabledTime < Time.time)
			{
				return null;
			}
		}
		return this.rightHandThrowables[throwableIndex];
	}

	// Token: 0x04001B35 RID: 6965
	public VRRig myRig;

	// Token: 0x04001B36 RID: 6966
	public GameObject[] leftHandThrowables;

	// Token: 0x04001B37 RID: 6967
	public GameObject[] rightHandThrowables;

	// Token: 0x04001B38 RID: 6968
	[FormerlySerializedAs("allObjects")]
	public TransferrableObject[] _allObjects;

	// Token: 0x04001B39 RID: 6969
	private List<int> objectsToEnable = new List<int>();

	// Token: 0x04001B3A RID: 6970
	private List<int> objectsToDisable = new List<int>();

	// Token: 0x04001B3B RID: 6971
	public Transform leftHandTransform;

	// Token: 0x04001B3C RID: 6972
	public Transform rightHandTransform;

	// Token: 0x04001B3D RID: 6973
	public Transform chestTransform;

	// Token: 0x04001B3E RID: 6974
	public Transform leftArmTransform;

	// Token: 0x04001B3F RID: 6975
	public Transform rightArmTransform;

	// Token: 0x04001B40 RID: 6976
	public Transform leftBackTransform;

	// Token: 0x04001B41 RID: 6977
	public Transform rightBackTransform;

	// Token: 0x04001B42 RID: 6978
	public WorldShareableItem leftBackSharableItem;

	// Token: 0x04001B43 RID: 6979
	public WorldShareableItem rightBackShareableItem;

	// Token: 0x04001B44 RID: 6980
	public GameObject SharableItemInstance;

	// Token: 0x04001B45 RID: 6981
	private int[] throwableDisabledIndex = new int[] { -1, -1 };

	// Token: 0x04001B46 RID: 6982
	private float[] throwableDisabledTime = new float[2];

	// Token: 0x020003F7 RID: 1015
	[Flags]
	public enum DropPositions
	{
		// Token: 0x04001B48 RID: 6984
		LeftArm = 1,
		// Token: 0x04001B49 RID: 6985
		RightArm = 2,
		// Token: 0x04001B4A RID: 6986
		Chest = 4,
		// Token: 0x04001B4B RID: 6987
		LeftBack = 8,
		// Token: 0x04001B4C RID: 6988
		RightBack = 16,
		// Token: 0x04001B4D RID: 6989
		MaxDropPostions = 5,
		// Token: 0x04001B4E RID: 6990
		All = 31,
		// Token: 0x04001B4F RID: 6991
		None = 0
	}

	// Token: 0x020003F8 RID: 1016
	public class DockingResult
	{
		// Token: 0x06001882 RID: 6274 RVA: 0x00077239 File Offset: 0x00075439
		public DockingResult()
		{
			this.dockedPosition = new List<BodyDockPositions.DropPositions>(2);
			this.positionsDisabled = new List<BodyDockPositions.DropPositions>(2);
		}

		// Token: 0x04001B50 RID: 6992
		public List<BodyDockPositions.DropPositions> positionsDisabled;

		// Token: 0x04001B51 RID: 6993
		public List<BodyDockPositions.DropPositions> dockedPosition;
	}
}
