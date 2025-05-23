using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000656 RID: 1622
public class GTSignalRelay : MonoBehaviourStatic<GTSignalRelay>, IOnEventCallback
{
	// Token: 0x170003DC RID: 988
	// (get) Token: 0x06002885 RID: 10373 RVA: 0x000C988E File Offset: 0x000C7A8E
	public static IReadOnlyList<GTSignalListener> ActiveListeners
	{
		get
		{
			return GTSignalRelay.gActiveListeners;
		}
	}

	// Token: 0x06002886 RID: 10374 RVA: 0x000C9895 File Offset: 0x000C7A95
	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.AddCallbackTarget(this);
		}
	}

	// Token: 0x06002887 RID: 10375 RVA: 0x000C98A4 File Offset: 0x000C7AA4
	private void OnDisable()
	{
		if (Application.isPlaying)
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x06002888 RID: 10376 RVA: 0x000C98B4 File Offset: 0x000C7AB4
	public static void Register(GTSignalListener listener)
	{
		if (listener == null)
		{
			return;
		}
		int num = listener.signal;
		if (num == 0)
		{
			return;
		}
		if (!GTSignalRelay.gListenerSet.Add(listener))
		{
			return;
		}
		GTSignalRelay.gActiveListeners.Add(listener);
		List<GTSignalListener> list;
		if (!GTSignalRelay.gSignalIdToListeners.TryGetValue(num, out list))
		{
			list = new List<GTSignalListener>(64);
			GTSignalRelay.gSignalIdToListeners.Add(num, list);
		}
		list.Add(listener);
	}

	// Token: 0x06002889 RID: 10377 RVA: 0x000C9920 File Offset: 0x000C7B20
	public static void Unregister(GTSignalListener listener)
	{
		if (listener == null)
		{
			return;
		}
		GTSignalRelay.gListenerSet.Remove(listener);
		GTSignalRelay.gActiveListeners.Remove(listener);
		List<GTSignalListener> list;
		if (GTSignalRelay.gSignalIdToListeners.TryGetValue(listener.signal, out list))
		{
			list.Remove(listener);
		}
	}

	// Token: 0x0600288A RID: 10378 RVA: 0x000C9970 File Offset: 0x000C7B70
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
		Object.DontDestroyOnLoad(new GameObject("GTSignalRelay").AddComponent<GTSignalRelay>());
	}

	// Token: 0x0600288B RID: 10379 RVA: 0x000C9988 File Offset: 0x000C7B88
	void IOnEventCallback.OnEvent(EventData eventData)
	{
		if (eventData.Code != 186)
		{
			return;
		}
		object[] array = (object[])eventData.CustomData;
		int num = (int)array[0];
		List<GTSignalListener> list;
		if (!GTSignalRelay.gSignalIdToListeners.TryGetValue(num, out list))
		{
			return;
		}
		int sender = eventData.Sender;
		for (int i = 0; i < list.Count; i++)
		{
			try
			{
				GTSignalListener gtsignalListener = list[i];
				if (!gtsignalListener.deafen)
				{
					if (gtsignalListener.IsReady())
					{
						if (!gtsignalListener.ignoreSelf || sender != gtsignalListener.rigActorID)
						{
							if (!gtsignalListener.listenToSelfOnly || sender == gtsignalListener.rigActorID)
							{
								gtsignalListener.HandleSignalReceived(sender, array);
								if (gtsignalListener.callUnityEvent)
								{
									UnityEvent onSignalReceived = gtsignalListener.onSignalReceived;
									if (onSignalReceived != null)
									{
										onSignalReceived.Invoke();
									}
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}

	// Token: 0x04002D55 RID: 11605
	private static List<GTSignalListener> gActiveListeners = new List<GTSignalListener>(128);

	// Token: 0x04002D56 RID: 11606
	private static HashSet<GTSignalListener> gListenerSet = new HashSet<GTSignalListener>(128);

	// Token: 0x04002D57 RID: 11607
	private static Dictionary<int, List<GTSignalListener>> gSignalIdToListeners = new Dictionary<int, List<GTSignalListener>>(128);
}
