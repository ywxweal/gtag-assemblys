using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000806 RID: 2054
public class KIDUIToggle : Slider
{
	// Token: 0x1700051B RID: 1307
	// (get) Token: 0x0600326D RID: 12909 RVA: 0x000F8F11 File Offset: 0x000F7111
	// (set) Token: 0x0600326E RID: 12910 RVA: 0x000F8F19 File Offset: 0x000F7119
	public bool CurrentValue { get; private set; }

	// Token: 0x1700051C RID: 1308
	// (get) Token: 0x0600326F RID: 12911 RVA: 0x000F8F22 File Offset: 0x000F7122
	public bool IsOn
	{
		get
		{
			return this.CurrentValue;
		}
	}

	// Token: 0x06003270 RID: 12912 RVA: 0x000F8F2C File Offset: 0x000F712C
	protected override void Awake()
	{
		base.Awake();
		this.SetupToggleComponent();
		this.controllerBehaviour = base.GetComponentInChildren<ControllerBehaviour>();
		if (this.controllerBehaviour == null)
		{
			Debug.LogError("[KID::UI_BUTTON] Could not find [ControllerBehaviour] in children, trying to create a new one.");
			if (this._cbUXSettings == null)
			{
				Debug.LogError("[KID::UI_BUTTON] [_cbUXSettings] has not been set but trying to create a new [ControllerBehaviour] reference.");
				return;
			}
			this.controllerBehaviour = ControllerBehaviour.CreateNewControllerBehaviour(base.gameObject, this._cbUXSettings);
		}
	}

	// Token: 0x06003271 RID: 12913 RVA: 0x000F8F99 File Offset: 0x000F7199
	protected override void Start()
	{
		base.Start();
		base.interactable = false;
	}

	// Token: 0x06003272 RID: 12914 RVA: 0x000F8FA8 File Offset: 0x000F71A8
	protected override void OnEnable()
	{
		base.OnEnable();
		base.interactable = false;
		if (this.controllerBehaviour)
		{
			this.controllerBehaviour.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06003273 RID: 12915 RVA: 0x000F8FDB File Offset: 0x000F71DB
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this.Toggle();
	}

	// Token: 0x06003274 RID: 12916 RVA: 0x000F8FEA File Offset: 0x000F71EA
	public override void OnPointerEnter(PointerEventData pointerEventData)
	{
		this.SetHighlighted();
		this.inside = true;
	}

	// Token: 0x06003275 RID: 12917 RVA: 0x000F8FF9 File Offset: 0x000F71F9
	public override void OnPointerExit(PointerEventData pointerEventData)
	{
		this.SetNormal();
		this.inside = false;
	}

	// Token: 0x06003276 RID: 12918 RVA: 0x000F9008 File Offset: 0x000F7208
	protected virtual void SetupToggleComponent()
	{
		this.SetupSliderComponent();
		base.handleRect.anchorMin = new Vector2(0f, 0.5f);
		base.handleRect.anchorMax = new Vector3(0f, 0.5f);
		base.handleRect.pivot = new Vector2(0f, 0.5f);
		base.handleRect.sizeDelta = new Vector2(base.handleRect.sizeDelta.x, base.handleRect.sizeDelta.x);
	}

	// Token: 0x06003277 RID: 12919 RVA: 0x000F90A0 File Offset: 0x000F72A0
	protected virtual void SetupSliderComponent()
	{
		base.interactable = false;
		base.colors.disabledColor = Color.white;
		this.SetColors();
		base.transition = Selectable.Transition.None;
	}

	// Token: 0x06003278 RID: 12920 RVA: 0x000F90D4 File Offset: 0x000F72D4
	public void RegisterOnChangeEvent(Action onChange)
	{
		this._onToggleChanged.AddListener(delegate
		{
			Action onChange2 = onChange;
			if (onChange2 == null)
			{
				return;
			}
			onChange2();
		});
	}

	// Token: 0x06003279 RID: 12921 RVA: 0x000F9108 File Offset: 0x000F7308
	public void UnregisterOnChangeEvent(Action onChange)
	{
		this._onToggleChanged.RemoveListener(delegate
		{
			Action onChange2 = onChange;
			if (onChange2 == null)
			{
				return;
			}
			onChange2();
		});
	}

	// Token: 0x0600327A RID: 12922 RVA: 0x000F913C File Offset: 0x000F733C
	public void RegisterToggleOnEvent(Action onToggle)
	{
		this._onToggleOn.AddListener(delegate
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x0600327B RID: 12923 RVA: 0x000F9170 File Offset: 0x000F7370
	public void UnregisterToggleOnEvent(Action onToggle)
	{
		this._onToggleOn.RemoveListener(delegate
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x0600327C RID: 12924 RVA: 0x000F91A4 File Offset: 0x000F73A4
	public void RegisterToggleOffEvent(Action onToggle)
	{
		this._onToggleOff.AddListener(delegate
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x0600327D RID: 12925 RVA: 0x000F91D8 File Offset: 0x000F73D8
	public void UnregisterToggleOffEvent(Action onToggle)
	{
		this._onToggleOff.RemoveListener(delegate
		{
			Action onToggle2 = onToggle;
			if (onToggle2 == null)
			{
				return;
			}
			onToggle2();
		});
	}

	// Token: 0x0600327E RID: 12926 RVA: 0x000F9209 File Offset: 0x000F7409
	private void SetColors()
	{
		base.colors = this._fillColors;
	}

	// Token: 0x0600327F RID: 12927 RVA: 0x000F9217 File Offset: 0x000F7417
	private void Toggle()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetStateAndStartAnimation(!this.CurrentValue, false);
	}

	// Token: 0x06003280 RID: 12928 RVA: 0x000F9232 File Offset: 0x000F7432
	public void SetValue(bool newValue)
	{
		if (newValue == this.CurrentValue)
		{
			return;
		}
		this.SetStateAndStartAnimation(newValue, false);
	}

	// Token: 0x06003281 RID: 12929 RVA: 0x000F9248 File Offset: 0x000F7448
	private void SetStateAndStartAnimation(bool state, bool skipAnim = false)
	{
		if (this.CurrentValue == state)
		{
			Debug.Log("IS SAME STATE, WILL NOT CHANGE");
			return;
		}
		this.CurrentValue = state;
		UnityEvent onToggleChanged = this._onToggleChanged;
		if (onToggleChanged != null)
		{
			onToggleChanged.Invoke();
		}
		if (this.CurrentValue)
		{
			UnityEvent onToggleOn = this._onToggleOn;
			if (onToggleOn != null)
			{
				onToggleOn.Invoke();
			}
		}
		else
		{
			UnityEvent onToggleOff = this._onToggleOff;
			if (onToggleOff != null)
			{
				onToggleOff.Invoke();
			}
		}
		if (this._animationCoroutine != null)
		{
			base.StopCoroutine(this._animationCoroutine);
		}
		this._handleUnlockIcon.gameObject.SetActive(this.CurrentValue);
		this._handleLockIcon.gameObject.SetActive(!this.CurrentValue);
		if (this._animationDuration == 0f || skipAnim)
		{
			Debug.Log("[KID::UI::SetStateAndStartAnimation] Skipping animation. Setting value to " + (this.CurrentValue ? "1f" : "0f"));
			this.value = (this.CurrentValue ? 1f : 0f);
			return;
		}
		this._animationCoroutine = base.StartCoroutine(this.AnimateSlider());
	}

	// Token: 0x06003282 RID: 12930 RVA: 0x000F9351 File Offset: 0x000F7551
	private IEnumerator AnimateSlider()
	{
		Debug.Log(string.Format("[KID::UI::TOGGLE] Toggle: [{0}] is {1}", base.name, this.CurrentValue));
		float startValue = (this.CurrentValue ? 0f : 1f);
		float endValue = (this.CurrentValue ? 1f : 0f);
		Debug.Log(string.Format("[KID::UI::TOGGLE] Toggle: [{0}] Start: {1}, End: {2}, Value: {3}", new object[] { base.name, startValue, endValue, this.value }));
		float time = 0f;
		while (time < this._animationDuration)
		{
			time += Time.deltaTime;
			float num = this._toggleEase.Evaluate(time / this._animationDuration);
			this.value = Mathf.Lerp(startValue, endValue, num);
			yield return null;
		}
		this.value = endValue;
		yield break;
	}

	// Token: 0x06003283 RID: 12931 RVA: 0x000F9360 File Offset: 0x000F7560
	private void PostUpdate()
	{
		if (!this.inside)
		{
			return;
		}
		if (this.controllerBehaviour && this.controllerBehaviour.TriggerDown)
		{
			string text = string.Concat(new string[]
			{
				"[",
				base.transform.parent.parent.parent.name,
				".",
				base.transform.parent.parent.name,
				".",
				base.transform.parent.name,
				".",
				base.transform.name,
				"]"
			});
			Debug.Log(string.Concat(new string[]
			{
				"[KID::UIBUTTON::DEBUG] ",
				text,
				" - STEAM - OnClick is pressed. Time: [",
				Time.time.ToString(),
				"]"
			}), this);
			this.Toggle();
			KIDUIToggle._triggeredThisFrame = true;
			KIDUIToggle._canTrigger = false;
		}
	}

	// Token: 0x06003284 RID: 12932 RVA: 0x000F9470 File Offset: 0x000F7670
	private void LateUpdate()
	{
		if (KIDUIToggle._triggeredThisFrame)
		{
			string text = string.Concat(new string[]
			{
				"[",
				base.transform.parent.parent.parent.name,
				".",
				base.transform.parent.parent.name,
				".",
				base.transform.parent.name,
				".",
				base.transform.name,
				"]"
			});
			Debug.Log(string.Concat(new string[]
			{
				"[KID::UIBUTTON::DEBUG] ",
				text,
				" - STEAM - OnLateUpdate triggered and Triggered Frame Reset. Time: [",
				Time.time.ToString(),
				"]"
			}), this);
		}
		KIDUIToggle._triggeredThisFrame = false;
	}

	// Token: 0x06003285 RID: 12933 RVA: 0x000F9555 File Offset: 0x000F7755
	protected new void OnDisable()
	{
		if (this.controllerBehaviour)
		{
			this.controllerBehaviour.OnAction -= this.PostUpdate;
		}
		this.inside = false;
	}

	// Token: 0x06003286 RID: 12934 RVA: 0x000F9582 File Offset: 0x000F7782
	private void SetDisabled(bool isLockedButEnabled)
	{
		this.SetSwitchColors(this._borderColors.disabledColor, this._handleColors.disabledColor, this._fillColors.disabledColor);
		this.SetBorderSize(this._disabledBorderSize);
		this.SetBackgroundActive(false);
	}

	// Token: 0x06003287 RID: 12935 RVA: 0x000F95C0 File Offset: 0x000F77C0
	private void SetNormal()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.normalColor, this._handleColors.normalColor, this._fillColors.normalColor);
		this.SetBorderSize(this._normalBorderSize);
		this.SetBackgroundActive(false);
	}

	// Token: 0x06003288 RID: 12936 RVA: 0x000F9610 File Offset: 0x000F7810
	private void SetSelected()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.selectedColor, this._handleColors.selectedColor, this._fillColors.selectedColor);
		this.SetBorderSize(this._selectedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x06003289 RID: 12937 RVA: 0x000F9660 File Offset: 0x000F7860
	private void SetHighlighted()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.highlightedColor, this._handleColors.highlightedColor, this._fillColors.highlightedColor);
		this.SetBorderSize(this._highlightedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x0600328A RID: 12938 RVA: 0x000F96B0 File Offset: 0x000F78B0
	private void SetPressed()
	{
		if (this._isDisabled)
		{
			return;
		}
		this.SetSwitchColors(this._borderColors.pressedColor, this._handleColors.pressedColor, this._fillColors.pressedColor);
		this.SetBorderSize(this._pressedBorderSize);
		this.SetBackgroundActive(true);
	}

	// Token: 0x0600328B RID: 12939 RVA: 0x000F9700 File Offset: 0x000F7900
	private void SetSwitchColors(Color borderColor, Color handleColor, Color fillColor)
	{
		this._borderImg.color = borderColor;
		this._handleImg.color = handleColor;
	}

	// Token: 0x0600328C RID: 12940 RVA: 0x000F971A File Offset: 0x000F791A
	private void SetBorderSize(float borderScale)
	{
		this._borderImgRef.offsetMin = new Vector2(-borderScale, -borderScale * this._borderHeightRatio);
		this._borderImgRef.offsetMax = new Vector2(borderScale, borderScale * this._borderHeightRatio);
	}

	// Token: 0x0600328D RID: 12941 RVA: 0x000F9750 File Offset: 0x000F7950
	private void SetBackgroundActive(bool isActive)
	{
		this._fillImg.gameObject.SetActive(isActive);
		this._fillInactiveImg.gameObject.SetActive(!isActive);
		this.SetBackgroundLocksActive(isActive);
	}

	// Token: 0x0600328E RID: 12942 RVA: 0x000F9780 File Offset: 0x000F7980
	private void SetBackgroundLocksActive(bool isActive)
	{
		Color color = (isActive ? this._lockActiveColor : this._lockInactiveColor);
		this._lockIcon.color = color;
		this._unlockIcon.color = color;
	}

	// Token: 0x04003927 RID: 14631
	[Header("Toggle Setup")]
	[SerializeField]
	[Range(0f, 1f)]
	private float _initValue;

	// Token: 0x04003928 RID: 14632
	[SerializeField]
	private Image _borderImg;

	// Token: 0x04003929 RID: 14633
	[SerializeField]
	private float _borderHeightRatio = 2f;

	// Token: 0x0400392A RID: 14634
	[SerializeField]
	private Image _fillImg;

	// Token: 0x0400392B RID: 14635
	[SerializeField]
	private Image _fillInactiveImg;

	// Token: 0x0400392C RID: 14636
	[SerializeField]
	private Image _handleImg;

	// Token: 0x0400392D RID: 14637
	[SerializeField]
	private Image _lockIcon;

	// Token: 0x0400392E RID: 14638
	[SerializeField]
	private Image _unlockIcon;

	// Token: 0x0400392F RID: 14639
	[SerializeField]
	private Image _handleLockIcon;

	// Token: 0x04003930 RID: 14640
	[SerializeField]
	private Image _handleUnlockIcon;

	// Token: 0x04003931 RID: 14641
	[SerializeField]
	private Color _lockActiveColor;

	// Token: 0x04003932 RID: 14642
	[SerializeField]
	private Color _lockInactiveColor;

	// Token: 0x04003933 RID: 14643
	[SerializeField]
	private RectTransform _borderImgRef;

	// Token: 0x04003934 RID: 14644
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04003935 RID: 14645
	[Header("Animation")]
	[SerializeField]
	private float _animationDuration = 0.15f;

	// Token: 0x04003936 RID: 14646
	[SerializeField]
	private AnimationCurve _toggleEase = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04003937 RID: 14647
	[Header("Fill Colors")]
	[SerializeField]
	private ColorBlock _fillColors;

	// Token: 0x04003938 RID: 14648
	[Header("Border Colors")]
	[SerializeField]
	private ColorBlock _borderColors;

	// Token: 0x04003939 RID: 14649
	[Header("Borders")]
	[SerializeField]
	private float _normalBorderSize = 1f;

	// Token: 0x0400393A RID: 14650
	[SerializeField]
	private float _disabledBorderSize = 1f;

	// Token: 0x0400393B RID: 14651
	[SerializeField]
	private float _highlightedBorderSize = 1f;

	// Token: 0x0400393C RID: 14652
	[SerializeField]
	private float _pressedBorderSize = 1f;

	// Token: 0x0400393D RID: 14653
	[SerializeField]
	private float _selectedBorderSize = 1f;

	// Token: 0x0400393E RID: 14654
	[Header("Handle Colors")]
	[SerializeField]
	private ColorBlock _handleColors;

	// Token: 0x0400393F RID: 14655
	[Header("Events")]
	[SerializeField]
	private UnityEvent _onToggleOn;

	// Token: 0x04003940 RID: 14656
	[SerializeField]
	private UnityEvent _onToggleOff;

	// Token: 0x04003941 RID: 14657
	[SerializeField]
	private UnityEvent _onToggleChanged;

	// Token: 0x04003942 RID: 14658
	private bool _previousValue;

	// Token: 0x04003943 RID: 14659
	private bool _isDisabled;

	// Token: 0x04003944 RID: 14660
	private Coroutine _animationCoroutine;

	// Token: 0x04003946 RID: 14662
	private ControllerBehaviour controllerBehaviour;

	// Token: 0x04003947 RID: 14663
	private bool inside;

	// Token: 0x04003948 RID: 14664
	private static bool _triggeredThisFrame = false;

	// Token: 0x04003949 RID: 14665
	private static bool _canTrigger = true;
}
