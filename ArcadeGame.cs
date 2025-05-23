using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200026D RID: 621
public abstract class ArcadeGame : MonoBehaviour
{
	// Token: 0x06000E3E RID: 3646 RVA: 0x0004897F File Offset: 0x00046B7F
	protected virtual void Awake()
	{
		this.InitializeMemoryStreams();
	}

	// Token: 0x06000E3F RID: 3647 RVA: 0x00048987 File Offset: 0x00046B87
	public void InitializeMemoryStreams()
	{
		if (!this.memoryStreamsInitialized)
		{
			this.netStateMemStream = new MemoryStream(this.netStateBuffer, true);
			this.netStateMemStreamAlt = new MemoryStream(this.netStateBufferAlt, true);
			this.memoryStreamsInitialized = true;
		}
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x000489BC File Offset: 0x00046BBC
	public void SetMachine(ArcadeMachine machine)
	{
		this.machine = machine;
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x000489C5 File Offset: 0x00046BC5
	protected bool getButtonState(int player, ArcadeButtons button)
	{
		return this.playerInputs[player].HasFlag(button);
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x000489E0 File Offset: 0x00046BE0
	public void OnInputStateChange(int player, ArcadeButtons buttons)
	{
		for (int i = 1; i < 256; i += i)
		{
			ArcadeButtons arcadeButtons = (ArcadeButtons)i;
			bool flag = buttons.HasFlag(arcadeButtons);
			bool flag2 = this.playerInputs[player].HasFlag(arcadeButtons);
			if (flag != flag2)
			{
				if (flag)
				{
					this.ButtonDown(player, arcadeButtons);
				}
				else
				{
					this.ButtonUp(player, arcadeButtons);
				}
			}
		}
		this.playerInputs[player] = buttons;
	}

	// Token: 0x06000E43 RID: 3651
	public abstract byte[] GetNetworkState();

	// Token: 0x06000E44 RID: 3652
	public abstract void SetNetworkState(byte[] obj);

	// Token: 0x06000E45 RID: 3653 RVA: 0x00048A4C File Offset: 0x00046C4C
	protected static void WrapNetState(object ns, MemoryStream stream)
	{
		if (stream == null)
		{
			Debug.LogWarning("Null MemoryStream passed to WrapNetState");
			return;
		}
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		stream.SetLength(0L);
		stream.Position = 0L;
		binaryFormatter.Serialize(stream, ns);
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x00048A78 File Offset: 0x00046C78
	protected static object UnwrapNetState(byte[] b)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(b);
		memoryStream.Position = 0L;
		object obj = binaryFormatter.Deserialize(memoryStream);
		memoryStream.Close();
		return obj;
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x00048AB0 File Offset: 0x00046CB0
	protected void SwapNetStateBuffersAndStreams()
	{
		byte[] array = this.netStateBufferAlt;
		byte[] array2 = this.netStateBuffer;
		this.netStateBuffer = array;
		this.netStateBufferAlt = array2;
		MemoryStream memoryStream = this.netStateMemStreamAlt;
		MemoryStream memoryStream2 = this.netStateMemStream;
		this.netStateMemStream = memoryStream;
		this.netStateMemStreamAlt = memoryStream2;
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x00048AF5 File Offset: 0x00046CF5
	protected void PlaySound(int clipId, int prio = 3)
	{
		this.machine.PlaySound(clipId, prio);
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x00048B04 File Offset: 0x00046D04
	protected bool IsPlayerLocallyControlled(int player)
	{
		return this.machine.IsPlayerLocallyControlled(player);
	}

	// Token: 0x06000E4A RID: 3658
	protected abstract void ButtonUp(int player, ArcadeButtons button);

	// Token: 0x06000E4B RID: 3659
	protected abstract void ButtonDown(int player, ArcadeButtons button);

	// Token: 0x06000E4C RID: 3660
	public abstract void OnTimeout();

	// Token: 0x06000E4D RID: 3661 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x000023F4 File Offset: 0x000005F4
	public virtual void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x04001192 RID: 4498
	[SerializeField]
	public Vector2 Scale = new Vector2(1f, 1f);

	// Token: 0x04001193 RID: 4499
	private ArcadeButtons[] playerInputs = new ArcadeButtons[4];

	// Token: 0x04001194 RID: 4500
	public AudioClip[] audioClips;

	// Token: 0x04001195 RID: 4501
	private ArcadeMachine machine;

	// Token: 0x04001196 RID: 4502
	protected static int NetStateBufferSize = 512;

	// Token: 0x04001197 RID: 4503
	protected byte[] netStateBuffer = new byte[ArcadeGame.NetStateBufferSize];

	// Token: 0x04001198 RID: 4504
	protected byte[] netStateBufferAlt = new byte[ArcadeGame.NetStateBufferSize];

	// Token: 0x04001199 RID: 4505
	protected MemoryStream netStateMemStream;

	// Token: 0x0400119A RID: 4506
	protected MemoryStream netStateMemStreamAlt;

	// Token: 0x0400119B RID: 4507
	public bool memoryStreamsInitialized;
}
