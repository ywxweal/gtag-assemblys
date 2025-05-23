using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020007FA RID: 2042
public class HandRayController : MonoBehaviour
{
	// Token: 0x17000515 RID: 1301
	// (get) Token: 0x06003212 RID: 12818 RVA: 0x000F77DA File Offset: 0x000F59DA
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

	// Token: 0x06003213 RID: 12819 RVA: 0x000F7814 File Offset: 0x000F5A14
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

	// Token: 0x06003214 RID: 12820 RVA: 0x000F784C File Offset: 0x000F5A4C
	private void Start()
	{
		this._leftHandRay.attachTransform = (this._leftHandRay.rayOriginTransform = KIDHandReference.LeftHand.transform);
		this._rightHandRay.attachTransform = (this._rightHandRay.rayOriginTransform = KIDHandReference.RightHand.transform);
		this.DisableHandRays();
		this._activationCounter = 0;
	}

	// Token: 0x06003215 RID: 12821 RVA: 0x000F78AC File Offset: 0x000F5AAC
	private void OnDisable()
	{
		this.DisableHandRays();
	}

	// Token: 0x06003216 RID: 12822 RVA: 0x000F78B4 File Offset: 0x000F5AB4
	public void EnableHandRays()
	{
		if (this._activationCounter == 0)
		{
			this._controllerBehaviour.OnAction += this.PostUpdate;
			this.ToggleHands();
		}
		this._activationCounter++;
	}

	// Token: 0x06003217 RID: 12823 RVA: 0x000F78E9 File Offset: 0x000F5AE9
	public void DisableHandRays()
	{
		this._activationCounter--;
		if (this._activationCounter == 0)
		{
			this._controllerBehaviour.OnAction -= this.PostUpdate;
			this.HideHands();
		}
	}

	// Token: 0x06003218 RID: 12824 RVA: 0x000F791E File Offset: 0x000F5B1E
	public void PulseActiveHandray(float vibrationStrength, float vibrationDuration)
	{
		if (this._activeHandRay == null)
		{
			return;
		}
		this._activeHandRay.SendHapticImpulse(vibrationStrength, vibrationDuration);
	}

	// Token: 0x06003219 RID: 12825 RVA: 0x000F793D File Offset: 0x000F5B3D
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

	// Token: 0x0600321A RID: 12826 RVA: 0x000F7978 File Offset: 0x000F5B78
	private void ToggleRightHandRay(bool enabled)
	{
		Debug.LogFormat(string.Format("[KID::UI::HAND_RAY_CONTROLLER] RIGHT Hand is: {0}. Setting to: {1}", this._rightHandRay.gameObject.activeInHierarchy, enabled), Array.Empty<object>());
		this._rightHandRay.gameObject.SetActive(enabled);
		if (enabled)
		{
			this._activeHandRay = this._rightHandRay;
		}
	}

	// Token: 0x0600321B RID: 12827 RVA: 0x000F79D4 File Offset: 0x000F5BD4
	private void ToggleLeftHandRay(bool enabled)
	{
		Debug.LogFormat(string.Format("[KID::UI::HAND_RAY_CONTROLLER] LEFT Hand is: {0}. Setting to: {1}", this._rightHandRay.gameObject.activeInHierarchy, enabled), Array.Empty<object>());
		this._leftHandRay.gameObject.SetActive(enabled);
		if (enabled)
		{
			this._activeHandRay = this._leftHandRay;
		}
	}

	// Token: 0x0600321C RID: 12828 RVA: 0x000F7A30 File Offset: 0x000F5C30
	private void InitialiseHands()
	{
		Debug.Log("[KID::UI::HAND_RAY_CONTROLLER] Initialising Hands");
		this.ToggleRightHandRay(this.ActiveHand == HandRayController.HandSide.Right);
		this.ToggleLeftHandRay(this.ActiveHand == HandRayController.HandSide.Left);
		this._hasInitialised = true;
	}

	// Token: 0x0600321D RID: 12829 RVA: 0x000F7A64 File Offset: 0x000F5C64
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

	// Token: 0x0600321E RID: 12830 RVA: 0x000F7AF9 File Offset: 0x000F5CF9
	private void HideHands()
	{
		this.ToggleRightHandRay(false);
		this.ToggleLeftHandRay(false);
		this._hasInitialised = false;
		this._activeHandRay = null;
	}

	// Token: 0x040038CA RID: 14538
	private static HandRayController instance;

	// Token: 0x040038CB RID: 14539
	[SerializeField]
	private XRRayInteractor _leftHandRay;

	// Token: 0x040038CC RID: 14540
	[SerializeField]
	private XRRayInteractor _rightHandRay;

	// Token: 0x040038CD RID: 14541
	private bool _hasInitialised;

	// Token: 0x040038CE RID: 14542
	private ControllerBehaviour _controllerBehaviour;

	// Token: 0x040038CF RID: 14543
	private HandRayController.HandSide ActiveHand = HandRayController.HandSide.Right;

	// Token: 0x040038D0 RID: 14544
	private XRRayInteractor _activeHandRay;

	// Token: 0x040038D1 RID: 14545
	private int _activationCounter;

	// Token: 0x020007FB RID: 2043
	private enum HandSide
	{
		// Token: 0x040038D3 RID: 14547
		Left,
		// Token: 0x040038D4 RID: 14548
		Right
	}
}
