using System;

namespace GorillaTagScripts
{
	// Token: 0x02000ADE RID: 2782
	public class BuilderOptionButton : GorillaPressableButton
	{
		// Token: 0x06004362 RID: 17250 RVA: 0x00137B07 File Offset: 0x00135D07
		public override void Start()
		{
			base.Start();
		}

		// Token: 0x06004363 RID: 17251 RVA: 0x000023F4 File Offset: 0x000005F4
		private void OnDestroy()
		{
		}

		// Token: 0x06004364 RID: 17252 RVA: 0x00137B0F File Offset: 0x00135D0F
		public void Setup(Action<BuilderOptionButton, bool> onPressed)
		{
			this.onPressed = onPressed;
		}

		// Token: 0x06004365 RID: 17253 RVA: 0x00137B18 File Offset: 0x00135D18
		public override void ButtonActivationWithHand(bool isLeftHand)
		{
			Action<BuilderOptionButton, bool> action = this.onPressed;
			if (action == null)
			{
				return;
			}
			action(this, isLeftHand);
		}

		// Token: 0x06004366 RID: 17254 RVA: 0x00137B2C File Offset: 0x00135D2C
		public void SetPressed(bool pressed)
		{
			this.buttonRenderer.material = (pressed ? this.pressedMaterial : this.unpressedMaterial);
		}

		// Token: 0x040045EA RID: 17898
		private new Action<BuilderOptionButton, bool> onPressed;
	}
}
