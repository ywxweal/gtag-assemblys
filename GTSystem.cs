using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000682 RID: 1666
[DisallowMultipleComponent]
public abstract class GTSystem<T> : MonoBehaviour, IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> where T : MonoBehaviour
{
	// Token: 0x17000401 RID: 1025
	// (get) Token: 0x06002991 RID: 10641 RVA: 0x000CE5B6 File Offset: 0x000CC7B6
	public PhotonView photonView
	{
		get
		{
			return this._photonView;
		}
	}

	// Token: 0x06002992 RID: 10642 RVA: 0x000CE5BE File Offset: 0x000CC7BE
	protected virtual void Awake()
	{
		GTSystem<T>.SetSingleton(this);
	}

	// Token: 0x06002993 RID: 10643 RVA: 0x000CE5C8 File Offset: 0x000CC7C8
	protected virtual void Tick()
	{
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < this._instances.Count; i++)
		{
			T t = this._instances[i];
			if (t)
			{
				this.OnTick(deltaTime, t);
			}
		}
	}

	// Token: 0x06002994 RID: 10644 RVA: 0x000CE613 File Offset: 0x000CC813
	protected virtual void OnApplicationQuit()
	{
		GTSystem<T>.gAppQuitting = true;
	}

	// Token: 0x06002995 RID: 10645 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnTick(float dt, T instance)
	{
	}

	// Token: 0x06002996 RID: 10646 RVA: 0x000CE61C File Offset: 0x000CC81C
	private bool RegisterInstance(T instance)
	{
		if (instance == null)
		{
			GTDev.LogError<string>("[" + base.GetType().Name + "::Register] Instance is null.", null);
			return false;
		}
		if (this._instances.Contains(instance))
		{
			return false;
		}
		this._instances.Add(instance);
		this.OnRegister(instance);
		return true;
	}

	// Token: 0x06002997 RID: 10647 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnRegister(T instance)
	{
	}

	// Token: 0x06002998 RID: 10648 RVA: 0x000CE680 File Offset: 0x000CC880
	private bool UnregisterInstance(T instance)
	{
		if (instance == null)
		{
			GTDev.LogError<string>("[" + base.GetType().Name + "::Unregister] Instance is null.", null);
			return false;
		}
		if (!this._instances.Contains(instance))
		{
			return false;
		}
		this._instances.Remove(instance);
		this.OnUnregister(instance);
		return true;
	}

	// Token: 0x06002999 RID: 10649 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnUnregister(T instance)
	{
	}

	// Token: 0x0600299A RID: 10650 RVA: 0x000CE6E2 File Offset: 0x000CC8E2
	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return ((IEnumerable<T>)this._instances).GetEnumerator();
	}

	// Token: 0x0600299B RID: 10651 RVA: 0x000CE6E2 File Offset: 0x000CC8E2
	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<T>)this._instances).GetEnumerator();
	}

	// Token: 0x17000402 RID: 1026
	// (get) Token: 0x0600299C RID: 10652 RVA: 0x000CE6EF File Offset: 0x000CC8EF
	int IReadOnlyCollection<T>.Count
	{
		get
		{
			return this._instances.Count;
		}
	}

	// Token: 0x17000403 RID: 1027
	T IReadOnlyList<T>.this[int index]
	{
		get
		{
			return this._instances[index];
		}
	}

	// Token: 0x17000404 RID: 1028
	// (get) Token: 0x0600299E RID: 10654 RVA: 0x000CE70A File Offset: 0x000CC90A
	public static PhotonView PhotonView
	{
		get
		{
			return GTSystem<T>.gSingleton._photonView;
		}
	}

	// Token: 0x0600299F RID: 10655 RVA: 0x000CE718 File Offset: 0x000CC918
	protected static void SetSingleton(GTSystem<T> system)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (GTSystem<T>.gSingleton != null && GTSystem<T>.gSingleton != system)
		{
			Object.Destroy(system);
			GTDev.LogWarning<string>("Singleton of type " + GTSystem<T>.gSingleton.GetType().Name + " already exists.", null);
			return;
		}
		GTSystem<T>.gSingleton = system;
		if (!GTSystem<T>.gInitializing)
		{
			return;
		}
		GTSystem<T>.gSingleton._instances.Clear();
		T[] array = GTSystem<T>.gQueueRegister.Where((T x) => x != null).ToArray<T>();
		GTSystem<T>.gSingleton._instances.AddRange(array);
		GTSystem<T>.gQueueRegister.Clear();
		PhotonView component = GTSystem<T>.gSingleton.GetComponent<PhotonView>();
		if (component != null)
		{
			GTSystem<T>.gSingleton._photonView = component;
			GTSystem<T>.gSingleton._networked = true;
		}
		GTSystem<T>.gInitializing = false;
	}

	// Token: 0x060029A0 RID: 10656 RVA: 0x000CE808 File Offset: 0x000CCA08
	public static void Register(T instance)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (instance == null)
		{
			return;
		}
		if (GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gQueueRegister.Add(instance);
			return;
		}
		if (GTSystem<T>.gSingleton == null && !GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gInitializing = true;
			GTSystem<T>.gQueueRegister.Add(instance);
			return;
		}
		GTSystem<T>.gSingleton.RegisterInstance(instance);
	}

	// Token: 0x060029A1 RID: 10657 RVA: 0x000CE874 File Offset: 0x000CCA74
	public static void Unregister(T instance)
	{
		if (GTSystem<T>.gAppQuitting)
		{
			return;
		}
		if (instance == null)
		{
			return;
		}
		if (GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gQueueRegister.Remove(instance);
			return;
		}
		if (GTSystem<T>.gSingleton == null && !GTSystem<T>.gInitializing)
		{
			GTSystem<T>.gInitializing = true;
			GTSystem<T>.gQueueRegister.Remove(instance);
			return;
		}
		GTSystem<T>.gSingleton.UnregisterInstance(instance);
	}

	// Token: 0x04002EA4 RID: 11940
	[SerializeField]
	protected List<T> _instances = new List<T>();

	// Token: 0x04002EA5 RID: 11941
	[SerializeField]
	private bool _networked;

	// Token: 0x04002EA6 RID: 11942
	[SerializeField]
	private PhotonView _photonView;

	// Token: 0x04002EA7 RID: 11943
	private static GTSystem<T> gSingleton;

	// Token: 0x04002EA8 RID: 11944
	private static bool gInitializing = false;

	// Token: 0x04002EA9 RID: 11945
	private static bool gAppQuitting = false;

	// Token: 0x04002EAA RID: 11946
	private static HashSet<T> gQueueRegister = new HashSet<T>();
}
