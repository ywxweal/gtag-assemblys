using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000789 RID: 1929
public class AgeSliderWithProgressBar : MonoBehaviour
{
	// Token: 0x170004D0 RID: 1232
	// (get) Token: 0x06003055 RID: 12373 RVA: 0x000EED94 File Offset: 0x000ECF94
	// (set) Token: 0x06003056 RID: 12374 RVA: 0x000EED9C File Offset: 0x000ECF9C
	public AgeSliderWithProgressBar.SliderHeldEvent onHoldComplete
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

	// Token: 0x170004D1 RID: 1233
	// (get) Token: 0x06003057 RID: 12375 RVA: 0x000EEDA5 File Offset: 0x000ECFA5
	public bool AdjustAge
	{
		get
		{
			return this._adjustAge;
		}
	}

	// Token: 0x170004D2 RID: 1234
	// (get) Token: 0x06003058 RID: 12376 RVA: 0x000EEDAD File Offset: 0x000ECFAD
	// (set) Token: 0x06003059 RID: 12377 RVA: 0x000EEDB5 File Offset: 0x000ECFB5
	public bool ControllerActive { get; set; }

	// Token: 0x170004D3 RID: 1235
	// (get) Token: 0x0600305A RID: 12378 RVA: 0x000EEDBE File Offset: 0x000ECFBE
	// (set) Token: 0x0600305B RID: 12379 RVA: 0x000EEDC6 File Offset: 0x000ECFC6
	public string LockMessage
	{
		get
		{
			return this._lockMessage;
		}
		set
		{
			this._lockMessage = value;
		}
	}

	// Token: 0x170004D4 RID: 1236
	// (get) Token: 0x0600305C RID: 12380 RVA: 0x000EEDCF File Offset: 0x000ECFCF
	public int CurrentAge
	{
		get
		{
			return this._currentAge;
		}
	}

	// Token: 0x0600305D RID: 12381 RVA: 0x000EEDD7 File Offset: 0x000ECFD7
	private void Awake()
	{
		this.controllerBehaviour = base.GetComponentInChildren<ControllerBehaviour>(true);
		if (this._messageText)
		{
			this._originalText = this._messageText.text;
		}
	}

	// Token: 0x0600305E RID: 12382 RVA: 0x000EEE04 File Offset: 0x000ED004
	public void SetOriginalText(string text)
	{
		this._originalText = text;
	}

	// Token: 0x0600305F RID: 12383 RVA: 0x000EEE10 File Offset: 0x000ED010
	private void OnEnable()
	{
		this.controllerBehaviour.OnAction += this.PostUpdate;
		if (this._progressBarContainer != null && this.progressBarFill != null)
		{
			this.progressBarFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		}
		if (this._ageValueTxt)
		{
			this._ageValueTxt.text = ((this._currentAge > 0) ? this._currentAge.ToString() : "?");
		}
	}

	// Token: 0x06003060 RID: 12384 RVA: 0x000EEEA7 File Offset: 0x000ED0A7
	private void OnDisable()
	{
		this.controllerBehaviour.OnAction -= this.PostUpdate;
	}

	// Token: 0x06003061 RID: 12385 RVA: 0x000EEEC0 File Offset: 0x000ED0C0
	protected void Update()
	{
		if (!this._progressBarContainer)
		{
			return;
		}
		if (!this.ControllerActive)
		{
			return;
		}
		if (!this._lockMessage.IsNullOrEmpty())
		{
			this.progress = 0f;
			if (this._messageText)
			{
				this._messageText.text = this.LockMessage;
			}
		}
		else
		{
			if (this._messageText)
			{
				this._messageText.text = this._originalText;
			}
			if ((double)this.progress == 1.0)
			{
				this.m_OnHoldComplete.Invoke(this._currentAge);
				this.progress = 0f;
			}
			if (this.controllerBehaviour.ButtonDown && this._progressBarContainer != null && (this._currentAge > 0 || !this.AdjustAge))
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progress = Mathf.Clamp01(this.progress);
			}
			else
			{
				this.progress = 0f;
			}
		}
		if (this._progressBarContainer != null)
		{
			this.progressBarFill.rectTransform.localScale = new Vector3(this.progress, 1f, 1f);
		}
	}

	// Token: 0x06003062 RID: 12386 RVA: 0x000EF004 File Offset: 0x000ED204
	private void PostUpdate()
	{
		if (this.ControllerActive && this._ageValueTxt)
		{
			if (this.controllerBehaviour.IsLeftStick)
			{
				this._currentAge = Mathf.Clamp(this._currentAge - 1, 0, this._maxAge);
				if (this._currentAge > 0 && this._currentAge < this._maxAge)
				{
					HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
				}
			}
			if (this.controllerBehaviour.IsRightStick)
			{
				this._currentAge = Mathf.Clamp(this._currentAge + 1, 0, this._maxAge);
				if (this._currentAge > 0 && this._currentAge < this._maxAge)
				{
					HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
				}
			}
		}
		if (this._ageValueTxt)
		{
			this._ageValueTxt.text = this.GetAgeString();
			if (this._progressBarContainer != null)
			{
				this._progressBarContainer.SetActive(this._currentAge > 0);
			}
		}
	}

	// Token: 0x06003063 RID: 12387 RVA: 0x000EF114 File Offset: 0x000ED314
	public string GetAgeString()
	{
		if (this._confirmButton)
		{
			this._confirmButton.interactable = true;
		}
		if (this._currentAge == 0)
		{
			if (this._confirmButton)
			{
				this._confirmButton.interactable = false;
			}
			return "?";
		}
		if (this._currentAge == this._maxAge)
		{
			return this._maxAge.ToString() + "+";
		}
		return this._currentAge.ToString();
	}

	// Token: 0x04003683 RID: 13955
	private const int MIN_AGE = 13;

	// Token: 0x04003684 RID: 13956
	[SerializeField]
	private AgeSliderWithProgressBar.SliderHeldEvent m_OnHoldComplete = new AgeSliderWithProgressBar.SliderHeldEvent();

	// Token: 0x04003685 RID: 13957
	[SerializeField]
	private bool _adjustAge;

	// Token: 0x04003686 RID: 13958
	[SerializeField]
	private int _maxAge = 25;

	// Token: 0x04003687 RID: 13959
	[SerializeField]
	private TMP_Text _ageValueTxt;

	// Token: 0x04003688 RID: 13960
	[Tooltip("Optional game object that should hold the Progress Bar Fill. Disables Hold functionality if null.")]
	[SerializeField]
	private GameObject _progressBarContainer;

	// Token: 0x04003689 RID: 13961
	[SerializeField]
	private float holdTime = 2.5f;

	// Token: 0x0400368A RID: 13962
	[SerializeField]
	private Image progressBarFill;

	// Token: 0x0400368B RID: 13963
	[SerializeField]
	private TMP_Text _messageText;

	// Token: 0x0400368C RID: 13964
	[SerializeField]
	private float _stickVibrationStrength = 0.1f;

	// Token: 0x0400368D RID: 13965
	[SerializeField]
	private float _stickVibrationDuration = 0.05f;

	// Token: 0x0400368E RID: 13966
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x04003690 RID: 13968
	[SerializeField]
	private string _lockMessage;

	// Token: 0x04003691 RID: 13969
	private string _originalText;

	// Token: 0x04003692 RID: 13970
	private int _currentAge;

	// Token: 0x04003693 RID: 13971
	private float progress;

	// Token: 0x04003694 RID: 13972
	private ControllerBehaviour controllerBehaviour;

	// Token: 0x0200078A RID: 1930
	[Serializable]
	public class SliderHeldEvent : UnityEvent<int>
	{
	}
}
