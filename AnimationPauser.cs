using System;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x020007F8 RID: 2040
public class AnimationPauser : StateMachineBehaviour
{
	// Token: 0x0600320C RID: 12812 RVA: 0x000F7654 File Offset: 0x000F5854
	public override async void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		this._animPauseDuration = Random.Range(this._minTimeBetweenAnims, this._maxTimeBetweenAnims);
		await Task.Delay(this._animPauseDuration * 1000);
		animator.SetTrigger(AnimationPauser.Restart_Anim_Name);
	}

	// Token: 0x040038BF RID: 14527
	[SerializeField]
	private int _maxTimeBetweenAnims = 5;

	// Token: 0x040038C0 RID: 14528
	[SerializeField]
	private int _minTimeBetweenAnims = 1;

	// Token: 0x040038C1 RID: 14529
	private int _animPauseDuration;

	// Token: 0x040038C2 RID: 14530
	private static readonly string Restart_Anim_Name = "RestartAnim";
}
