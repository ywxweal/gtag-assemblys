using System;
using Cinemachine;
using Liv.Lck;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x02000269 RID: 617
public class MonitorOutputController : MonoBehaviour
{
	// Token: 0x06000E2B RID: 3627 RVA: 0x00048596 File Offset: 0x00046796
	private void Awake()
	{
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x000485A9 File Offset: 0x000467A9
	private void OnEnable()
	{
		this._gtLckController.OnCameraModeChanged += this.OnCameraModeChanged;
		LckBodyCameraSpawner.OnCameraStateChange += this.CameraStateChanged;
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x000485D4 File Offset: 0x000467D4
	private void Update()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			Object.Destroy(this);
		}
		if (this._shoulderCamera == null)
		{
			this.FindShoulderCamera();
		}
		if (this._lckCamera != null)
		{
			this._shoulderCamera.transform.position = this._lckCamera.transform.position;
			this._shoulderCamera.transform.rotation = this._lckCamera.transform.rotation;
			this._shoulderCamera.fieldOfView = this._lckCamera.fieldOfView;
			return;
		}
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x0004867A File Offset: 0x0004687A
	private void CameraStateChanged(LckBodyCameraSpawner.CameraState state)
	{
		switch (state)
		{
		case LckBodyCameraSpawner.CameraState.CameraDisabled:
			this.RestoreShoulderCamera();
			return;
		case LckBodyCameraSpawner.CameraState.CameraOnNeck:
			this.TakeOverShoulderCamera();
			return;
		case LckBodyCameraSpawner.CameraState.CameraSpawned:
			this.TakeOverShoulderCamera();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x000486A3 File Offset: 0x000468A3
	private void OnDisable()
	{
		this._gtLckController.OnCameraModeChanged -= this.OnCameraModeChanged;
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = true;
		LckBodyCameraSpawner.OnCameraStateChange -= this.CameraStateChanged;
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x000486E3 File Offset: 0x000468E3
	private void OnCameraModeChanged(CameraMode mode, ILckCamera lckCamera)
	{
		this._lckCamera = lckCamera.GetCameraComponent();
		this._lckActiveCameraMode = mode;
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x000486F8 File Offset: 0x000468F8
	private void TakeOverShoulderCamera()
	{
		this.FindShoulderCamera();
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = false;
		this._shoulderCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("LCKHide"));
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x00048738 File Offset: 0x00046938
	private void RestoreShoulderCamera()
	{
		this.FindShoulderCamera();
		this._shoulderCamera.gameObject.GetComponentInChildren<CinemachineBrain>().enabled = true;
		this._shoulderCamera.cullingMask |= 1 << LayerMask.NameToLayer("LCKHide");
		this._shoulderCamera.fieldOfView = this._shoulderCameraFov;
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x00048794 File Offset: 0x00046994
	private void FindShoulderCamera()
	{
		if (this._shoulderCamera != null)
		{
			return;
		}
		if (!GorillaTagger.hasInstance || !base.isActiveAndEnabled)
		{
			return;
		}
		this._shoulderCamera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>();
		this._shoulderCameraFov = this._shoulderCamera.fieldOfView;
	}

	// Token: 0x04001188 RID: 4488
	[SerializeField]
	private GTLckController _gtLckController;

	// Token: 0x04001189 RID: 4489
	private Camera _lckCamera;

	// Token: 0x0400118A RID: 4490
	private CameraMode _lckActiveCameraMode;

	// Token: 0x0400118B RID: 4491
	private Camera _shoulderCamera;

	// Token: 0x0400118C RID: 4492
	private float _shoulderCameraFov;
}
