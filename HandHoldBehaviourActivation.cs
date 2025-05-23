using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000253 RID: 595
public class HandHoldBehaviourActivation : Tappable
{
	// Token: 0x06000D87 RID: 3463 RVA: 0x00046360 File Offset: 0x00044560
	protected override void OnEnable()
	{
		base.OnEnable();
		RoomSystem.PlayerLeftEvent = (Action<NetPlayer>)Delegate.Combine(RoomSystem.PlayerLeftEvent, new Action<NetPlayer>(this.OnPlayerLeftRoom));
		RoomSystem.LeftRoomEvent = (Action)Delegate.Combine(RoomSystem.LeftRoomEvent, new Action(this.OnLeftRoom));
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x000463B4 File Offset: 0x000445B4
	public override void OnGrabLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
		byte b = this.m_playerGrabCounts.GetValueOrDefault(sender.Sender.ActorNumber, 0);
		b += 1;
		if (b > 2)
		{
			return;
		}
		this.m_playerGrabCounts[sender.Sender.ActorNumber] = b;
		this.grabs++;
		if (this.grabs < 2)
		{
			this.ActivationStart.Invoke();
		}
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x00046420 File Offset: 0x00044620
	public override void OnReleaseLocal(float tapTime, PhotonMessageInfoWrapped sender)
	{
		byte b;
		if (!this.m_playerGrabCounts.TryGetValue(sender.Sender.ActorNumber, out b) || b < 1)
		{
			return;
		}
		b -= 1;
		this.m_playerGrabCounts[sender.Sender.ActorNumber] = b;
		bool flag = this.grabs > 0;
		this.grabs = Mathf.Max(0, this.grabs - 1);
		if (flag && this.grabs < 1)
		{
			this.ActivationStop.Invoke();
		}
	}

	// Token: 0x06000D8A RID: 3466 RVA: 0x000464A0 File Offset: 0x000446A0
	private void OnPlayerLeftRoom(NetPlayer player)
	{
		byte b;
		if (!this.m_playerGrabCounts.TryGetValue(player.ActorNumber, out b))
		{
			return;
		}
		bool flag = this.grabs > 0;
		this.grabs = Mathf.Max(0, this.grabs - (int)b);
		this.m_playerGrabCounts.Remove(player.ActorNumber);
		if (flag && this.grabs < 1)
		{
			this.ActivationStop.Invoke();
		}
	}

	// Token: 0x06000D8B RID: 3467 RVA: 0x00046508 File Offset: 0x00044708
	private void OnLeftRoom()
	{
		byte valueOrDefault = this.m_playerGrabCounts.GetValueOrDefault(NetworkSystem.Instance.LocalPlayer.ActorNumber, 0);
		if (this.grabs > 0 && valueOrDefault < 1)
		{
			this.ActivationStop.Invoke();
		}
		this.grabs = (int)valueOrDefault;
		this.m_playerGrabCounts.Clear();
		this.m_playerGrabCounts[NetworkSystem.Instance.LocalPlayer.ActorNumber] = valueOrDefault;
	}

	// Token: 0x04001119 RID: 4377
	[SerializeField]
	private UnityEvent ActivationStart;

	// Token: 0x0400111A RID: 4378
	[SerializeField]
	private UnityEvent ActivationStop;

	// Token: 0x0400111B RID: 4379
	private int grabs;

	// Token: 0x0400111C RID: 4380
	private readonly Dictionary<int, byte> m_playerGrabCounts = new Dictionary<int, byte>(10);
}
