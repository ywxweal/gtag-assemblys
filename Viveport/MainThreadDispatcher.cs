using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viveport
{
	// Token: 0x02000A3F RID: 2623
	public class MainThreadDispatcher : MonoBehaviour
	{
		// Token: 0x06003E5C RID: 15964 RVA: 0x00127F79 File Offset: 0x00126179
		private void Awake()
		{
			if (MainThreadDispatcher.instance == null)
			{
				MainThreadDispatcher.instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		// Token: 0x06003E5D RID: 15965 RVA: 0x00127F9C File Offset: 0x0012619C
		public void Update()
		{
			Queue<Action> queue = MainThreadDispatcher.actions;
			lock (queue)
			{
				while (MainThreadDispatcher.actions.Count > 0)
				{
					MainThreadDispatcher.actions.Dequeue()();
				}
			}
		}

		// Token: 0x06003E5E RID: 15966 RVA: 0x00127FF4 File Offset: 0x001261F4
		public static MainThreadDispatcher Instance()
		{
			if (MainThreadDispatcher.instance == null)
			{
				throw new Exception("Could not find the MainThreadDispatcher GameObject. Please ensure you have added this script to an empty GameObject in your scene.");
			}
			return MainThreadDispatcher.instance;
		}

		// Token: 0x06003E5F RID: 15967 RVA: 0x00128013 File Offset: 0x00126213
		private void OnDestroy()
		{
			MainThreadDispatcher.instance = null;
		}

		// Token: 0x06003E60 RID: 15968 RVA: 0x0012801C File Offset: 0x0012621C
		public void Enqueue(IEnumerator action)
		{
			Queue<Action> queue = MainThreadDispatcher.actions;
			lock (queue)
			{
				MainThreadDispatcher.actions.Enqueue(delegate
				{
					this.StartCoroutine(action);
				});
			}
		}

		// Token: 0x06003E61 RID: 15969 RVA: 0x00128080 File Offset: 0x00126280
		public void Enqueue(Action action)
		{
			this.Enqueue(this.ActionWrapper(action));
		}

		// Token: 0x06003E62 RID: 15970 RVA: 0x0012808F File Offset: 0x0012628F
		public void Enqueue<T1>(Action<T1> action, T1 param1)
		{
			this.Enqueue(this.ActionWrapper<T1>(action, param1));
		}

		// Token: 0x06003E63 RID: 15971 RVA: 0x0012809F File Offset: 0x0012629F
		public void Enqueue<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			this.Enqueue(this.ActionWrapper<T1, T2>(action, param1, param2));
		}

		// Token: 0x06003E64 RID: 15972 RVA: 0x001280B0 File Offset: 0x001262B0
		public void Enqueue<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3>(action, param1, param2, param3));
		}

		// Token: 0x06003E65 RID: 15973 RVA: 0x001280C3 File Offset: 0x001262C3
		public void Enqueue<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			this.Enqueue(this.ActionWrapper<T1, T2, T3, T4>(action, param1, param2, param3, param4));
		}

		// Token: 0x06003E66 RID: 15974 RVA: 0x001280D8 File Offset: 0x001262D8
		private IEnumerator ActionWrapper(Action action)
		{
			action();
			yield return null;
			yield break;
		}

		// Token: 0x06003E67 RID: 15975 RVA: 0x001280E7 File Offset: 0x001262E7
		private IEnumerator ActionWrapper<T1>(Action<T1> action, T1 param1)
		{
			action(param1);
			yield return null;
			yield break;
		}

		// Token: 0x06003E68 RID: 15976 RVA: 0x001280FD File Offset: 0x001262FD
		private IEnumerator ActionWrapper<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
		{
			action(param1, param2);
			yield return null;
			yield break;
		}

		// Token: 0x06003E69 RID: 15977 RVA: 0x0012811A File Offset: 0x0012631A
		private IEnumerator ActionWrapper<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
		{
			action(param1, param2, param3);
			yield return null;
			yield break;
		}

		// Token: 0x06003E6A RID: 15978 RVA: 0x0012813F File Offset: 0x0012633F
		private IEnumerator ActionWrapper<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
		{
			action(param1, param2, param3, param4);
			yield return null;
			yield break;
		}

		// Token: 0x040042E7 RID: 17127
		private static readonly Queue<Action> actions = new Queue<Action>();

		// Token: 0x040042E8 RID: 17128
		private static MainThreadDispatcher instance = null;
	}
}
