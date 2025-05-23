using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityChan
{
	// Token: 0x02000E49 RID: 3657
	public class IdleChanger : MonoBehaviour
	{
		// Token: 0x06005B8E RID: 23438 RVA: 0x001C18C5 File Offset: 0x001BFAC5
		private void Start()
		{
			this.currentState = this.UnityChanA.GetCurrentAnimatorStateInfo(0);
			this.previousState = this.currentState;
			base.StartCoroutine("RandomChange");
			this.kb = Keyboard.current;
		}

		// Token: 0x06005B8F RID: 23439 RVA: 0x001C18FC File Offset: 0x001BFAFC
		private void Update()
		{
			if (this.kb.upArrowKey.wasPressedThisFrame || this.kb.spaceKey.wasPressedThisFrame)
			{
				this.UnityChanA.SetBool("Next", true);
				this.UnityChanB.SetBool("Next", true);
			}
			if (this.kb.downArrowKey.wasPressedThisFrame)
			{
				this.UnityChanA.SetBool("Back", true);
				this.UnityChanB.SetBool("Back", true);
			}
			if (this.UnityChanA.GetBool("Next"))
			{
				this.currentState = this.UnityChanA.GetCurrentAnimatorStateInfo(0);
				if (this.previousState.fullPathHash != this.currentState.fullPathHash)
				{
					this.UnityChanA.SetBool("Next", false);
					this.UnityChanB.SetBool("Next", false);
					this.previousState = this.currentState;
				}
			}
			if (this.UnityChanA.GetBool("Back"))
			{
				this.currentState = this.UnityChanA.GetCurrentAnimatorStateInfo(0);
				if (this.previousState.fullPathHash != this.currentState.fullPathHash)
				{
					this.UnityChanA.SetBool("Back", false);
					this.UnityChanB.SetBool("Back", false);
					this.previousState = this.currentState;
				}
			}
		}

		// Token: 0x06005B90 RID: 23440 RVA: 0x001C1A58 File Offset: 0x001BFC58
		private void OnGUI()
		{
			if (this.isGUI)
			{
				GUI.Box(new Rect((float)(Screen.width - 110), 10f, 100f, 90f), "Change Motion");
				if (GUI.Button(new Rect((float)(Screen.width - 100), 40f, 80f, 20f), "Next"))
				{
					this.UnityChanA.SetBool("Next", true);
					this.UnityChanB.SetBool("Next", true);
				}
				if (GUI.Button(new Rect((float)(Screen.width - 100), 70f, 80f, 20f), "Back"))
				{
					this.UnityChanA.SetBool("Back", true);
					this.UnityChanB.SetBool("Back", true);
				}
			}
		}

		// Token: 0x06005B91 RID: 23441 RVA: 0x001C1B2D File Offset: 0x001BFD2D
		private IEnumerator RandomChange()
		{
			for (;;)
			{
				if (this._random)
				{
					float num = Random.Range(0f, 1f);
					if (num < this._threshold)
					{
						this.UnityChanA.SetBool("Back", true);
						this.UnityChanB.SetBool("Back", true);
					}
					else if (num >= this._threshold)
					{
						this.UnityChanA.SetBool("Next", true);
						this.UnityChanB.SetBool("Next", true);
					}
				}
				yield return new WaitForSeconds(this._interval);
			}
			yield break;
		}

		// Token: 0x04005F56 RID: 24406
		private AnimatorStateInfo currentState;

		// Token: 0x04005F57 RID: 24407
		private AnimatorStateInfo previousState;

		// Token: 0x04005F58 RID: 24408
		public bool _random;

		// Token: 0x04005F59 RID: 24409
		public float _threshold = 0.5f;

		// Token: 0x04005F5A RID: 24410
		public float _interval = 10f;

		// Token: 0x04005F5B RID: 24411
		public bool isGUI = true;

		// Token: 0x04005F5C RID: 24412
		public Animator UnityChanA;

		// Token: 0x04005F5D RID: 24413
		public Animator UnityChanB;

		// Token: 0x04005F5E RID: 24414
		private Keyboard kb;
	}
}
