using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace LitJson
{
	// Token: 0x02000A9E RID: 2718
	public class JsonWriter
	{
		// Token: 0x1700068B RID: 1675
		// (get) Token: 0x0600415C RID: 16732 RVA: 0x0012E83C File Offset: 0x0012CA3C
		// (set) Token: 0x0600415D RID: 16733 RVA: 0x0012E844 File Offset: 0x0012CA44
		public int IndentValue
		{
			get
			{
				return this.indent_value;
			}
			set
			{
				this.indentation = this.indentation / this.indent_value * value;
				this.indent_value = value;
			}
		}

		// Token: 0x1700068C RID: 1676
		// (get) Token: 0x0600415E RID: 16734 RVA: 0x0012E862 File Offset: 0x0012CA62
		// (set) Token: 0x0600415F RID: 16735 RVA: 0x0012E86A File Offset: 0x0012CA6A
		public bool PrettyPrint
		{
			get
			{
				return this.pretty_print;
			}
			set
			{
				this.pretty_print = value;
			}
		}

		// Token: 0x1700068D RID: 1677
		// (get) Token: 0x06004160 RID: 16736 RVA: 0x0012E873 File Offset: 0x0012CA73
		public TextWriter TextWriter
		{
			get
			{
				return this.writer;
			}
		}

		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x06004161 RID: 16737 RVA: 0x0012E87B File Offset: 0x0012CA7B
		// (set) Token: 0x06004162 RID: 16738 RVA: 0x0012E883 File Offset: 0x0012CA83
		public bool Validate
		{
			get
			{
				return this.validate;
			}
			set
			{
				this.validate = value;
			}
		}

		// Token: 0x06004164 RID: 16740 RVA: 0x0012E898 File Offset: 0x0012CA98
		public JsonWriter()
		{
			this.inst_string_builder = new StringBuilder();
			this.writer = new StringWriter(this.inst_string_builder);
			this.Init();
		}

		// Token: 0x06004165 RID: 16741 RVA: 0x0012E8C2 File Offset: 0x0012CAC2
		public JsonWriter(StringBuilder sb)
			: this(new StringWriter(sb))
		{
		}

		// Token: 0x06004166 RID: 16742 RVA: 0x0012E8D0 File Offset: 0x0012CAD0
		public JsonWriter(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
			this.Init();
		}

		// Token: 0x06004167 RID: 16743 RVA: 0x0012E8F4 File Offset: 0x0012CAF4
		private void DoValidation(Condition cond)
		{
			if (!this.context.ExpectingValue)
			{
				this.context.Count++;
			}
			if (!this.validate)
			{
				return;
			}
			if (this.has_reached_end)
			{
				throw new JsonException("A complete JSON symbol has already been written");
			}
			switch (cond)
			{
			case Condition.InArray:
				if (!this.context.InArray)
				{
					throw new JsonException("Can't close an array here");
				}
				break;
			case Condition.InObject:
				if (!this.context.InObject || this.context.ExpectingValue)
				{
					throw new JsonException("Can't close an object here");
				}
				break;
			case Condition.NotAProperty:
				if (this.context.InObject && !this.context.ExpectingValue)
				{
					throw new JsonException("Expected a property");
				}
				break;
			case Condition.Property:
				if (!this.context.InObject || this.context.ExpectingValue)
				{
					throw new JsonException("Can't add a property here");
				}
				break;
			case Condition.Value:
				if (!this.context.InArray && (!this.context.InObject || !this.context.ExpectingValue))
				{
					throw new JsonException("Can't add a value here");
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06004168 RID: 16744 RVA: 0x0012EA18 File Offset: 0x0012CC18
		private void Init()
		{
			this.has_reached_end = false;
			this.hex_seq = new char[4];
			this.indentation = 0;
			this.indent_value = 4;
			this.pretty_print = false;
			this.validate = true;
			this.ctx_stack = new Stack<WriterContext>();
			this.context = new WriterContext();
			this.ctx_stack.Push(this.context);
		}

		// Token: 0x06004169 RID: 16745 RVA: 0x0012EA7C File Offset: 0x0012CC7C
		private static void IntToHex(int n, char[] hex)
		{
			for (int i = 0; i < 4; i++)
			{
				int num = n % 16;
				if (num < 10)
				{
					hex[3 - i] = (char)(48 + num);
				}
				else
				{
					hex[3 - i] = (char)(65 + (num - 10));
				}
				n >>= 4;
			}
		}

		// Token: 0x0600416A RID: 16746 RVA: 0x0012EABD File Offset: 0x0012CCBD
		private void Indent()
		{
			if (this.pretty_print)
			{
				this.indentation += this.indent_value;
			}
		}

		// Token: 0x0600416B RID: 16747 RVA: 0x0012EADC File Offset: 0x0012CCDC
		private void Put(string str)
		{
			if (this.pretty_print && !this.context.ExpectingValue)
			{
				for (int i = 0; i < this.indentation; i++)
				{
					this.writer.Write(' ');
				}
			}
			this.writer.Write(str);
		}

		// Token: 0x0600416C RID: 16748 RVA: 0x0012EB28 File Offset: 0x0012CD28
		private void PutNewline()
		{
			this.PutNewline(true);
		}

		// Token: 0x0600416D RID: 16749 RVA: 0x0012EB34 File Offset: 0x0012CD34
		private void PutNewline(bool add_comma)
		{
			if (add_comma && !this.context.ExpectingValue && this.context.Count > 1)
			{
				this.writer.Write(',');
			}
			if (this.pretty_print && !this.context.ExpectingValue)
			{
				this.writer.Write('\n');
			}
		}

		// Token: 0x0600416E RID: 16750 RVA: 0x0012EB90 File Offset: 0x0012CD90
		private void PutString(string str)
		{
			this.Put(string.Empty);
			this.writer.Write('"');
			int length = str.Length;
			int i = 0;
			while (i < length)
			{
				char c = str[i];
				switch (c)
				{
				case '\b':
					this.writer.Write("\\b");
					break;
				case '\t':
					this.writer.Write("\\t");
					break;
				case '\n':
					this.writer.Write("\\n");
					break;
				case '\v':
					goto IL_00E4;
				case '\f':
					this.writer.Write("\\f");
					break;
				case '\r':
					this.writer.Write("\\r");
					break;
				default:
					if (c != '"' && c != '\\')
					{
						goto IL_00E4;
					}
					this.writer.Write('\\');
					this.writer.Write(str[i]);
					break;
				}
				IL_0141:
				i++;
				continue;
				IL_00E4:
				if (str[i] >= ' ' && str[i] <= '~')
				{
					this.writer.Write(str[i]);
					goto IL_0141;
				}
				JsonWriter.IntToHex((int)str[i], this.hex_seq);
				this.writer.Write("\\u");
				this.writer.Write(this.hex_seq);
				goto IL_0141;
			}
			this.writer.Write('"');
		}

		// Token: 0x0600416F RID: 16751 RVA: 0x0012ECF6 File Offset: 0x0012CEF6
		private void Unindent()
		{
			if (this.pretty_print)
			{
				this.indentation -= this.indent_value;
			}
		}

		// Token: 0x06004170 RID: 16752 RVA: 0x0012ED13 File Offset: 0x0012CF13
		public override string ToString()
		{
			if (this.inst_string_builder == null)
			{
				return string.Empty;
			}
			return this.inst_string_builder.ToString();
		}

		// Token: 0x06004171 RID: 16753 RVA: 0x0012ED30 File Offset: 0x0012CF30
		public void Reset()
		{
			this.has_reached_end = false;
			this.ctx_stack.Clear();
			this.context = new WriterContext();
			this.ctx_stack.Push(this.context);
			if (this.inst_string_builder != null)
			{
				this.inst_string_builder.Remove(0, this.inst_string_builder.Length);
			}
		}

		// Token: 0x06004172 RID: 16754 RVA: 0x0012ED8B File Offset: 0x0012CF8B
		public void Write(bool boolean)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(boolean ? "true" : "false");
			this.context.ExpectingValue = false;
		}

		// Token: 0x06004173 RID: 16755 RVA: 0x0012EDBB File Offset: 0x0012CFBB
		public void Write(decimal number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x06004174 RID: 16756 RVA: 0x0012EDE8 File Offset: 0x0012CFE8
		public void Write(double number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			string text = Convert.ToString(number, JsonWriter.number_format);
			this.Put(text);
			if (text.IndexOf('.') == -1 && text.IndexOf('E') == -1)
			{
				this.writer.Write(".0");
			}
			this.context.ExpectingValue = false;
		}

		// Token: 0x06004175 RID: 16757 RVA: 0x0012EE47 File Offset: 0x0012D047
		public void Write(int number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x06004176 RID: 16758 RVA: 0x0012EE73 File Offset: 0x0012D073
		public void Write(long number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x06004177 RID: 16759 RVA: 0x0012EE9F File Offset: 0x0012D09F
		public void Write(string str)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			if (str == null)
			{
				this.Put("null");
			}
			else
			{
				this.PutString(str);
			}
			this.context.ExpectingValue = false;
		}

		// Token: 0x06004178 RID: 16760 RVA: 0x0012EED1 File Offset: 0x0012D0D1
		public void Write(ulong number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x06004179 RID: 16761 RVA: 0x0012EF00 File Offset: 0x0012D100
		public void WriteArrayEnd()
		{
			this.DoValidation(Condition.InArray);
			this.PutNewline(false);
			this.ctx_stack.Pop();
			if (this.ctx_stack.Count == 1)
			{
				this.has_reached_end = true;
			}
			else
			{
				this.context = this.ctx_stack.Peek();
				this.context.ExpectingValue = false;
			}
			this.Unindent();
			this.Put("]");
		}

		// Token: 0x0600417A RID: 16762 RVA: 0x0012EF6C File Offset: 0x0012D16C
		public void WriteArrayStart()
		{
			this.DoValidation(Condition.NotAProperty);
			this.PutNewline();
			this.Put("[");
			this.context = new WriterContext();
			this.context.InArray = true;
			this.ctx_stack.Push(this.context);
			this.Indent();
		}

		// Token: 0x0600417B RID: 16763 RVA: 0x0012EFC0 File Offset: 0x0012D1C0
		public void WriteObjectEnd()
		{
			this.DoValidation(Condition.InObject);
			this.PutNewline(false);
			this.ctx_stack.Pop();
			if (this.ctx_stack.Count == 1)
			{
				this.has_reached_end = true;
			}
			else
			{
				this.context = this.ctx_stack.Peek();
				this.context.ExpectingValue = false;
			}
			this.Unindent();
			this.Put("}");
		}

		// Token: 0x0600417C RID: 16764 RVA: 0x0012F02C File Offset: 0x0012D22C
		public void WriteObjectStart()
		{
			this.DoValidation(Condition.NotAProperty);
			this.PutNewline();
			this.Put("{");
			this.context = new WriterContext();
			this.context.InObject = true;
			this.ctx_stack.Push(this.context);
			this.Indent();
		}

		// Token: 0x0600417D RID: 16765 RVA: 0x0012F080 File Offset: 0x0012D280
		public void WritePropertyName(string property_name)
		{
			this.DoValidation(Condition.Property);
			this.PutNewline();
			this.PutString(property_name);
			if (this.pretty_print)
			{
				if (property_name.Length > this.context.Padding)
				{
					this.context.Padding = property_name.Length;
				}
				for (int i = this.context.Padding - property_name.Length; i >= 0; i--)
				{
					this.writer.Write(' ');
				}
				this.writer.Write(": ");
			}
			else
			{
				this.writer.Write(':');
			}
			this.context.ExpectingValue = true;
		}

		// Token: 0x04004437 RID: 17463
		private static NumberFormatInfo number_format = NumberFormatInfo.InvariantInfo;

		// Token: 0x04004438 RID: 17464
		private WriterContext context;

		// Token: 0x04004439 RID: 17465
		private Stack<WriterContext> ctx_stack;

		// Token: 0x0400443A RID: 17466
		private bool has_reached_end;

		// Token: 0x0400443B RID: 17467
		private char[] hex_seq;

		// Token: 0x0400443C RID: 17468
		private int indentation;

		// Token: 0x0400443D RID: 17469
		private int indent_value;

		// Token: 0x0400443E RID: 17470
		private StringBuilder inst_string_builder;

		// Token: 0x0400443F RID: 17471
		private bool pretty_print;

		// Token: 0x04004440 RID: 17472
		private bool validate;

		// Token: 0x04004441 RID: 17473
		private TextWriter writer;
	}
}
