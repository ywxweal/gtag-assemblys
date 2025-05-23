using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x0200037C RID: 892
[RequireComponent(typeof(SpatialAnchorLoader))]
public class AnchorUIManager : MonoBehaviour
{
	// Token: 0x17000251 RID: 593
	// (get) Token: 0x060014A3 RID: 5283 RVA: 0x00064D8C File Offset: 0x00062F8C
	public Anchor AnchorPrefab
	{
		get
		{
			return this._anchorPrefab;
		}
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x00064D94 File Offset: 0x00062F94
	private void Awake()
	{
		if (AnchorUIManager.Instance == null)
		{
			AnchorUIManager.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x00064DB0 File Offset: 0x00062FB0
	private void Start()
	{
		this._raycastOrigin = this._trackedDevice;
		this._mode = AnchorUIManager.AnchorMode.Select;
		this.StartSelectMode();
		this._menuIndex = 0;
		this._selectedButton = this._buttonList[0];
		this._selectedButton.OnSelect(null);
		this._lineRenderer.startWidth = 0.005f;
		this._lineRenderer.endWidth = 0.005f;
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x00064E1C File Offset: 0x0006301C
	private void Update()
	{
		if (this._drawRaycast)
		{
			this.ControllerRaycast();
		}
		if (this._selectedAnchor == null)
		{
			this._selectedButton.OnSelect(null);
			this._isFocused = true;
		}
		this.HandleMenuNavigation();
		if (OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.Active))
		{
			AnchorUIManager.PrimaryPressDelegate primaryPressDelegate = this._primaryPressDelegate;
			if (primaryPressDelegate == null)
			{
				return;
			}
			primaryPressDelegate();
		}
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x00064E7B File Offset: 0x0006307B
	public void OnCreateModeButtonPressed()
	{
		this.ToggleCreateMode();
		this._createModeButton.SetActive(!this._createModeButton.activeSelf);
		this._selectModeButton.SetActive(!this._selectModeButton.activeSelf);
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x00064EB5 File Offset: 0x000630B5
	public void OnLoadAnchorsButtonPressed()
	{
		base.GetComponent<SpatialAnchorLoader>().LoadAnchorsByUuid();
	}

	// Token: 0x060014A9 RID: 5289 RVA: 0x00064EC2 File Offset: 0x000630C2
	private void ToggleCreateMode()
	{
		if (this._mode == AnchorUIManager.AnchorMode.Select)
		{
			this._mode = AnchorUIManager.AnchorMode.Create;
			this.EndSelectMode();
			this.StartPlacementMode();
			return;
		}
		this._mode = AnchorUIManager.AnchorMode.Select;
		this.EndPlacementMode();
		this.StartSelectMode();
	}

	// Token: 0x060014AA RID: 5290 RVA: 0x00064EF4 File Offset: 0x000630F4
	private void StartPlacementMode()
	{
		this.ShowAnchorPreview();
		this._primaryPressDelegate = new AnchorUIManager.PrimaryPressDelegate(this.PlaceAnchor);
	}

	// Token: 0x060014AB RID: 5291 RVA: 0x00064F0E File Offset: 0x0006310E
	private void EndPlacementMode()
	{
		this.HideAnchorPreview();
		this._primaryPressDelegate = null;
	}

	// Token: 0x060014AC RID: 5292 RVA: 0x00064F1D File Offset: 0x0006311D
	private void StartSelectMode()
	{
		this.ShowRaycastLine();
		this._primaryPressDelegate = new AnchorUIManager.PrimaryPressDelegate(this.SelectAnchor);
	}

	// Token: 0x060014AD RID: 5293 RVA: 0x00064F37 File Offset: 0x00063137
	private void EndSelectMode()
	{
		this.HideRaycastLine();
		this._primaryPressDelegate = null;
	}

	// Token: 0x060014AE RID: 5294 RVA: 0x00064F48 File Offset: 0x00063148
	private void HandleMenuNavigation()
	{
		if (!this._isFocused)
		{
			return;
		}
		if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickUp, OVRInput.Controller.Active))
		{
			this.NavigateToIndexInMenu(false);
		}
		if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickDown, OVRInput.Controller.Active))
		{
			this.NavigateToIndexInMenu(true);
		}
		if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger, OVRInput.Controller.Active))
		{
			this._selectedButton.OnSubmit(null);
		}
	}

	// Token: 0x060014AF RID: 5295 RVA: 0x00064FAC File Offset: 0x000631AC
	private void NavigateToIndexInMenu(bool moveNext)
	{
		if (moveNext)
		{
			this._menuIndex++;
			if (this._menuIndex > this._buttonList.Count - 1)
			{
				this._menuIndex = 0;
			}
		}
		else
		{
			this._menuIndex--;
			if (this._menuIndex < 0)
			{
				this._menuIndex = this._buttonList.Count - 1;
			}
		}
		this._selectedButton.OnDeselect(null);
		this._selectedButton = this._buttonList[this._menuIndex];
		this._selectedButton.OnSelect(null);
	}

	// Token: 0x060014B0 RID: 5296 RVA: 0x00065041 File Offset: 0x00063241
	private void ShowAnchorPreview()
	{
		this._placementPreview.SetActive(true);
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x0006504F File Offset: 0x0006324F
	private void HideAnchorPreview()
	{
		this._placementPreview.SetActive(false);
	}

	// Token: 0x060014B2 RID: 5298 RVA: 0x0006505D File Offset: 0x0006325D
	private void PlaceAnchor()
	{
		Object.Instantiate<Anchor>(this._anchorPrefab, this._anchorPlacementTransform.position, this._anchorPlacementTransform.rotation);
	}

	// Token: 0x060014B3 RID: 5299 RVA: 0x00065081 File Offset: 0x00063281
	private void ShowRaycastLine()
	{
		this._drawRaycast = true;
		this._lineRenderer.gameObject.SetActive(true);
	}

	// Token: 0x060014B4 RID: 5300 RVA: 0x0006509B File Offset: 0x0006329B
	private void HideRaycastLine()
	{
		this._drawRaycast = false;
		this._lineRenderer.gameObject.SetActive(false);
	}

	// Token: 0x060014B5 RID: 5301 RVA: 0x000650B8 File Offset: 0x000632B8
	private void ControllerRaycast()
	{
		Ray ray = new Ray(this._raycastOrigin.position, this._raycastOrigin.TransformDirection(Vector3.forward));
		this._lineRenderer.SetPosition(0, this._raycastOrigin.position);
		this._lineRenderer.SetPosition(1, this._raycastOrigin.position + this._raycastOrigin.TransformDirection(Vector3.forward) * 10f);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity))
		{
			Anchor component = raycastHit.collider.GetComponent<Anchor>();
			if (component != null)
			{
				this._lineRenderer.SetPosition(1, raycastHit.point);
				this.HoverAnchor(component);
				return;
			}
		}
		this.UnhoverAnchor();
	}

	// Token: 0x060014B6 RID: 5302 RVA: 0x00065177 File Offset: 0x00063377
	private void HoverAnchor(Anchor anchor)
	{
		this._hoveredAnchor = anchor;
		this._hoveredAnchor.OnHoverStart();
	}

	// Token: 0x060014B7 RID: 5303 RVA: 0x0006518B File Offset: 0x0006338B
	private void UnhoverAnchor()
	{
		if (this._hoveredAnchor == null)
		{
			return;
		}
		this._hoveredAnchor.OnHoverEnd();
		this._hoveredAnchor = null;
	}

	// Token: 0x060014B8 RID: 5304 RVA: 0x000651B0 File Offset: 0x000633B0
	private void SelectAnchor()
	{
		if (this._hoveredAnchor != null)
		{
			if (this._selectedAnchor != null)
			{
				this._selectedAnchor.OnSelect();
				this._selectedAnchor = null;
			}
			this._selectedAnchor = this._hoveredAnchor;
			this._selectedAnchor.OnSelect();
			this._selectedButton.OnDeselect(null);
			this._isFocused = false;
			return;
		}
		if (this._selectedAnchor != null)
		{
			this._selectedAnchor.OnSelect();
			this._selectedAnchor = null;
			this._selectedButton.OnSelect(null);
			this._isFocused = true;
		}
	}

	// Token: 0x040016FE RID: 5886
	public static AnchorUIManager Instance;

	// Token: 0x040016FF RID: 5887
	[SerializeField]
	[FormerlySerializedAs("createModeButton_")]
	private GameObject _createModeButton;

	// Token: 0x04001700 RID: 5888
	[SerializeField]
	[FormerlySerializedAs("selectModeButton_")]
	private GameObject _selectModeButton;

	// Token: 0x04001701 RID: 5889
	[SerializeField]
	[FormerlySerializedAs("trackedDevice_")]
	private Transform _trackedDevice;

	// Token: 0x04001702 RID: 5890
	private Transform _raycastOrigin;

	// Token: 0x04001703 RID: 5891
	private bool _drawRaycast;

	// Token: 0x04001704 RID: 5892
	[SerializeField]
	[FormerlySerializedAs("lineRenderer_")]
	private LineRenderer _lineRenderer;

	// Token: 0x04001705 RID: 5893
	private Anchor _hoveredAnchor;

	// Token: 0x04001706 RID: 5894
	private Anchor _selectedAnchor;

	// Token: 0x04001707 RID: 5895
	private AnchorUIManager.AnchorMode _mode = AnchorUIManager.AnchorMode.Select;

	// Token: 0x04001708 RID: 5896
	[SerializeField]
	[FormerlySerializedAs("buttonList_")]
	private List<Button> _buttonList;

	// Token: 0x04001709 RID: 5897
	private int _menuIndex;

	// Token: 0x0400170A RID: 5898
	private Button _selectedButton;

	// Token: 0x0400170B RID: 5899
	[SerializeField]
	private Anchor _anchorPrefab;

	// Token: 0x0400170C RID: 5900
	[SerializeField]
	[FormerlySerializedAs("placementPreview_")]
	private GameObject _placementPreview;

	// Token: 0x0400170D RID: 5901
	[SerializeField]
	[FormerlySerializedAs("anchorPlacementTransform_")]
	private Transform _anchorPlacementTransform;

	// Token: 0x0400170E RID: 5902
	private AnchorUIManager.PrimaryPressDelegate _primaryPressDelegate;

	// Token: 0x0400170F RID: 5903
	private bool _isFocused = true;

	// Token: 0x0200037D RID: 893
	public enum AnchorMode
	{
		// Token: 0x04001711 RID: 5905
		Create,
		// Token: 0x04001712 RID: 5906
		Select
	}

	// Token: 0x0200037E RID: 894
	// (Invoke) Token: 0x060014BB RID: 5307
	private delegate void PrimaryPressDelegate();
}
