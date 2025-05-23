using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000AD0 RID: 2768
	public class AttachPoint : MonoBehaviour
	{
		// Token: 0x060042E9 RID: 17129 RVA: 0x001355B9 File Offset: 0x001337B9
		private void Start()
		{
			base.transform.parent.parent = null;
		}

		// Token: 0x060042EA RID: 17130 RVA: 0x001355CC File Offset: 0x001337CC
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

		// Token: 0x060042EB RID: 17131 RVA: 0x00135628 File Offset: 0x00133828
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

		// Token: 0x060042EC RID: 17132 RVA: 0x00135661 File Offset: 0x00133861
		private void UpdateHookState(bool isHooked)
		{
			this.SetIsHook(isHooked);
		}

		// Token: 0x060042ED RID: 17133 RVA: 0x0013566A File Offset: 0x0013386A
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

		// Token: 0x060042EE RID: 17134 RVA: 0x00135683 File Offset: 0x00133883
		public bool IsHooked()
		{
			return this.isHooked || this.attachPoint.childCount != 0;
		}

		// Token: 0x04004571 RID: 17777
		public Transform attachPoint;

		// Token: 0x04004572 RID: 17778
		public UnityAction onHookedChanged;

		// Token: 0x04004573 RID: 17779
		private bool isHooked;

		// Token: 0x04004574 RID: 17780
		private bool wasHooked;

		// Token: 0x04004575 RID: 17781
		public bool inForest;
	}
}
