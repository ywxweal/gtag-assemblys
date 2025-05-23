using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000391 RID: 913
public class PrivateUIRoom : MonoBehaviour
{
	// Token: 0x17000255 RID: 597
	// (get) Token: 0x06001519 RID: 5401 RVA: 0x0006618D File Offset: 0x0006438D
	private GTPlayer localPlayer
	{
		get
		{
			return GTPlayer.Instance;
		}
	}

	// Token: 0x0600151A RID: 5402 RVA: 0x00066CD8 File Offset: 0x00064ED8
	private void Awake()
	{
		if (PrivateUIRoom.instance == null)
		{
			PrivateUIRoom.instance = this;
			this.occluder.SetActive(false);
			this.leftHandObject.SetActive(false);
			this.rightHandObject.SetActive(false);
			this.ui = new List<Transform>();
			this.uiParents = new Dictionary<Transform, Transform>();
			this.backgroundDirectionPropertyID = Shader.PropertyToID(this.backgroundDirectionPropertyName);
			this._uiRoot = new GameObject("UIRoot").transform;
			this._uiRoot.parent = base.transform;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x0600151B RID: 5403 RVA: 0x00066D70 File Offset: 0x00064F70
	private void ToggleLevelVisibility(bool levelShouldBeVisible)
	{
		Camera component = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
		if (levelShouldBeVisible)
		{
			component.cullingMask = this.savedCullingLayers;
			return;
		}
		this.savedCullingLayers = component.cullingMask;
		component.cullingMask = this.visibleLayers;
	}

	// Token: 0x0600151C RID: 5404 RVA: 0x00066DBC File Offset: 0x00064FBC
	private static void StopOverlay()
	{
		PrivateUIRoom.instance.localPlayer.inOverlay = false;
		PrivateUIRoom.instance.inOverlay = false;
		PrivateUIRoom.instance.localPlayer.disableMovement = false;
		PrivateUIRoom.instance.localPlayer.InReportMenu = false;
		PrivateUIRoom.instance.ToggleLevelVisibility(true);
		PrivateUIRoom.instance.occluder.SetActive(false);
		PrivateUIRoom.instance.leftHandObject.SetActive(false);
		PrivateUIRoom.instance.rightHandObject.SetActive(false);
		AudioListener.volume = PrivateUIRoom.instance._initialAudioVolume;
		Debug.Log("[PrivateUIRoom::StopOverlay] Re-enabling Audio Listener");
	}

	// Token: 0x0600151D RID: 5405 RVA: 0x00066E58 File Offset: 0x00065058
	private void GetIdealScreenPositionRotation(out Vector3 position, out Quaternion rotation, out Vector3 scale)
	{
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
		scale = this.localPlayer.turnParent.transform.localScale;
		position = mainCamera.transform.position + rotation * Vector3.zero * scale.x;
	}

	// Token: 0x0600151E RID: 5406 RVA: 0x00066EE4 File Offset: 0x000650E4
	public static void AddUI(Transform focus)
	{
		if (PrivateUIRoom.instance.ui.Contains(focus))
		{
			return;
		}
		PrivateUIRoom.instance.uiParents.Add(focus, focus.parent);
		focus.gameObject.SetActive(false);
		focus.parent = PrivateUIRoom.instance._uiRoot;
		focus.localPosition = Vector3.zero;
		focus.localRotation = Quaternion.identity;
		PrivateUIRoom.instance.ui.Add(focus);
		if (PrivateUIRoom.instance.ui.Count == 1 && PrivateUIRoom.instance.focusTransform == null)
		{
			PrivateUIRoom.instance.focusTransform = PrivateUIRoom.instance.ui[0];
			PrivateUIRoom.instance.focusTransform.gameObject.SetActive(true);
			if (!PrivateUIRoom.instance.inOverlay)
			{
				PrivateUIRoom.StartOverlay();
			}
		}
	}

	// Token: 0x0600151F RID: 5407 RVA: 0x00066FC0 File Offset: 0x000651C0
	public static void RemoveUI(Transform focus)
	{
		if (!PrivateUIRoom.instance.ui.Contains(focus))
		{
			return;
		}
		focus.gameObject.SetActive(false);
		PrivateUIRoom.instance.ui.Remove(focus);
		if (PrivateUIRoom.instance.focusTransform == focus)
		{
			PrivateUIRoom.instance.focusTransform = null;
		}
		if (PrivateUIRoom.instance.uiParents[focus] != null)
		{
			focus.parent = PrivateUIRoom.instance.uiParents[focus];
			PrivateUIRoom.instance.uiParents.Remove(focus);
		}
		else
		{
			Object.Destroy(focus.gameObject);
		}
		if (PrivateUIRoom.instance.ui.Count > 0)
		{
			PrivateUIRoom.instance.focusTransform = PrivateUIRoom.instance.ui[0];
			PrivateUIRoom.instance.focusTransform.gameObject.SetActive(true);
			return;
		}
		if (!PrivateUIRoom.instance.overlayForcedActive)
		{
			PrivateUIRoom.StopOverlay();
		}
	}

	// Token: 0x06001520 RID: 5408 RVA: 0x000670B9 File Offset: 0x000652B9
	public static void ForceStartOverlay()
	{
		if (PrivateUIRoom.instance == null)
		{
			return;
		}
		PrivateUIRoom.instance.overlayForcedActive = true;
		if (PrivateUIRoom.instance.inOverlay)
		{
			return;
		}
		PrivateUIRoom.StartOverlay();
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x000670E6 File Offset: 0x000652E6
	public static void StopForcedOverlay()
	{
		if (PrivateUIRoom.instance == null)
		{
			return;
		}
		PrivateUIRoom.instance.overlayForcedActive = false;
		if (PrivateUIRoom.instance.ui.Count == 0 && PrivateUIRoom.instance.inOverlay)
		{
			PrivateUIRoom.StopOverlay();
		}
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x00067124 File Offset: 0x00065324
	private static void StartOverlay()
	{
		Vector3 vector;
		Quaternion quaternion;
		Vector3 vector2;
		PrivateUIRoom.instance.GetIdealScreenPositionRotation(out vector, out quaternion, out vector2);
		PrivateUIRoom.instance.leftHandObject.transform.localScale = vector2;
		PrivateUIRoom.instance.rightHandObject.transform.localScale = vector2;
		PrivateUIRoom.instance.occluder.transform.localScale = vector2;
		PrivateUIRoom.instance.localPlayer.InReportMenu = true;
		PrivateUIRoom.instance.localPlayer.disableMovement = true;
		PrivateUIRoom.instance.occluder.SetActive(true);
		PrivateUIRoom.instance.rightHandObject.SetActive(true);
		PrivateUIRoom.instance.leftHandObject.SetActive(true);
		PrivateUIRoom.instance.ToggleLevelVisibility(false);
		PrivateUIRoom.instance.localPlayer.inOverlay = true;
		PrivateUIRoom.instance.inOverlay = true;
		PrivateUIRoom.instance._initialAudioVolume = AudioListener.volume;
		AudioListener.volume = 0f;
		Debug.Log("[PrivateUIRoom::StartOverlay] Muting Audio Listener");
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x0006721C File Offset: 0x0006541C
	private void Update()
	{
		if (!this.localPlayer.InReportMenu)
		{
			return;
		}
		this.occluder.transform.position = GorillaTagger.Instance.mainCamera.transform.position;
		this.rightHandObject.transform.SetPositionAndRotation(this.localPlayer.rightControllerTransform.position, this.localPlayer.rightControllerTransform.rotation);
		this.leftHandObject.transform.SetPositionAndRotation(this.localPlayer.leftControllerTransform.position, this.localPlayer.leftControllerTransform.rotation);
		if (this.ShouldUpdateRotation())
		{
			this.UpdateUIPositionAndRotation();
			return;
		}
		if (this.ShouldUpdatePosition())
		{
			this.UpdateUIPosition();
		}
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x000672D8 File Offset: 0x000654D8
	private bool ShouldUpdateRotation()
	{
		float magnitude = (GorillaTagger.Instance.mainCamera.transform.position - this.lastStablePosition).X_Z().magnitude;
		Quaternion quaternion = Quaternion.Euler(0f, GorillaTagger.Instance.mainCamera.transform.rotation.eulerAngles.y, 0f);
		float num = Quaternion.Angle(this.lastStableRotation, quaternion);
		return magnitude > this.lateralPlay || num >= this.rotationalPlay;
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x00067365 File Offset: 0x00065565
	private bool ShouldUpdatePosition()
	{
		return Mathf.Abs(GorillaTagger.Instance.mainCamera.transform.position.y - this.lastStablePosition.y) > this.verticalPlay;
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x0006739C File Offset: 0x0006559C
	private void UpdateUIPositionAndRotation()
	{
		Transform transform = GorillaTagger.Instance.mainCamera.transform;
		this.lastStablePosition = transform.position;
		this.lastStableRotation = transform.rotation;
		Vector3 normalized = transform.forward.X_Z().normalized;
		this._uiRoot.SetPositionAndRotation(this.lastStablePosition + normalized * 0.02f, Quaternion.LookRotation(normalized));
		this.backgroundRenderer.material.SetVector(this.backgroundDirectionPropertyID, this.backgroundRenderer.transform.InverseTransformDirection(normalized));
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x00067438 File Offset: 0x00065638
	private void UpdateUIPosition()
	{
		Transform transform = GorillaTagger.Instance.mainCamera.transform;
		this.lastStablePosition = transform.position;
		this._uiRoot.position = this.lastStablePosition + this.lastStableRotation * new Vector3(0f, 0f, 0.02f);
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x00067496 File Offset: 0x00065696
	public static bool GetInOverlay()
	{
		return !(PrivateUIRoom.instance == null) && PrivateUIRoom.instance.inOverlay;
	}

	// Token: 0x04001777 RID: 6007
	[SerializeField]
	private GameObject occluder;

	// Token: 0x04001778 RID: 6008
	[SerializeField]
	private LayerMask visibleLayers;

	// Token: 0x04001779 RID: 6009
	[SerializeField]
	private GameObject leftHandObject;

	// Token: 0x0400177A RID: 6010
	[SerializeField]
	private GameObject rightHandObject;

	// Token: 0x0400177B RID: 6011
	[SerializeField]
	private MeshRenderer backgroundRenderer;

	// Token: 0x0400177C RID: 6012
	[SerializeField]
	private string backgroundDirectionPropertyName = "_SpotDirection";

	// Token: 0x0400177D RID: 6013
	private int backgroundDirectionPropertyID;

	// Token: 0x0400177E RID: 6014
	private int savedCullingLayers;

	// Token: 0x0400177F RID: 6015
	private Transform _uiRoot;

	// Token: 0x04001780 RID: 6016
	private Transform focusTransform;

	// Token: 0x04001781 RID: 6017
	private List<Transform> ui;

	// Token: 0x04001782 RID: 6018
	private Dictionary<Transform, Transform> uiParents;

	// Token: 0x04001783 RID: 6019
	private float _initialAudioVolume;

	// Token: 0x04001784 RID: 6020
	private bool inOverlay;

	// Token: 0x04001785 RID: 6021
	private bool overlayForcedActive;

	// Token: 0x04001786 RID: 6022
	private static PrivateUIRoom instance;

	// Token: 0x04001787 RID: 6023
	private Vector3 lastStablePosition;

	// Token: 0x04001788 RID: 6024
	private Quaternion lastStableRotation;

	// Token: 0x04001789 RID: 6025
	[SerializeField]
	private float verticalPlay = 0.1f;

	// Token: 0x0400178A RID: 6026
	[SerializeField]
	private float lateralPlay = 0.5f;

	// Token: 0x0400178B RID: 6027
	[SerializeField]
	private float rotationalPlay = 45f;
}
