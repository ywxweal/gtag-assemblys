using System;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x02000C02 RID: 3074
	public class WindmillController : MonoBehaviour
	{
		// Token: 0x06004BF7 RID: 19447 RVA: 0x00167D4B File Offset: 0x00165F4B
		private void Awake()
		{
			this._bladesRotation = base.GetComponentInChildren<WindmillBladesController>();
			this._bladesRotation.SetMoveState(true, this._maxSpeed);
		}

		// Token: 0x06004BF8 RID: 19448 RVA: 0x00167D6B File Offset: 0x00165F6B
		private void OnEnable()
		{
			this._startStopButton.GetComponent<Interactable>().InteractableStateChanged.AddListener(new UnityAction<InteractableStateArgs>(this.StartStopStateChanged));
		}

		// Token: 0x06004BF9 RID: 19449 RVA: 0x00167D8E File Offset: 0x00165F8E
		private void OnDisable()
		{
			if (this._startStopButton != null)
			{
				this._startStopButton.GetComponent<Interactable>().InteractableStateChanged.RemoveListener(new UnityAction<InteractableStateArgs>(this.StartStopStateChanged));
			}
		}

		// Token: 0x06004BFA RID: 19450 RVA: 0x00167DC0 File Offset: 0x00165FC0
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

		// Token: 0x06004BFB RID: 19451 RVA: 0x00167E24 File Offset: 0x00166024
		private void Update()
		{
			if (this._toolInteractingWithMe == null)
			{
				this._selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
				return;
			}
			this._selectionCylinder.CurrSelectionState = ((this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown || this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay) ? SelectionCylinder.SelectionState.Highlighted : SelectionCylinder.SelectionState.Selected);
		}

		// Token: 0x04004EAD RID: 20141
		[SerializeField]
		private GameObject _startStopButton;

		// Token: 0x04004EAE RID: 20142
		[SerializeField]
		private float _maxSpeed = 10f;

		// Token: 0x04004EAF RID: 20143
		[SerializeField]
		private SelectionCylinder _selectionCylinder;

		// Token: 0x04004EB0 RID: 20144
		private WindmillBladesController _bladesRotation;

		// Token: 0x04004EB1 RID: 20145
		private InteractableTool _toolInteractingWithMe;
	}
}
