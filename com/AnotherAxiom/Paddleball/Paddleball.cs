using System;
using GorillaExtensions;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace com.AnotherAxiom.Paddleball
{
	// Token: 0x02000CAD RID: 3245
	public class Paddleball : ArcadeGame
	{
		// Token: 0x0600503E RID: 20542 RVA: 0x0017EF76 File Offset: 0x0017D176
		protected override void Awake()
		{
			base.Awake();
			this.yPosToByteFactor = 255f / (2f * this.tableSizeBall.y);
			this.byteToYPosFactor = 1f / this.yPosToByteFactor;
		}

		// Token: 0x0600503F RID: 20543 RVA: 0x0017EFB0 File Offset: 0x0017D1B0
		private void Start()
		{
			this.whiteWinScreen.SetActive(false);
			this.blackWinScreen.SetActive(false);
			this.titleScreen.SetActive(true);
			this.ball.gameObject.SetActive(false);
			this.currentScreenMode = Paddleball.ScreenMode.Title;
			this.paddleIdle = new float[this.p.Length];
			for (int i = 0; i < this.p.Length; i++)
			{
				this.p[i].gameObject.SetActive(false);
				this.paddleIdle[i] = 30f;
			}
			this.gameBallSpeed = this.initialBallSpeed;
			this.scoreR = (this.scoreL = 0);
			this.scoreFormat = this.scoreDisplay.text;
			this.UpdateScore();
		}

		// Token: 0x06005040 RID: 20544 RVA: 0x0017F074 File Offset: 0x0017D274
		private void Update()
		{
			if (this.currentScreenMode == Paddleball.ScreenMode.Gameplay)
			{
				this.ball.Translate(this.ballTrajectory.normalized * Time.deltaTime * this.gameBallSpeed);
				if (this.ball.localPosition.y > this.tableSizeBall.y)
				{
					this.ball.localPosition = new Vector3(this.ball.localPosition.x, this.tableSizeBall.y, this.ball.localPosition.z);
					this.ballTrajectory.y = -this.ballTrajectory.y;
					base.PlaySound(0, 3);
				}
				if (this.ball.localPosition.y < -this.tableSizeBall.y)
				{
					this.ball.localPosition = new Vector3(this.ball.localPosition.x, -this.tableSizeBall.y, this.ball.localPosition.z);
					this.ballTrajectory.y = -this.ballTrajectory.y;
					base.PlaySound(0, 3);
				}
				if (this.ball.localPosition.x > this.tableSizeBall.x)
				{
					this.ball.localPosition = new Vector3(this.tableSizeBall.x, this.ball.localPosition.y, this.ball.localPosition.z);
					this.ballTrajectory.x = -this.ballTrajectory.x;
					this.gameBallSpeed = this.initialBallSpeed;
					this.scoreL++;
					this.UpdateScore();
					base.PlaySound(2, 3);
					if (this.scoreL >= 10)
					{
						this.ChangeScreen(Paddleball.ScreenMode.WhiteWin);
					}
				}
				if (this.ball.localPosition.x < -this.tableSizeBall.x)
				{
					this.ball.localPosition = new Vector3(-this.tableSizeBall.x, this.ball.localPosition.y, this.ball.localPosition.z);
					this.ballTrajectory.x = -this.ballTrajectory.x;
					this.gameBallSpeed = this.initialBallSpeed;
					this.scoreR++;
					this.UpdateScore();
					base.PlaySound(2, 3);
					if (this.scoreR >= 10)
					{
						this.ChangeScreen(Paddleball.ScreenMode.BlackWin);
					}
				}
			}
			if (this.returnToTitleAfterTimestamp != 0f && Time.time > this.returnToTitleAfterTimestamp)
			{
				this.ChangeScreen(Paddleball.ScreenMode.Title);
			}
			for (int i = 0; i < this.p.Length; i++)
			{
				if (base.IsPlayerLocallyControlled(i))
				{
					float num = this.requestedPos[i];
					if (base.getButtonState(i, ArcadeButtons.UP))
					{
						this.requestedPos[i] += Time.deltaTime * this.paddleSpeed;
					}
					else if (base.getButtonState(i, ArcadeButtons.DOWN))
					{
						this.requestedPos[i] -= Time.deltaTime * this.paddleSpeed;
					}
					this.requestedPos[i] = Mathf.Clamp(this.requestedPos[i], -this.tableSizePaddle.y, this.tableSizePaddle.y);
				}
				float num2;
				if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
				{
					num2 = Mathf.MoveTowards(this.p[i].transform.localPosition.y, this.requestedPos[i], Time.deltaTime * this.paddleSpeed);
				}
				else
				{
					num2 = Mathf.MoveTowards(this.p[i].transform.localPosition.y, this.officialPos[i], Time.deltaTime * this.paddleSpeed);
				}
				this.p[i].transform.localPosition = this.p[i].transform.localPosition.WithY(Mathf.Clamp(num2, -this.tableSizePaddle.y, this.tableSizePaddle.y));
				if (base.getButtonState(i, ArcadeButtons.GRAB))
				{
					this.paddleIdle[i] = 0f;
					Paddleball.ScreenMode screenMode = this.currentScreenMode;
					if (screenMode != Paddleball.ScreenMode.Title)
					{
						if (screenMode == Paddleball.ScreenMode.Gameplay)
						{
							this.returnToTitleAfterTimestamp = Time.time + 30f;
						}
					}
					else
					{
						this.ChangeScreen(Paddleball.ScreenMode.Gameplay);
					}
				}
				else
				{
					this.paddleIdle[i] += Time.deltaTime;
				}
				bool flag = this.paddleIdle[i] < 30f;
				if (this.p[i].gameObject.activeSelf != flag)
				{
					if (flag)
					{
						base.PlaySound(4, 3);
						Vector3 localPosition = this.p[i].transform.localPosition;
						localPosition.y = 0f;
						this.requestedPos[i] = localPosition.y;
						this.p[i].transform.localPosition = localPosition;
					}
					this.p[i].gameObject.SetActive(this.paddleIdle[i] < 30f);
				}
				if (this.p[i].gameObject.activeInHierarchy && Mathf.Abs(this.ball.localPosition.x - this.p[i].transform.localPosition.x) < 0.1f && Mathf.Abs(this.ball.localPosition.y - this.p[i].transform.localPosition.y) < 0.5f)
				{
					this.ballTrajectory.y = (this.ball.localPosition.y - this.p[i].transform.localPosition.y) * 1.25f;
					float x = this.ballTrajectory.x;
					if (this.p[i].Right)
					{
						this.ballTrajectory.x = Mathf.Abs(this.ballTrajectory.y) - 1f;
					}
					else
					{
						this.ballTrajectory.x = 1f - Mathf.Abs(this.ballTrajectory.y);
					}
					if (x > 0f != this.ballTrajectory.x > 0f)
					{
						base.PlaySound(1, 3);
					}
					this.ballTrajectory.Normalize();
					this.gameBallSpeed += this.ballSpeedBoost;
				}
			}
		}

		// Token: 0x06005041 RID: 20545 RVA: 0x0017F6DC File Offset: 0x0017D8DC
		private void UpdateScore()
		{
			if (this.scoreFormat == null)
			{
				return;
			}
			this.scoreL = Mathf.Clamp(this.scoreL, 0, 10);
			this.scoreR = Mathf.Clamp(this.scoreR, 0, 10);
			this.scoreDisplay.text = string.Format(this.scoreFormat, this.scoreL, this.scoreR);
		}

		// Token: 0x06005042 RID: 20546 RVA: 0x0017F746 File Offset: 0x0017D946
		private float ByteToYPos(byte Y)
		{
			return (float)Y / this.yPosToByteFactor - this.tableSizeBall.y;
		}

		// Token: 0x06005043 RID: 20547 RVA: 0x0017F75D File Offset: 0x0017D95D
		private byte YPosToByte(float Y)
		{
			return (byte)Mathf.RoundToInt((Y + this.tableSizeBall.y) * this.yPosToByteFactor);
		}

		// Token: 0x06005044 RID: 20548 RVA: 0x0017F77C File Offset: 0x0017D97C
		public override byte[] GetNetworkState()
		{
			this.netStateCur.P0LocY = this.YPosToByte(this.p[0].transform.localPosition.y);
			this.netStateCur.P1LocY = this.YPosToByte(this.p[1].transform.localPosition.y);
			this.netStateCur.P2LocY = this.YPosToByte(this.p[2].transform.localPosition.y);
			this.netStateCur.P3LocY = this.YPosToByte(this.p[3].transform.localPosition.y);
			this.netStateCur.BallLocX = this.ball.localPosition.x;
			this.netStateCur.BallLocY = this.YPosToByte(this.ball.localPosition.y);
			this.netStateCur.BallTrajectoryX = (byte)((this.ballTrajectory.x + 1f) * 127.5f);
			this.netStateCur.BallTrajectoryY = (byte)((this.ballTrajectory.y + 1f) * 127.5f);
			this.netStateCur.BallSpeed = this.gameBallSpeed;
			this.netStateCur.ScoreLeft = this.scoreL;
			this.netStateCur.ScoreRight = this.scoreR;
			this.netStateCur.ScreenMode = (int)this.currentScreenMode;
			if (!this.netStateCur.Equals(this.netStateLast))
			{
				this.netStateLast = this.netStateCur;
				base.SwapNetStateBuffersAndStreams();
				ArcadeGame.WrapNetState(this.netStateLast, this.netStateMemStream);
			}
			return this.netStateBuffer;
		}

		// Token: 0x06005045 RID: 20549 RVA: 0x0017F930 File Offset: 0x0017DB30
		public override void SetNetworkState(byte[] b)
		{
			Paddleball.PaddleballNetState paddleballNetState = (Paddleball.PaddleballNetState)ArcadeGame.UnwrapNetState(b);
			this.officialPos[0] = this.ByteToYPos(paddleballNetState.P0LocY);
			this.officialPos[1] = this.ByteToYPos(paddleballNetState.P1LocY);
			this.officialPos[2] = this.ByteToYPos(paddleballNetState.P2LocY);
			this.officialPos[3] = this.ByteToYPos(paddleballNetState.P3LocY);
			Vector2 vector = new Vector2(paddleballNetState.BallLocX, this.ByteToYPos(paddleballNetState.BallLocY));
			Vector2 normalized = new Vector2((float)paddleballNetState.BallTrajectoryX * 0.007843138f - 1f, (float)paddleballNetState.BallTrajectoryY * 0.007843138f - 1f).normalized;
			Vector2 vector2 = vector - normalized * Vector2.Dot(vector, normalized);
			Vector2 vector3 = this.ball.localPosition.xy();
			Vector2 vector4 = vector3 - this.ballTrajectory * Vector2.Dot(vector3, this.ballTrajectory);
			if ((vector2 - vector4).IsLongerThan(0.1f))
			{
				this.ball.localPosition = vector;
				this.ballTrajectory = normalized.xy();
			}
			this.gameBallSpeed = paddleballNetState.BallSpeed;
			this.ChangeScreen((Paddleball.ScreenMode)paddleballNetState.ScreenMode);
			if (this.scoreL != paddleballNetState.ScoreLeft || this.scoreR != paddleballNetState.ScoreRight)
			{
				this.scoreL = paddleballNetState.ScoreLeft;
				this.scoreR = paddleballNetState.ScoreRight;
				this.UpdateScore();
			}
		}

		// Token: 0x06005046 RID: 20550 RVA: 0x000023F4 File Offset: 0x000005F4
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
		}

		// Token: 0x06005047 RID: 20551 RVA: 0x000023F4 File Offset: 0x000005F4
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
		}

		// Token: 0x06005048 RID: 20552 RVA: 0x0017FAAC File Offset: 0x0017DCAC
		private void ChangeScreen(Paddleball.ScreenMode mode)
		{
			if (this.currentScreenMode == mode)
			{
				return;
			}
			switch (this.currentScreenMode)
			{
			case Paddleball.ScreenMode.Title:
				this.titleScreen.SetActive(false);
				break;
			case Paddleball.ScreenMode.Gameplay:
				this.ball.gameObject.SetActive(false);
				break;
			case Paddleball.ScreenMode.WhiteWin:
				this.whiteWinScreen.SetActive(false);
				break;
			case Paddleball.ScreenMode.BlackWin:
				this.blackWinScreen.SetActive(false);
				break;
			}
			this.currentScreenMode = mode;
			switch (mode)
			{
			case Paddleball.ScreenMode.Title:
				this.gameBallSpeed = this.initialBallSpeed;
				this.scoreL = 0;
				this.scoreR = 0;
				this.UpdateScore();
				this.returnToTitleAfterTimestamp = 0f;
				this.titleScreen.SetActive(true);
				return;
			case Paddleball.ScreenMode.Gameplay:
				this.ball.gameObject.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + 30f;
				return;
			case Paddleball.ScreenMode.WhiteWin:
				this.whiteWinScreen.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + this.winScreenDuration;
				base.PlaySound(3, 3);
				return;
			case Paddleball.ScreenMode.BlackWin:
				this.blackWinScreen.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + this.winScreenDuration;
				base.PlaySound(3, 3);
				return;
			default:
				return;
			}
		}

		// Token: 0x06005049 RID: 20553 RVA: 0x0017FBE3 File Offset: 0x0017DDE3
		public override void OnTimeout()
		{
			this.ChangeScreen(Paddleball.ScreenMode.Title);
		}

		// Token: 0x0600504A RID: 20554 RVA: 0x0017FBEC File Offset: 0x0017DDEC
		public override void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
		{
			this.requestedPos[player] = this.ByteToYPos((byte)stream.ReceiveNext());
		}

		// Token: 0x0600504B RID: 20555 RVA: 0x0017FC07 File Offset: 0x0017DE07
		public override void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext(this.YPosToByte(this.requestedPos[player]));
		}

		// Token: 0x0400534D RID: 21325
		[SerializeField]
		private PaddleballPaddle[] p;

		// Token: 0x0400534E RID: 21326
		private float[] requestedPos = new float[4];

		// Token: 0x0400534F RID: 21327
		private float[] officialPos = new float[4];

		// Token: 0x04005350 RID: 21328
		[SerializeField]
		private Transform ball;

		// Token: 0x04005351 RID: 21329
		[SerializeField]
		private Vector2 ballTrajectory;

		// Token: 0x04005352 RID: 21330
		[SerializeField]
		private float paddleSpeed = 1f;

		// Token: 0x04005353 RID: 21331
		[SerializeField]
		private float initialBallSpeed = 1f;

		// Token: 0x04005354 RID: 21332
		[SerializeField]
		private float ballSpeedBoost = 0.02f;

		// Token: 0x04005355 RID: 21333
		private float gameBallSpeed = 1f;

		// Token: 0x04005356 RID: 21334
		[SerializeField]
		private Vector2 tableSizeBall;

		// Token: 0x04005357 RID: 21335
		[SerializeField]
		private Vector2 tableSizePaddle;

		// Token: 0x04005358 RID: 21336
		[SerializeField]
		private GameObject blackWinScreen;

		// Token: 0x04005359 RID: 21337
		[SerializeField]
		private GameObject whiteWinScreen;

		// Token: 0x0400535A RID: 21338
		[SerializeField]
		private GameObject titleScreen;

		// Token: 0x0400535B RID: 21339
		[SerializeField]
		private float winScreenDuration;

		// Token: 0x0400535C RID: 21340
		private float returnToTitleAfterTimestamp;

		// Token: 0x0400535D RID: 21341
		private int scoreL;

		// Token: 0x0400535E RID: 21342
		private int scoreR;

		// Token: 0x0400535F RID: 21343
		private string scoreFormat;

		// Token: 0x04005360 RID: 21344
		[SerializeField]
		private TMP_Text scoreDisplay;

		// Token: 0x04005361 RID: 21345
		private float[] paddleIdle;

		// Token: 0x04005362 RID: 21346
		private Paddleball.ScreenMode currentScreenMode;

		// Token: 0x04005363 RID: 21347
		private const int AUDIO_WALLBOUNCE = 0;

		// Token: 0x04005364 RID: 21348
		private const int AUDIO_PADDLEBOUNCE = 1;

		// Token: 0x04005365 RID: 21349
		private const int AUDIO_SCORE = 2;

		// Token: 0x04005366 RID: 21350
		private const int AUDIO_WIN = 3;

		// Token: 0x04005367 RID: 21351
		private const int AUDIO_PLAYERJOIN = 4;

		// Token: 0x04005368 RID: 21352
		private const int VAR_REQUESTEDPOS = 0;

		// Token: 0x04005369 RID: 21353
		private const int MAXSCORE = 10;

		// Token: 0x0400536A RID: 21354
		private float yPosToByteFactor;

		// Token: 0x0400536B RID: 21355
		private float byteToYPosFactor;

		// Token: 0x0400536C RID: 21356
		private const float directionToByteFactor = 127.5f;

		// Token: 0x0400536D RID: 21357
		private const float byteToDirectionFactor = 0.007843138f;

		// Token: 0x0400536E RID: 21358
		private Paddleball.PaddleballNetState netStateLast;

		// Token: 0x0400536F RID: 21359
		private Paddleball.PaddleballNetState netStateCur;

		// Token: 0x02000CAE RID: 3246
		private enum ScreenMode
		{
			// Token: 0x04005371 RID: 21361
			Title,
			// Token: 0x04005372 RID: 21362
			Gameplay,
			// Token: 0x04005373 RID: 21363
			WhiteWin,
			// Token: 0x04005374 RID: 21364
			BlackWin
		}

		// Token: 0x02000CAF RID: 3247
		[Serializable]
		private struct PaddleballNetState : IEquatable<Paddleball.PaddleballNetState>
		{
			// Token: 0x0600504D RID: 20557 RVA: 0x0017FC7C File Offset: 0x0017DE7C
			public bool Equals(Paddleball.PaddleballNetState other)
			{
				return this.P0LocY == other.P0LocY && this.P1LocY == other.P1LocY && this.P2LocY == other.P2LocY && this.P3LocY == other.P3LocY && this.BallLocX.Approx(other.BallLocX, 1E-06f) && this.BallLocY == other.BallLocY && this.BallTrajectoryX == other.BallTrajectoryX && this.BallTrajectoryY == other.BallTrajectoryY && this.BallSpeed.Approx(other.BallSpeed, 1E-06f) && this.ScoreLeft == other.ScoreLeft && this.ScoreRight == other.ScoreRight && this.ScreenMode == other.ScreenMode;
			}

			// Token: 0x04005375 RID: 21365
			public byte P0LocY;

			// Token: 0x04005376 RID: 21366
			public byte P1LocY;

			// Token: 0x04005377 RID: 21367
			public byte P2LocY;

			// Token: 0x04005378 RID: 21368
			public byte P3LocY;

			// Token: 0x04005379 RID: 21369
			public float BallLocX;

			// Token: 0x0400537A RID: 21370
			public byte BallLocY;

			// Token: 0x0400537B RID: 21371
			public byte BallTrajectoryX;

			// Token: 0x0400537C RID: 21372
			public byte BallTrajectoryY;

			// Token: 0x0400537D RID: 21373
			public float BallSpeed;

			// Token: 0x0400537E RID: 21374
			public int ScoreLeft;

			// Token: 0x0400537F RID: 21375
			public int ScoreRight;

			// Token: 0x04005380 RID: 21376
			public int ScreenMode;
		}
	}
}
