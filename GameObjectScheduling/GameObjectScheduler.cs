using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x02000E29 RID: 3625
	public class GameObjectScheduler : MonoBehaviour
	{
		// Token: 0x06005AB1 RID: 23217 RVA: 0x001B9A80 File Offset: 0x001B7C80
		private void Start()
		{
			this.schedule.Validate();
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < base.transform.childCount; i++)
			{
				list.Add(base.transform.GetChild(i).gameObject);
			}
			this.scheduledGameObject = list.ToArray();
			for (int j = 0; j < this.scheduledGameObject.Length; j++)
			{
				this.scheduledGameObject[j].SetActive(false);
			}
			this.dispatcher = base.GetComponent<GameObjectSchedulerEventDispatcher>();
			this.monitor = base.StartCoroutine(this.MonitorTime());
		}

		// Token: 0x06005AB2 RID: 23218 RVA: 0x001B9B16 File Offset: 0x001B7D16
		private void OnEnable()
		{
			if (this.monitor == null && this.scheduledGameObject != null)
			{
				this.monitor = base.StartCoroutine(this.MonitorTime());
			}
		}

		// Token: 0x06005AB3 RID: 23219 RVA: 0x001B9B3A File Offset: 0x001B7D3A
		private void OnDisable()
		{
			if (this.monitor != null)
			{
				base.StopCoroutine(this.monitor);
			}
			this.monitor = null;
		}

		// Token: 0x06005AB4 RID: 23220 RVA: 0x001B9B57 File Offset: 0x001B7D57
		private IEnumerator MonitorTime()
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			bool previousState = this.getActiveState();
			for (int i = 0; i < this.scheduledGameObject.Length; i++)
			{
				this.scheduledGameObject[i].SetActive(previousState);
			}
			for (;;)
			{
				yield return new WaitForSeconds(60f);
				bool activeState = this.getActiveState();
				if (previousState != activeState)
				{
					this.changeActiveState(activeState);
					previousState = activeState;
				}
			}
			yield break;
		}

		// Token: 0x06005AB5 RID: 23221 RVA: 0x001B9B68 File Offset: 0x001B7D68
		private bool getActiveState()
		{
			this.currentNodeIndex = this.schedule.GetCurrentNodeIndex(this.getServerTime(), 0);
			bool flag;
			if (this.currentNodeIndex == -1)
			{
				flag = this.schedule.InitialState;
			}
			else if (this.currentNodeIndex < this.schedule.Nodes.Length)
			{
				flag = this.schedule.Nodes[this.currentNodeIndex].ActiveState;
			}
			else
			{
				flag = this.schedule.Nodes[this.schedule.Nodes.Length - 1].ActiveState;
			}
			return flag;
		}

		// Token: 0x06005AB6 RID: 23222 RVA: 0x0017C557 File Offset: 0x0017A757
		private DateTime getServerTime()
		{
			return GorillaComputer.instance.GetServerTime();
		}

		// Token: 0x06005AB7 RID: 23223 RVA: 0x001B9BF8 File Offset: 0x001B7DF8
		private void changeActiveState(bool state)
		{
			if (state)
			{
				for (int i = 0; i < this.scheduledGameObject.Length; i++)
				{
					this.scheduledGameObject[i].SetActive(true);
				}
				if (this.dispatcher != null && this.dispatcher.OnScheduledActivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
			}
			else
			{
				if (this.dispatcher != null && this.dispatcher.OnScheduledDeactivation != null)
				{
					this.dispatcher.OnScheduledActivation.Invoke();
					return;
				}
				for (int j = 0; j < this.scheduledGameObject.Length; j++)
				{
					this.scheduledGameObject[j].SetActive(false);
				}
			}
		}

		// Token: 0x04005EBD RID: 24253
		[SerializeField]
		private GameObjectSchedule schedule;

		// Token: 0x04005EBE RID: 24254
		private GameObject[] scheduledGameObject;

		// Token: 0x04005EBF RID: 24255
		private GameObjectSchedulerEventDispatcher dispatcher;

		// Token: 0x04005EC0 RID: 24256
		private int currentNodeIndex = -1;

		// Token: 0x04005EC1 RID: 24257
		private Coroutine monitor;
	}
}
