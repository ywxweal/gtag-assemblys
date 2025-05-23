using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000B05 RID: 2821
	public class Flower : MonoBehaviour
	{
		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x0600450F RID: 17679 RVA: 0x0014728B File Offset: 0x0014548B
		// (set) Token: 0x06004510 RID: 17680 RVA: 0x00147293 File Offset: 0x00145493
		public bool IsWatered { get; private set; }

		// Token: 0x06004511 RID: 17681 RVA: 0x0014729C File Offset: 0x0014549C
		private void Awake()
		{
			this.shouldUpdateVisuals = true;
			this.anim = base.GetComponent<Animator>();
			this.timer = base.GetComponent<GorillaTimer>();
			this.perchPoint = base.GetComponent<BeePerchPoint>();
			this.timer.onTimerStopped.AddListener(new UnityAction<GorillaTimer>(this.HandleOnFlowerTimerEnded));
			this.currentState = Flower.FlowerState.None;
			this.wateredFx = this.wateredFx.GetComponent<ParticleSystem>();
			this.IsWatered = false;
			this.meshRenderer = base.GetComponent<SkinnedMeshRenderer>();
			this.meshRenderer.enabled = false;
			this.anim.enabled = false;
		}

		// Token: 0x06004512 RID: 17682 RVA: 0x00147333 File Offset: 0x00145533
		private void OnDestroy()
		{
			this.timer.onTimerStopped.RemoveListener(new UnityAction<GorillaTimer>(this.HandleOnFlowerTimerEnded));
		}

		// Token: 0x06004513 RID: 17683 RVA: 0x00147354 File Offset: 0x00145554
		public void WaterFlower(bool isWatered = false)
		{
			this.IsWatered = isWatered;
			switch (this.currentState)
			{
			case Flower.FlowerState.None:
				this.UpdateFlowerState(Flower.FlowerState.Healthy, false, true);
				return;
			case Flower.FlowerState.Healthy:
				if (!isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Middle, false, true);
					return;
				}
				break;
			case Flower.FlowerState.Middle:
				if (isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Healthy, true, true);
					return;
				}
				this.UpdateFlowerState(Flower.FlowerState.Wilted, false, true);
				return;
			case Flower.FlowerState.Wilted:
				if (isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Middle, true, true);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06004514 RID: 17684 RVA: 0x001473C4 File Offset: 0x001455C4
		public void UpdateFlowerState(Flower.FlowerState newState, bool isWatered = false, bool updateVisual = true)
		{
			if (FlowersManager.Instance.IsMine)
			{
				this.timer.RestartTimer();
			}
			this.ChangeState(newState);
			if (this.perchPoint)
			{
				this.perchPoint.enabled = this.currentState == Flower.FlowerState.Healthy;
			}
			if (updateVisual)
			{
				this.LocalUpdateFlowers(newState, isWatered);
			}
		}

		// Token: 0x06004515 RID: 17685 RVA: 0x0014741C File Offset: 0x0014561C
		private void LocalUpdateFlowers(Flower.FlowerState state, bool isWatered = false)
		{
			GameObject[] array = this.meshStates;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			if (!this.shouldUpdateVisuals)
			{
				this.meshStates[(int)this.currentState].SetActive(true);
				return;
			}
			if (isWatered && this.wateredFx)
			{
				this.wateredFx.Play();
			}
			this.meshRenderer.enabled = true;
			this.anim.enabled = true;
			switch (state)
			{
			case Flower.FlowerState.Healthy:
				this.anim.SetTrigger(Flower.middle_to_healthy);
				return;
			case Flower.FlowerState.Middle:
				if (this.lastState == Flower.FlowerState.Wilted)
				{
					this.anim.SetTrigger(Flower.wilted_to_middle);
					return;
				}
				this.anim.SetTrigger(Flower.healthy_to_middle);
				return;
			case Flower.FlowerState.Wilted:
				this.anim.SetTrigger(Flower.middle_to_wilted);
				return;
			default:
				return;
			}
		}

		// Token: 0x06004516 RID: 17686 RVA: 0x001474F5 File Offset: 0x001456F5
		private void HandleOnFlowerTimerEnded(GorillaTimer _timer)
		{
			if (!FlowersManager.Instance.IsMine)
			{
				return;
			}
			if (this.timer == _timer)
			{
				this.WaterFlower(false);
			}
		}

		// Token: 0x06004517 RID: 17687 RVA: 0x00147519 File Offset: 0x00145719
		private void ChangeState(Flower.FlowerState state)
		{
			this.lastState = this.currentState;
			this.currentState = state;
		}

		// Token: 0x06004518 RID: 17688 RVA: 0x0014752E File Offset: 0x0014572E
		public Flower.FlowerState GetCurrentState()
		{
			return this.currentState;
		}

		// Token: 0x06004519 RID: 17689 RVA: 0x00147538 File Offset: 0x00145738
		public void OnAnimationIsDone(int state)
		{
			if (this.meshRenderer.enabled)
			{
				for (int i = 0; i < this.meshStates.Length; i++)
				{
					bool flag = i == (int)this.currentState;
					this.meshStates[i].SetActive(flag);
				}
				this.anim.enabled = false;
				this.meshRenderer.enabled = false;
			}
		}

		// Token: 0x0600451A RID: 17690 RVA: 0x00147595 File Offset: 0x00145795
		public void UpdateVisuals(bool enable)
		{
			this.shouldUpdateVisuals = enable;
			this.meshStatesGameObject.SetActive(enable);
		}

		// Token: 0x0600451B RID: 17691 RVA: 0x001475AC File Offset: 0x001457AC
		public void AnimCatch()
		{
			if (this.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				this.OnAnimationIsDone(0);
			}
		}

		// Token: 0x040047C2 RID: 18370
		private Animator anim;

		// Token: 0x040047C3 RID: 18371
		private SkinnedMeshRenderer meshRenderer;

		// Token: 0x040047C4 RID: 18372
		[HideInInspector]
		public GorillaTimer timer;

		// Token: 0x040047C5 RID: 18373
		private BeePerchPoint perchPoint;

		// Token: 0x040047C6 RID: 18374
		public ParticleSystem wateredFx;

		// Token: 0x040047C7 RID: 18375
		public ParticleSystem sparkleFx;

		// Token: 0x040047C8 RID: 18376
		public GameObject meshStatesGameObject;

		// Token: 0x040047C9 RID: 18377
		public GameObject[] meshStates;

		// Token: 0x040047CA RID: 18378
		private static readonly int healthy_to_middle = Animator.StringToHash("healthy_to_middle");

		// Token: 0x040047CB RID: 18379
		private static readonly int middle_to_healthy = Animator.StringToHash("middle_to_healthy");

		// Token: 0x040047CC RID: 18380
		private static readonly int wilted_to_middle = Animator.StringToHash("wilted_to_middle");

		// Token: 0x040047CD RID: 18381
		private static readonly int middle_to_wilted = Animator.StringToHash("middle_to_wilted");

		// Token: 0x040047CE RID: 18382
		private Flower.FlowerState currentState;

		// Token: 0x040047CF RID: 18383
		private string id;

		// Token: 0x040047D0 RID: 18384
		private bool shouldUpdateVisuals;

		// Token: 0x040047D1 RID: 18385
		private Flower.FlowerState lastState;

		// Token: 0x02000B06 RID: 2822
		public enum FlowerState
		{
			// Token: 0x040047D4 RID: 18388
			None = -1,
			// Token: 0x040047D5 RID: 18389
			Healthy,
			// Token: 0x040047D6 RID: 18390
			Middle,
			// Token: 0x040047D7 RID: 18391
			Wilted
		}
	}
}
