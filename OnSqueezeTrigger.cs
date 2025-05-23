using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000191 RID: 401
public class OnSqueezeTrigger : MonoBehaviour
{
	// Token: 0x060009E4 RID: 2532 RVA: 0x0003490B File Offset: 0x00032B0B
	private void Start()
	{
		this.myRig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x0003491C File Offset: 0x00032B1C
	private void Update()
	{
		bool flag;
		if (this.myHoldable.InLeftHand())
		{
			flag = (this.indexFinger ? this.myRig.leftIndex.calcT : this.myRig.leftMiddle.calcT) > 0.5f;
		}
		else
		{
			flag = this.myHoldable.InRightHand() && (this.indexFinger ? this.myRig.rightIndex.calcT : this.myRig.rightMiddle.calcT) > 0.5f;
		}
		if (flag != this.triggerWasDown)
		{
			if (flag)
			{
				this.onPress.Invoke();
				this.updateWhilePressed.Invoke();
			}
			else
			{
				this.onRelease.Invoke();
			}
		}
		else if (flag)
		{
			this.updateWhilePressed.Invoke();
		}
		this.triggerWasDown = flag;
	}

	// Token: 0x04000C0C RID: 3084
	[SerializeField]
	private TransferrableObject myHoldable;

	// Token: 0x04000C0D RID: 3085
	[SerializeField]
	private UnityEvent onPress;

	// Token: 0x04000C0E RID: 3086
	[SerializeField]
	private UnityEvent onRelease;

	// Token: 0x04000C0F RID: 3087
	[SerializeField]
	private UnityEvent updateWhilePressed;

	// Token: 0x04000C10 RID: 3088
	private VRRig myRig;

	// Token: 0x04000C11 RID: 3089
	private bool indexFinger = true;

	// Token: 0x04000C12 RID: 3090
	private bool triggerWasDown;
}
