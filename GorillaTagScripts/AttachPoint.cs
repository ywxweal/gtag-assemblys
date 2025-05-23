using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000AD0 RID: 2768
	public class AttachPoint : MonoBehaviour
	{
		// Token: 0x060042EA RID: 17130 RVA: 0x00135691 File Offset: 0x00133891
		private void Start()
		{
			base.transform.parent.parent = null;
		}

		// Token: 0x060042EB RID: 17131 RVA: 0x001356A4 File Offset: 0x001338A4
		private void OnTriggerEnter(Collider other)
		{
			if (this.attachPoint.childCount == 0)
			{
				this.UpdateHookState(false);
			}
			DecorativeItem componentInParent = other.GetComponentInParent<DecorativeItem>();
			if (componentInParent == null || componentInParent.InHand())
			{
				return;
			}
			if (this.IsHooked())
			{
				return;
			}
			this.UpdateHookState(true);
			componentInParent.SnapItem(true, this.attachPoint.position);
		}

		// Token: 0x060042EC RID: 17132 RVA: 0x00135700 File Offset: 0x00133900
		private void OnTriggerExit(Collider other)
		{
			DecorativeItem componentInParent = other.GetComponentInParent<DecorativeItem>();
			if (componentInParent == null || !componentInParent.InHand())
			{
				return;
			}
			this.UpdateHookState(false);
			componentInParent.SnapItem(false, Vector3.zero);
		}

		// Token: 0x060042ED RID: 17133 RVA: 0x00135739 File Offset: 0x00133939
		private void UpdateHookState(bool isHooked)
		{
			this.SetIsHook(isHooked);
		}

		// Token: 0x060042EE RID: 17134 RVA: 0x00135742 File Offset: 0x00133942
		internal void SetIsHook(bool isHooked)
		{
			this.isHooked = isHooked;
			UnityAction unityAction = this.onHookedChanged;
			if (unityAction == null)
			{
				return;
			}
			unityAction();
		}

		// Token: 0x060042EF RID: 17135 RVA: 0x0013575B File Offset: 0x0013395B
		public bool IsHooked()
		{
			return this.isHooked || this.attachPoint.childCount != 0;
		}

		// Token: 0x04004572 RID: 17778
		public Transform attachPoint;

		// Token: 0x04004573 RID: 17779
		public UnityAction onHookedChanged;

		// Token: 0x04004574 RID: 17780
		private bool isHooked;

		// Token: 0x04004575 RID: 17781
		private bool wasHooked;

		// Token: 0x04004576 RID: 17782
		public bool inForest;
	}
}
