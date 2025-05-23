using System;
using Liv.Lck;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000257 RID: 599
public class LckBodyCameraSpawner : MonoBehaviour
{
	// Token: 0x06000DA7 RID: 3495 RVA: 0x000468D1 File Offset: 0x00044AD1
	public void SetFollowTransform(Transform transform)
	{
		this._followTransform = transform;
	}

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x06000DA8 RID: 3496 RVA: 0x000468DA File Offset: 0x00044ADA
	public TabletSpawnInstance tabletSpawnInstance
	{
		get
		{
			return this._tabletSpawnInstance;
		}
	}

	// Token: 0x14000025 RID: 37
	// (add) Token: 0x06000DA9 RID: 3497 RVA: 0x000468E4 File Offset: 0x00044AE4
	// (remove) Token: 0x06000DAA RID: 3498 RVA: 0x00046918 File Offset: 0x00044B18
	public static event LckBodyCameraSpawner.CameraStateDelegate OnCameraStateChange;

	// Token: 0x1700015A RID: 346
	// (get) Token: 0x06000DAB RID: 3499 RVA: 0x0004694B File Offset: 0x00044B4B
	// (set) Token: 0x06000DAC RID: 3500 RVA: 0x00046954 File Offset: 0x00044B54
	public LckBodyCameraSpawner.CameraState cameraState
	{
		get
		{
			return this._cameraState;
		}
		set
		{
			switch (value)
			{
			case LckBodyCameraSpawner.CameraState.CameraDisabled:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.NotVisible;
				this._tabletSpawnInstance.uiVisible = false;
				this._tabletSpawnInstance.cameraActive = false;
				this.ResetCameraModel();
				this.cameraVisible = false;
				this._shouldMoveCameraToNeck = false;
				break;
			case LckBodyCameraSpawner.CameraState.CameraOnNeck:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
				this._tabletSpawnInstance.uiVisible = false;
				this._tabletSpawnInstance.cameraActive = true;
				this.ResetCameraModel();
				if (Application.platform == RuntimePlatform.Android)
				{
					this.SetPreviewActive(false);
				}
				this.cameraVisible = true;
				this._shouldMoveCameraToNeck = false;
				this._dummyTablet.SetTabletIsSpawned(false);
				break;
			case LckBodyCameraSpawner.CameraState.CameraSpawned:
				this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
				this._tabletSpawnInstance.uiVisible = true;
				this._tabletSpawnInstance.cameraActive = true;
				if (Application.platform == RuntimePlatform.Android)
				{
					this.SetPreviewActive(true);
				}
				this.ResetCameraModel();
				this.cameraVisible = true;
				this._shouldMoveCameraToNeck = false;
				this._dummyTablet.SetTabletIsSpawned(true);
				break;
			}
			this._cameraState = value;
			LckBodyCameraSpawner.CameraStateDelegate onCameraStateChange = LckBodyCameraSpawner.OnCameraStateChange;
			if (onCameraStateChange == null)
			{
				return;
			}
			onCameraStateChange(this._cameraState);
		}
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x00046A6C File Offset: 0x00044C6C
	private void SetPreviewActive(bool isActive)
	{
		LckResult<LckService> service = LckService.GetService();
		if (!service.Success)
		{
			Debug.LogError("LCK Could not get Service" + service.Error.ToString());
			return;
		}
		LckService result = service.Result;
		if (result == null)
		{
			return;
		}
		result.SetPreviewActive(isActive);
	}

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x06000DAE RID: 3502 RVA: 0x00046ABD File Offset: 0x00044CBD
	// (set) Token: 0x06000DAF RID: 3503 RVA: 0x00046AC8 File Offset: 0x00044CC8
	public LckBodyCameraSpawner.CameraPosition cameraPosition
	{
		get
		{
			return this._cameraPosition;
		}
		set
		{
			if (this._cameraModelTransform != null && this._cameraPosition != value)
			{
				switch (value)
				{
				case LckBodyCameraSpawner.CameraPosition.CameraDefault:
					this.ChangeCameraModelParent(this._cameraPositionDefault);
					this._cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
					return;
				case LckBodyCameraSpawner.CameraPosition.CameraSlingshot:
					this.ChangeCameraModelParent(this._cameraPositionSlingshot);
					this._cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraSlingshot;
					break;
				case LckBodyCameraSpawner.CameraPosition.NotVisible:
					break;
				default:
					return;
				}
			}
		}
	}

	// Token: 0x1700015C RID: 348
	// (get) Token: 0x06000DB0 RID: 3504 RVA: 0x00046B26 File Offset: 0x00044D26
	// (set) Token: 0x06000DB1 RID: 3505 RVA: 0x00046B38 File Offset: 0x00044D38
	private bool cameraVisible
	{
		get
		{
			return this._cameraModelTransform.gameObject.activeSelf;
		}
		set
		{
			this._cameraModelTransform.gameObject.SetActive(value);
			this._cameraStrapRenderer.enabled = value;
		}
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x00046B57 File Offset: 0x00044D57
	private void Awake()
	{
		this._tabletSpawnInstance = new TabletSpawnInstance(this._cameraSpawnPrefab, this._cameraSpawnParentTransform);
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x00046B70 File Offset: 0x00044D70
	private void OnEnable()
	{
		this.InitCameraStrap();
		this.cameraState = LckBodyCameraSpawner.CameraState.CameraDisabled;
		this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
		ZoneManagement.OnZoneChange += this.OnZoneChanged;
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x00046B98 File Offset: 0x00044D98
	private void Update()
	{
		if (this._followTransform != null && base.transform.parent != null)
		{
			Matrix4x4 localToWorldMatrix = base.transform.parent.localToWorldMatrix;
			Vector3 vector = localToWorldMatrix.MultiplyPoint(this._followTransform.localPosition + this._followTransform.localRotation * new Vector3(0f, -0.05f, 0.1f));
			Quaternion quaternion = Quaternion.LookRotation(localToWorldMatrix.MultiplyVector(this._followTransform.localRotation * Vector3.forward), localToWorldMatrix.MultiplyVector(this._followTransform.localRotation * Vector3.up));
			base.transform.SetPositionAndRotation(vector, quaternion);
		}
		LckBodyCameraSpawner.CameraState cameraState = this._cameraState;
		if (cameraState != LckBodyCameraSpawner.CameraState.CameraOnNeck)
		{
			if (cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned)
			{
				this.UpdateCameraStrap();
				if (this._cameraModelGrabbable.isGrabbed)
				{
					GorillaGrabber grabber = this._cameraModelGrabbable.grabber;
					Transform transform = grabber.transform;
					if (this.ShouldSpawnCamera(transform))
					{
						this.SpawnCamera(grabber, transform);
					}
				}
				else
				{
					this.ResetCameraModel();
				}
				if (this._tabletSpawnInstance.isSpawned)
				{
					Transform transform3;
					if (this._tabletSpawnInstance.directGrabbable.isGrabbed)
					{
						GorillaGrabber grabber2 = this._tabletSpawnInstance.directGrabbable.grabber;
						Transform transform2 = grabber2.transform;
						if (!this.ShouldSpawnCamera(transform2))
						{
							this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
							this._cameraModelGrabbable.target.SetPositionAndRotation(transform2.position, transform2.rotation * Quaternion.Euler(0f, 180f, 0f));
							this._tabletSpawnInstance.directGrabbable.ForceRelease();
							this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
							this._tabletSpawnInstance.ResetLocalPose();
							this._cameraModelGrabbable.ForceGrab(grabber2);
							this._cameraModelGrabbable.onReleased += this.OnCameraModelReleased;
						}
					}
					else if (this._shouldMoveCameraToNeck && GtTag.TryGetTransform(GtTagType.HMD, out transform3) && Vector3.SqrMagnitude(transform3.position - this.tabletSpawnInstance.position) >= this._snapToNeckDistance * this._snapToNeckDistance)
					{
						this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
						this._tabletSpawnInstance.SetParent(this._cameraModelTransform);
						this._tabletSpawnInstance.ResetLocalPose();
						this._shouldMoveCameraToNeck = false;
					}
				}
			}
		}
		else
		{
			this.UpdateCameraStrap();
			if (this._cameraModelGrabbable.isGrabbed)
			{
				GorillaGrabber grabber3 = this._cameraModelGrabbable.grabber;
				Transform transform4 = grabber3.transform;
				if (this.ShouldSpawnCamera(transform4))
				{
					this.SpawnCamera(grabber3, transform4);
				}
			}
			else
			{
				this.ResetCameraModel();
			}
		}
		if (!this.IsSlingshotActiveInHierarchy())
		{
			this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraDefault;
			return;
		}
		this.cameraPosition = LckBodyCameraSpawner.CameraPosition.CameraSlingshot;
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x00046E6C File Offset: 0x0004506C
	private void OnDisable()
	{
		ZoneManagement.OnZoneChange -= this.OnZoneChanged;
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x00046E7F File Offset: 0x0004507F
	private void OnDestroy()
	{
		this._tabletSpawnInstance.Dispose();
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x00046E8C File Offset: 0x0004508C
	public void ManuallySetCameraOnNeck()
	{
		if (this._tabletSpawnInstance.isSpawned)
		{
			this.cameraState = LckBodyCameraSpawner.CameraState.CameraOnNeck;
		}
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x00046EA2 File Offset: 0x000450A2
	private void OnZoneChanged(ZoneData[] zones)
	{
		if (!this._tabletSpawnInstance.isSpawned || this._tabletSpawnInstance.directGrabbable.isGrabbed)
		{
			return;
		}
		this._shouldMoveCameraToNeck = true;
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x00046ECB File Offset: 0x000450CB
	private void OnCameraModelReleased()
	{
		this._cameraModelGrabbable.onReleased -= this.OnCameraModelReleased;
		this.ResetCameraModel();
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x00046EEC File Offset: 0x000450EC
	public void SpawnCamera(GorillaGrabber overrideGorillaGrabber, Transform transform)
	{
		if (!this._tabletSpawnInstance.isSpawned)
		{
			this._tabletSpawnInstance.SpawnCamera();
		}
		Vector3 zero = Vector3.zero;
		XRNode xrNode = overrideGorillaGrabber.XrNode;
		if (xrNode != XRNode.LeftHand)
		{
			if (xrNode == XRNode.RightHand)
			{
				zero = new Vector3(0.25f, -0.12f, 0.03f);
			}
		}
		else
		{
			zero = new Vector3(-0.25f, -0.12f, 0.03f);
		}
		this.cameraState = LckBodyCameraSpawner.CameraState.CameraSpawned;
		this._cameraModelGrabbable.ForceRelease();
		this._tabletSpawnInstance.ResetParent();
		Quaternion quaternion = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
		Matrix4x4 matrix4x = transform.localToWorldMatrix;
		matrix4x *= Matrix4x4.Rotate(Quaternion.Euler(0f, 180f, 0f));
		this._tabletSpawnInstance.SetPositionAndRotation(matrix4x.MultiplyPoint(zero), quaternion);
		this._tabletSpawnInstance.directGrabbable.ForceGrab(overrideGorillaGrabber);
		this._tabletSpawnInstance.SetLocalScale(Vector3.one);
	}

	// Token: 0x06000DBB RID: 3515 RVA: 0x00046FF0 File Offset: 0x000451F0
	private bool ShouldSpawnCamera(Transform gorillaGrabberTransform)
	{
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		Vector3 vector = worldToLocalMatrix.MultiplyPoint(this._cameraModelOriginTransform.position);
		Vector3 vector2 = worldToLocalMatrix.MultiplyPoint(gorillaGrabberTransform.position);
		return Vector3.SqrMagnitude(vector - vector2) >= this._activateDistance * this._activateDistance;
	}

	// Token: 0x06000DBC RID: 3516 RVA: 0x00047048 File Offset: 0x00045248
	private void ChangeCameraModelParent(Transform transform)
	{
		if (this._cameraModelTransform != null)
		{
			this._cameraModelGrabbable.SetOriginalTargetParent(transform);
			if (!this._cameraModelGrabbable.isGrabbed)
			{
				this._cameraModelTransform.transform.parent = transform;
				this._cameraModelTransform.transform.localPosition = Vector3.zero;
			}
		}
	}

	// Token: 0x06000DBD RID: 3517 RVA: 0x000470A2 File Offset: 0x000452A2
	private void InitCameraStrap()
	{
		this._cameraStrapRenderer.positionCount = this._cameraStrapPoints.Length;
		this._cameraStrapPositions = new Vector3[this._cameraStrapPoints.Length];
	}

	// Token: 0x06000DBE RID: 3518 RVA: 0x000470CC File Offset: 0x000452CC
	private void UpdateCameraStrap()
	{
		for (int i = 0; i < this._cameraStrapPoints.Length; i++)
		{
			this._cameraStrapPositions[i] = this._cameraStrapPoints[i].position;
		}
		this._cameraStrapRenderer.SetPositions(this._cameraStrapPositions);
		Vector3 lossyScale = base.transform.lossyScale;
		float num = (lossyScale.x + lossyScale.y + lossyScale.z) * 0.3333333f;
		this._cameraStrapRenderer.widthMultiplier = num * 0.02f;
		Color color = ((this.cameraState == LckBodyCameraSpawner.CameraState.CameraSpawned) ? this._ghostColor : this._normalColor);
		this._cameraStrapRenderer.startColor = color;
		this._cameraStrapRenderer.endColor = color;
	}

	// Token: 0x06000DBF RID: 3519 RVA: 0x0004717F File Offset: 0x0004537F
	private void ResetCameraModel()
	{
		this._cameraModelTransform.localPosition = Vector3.zero;
		this._cameraModelTransform.localRotation = Quaternion.identity;
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x000471A1 File Offset: 0x000453A1
	private VRRig GetLocalRig()
	{
		if (this._localRig == null)
		{
			this._localRig = VRRigCache.Instance.localRig.Rig;
		}
		return this._localRig;
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x000471CC File Offset: 0x000453CC
	private bool IsSlingshotHeldInHand(out bool leftHand, out bool rightHand)
	{
		VRRig localRig = this.GetLocalRig();
		if (localRig == null)
		{
			leftHand = false;
			rightHand = false;
			return false;
		}
		leftHand = localRig.projectileWeapon.InLeftHand();
		rightHand = localRig.projectileWeapon.InRightHand();
		return localRig.projectileWeapon.InHand();
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x00047218 File Offset: 0x00045418
	private bool IsSlingshotActiveInHierarchy()
	{
		VRRig localRig = this.GetLocalRig();
		return !(localRig == null) && !(localRig.projectileWeapon == null) && localRig.projectileWeapon.gameObject.activeInHierarchy;
	}

	// Token: 0x0400112A RID: 4394
	[SerializeField]
	private GameObject _cameraSpawnPrefab;

	// Token: 0x0400112B RID: 4395
	[SerializeField]
	private Transform _cameraSpawnParentTransform;

	// Token: 0x0400112C RID: 4396
	[SerializeField]
	private Transform _cameraModelOriginTransform;

	// Token: 0x0400112D RID: 4397
	[SerializeField]
	private Transform _cameraModelTransform;

	// Token: 0x0400112E RID: 4398
	[SerializeField]
	private LckDirectGrabbable _cameraModelGrabbable;

	// Token: 0x0400112F RID: 4399
	[SerializeField]
	private Transform _cameraPositionDefault;

	// Token: 0x04001130 RID: 4400
	[SerializeField]
	private Transform _cameraPositionSlingshot;

	// Token: 0x04001131 RID: 4401
	[SerializeField]
	private float _activateDistance = 0.25f;

	// Token: 0x04001132 RID: 4402
	[SerializeField]
	private float _snapToNeckDistance = 15f;

	// Token: 0x04001133 RID: 4403
	[SerializeField]
	private LineRenderer _cameraStrapRenderer;

	// Token: 0x04001134 RID: 4404
	[SerializeField]
	private Transform[] _cameraStrapPoints;

	// Token: 0x04001135 RID: 4405
	[SerializeField]
	private Color _normalColor = Color.red;

	// Token: 0x04001136 RID: 4406
	[SerializeField]
	private Color _ghostColor = Color.gray;

	// Token: 0x04001137 RID: 4407
	[SerializeField]
	private GtDummyTablet _dummyTablet;

	// Token: 0x04001138 RID: 4408
	private Transform _followTransform;

	// Token: 0x04001139 RID: 4409
	private Vector3[] _cameraStrapPositions;

	// Token: 0x0400113A RID: 4410
	private TabletSpawnInstance _tabletSpawnInstance;

	// Token: 0x0400113B RID: 4411
	private VRRig _localRig;

	// Token: 0x0400113C RID: 4412
	private bool _shouldMoveCameraToNeck;

	// Token: 0x0400113E RID: 4414
	private LckBodyCameraSpawner.CameraState _cameraState;

	// Token: 0x0400113F RID: 4415
	private LckBodyCameraSpawner.CameraPosition _cameraPosition;

	// Token: 0x02000258 RID: 600
	public enum CameraState
	{
		// Token: 0x04001141 RID: 4417
		CameraDisabled,
		// Token: 0x04001142 RID: 4418
		CameraOnNeck,
		// Token: 0x04001143 RID: 4419
		CameraSpawned
	}

	// Token: 0x02000259 RID: 601
	public enum CameraPosition
	{
		// Token: 0x04001145 RID: 4421
		CameraDefault,
		// Token: 0x04001146 RID: 4422
		CameraSlingshot,
		// Token: 0x04001147 RID: 4423
		NotVisible
	}

	// Token: 0x0200025A RID: 602
	// (Invoke) Token: 0x06000DC5 RID: 3525
	public delegate void CameraStateDelegate(LckBodyCameraSpawner.CameraState state);
}
