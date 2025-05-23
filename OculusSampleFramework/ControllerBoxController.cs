using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000BEC RID: 3052
	public class ControllerBoxController : MonoBehaviour
	{
		// Token: 0x06004B64 RID: 19300 RVA: 0x000023F4 File Offset: 0x000005F4
		private void Awake()
		{
		}

		// Token: 0x06004B65 RID: 19301 RVA: 0x00165D3D File Offset: 0x00163F3D
		public void StartStopStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.StartStopStateChanged();
			}
		}

		// Token: 0x06004B66 RID: 19302 RVA: 0x00165D53 File Offset: 0x00163F53
		public void DecreaseSpeedStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.DecreaseSpeedStateChanged();
			}
		}

		// Token: 0x06004B67 RID: 19303 RVA: 0x00165D69 File Offset: 0x00163F69
		public void IncreaseSpeedStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.IncreaseSpeedStateChanged();
			}
		}

		// Token: 0x06004B68 RID: 19304 RVA: 0x00165D7F File Offset: 0x00163F7F
		public void SmokeButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.SmokeButtonStateChanged();
			}
		}

		// Token: 0x06004B69 RID: 19305 RVA: 0x00165D95 File Offset: 0x00163F95
		public void WhistleButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.WhistleButtonStateChanged();
			}
		}

		// Token: 0x06004B6A RID: 19306 RVA: 0x00165DAB File Offset: 0x00163FAB
		public void ReverseButtonStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._locomotive.ReverseButtonStateChanged();
			}
		}

		// Token: 0x06004B6B RID: 19307 RVA: 0x00165DC1 File Offset: 0x00163FC1
		public void SwitchVisualization(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				HandsManager.Instance.SwitchVisualization();
			}
		}

		// Token: 0x06004B6C RID: 19308 RVA: 0x00165DD6 File Offset: 0x00163FD6
		public void GoMoo(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				this._cowController.GoMooCowGo();
			}
		}

		// Token: 0x04004DFB RID: 19963
		[SerializeField]
		private TrainLocomotive _locomotive;

		// Token: 0x04004DFC RID: 19964
		[SerializeField]
		private CowController _cowController;
	}
}
