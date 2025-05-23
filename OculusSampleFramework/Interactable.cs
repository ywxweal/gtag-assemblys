using System;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x02000BD6 RID: 3030
	public abstract class Interactable : MonoBehaviour
	{
		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x06004AD4 RID: 19156 RVA: 0x00164113 File Offset: 0x00162313
		public ColliderZone ProximityCollider
		{
			get
			{
				return this._proximityZoneCollider;
			}
		}

		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x06004AD5 RID: 19157 RVA: 0x0016411B File Offset: 0x0016231B
		public ColliderZone ContactCollider
		{
			get
			{
				return this._contactZoneCollider;
			}
		}

		// Token: 0x1700074C RID: 1868
		// (get) Token: 0x06004AD6 RID: 19158 RVA: 0x00164123 File Offset: 0x00162323
		public ColliderZone ActionCollider
		{
			get
			{
				return this._actionZoneCollider;
			}
		}

		// Token: 0x1700074D RID: 1869
		// (get) Token: 0x06004AD7 RID: 19159 RVA: 0x000BC497 File Offset: 0x000BA697
		public virtual int ValidToolTagsMask
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x14000090 RID: 144
		// (add) Token: 0x06004AD8 RID: 19160 RVA: 0x0016412C File Offset: 0x0016232C
		// (remove) Token: 0x06004AD9 RID: 19161 RVA: 0x00164164 File Offset: 0x00162364
		public event Action<ColliderZoneArgs> ProximityZoneEvent;

		// Token: 0x06004ADA RID: 19162 RVA: 0x00164199 File Offset: 0x00162399
		protected virtual void OnProximityZoneEvent(ColliderZoneArgs args)
		{
			if (this.ProximityZoneEvent != null)
			{
				this.ProximityZoneEvent(args);
			}
		}

		// Token: 0x14000091 RID: 145
		// (add) Token: 0x06004ADB RID: 19163 RVA: 0x001641B0 File Offset: 0x001623B0
		// (remove) Token: 0x06004ADC RID: 19164 RVA: 0x001641E8 File Offset: 0x001623E8
		public event Action<ColliderZoneArgs> ContactZoneEvent;

		// Token: 0x06004ADD RID: 19165 RVA: 0x0016421D File Offset: 0x0016241D
		protected virtual void OnContactZoneEvent(ColliderZoneArgs args)
		{
			if (this.ContactZoneEvent != null)
			{
				this.ContactZoneEvent(args);
			}
		}

		// Token: 0x14000092 RID: 146
		// (add) Token: 0x06004ADE RID: 19166 RVA: 0x00164234 File Offset: 0x00162434
		// (remove) Token: 0x06004ADF RID: 19167 RVA: 0x0016426C File Offset: 0x0016246C
		public event Action<ColliderZoneArgs> ActionZoneEvent;

		// Token: 0x06004AE0 RID: 19168 RVA: 0x001642A1 File Offset: 0x001624A1
		protected virtual void OnActionZoneEvent(ColliderZoneArgs args)
		{
			if (this.ActionZoneEvent != null)
			{
				this.ActionZoneEvent(args);
			}
		}

		// Token: 0x06004AE1 RID: 19169
		public abstract void UpdateCollisionDepth(InteractableTool interactableTool, InteractableCollisionDepth oldCollisionDepth, InteractableCollisionDepth newCollisionDepth);

		// Token: 0x06004AE2 RID: 19170 RVA: 0x001642B7 File Offset: 0x001624B7
		protected virtual void Awake()
		{
			InteractableRegistry.RegisterInteractable(this);
		}

		// Token: 0x06004AE3 RID: 19171 RVA: 0x001642BF File Offset: 0x001624BF
		protected virtual void OnDestroy()
		{
			InteractableRegistry.UnregisterInteractable(this);
		}

		// Token: 0x04004D85 RID: 19845
		protected ColliderZone _proximityZoneCollider;

		// Token: 0x04004D86 RID: 19846
		protected ColliderZone _contactZoneCollider;

		// Token: 0x04004D87 RID: 19847
		protected ColliderZone _actionZoneCollider;

		// Token: 0x04004D8B RID: 19851
		public Interactable.InteractableStateArgsEvent InteractableStateChanged;

		// Token: 0x02000BD7 RID: 3031
		[Serializable]
		public class InteractableStateArgsEvent : UnityEvent<InteractableStateArgs>
		{
		}
	}
}
