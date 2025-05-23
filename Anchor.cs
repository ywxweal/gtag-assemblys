using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x0200037A RID: 890
[RequireComponent(typeof(OVRSpatialAnchor))]
public class Anchor : MonoBehaviour
{
	// Token: 0x0600148B RID: 5259 RVA: 0x000647C4 File Offset: 0x000629C4
	private void Awake()
	{
		this._anchorMenu.SetActive(false);
		this._renderers = base.GetComponentsInChildren<MeshRenderer>();
		this._canvas.worldCamera = Camera.main;
		this._selectedButton = this._buttonList[0];
		this._selectedButton.OnSelect(null);
		this._spatialAnchor = base.GetComponent<OVRSpatialAnchor>();
		this._icon = base.GetComponent<Transform>().FindChildRecursive("Sphere").gameObject;
	}

	// Token: 0x0600148C RID: 5260 RVA: 0x00064840 File Offset: 0x00062A40
	private static string ConvertUuidToString(Guid guid)
	{
		byte[] array = guid.ToByteArray();
		StringBuilder stringBuilder = new StringBuilder(array.Length * 2 + 4);
		for (int i = 0; i < array.Length; i++)
		{
			if (3 < i && i < 11 && i % 2 == 0)
			{
				stringBuilder.Append("-");
			}
			stringBuilder.AppendFormat("{0:x2}", array[i]);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x000648A4 File Offset: 0x00062AA4
	private IEnumerator Start()
	{
		while (this._spatialAnchor && !this._spatialAnchor.Created)
		{
			yield return null;
		}
		if (this._spatialAnchor)
		{
			this._anchorName.text = Anchor.ConvertUuidToString(this._spatialAnchor.Uuid);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		yield break;
	}

	// Token: 0x0600148E RID: 5262 RVA: 0x000648B3 File Offset: 0x00062AB3
	private void Update()
	{
		this.BillboardPanel(this._canvas.transform);
		this.BillboardPanel(this._pivot);
		this.HandleMenuNavigation();
		this.BillboardPanel(this._icon.transform);
	}

	// Token: 0x0600148F RID: 5263 RVA: 0x000648E9 File Offset: 0x00062AE9
	public void OnSaveLocalButtonPressed()
	{
		if (!this._spatialAnchor)
		{
			return;
		}
		this._spatialAnchor.Save(delegate(OVRSpatialAnchor anchor, bool success)
		{
			if (!success)
			{
				return;
			}
			this.ShowSaveIcon = true;
			this.SaveUuidToPlayerPrefs(anchor.Uuid);
		});
	}

	// Token: 0x06001490 RID: 5264 RVA: 0x00064910 File Offset: 0x00062B10
	private void SaveUuidToPlayerPrefs(Guid uuid)
	{
		if (!PlayerPrefs.HasKey("numUuids"))
		{
			PlayerPrefs.SetInt("numUuids", 0);
		}
		int @int = PlayerPrefs.GetInt("numUuids");
		PlayerPrefs.SetString("uuid" + @int.ToString(), uuid.ToString());
		PlayerPrefs.SetInt("numUuids", @int + 1);
	}

	// Token: 0x06001491 RID: 5265 RVA: 0x0003A34F File Offset: 0x0003854F
	public void OnHideButtonPressed()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06001492 RID: 5266 RVA: 0x00064971 File Offset: 0x00062B71
	public void OnEraseButtonPressed()
	{
		if (!this._spatialAnchor)
		{
			return;
		}
		this._spatialAnchor.Erase(delegate(OVRSpatialAnchor anchor, bool success)
		{
			if (success)
			{
				this._saveIcon.SetActive(false);
			}
		});
	}

	// Token: 0x1700024E RID: 590
	// (set) Token: 0x06001493 RID: 5267 RVA: 0x00064998 File Offset: 0x00062B98
	public bool ShowSaveIcon
	{
		set
		{
			this._saveIcon.SetActive(value);
		}
	}

	// Token: 0x06001494 RID: 5268 RVA: 0x000649A8 File Offset: 0x00062BA8
	public void OnHoverStart()
	{
		if (this._isHovered)
		{
			return;
		}
		this._isHovered = true;
		MeshRenderer[] renderers = this._renderers;
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].material.SetColor("_EmissionColor", Color.yellow);
		}
		this._labelImage.color = this._labelHighlightColor;
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x00064A04 File Offset: 0x00062C04
	public void OnHoverEnd()
	{
		if (!this._isHovered)
		{
			return;
		}
		this._isHovered = false;
		MeshRenderer[] renderers = this._renderers;
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].material.SetColor("_EmissionColor", Color.clear);
		}
		if (this._isSelected)
		{
			this._labelImage.color = this._labelSelectedColor;
			return;
		}
		this._labelImage.color = this._labelBaseColor;
	}

	// Token: 0x06001496 RID: 5270 RVA: 0x00064A78 File Offset: 0x00062C78
	public void OnSelect()
	{
		if (this._isSelected)
		{
			this._anchorMenu.SetActive(false);
			this._isSelected = false;
			this._selectedButton = null;
			if (this._isHovered)
			{
				this._labelImage.color = this._labelHighlightColor;
				return;
			}
			this._labelImage.color = this._labelBaseColor;
			return;
		}
		else
		{
			this._anchorMenu.SetActive(true);
			this._isSelected = true;
			this._menuIndex = -1;
			this.NavigateToIndexInMenu(true);
			if (this._isHovered)
			{
				this._labelImage.color = this._labelHighlightColor;
				return;
			}
			this._labelImage.color = this._labelSelectedColor;
			return;
		}
	}

	// Token: 0x06001497 RID: 5271 RVA: 0x00064B20 File Offset: 0x00062D20
	private void BillboardPanel(Transform panel)
	{
		panel.LookAt(new Vector3(panel.position.x * 2f - Camera.main.transform.position.x, panel.position.y * 2f - Camera.main.transform.position.y, panel.position.z * 2f - Camera.main.transform.position.z), Vector3.up);
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x00064BB0 File Offset: 0x00062DB0
	private void HandleMenuNavigation()
	{
		if (!this._isSelected)
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

	// Token: 0x06001499 RID: 5273 RVA: 0x00064C14 File Offset: 0x00062E14
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
		if (this._selectedButton)
		{
			this._selectedButton.OnDeselect(null);
		}
		this._selectedButton = this._buttonList[this._menuIndex];
		this._selectedButton.OnSelect(null);
	}

	// Token: 0x040016E8 RID: 5864
	public const string NumUuidsPlayerPref = "numUuids";

	// Token: 0x040016E9 RID: 5865
	[SerializeField]
	[FormerlySerializedAs("canvas_")]
	private Canvas _canvas;

	// Token: 0x040016EA RID: 5866
	[SerializeField]
	[FormerlySerializedAs("pivot_")]
	private Transform _pivot;

	// Token: 0x040016EB RID: 5867
	[SerializeField]
	[FormerlySerializedAs("anchorMenu_")]
	private GameObject _anchorMenu;

	// Token: 0x040016EC RID: 5868
	private bool _isSelected;

	// Token: 0x040016ED RID: 5869
	private bool _isHovered;

	// Token: 0x040016EE RID: 5870
	[SerializeField]
	[FormerlySerializedAs("anchorName_")]
	private TextMeshProUGUI _anchorName;

	// Token: 0x040016EF RID: 5871
	[SerializeField]
	[FormerlySerializedAs("saveIcon_")]
	private GameObject _saveIcon;

	// Token: 0x040016F0 RID: 5872
	[SerializeField]
	[FormerlySerializedAs("labelImage_")]
	private Image _labelImage;

	// Token: 0x040016F1 RID: 5873
	[SerializeField]
	[FormerlySerializedAs("labelBaseColor_")]
	private Color _labelBaseColor;

	// Token: 0x040016F2 RID: 5874
	[SerializeField]
	[FormerlySerializedAs("labelHighlightColor_")]
	private Color _labelHighlightColor;

	// Token: 0x040016F3 RID: 5875
	[SerializeField]
	[FormerlySerializedAs("labelSelectedColor_")]
	private Color _labelSelectedColor;

	// Token: 0x040016F4 RID: 5876
	[SerializeField]
	[FormerlySerializedAs("uiManager_")]
	private AnchorUIManager _uiManager;

	// Token: 0x040016F5 RID: 5877
	[SerializeField]
	[FormerlySerializedAs("renderers_")]
	private MeshRenderer[] _renderers;

	// Token: 0x040016F6 RID: 5878
	private int _menuIndex;

	// Token: 0x040016F7 RID: 5879
	[SerializeField]
	[FormerlySerializedAs("buttonList_")]
	private List<Button> _buttonList;

	// Token: 0x040016F8 RID: 5880
	private Button _selectedButton;

	// Token: 0x040016F9 RID: 5881
	private OVRSpatialAnchor _spatialAnchor;

	// Token: 0x040016FA RID: 5882
	private GameObject _icon;
}
