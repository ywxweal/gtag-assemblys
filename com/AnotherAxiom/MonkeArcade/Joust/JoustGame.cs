using System;
using UnityEngine;

namespace com.AnotherAxiom.MonkeArcade.Joust
{
	// Token: 0x02000CB1 RID: 3249
	public class JoustGame : ArcadeGame
	{
		// Token: 0x06005050 RID: 20560 RVA: 0x0017FD66 File Offset: 0x0017DF66
		public override byte[] GetNetworkState()
		{
			return new byte[0];
		}

		// Token: 0x06005051 RID: 20561 RVA: 0x000023F4 File Offset: 0x000005F4
		public override void SetNetworkState(byte[] obj)
		{
		}

		// Token: 0x06005052 RID: 20562 RVA: 0x0017FD6E File Offset: 0x0017DF6E
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
			if (button != ArcadeButtons.GRAB)
			{
				if (button == ArcadeButtons.TRIGGER)
				{
					this.joustPlayers[player].Flap();
					return;
				}
			}
			else
			{
				this.joustPlayers[player].gameObject.SetActive(true);
			}
		}

		// Token: 0x06005053 RID: 20563 RVA: 0x0017FD9D File Offset: 0x0017DF9D
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
			if (button == ArcadeButtons.GRAB)
			{
				this.joustPlayers[player].gameObject.SetActive(false);
			}
		}

		// Token: 0x06005054 RID: 20564 RVA: 0x0017FDB8 File Offset: 0x0017DFB8
		private void Start()
		{
			for (int i = 0; i < this.joustPlayers.Length; i++)
			{
				this.joustPlayers[i].gameObject.SetActive(false);
			}
		}

		// Token: 0x06005055 RID: 20565 RVA: 0x0017FDEC File Offset: 0x0017DFEC
		private void Update()
		{
			for (int i = 0; i < this.joustPlayers.Length; i++)
			{
				if (this.joustPlayers[i].gameObject.activeInHierarchy)
				{
					int num = (base.getButtonState(i, ArcadeButtons.LEFT) ? (-1) : 0) + (base.getButtonState(i, ArcadeButtons.RIGHT) ? 1 : 0);
					this.joustPlayers[i].HorizontalSpeed = Mathf.Clamp(this.joustPlayers[i].HorizontalSpeed + (float)num * Time.deltaTime, -1f, 1f);
				}
			}
		}

		// Token: 0x06005056 RID: 20566 RVA: 0x000023F4 File Offset: 0x000005F4
		public override void OnTimeout()
		{
		}

		// Token: 0x04005382 RID: 21378
		[SerializeField]
		private JoustPlayer[] joustPlayers;
	}
}
