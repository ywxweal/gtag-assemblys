using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000789 RID: 1929
public class AgeSliderWithProgressBar : MonoBehaviour
{
	// Token: 0x170004D0 RID: 1232
	// (get) Token: 0x06003054 RID: 12372 RVA: 0x000EECF0 File Offset: 0x000ECEF0
	// (set) Token: 0x06003055 RID: 12373 RVA: 0x000EECF8 File Offset: 0x000ECEF8
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
	// (get) Token: 0x06003056 RID: 12374 RVA: 0x000EED01 File Offset: 0x000ECF01
	public bool AdjustAge
	{
		get
		{
			return this._adjustAge;
		}
	}

	// Token: 0x170004D2 RID: 1234
	// (get) Token: 0x06003057 RID: 12375 RVA: 0x000EED09 File Offset: 0x000ECF09
	// (set) Token: 0x06003058 RID: 12376 RVA: 0x000EED11 File Offset: 0x000ECF11
	public bool ControllerActive { get; set; }

	// Token: 0x170004D3 RID: 1235
	// (get) Token: 0x06003059 RID: 12377 RVA: 0x000EED1A File Offset: 0x000ECF1A
	// (set) Token: 0x0600305A RID: 12378 RVA: 0x000EED22 File Offset: 0x000ECF22
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
	// (get) Token: 0x0600305B RID: 12379 RVA: 0x000EED2B File Offset: 0x000ECF2B
	public int CurrentAge
	{
		get
		{
			return this._currentAge;
		}
	}

	// Token: 0x0600305C RID: 12380 RVA: 0x000EED33 File Offset: 0x000ECF33
	private void Awake()
	{
		this.controllerBehaviour = base.GetComponentInChildren<ControllerBehaviour>(true);
		if (this._messageText)
		{
			this._originalText = this._messageText.text;
		}
	}

	// Token: 0x0600305D RID: 12381 RVA: 0x000EED60 File Offset: 0x000ECF60
	public void SetOriginalText(string text)
	{
		this._originalText = text;
	}

	// Token: 0x0600305E RID: 12382 RVA: 0x000EED6C File Offset: 0x000ECF6C
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

	// Token: 0x0600305F RID: 12383 RVA: 0x000EEE03 File Offset: 0x000ED003
	private void OnDisable()
	{
		this.controllerBehaviour.OnAction -= this.PostUpdate;
	}

	// Token: 0x06003060 RID: 12384 RVA: 0x000EEE1C File Offset: 0x000ED01C
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

	// Token: 0x06003061 RID: 12385 RVA: 0x000EEF60 File Offset: 0x000ED160
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

	// Token: 0x06003062 RID: 12386 RVA: 0x000EF070 File Offset: 0x000ED270
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

	// Token: 0x04003681 RID: 13953
	private const int MIN_AGE = 13;

	// Token: 0x04003682 RID: 13954
	[SerializeField]
	private AgeSliderWithProgressBar.SliderHeldEvent m_OnHoldComplete = new AgeSliderWithProgressBar.SliderHeldEvent();

	// Token: 0x04003683 RID: 13955
	[SerializeField]
	private bool _adjustAge;

	// Token: 0x04003684 RID: 13956
	[SerializeField]
	private int _maxAge = 25;

	// Token: 0x04003685 RID: 13957
	[SerializeField]
	private TMP_Text _ageValueTxt;

	// Token: 0x04003686 RID: 13958
	[Tooltip("Optional game object that should hold the Progress Bar Fill. Disables Hold functionality if null.")]
	[SerializeField]
	private GameObject _progressBarContainer;

	// Token: 0x04003687 RID: 13959
	[SerializeField]
	private float holdTime = 2.5f;

	// Token: 0x04003688 RID: 13960
	[SerializeField]
	private Image progressBarFill;

	// Token: 0x04003689 RID: 13961
	[SerializeField]
	private TMP_Text _messageText;

	// Token: 0x0400368A RID: 13962
	[SerializeField]
	private float _stickVibrationStrength = 0.1f;

	// Token: 0x0400368B RID: 13963
	[SerializeField]
	private float _stickVibrationDuration = 0.05f;

	// Token: 0x0400368C RID: 13964
	[SerializeField]
	private KIDUIButton _confirmButton;

	// Token: 0x0400368E RID: 13966
	[SerializeField]
	private string _lockMessage;

	// Token: 0x0400368F RID: 13967
	private string _originalText;

	// Token: 0x04003690 RID: 13968
	private int _currentAge;

	// Token: 0x04003691 RID: 13969
	private float progress;

	// Token: 0x04003692 RID: 13970
	private ControllerBehaviour controllerBehaviour;

	// Token: 0x0200078A RID: 1930
	[Serializable]
	public class SliderHeldEvent : UnityEvent<int>
	{
	}
}
