using System;
using Liv.Lck;
using Liv.Lck.GorillaTag;
using UnityEngine;

// Token: 0x02000264 RID: 612
public class LckSocialCameraManager : MonoBehaviour
{
	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000DFD RID: 3581 RVA: 0x00047A6E File Offset: 0x00045C6E
	public LckDirectGrabbable lckDirectGrabbable
	{
		get
		{
			return this._lckDirectGrabbable;
		}
	}

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x06000DFE RID: 3582 RVA: 0x00047A76 File Offset: 0x00045C76
	public static LckSocialCameraManager Instance
	{
		get
		{
			return LckSocialCameraManager._instance;
		}
	}

	// Token: 0x06000DFF RID: 3583 RVA: 0x00047A7D File Offset: 0x00045C7D
	private void Awake()
	{
		this.SetManagerInstance();
		this._lckCamera = this._gtLckController.GetActiveCamera();
	}

	// Token: 0x06000E00 RID: 3584 RVA: 0x00047A96 File Offset: 0x00045C96
	public void SetLckSocialCamera(LckSocialCamera socialCamera)
	{
		this._socialCameraInstance = socialCamera;
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x00047A9F File Offset: 0x00045C9F
	private void SetManagerInstance()
	{
		LckSocialCameraManager._instance = this;
		Action<LckSocialCameraManager> onManagerSpawned = LckSocialCameraManager.OnManagerSpawned;
		if (onManagerSpawned == null)
		{
			return;
		}
		onManagerSpawned(this);
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x00047AB8 File Offset: 0x00045CB8
	private void OnEnable()
	{
		LckResult<LckService> service = LckService.GetService();
		if (service.Result != null)
		{
			service.Result.OnRecordingStarted += this.OnRecordingStarted;
			service.Result.OnRecordingStopped += this.OnRecordingStopped;
		}
		this._gtLckController.OnCameraModeChanged += this.OnCameraModeChanged;
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x00047B18 File Offset: 0x00045D18
	private void Update()
	{
		if (this._socialCameraInstance != null)
		{
			if (this._lckCamera != null)
			{
				Transform transform = this._lckCamera.transform;
				this._socialCameraInstance.transform.position = transform.position;
				this._socialCameraInstance.transform.rotation = transform.rotation;
				Camera main = Camera.main;
				if (main != null)
				{
					this._lckCamera.nearClipPlane = main.nearClipPlane;
					this._lckCamera.farClipPlane = main.farClipPlane;
				}
			}
			CameraMode cameraMode = this._lckActiveCameraMode;
			if (cameraMode != CameraMode.Selfie)
			{
				if (cameraMode == CameraMode.ThirdPerson)
				{
					this._socialCameraInstance.visible = this.cameraActive;
				}
				else
				{
					this._socialCameraInstance.visible = false;
				}
			}
			else
			{
				this._socialCameraInstance.visible = this.cameraActive;
			}
			this._socialCameraInstance.recording = this._recording;
		}
		if (this.CoconutCamera.gameObject.activeSelf)
		{
			CameraMode cameraMode = this._lckActiveCameraMode;
			if (cameraMode != CameraMode.Selfie)
			{
				if (cameraMode == CameraMode.ThirdPerson)
				{
					this.CoconutCamera.SetVisualsActive(this.cameraActive);
				}
				else
				{
					this.CoconutCamera.SetVisualsActive(false);
				}
			}
			else
			{
				this.CoconutCamera.SetVisualsActive(false);
			}
			this.CoconutCamera.SetRecordingState(this._recording);
		}
	}

	// Token: 0x06000E04 RID: 3588 RVA: 0x00047C5C File Offset: 0x00045E5C
	private void OnDisable()
	{
		LckResult<LckService> service = LckService.GetService();
		if (service.Result != null)
		{
			service.Result.OnRecordingStarted -= this.OnRecordingStarted;
			service.Result.OnRecordingStopped -= this.OnRecordingStopped;
		}
		this._gtLckController.OnCameraModeChanged -= this.OnCameraModeChanged;
	}

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06000E05 RID: 3589 RVA: 0x00047CBC File Offset: 0x00045EBC
	// (set) Token: 0x06000E06 RID: 3590 RVA: 0x00047CC9 File Offset: 0x00045EC9
	public bool cameraActive
	{
		get
		{
			return this._localCameras.activeSelf;
		}
		set
		{
			this._localCameras.SetActive(value);
			if (!value)
			{
				this._gtLckController.StopRecording();
			}
		}
	}

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000E07 RID: 3591 RVA: 0x00047CE6 File Offset: 0x00045EE6
	// (set) Token: 0x06000E08 RID: 3592 RVA: 0x00047CF3 File Offset: 0x00045EF3
	public bool uiVisible
	{
		get
		{
			return this._localUi.activeSelf;
		}
		set
		{
			this._localUi.SetActive(value);
		}
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x00047D01 File Offset: 0x00045F01
	private void OnRecordingStarted(LckResult result)
	{
		this._recording = result.Success;
	}

	// Token: 0x06000E0A RID: 3594 RVA: 0x00047D0F File Offset: 0x00045F0F
	private void OnRecordingStopped(LckResult result)
	{
		this._recording = false;
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x00047D18 File Offset: 0x00045F18
	private void OnCameraModeChanged(CameraMode mode, ILckCamera lckCamera)
	{
		this._lckCamera = lckCamera.GetCameraComponent();
		this._lckActiveCameraMode = mode;
	}

	// Token: 0x04001166 RID: 4454
	[SerializeField]
	private GameObject _localUi;

	// Token: 0x04001167 RID: 4455
	[SerializeField]
	private GameObject _localCameras;

	// Token: 0x04001168 RID: 4456
	[SerializeField]
	private GTLckController _gtLckController;

	// Token: 0x04001169 RID: 4457
	[SerializeField]
	private LckDirectGrabbable _lckDirectGrabbable;

	// Token: 0x0400116A RID: 4458
	[SerializeField]
	public CoconutCamera CoconutCamera;

	// Token: 0x0400116B RID: 4459
	private LckSocialCamera _socialCameraInstance;

	// Token: 0x0400116C RID: 4460
	private Camera _lckCamera;

	// Token: 0x0400116D RID: 4461
	private CameraMode _lckActiveCameraMode;

	// Token: 0x0400116E RID: 4462
	[OnEnterPlay_SetNull]
	private static LckSocialCameraManager _instance;

	// Token: 0x0400116F RID: 4463
	public static Action<LckSocialCameraManager> OnManagerSpawned;

	// Token: 0x04001170 RID: 4464
	private bool _recording;
}
