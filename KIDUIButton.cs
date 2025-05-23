using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

// Token: 0x02000802 RID: 2050
public class KIDUIButton : Button, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x17000517 RID: 1303
	// (get) Token: 0x06003237 RID: 12855 RVA: 0x000F81DE File Offset: 0x000F63DE
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x06003238 RID: 12856 RVA: 0x000F81F0 File Offset: 0x000F63F0
	protected override void Awake()
	{
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

	// Token: 0x06003239 RID: 12857 RVA: 0x000F8251 File Offset: 0x000F6451
	protected override void OnEnable()
	{
		if (this.controllerBehaviour)
		{
			this.controllerBehaviour.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x0600323A RID: 12858 RVA: 0x000F8278 File Offset: 0x000F6478
	private void PostUpdate()
	{
		if (!KIDUIButton._canTrigger)
		{
			KIDUIButton._canTrigger = !this.controllerBehaviour.TriggerDown;
		}
		if (!base.interactable || !this.inside || !KIDUIButton._canTrigger)
		{
			return;
		}
		if (this.controllerBehaviour && this.controllerBehaviour.TriggerDown && !KIDUIButton._triggeredThisFrame)
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
			Button.ButtonClickedEvent onClick = base.onClick;
			if (onClick != null)
			{
				onClick.Invoke();
			}
			this.inside = false;
			KIDUIButton._triggeredThisFrame = true;
			KIDUIButton._canTrigger = false;
		}
	}

	// Token: 0x0600323B RID: 12859 RVA: 0x000F83D0 File Offset: 0x000F65D0
	private void LateUpdate()
	{
		if (KIDUIButton._triggeredThisFrame)
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
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x0600323C RID: 12860 RVA: 0x000F84B5 File Offset: 0x000F66B5
	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		this.inside = false;
	}

	// Token: 0x0600323D RID: 12861 RVA: 0x000F84C5 File Offset: 0x000F66C5
	public void ResetButton()
	{
		this.inside = false;
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x0600323E RID: 12862 RVA: 0x000F84D4 File Offset: 0x000F66D4
	protected override void OnDisable()
	{
		this.FixStuckPressedState();
		if (this.controllerBehaviour)
		{
			this.controllerBehaviour.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x0600323F RID: 12863 RVA: 0x000F8500 File Offset: 0x000F6700
	private void FixStuckPressedState()
	{
		this.InstantClearState();
		this._buttonText.color = (base.interactable ? this._normalTextColor : this._disabledTextColor);
		this.inside = false;
		KIDUIButton._triggeredThisFrame = false;
	}

	// Token: 0x06003240 RID: 12864 RVA: 0x000F8538 File Offset: 0x000F6738
	protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
	{
		base.DoStateTransition(state, instant);
		switch (state)
		{
		default:
			this._buttonText.color = this._normalTextColor;
			this.SetIcons(true, false);
			return;
		case Selectable.SelectionState.Highlighted:
			this._buttonText.color = this._highlightedTextColor;
			this.SetIcons(false, true);
			return;
		case Selectable.SelectionState.Pressed:
			this._buttonText.color = this._pressedTextColor;
			this.SetIcons(true, false);
			return;
		case Selectable.SelectionState.Selected:
			this._buttonText.color = this._selectedTextColor;
			this.SetIcons(true, false);
			return;
		case Selectable.SelectionState.Disabled:
			this._buttonText.color = this._disabledTextColor;
			this.SetIcons(true, false);
			return;
		}
	}

	// Token: 0x06003241 RID: 12865 RVA: 0x000F85E8 File Offset: 0x000F67E8
	private void SetIcons(bool normalEnabled, bool highlightedEnabled)
	{
		if (this._normalIcon == null || this._highlightedIcon == null)
		{
			return;
		}
		GameObject normalIcon = this._normalIcon;
		if (normalIcon != null)
		{
			normalIcon.SetActive(normalEnabled);
		}
		GameObject highlightedIcon = this._highlightedIcon;
		if (highlightedIcon == null)
		{
			return;
		}
		highlightedIcon.SetActive(highlightedEnabled);
	}

	// Token: 0x06003242 RID: 12866 RVA: 0x000F8638 File Offset: 0x000F6838
	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		this.inside = true;
		if (!this.IsInteractable() || !this.IsActive())
		{
			return;
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
		if (!xrrayInteractor)
		{
			return;
		}
		xrrayInteractor.xrController.SendHapticImpulse(this._highlightedVibrationStrength, this._highlightedVibrationDuration);
	}

	// Token: 0x06003243 RID: 12867 RVA: 0x000F869C File Offset: 0x000F689C
	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this.inside = false;
		if (!this.IsInteractable() || !this.IsActive())
		{
			return;
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
		if (!xrrayInteractor)
		{
			return;
		}
		xrrayInteractor.xrController.SendHapticImpulse(this._pressedVibrationStrength, this._pressedVibrationDuration);
	}

	// Token: 0x06003244 RID: 12868 RVA: 0x000F8700 File Offset: 0x000F6900
	public void SetText(string text)
	{
		this._buttonText.SetText(text, true);
	}

	// Token: 0x040038F0 RID: 14576
	[SerializeField]
	private Image _borderImage;

	// Token: 0x040038F1 RID: 14577
	[SerializeField]
	private RectTransform _fillImageRef;

	// Token: 0x040038F2 RID: 14578
	[SerializeField]
	private TMP_Text _buttonText;

	// Token: 0x040038F3 RID: 14579
	[Header("Transition States")]
	[Header("Normal")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _normalBorderColor;

	// Token: 0x040038F4 RID: 14580
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _normalTextColor;

	// Token: 0x040038F5 RID: 14581
	[SerializeField]
	private float _normalBorderSize;

	// Token: 0x040038F6 RID: 14582
	[Header("Highlighted")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _highlightedBorderColor;

	// Token: 0x040038F7 RID: 14583
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _highlightedTextColor;

	// Token: 0x040038F8 RID: 14584
	[SerializeField]
	private float _highlightedBorderSize;

	// Token: 0x040038F9 RID: 14585
	[SerializeField]
	private float _highlightedVibrationStrength = 0.1f;

	// Token: 0x040038FA RID: 14586
	[SerializeField]
	private float _highlightedVibrationDuration = 0.1f;

	// Token: 0x040038FB RID: 14587
	[Header("Pressed")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _pressedBorderColor;

	// Token: 0x040038FC RID: 14588
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _pressedTextColor;

	// Token: 0x040038FD RID: 14589
	[SerializeField]
	private float _pressedBorderSize;

	// Token: 0x040038FE RID: 14590
	[SerializeField]
	private float _pressedVibrationStrength = 0.5f;

	// Token: 0x040038FF RID: 14591
	[SerializeField]
	private float _pressedVibrationDuration = 0.1f;

	// Token: 0x04003900 RID: 14592
	[Header("Selected")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _selectedBorderColor;

	// Token: 0x04003901 RID: 14593
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _selectedTextColor;

	// Token: 0x04003902 RID: 14594
	[SerializeField]
	private float _selectedBorderSize;

	// Token: 0x04003903 RID: 14595
	[Header("Disabled")]
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _disabledBorderColor;

	// Token: 0x04003904 RID: 14596
	[SerializeField]
	[ColorUsage(true, false)]
	private Color _disabledTextColor;

	// Token: 0x04003905 RID: 14597
	[SerializeField]
	private float _disabledBorderSize;

	// Token: 0x04003906 RID: 14598
	[Header("Icon Swap Settings")]
	[SerializeField]
	private GameObject _normalIcon;

	// Token: 0x04003907 RID: 14599
	[SerializeField]
	private GameObject _highlightedIcon;

	// Token: 0x04003908 RID: 14600
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04003909 RID: 14601
	private ControllerBehaviour controllerBehaviour;

	// Token: 0x0400390A RID: 14602
	private bool inside;

	// Token: 0x0400390B RID: 14603
	private static bool _triggeredThisFrame = false;

	// Token: 0x0400390C RID: 14604
	private static bool _canTrigger = true;
}
