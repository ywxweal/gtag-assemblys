using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Valve.VR;

// Token: 0x02000829 RID: 2089
public class KIDUI_InputFieldController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x17000527 RID: 1319
	// (get) Token: 0x06003313 RID: 13075 RVA: 0x000F8106 File Offset: 0x000F6306
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x06003314 RID: 13076 RVA: 0x000FBC14 File Offset: 0x000F9E14
	protected void Awake()
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

	// Token: 0x06003315 RID: 13077 RVA: 0x000FBC78 File Offset: 0x000F9E78
	protected void OnEnable()
	{
		if (this.controllerBehaviour)
		{
			this.controllerBehaviour.OnAction += this.PostUpdate;
		}
		SteamVR_Utils.Event.Listen("KeyboardCharInput", new SteamVR_Utils.Event.Handler(this.OnKeyboard));
		SteamVR_Utils.Event.Listen("KeyboardClosed", new SteamVR_Utils.Event.Handler(this.OnKeyboardClosed));
	}

	// Token: 0x06003316 RID: 13078 RVA: 0x000FBCD5 File Offset: 0x000F9ED5
	protected void OnDisable()
	{
		if (this.controllerBehaviour)
		{
			this.controllerBehaviour.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x06003317 RID: 13079 RVA: 0x000FBCFC File Offset: 0x000F9EFC
	private void PostUpdate()
	{
		if (!this._inputField.interactable || !this.inside)
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
			this.OnClickedInputField();
		}
	}

	// Token: 0x06003318 RID: 13080 RVA: 0x000FBE10 File Offset: 0x000FA010
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.inside = true;
		if (!this._inputField.IsInteractable() || !this._inputField.IsActive())
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

	// Token: 0x06003319 RID: 13081 RVA: 0x000FBE77 File Offset: 0x000FA077
	public void OnPointerExit(PointerEventData eventData)
	{
		this.inside = false;
	}

	// Token: 0x0600331A RID: 13082 RVA: 0x000FBE80 File Offset: 0x000FA080
	private void OnClickedInputField()
	{
		Debug.Log("[KID::INPUTFIELD_CONTROLLER] Selecting and Activating Input Field");
		this._inputField.Select();
		this._inputField.ActivateInputField();
		SteamVR.instance.overlay.ShowKeyboard(0, 0, 0U, "Input", 256U, this._inputField.text, 0UL);
	}

	// Token: 0x0600331B RID: 13083 RVA: 0x000FBED8 File Offset: 0x000FA0D8
	private void OnKeyboard(object[] args)
	{
		StringBuilder stringBuilder = new StringBuilder(1024);
		SteamVR.instance.overlay.GetKeyboardText(stringBuilder, 1024U);
		this._inputField.text = stringBuilder.ToString();
	}

	// Token: 0x0600331C RID: 13084 RVA: 0x000FBF17 File Offset: 0x000FA117
	private void OnKeyboardClosed(object[] args)
	{
		this.keyboardShowing = false;
	}

	// Token: 0x040039E7 RID: 14823
	[Header("Haptics")]
	[SerializeField]
	private float _highlightedVibrationStrength = 0.1f;

	// Token: 0x040039E8 RID: 14824
	[SerializeField]
	private float _highlightedVibrationDuration = 0.1f;

	// Token: 0x040039E9 RID: 14825
	[Header("Steam Settings")]
	[SerializeField]
	private TMP_InputField _inputField;

	// Token: 0x040039EA RID: 14826
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x040039EB RID: 14827
	private ControllerBehaviour controllerBehaviour;

	// Token: 0x040039EC RID: 14828
	private bool inside;

	// Token: 0x040039ED RID: 14829
	private bool keyboardShowing;
}
