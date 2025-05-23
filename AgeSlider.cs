using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000787 RID: 1927
public class AgeSlider : MonoBehaviour, IBuildValidation
{
	// Token: 0x170004CF RID: 1231
	// (get) Token: 0x0600304A RID: 12362 RVA: 0x000EEAF1 File Offset: 0x000ECCF1
	// (set) Token: 0x0600304B RID: 12363 RVA: 0x000EEAF9 File Offset: 0x000ECCF9
	public AgeSlider.SliderHeldEvent onHoldComplete
	{
		get
		{
			return this.m_OnHoldComplete;
		}
		set
		{
			this.m_OnHoldComplete = value;
		}
	}

	// Token: 0x0600304C RID: 12364 RVA: 0x000EEB02 File Offset: 0x000ECD02
	private void Awake()
	{
		this.controllerBehaviour = base.GetComponentInChildren<ControllerBehaviour>(true);
	}

	// Token: 0x0600304D RID: 12365 RVA: 0x000EEB11 File Offset: 0x000ECD11
	private void OnEnable()
	{
		this.controllerBehaviour.OnAction += this.PostUpdate;
	}

	// Token: 0x0600304E RID: 12366 RVA: 0x000EEB2A File Offset: 0x000ECD2A
	private void OnDisable()
	{
		this.controllerBehaviour.OnAction -= this.PostUpdate;
	}

	// Token: 0x0600304F RID: 12367 RVA: 0x000EEB44 File Offset: 0x000ECD44
	protected void Update()
	{
		if (!AgeSlider._ageGateActive)
		{
			return;
		}
		if (this.controllerBehaviour.ButtonDown && this._confirmButton.activeInHierarchy)
		{
			this.progress += Time.deltaTime / this.holdTime;
			this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
			this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
			if (this.progress >= 1f)
			{
				this.m_OnHoldComplete.Invoke(this._currentAge);
				return;
			}
		}
		else
		{
			this.progress = 0f;
			this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
			this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
		}
	}

	// Token: 0x06003050 RID: 12368 RVA: 0x000EEC50 File Offset: 0x000ECE50
	private void PostUpdate()
	{
		if (!AgeSlider._ageGateActive)
		{
			return;
		}
		if (this.controllerBehaviour.IsLeftStick || this.controllerBehaviour.IsUpStick)
		{
			this._currentAge = Mathf.Clamp(this._currentAge - 1, 0, this._maxAge);
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
			this._confirmButton.SetActive(this._currentAge > 0);
		}
		if (this.controllerBehaviour.IsRightStick || this.controllerBehaviour.IsDownStick)
		{
			this._currentAge = Mathf.Clamp(this._currentAge + 1, 0, this._maxAge);
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
			this._confirmButton.SetActive(this._currentAge > 0);
		}
	}

	// Token: 0x06003051 RID: 12369 RVA: 0x000EED41 File Offset: 0x000ECF41
	public static void ToggleAgeGate(bool state)
	{
		AgeSlider._ageGateActive = state;
	}

	// Token: 0x06003052 RID: 12370 RVA: 0x000EED49 File Offset: 0x000ECF49
	public bool BuildValidationCheck()
	{
		if (this._confirmButton == null)
		{
			Debug.LogError("[KID] Object [_confirmButton] is NULL. Must be assigned in editor");
			return false;
		}
		return true;
	}

	// Token: 0x04003678 RID: 13944
	private const int MIN_AGE = 13;

	// Token: 0x04003679 RID: 13945
	[SerializeField]
	private AgeSlider.SliderHeldEvent m_OnHoldComplete = new AgeSlider.SliderHeldEvent();

	// Token: 0x0400367A RID: 13946
	[SerializeField]
	private int _maxAge = 99;

	// Token: 0x0400367B RID: 13947
	[SerializeField]
	private TMP_Text _ageValueTxt;

	// Token: 0x0400367C RID: 13948
	[SerializeField]
	private GameObject _confirmButton;

	// Token: 0x0400367D RID: 13949
	[SerializeField]
	private float holdTime = 5f;

	// Token: 0x0400367E RID: 13950
	[SerializeField]
	private LineRenderer progressBar;

	// Token: 0x0400367F RID: 13951
	private int _currentAge;

	// Token: 0x04003680 RID: 13952
	private static bool _ageGateActive;

	// Token: 0x04003681 RID: 13953
	private float progress;

	// Token: 0x04003682 RID: 13954
	private ControllerBehaviour controllerBehaviour;

	// Token: 0x02000788 RID: 1928
	[Serializable]
	public class SliderHeldEvent : UnityEvent<int>
	{
	}
}
