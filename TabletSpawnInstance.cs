using System;
using UnityEngine;

// Token: 0x02000256 RID: 598
public class TabletSpawnInstance : IDisposable
{
	// Token: 0x14000023 RID: 35
	// (add) Token: 0x06000D91 RID: 3473 RVA: 0x0004658C File Offset: 0x0004478C
	// (remove) Token: 0x06000D92 RID: 3474 RVA: 0x000465C4 File Offset: 0x000447C4
	public event Action onGrabbed;

	// Token: 0x14000024 RID: 36
	// (add) Token: 0x06000D93 RID: 3475 RVA: 0x000465FC File Offset: 0x000447FC
	// (remove) Token: 0x06000D94 RID: 3476 RVA: 0x00046634 File Offset: 0x00044834
	public event Action onReleased;

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06000D95 RID: 3477 RVA: 0x00046669 File Offset: 0x00044869
	public LckDirectGrabbable directGrabbable
	{
		get
		{
			return this._lckSocialCameraManager.lckDirectGrabbable;
		}
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x00046676 File Offset: 0x00044876
	public bool ResetLocalPose()
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.localPosition = Vector3.zero;
		this._cameraSpawnInstanceTransform.localRotation = Quaternion.identity;
		return true;
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x000466A9 File Offset: 0x000448A9
	public bool ResetParent()
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.SetParent(this._cameraSpawnParentTransform);
		return true;
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x000466CD File Offset: 0x000448CD
	public bool SetParent(Transform transform)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return false;
		}
		this._cameraSpawnInstanceTransform.SetParent(transform);
		return true;
	}

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06000D99 RID: 3481 RVA: 0x000466EC File Offset: 0x000448EC
	// (set) Token: 0x06000D9A RID: 3482 RVA: 0x000466F4 File Offset: 0x000448F4
	public bool cameraActive
	{
		get
		{
			return this._cameraActive;
		}
		set
		{
			this._cameraActive = value;
			if (this._lckSocialCameraManager != null)
			{
				this._lckSocialCameraManager.cameraActive = this._cameraActive;
			}
		}
	}

	// Token: 0x17000155 RID: 341
	// (get) Token: 0x06000D9B RID: 3483 RVA: 0x0004671C File Offset: 0x0004491C
	// (set) Token: 0x06000D9C RID: 3484 RVA: 0x00046724 File Offset: 0x00044924
	public bool uiVisible
	{
		get
		{
			return this._uiVisible;
		}
		set
		{
			this._uiVisible = value;
			if (this._lckSocialCameraManager != null)
			{
				this._lckSocialCameraManager.uiVisible = this._uiVisible;
			}
		}
	}

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06000D9D RID: 3485 RVA: 0x0004674C File Offset: 0x0004494C
	public bool isSpawned
	{
		get
		{
			return this._cameraGameObjectInstance != null;
		}
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x0004675A File Offset: 0x0004495A
	public TabletSpawnInstance(GameObject cameraSpawnPrefab, Transform cameraSpawnParentTransform)
	{
		this._cameraSpawnPrefab = cameraSpawnPrefab;
		this._cameraSpawnParentTransform = cameraSpawnParentTransform;
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x00046770 File Offset: 0x00044970
	public void SpawnCamera()
	{
		if (!this.isSpawned)
		{
			this._cameraGameObjectInstance = Object.Instantiate<GameObject>(this._cameraSpawnPrefab, this._cameraSpawnParentTransform);
			this._lckSocialCameraManager = this._cameraGameObjectInstance.GetComponent<LckSocialCameraManager>();
			this._lckSocialCameraManager.lckDirectGrabbable.onGrabbed += delegate
			{
				Action action = this.onGrabbed;
				if (action == null)
				{
					return;
				}
				action();
			};
			this._lckSocialCameraManager.lckDirectGrabbable.onReleased += delegate
			{
				Action action2 = this.onReleased;
				if (action2 == null)
				{
					return;
				}
				action2();
			};
			this._cameraSpawnInstanceTransform = this._cameraGameObjectInstance.transform;
		}
		this.uiVisible = this.uiVisible;
		this.cameraActive = this.cameraActive;
	}

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06000DA0 RID: 3488 RVA: 0x0004680E File Offset: 0x00044A0E
	public Vector3 position
	{
		get
		{
			if (this._cameraSpawnInstanceTransform == null)
			{
				return Vector3.zero;
			}
			return this._cameraSpawnInstanceTransform.position;
		}
	}

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06000DA1 RID: 3489 RVA: 0x0004682F File Offset: 0x00044A2F
	public Quaternion rotation
	{
		get
		{
			if (this._cameraSpawnInstanceTransform == null)
			{
				return Quaternion.identity;
			}
			return this._cameraSpawnInstanceTransform.rotation;
		}
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x00046850 File Offset: 0x00044A50
	public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return;
		}
		this._cameraSpawnInstanceTransform.SetPositionAndRotation(position, rotation);
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x0004686E File Offset: 0x00044A6E
	public void SetLocalScale(Vector3 scale)
	{
		if (this._cameraSpawnInstanceTransform == null)
		{
			return;
		}
		this._cameraSpawnInstanceTransform.localScale = scale;
	}

	// Token: 0x06000DA4 RID: 3492 RVA: 0x0004688B File Offset: 0x00044A8B
	public void Dispose()
	{
		if (this._cameraGameObjectInstance != null)
		{
			Object.Destroy(this._cameraGameObjectInstance);
			this._cameraGameObjectInstance = null;
		}
	}

	// Token: 0x04001122 RID: 4386
	private GameObject _cameraGameObjectInstance;

	// Token: 0x04001123 RID: 4387
	private GameObject _cameraSpawnPrefab;

	// Token: 0x04001124 RID: 4388
	private GameEvents _GtCamera;

	// Token: 0x04001125 RID: 4389
	private Transform _cameraSpawnParentTransform;

	// Token: 0x04001126 RID: 4390
	private Transform _cameraSpawnInstanceTransform;

	// Token: 0x04001127 RID: 4391
	private LckSocialCameraManager _lckSocialCameraManager;

	// Token: 0x04001128 RID: 4392
	private bool _cameraActive;

	// Token: 0x04001129 RID: 4393
	private bool _uiVisible;
}
