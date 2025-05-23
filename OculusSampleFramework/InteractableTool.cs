using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BE5 RID: 3045
	public abstract class InteractableTool : MonoBehaviour
	{
		// Token: 0x1700075C RID: 1884
		// (get) Token: 0x06004B1F RID: 19231 RVA: 0x00045F89 File Offset: 0x00044189
		public Transform ToolTransform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x1700075D RID: 1885
		// (get) Token: 0x06004B20 RID: 19232 RVA: 0x00164ECA File Offset: 0x001630CA
		// (set) Token: 0x06004B21 RID: 19233 RVA: 0x00164ED2 File Offset: 0x001630D2
		public bool IsRightHandedTool { get; set; }

		// Token: 0x1700075E RID: 1886
		// (get) Token: 0x06004B22 RID: 19234
		public abstract InteractableToolTags ToolTags { get; }

		// Token: 0x1700075F RID: 1887
		// (get) Token: 0x06004B23 RID: 19235
		public abstract ToolInputState ToolInputState { get; }

		// Token: 0x17000760 RID: 1888
		// (get) Token: 0x06004B24 RID: 19236
		public abstract bool IsFarFieldTool { get; }

		// Token: 0x17000761 RID: 1889
		// (get) Token: 0x06004B25 RID: 19237 RVA: 0x00164EDB File Offset: 0x001630DB
		// (set) Token: 0x06004B26 RID: 19238 RVA: 0x00164EE3 File Offset: 0x001630E3
		public Vector3 Velocity { get; protected set; }

		// Token: 0x17000762 RID: 1890
		// (get) Token: 0x06004B27 RID: 19239 RVA: 0x00164EEC File Offset: 0x001630EC
		// (set) Token: 0x06004B28 RID: 19240 RVA: 0x00164EF4 File Offset: 0x001630F4
		public Vector3 InteractionPosition { get; protected set; }

		// Token: 0x06004B29 RID: 19241 RVA: 0x00164EFD File Offset: 0x001630FD
		public List<InteractableCollisionInfo> GetCurrentIntersectingObjects()
		{
			return this._currentIntersectingObjects;
		}

		// Token: 0x06004B2A RID: 19242
		public abstract List<InteractableCollisionInfo> GetNextIntersectingObjects();

		// Token: 0x06004B2B RID: 19243
		public abstract void FocusOnInteractable(Interactable focusedInteractable, ColliderZone colliderZone);

		// Token: 0x06004B2C RID: 19244
		public abstract void DeFocus();

		// Token: 0x17000763 RID: 1891
		// (get) Token: 0x06004B2D RID: 19245
		// (set) Token: 0x06004B2E RID: 19246
		public abstract bool EnableState { get; set; }

		// Token: 0x06004B2F RID: 19247
		public abstract void Initialize();

		// Token: 0x06004B30 RID: 19248 RVA: 0x00164F05 File Offset: 0x00163105
		public KeyValuePair<Interactable, InteractableCollisionInfo> GetFirstCurrentCollisionInfo()
		{
			return this._currInteractableToCollisionInfos.First<KeyValuePair<Interactable, InteractableCollisionInfo>>();
		}

		// Token: 0x06004B31 RID: 19249 RVA: 0x00164F12 File Offset: 0x00163112
		public void ClearAllCurrentCollisionInfos()
		{
			this._currInteractableToCollisionInfos.Clear();
		}

		// Token: 0x06004B32 RID: 19250 RVA: 0x00164F20 File Offset: 0x00163120
		public virtual void UpdateCurrentCollisionsBasedOnDepth()
		{
			this._currInteractableToCollisionInfos.Clear();
			foreach (InteractableCollisionInfo interactableCollisionInfo in this._currentIntersectingObjects)
			{
				Interactable parentInteractable = interactableCollisionInfo.InteractableCollider.ParentInteractable;
				InteractableCollisionDepth collisionDepth = interactableCollisionInfo.CollisionDepth;
				InteractableCollisionInfo interactableCollisionInfo2 = null;
				if (!this._currInteractableToCollisionInfos.TryGetValue(parentInteractable, out interactableCollisionInfo2))
				{
					this._currInteractableToCollisionInfos[parentInteractable] = interactableCollisionInfo;
				}
				else if (interactableCollisionInfo2.CollisionDepth < collisionDepth)
				{
					interactableCollisionInfo2.InteractableCollider = interactableCollisionInfo.InteractableCollider;
					interactableCollisionInfo2.CollisionDepth = collisionDepth;
				}
			}
		}

		// Token: 0x06004B33 RID: 19251 RVA: 0x00164FCC File Offset: 0x001631CC
		public virtual void UpdateLatestCollisionData()
		{
			this._addedInteractables.Clear();
			this._removedInteractables.Clear();
			this._remainingInteractables.Clear();
			foreach (Interactable interactable in this._currInteractableToCollisionInfos.Keys)
			{
				if (!this._prevInteractableToCollisionInfos.ContainsKey(interactable))
				{
					this._addedInteractables.Add(interactable);
				}
				else
				{
					this._remainingInteractables.Add(interactable);
				}
			}
			foreach (Interactable interactable2 in this._prevInteractableToCollisionInfos.Keys)
			{
				if (!this._currInteractableToCollisionInfos.ContainsKey(interactable2))
				{
					this._removedInteractables.Add(interactable2);
				}
			}
			foreach (Interactable interactable3 in this._removedInteractables)
			{
				interactable3.UpdateCollisionDepth(this, this._prevInteractableToCollisionInfos[interactable3].CollisionDepth, InteractableCollisionDepth.None);
			}
			foreach (Interactable interactable4 in this._addedInteractables)
			{
				InteractableCollisionDepth collisionDepth = this._currInteractableToCollisionInfos[interactable4].CollisionDepth;
				interactable4.UpdateCollisionDepth(this, InteractableCollisionDepth.None, collisionDepth);
			}
			foreach (Interactable interactable5 in this._remainingInteractables)
			{
				InteractableCollisionDepth collisionDepth2 = this._currInteractableToCollisionInfos[interactable5].CollisionDepth;
				InteractableCollisionDepth collisionDepth3 = this._prevInteractableToCollisionInfos[interactable5].CollisionDepth;
				interactable5.UpdateCollisionDepth(this, collisionDepth3, collisionDepth2);
			}
			this._prevInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>(this._currInteractableToCollisionInfos);
		}

		// Token: 0x04004DD0 RID: 19920
		protected List<InteractableCollisionInfo> _currentIntersectingObjects = new List<InteractableCollisionInfo>();

		// Token: 0x04004DD1 RID: 19921
		private List<Interactable> _addedInteractables = new List<Interactable>();

		// Token: 0x04004DD2 RID: 19922
		private List<Interactable> _removedInteractables = new List<Interactable>();

		// Token: 0x04004DD3 RID: 19923
		private List<Interactable> _remainingInteractables = new List<Interactable>();

		// Token: 0x04004DD4 RID: 19924
		private Dictionary<Interactable, InteractableCollisionInfo> _currInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>();

		// Token: 0x04004DD5 RID: 19925
		private Dictionary<Interactable, InteractableCollisionInfo> _prevInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>();
	}
}
