using System;
using System.IO;
using System.Text;

namespace LitJson
{
	// Token: 0x02000AA0 RID: 2720
	internal class Lexer
	{
		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x0600417F RID: 16767 RVA: 0x0012F122 File Offset: 0x0012D322
		// (set) Token: 0x06004180 RID: 16768 RVA: 0x0012F12A File Offset: 0x0012D32A
		public bool AllowComments
		{
			get
			{
				return this.allow_comments;
			}
			set
			{
				this.allow_comments = value;
			}
		}

		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x06004181 RID: 16769 RVA: 0x0012F133 File Offset: 0x0012D333
		// (set) Token: 0x06004182 RID: 16770 RVA: 0x0012F13B File Offset: 0x0012D33B
		public bool AllowSingleQuotedStrings
		{
			get
			{
				return this.allow_single_quoted_strings;
			}
			set
			{
				this.allow_single_quoted_strings = value;
			}
		}

		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x06004183 RID: 16771 RVA: 0x0012F144 File Offset: 0x0012D344
		public bool EndOfInput
		{
			get
			{
				return this.end_of_input;
			}
		}

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x06004184 RID: 16772 RVA: 0x0012F14C File Offset: 0x0012D34C
		public int Token
		{
			get
			{
				return this.token;
			}
		}

		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x06004185 RID: 16773 RVA: 0x0012F154 File Offset: 0x0012D354
		public string StringValue
		{
			get
			{
				return this.string_value;
			}
		}

		// Token: 0x06004186 RID: 16774 RVA: 0x0012F15C File Offset: 0x0012D35C
		static Lexer()
		{
			Lexer.PopulateFsmTables();
		}

		// Token: 0x06004187 RID: 16775 RVA: 0x0012F164 File Offset: 0x0012D364
		public Lexer(TextReader reader)
		{
			this.allow_comments = true;
			this.allow_single_quoted_strings = true;
			this.input_buffer = 0;
			this.string_buffer = new StringBuilder(128);
			this.state = 1;
			this.end_of_input = false;
			this.reader = reader;
			this.fsm_context = new FsmContext();
			this.fsm_context.L = this;
		}

		// Token: 0x06004188 RID: 16776 RVA: 0x0012F1C8 File Offset: 0x0012D3C8
		private static int HexValue(int digit)
		{
			switch (digit)
			{
			case 65:
				break;
			case 66:
				return 11;
			case 67:
				return 12;
			case 68:
				return 13;
			case 69:
				return 14;
			case 70:
				return 15;
			default:
				switch (digit)
				{
				case 97:
					break;
				case 98:
					return 11;
				case 99:
					return 12;
				case 100:
					return 13;
				case 101:
					return 14;
				case 102:
					return 15;
				default:
					return digit - 48;
				}
				break;
			}
			return 10;
		}

		// Token: 0x06004189 RID: 16777 RVA: 0x0012F230 File Offset: 0x0012D430
		private static void PopulateFsmTables()
		{
			Lexer.fsm_handler_table = new Lexer.StateHandler[]
			{
				new Lexer.StateHandler(Lexer.State1),
				new Lexer.StateHandler(Lexer.State2),
				new Lexer.StateHandler(Lexer.State3),
				new Lexer.StateHandler(Lexer.State4),
				new Lexer.StateHandler(Lexer.State5),
				new Lexer.StateHandler(Lexer.State6),
				new Lexer.StateHandler(Lexer.State7),
				new Lexer.StateHandler(Lexer.State8),
				new Lexer.StateHandler(Lexer.State9),
				new Lexer.StateHandler(Lexer.State10),
				new Lexer.StateHandler(Lexer.State11),
				new Lexer.StateHandler(Lexer.State12),
				new Lexer.StateHandler(Lexer.State13),
				new Lexer.StateHandler(Lexer.State14),
				new Lexer.StateHandler(Lexer.State15),
				new Lexer.StateHandler(Lexer.State16),
				new Lexer.StateHandler(Lexer.State17),
				new Lexer.StateHandler(Lexer.State18),
				new Lexer.StateHandler(Lexer.State19),
				new Lexer.StateHandler(Lexer.State20),
				new Lexer.StateHandler(Lexer.State21),
				new Lexer.StateHandler(Lexer.State22),
				new Lexer.StateHandler(Lexer.State23),
				new Lexer.StateHandler(Lexer.State24),
				new Lexer.StateHandler(Lexer.State25),
				new Lexer.StateHandler(Lexer.State26),
				new Lexer.StateHandler(Lexer.State27),
				new Lexer.StateHandler(Lexer.State28)
			};
			Lexer.fsm_return_table = new int[]
			{
				65542, 0, 65537, 65537, 0, 65537, 0, 65537, 0, 0,
				65538, 0, 0, 0, 65539, 0, 0, 65540, 65541, 65542,
				0, 0, 65541, 65542, 0, 0, 0, 0
			};
		}

		// Token: 0x0600418A RID: 16778 RVA: 0x0012F418 File Offset: 0x0012D618
		private static char ProcessEscChar(int esc_char)
		{
			if (esc_char <= 92)
			{
				if (esc_char <= 39)
				{
					if (esc_char != 34 && esc_char != 39)
					{
						return '?';
					}
				}
				else if (esc_char != 47 && esc_char != 92)
				{
					return '?';
				}
				return Convert.ToChar(esc_char);
			}
			if (esc_char <= 102)
			{
				if (esc_char == 98)
				{
					return '\b';
				}
				if (esc_char == 102)
				{
					return '\f';
				}
			}
			else
			{
				if (esc_char == 110)
				{
					return '\n';
				}
				if (esc_char == 114)
				{
					return '\r';
				}
				if (esc_char == 116)
				{
					return '\t';
				}
			}
			return '?';
		}

		// Token: 0x0600418B RID: 16779 RVA: 0x0012F480 File Offset: 0x0012D680
		private static bool State1(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char != 32 && (ctx.L.input_char < 9 || ctx.L.input_char > 13))
				{
					if (ctx.L.input_char >= 49 && ctx.L.input_char <= 57)
					{
						ctx.L.string_buffer.Append((char)ctx.L.input_char);
						ctx.NextState = 3;
						return true;
					}
					int num = ctx.L.input_char;
					if (num <= 91)
					{
						if (num <= 39)
						{
							if (num == 34)
							{
								ctx.NextState = 19;
								ctx.Return = true;
								return true;
							}
							if (num != 39)
							{
								return false;
							}
							if (!ctx.L.allow_single_quoted_strings)
							{
								return false;
							}
							ctx.L.input_char = 34;
							ctx.NextState = 23;
							ctx.Return = true;
							return true;
						}
						else
						{
							switch (num)
							{
							case 44:
								break;
							case 45:
								ctx.L.string_buffer.Append((char)ctx.L.input_char);
								ctx.NextState = 2;
								return true;
							case 46:
								return false;
							case 47:
								if (!ctx.L.allow_comments)
								{
									return false;
								}
								ctx.NextState = 25;
								return true;
							case 48:
								ctx.L.string_buffer.Append((char)ctx.L.input_char);
								ctx.NextState = 4;
								return true;
							default:
								if (num != 58 && num != 91)
								{
									return false;
								}
								break;
							}
						}
					}
					else if (num <= 110)
					{
						if (num != 93)
						{
							if (num == 102)
							{
								ctx.NextState = 12;
								return true;
							}
							if (num != 110)
							{
								return false;
							}
							ctx.NextState = 16;
							return true;
						}
					}
					else
					{
						if (num == 116)
						{
							ctx.NextState = 9;
							return true;
						}
						if (num != 123 && num != 125)
						{
							return false;
						}
					}
					ctx.NextState = 1;
					ctx.Return = true;
					return true;
				}
			}
			return true;
		}

		// Token: 0x0600418C RID: 16780 RVA: 0x0012F678 File Offset: 0x0012D878
		private static bool State2(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char >= 49 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 3;
				return true;
			}
			if (ctx.L.input_char == 48)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 4;
				return true;
			}
			return false;
		}

		// Token: 0x0600418D RID: 16781 RVA: 0x0012F70C File Offset: 0x0012D90C
		private static bool State3(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
				}
				else
				{
					if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
					{
						ctx.Return = true;
						ctx.NextState = 1;
						return true;
					}
					int num = ctx.L.input_char;
					if (num <= 69)
					{
						if (num != 44)
						{
							if (num == 46)
							{
								ctx.L.string_buffer.Append((char)ctx.L.input_char);
								ctx.NextState = 5;
								return true;
							}
							if (num != 69)
							{
								return false;
							}
							goto IL_00F4;
						}
					}
					else if (num != 93)
					{
						if (num == 101)
						{
							goto IL_00F4;
						}
						if (num != 125)
						{
							return false;
						}
					}
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
					IL_00F4:
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 7;
					return true;
				}
			}
			return true;
		}

		// Token: 0x0600418E RID: 16782 RVA: 0x0012F848 File Offset: 0x0012DA48
		private static bool State4(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			int num = ctx.L.input_char;
			if (num <= 69)
			{
				if (num != 44)
				{
					if (num == 46)
					{
						ctx.L.string_buffer.Append((char)ctx.L.input_char);
						ctx.NextState = 5;
						return true;
					}
					if (num != 69)
					{
						return false;
					}
					goto IL_00BB;
				}
			}
			else if (num != 93)
			{
				if (num == 101)
				{
					goto IL_00BB;
				}
				if (num != 125)
				{
					return false;
				}
			}
			ctx.L.UngetChar();
			ctx.Return = true;
			ctx.NextState = 1;
			return true;
			IL_00BB:
			ctx.L.string_buffer.Append((char)ctx.L.input_char);
			ctx.NextState = 7;
			return true;
		}

		// Token: 0x0600418F RID: 16783 RVA: 0x0012F938 File Offset: 0x0012DB38
		private static bool State5(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 6;
				return true;
			}
			return false;
		}

		// Token: 0x06004190 RID: 16784 RVA: 0x0012F998 File Offset: 0x0012DB98
		private static bool State6(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
				}
				else
				{
					if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
					{
						ctx.Return = true;
						ctx.NextState = 1;
						return true;
					}
					int num = ctx.L.input_char;
					if (num <= 69)
					{
						if (num != 44)
						{
							if (num != 69)
							{
								return false;
							}
							goto IL_00C9;
						}
					}
					else if (num != 93)
					{
						if (num == 101)
						{
							goto IL_00C9;
						}
						if (num != 125)
						{
							return false;
						}
					}
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
					IL_00C9:
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 7;
					return true;
				}
			}
			return true;
		}

		// Token: 0x06004191 RID: 16785 RVA: 0x0012FAA8 File Offset: 0x0012DCA8
		private static bool State7(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 8;
				return true;
			}
			int num = ctx.L.input_char;
			if (num == 43 || num == 45)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 8;
				return true;
			}
			return false;
		}

		// Token: 0x06004192 RID: 16786 RVA: 0x0012FB44 File Offset: 0x0012DD44
		private static bool State8(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
				}
				else
				{
					if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
					{
						ctx.Return = true;
						ctx.NextState = 1;
						return true;
					}
					int num = ctx.L.input_char;
					if (num == 44 || num == 93 || num == 125)
					{
						ctx.L.UngetChar();
						ctx.Return = true;
						ctx.NextState = 1;
						return true;
					}
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004193 RID: 16787 RVA: 0x0012FC19 File Offset: 0x0012DE19
		private static bool State9(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 114)
			{
				ctx.NextState = 10;
				return true;
			}
			return false;
		}

		// Token: 0x06004194 RID: 16788 RVA: 0x0012FC41 File Offset: 0x0012DE41
		private static bool State10(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 117)
			{
				ctx.NextState = 11;
				return true;
			}
			return false;
		}

		// Token: 0x06004195 RID: 16789 RVA: 0x0012FC69 File Offset: 0x0012DE69
		private static bool State11(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 101)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x06004196 RID: 16790 RVA: 0x0012FC97 File Offset: 0x0012DE97
		private static bool State12(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 97)
			{
				ctx.NextState = 13;
				return true;
			}
			return false;
		}

		// Token: 0x06004197 RID: 16791 RVA: 0x0012FCBF File Offset: 0x0012DEBF
		private static bool State13(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 108)
			{
				ctx.NextState = 14;
				return true;
			}
			return false;
		}

		// Token: 0x06004198 RID: 16792 RVA: 0x0012FCE7 File Offset: 0x0012DEE7
		private static bool State14(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 115)
			{
				ctx.NextState = 15;
				return true;
			}
			return false;
		}

		// Token: 0x06004199 RID: 16793 RVA: 0x0012FC69 File Offset: 0x0012DE69
		private static bool State15(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 101)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x0600419A RID: 16794 RVA: 0x0012FD0F File Offset: 0x0012DF0F
		private static bool State16(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 117)
			{
				ctx.NextState = 17;
				return true;
			}
			return false;
		}

		// Token: 0x0600419B RID: 16795 RVA: 0x0012FD37 File Offset: 0x0012DF37
		private static bool State17(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 108)
			{
				ctx.NextState = 18;
				return true;
			}
			return false;
		}

		// Token: 0x0600419C RID: 16796 RVA: 0x0012FD5F File Offset: 0x0012DF5F
		private static bool State18(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 108)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x0600419D RID: 16797 RVA: 0x0012FD90 File Offset: 0x0012DF90
		private static bool State19(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				int num = ctx.L.input_char;
				if (num == 34)
				{
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 20;
					return true;
				}
				if (num == 92)
				{
					ctx.StateStack = 19;
					ctx.NextState = 21;
					return true;
				}
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
			}
			return true;
		}

		// Token: 0x0600419E RID: 16798 RVA: 0x0012FE10 File Offset: 0x0012E010
		private static bool State20(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 34)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x0600419F RID: 16799 RVA: 0x0012FE40 File Offset: 0x0012E040
		private static bool State21(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num <= 92)
			{
				if (num <= 39)
				{
					if (num != 34 && num != 39)
					{
						return false;
					}
				}
				else if (num != 47 && num != 92)
				{
					return false;
				}
			}
			else if (num <= 102)
			{
				if (num != 98 && num != 102)
				{
					return false;
				}
			}
			else if (num != 110)
			{
				switch (num)
				{
				case 114:
				case 116:
					break;
				case 115:
					return false;
				case 117:
					ctx.NextState = 22;
					return true;
				default:
					return false;
				}
			}
			ctx.L.string_buffer.Append(Lexer.ProcessEscChar(ctx.L.input_char));
			ctx.NextState = ctx.StateStack;
			return true;
		}

		// Token: 0x060041A0 RID: 16800 RVA: 0x0012FEF4 File Offset: 0x0012E0F4
		private static bool State22(FsmContext ctx)
		{
			int num = 0;
			int num2 = 4096;
			ctx.L.unichar = 0;
			while (ctx.L.GetChar())
			{
				if ((ctx.L.input_char < 48 || ctx.L.input_char > 57) && (ctx.L.input_char < 65 || ctx.L.input_char > 70) && (ctx.L.input_char < 97 || ctx.L.input_char > 102))
				{
					return false;
				}
				ctx.L.unichar += Lexer.HexValue(ctx.L.input_char) * num2;
				num++;
				num2 /= 16;
				if (num == 4)
				{
					ctx.L.string_buffer.Append(Convert.ToChar(ctx.L.unichar));
					ctx.NextState = ctx.StateStack;
					return true;
				}
			}
			return true;
		}

		// Token: 0x060041A1 RID: 16801 RVA: 0x0012FFE8 File Offset: 0x0012E1E8
		private static bool State23(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				int num = ctx.L.input_char;
				if (num == 39)
				{
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 24;
					return true;
				}
				if (num == 92)
				{
					ctx.StateStack = 23;
					ctx.NextState = 21;
					return true;
				}
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
			}
			return true;
		}

		// Token: 0x060041A2 RID: 16802 RVA: 0x00130068 File Offset: 0x0012E268
		private static bool State24(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 39)
			{
				ctx.L.input_char = 34;
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x060041A3 RID: 16803 RVA: 0x001300A4 File Offset: 0x0012E2A4
		private static bool State25(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 42)
			{
				ctx.NextState = 27;
				return true;
			}
			if (num != 47)
			{
				return false;
			}
			ctx.NextState = 26;
			return true;
		}

		// Token: 0x060041A4 RID: 16804 RVA: 0x001300EA File Offset: 0x0012E2EA
		private static bool State26(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char == 10)
				{
					ctx.NextState = 1;
					return true;
				}
			}
			return true;
		}

		// Token: 0x060041A5 RID: 16805 RVA: 0x00130114 File Offset: 0x0012E314
		private static bool State27(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char == 42)
				{
					ctx.NextState = 28;
					return true;
				}
			}
			return true;
		}

		// Token: 0x060041A6 RID: 16806 RVA: 0x00130140 File Offset: 0x0012E340
		private static bool State28(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char != 42)
				{
					if (ctx.L.input_char == 47)
					{
						ctx.NextState = 1;
						return true;
					}
					ctx.NextState = 27;
					return true;
				}
			}
			return true;
		}

		// Token: 0x060041A7 RID: 16807 RVA: 0x00130190 File Offset: 0x0012E390
		private bool GetChar()
		{
			if ((this.input_char = this.NextChar()) != -1)
			{
				return true;
			}
			this.end_of_input = true;
			return false;
		}

		// Token: 0x060041A8 RID: 16808 RVA: 0x001301B9 File Offset: 0x0012E3B9
		private int NextChar()
		{
			if (this.input_buffer != 0)
			{
				int num = this.input_buffer;
				this.input_buffer = 0;
				return num;
			}
			return this.reader.Read();
		}

		// Token: 0x060041A9 RID: 16809 RVA: 0x001301DC File Offset: 0x0012E3DC
		public bool NextToken()
		{
			this.fsm_context.Return = false;
			while (Lexer.fsm_handler_table[this.state - 1](this.fsm_context))
			{
				if (this.end_of_input)
				{
					return false;
				}
				if (this.fsm_context.Return)
				{
					this.string_value = this.string_buffer.ToString();
					this.string_buffer.Remove(0, this.string_buffer.Length);
					this.token = Lexer.fsm_return_table[this.state - 1];
					if (this.token == 65542)
					{
						this.token = this.input_char;
					}
					this.state = this.fsm_context.NextState;
					return true;
				}
				this.state = this.fsm_context.NextState;
			}
			throw new JsonException(this.input_char);
		}

		// Token: 0x060041AA RID: 16810 RVA: 0x001302B1 File Offset: 0x0012E4B1
		private void UngetChar()
		{
			this.input_buffer = this.input_char;
		}

		// Token: 0x04004446 RID: 17478
		private static int[] fsm_return_table;

		// Token: 0x04004447 RID: 17479
		private static Lexer.StateHandler[] fsm_handler_table;

		// Token: 0x04004448 RID: 17480
		private bool allow_comments;

		// Token: 0x04004449 RID: 17481
		private bool allow_single_quoted_strings;

		// Token: 0x0400444A RID: 17482
		private bool end_of_input;

		// Token: 0x0400444B RID: 17483
		private FsmContext fsm_context;

		// Token: 0x0400444C RID: 17484
		private int input_buffer;

		// Token: 0x0400444D RID: 17485
		private int input_char;

		// Token: 0x0400444E RID: 17486
		private TextReader reader;

		// Token: 0x0400444F RID: 17487
		private int state;

		// Token: 0x04004450 RID: 17488
		private StringBuilder string_buffer;

		// Token: 0x04004451 RID: 17489
		private string string_value;

		// Token: 0x04004452 RID: 17490
		private int token;

		// Token: 0x04004453 RID: 17491
		private int unichar;

		// Token: 0x02000AA1 RID: 2721
		// (Invoke) Token: 0x060041AC RID: 16812
		private delegate bool StateHandler(FsmContext ctx);
	}
}
