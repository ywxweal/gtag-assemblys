using System;

namespace GorillaTagScripts
{
	// Token: 0x02000ADE RID: 2782
	public class BuilderOptionButton : GorillaPressableButton
	{
		// Token: 0x06004361 RID: 17249 RVA: 0x00137A2F File Offset: 0x00135C2F
		public override void Start()
		{
			base.Start();
		}

		// Token: 0x06004362 RID: 17250 RVA: 0x000023F4 File Offset: 0x000005F4
		private void OnDestroy()
		{
		}

		// Token: 0x06004363 RID: 17251 RVA: 0x00137A37 File Offset: 0x00135C37
		public void Setup(Action<BuilderOptionButton, bool> onPressed)
		{
			this.onPressed = onPressed;
		}

		// Token: 0x06004364 RID: 17252 RVA: 0x00137A40 File Offset: 0x00135C40
		public override void ButtonActivationWithHand(bool isLeftHand)
		{
			Action<BuilderOptionButton, bool> action = this.onPressed;
			if (action == null)
			{
				return;
			}
			action(this, isLeftHand);
		}

		// Token: 0x06004365 RID: 17253 RVA: 0x00137A54 File Offset: 0x00135C54
		public void SetPressed(bool pressed)
		{
			this.buttonRenderer.material = (pressed ? this.pressedMaterial : this.unpressedMaterial);
		}

		// Token: 0x040045E9 RID: 17897
		private new Action<BuilderOptionButton, bool> onPressed;
	}
}
