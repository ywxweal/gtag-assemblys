using System;
using KID.Model;
using TMPro;
using UnityEngine;

// Token: 0x02000803 RID: 2051
public class KIDUIFeatureSetting : MonoBehaviour
{
	// Token: 0x17000518 RID: 1304
	// (get) Token: 0x06003247 RID: 12871 RVA: 0x000F8751 File Offset: 0x000F6951
	// (set) Token: 0x06003248 RID: 12872 RVA: 0x000F8759 File Offset: 0x000F6959
	public bool AlwaysCheckFeatureSetting { get; private set; }

	// Token: 0x06003249 RID: 12873 RVA: 0x000F8762 File Offset: 0x000F6962
	public void CreateNewFeatureSettingGuardianManaged(KIDUI_MainScreen.FeatureToggleSetup feature, bool isEnabled)
	{
		this.CreateNewFeatureSettingWithoutToggle(feature, false);
		this._guardianManagedEnabled.SetActive(isEnabled);
		this._guardianManagedLocked.SetActive(!isEnabled);
	}

	// Token: 0x0600324A RID: 12874 RVA: 0x000F8787 File Offset: 0x000F6987
	public KIDUIToggle CreateNewFeatureSettingWithToggle(KIDUI_MainScreen.FeatureToggleSetup feature, bool initialState = false, bool alwaysCheckFeatureSetting = false)
	{
		this.SetFeatureData(feature, alwaysCheckFeatureSetting, true);
		this._featureToggle.SetValue(initialState);
		KIDUIToggle featureToggle = this._featureToggle;
		if (featureToggle != null)
		{
			featureToggle.RegisterOnChangeEvent(new Action(this.SetFeatureName));
		}
		return this._featureToggle;
	}

	// Token: 0x0600324B RID: 12875 RVA: 0x000F87C1 File Offset: 0x000F69C1
	public void CreateNewFeatureSettingWithoutToggle(KIDUI_MainScreen.FeatureToggleSetup feature, bool alwaysCheckFeatureSetting = false)
	{
		this.SetFeatureData(feature, alwaysCheckFeatureSetting, false);
	}

	// Token: 0x0600324C RID: 12876 RVA: 0x000F87CC File Offset: 0x000F69CC
	private void SetFeatureData(KIDUI_MainScreen.FeatureToggleSetup feature, bool alwaysCheckFeatureSetting, bool featureToggleEnabled)
	{
		this._enabledTextStr = feature.enabledText;
		this._disabledTextStr = feature.disabledText;
		this._hasToggle = featureToggleEnabled;
		this._featureType = feature.linkedFeature;
		this._featureName = feature.featureName;
		this.SetFeatureName();
		GameObject gameObject = base.gameObject;
		gameObject.name = gameObject.name + "_" + feature.featureName;
		this._permissionName = feature.permissionName;
		this._featureToggle.gameObject.SetActive(featureToggleEnabled);
		this.AlwaysCheckFeatureSetting = alwaysCheckFeatureSetting;
	}

	// Token: 0x0600324D RID: 12877 RVA: 0x000F885B File Offset: 0x000F6A5B
	public void UnregisterOnToggleChangeEvent(Action action)
	{
		this._featureToggle.UnregisterOnChangeEvent(action);
	}

	// Token: 0x0600324E RID: 12878 RVA: 0x000F8869 File Offset: 0x000F6A69
	public void RegisterToggleOnEvent(Action action)
	{
		this._featureToggle.RegisterToggleOnEvent(action);
	}

	// Token: 0x0600324F RID: 12879 RVA: 0x000F8877 File Offset: 0x000F6A77
	public void UnregisterToggleOnEvent(Action action)
	{
		this._featureToggle.UnregisterToggleOnEvent(action);
	}

	// Token: 0x06003250 RID: 12880 RVA: 0x000F8885 File Offset: 0x000F6A85
	public void RegisterToggleOffEvent(Action action)
	{
		this._featureToggle.RegisterToggleOffEvent(action);
	}

	// Token: 0x06003251 RID: 12881 RVA: 0x000F8893 File Offset: 0x000F6A93
	public void UnregisterToggleOffEvent(Action action)
	{
		this._featureToggle.UnregisterToggleOffEvent(action);
	}

	// Token: 0x06003252 RID: 12882 RVA: 0x000F88A1 File Offset: 0x000F6AA1
	public bool GetFeatureToggleState()
	{
		if (this._hasToggle)
		{
			return this._featureToggle.IsOn;
		}
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(this._featureType);
		if (permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.GUARDIAN)
		{
			Debug.LogError("[KID::FeatureSetting] GetToggleState: feature has no toggle AND is not managed by Guardian");
		}
		return permissionDataByFeature.Enabled;
	}

	// Token: 0x06003253 RID: 12883 RVA: 0x000F88DA File Offset: 0x000F6ADA
	public void SetFeatureSettingVisible(bool visible)
	{
		base.gameObject.SetActive(visible);
	}

	// Token: 0x06003254 RID: 12884 RVA: 0x000F88E8 File Offset: 0x000F6AE8
	public void SetFeatureToggle(bool enableToggle)
	{
		this._featureToggle.interactable = enableToggle;
	}

	// Token: 0x06003255 RID: 12885 RVA: 0x000F88F6 File Offset: 0x000F6AF6
	public void SetGuardianManagedState(bool isEnabled)
	{
		this._featureToggle.gameObject.SetActive(false);
		this._guardianManagedEnabled.SetActive(isEnabled);
		this._guardianManagedLocked.SetActive(!isEnabled);
		this.SetFeatureName();
	}

	// Token: 0x06003256 RID: 12886 RVA: 0x000F892C File Offset: 0x000F6B2C
	public void SetPlayerManagedState(bool isInteractable, bool isOptedIn)
	{
		this._featureToggle.gameObject.SetActive(true);
		this._guardianManagedEnabled.SetActive(false);
		this._guardianManagedLocked.SetActive(false);
		this._featureToggle.interactable = isInteractable;
		this._featureToggle.SetValue(isOptedIn);
	}

	// Token: 0x06003257 RID: 12887 RVA: 0x000F897C File Offset: 0x000F6B7C
	private void SetFeatureName()
	{
		string text = (this.GetFeatureToggleState() ? ("<b>(" + this._enabledTextStr + ")</b>") : ("<b>(" + this._disabledTextStr + ")</b>"));
		this._featureNameTxt.text = "<b>" + this._featureName + "</b>";
		this._featureStatusTxt.text = text ?? "";
	}

	// Token: 0x0400390D RID: 14605
	[SerializeField]
	private TMP_Text _featureNameTxt;

	// Token: 0x0400390E RID: 14606
	[SerializeField]
	private TMP_Text _featureStatusTxt;

	// Token: 0x0400390F RID: 14607
	[SerializeField]
	private KIDUIToggle _featureToggle;

	// Token: 0x04003910 RID: 14608
	[SerializeField]
	private GameObject _tickIcon;

	// Token: 0x04003911 RID: 14609
	[SerializeField]
	private GameObject _crossIcon;

	// Token: 0x04003912 RID: 14610
	[SerializeField]
	private GameObject _guardianManagedLocked;

	// Token: 0x04003913 RID: 14611
	[SerializeField]
	private GameObject _guardianManagedEnabled;

	// Token: 0x04003914 RID: 14612
	private bool _hasToggle;

	// Token: 0x04003915 RID: 14613
	private string _featureName;

	// Token: 0x04003916 RID: 14614
	private string _permissionName;

	// Token: 0x04003917 RID: 14615
	private string _enabledTextStr;

	// Token: 0x04003918 RID: 14616
	private string _disabledTextStr;

	// Token: 0x04003919 RID: 14617
	private EKIDFeatures _featureType;

	// Token: 0x0400391A RID: 14618
	private Action<EKIDFeatures> _onChangeCallback;
}
