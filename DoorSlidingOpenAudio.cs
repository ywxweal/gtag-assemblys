using System;
using UnityEngine;

// Token: 0x020000C9 RID: 201
public class DoorSlidingOpenAudio : MonoBehaviour, IBuildValidation, ITickSystemTick
{
	// Token: 0x1700005E RID: 94
	// (get) Token: 0x060004F2 RID: 1266 RVA: 0x0001C93E File Offset: 0x0001AB3E
	// (set) Token: 0x060004F3 RID: 1267 RVA: 0x0001C946 File Offset: 0x0001AB46
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x060004F4 RID: 1268 RVA: 0x0001C94F File Offset: 0x0001AB4F
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x0001C957 File Offset: 0x0001AB57
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x0001C960 File Offset: 0x0001AB60
	public bool BuildValidationCheck()
	{
		if (this.button == null)
		{
			Debug.LogError("reference button missing for doorslidingopenaudio", base.gameObject);
			return false;
		}
		if (this.audioSource == null)
		{
			Debug.LogError("missing audio source on doorslidingopenaudio", base.gameObject);
			return false;
		}
		return true;
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x0001C9B0 File Offset: 0x0001ABB0
	void ITickSystemTick.Tick()
	{
		if (this.button.ghostLab.IsDoorMoving(this.button.forSingleDoor, this.button.buttonIndex))
		{
			if (!this.audioSource.isPlaying)
			{
				this.audioSource.time = 0f;
				this.audioSource.GTPlay();
				return;
			}
		}
		else if (this.audioSource.isPlaying)
		{
			this.audioSource.time = 0f;
			this.audioSource.GTStop();
		}
	}

	// Token: 0x040005D6 RID: 1494
	public GhostLabButton button;

	// Token: 0x040005D7 RID: 1495
	public AudioSource audioSource;
}
