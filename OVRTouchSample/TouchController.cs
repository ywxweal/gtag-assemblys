using System;
using UnityEngine;

namespace OVRTouchSample
{
	// Token: 0x02000C0A RID: 3082
	public class TouchController : MonoBehaviour
	{
		// Token: 0x06004C27 RID: 19495 RVA: 0x00168CA0 File Offset: 0x00166EA0
		private void Update()
		{
			this.m_animator.SetFloat("Button 1", OVRInput.Get(OVRInput.Button.One, this.m_controller) ? 1f : 0f);
			this.m_animator.SetFloat("Button 2", OVRInput.Get(OVRInput.Button.Two, this.m_controller) ? 1f : 0f);
			this.m_animator.SetFloat("Joy X", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, this.m_controller).x);
			this.m_animator.SetFloat("Joy Y", OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, this.m_controller).y);
			this.m_animator.SetFloat("Grip", OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, this.m_controller));
			this.m_animator.SetFloat("Trigger", OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, this.m_controller));
			OVRManager.InputFocusAcquired += this.OnInputFocusAcquired;
			OVRManager.InputFocusLost += this.OnInputFocusLost;
		}

		// Token: 0x06004C28 RID: 19496 RVA: 0x00168D9D File Offset: 0x00166F9D
		private void OnInputFocusLost()
		{
			if (base.gameObject.activeInHierarchy)
			{
				base.gameObject.SetActive(false);
				this.m_restoreOnInputAcquired = true;
			}
		}

		// Token: 0x06004C29 RID: 19497 RVA: 0x00168DBF File Offset: 0x00166FBF
		private void OnInputFocusAcquired()
		{
			if (this.m_restoreOnInputAcquired)
			{
				base.gameObject.SetActive(true);
				this.m_restoreOnInputAcquired = false;
			}
		}

		// Token: 0x04004EF9 RID: 20217
		[SerializeField]
		private OVRInput.Controller m_controller;

		// Token: 0x04004EFA RID: 20218
		[SerializeField]
		private Animator m_animator;

		// Token: 0x04004EFB RID: 20219
		private bool m_restoreOnInputAcquired;
	}
}
