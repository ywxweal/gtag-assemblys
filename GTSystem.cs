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
	// (get) Token: 0x06002990 RID: 10640 RVA: 0x000CE512 File Offset: 0x000CC712
	public PhotonView photonView
	{
		get
		{
			return this._photonView;
		}
	}

	// Token: 0x06002991 RID: 10641 RVA: 0x000CE51A File Offset: 0x000CC71A
	protected virtual void Awake()
	{
		GTSystem<T>.SetSingleton(this);
	}

	// Token: 0x06002992 RID: 10642 RVA: 0x000CE524 File Offset: 0x000CC724
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

	// Token: 0x06002993 RID: 10643 RVA: 0x000CE56F File Offset: 0x000CC76F
	protected virtual void OnApplicationQuit()
	{
		GTSystem<T>.gAppQuitting = true;
	}

	// Token: 0x06002994 RID: 10644 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnTick(float dt, T instance)
	{
	}

	// Token: 0x06002995 RID: 10645 RVA: 0x000CE578 File Offset: 0x000CC778
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

	// Token: 0x06002996 RID: 10646 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnRegister(T instance)
	{
	}

	// Token: 0x06002997 RID: 10647 RVA: 0x000CE5DC File Offset: 0x000CC7DC
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

	// Token: 0x06002998 RID: 10648 RVA: 0x000023F4 File Offset: 0x000005F4
	protected virtual void OnUnregister(T instance)
	{
	}

	// Token: 0x06002999 RID: 10649 RVA: 0x000CE63E File Offset: 0x000CC83E
	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return ((IEnumerable<T>)this._instances).GetEnumerator();
	}

	// Token: 0x0600299A RID: 10650 RVA: 0x000CE63E File Offset: 0x000CC83E
	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable<T>)this._instances).GetEnumerator();
	}

	// Token: 0x17000402 RID: 1026
	// (get) Token: 0x0600299B RID: 10651 RVA: 0x000CE64B File Offset: 0x000CC84B
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
	// (get) Token: 0x0600299D RID: 10653 RVA: 0x000CE666 File Offset: 0x000CC866
	public static PhotonView PhotonView
	{
		get
		{
			return GTSystem<T>.gSingleton._photonView;
		}
	}

	// Token: 0x0600299E RID: 10654 RVA: 0x000CE674 File Offset: 0x000CC874
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

	// Token: 0x0600299F RID: 10655 RVA: 0x000CE764 File Offset: 0x000CC964
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

	// Token: 0x060029A0 RID: 10656 RVA: 0x000CE7D0 File Offset: 0x000CC9D0
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

	// Token: 0x04002EA2 RID: 11938
	[SerializeField]
	protected List<T> _instances = new List<T>();

	// Token: 0x04002EA3 RID: 11939
	[SerializeField]
	private bool _networked;

	// Token: 0x04002EA4 RID: 11940
	[SerializeField]
	private PhotonView _photonView;

	// Token: 0x04002EA5 RID: 11941
	private static GTSystem<T> gSingleton;

	// Token: 0x04002EA6 RID: 11942
	private static bool gInitializing = false;

	// Token: 0x04002EA7 RID: 11943
	private static bool gAppQuitting = false;

	// Token: 0x04002EA8 RID: 11944
	private static HashSet<T> gQueueRegister = new HashSet<T>();
}
