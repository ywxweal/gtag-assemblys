using System;
using UnityEngine;

namespace com.AnotherAxiom.SpaceFight
{
	// Token: 0x02000CAB RID: 3243
	public class SpaceFight : ArcadeGame
	{
		// Token: 0x06005032 RID: 20530 RVA: 0x0017E8A4 File Offset: 0x0017CAA4
		private void Update()
		{
			for (int i = 0; i < 2; i++)
			{
				if (base.getButtonState(i, ArcadeButtons.UP))
				{
					this.move(this.player[i], 0.15f);
					this.clamp(this.player[i]);
				}
				if (base.getButtonState(i, ArcadeButtons.RIGHT))
				{
					this.turn(this.player[i], true);
				}
				if (base.getButtonState(i, ArcadeButtons.LEFT))
				{
					this.turn(this.player[i], false);
				}
				if (this.projectilesFired[i])
				{
					this.move(this.projectile[i], 0.5f);
					if (Vector2.Distance(this.player[1 - i].localPosition, this.projectile[i].localPosition) < 0.25f)
					{
						base.PlaySound(1, 2);
						this.player[1 - i].Rotate(0f, 0f, 180f);
						this.projectilesFired[i] = false;
					}
					if (Mathf.Abs(this.projectile[i].localPosition.x) > this.tableSize.x || Mathf.Abs(this.projectile[i].localPosition.y) > this.tableSize.y)
					{
						this.projectilesFired[i] = false;
					}
				}
				if (!this.projectilesFired[i])
				{
					this.projectile[i].position = this.player[i].position;
					this.projectile[i].rotation = this.player[i].rotation;
				}
			}
		}

		// Token: 0x06005033 RID: 20531 RVA: 0x0017EA34 File Offset: 0x0017CC34
		private void clamp(Transform tr)
		{
			tr.localPosition = new Vector2(Mathf.Clamp(tr.localPosition.x, -this.tableSize.x, this.tableSize.x), Mathf.Clamp(tr.localPosition.y, -this.tableSize.y, this.tableSize.y));
		}

		// Token: 0x06005034 RID: 20532 RVA: 0x0017EA9F File Offset: 0x0017CC9F
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
			if (button == ArcadeButtons.TRIGGER)
			{
				if (!this.projectilesFired[player])
				{
					base.PlaySound(0, 3);
				}
				this.projectilesFired[player] = true;
			}
		}

		// Token: 0x06005035 RID: 20533 RVA: 0x0017EAC4 File Offset: 0x0017CCC4
		private void move(Transform p, float speed)
		{
			p.Translate(p.up * Time.deltaTime * speed, Space.World);
		}

		// Token: 0x06005036 RID: 20534 RVA: 0x0017EAE3 File Offset: 0x0017CCE3
		private void turn(Transform p, bool cw)
		{
			p.Rotate(0f, 0f, (float)(cw ? 180 : (-180)) * Time.deltaTime);
		}

		// Token: 0x06005037 RID: 20535 RVA: 0x0017EB0C File Offset: 0x0017CD0C
		public override byte[] GetNetworkState()
		{
			this.netStateCur.P1LocX = this.player[0].localPosition.x;
			this.netStateCur.P1LocY = this.player[0].localPosition.y;
			this.netStateCur.P1Rot = this.player[0].localRotation.eulerAngles.z;
			this.netStateCur.P2LocX = this.player[1].localPosition.x;
			this.netStateCur.P2LocY = this.player[1].localPosition.y;
			this.netStateCur.P2Rot = this.player[1].localRotation.eulerAngles.z;
			this.netStateCur.P1PrLocX = this.projectile[0].localPosition.x;
			this.netStateCur.P1PrLocY = this.projectile[0].localPosition.y;
			this.netStateCur.P2PrLocX = this.projectile[1].localPosition.x;
			this.netStateCur.P2PrLocY = this.projectile[1].localPosition.y;
			if (!this.netStateCur.Equals(this.netStateLast))
			{
				this.netStateLast = this.netStateCur;
				base.SwapNetStateBuffersAndStreams();
				ArcadeGame.WrapNetState(this.netStateLast, this.netStateMemStream);
			}
			return this.netStateBuffer;
		}

		// Token: 0x06005038 RID: 20536 RVA: 0x0017EC8C File Offset: 0x0017CE8C
		public override void SetNetworkState(byte[] b)
		{
			SpaceFight.SpaceFlightNetState spaceFlightNetState = (SpaceFight.SpaceFlightNetState)ArcadeGame.UnwrapNetState(b);
			this.player[0].localPosition = new Vector2(spaceFlightNetState.P1LocX, spaceFlightNetState.P1LocY);
			this.player[0].localRotation = Quaternion.Euler(0f, 0f, spaceFlightNetState.P1Rot);
			this.player[1].localPosition = new Vector2(spaceFlightNetState.P2LocX, spaceFlightNetState.P2LocY);
			this.player[1].localRotation = Quaternion.Euler(0f, 0f, spaceFlightNetState.P2Rot);
			this.projectile[0].localPosition = new Vector2(spaceFlightNetState.P1PrLocX, spaceFlightNetState.P1PrLocY);
			this.projectile[1].localPosition = new Vector2(spaceFlightNetState.P2PrLocX, spaceFlightNetState.P2PrLocY);
		}

		// Token: 0x06005039 RID: 20537 RVA: 0x000023F4 File Offset: 0x000005F4
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
		}

		// Token: 0x0600503A RID: 20538 RVA: 0x000023F4 File Offset: 0x000005F4
		public override void OnTimeout()
		{
		}

		// Token: 0x0400533C RID: 21308
		[SerializeField]
		private Transform[] player;

		// Token: 0x0400533D RID: 21309
		[SerializeField]
		private Transform[] projectile;

		// Token: 0x0400533E RID: 21310
		[SerializeField]
		private Vector2 tableSize;

		// Token: 0x0400533F RID: 21311
		private bool[] projectilesFired = new bool[2];

		// Token: 0x04005340 RID: 21312
		private SpaceFight.SpaceFlightNetState netStateLast;

		// Token: 0x04005341 RID: 21313
		private SpaceFight.SpaceFlightNetState netStateCur;

		// Token: 0x02000CAC RID: 3244
		[Serializable]
		private struct SpaceFlightNetState : IEquatable<SpaceFight.SpaceFlightNetState>
		{
			// Token: 0x0600503C RID: 20540 RVA: 0x0017ED8C File Offset: 0x0017CF8C
			public bool Equals(SpaceFight.SpaceFlightNetState other)
			{
				return this.P1LocX.Approx(other.P1LocX, 1E-06f) && this.P1LocY.Approx(other.P1LocY, 1E-06f) && this.P1Rot.Approx(other.P1Rot, 1E-06f) && this.P2LocX.Approx(other.P2LocX, 1E-06f) && this.P2LocY.Approx(other.P2LocY, 1E-06f) && this.P1Rot.Approx(other.P1Rot, 1E-06f) && this.P1PrLocX.Approx(other.P1PrLocX, 1E-06f) && this.P1PrLocY.Approx(other.P1PrLocY, 1E-06f) && this.P2PrLocX.Approx(other.P2PrLocX, 1E-06f) && this.P2PrLocY.Approx(other.P2PrLocY, 1E-06f);
			}

			// Token: 0x04005342 RID: 21314
			public float P1LocX;

			// Token: 0x04005343 RID: 21315
			public float P1LocY;

			// Token: 0x04005344 RID: 21316
			public float P1Rot;

			// Token: 0x04005345 RID: 21317
			public float P2LocX;

			// Token: 0x04005346 RID: 21318
			public float P2LocY;

			// Token: 0x04005347 RID: 21319
			public float P2Rot;

			// Token: 0x04005348 RID: 21320
			public float P1PrLocX;

			// Token: 0x04005349 RID: 21321
			public float P1PrLocY;

			// Token: 0x0400534A RID: 21322
			public float P2PrLocX;

			// Token: 0x0400534B RID: 21323
			public float P2PrLocY;
		}
	}
}
