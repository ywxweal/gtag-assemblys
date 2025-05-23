using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using UnityEngine;

// Token: 0x0200094E RID: 2382
internal abstract class TickSystem<T> : MonoBehaviour
{
	// Token: 0x060039C0 RID: 14784 RVA: 0x0011611E File Offset: 0x0011431E
	private void Awake()
	{
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(this);
	}

	// Token: 0x060039C1 RID: 14785 RVA: 0x00116133 File Offset: 0x00114333
	private void Update()
	{
		TickSystem<T>.preTickCallbacks.TryRunCallbacks();
		TickSystem<T>.tickCallbacks.TryRunCallbacks();
	}

	// Token: 0x060039C2 RID: 14786 RVA: 0x00116149 File Offset: 0x00114349
	private void LateUpdate()
	{
		TickSystem<T>.postTickCallbacks.TryRunCallbacks();
	}

	// Token: 0x060039C4 RID: 14788 RVA: 0x001161CB File Offset: 0x001143CB
	private static void OnEnterPlay()
	{
		TickSystem<T>.preTickCallbacks.Clear();
		TickSystem<T>.preTickWrapperTable.Clear();
		TickSystem<T>.tickCallbacks.Clear();
		TickSystem<T>.tickWrapperTable.Clear();
		TickSystem<T>.postTickCallbacks.Clear();
		TickSystem<T>.postTickWrapperTable.Clear();
	}

	// Token: 0x060039C5 RID: 14789 RVA: 0x0011620C File Offset: 0x0011440C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPreTickCallback(ITickSystemPre callback)
	{
		if (callback.PreTickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperPre tickCallbackWrapperPre = TickSystem<T>.preTickWrapperPool.Take();
		tickCallbackWrapperPre.target = callback;
		TickSystem<T>.preTickWrapperTable[callback] = tickCallbackWrapperPre;
		TickSystem<T>.preTickCallbacks.Add(tickCallbackWrapperPre);
		callback.PreTickRunning = true;
	}

	// Token: 0x060039C6 RID: 14790 RVA: 0x00116254 File Offset: 0x00114454
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddTickCallback(ITickSystemTick callback)
	{
		if (callback.TickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperTick tickCallbackWrapperTick = TickSystem<T>.tickWrapperPool.Take();
		tickCallbackWrapperTick.target = callback;
		TickSystem<T>.tickWrapperTable[callback] = tickCallbackWrapperTick;
		TickSystem<T>.tickCallbacks.Add(tickCallbackWrapperTick);
		callback.TickRunning = true;
	}

	// Token: 0x060039C7 RID: 14791 RVA: 0x0011629C File Offset: 0x0011449C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPostTickCallback(ITickSystemPost callback)
	{
		if (callback.PostTickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperPost tickCallbackWrapperPost = TickSystem<T>.postTickWrapperPool.Take();
		tickCallbackWrapperPost.target = callback;
		TickSystem<T>.postTickWrapperTable[callback] = tickCallbackWrapperPost;
		TickSystem<T>.postTickCallbacks.Add(tickCallbackWrapperPost);
		callback.PostTickRunning = true;
	}

	// Token: 0x060039C8 RID: 14792 RVA: 0x001162E2 File Offset: 0x001144E2
	public static void AddTickSystemCallBack(ITickSystem callback)
	{
		TickSystem<T>.AddPreTickCallback(callback);
		TickSystem<T>.AddTickCallback(callback);
		TickSystem<T>.AddPostTickCallback(callback);
	}

	// Token: 0x060039C9 RID: 14793 RVA: 0x001162F8 File Offset: 0x001144F8
	public static void AddCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem<T>.AddTickSystemCallBack(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem<T>.AddPreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem<T>.AddTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem<T>.AddPostTickCallback(tickSystemPost);
		}
	}

	// Token: 0x060039CA RID: 14794 RVA: 0x00116348 File Offset: 0x00114548
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePreTickCallback(ITickSystemPre callback)
	{
		TickSystem<T>.TickCallbackWrapperPre tickCallbackWrapperPre;
		if (!callback.PreTickRunning || !TickSystem<T>.preTickWrapperTable.TryGetValue(callback, out tickCallbackWrapperPre))
		{
			return;
		}
		TickSystem<T>.preTickCallbacks.Remove(tickCallbackWrapperPre);
		callback.PreTickRunning = false;
		TickSystem<T>.preTickWrapperPool.Return(tickCallbackWrapperPre);
	}

	// Token: 0x060039CB RID: 14795 RVA: 0x0011638C File Offset: 0x0011458C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveTickCallback(ITickSystemTick callback)
	{
		TickSystem<T>.TickCallbackWrapperTick tickCallbackWrapperTick;
		if (!callback.TickRunning || !TickSystem<T>.tickWrapperTable.TryGetValue(callback, out tickCallbackWrapperTick))
		{
			return;
		}
		TickSystem<T>.tickCallbacks.Remove(tickCallbackWrapperTick);
		callback.TickRunning = false;
		TickSystem<T>.tickWrapperPool.Return(tickCallbackWrapperTick);
	}

	// Token: 0x060039CC RID: 14796 RVA: 0x001163D0 File Offset: 0x001145D0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePostTickCallback(ITickSystemPost callback)
	{
		TickSystem<T>.TickCallbackWrapperPost tickCallbackWrapperPost;
		if (!callback.PostTickRunning || !TickSystem<T>.postTickWrapperTable.TryGetValue(callback, out tickCallbackWrapperPost))
		{
			return;
		}
		TickSystem<T>.postTickCallbacks.Remove(tickCallbackWrapperPost);
		callback.PostTickRunning = false;
		TickSystem<T>.postTickWrapperPool.Return(tickCallbackWrapperPost);
	}

	// Token: 0x060039CD RID: 14797 RVA: 0x00116412 File Offset: 0x00114612
	public static void RemoveTickSystemCallback(ITickSystem callback)
	{
		TickSystem<T>.RemovePreTickCallback(callback);
		TickSystem<T>.RemoveTickCallback(callback);
		TickSystem<T>.RemovePostTickCallback(callback);
	}

	// Token: 0x060039CE RID: 14798 RVA: 0x00116428 File Offset: 0x00114628
	public static void RemoveCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem<T>.RemoveTickSystemCallback(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem<T>.RemovePreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem<T>.RemoveTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem<T>.RemovePostTickCallback(tickSystemPost);
		}
	}

	// Token: 0x04003EE0 RID: 16096
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperPre> preTickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperPre>(100);

	// Token: 0x04003EE1 RID: 16097
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperPre> preTickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperPre>();

	// Token: 0x04003EE2 RID: 16098
	private static readonly Dictionary<ITickSystemPre, TickSystem<T>.TickCallbackWrapperPre> preTickWrapperTable = new Dictionary<ITickSystemPre, TickSystem<T>.TickCallbackWrapperPre>(100);

	// Token: 0x04003EE3 RID: 16099
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperTick> tickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperTick>(100);

	// Token: 0x04003EE4 RID: 16100
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperTick> tickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperTick>();

	// Token: 0x04003EE5 RID: 16101
	private static readonly Dictionary<ITickSystemTick, TickSystem<T>.TickCallbackWrapperTick> tickWrapperTable = new Dictionary<ITickSystemTick, TickSystem<T>.TickCallbackWrapperTick>(100);

	// Token: 0x04003EE6 RID: 16102
	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperPost> postTickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperPost>(100);

	// Token: 0x04003EE7 RID: 16103
	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperPost> postTickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperPost>();

	// Token: 0x04003EE8 RID: 16104
	private static readonly Dictionary<ITickSystemPost, TickSystem<T>.TickCallbackWrapperPost> postTickWrapperTable = new Dictionary<ITickSystemPost, TickSystem<T>.TickCallbackWrapperPost>(100);

	// Token: 0x0200094F RID: 2383
	private class TickCallbackWrapper<U> : ObjectPoolEvents, ICallBack where U : class
	{
		// Token: 0x060039D0 RID: 14800 RVA: 0x000023F4 File Offset: 0x000005F4
		public virtual void CallBack()
		{
		}

		// Token: 0x060039D1 RID: 14801 RVA: 0x000023F4 File Offset: 0x000005F4
		public void OnTaken()
		{
		}

		// Token: 0x060039D2 RID: 14802 RVA: 0x00116476 File Offset: 0x00114676
		public void OnReturned()
		{
			this.target = default(U);
		}

		// Token: 0x04003EE9 RID: 16105
		public U target;
	}

	// Token: 0x02000950 RID: 2384
	private class TickCallbackWrapperPre : TickSystem<T>.TickCallbackWrapper<ITickSystemPre>
	{
		// Token: 0x060039D4 RID: 14804 RVA: 0x00116484 File Offset: 0x00114684
		public override void CallBack()
		{
			this.target.PreTick();
		}
	}

	// Token: 0x02000951 RID: 2385
	private class TickCallbackWrapperTick : TickSystem<T>.TickCallbackWrapper<ITickSystemTick>
	{
		// Token: 0x060039D6 RID: 14806 RVA: 0x00116499 File Offset: 0x00114699
		public override void CallBack()
		{
			this.target.Tick();
		}
	}

	// Token: 0x02000952 RID: 2386
	private class TickCallbackWrapperPost : TickSystem<T>.TickCallbackWrapper<ITickSystemPost>
	{
		// Token: 0x060039D8 RID: 14808 RVA: 0x001164AE File Offset: 0x001146AE
		public override void CallBack()
		{
			this.target.PostTick();
		}
	}
}
