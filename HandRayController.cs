using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020007FA RID: 2042
public class HandRayController : MonoBehaviour
{
	// Token: 0x17000515 RID: 1301
	// (get) Token: 0x06003211 RID: 12817 RVA: 0x000F7702 File Offset: 0x000F5902
	public static HandRayController Instance
	{
		get
		{
			if (HandRayController.instance == null)
			{
				HandRayController.instance = Object.FindObjectOfType<HandRayController>();
				if (HandRayController.instance == null)
				{
					Debug.LogErrorFormat("[KID::UI::HAND_RAY_CONTROLLER] Not found in scene", Array.Empty<object>());
				}
			}
			return HandRayController.instance;
		}
	}

	// Token: 0x06003212 RID: 12818 RVA: 0x000F773C File Offset: 0x000F593C
	private void Awake()
	{
		this._controllerBehaviour = base.GetComponentInChildren<ControllerBehaviour>();
		if (HandRayController.instance != null)
		{
			Debug.LogErrorFormat("[KID::UI::HAND_RAY_CONTROLLER] Duplicate instance of HandRayController", Array.Empty<object>());
			Object.DestroyImmediate(this);
			return;
		}
		HandRayController.instance = this;
	}

	// Token: 0x06003213 RID: 12819 RVA: 0x000F7774 File Offset: 0x000F5974
	private void Start()
	{
		this._leftHandRay.attachTransform = (this._leftHandRay.rayOriginTransform = KIDHandReference.LeftHand.transform);
		this._rightHandRay.attachTransform = (this._rightHandRay.rayOriginTransform = KIDHandReference.RightHand.transform);
		this.DisableHandRays();
		this._activationCounter = 0;
	}

	// Token: 0x06003214 RID: 12820 RVA: 0x000F77D4 File Offset: 0x000F59D4
	private void OnDisable()
	{
		this.DisableHandRays();
	}

	// Token: 0x06003215 RID: 12821 RVA: 0x000F77DC File Offset: 0x000F59DC
	public void EnableHandRays()
	{
		if (this._activationCounter == 0)
		{
			this._controllerBehaviour.OnAction += this.PostUpdate;
			this.ToggleHands();
		}
		this._activationCounter++;
	}

	// Token: 0x06003216 RID: 12822 RVA: 0x000F7811 File Offset: 0x000F5A11
	public void DisableHandRays()
	{
		this._activationCounter--;
		if (this._activationCounter == 0)
		{
			this._controllerBehaviour.OnAction -= this.PostUpdate;
			this.HideHands();
		}
	}

	// Token: 0x06003217 RID: 12823 RVA: 0x000F7846 File Offset: 0x000F5A46
	public void PulseActiveHandray(float vibrationStrength, float vibrationDuration)
	{
		if (this._activeHandRay == null)
		{
			return;
		}
		this._activeHandRay.SendHapticImpulse(vibrationStrength, vibrationDuration);
	}

	// Token: 0x06003218 RID: 12824 RVA: 0x000F7865 File Offset: 0x000F5A65
	private void PostUpdate()
	{
		if (!this._hasInitialised)
		{
			return;
		}
		if (this.ActiveHand == HandRayController.HandSide.Left)
		{
			if (this._controllerBehaviour.RightButtonDown)
			{
				this.ToggleHands();
			}
			return;
		}
		if (this._controllerBehaviour.LeftButtonDown)
		{
			this.ToggleHands();
		}
	}

	// Token: 0x06003219 RID: 12825 RVA: 0x000F78A0 File Offset: 0x000F5AA0
	private void ToggleRightHandRay(bool enabled)
	{
		Debug.LogFormat(string.Format("[KID::UI::HAND_RAY_CONTROLLER] RIGHT Hand is: {0}. Setting to: {1}", this._rightHandRay.gameObject.activeInHierarchy, enabled), Array.Empty<object>());
		this._rightHandRay.gameObject.SetActive(enabled);
		if (enabled)
		{
			this._activeHandRay = this._rightHandRay;
		}
	}

	// Token: 0x0600321A RID: 12826 RVA: 0x000F78FC File Offset: 0x000F5AFC
	private void ToggleLeftHandRay(bool enabled)
	{
		Debug.LogFormat(string.Format("[KID::UI::HAND_RAY_CONTROLLER] LEFT Hand is: {0}. Setting to: {1}", this._rightHandRay.gameObject.activeInHierarchy, enabled), Array.Empty<object>());
		this._leftHandRay.gameObject.SetActive(enabled);
		if (enabled)
		{
			this._activeHandRay = this._leftHandRay;
		}
	}

	// Token: 0x0600321B RID: 12827 RVA: 0x000F7958 File Offset: 0x000F5B58
	private void InitialiseHands()
	{
		Debug.Log("[KID::UI::HAND_RAY_CONTROLLER] Initialising Hands");
		this.ToggleRightHandRay(this.ActiveHand == HandRayController.HandSide.Right);
		this.ToggleLeftHandRay(this.ActiveHand == HandRayController.HandSide.Left);
		this._hasInitialised = true;
	}

	// Token: 0x0600321C RID: 12828 RVA: 0x000F798C File Offset: 0x000F5B8C
	private void ToggleHands()
	{
		if (!this._hasInitialised)
		{
			this.InitialiseHands();
			return;
		}
		HandRayController.HandSide handSide = ((this.ActiveHand == HandRayController.HandSide.Left) ? HandRayController.HandSide.Right : HandRayController.HandSide.Left);
		Debug.LogFormat(string.Concat(new string[]
		{
			"[KID::UI::HAND_RAY_CONTROLLER] Setting ActiveHand FROM: [",
			this.ActiveHand.ToString(),
			"] TO: [",
			handSide.ToString(),
			"]"
		}), Array.Empty<object>());
		this.ActiveHand = handSide;
		this.ToggleRightHandRay(handSide == HandRayController.HandSide.Right);
		this.ToggleLeftHandRay(handSide == HandRayController.HandSide.Left);
	}

	// Token: 0x0600321D RID: 12829 RVA: 0x000F7A21 File Offset: 0x000F5C21
	private void HideHands()
	{
		this.ToggleRightHandRay(false);
		this.ToggleLeftHandRay(false);
		this._hasInitialised = false;
		this._activeHandRay = null;
	}

	// Token: 0x040038C9 RID: 14537
	private static HandRayController instance;

	// Token: 0x040038CA RID: 14538
	[SerializeField]
	private XRRayInteractor _leftHandRay;

	// Token: 0x040038CB RID: 14539
	[SerializeField]
	private XRRayInteractor _rightHandRay;

	// Token: 0x040038CC RID: 14540
	private bool _hasInitialised;

	// Token: 0x040038CD RID: 14541
	private ControllerBehaviour _controllerBehaviour;

	// Token: 0x040038CE RID: 14542
	private HandRayController.HandSide ActiveHand = HandRayController.HandSide.Right;

	// Token: 0x040038CF RID: 14543
	private XRRayInteractor _activeHandRay;

	// Token: 0x040038D0 RID: 14544
	private int _activationCounter;

	// Token: 0x020007FB RID: 2043
	private enum HandSide
	{
		// Token: 0x040038D2 RID: 14546
		Left,
		// Token: 0x040038D3 RID: 14547
		Right
	}
}
