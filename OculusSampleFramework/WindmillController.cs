using System;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x02000C02 RID: 3074
	public class WindmillController : MonoBehaviour
	{
		// Token: 0x06004BF8 RID: 19448 RVA: 0x00167E23 File Offset: 0x00166023
		private void Awake()
		{
			this._bladesRotation = base.GetComponentInChildren<WindmillBladesController>();
			this._bladesRotation.SetMoveState(true, this._maxSpeed);
		}

		// Token: 0x06004BF9 RID: 19449 RVA: 0x00167E43 File Offset: 0x00166043
		private void OnEnable()
		{
			this._startStopButton.GetComponent<Interactable>().InteractableStateChanged.AddListener(new UnityAction<InteractableStateArgs>(this.StartStopStateChanged));
		}

		// Token: 0x06004BFA RID: 19450 RVA: 0x00167E66 File Offset: 0x00166066
		private void OnDisable()
		{
			if (this._startStopButton != null)
			{
				this._startStopButton.GetComponent<Interactable>().InteractableStateChanged.RemoveListener(new UnityAction<InteractableStateArgs>(this.StartStopStateChanged));
			}
		}

		// Token: 0x06004BFB RID: 19451 RVA: 0x00167E98 File Offset: 0x00166098
		private void StartStopStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				if (this._bladesRotation.IsMoving)
				{
					this._bladesRotation.SetMoveState(false, 0f);
				}
				else
				{
					this._bladesRotation.SetMoveState(true, this._maxSpeed);
				}
			}
			this._toolInteractingWithMe = ((obj.NewInteractableState > InteractableState.Default) ? obj.Tool : null);
		}

		// Token: 0x06004BFC RID: 19452 RVA: 0x00167EFC File Offset: 0x001660FC
		private void Update()
		{
			if (this._toolInteractingWithMe == null)
			{
				this._selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
				return;
			}
			this._selectionCylinder.CurrSelectionState = ((this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown || this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay) ? SelectionCylinder.SelectionState.Highlighted : SelectionCylinder.SelectionState.Selected);
		}

		// Token: 0x04004EAE RID: 20142
		[SerializeField]
		private GameObject _startStopButton;

		// Token: 0x04004EAF RID: 20143
		[SerializeField]
		private float _maxSpeed = 10f;

		// Token: 0x04004EB0 RID: 20144
		[SerializeField]
		private SelectionCylinder _selectionCylinder;

		// Token: 0x04004EB1 RID: 20145
		private WindmillBladesController _bladesRotation;

		// Token: 0x04004EB2 RID: 20146
		private InteractableTool _toolInteractingWithMe;
	}
}
