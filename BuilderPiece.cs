using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x020004F5 RID: 1269
public class BuilderPiece : MonoBehaviour
{
	// Token: 0x06001EA4 RID: 7844 RVA: 0x0009574C File Offset: 0x0009394C
	private void Awake()
	{
		if (this.vFXInfo == null)
		{
			Debug.LogErrorFormat("BuilderPiece {0} is missing Effect Info", new object[] { base.gameObject.name });
		}
		this.materialType = -1;
		this.pieceType = -1;
		this.pieceId = -1;
		this.pieceDataIndex = -1;
		this.state = BuilderPiece.State.None;
		this.isStatic = true;
		this.parentPiece = null;
		this.firstChildPiece = null;
		this.nextSiblingPiece = null;
		this.attachIndex = -1;
		this.parentAttachIndex = -1;
		this.parentHeld = null;
		this.heldByPlayerActorNumber = -1;
		this.placedOnlyColliders = new List<Collider>(4);
		List<Collider> list = new List<Collider>(4);
		foreach (GameObject gameObject in this.onlyWhenPlaced)
		{
			list.Clear();
			gameObject.GetComponentsInChildren<Collider>(list);
			for (int i = 0; i < list.Count; i++)
			{
				if (!list[i].isTrigger)
				{
					BuilderPieceCollider builderPieceCollider = list[i].GetComponent<BuilderPieceCollider>();
					if (builderPieceCollider == null)
					{
						builderPieceCollider = list[i].AddComponent<BuilderPieceCollider>();
					}
					builderPieceCollider.piece = this;
					this.placedOnlyColliders.Add(list[i]);
				}
			}
		}
		this.SetActive(this.onlyWhenPlaced, false);
		this.SetActive(this.onlyWhenNotPlaced, true);
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		for (int j = this.colliders.Count - 1; j >= 0; j--)
		{
			if (this.colliders[j].isTrigger)
			{
				this.colliders.RemoveAt(j);
			}
			else
			{
				BuilderPieceCollider builderPieceCollider2 = this.colliders[j].GetComponent<BuilderPieceCollider>();
				if (builderPieceCollider2 == null)
				{
					builderPieceCollider2 = this.colliders[j].AddComponent<BuilderPieceCollider>();
				}
				builderPieceCollider2.piece = this;
			}
		}
		this.gridPlanes = new List<BuilderAttachGridPlane>(8);
		base.GetComponentsInChildren<BuilderAttachGridPlane>(this.gridPlanes);
		this.pieceComponents = new List<IBuilderPieceComponent>(1);
		base.GetComponentsInChildren<IBuilderPieceComponent>(true, this.pieceComponents);
		this.pieceComponentsActive = false;
		this.functionalPieceComponent = base.GetComponentInChildren<IBuilderPieceFunctional>();
		this.SetCollidersEnabled<Collider>(this.colliders, false);
		this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
		this.preventSnapUntilMoved = 0;
		this.preventSnapUntilMovedFromPos = Vector3.zero;
		this.renderingIndirect = new List<MeshRenderer>(4);
		this.paintingCount = 0;
		this.potentialGrabCount = 0;
		this.potentialGrabChildCount = 0;
		this.isPrivatePlot = this.plotComponent != null;
		this.privatePlotIndex = -1;
	}

	// Token: 0x06001EA5 RID: 7845 RVA: 0x000959F0 File Offset: 0x00093BF0
	public void SetTable(BuilderTable table)
	{
		this.tableOwner = table;
	}

	// Token: 0x06001EA6 RID: 7846 RVA: 0x000959F9 File Offset: 0x00093BF9
	public BuilderTable GetTable()
	{
		return this.tableOwner;
	}

	// Token: 0x06001EA7 RID: 7847 RVA: 0x00095A04 File Offset: 0x00093C04
	public void OnReturnToPool()
	{
		this.tableOwner.builderRenderer.RemovePiece(this);
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPieceDestroy();
		}
		this.functionalPieceState = 0;
		this.state = BuilderPiece.State.None;
		this.isStatic = true;
		this.materialType = -1;
		this.pieceType = -1;
		this.pieceId = -1;
		this.pieceDataIndex = -1;
		this.parentPiece = null;
		this.firstChildPiece = null;
		this.nextSiblingPiece = null;
		this.attachIndex = -1;
		this.parentAttachIndex = -1;
		this.overrideSavedPiece = false;
		this.savedMaterialType = -1;
		this.savedPieceType = -1;
		this.shelfOwner = -1;
		this.parentHeld = null;
		this.heldByPlayerActorNumber = -1;
		this.activatedTimeStamp = 0;
		this.SetActive(this.onlyWhenPlaced, false);
		this.SetActive(this.onlyWhenNotPlaced, true);
		this.SetCollidersEnabled<Collider>(this.colliders, false);
		this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
		this.preventSnapUntilMoved = 0;
		this.preventSnapUntilMovedFromPos = Vector3.zero;
		base.transform.localScale = Vector3.one;
		if (this.isArmShelf)
		{
			if (this.armShelf != null)
			{
				this.armShelf.piece = null;
			}
			this.armShelf = null;
		}
		for (int j = 0; j < this.gridPlanes.Count; j++)
		{
			this.gridPlanes[j].OnReturnToPool(this.tableOwner.builderPool);
		}
	}

	// Token: 0x06001EA8 RID: 7848 RVA: 0x00095B7F File Offset: 0x00093D7F
	public void OnCreatedByPool()
	{
		this.materialSwapTargets = new List<MeshRenderer>(4);
		base.GetComponentsInChildren<MeshRenderer>(this.areMeshesToggledOnPlace, this.materialSwapTargets);
		this.surfaceOverrides = new List<GorillaSurfaceOverride>(4);
		base.GetComponentsInChildren<GorillaSurfaceOverride>(this.areMeshesToggledOnPlace, this.surfaceOverrides);
	}

	// Token: 0x06001EA9 RID: 7849 RVA: 0x00095BC0 File Offset: 0x00093DC0
	public void SetupPiece(float gridSize)
	{
		for (int i = 0; i < this.gridPlanes.Count; i++)
		{
			this.gridPlanes[i].Setup(this, i, gridSize);
		}
	}

	// Token: 0x06001EAA RID: 7850 RVA: 0x00095BF8 File Offset: 0x00093DF8
	public void SetMaterial(int inMaterialType, bool force = false)
	{
		if (this.materialOptions == null || this.materialSwapTargets == null || this.materialSwapTargets.Count < 1)
		{
			return;
		}
		if (this.materialType == inMaterialType && !force)
		{
			return;
		}
		this.materialType = inMaterialType;
		Material material = null;
		int num = -1;
		if (inMaterialType == -1)
		{
			this.materialOptions.GetDefaultMaterial(out this.materialType, out material, out num);
		}
		else
		{
			this.materialOptions.GetMaterialFromType(this.materialType, out material, out num);
			if (material == null)
			{
				this.materialOptions.GetDefaultMaterial(out this.materialType, out material, out num);
			}
		}
		if (material == null)
		{
			Debug.LogErrorFormat("Piece {0} has no material matching Type {1}", new object[]
			{
				this.GetPieceId(),
				inMaterialType
			});
			return;
		}
		foreach (MeshRenderer meshRenderer in this.materialSwapTargets)
		{
			if (!(meshRenderer == null) && meshRenderer.enabled)
			{
				meshRenderer.material = material;
			}
		}
		if (this.surfaceOverrides != null && num != -1)
		{
			foreach (GorillaSurfaceOverride gorillaSurfaceOverride in this.surfaceOverrides)
			{
				gorillaSurfaceOverride.overrideIndex = num;
			}
		}
		if (this.renderingIndirect.Count > 0)
		{
			this.tableOwner.builderRenderer.ChangePieceIndirectMaterial(this, this.materialSwapTargets, material);
		}
	}

	// Token: 0x06001EAB RID: 7851 RVA: 0x00095D8C File Offset: 0x00093F8C
	public int GetPieceId()
	{
		return this.pieceId;
	}

	// Token: 0x06001EAC RID: 7852 RVA: 0x00095D94 File Offset: 0x00093F94
	public int GetParentPieceId()
	{
		if (!(this.parentPiece == null))
		{
			return this.parentPiece.pieceId;
		}
		return -1;
	}

	// Token: 0x06001EAD RID: 7853 RVA: 0x00095DB1 File Offset: 0x00093FB1
	public int GetAttachIndex()
	{
		return this.attachIndex;
	}

	// Token: 0x06001EAE RID: 7854 RVA: 0x00095DB9 File Offset: 0x00093FB9
	public int GetParentAttachIndex()
	{
		return this.parentAttachIndex;
	}

	// Token: 0x06001EAF RID: 7855 RVA: 0x00095DC4 File Offset: 0x00093FC4
	private void SetPieceActive(List<IBuilderPieceComponent> components, bool active)
	{
		if (components == null || active == this.pieceComponentsActive)
		{
			return;
		}
		this.pieceComponentsActive = active;
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				if (active)
				{
					components[i].OnPieceActivate();
				}
				else
				{
					components[i].OnPieceDeactivate();
				}
			}
		}
	}

	// Token: 0x06001EB0 RID: 7856 RVA: 0x00095E1C File Offset: 0x0009401C
	private void SetBehavioursEnabled<T>(List<T> components, bool enabled) where T : Behaviour
	{
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].enabled = enabled;
			}
		}
	}

	// Token: 0x06001EB1 RID: 7857 RVA: 0x00095E64 File Offset: 0x00094064
	private void SetCollidersEnabled<T>(List<T> components, bool enabled) where T : Collider
	{
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].enabled = enabled;
			}
		}
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x00095EAC File Offset: 0x000940AC
	public void SetColliderLayers<T>(List<T> components, int layer) where T : Collider
	{
		if (components == null)
		{
			return;
		}
		for (int i = 0; i < components.Count; i++)
		{
			if (components[i] != null)
			{
				components[i].gameObject.layer = layer;
			}
		}
	}

	// Token: 0x06001EB3 RID: 7859 RVA: 0x00095EFC File Offset: 0x000940FC
	private void SetActive(List<GameObject> gameObjects, bool active)
	{
		if (gameObjects == null)
		{
			return;
		}
		for (int i = 0; i < gameObjects.Count; i++)
		{
			if (gameObjects[i] != null)
			{
				gameObjects[i].SetActive(active);
			}
		}
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x00095F3A File Offset: 0x0009413A
	public void SetFunctionalPieceState(byte fState, NetPlayer instigator, int timeStamp)
	{
		if (this.functionalPieceComponent == null || !this.functionalPieceComponent.IsStateValid(fState))
		{
			fState = 0;
		}
		this.functionalPieceState = fState;
		IBuilderPieceFunctional builderPieceFunctional = this.functionalPieceComponent;
		if (builderPieceFunctional == null)
		{
			return;
		}
		builderPieceFunctional.OnStateChanged(fState, instigator, timeStamp);
	}

	// Token: 0x06001EB5 RID: 7861 RVA: 0x00095F6F File Offset: 0x0009416F
	public void SetScale(float scale)
	{
		if (this.scaleRoot != null)
		{
			this.scaleRoot.localScale = Vector3.one * scale;
		}
		this.pieceScale = scale;
	}

	// Token: 0x06001EB6 RID: 7862 RVA: 0x00095F9C File Offset: 0x0009419C
	public float GetScale()
	{
		return this.pieceScale;
	}

	// Token: 0x06001EB7 RID: 7863 RVA: 0x00095FA4 File Offset: 0x000941A4
	public void PaintingTint(bool enable)
	{
		if (enable)
		{
			this.paintingCount++;
			if (this.paintingCount == 1)
			{
				this.RefreshTint();
				return;
			}
		}
		else
		{
			this.paintingCount--;
			if (this.paintingCount == 0)
			{
				this.RefreshTint();
			}
		}
	}

	// Token: 0x06001EB8 RID: 7864 RVA: 0x00095FE4 File Offset: 0x000941E4
	public void PotentialGrab(bool enable)
	{
		if (enable)
		{
			this.potentialGrabCount++;
			if (this.potentialGrabCount == 1 && this.potentialGrabChildCount == 0)
			{
				this.RefreshTint();
				return;
			}
		}
		else
		{
			this.potentialGrabCount--;
			if (this.potentialGrabCount == 0 && this.potentialGrabChildCount == 0)
			{
				this.RefreshTint();
			}
		}
	}

	// Token: 0x06001EB9 RID: 7865 RVA: 0x00096040 File Offset: 0x00094240
	public static void PotentialGrabChildren(BuilderPiece piece, bool enable)
	{
		BuilderPiece builderPiece = piece.firstChildPiece;
		while (builderPiece != null)
		{
			if (enable)
			{
				builderPiece.potentialGrabChildCount++;
				if (builderPiece.potentialGrabChildCount == 1 && builderPiece.potentialGrabCount == 0)
				{
					builderPiece.RefreshTint();
				}
			}
			else
			{
				builderPiece.potentialGrabChildCount--;
				if (builderPiece.potentialGrabChildCount == 0 && builderPiece.potentialGrabCount == 0)
				{
					builderPiece.RefreshTint();
				}
			}
			BuilderPiece.PotentialGrabChildren(builderPiece, enable);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06001EBA RID: 7866 RVA: 0x000960BC File Offset: 0x000942BC
	private void RefreshTint()
	{
		if (this.potentialGrabCount > 0 || this.potentialGrabChildCount > 0)
		{
			this.SetTint(this.tableOwner.potentialGrabTint);
			return;
		}
		if (this.paintingCount > 0)
		{
			this.SetTint(this.tableOwner.paintingTint);
			return;
		}
		switch (this.state)
		{
		case BuilderPiece.State.AttachedToDropped:
		case BuilderPiece.State.Dropped:
			this.SetTint(this.tableOwner.droppedTint);
			return;
		case BuilderPiece.State.Grabbed:
		case BuilderPiece.State.GrabbedLocal:
		case BuilderPiece.State.AttachedToArm:
			this.SetTint(this.tableOwner.grabbedTint);
			return;
		case BuilderPiece.State.OnShelf:
		case BuilderPiece.State.OnConveyor:
			this.SetTint(this.tableOwner.shelfTint);
			return;
		}
		this.SetTint(this.tableOwner.defaultTint);
	}

	// Token: 0x06001EBB RID: 7867 RVA: 0x00096180 File Offset: 0x00094380
	private void SetTint(float tint)
	{
		if (tint == this.tint)
		{
			return;
		}
		this.tint = tint;
		this.tableOwner.builderRenderer.SetPieceTint(this, tint);
	}

	// Token: 0x06001EBC RID: 7868 RVA: 0x000961A8 File Offset: 0x000943A8
	public void SetParentPiece(int newAttachIndex, BuilderPiece newParentPiece, int newParentAttachIndex)
	{
		if (this.parentHeld != null)
		{
			Debug.LogErrorFormat(newParentPiece.gameObject, "Cannot attach to piece {0} while already held", new object[] { (newParentPiece == null) ? null : newParentPiece.gameObject.name });
			return;
		}
		BuilderPiece.RemovePieceFromParent(this);
		this.attachIndex = newAttachIndex;
		this.parentPiece = newParentPiece;
		this.parentAttachIndex = newParentAttachIndex;
		this.AddPieceToParent(this);
		Transform transform = null;
		if (newParentPiece != null)
		{
			if (newParentAttachIndex >= 0)
			{
				transform = newParentPiece.gridPlanes[newParentAttachIndex].transform;
			}
			else
			{
				transform = newParentPiece.transform;
			}
		}
		base.transform.SetParent(transform, true);
		this.requestedParentPiece = null;
		this.tableOwner.UpdatePieceData(this);
	}

	// Token: 0x06001EBD RID: 7869 RVA: 0x00096260 File Offset: 0x00094460
	public void ClearParentPiece(bool ignoreSnaps = false)
	{
		if (this.parentPiece == null)
		{
			if (!ignoreSnaps)
			{
				BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(this, this, this.tableOwner.builderPool);
			}
			return;
		}
		BuilderPiece builderPiece = this.parentPiece;
		BuilderPiece.RemovePieceFromParent(this);
		this.attachIndex = -1;
		this.parentPiece = null;
		this.parentAttachIndex = -1;
		base.transform.SetParent(null, true);
		this.requestedParentPiece = null;
		this.tableOwner.UpdatePieceData(this);
		if (!ignoreSnaps)
		{
			BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(this, this.GetRootPiece(), this.tableOwner.builderPool);
		}
	}

	// Token: 0x06001EBE RID: 7870 RVA: 0x000962F0 File Offset: 0x000944F0
	public static void RemoveOverlapsWithDifferentPieceRoot(BuilderPiece piece, BuilderPiece root, BuilderPool pool)
	{
		for (int i = 0; i < piece.gridPlanes.Count; i++)
		{
			piece.gridPlanes[i].RemoveSnapsWithDifferentRoot(root, pool);
		}
		BuilderPiece builderPiece = piece.firstChildPiece;
		while (builderPiece != null)
		{
			BuilderPiece.RemoveOverlapsWithDifferentPieceRoot(builderPiece, root, pool);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06001EBF RID: 7871 RVA: 0x00096348 File Offset: 0x00094548
	private void AddPieceToParent(BuilderPiece piece)
	{
		BuilderPiece builderPiece = piece.parentPiece;
		if (builderPiece == null)
		{
			return;
		}
		this.nextSiblingPiece = builderPiece.firstChildPiece;
		builderPiece.firstChildPiece = piece;
		if (piece.parentAttachIndex >= 0 && piece.parentAttachIndex < builderPiece.gridPlanes.Count)
		{
			builderPiece.gridPlanes[piece.parentAttachIndex].ChangeChildPieceCount(1 + piece.GetChildCount());
		}
	}

	// Token: 0x06001EC0 RID: 7872 RVA: 0x000963B4 File Offset: 0x000945B4
	private static void RemovePieceFromParent(BuilderPiece piece)
	{
		BuilderPiece builderPiece = piece.parentPiece;
		if (builderPiece == null)
		{
			return;
		}
		BuilderPiece builderPiece2 = builderPiece.firstChildPiece;
		if (builderPiece2 == null)
		{
			Debug.LogErrorFormat("Parent {0} of piece {1} doesn't have any children", new object[] { builderPiece.name, piece.name });
		}
		bool flag = false;
		if (builderPiece2 == piece)
		{
			builderPiece.firstChildPiece = builderPiece2.nextSiblingPiece;
			flag = true;
		}
		else
		{
			while (builderPiece2 != null)
			{
				if (builderPiece2.nextSiblingPiece == piece)
				{
					builderPiece2.nextSiblingPiece = piece.nextSiblingPiece;
					piece.nextSiblingPiece = null;
					flag = true;
					break;
				}
				builderPiece2 = builderPiece2.nextSiblingPiece;
			}
		}
		if (!flag)
		{
			Debug.LogErrorFormat("Parent {0} of piece {1} doesn't have the piece a child", new object[] { builderPiece.name, piece.name });
			return;
		}
		if (piece.parentAttachIndex >= 0 && piece.parentAttachIndex < builderPiece.gridPlanes.Count)
		{
			builderPiece.gridPlanes[piece.parentAttachIndex].ChangeChildPieceCount(-1 * (1 + piece.GetChildCount()));
		}
	}

	// Token: 0x06001EC1 RID: 7873 RVA: 0x000964B8 File Offset: 0x000946B8
	public void SetParentHeld(Transform parentHeld, int heldByPlayerActorNumber, bool heldInLeftHand)
	{
		if (this.parentPiece != null)
		{
			Debug.LogErrorFormat(this.parentPiece.gameObject, "Cannot hold while already attached to piece {0}", new object[] { this.parentPiece.gameObject.name });
			return;
		}
		this.heldByPlayerActorNumber = heldByPlayerActorNumber;
		this.parentHeld = parentHeld;
		this.heldInLeftHand = heldInLeftHand;
		base.transform.SetParent(parentHeld);
		this.tableOwner.UpdatePieceData(this);
	}

	// Token: 0x06001EC2 RID: 7874 RVA: 0x00096530 File Offset: 0x00094730
	public void ClearParentHeld()
	{
		if (this.parentHeld == null)
		{
			return;
		}
		if (this.isArmShelf && this.armShelf != null)
		{
			this.armShelf.piece = null;
			this.armShelf = null;
		}
		this.heldByPlayerActorNumber = -1;
		this.parentHeld = null;
		this.heldInLeftHand = false;
		base.transform.SetParent(this.parentHeld);
		this.tableOwner.UpdatePieceData(this);
	}

	// Token: 0x06001EC3 RID: 7875 RVA: 0x000965A7 File Offset: 0x000947A7
	public bool IsHeldLocal()
	{
		return this.heldByPlayerActorNumber != -1 && this.heldByPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06001EC4 RID: 7876 RVA: 0x000965C6 File Offset: 0x000947C6
	public bool IsHeldBy(int actorNumber)
	{
		return actorNumber != -1 && this.heldByPlayerActorNumber == actorNumber;
	}

	// Token: 0x06001EC5 RID: 7877 RVA: 0x000965D7 File Offset: 0x000947D7
	public bool IsHeldInLeftHand()
	{
		return this.heldInLeftHand;
	}

	// Token: 0x06001EC6 RID: 7878 RVA: 0x000965DF File Offset: 0x000947DF
	public static bool IsDroppedState(BuilderPiece.State state)
	{
		return state == BuilderPiece.State.Dropped || state == BuilderPiece.State.AttachedToDropped || state == BuilderPiece.State.OnShelf || state == BuilderPiece.State.OnConveyor;
	}

	// Token: 0x06001EC7 RID: 7879 RVA: 0x000965F4 File Offset: 0x000947F4
	public void SetActivateTimeStamp(int timeStamp)
	{
		this.activatedTimeStamp = timeStamp;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetActivateTimeStamp(timeStamp);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06001EC8 RID: 7880 RVA: 0x00096628 File Offset: 0x00094828
	public void SetState(BuilderPiece.State newState, bool force = false)
	{
		if (newState == this.state && !force)
		{
			return;
		}
		if (newState == BuilderPiece.State.Dropped && this.state != BuilderPiece.State.Dropped)
		{
			this.tableOwner.AddPieceToDropList(this);
		}
		else if (this.state == BuilderPiece.State.Dropped && newState != BuilderPiece.State.Dropped)
		{
			this.tableOwner.RemovePieceFromDropList(this);
		}
		BuilderPiece.State state = this.state;
		this.state = newState;
		if (this.pieceDataIndex >= 0)
		{
			this.tableOwner.UpdatePieceData(this);
		}
		switch (this.state)
		{
		case BuilderPiece.State.None:
			this.SetCollidersEnabled<Collider>(this.colliders, false);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, false);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.None, force);
			this.tableOwner.builderRenderer.RemovePiece(this);
			this.isStatic = true;
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedAndPlaced:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, true);
			this.SetActive(this.onlyWhenPlaced, true);
			this.SetActive(this.onlyWhenNotPlaced, false);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.placedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedAndPlaced, force);
			this.SetStatic(false, force || this.areMeshesToggledOnPlace);
			this.SetPieceActive(this.pieceComponents, true);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedToDropped:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedToDropped, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.Grabbed:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.heldLayer);
			this.SetChildrenState(BuilderPiece.State.Grabbed, force);
			this.SetStatic(false, force || (this.areMeshesToggledOnPlace && state == BuilderPiece.State.AttachedAndPlaced));
			this.SetPieceActive(this.pieceComponents, false);
			this.SetActivateTimeStamp(0);
			this.RefreshTint();
			return;
		case BuilderPiece.State.Dropped:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(false, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.AttachedToDropped, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.OnShelf:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.OnShelf, force);
			this.SetStatic(true, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.Displayed:
			this.SetCollidersEnabled<Collider>(this.colliders, false);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetChildrenState(BuilderPiece.State.Displayed, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.GrabbedLocal:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.heldLayerLocal);
			this.SetChildrenState(BuilderPiece.State.GrabbedLocal, force);
			this.SetStatic(false, force || (this.areMeshesToggledOnPlace && state == BuilderPiece.State.AttachedAndPlaced));
			this.SetPieceActive(this.pieceComponents, false);
			this.SetActivateTimeStamp(0);
			this.RefreshTint();
			return;
		case BuilderPiece.State.OnConveyor:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.droppedLayer);
			this.SetChildrenState(BuilderPiece.State.OnConveyor, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		case BuilderPiece.State.AttachedToArm:
			this.SetCollidersEnabled<Collider>(this.colliders, true);
			this.SetBehavioursEnabled<Behaviour>(this.onlyWhenPlacedBehaviours, false);
			this.SetActive(this.onlyWhenPlaced, false);
			this.SetActive(this.onlyWhenNotPlaced, true);
			this.SetKinematic(true, true);
			this.SetColliderLayers<Collider>(this.colliders, BuilderTable.heldLayerLocal);
			this.SetChildrenState(BuilderPiece.State.AttachedToArm, force);
			this.SetStatic(false, force);
			this.SetPieceActive(this.pieceComponents, false);
			this.RefreshTint();
			return;
		default:
			return;
		}
	}

	// Token: 0x06001EC9 RID: 7881 RVA: 0x00096B7C File Offset: 0x00094D7C
	public void SetKinematic(bool kinematic, bool destroyImmediate = true)
	{
		if (kinematic && this.rigidBody != null)
		{
			if (destroyImmediate)
			{
				Object.DestroyImmediate(this.rigidBody);
				this.rigidBody = null;
			}
			else
			{
				Object.Destroy(this.rigidBody);
				this.rigidBody = null;
			}
		}
		else if (!kinematic && this.rigidBody == null)
		{
			this.rigidBody = base.gameObject.GetComponent<Rigidbody>();
			if (this.rigidBody != null)
			{
				Debug.LogErrorFormat("We should never already have a rigid body here {0} {1}", new object[] { this.pieceId, this.pieceType });
			}
			if (this.rigidBody == null)
			{
				this.rigidBody = base.gameObject.AddComponent<Rigidbody>();
			}
			if (this.rigidBody != null)
			{
				this.rigidBody.isKinematic = kinematic;
			}
		}
		if (this.rigidBody != null)
		{
			this.rigidBody.mass = 1f;
		}
	}

	// Token: 0x06001ECA RID: 7882 RVA: 0x000023F4 File Offset: 0x000005F4
	public void SetStatic(bool isStatic, bool force = false)
	{
	}

	// Token: 0x06001ECB RID: 7883 RVA: 0x00096C84 File Offset: 0x00094E84
	private void SetChildrenState(BuilderPiece.State newState, bool force)
	{
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			builderPiece.SetState(newState, force);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06001ECC RID: 7884 RVA: 0x00096CB4 File Offset: 0x00094EB4
	public void OnCreate()
	{
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPieceCreate(this.pieceType, this.pieceId);
		}
	}

	// Token: 0x06001ECD RID: 7885 RVA: 0x00096CF4 File Offset: 0x00094EF4
	public void OnPlacementDeserialized()
	{
		for (int i = 0; i < this.pieceComponents.Count; i++)
		{
			this.pieceComponents[i].OnPiecePlacementDeserialized();
		}
	}

	// Token: 0x06001ECE RID: 7886 RVA: 0x00096D28 File Offset: 0x00094F28
	public void PlayPlacementFx()
	{
		this.PlayVFX(this.vFXInfo.placeVFX);
	}

	// Token: 0x06001ECF RID: 7887 RVA: 0x00096D3B File Offset: 0x00094F3B
	public void PlayDisconnectFx()
	{
		this.PlayVFX(this.vFXInfo.disconnectVFX);
	}

	// Token: 0x06001ED0 RID: 7888 RVA: 0x00096D4E File Offset: 0x00094F4E
	public void PlayGrabbedFx()
	{
		this.PlayVFX(this.vFXInfo.grabbedVFX);
	}

	// Token: 0x06001ED1 RID: 7889 RVA: 0x00096D61 File Offset: 0x00094F61
	public void PlayTooHeavyFx()
	{
		this.PlayVFX(this.vFXInfo.tooHeavyVFX);
	}

	// Token: 0x06001ED2 RID: 7890 RVA: 0x00096D74 File Offset: 0x00094F74
	public void PlayLocationLockFx()
	{
		this.PlayVFX(this.vFXInfo.locationLockVFX);
	}

	// Token: 0x06001ED3 RID: 7891 RVA: 0x00096D87 File Offset: 0x00094F87
	public void PlayRecycleFx()
	{
		this.PlayVFX(this.vFXInfo.recycleVFX);
	}

	// Token: 0x06001ED4 RID: 7892 RVA: 0x00096D9A File Offset: 0x00094F9A
	private void PlayVFX(GameObject vfx)
	{
		ObjectPools.instance.Instantiate(vfx, base.transform.position, true);
	}

	// Token: 0x06001ED5 RID: 7893 RVA: 0x00096DB4 File Offset: 0x00094FB4
	public static BuilderPiece GetBuilderPieceFromCollider(Collider collider)
	{
		if (collider == null)
		{
			return null;
		}
		BuilderPieceCollider component = collider.GetComponent<BuilderPieceCollider>();
		if (!(component == null))
		{
			return component.piece;
		}
		return null;
	}

	// Token: 0x06001ED6 RID: 7894 RVA: 0x00096DE4 File Offset: 0x00094FE4
	public static BuilderPiece GetBuilderPieceFromTransform(Transform transform)
	{
		while (transform != null)
		{
			BuilderPiece component = transform.GetComponent<BuilderPiece>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x06001ED7 RID: 7895 RVA: 0x00096E18 File Offset: 0x00095018
	public static void MakePieceRoot(BuilderPiece piece)
	{
		if (piece == null)
		{
			return;
		}
		if (piece.parentPiece == null || piece.parentPiece.isBuiltIntoTable)
		{
			return;
		}
		BuilderPiece.MakePieceRoot(piece.parentPiece);
		int num = piece.parentAttachIndex;
		int num2 = piece.attachIndex;
		BuilderPiece builderPiece = piece.parentPiece;
		bool flag = true;
		piece.ClearParentPiece(flag);
		builderPiece.SetParentPiece(num, piece, num2);
	}

	// Token: 0x06001ED8 RID: 7896 RVA: 0x00096E7C File Offset: 0x0009507C
	public BuilderPiece GetRootPiece()
	{
		BuilderPiece builderPiece = this;
		while (builderPiece.parentPiece != null && !builderPiece.parentPiece.isBuiltIntoTable)
		{
			builderPiece = builderPiece.parentPiece;
		}
		return builderPiece;
	}

	// Token: 0x06001ED9 RID: 7897 RVA: 0x00096EB0 File Offset: 0x000950B0
	public bool IsPrivatePlot()
	{
		return this.isPrivatePlot;
	}

	// Token: 0x06001EDA RID: 7898 RVA: 0x00096EB8 File Offset: 0x000950B8
	public bool TryGetPlotComponent(out BuilderPiecePrivatePlot plot)
	{
		plot = this.plotComponent;
		return this.isPrivatePlot;
	}

	// Token: 0x06001EDB RID: 7899 RVA: 0x00096ED0 File Offset: 0x000950D0
	public static bool CanPlayerAttachPieceToPiece(int playerActorNumber, BuilderPiece attachingPiece, BuilderPiece attachToPiece)
	{
		if (attachToPiece.state != BuilderPiece.State.AttachedAndPlaced && !attachToPiece.IsPrivatePlot() && attachToPiece.state != BuilderPiece.State.AttachedToArm)
		{
			return true;
		}
		BuilderPiece attachedBuiltInPiece = attachToPiece.GetAttachedBuiltInPiece();
		if (attachedBuiltInPiece == null || (!attachedBuiltInPiece.isPrivatePlot && !attachedBuiltInPiece.isArmShelf))
		{
			return true;
		}
		if (attachedBuiltInPiece.isArmShelf)
		{
			return attachedBuiltInPiece.heldByPlayerActorNumber == playerActorNumber && attachedBuiltInPiece.armShelf != null && attachedBuiltInPiece.armShelf.CanAttachToArmPiece();
		}
		BuilderPiecePrivatePlot builderPiecePrivatePlot;
		return !attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot) || (builderPiecePrivatePlot.CanPlayerAttachToPlot(playerActorNumber) && builderPiecePrivatePlot.IsChainUnderCapacity(attachingPiece));
	}

	// Token: 0x06001EDC RID: 7900 RVA: 0x00096F68 File Offset: 0x00095168
	public bool CanPlayerGrabPiece(int actorNumber, Vector3 worldPosition)
	{
		if (this.state != BuilderPiece.State.AttachedAndPlaced && !this.isPrivatePlot)
		{
			return true;
		}
		BuilderPiece attachedBuiltInPiece = this.GetAttachedBuiltInPiece();
		BuilderPiecePrivatePlot builderPiecePrivatePlot;
		return attachedBuiltInPiece == null || !attachedBuiltInPiece.isPrivatePlot || !attachedBuiltInPiece.TryGetPlotComponent(out builderPiecePrivatePlot) || builderPiecePrivatePlot.CanPlayerGrabFromPlot(actorNumber, worldPosition) || this.tableOwner.IsLocationWithinSharedBuildArea(worldPosition);
	}

	// Token: 0x06001EDD RID: 7901 RVA: 0x00096FC8 File Offset: 0x000951C8
	public bool IsPieceMoving()
	{
		if (this.state != BuilderPiece.State.AttachedAndPlaced)
		{
			return false;
		}
		if (this.attachIndex < 0 || this.attachIndex >= this.gridPlanes.Count)
		{
			return false;
		}
		if (this.gridPlanes[this.attachIndex].IsAttachedToMovingGrid())
		{
			return true;
		}
		using (List<BuilderAttachGridPlane>.Enumerator enumerator = this.gridPlanes.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.isMoving)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001EDE RID: 7902 RVA: 0x00097064 File Offset: 0x00095264
	public BuilderPiece GetAttachedBuiltInPiece()
	{
		if (this.isBuiltIntoTable)
		{
			return this;
		}
		if (this.state != BuilderPiece.State.AttachedAndPlaced)
		{
			return null;
		}
		BuilderPiece rootPiece = this.GetRootPiece();
		if (rootPiece.parentPiece != null)
		{
			rootPiece = rootPiece.parentPiece;
		}
		if (rootPiece.isBuiltIntoTable)
		{
			return rootPiece;
		}
		return null;
	}

	// Token: 0x06001EDF RID: 7903 RVA: 0x000970AC File Offset: 0x000952AC
	public int GetChainCostAndCount(int[] costArray)
	{
		for (int i = 0; i < costArray.Length; i++)
		{
			costArray[i] = 0;
		}
		foreach (BuilderResourceQuantity builderResourceQuantity in this.cost.quantities)
		{
			if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
			{
				costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
			}
		}
		return 1 + this.GetChildCountAndCost(costArray);
	}

	// Token: 0x06001EE0 RID: 7904 RVA: 0x00097140 File Offset: 0x00095340
	public int GetChildCountAndCost(int[] costArray)
	{
		int num = 0;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			num++;
			foreach (BuilderResourceQuantity builderResourceQuantity in builderPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			num += builderPiece.GetChildCountAndCost(costArray);
			builderPiece = builderPiece.nextSiblingPiece;
		}
		return num;
	}

	// Token: 0x06001EE1 RID: 7905 RVA: 0x000971E4 File Offset: 0x000953E4
	public int GetChildCount()
	{
		int num = 0;
		foreach (BuilderAttachGridPlane builderAttachGridPlane in this.gridPlanes)
		{
			num += builderAttachGridPlane.GetChildCount();
		}
		return num;
	}

	// Token: 0x06001EE2 RID: 7906 RVA: 0x0009723C File Offset: 0x0009543C
	public void GetChainCost(int[] costArray)
	{
		for (int i = 0; i < costArray.Length; i++)
		{
			costArray[i] = 0;
		}
		foreach (BuilderResourceQuantity builderResourceQuantity in this.cost.quantities)
		{
			if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
			{
				costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
			}
		}
		this.AddChildCost(costArray);
	}

	// Token: 0x06001EE3 RID: 7907 RVA: 0x000972D0 File Offset: 0x000954D0
	public void AddChildCost(int[] costArray)
	{
		int num = 0;
		BuilderPiece builderPiece = this.firstChildPiece;
		while (builderPiece != null)
		{
			num++;
			foreach (BuilderResourceQuantity builderResourceQuantity in builderPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					costArray[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			builderPiece.AddChildCost(costArray);
			builderPiece = builderPiece.nextSiblingPiece;
		}
	}

	// Token: 0x06001EE4 RID: 7908 RVA: 0x00097370 File Offset: 0x00095570
	public void BumpTwistToPositionRotation(byte twist, sbyte xOffset, sbyte zOffset, int potentialAttachIndex, BuilderAttachGridPlane potentialParentGridPlane, out Vector3 localPosition, out Quaternion localRotation, out Vector3 worldPosition, out Quaternion worldRotation)
	{
		float gridSize = this.tableOwner.gridSize;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[potentialAttachIndex];
		bool flag = (long)(twist % 2) == 1L;
		Transform center = potentialParentGridPlane.center;
		Vector3 position = center.position;
		Quaternion rotation = center.rotation;
		float num = (flag ? builderAttachGridPlane.lengthOffset : builderAttachGridPlane.widthOffset);
		float num2 = (flag ? builderAttachGridPlane.widthOffset : builderAttachGridPlane.lengthOffset);
		float num3 = num - potentialParentGridPlane.widthOffset;
		float num4 = num2 - potentialParentGridPlane.lengthOffset;
		Quaternion quaternion = Quaternion.Euler(0f, (float)twist * 90f, 0f);
		Quaternion quaternion2 = rotation * quaternion;
		float num5 = (float)xOffset * gridSize + num3;
		float num6 = (float)zOffset * gridSize + num4;
		Vector3 vector = new Vector3(num5, 0f, num6);
		Vector3 vector2 = position + rotation * vector;
		Transform center2 = builderAttachGridPlane.center;
		Quaternion quaternion3 = quaternion2 * Quaternion.Inverse(center2.localRotation);
		Vector3 vector3 = base.transform.InverseTransformPoint(center2.position);
		Vector3 vector4 = vector2 - quaternion3 * vector3;
		localPosition = potentialParentGridPlane.transform.InverseTransformPoint(vector4);
		localRotation = quaternion * Quaternion.Inverse(center2.localRotation);
		worldPosition = vector4;
		worldRotation = quaternion3;
	}

	// Token: 0x06001EE5 RID: 7909 RVA: 0x000974C4 File Offset: 0x000956C4
	public Quaternion TwistToLocalRotation(byte twist, int potentialAttachIndex)
	{
		float num = 90f * (float)twist;
		Quaternion quaternion = Quaternion.Euler(0f, num, 0f);
		if (potentialAttachIndex < 0 || potentialAttachIndex >= this.gridPlanes.Count)
		{
			return quaternion;
		}
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[potentialAttachIndex];
		Transform transform = ((builderAttachGridPlane.center != null) ? builderAttachGridPlane.center : builderAttachGridPlane.transform);
		return quaternion * Quaternion.Inverse(transform.localRotation);
	}

	// Token: 0x06001EE6 RID: 7910 RVA: 0x0009753C File Offset: 0x0009573C
	public int GetPiecePlacement()
	{
		byte pieceTwist = this.GetPieceTwist();
		sbyte b;
		sbyte b2;
		this.GetPieceBumpOffset(pieceTwist, out b, out b2);
		return BuilderTable.PackPiecePlacement(pieceTwist, b, b2);
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x00097564 File Offset: 0x00095764
	public byte GetPieceTwist()
	{
		if (this.attachIndex == -1)
		{
			return 0;
		}
		Quaternion localRotation = base.transform.localRotation;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[this.attachIndex];
		Quaternion quaternion = localRotation * builderAttachGridPlane.transform.localRotation;
		float num = 0.866f;
		Vector3 vector = quaternion * Vector3.forward;
		float num2 = Vector3.Dot(vector, Vector3.forward);
		float num3 = Vector3.Dot(vector, Vector3.right);
		bool flag = Mathf.Abs(num2) > num;
		bool flag2 = Mathf.Abs(num3) > num;
		if (!flag && !flag2)
		{
			return 0;
		}
		uint num4;
		if (flag)
		{
			num4 = ((num2 > 0f) ? 0U : 2U);
		}
		else
		{
			num4 = ((num3 > 0f) ? 1U : 3U);
		}
		return (byte)num4;
	}

	// Token: 0x06001EE8 RID: 7912 RVA: 0x00097618 File Offset: 0x00095818
	public void GetPieceBumpOffset(byte twist, out sbyte xOffset, out sbyte zOffset)
	{
		if (this.attachIndex == -1 || this.parentPiece == null)
		{
			xOffset = 0;
			zOffset = 0;
			return;
		}
		float gridSize = this.tableOwner.gridSize;
		BuilderAttachGridPlane builderAttachGridPlane = this.gridPlanes[this.attachIndex];
		BuilderAttachGridPlane builderAttachGridPlane2 = this.parentPiece.gridPlanes[this.parentAttachIndex];
		bool flag = (long)(twist % 2) == 1L;
		float num = (flag ? builderAttachGridPlane.lengthOffset : builderAttachGridPlane.widthOffset);
		float num2 = (flag ? builderAttachGridPlane.widthOffset : builderAttachGridPlane.lengthOffset);
		float num3 = num - builderAttachGridPlane2.widthOffset;
		float num4 = num2 - builderAttachGridPlane2.lengthOffset;
		Vector3 position = builderAttachGridPlane.center.position;
		Vector3 position2 = builderAttachGridPlane2.center.position;
		Vector3 vector = Quaternion.Inverse(builderAttachGridPlane2.center.rotation) * (position - position2);
		xOffset = (sbyte)Mathf.RoundToInt((vector.x - num3) / gridSize);
		zOffset = (sbyte)Mathf.RoundToInt((vector.z - num4) / gridSize);
	}

	// Token: 0x040021F2 RID: 8690
	public const int INVALID = -1;

	// Token: 0x040021F3 RID: 8691
	public const float LIGHT_MASS = 1f;

	// Token: 0x040021F4 RID: 8692
	public const float HEAVY_MASS = 10000f;

	// Token: 0x040021F5 RID: 8693
	[Header("Piece Properties")]
	public string displayName;

	// Token: 0x040021F6 RID: 8694
	public BuilderMaterialOptions materialOptions;

	// Token: 0x040021F7 RID: 8695
	public BuilderResources cost;

	// Token: 0x040021F8 RID: 8696
	private List<MeshRenderer> materialSwapTargets;

	// Token: 0x040021F9 RID: 8697
	private List<GorillaSurfaceOverride> surfaceOverrides;

	// Token: 0x040021FA RID: 8698
	public Transform scaleRoot;

	// Token: 0x040021FB RID: 8699
	public bool isBuiltIntoTable;

	// Token: 0x040021FC RID: 8700
	public bool isArmShelf;

	// Token: 0x040021FD RID: 8701
	[HideInInspector]
	public BuilderArmShelf armShelf;

	// Token: 0x040021FE RID: 8702
	public Vector3 desiredShelfOffset = Vector3.zero;

	// Token: 0x040021FF RID: 8703
	public Vector3 desiredShelfRotationOffset = Vector3.zero;

	// Token: 0x04002200 RID: 8704
	private bool isPrivatePlot;

	// Token: 0x04002201 RID: 8705
	[HideInInspector]
	public int privatePlotIndex;

	// Token: 0x04002202 RID: 8706
	public BuilderPiecePrivatePlot plotComponent;

	// Token: 0x04002203 RID: 8707
	[Header("VFX")]
	[SerializeField]
	private BuilderPieceEffectInfo vFXInfo;

	// Token: 0x04002204 RID: 8708
	[Header("Piece Properties")]
	public int pieceType;

	// Token: 0x04002205 RID: 8709
	public int pieceId;

	// Token: 0x04002206 RID: 8710
	public int pieceDataIndex;

	// Token: 0x04002207 RID: 8711
	public int materialType = -1;

	// Token: 0x04002208 RID: 8712
	public bool suppressMaterialWarnings;

	// Token: 0x04002209 RID: 8713
	public int heldByPlayerActorNumber;

	// Token: 0x0400220A RID: 8714
	public bool heldInLeftHand;

	// Token: 0x0400220B RID: 8715
	public Transform parentHeld;

	// Token: 0x0400220C RID: 8716
	[HideInInspector]
	public BuilderPiece parentPiece;

	// Token: 0x0400220D RID: 8717
	[HideInInspector]
	public BuilderPiece firstChildPiece;

	// Token: 0x0400220E RID: 8718
	[HideInInspector]
	public BuilderPiece nextSiblingPiece;

	// Token: 0x0400220F RID: 8719
	[HideInInspector]
	public int attachIndex;

	// Token: 0x04002210 RID: 8720
	[HideInInspector]
	public int parentAttachIndex;

	// Token: 0x04002211 RID: 8721
	public int shelfOwner = -1;

	// Token: 0x04002212 RID: 8722
	[HideInInspector]
	public List<BuilderAttachGridPlane> gridPlanes;

	// Token: 0x04002213 RID: 8723
	[HideInInspector]
	public List<Collider> colliders;

	// Token: 0x04002214 RID: 8724
	public List<Collider> placedOnlyColliders;

	// Token: 0x04002215 RID: 8725
	[Header("Toggle on Place")]
	public List<Behaviour> onlyWhenPlacedBehaviours;

	// Token: 0x04002216 RID: 8726
	public List<IBuilderPieceComponent> pieceComponents;

	// Token: 0x04002217 RID: 8727
	public IBuilderPieceFunctional functionalPieceComponent;

	// Token: 0x04002218 RID: 8728
	public byte functionalPieceState;

	// Token: 0x04002219 RID: 8729
	public List<IBuilderPieceFunctional> pieceFunctionComponents;

	// Token: 0x0400221A RID: 8730
	private bool pieceComponentsActive;

	// Token: 0x0400221B RID: 8731
	public List<GameObject> onlyWhenPlaced;

	// Token: 0x0400221C RID: 8732
	public List<GameObject> onlyWhenNotPlaced;

	// Token: 0x0400221D RID: 8733
	public bool areMeshesToggledOnPlace;

	// Token: 0x0400221E RID: 8734
	[NonSerialized]
	public Rigidbody rigidBody;

	// Token: 0x0400221F RID: 8735
	[NonSerialized]
	public int activatedTimeStamp;

	// Token: 0x04002220 RID: 8736
	[HideInInspector]
	public int preventSnapUntilMoved;

	// Token: 0x04002221 RID: 8737
	[HideInInspector]
	public Vector3 preventSnapUntilMovedFromPos;

	// Token: 0x04002222 RID: 8738
	[HideInInspector]
	public BuilderPiece requestedParentPiece;

	// Token: 0x04002223 RID: 8739
	private BuilderTable tableOwner;

	// Token: 0x04002224 RID: 8740
	public PieceFallbackInfo fallbackInfo;

	// Token: 0x04002225 RID: 8741
	[NonSerialized]
	public bool overrideSavedPiece;

	// Token: 0x04002226 RID: 8742
	[NonSerialized]
	public int savedPieceType = -1;

	// Token: 0x04002227 RID: 8743
	[NonSerialized]
	public int savedMaterialType = -1;

	// Token: 0x04002228 RID: 8744
	[Header("Mesh Combining")]
	public List<MeshRenderer> meshesToCombine;

	// Token: 0x04002229 RID: 8745
	public GameObject bumpPrefab;

	// Token: 0x0400222A RID: 8746
	public List<GameObject> bumps;

	// Token: 0x0400222B RID: 8747
	private float pieceScale;

	// Token: 0x0400222C RID: 8748
	[HideInInspector]
	public BuilderPiece.State state;

	// Token: 0x0400222D RID: 8749
	[HideInInspector]
	public bool isStatic;

	// Token: 0x0400222E RID: 8750
	[HideInInspector]
	public List<MeshRenderer> renderingIndirect;

	// Token: 0x0400222F RID: 8751
	[HideInInspector]
	public List<int> renderingIndirectTransformIndex;

	// Token: 0x04002230 RID: 8752
	[HideInInspector]
	public float tint;

	// Token: 0x04002231 RID: 8753
	private int paintingCount;

	// Token: 0x04002232 RID: 8754
	private int potentialGrabCount;

	// Token: 0x04002233 RID: 8755
	private int potentialGrabChildCount;

	// Token: 0x020004F6 RID: 1270
	public enum State
	{
		// Token: 0x04002235 RID: 8757
		None = -1,
		// Token: 0x04002236 RID: 8758
		AttachedAndPlaced,
		// Token: 0x04002237 RID: 8759
		AttachedToDropped,
		// Token: 0x04002238 RID: 8760
		Grabbed,
		// Token: 0x04002239 RID: 8761
		Dropped,
		// Token: 0x0400223A RID: 8762
		OnShelf,
		// Token: 0x0400223B RID: 8763
		Displayed,
		// Token: 0x0400223C RID: 8764
		GrabbedLocal,
		// Token: 0x0400223D RID: 8765
		OnConveyor,
		// Token: 0x0400223E RID: 8766
		AttachedToArm
	}
}
