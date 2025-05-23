using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ADB RID: 2779
	public class BuilderItem : TransferrableObject
	{
		// Token: 0x06004347 RID: 17223 RVA: 0x0013741C File Offset: 0x0013561C
		public override bool ShouldBeKinematic()
		{
			return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State4 || base.ShouldBeKinematic();
		}

		// Token: 0x06004348 RID: 17224 RVA: 0x0013743C File Offset: 0x0013563C
		protected override void Awake()
		{
			base.Awake();
			this.parent = base.transform.parent;
			this.currTable = null;
			this.initialPosition = base.transform.position;
			this.initialRotation = base.transform.rotation;
			this.initialGrabInteractorScale = this.gripInteractor.transform.localScale;
		}

		// Token: 0x06004349 RID: 17225 RVA: 0x000783DC File Offset: 0x000765DC
		internal override void OnEnable()
		{
			base.OnEnable();
		}

		// Token: 0x0600434A RID: 17226 RVA: 0x00022B8F File Offset: 0x00020D8F
		internal override void OnDisable()
		{
			base.OnDisable();
		}

		// Token: 0x0600434B RID: 17227 RVA: 0x0013749F File Offset: 0x0013569F
		protected override void Start()
		{
			base.Start();
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x0600434C RID: 17228 RVA: 0x001374BC File Offset: 0x001356BC
		public void AttachPiece(BuilderPiece piece)
		{
			base.transform.SetPositionAndRotation(piece.transform.position, piece.transform.rotation);
			piece.transform.localScale = Vector3.one;
			piece.transform.SetParent(this.itemRoot.transform);
			Debug.LogFormat(piece.gameObject, "Attach Piece {0} to container {1}", new object[]
			{
				piece.gameObject.GetInstanceID(),
				base.gameObject.GetInstanceID()
			});
			this.attachedPiece = piece;
		}

		// Token: 0x0600434D RID: 17229 RVA: 0x00137554 File Offset: 0x00135754
		public void DetachPiece(BuilderPiece piece)
		{
			if (piece != this.attachedPiece)
			{
				Debug.LogErrorFormat("Trying to detach piece {0} from a container containing {1}", new object[]
				{
					piece.pieceId,
					this.attachedPiece.pieceId
				});
				return;
			}
			piece.transform.SetParent(null);
			Debug.LogFormat(this.attachedPiece.gameObject, "Detach Piece {0} from container {1}", new object[]
			{
				this.attachedPiece.gameObject.GetInstanceID(),
				base.gameObject.GetInstanceID()
			});
			this.attachedPiece = null;
		}

		// Token: 0x0600434E RID: 17230 RVA: 0x001375FC File Offset: 0x001357FC
		private new void OnStateChanged()
		{
			if (this.itemState == TransferrableObject.ItemStates.State2)
			{
				this.enableCollidersWhenReady = true;
				this.gripInteractor.transform.localScale = this.initialGrabInteractorScale * 2f;
				this.handsFreeOfCollidersTime = 0f;
				return;
			}
			this.enableCollidersWhenReady = false;
			this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
			this.handsFreeOfCollidersTime = 0f;
		}

		// Token: 0x0600434F RID: 17231 RVA: 0x00137670 File Offset: 0x00135870
		public override Matrix4x4 GetDefaultTransformationMatrix()
		{
			if (this.reliableState.dirty)
			{
				base.SetupHandMatrix(this.reliableState.leftHandAttachPos, this.reliableState.leftHandAttachRot, this.reliableState.rightHandAttachPos, this.reliableState.rightHandAttachRot);
				this.reliableState.dirty = false;
			}
			return base.GetDefaultTransformationMatrix();
		}

		// Token: 0x06004350 RID: 17232 RVA: 0x001376D0 File Offset: 0x001358D0
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (base.InHand())
			{
				this.itemState = TransferrableObject.ItemStates.State0;
			}
			BuilderItem.BuilderItemState itemState = (BuilderItem.BuilderItemState)this.itemState;
			if (itemState != this.previousItemState)
			{
				this.OnStateChanged();
			}
			this.previousItemState = itemState;
			if (this.enableCollidersWhenReady)
			{
				bool flag = this.IsOverlapping(EquipmentInteractor.instance.overlapInteractionPointsRight) || this.IsOverlapping(EquipmentInteractor.instance.overlapInteractionPointsLeft);
				this.handsFreeOfCollidersTime += (flag ? 0f : Time.deltaTime);
				if (this.handsFreeOfCollidersTime > 0.1f)
				{
					this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
					this.enableCollidersWhenReady = false;
				}
			}
		}

		// Token: 0x06004351 RID: 17233 RVA: 0x00137788 File Offset: 0x00135988
		private bool IsOverlapping(List<InteractionPoint> interactionPoints)
		{
			if (interactionPoints == null)
			{
				return false;
			}
			for (int i = 0; i < interactionPoints.Count; i++)
			{
				if (interactionPoints[i] == this.gripInteractor)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004352 RID: 17234 RVA: 0x001377C2 File Offset: 0x001359C2
		protected override void LateUpdateLocal()
		{
			base.LateUpdateLocal();
		}

		// Token: 0x06004353 RID: 17235 RVA: 0x001377CA File Offset: 0x001359CA
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (GorillaTagger.Instance.offlineVRRig.scaleFactor < 1f)
			{
				return;
			}
			base.OnGrab(pointGrabbed, grabbingHand);
			this.itemState = TransferrableObject.ItemStates.State0;
		}

		// Token: 0x06004354 RID: 17236 RVA: 0x001377F2 File Offset: 0x001359F2
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			this.itemState = TransferrableObject.ItemStates.State1;
			this.Reparent(null);
			this.parentItem = null;
			this.gripInteractor.transform.localScale = this.initialGrabInteractorScale;
			return true;
		}

		// Token: 0x06004355 RID: 17237 RVA: 0x0013782D File Offset: 0x00135A2D
		public void OnHoverOverTableStart(BuilderTable table)
		{
			this.currTable = table;
		}

		// Token: 0x06004356 RID: 17238 RVA: 0x00137836 File Offset: 0x00135A36
		public void OnHoverOverTableEnd(BuilderTable table)
		{
			this.currTable = null;
		}

		// Token: 0x06004357 RID: 17239 RVA: 0x0013783F File Offset: 0x00135A3F
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
		}

		// Token: 0x06004358 RID: 17240 RVA: 0x00137848 File Offset: 0x00135A48
		public override void OnLeftRoom()
		{
			base.OnLeftRoom();
			base.transform.position = this.initialPosition;
			base.transform.rotation = this.initialRotation;
			if (this.worldShareableInstance != null)
			{
				this.worldShareableInstance.transform.position = this.initialPosition;
				this.worldShareableInstance.transform.rotation = this.initialRotation;
			}
			this.itemState = TransferrableObject.ItemStates.State4;
			this.currentState = TransferrableObject.PositionState.Dropped;
		}

		// Token: 0x06004359 RID: 17241 RVA: 0x00096D9A File Offset: 0x00094F9A
		private void PlayVFX(GameObject vfx)
		{
			ObjectPools.instance.Instantiate(vfx, base.transform.position, true);
		}

		// Token: 0x0600435A RID: 17242 RVA: 0x001378CA File Offset: 0x00135ACA
		private bool Reparent(Transform _transform)
		{
			if (!this.allowReparenting)
			{
				return false;
			}
			if (this.parent)
			{
				this.parent.SetParent(_transform);
				base.transform.SetParent(this.parent);
				return true;
			}
			return false;
		}

		// Token: 0x0600435B RID: 17243 RVA: 0x00137903 File Offset: 0x00135B03
		private bool ShouldPlayFX()
		{
			return this.previousItemState == BuilderItem.BuilderItemState.isHeld || this.previousItemState == BuilderItem.BuilderItemState.dropped;
		}

		// Token: 0x0600435C RID: 17244 RVA: 0x0013791A File Offset: 0x00135B1A
		public static GameObject BuildEnvItem(int prefabHash, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(prefabHash, true);
			gameObject.transform.SetPositionAndRotation(position, rotation);
			return gameObject;
		}

		// Token: 0x0600435D RID: 17245 RVA: 0x00137938 File Offset: 0x00135B38
		protected override void OnHandMatrixUpdate(Vector3 localPosition, Quaternion localRotation, bool leftHand)
		{
			if (leftHand)
			{
				this.reliableState.leftHandAttachPos = localPosition;
				this.reliableState.leftHandAttachRot = localRotation;
			}
			else
			{
				this.reliableState.rightHandAttachPos = localPosition;
				this.reliableState.rightHandAttachRot = localRotation;
			}
			this.reliableState.dirty = true;
		}

		// Token: 0x0600435E RID: 17246 RVA: 0x00137986 File Offset: 0x00135B86
		public int GetPhotonViewId()
		{
			if (this.worldShareableInstance == null)
			{
				return -1;
			}
			return this.worldShareableInstance.ViewID;
		}

		// Token: 0x040045CA RID: 17866
		public BuilderItemReliableState reliableState;

		// Token: 0x040045CB RID: 17867
		public string builtItemPath;

		// Token: 0x040045CC RID: 17868
		public GameObject itemRoot;

		// Token: 0x040045CD RID: 17869
		private bool enableCollidersWhenReady;

		// Token: 0x040045CE RID: 17870
		private float handsFreeOfCollidersTime;

		// Token: 0x040045CF RID: 17871
		[NonSerialized]
		public BuilderPiece attachedPiece;

		// Token: 0x040045D0 RID: 17872
		public List<Behaviour> onlyWhenPlacedBehaviours;

		// Token: 0x040045D1 RID: 17873
		[NonSerialized]
		public BuilderItem parentItem;

		// Token: 0x040045D2 RID: 17874
		public List<BuilderAttachGridPlane> gridPlanes;

		// Token: 0x040045D3 RID: 17875
		public List<BuilderAttachEdge> edges;

		// Token: 0x040045D4 RID: 17876
		private List<Collider> colliders;

		// Token: 0x040045D5 RID: 17877
		private Transform parent;

		// Token: 0x040045D6 RID: 17878
		private Vector3 initialPosition;

		// Token: 0x040045D7 RID: 17879
		private Quaternion initialRotation;

		// Token: 0x040045D8 RID: 17880
		private Vector3 initialGrabInteractorScale;

		// Token: 0x040045D9 RID: 17881
		private BuilderTable currTable;

		// Token: 0x040045DA RID: 17882
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040045DB RID: 17883
		public AudioClip snapAudio;

		// Token: 0x040045DC RID: 17884
		public AudioClip placeAudio;

		// Token: 0x040045DD RID: 17885
		public GameObject placeVFX;

		// Token: 0x040045DE RID: 17886
		private BuilderItem.BuilderItemState previousItemState = BuilderItem.BuilderItemState.dropped;

		// Token: 0x02000ADC RID: 2780
		private enum BuilderItemState
		{
			// Token: 0x040045E0 RID: 17888
			isHeld = 1,
			// Token: 0x040045E1 RID: 17889
			dropped,
			// Token: 0x040045E2 RID: 17890
			placed = 4,
			// Token: 0x040045E3 RID: 17891
			unused0 = 8,
			// Token: 0x040045E4 RID: 17892
			none = 16
		}
	}
}
