using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000804 RID: 2052
public class KIDUIHoldableButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
	// Token: 0x17000519 RID: 1305
	// (get) Token: 0x06003259 RID: 12889 RVA: 0x000F89F3 File Offset: 0x000F6BF3
	// (set) Token: 0x0600325A RID: 12890 RVA: 0x000F89FB File Offset: 0x000F6BFB
	public KIDUIHoldableButton.ButtonHoldCompleteEvent onHoldComplete
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

	// Token: 0x1700051A RID: 1306
	// (get) Token: 0x0600325B RID: 12891 RVA: 0x000F8A04 File Offset: 0x000F6C04
	public float HoldPercentage
	{
		get
		{
			return this._elapsedTime / this._holdDuration;
		}
	}

	// Token: 0x0600325C RID: 12892 RVA: 0x000F8A14 File Offset: 0x000F6C14
	private void OnEnable()
	{
		this._holdProgressFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		if (this.controllerBehaviour)
		{
			this.controllerBehaviour.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x0600325D RID: 12893 RVA: 0x000F8A69 File Offset: 0x000F6C69
	private void Update()
	{
		this.ManageButtonInteraction(false);
	}

	// Token: 0x0600325E RID: 12894 RVA: 0x000F8A72 File Offset: 0x000F6C72
	public void OnPointerDown(PointerEventData eventData)
	{
		this.ToggleHoldingButton(true);
	}

	// Token: 0x0600325F RID: 12895 RVA: 0x000F8A7B File Offset: 0x000F6C7B
	public void OnPointerUp(PointerEventData eventData)
	{
		this.ManageButtonInteraction(true);
		this.ToggleHoldingButton(false);
	}

	// Token: 0x06003260 RID: 12896 RVA: 0x000F8A8C File Offset: 0x000F6C8C
	private void ToggleHoldingButton(bool isPointerDown)
	{
		this._isHoldingButton = isPointerDown && this._button.interactable;
		this._holdProgressFill.rectTransform.localScale = new Vector3(0f, 1f, 1f);
		if (isPointerDown)
		{
			this._elapsedTime = 0f;
		}
	}

	// Token: 0x06003261 RID: 12897 RVA: 0x000F8AE4 File Offset: 0x000F6CE4
	private void ManageButtonInteraction(bool isPointerUp = false)
	{
		if (!this._isHoldingButton)
		{
			return;
		}
		if (isPointerUp)
		{
			return;
		}
		if (this._holdDuration <= 0f)
		{
			this.HoldComplete();
			return;
		}
		this._elapsedTime += Time.deltaTime;
		bool flag = this._elapsedTime > this._holdDuration;
		float num = this._elapsedTime / this._holdDuration;
		this._holdProgressFill.rectTransform.localScale = new Vector3(num, 1f, 1f);
		HandRayController.Instance.PulseActiveHandray(num, 0.1f);
		if (flag)
		{
			this.HoldComplete();
		}
	}

	// Token: 0x06003262 RID: 12898 RVA: 0x000F8B78 File Offset: 0x000F6D78
	private void HoldComplete()
	{
		this.ToggleHoldingButton(false);
		KIDUIHoldableButton.ButtonHoldCompleteEvent onHoldComplete = this.m_OnHoldComplete;
		if (onHoldComplete != null)
		{
			onHoldComplete.Invoke();
		}
		Debug.Log("[HOLD_BUTTON " + base.name + " ]: Hold Complete");
		this.ResetButton();
	}

	// Token: 0x06003263 RID: 12899 RVA: 0x000F8BB2 File Offset: 0x000F6DB2
	private void ResetButton()
	{
		this._elapsedTime = 0f;
		this.inside = false;
		KIDUIHoldableButton._triggeredThisFrame = false;
		this._button.ResetButton();
	}

	// Token: 0x06003264 RID: 12900 RVA: 0x000F8BD8 File Offset: 0x000F6DD8
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
		if (this._button != null)
		{
			return;
		}
		this._button = base.GetComponentInChildren<KIDUIButton>();
		if (this._button == null)
		{
			Debug.LogError("[KID::UI_BUTTON] Could not find [KIDUIButton] in children, trying to create a new one.");
			return;
		}
	}

	// Token: 0x06003265 RID: 12901 RVA: 0x000F8C70 File Offset: 0x000F6E70
	private void PostUpdate()
	{
		if (!KIDUIHoldableButton._canTrigger)
		{
			KIDUIHoldableButton._canTrigger = !this.controllerBehaviour.TriggerDown;
		}
		if (!this._button.interactable || !this.inside || !KIDUIHoldableButton._canTrigger)
		{
			return;
		}
		if (this.controllerBehaviour)
		{
			if (this.controllerBehaviour.TriggerDown)
			{
				if (!this._isHoldingButton)
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
					this.ToggleHoldingButton(true);
					KIDUIHoldableButton._triggeredThisFrame = true;
					KIDUIHoldableButton._canTrigger = false;
					return;
				}
			}
			else if (this._isHoldingButton)
			{
				this.ToggleHoldingButton(false);
			}
		}
	}

	// Token: 0x06003266 RID: 12902 RVA: 0x000F8DCC File Offset: 0x000F6FCC
	private void LateUpdate()
	{
		if (KIDUIHoldableButton._triggeredThisFrame)
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
		KIDUIHoldableButton._triggeredThisFrame = false;
	}

	// Token: 0x06003267 RID: 12903 RVA: 0x000F8EB1 File Offset: 0x000F70B1
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.inside = true;
	}

	// Token: 0x06003268 RID: 12904 RVA: 0x000F8EBA File Offset: 0x000F70BA
	public void OnPointerExit(PointerEventData eventData)
	{
		this.inside = false;
	}

	// Token: 0x06003269 RID: 12905 RVA: 0x000F8EC3 File Offset: 0x000F70C3
	protected void OnDisable()
	{
		if (this.controllerBehaviour)
		{
			this.controllerBehaviour.OnAction -= this.PostUpdate;
		}
		this.inside = false;
	}

	// Token: 0x0400391C RID: 14620
	public KIDUIButton _button;

	// Token: 0x0400391D RID: 14621
	[SerializeField]
	private float _holdDuration;

	// Token: 0x0400391E RID: 14622
	[SerializeField]
	private Image _holdProgressFill;

	// Token: 0x0400391F RID: 14623
	[Header("Steam Settings")]
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04003920 RID: 14624
	[SerializeField]
	private KIDUIHoldableButton.ButtonHoldCompleteEvent m_OnHoldComplete = new KIDUIHoldableButton.ButtonHoldCompleteEvent();

	// Token: 0x04003921 RID: 14625
	private bool _isHoldingButton;

	// Token: 0x04003922 RID: 14626
	private float _elapsedTime;

	// Token: 0x04003923 RID: 14627
	private ControllerBehaviour controllerBehaviour;

	// Token: 0x04003924 RID: 14628
	private bool inside;

	// Token: 0x04003925 RID: 14629
	private static bool _triggeredThisFrame = false;

	// Token: 0x04003926 RID: 14630
	private static bool _canTrigger = true;

	// Token: 0x02000805 RID: 2053
	[Serializable]
	public class ButtonHoldCompleteEvent : UnityEvent
	{
	}
}
