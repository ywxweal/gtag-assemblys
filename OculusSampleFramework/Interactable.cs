using System;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x02000BD6 RID: 3030
	public abstract class Interactable : MonoBehaviour
	{
		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x06004AD3 RID: 19155 RVA: 0x0016403B File Offset: 0x0016223B
		public ColliderZone ProximityCollider
		{
			get
			{
				return this._proximityZoneCollider;
			}
		}

		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x06004AD4 RID: 19156 RVA: 0x00164043 File Offset: 0x00162243
		public ColliderZone ContactCollider
		{
			get
			{
				return this._contactZoneCollider;
			}
		}

		// Token: 0x1700074C RID: 1868
		// (get) Token: 0x06004AD5 RID: 19157 RVA: 0x0016404B File Offset: 0x0016224B
		public ColliderZone ActionCollider
		{
			get
			{
				return this._actionZoneCollider;
			}
		}

		// Token: 0x1700074D RID: 1869
		// (get) Token: 0x06004AD6 RID: 19158 RVA: 0x000BC477 File Offset: 0x000BA677
		public virtual int ValidToolTagsMask
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x14000090 RID: 144
		// (add) Token: 0x06004AD7 RID: 19159 RVA: 0x00164054 File Offset: 0x00162254
		// (remove) Token: 0x06004AD8 RID: 19160 RVA: 0x0016408C File Offset: 0x0016228C
		public event Action<ColliderZoneArgs> ProximityZoneEvent;

		// Token: 0x06004AD9 RID: 19161 RVA: 0x001640C1 File Offset: 0x001622C1
		protected virtual void OnProximityZoneEvent(ColliderZoneArgs args)
		{
			if (this.ProximityZoneEvent != null)
			{
				this.ProximityZoneEvent(args);
			}
		}

		// Token: 0x14000091 RID: 145
		// (add) Token: 0x06004ADA RID: 19162 RVA: 0x001640D8 File Offset: 0x001622D8
		// (remove) Token: 0x06004ADB RID: 19163 RVA: 0x00164110 File Offset: 0x00162310
		public event Action<ColliderZoneArgs> ContactZoneEvent;

		// Token: 0x06004ADC RID: 19164 RVA: 0x00164145 File Offset: 0x00162345
		protected virtual void OnContactZoneEvent(ColliderZoneArgs args)
		{
			if (this.ContactZoneEvent != null)
			{
				this.ContactZoneEvent(args);
			}
		}

		// Token: 0x14000092 RID: 146
		// (add) Token: 0x06004ADD RID: 19165 RVA: 0x0016415C File Offset: 0x0016235C
		// (remove) Token: 0x06004ADE RID: 19166 RVA: 0x00164194 File Offset: 0x00162394
		public event Action<ColliderZoneArgs> ActionZoneEvent;

		// Token: 0x06004ADF RID: 19167 RVA: 0x001641C9 File Offset: 0x001623C9
		protected virtual void OnActionZoneEvent(ColliderZoneArgs args)
		{
			if (this.ActionZoneEvent != null)
			{
				this.ActionZoneEvent(args);
			}
		}

		// Token: 0x06004AE0 RID: 19168
		public abstract void UpdateCollisionDepth(InteractableTool interactableTool, InteractableCollisionDepth oldCollisionDepth, InteractableCollisionDepth newCollisionDepth);

		// Token: 0x06004AE1 RID: 19169 RVA: 0x001641DF File Offset: 0x001623DF
		protected virtual void Awake()
		{
			InteractableRegistry.RegisterInteractable(this);
		}

		// Token: 0x06004AE2 RID: 19170 RVA: 0x001641E7 File Offset: 0x001623E7
		protected virtual void OnDestroy()
		{
			InteractableRegistry.UnregisterInteractable(this);
		}

		// Token: 0x04004D84 RID: 19844
		protected ColliderZone _proximityZoneCollider;

		// Token: 0x04004D85 RID: 19845
		protected ColliderZone _contactZoneCollider;

		// Token: 0x04004D86 RID: 19846
		protected ColliderZone _actionZoneCollider;

		// Token: 0x04004D8A RID: 19850
		public Interactable.InteractableStateArgsEvent InteractableStateChanged;

		// Token: 0x02000BD7 RID: 3031
		[Serializable]
		public class InteractableStateArgsEvent : UnityEvent<InteractableStateArgs>
		{
		}
	}
}
