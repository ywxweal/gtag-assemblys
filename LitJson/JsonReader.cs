using System;
using System.Collections.Generic;
using System.IO;

namespace LitJson
{
	// Token: 0x02000A9B RID: 2715
	public class JsonReader
	{
		// Token: 0x17000685 RID: 1669
		// (get) Token: 0x06004147 RID: 16711 RVA: 0x0012DFAC File Offset: 0x0012C1AC
		// (set) Token: 0x06004148 RID: 16712 RVA: 0x0012DFB9 File Offset: 0x0012C1B9
		public bool AllowComments
		{
			get
			{
				return this.lexer.AllowComments;
			}
			set
			{
				this.lexer.AllowComments = value;
			}
		}

		// Token: 0x17000686 RID: 1670
		// (get) Token: 0x06004149 RID: 16713 RVA: 0x0012DFC7 File Offset: 0x0012C1C7
		// (set) Token: 0x0600414A RID: 16714 RVA: 0x0012DFD4 File Offset: 0x0012C1D4
		public bool AllowSingleQuotedStrings
		{
			get
			{
				return this.lexer.AllowSingleQuotedStrings;
			}
			set
			{
				this.lexer.AllowSingleQuotedStrings = value;
			}
		}

		// Token: 0x17000687 RID: 1671
		// (get) Token: 0x0600414B RID: 16715 RVA: 0x0012DFE2 File Offset: 0x0012C1E2
		public bool EndOfInput
		{
			get
			{
				return this.end_of_input;
			}
		}

		// Token: 0x17000688 RID: 1672
		// (get) Token: 0x0600414C RID: 16716 RVA: 0x0012DFEA File Offset: 0x0012C1EA
		public bool EndOfJson
		{
			get
			{
				return this.end_of_json;
			}
		}

		// Token: 0x17000689 RID: 1673
		// (get) Token: 0x0600414D RID: 16717 RVA: 0x0012DFF2 File Offset: 0x0012C1F2
		public JsonToken Token
		{
			get
			{
				return this.token;
			}
		}

		// Token: 0x1700068A RID: 1674
		// (get) Token: 0x0600414E RID: 16718 RVA: 0x0012DFFA File Offset: 0x0012C1FA
		public object Value
		{
			get
			{
				return this.token_value;
			}
		}

		// Token: 0x0600414F RID: 16719 RVA: 0x0012E002 File Offset: 0x0012C202
		static JsonReader()
		{
			JsonReader.PopulateParseTable();
		}

		// Token: 0x06004150 RID: 16720 RVA: 0x0012E009 File Offset: 0x0012C209
		public JsonReader(string json_text)
			: this(new StringReader(json_text), true)
		{
		}

		// Token: 0x06004151 RID: 16721 RVA: 0x0012E018 File Offset: 0x0012C218
		public JsonReader(TextReader reader)
			: this(reader, false)
		{
		}

		// Token: 0x06004152 RID: 16722 RVA: 0x0012E024 File Offset: 0x0012C224
		private JsonReader(TextReader reader, bool owned)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.parser_in_string = false;
			this.parser_return = false;
			this.read_started = false;
			this.automaton_stack = new Stack<int>();
			this.automaton_stack.Push(65553);
			this.automaton_stack.Push(65543);
			this.lexer = new Lexer(reader);
			this.end_of_input = false;
			this.end_of_json = false;
			this.reader = reader;
			this.reader_is_owned = owned;
		}

		// Token: 0x06004153 RID: 16723 RVA: 0x0012E0B0 File Offset: 0x0012C2B0
		private static void PopulateParseTable()
		{
			JsonReader.parse_table = new Dictionary<int, IDictionary<int, int[]>>();
			JsonReader.TableAddRow(ParserToken.Array);
			JsonReader.TableAddCol(ParserToken.Array, 91, new int[] { 91, 65549 });
			JsonReader.TableAddRow(ParserToken.ArrayPrime);
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 34, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 91, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 93, new int[] { 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 123, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65537, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65538, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65539, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65540, new int[] { 65550, 65551, 93 });
			JsonReader.TableAddRow(ParserToken.Object);
			JsonReader.TableAddCol(ParserToken.Object, 123, new int[] { 123, 65545 });
			JsonReader.TableAddRow(ParserToken.ObjectPrime);
			JsonReader.TableAddCol(ParserToken.ObjectPrime, 34, new int[] { 65546, 65547, 125 });
			JsonReader.TableAddCol(ParserToken.ObjectPrime, 125, new int[] { 125 });
			JsonReader.TableAddRow(ParserToken.Pair);
			JsonReader.TableAddCol(ParserToken.Pair, 34, new int[] { 65552, 58, 65550 });
			JsonReader.TableAddRow(ParserToken.PairRest);
			JsonReader.TableAddCol(ParserToken.PairRest, 44, new int[] { 44, 65546, 65547 });
			JsonReader.TableAddCol(ParserToken.PairRest, 125, new int[] { 65554 });
			JsonReader.TableAddRow(ParserToken.String);
			JsonReader.TableAddCol(ParserToken.String, 34, new int[] { 34, 65541, 34 });
			JsonReader.TableAddRow(ParserToken.Text);
			JsonReader.TableAddCol(ParserToken.Text, 91, new int[] { 65548 });
			JsonReader.TableAddCol(ParserToken.Text, 123, new int[] { 65544 });
			JsonReader.TableAddRow(ParserToken.Value);
			JsonReader.TableAddCol(ParserToken.Value, 34, new int[] { 65552 });
			JsonReader.TableAddCol(ParserToken.Value, 91, new int[] { 65548 });
			JsonReader.TableAddCol(ParserToken.Value, 123, new int[] { 65544 });
			JsonReader.TableAddCol(ParserToken.Value, 65537, new int[] { 65537 });
			JsonReader.TableAddCol(ParserToken.Value, 65538, new int[] { 65538 });
			JsonReader.TableAddCol(ParserToken.Value, 65539, new int[] { 65539 });
			JsonReader.TableAddCol(ParserToken.Value, 65540, new int[] { 65540 });
			JsonReader.TableAddRow(ParserToken.ValueRest);
			JsonReader.TableAddCol(ParserToken.ValueRest, 44, new int[] { 44, 65550, 65551 });
			JsonReader.TableAddCol(ParserToken.ValueRest, 93, new int[] { 65554 });
		}

		// Token: 0x06004154 RID: 16724 RVA: 0x0012E429 File Offset: 0x0012C629
		private static void TableAddCol(ParserToken row, int col, params int[] symbols)
		{
			JsonReader.parse_table[(int)row].Add(col, symbols);
		}

		// Token: 0x06004155 RID: 16725 RVA: 0x0012E43D File Offset: 0x0012C63D
		private static void TableAddRow(ParserToken rule)
		{
			JsonReader.parse_table.Add((int)rule, new Dictionary<int, int[]>());
		}

		// Token: 0x06004156 RID: 16726 RVA: 0x0012E450 File Offset: 0x0012C650
		private void ProcessNumber(string number)
		{
			double num;
			if ((number.IndexOf('.') != -1 || number.IndexOf('e') != -1 || number.IndexOf('E') != -1) && double.TryParse(number, out num))
			{
				this.token = JsonToken.Double;
				this.token_value = num;
				return;
			}
			int num2;
			if (int.TryParse(number, out num2))
			{
				this.token = JsonToken.Int;
				this.token_value = num2;
				return;
			}
			long num3;
			if (long.TryParse(number, out num3))
			{
				this.token = JsonToken.Long;
				this.token_value = num3;
				return;
			}
			this.token = JsonToken.Int;
			this.token_value = 0;
		}

		// Token: 0x06004157 RID: 16727 RVA: 0x0012E4EC File Offset: 0x0012C6EC
		private void ProcessSymbol()
		{
			if (this.current_symbol == 91)
			{
				this.token = JsonToken.ArrayStart;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 93)
			{
				this.token = JsonToken.ArrayEnd;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 123)
			{
				this.token = JsonToken.ObjectStart;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 125)
			{
				this.token = JsonToken.ObjectEnd;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 34)
			{
				if (this.parser_in_string)
				{
					this.parser_in_string = false;
					this.parser_return = true;
					return;
				}
				if (this.token == JsonToken.None)
				{
					this.token = JsonToken.String;
				}
				this.parser_in_string = true;
				return;
			}
			else
			{
				if (this.current_symbol == 65541)
				{
					this.token_value = this.lexer.StringValue;
					return;
				}
				if (this.current_symbol == 65539)
				{
					this.token = JsonToken.Boolean;
					this.token_value = false;
					this.parser_return = true;
					return;
				}
				if (this.current_symbol == 65540)
				{
					this.token = JsonToken.Null;
					this.parser_return = true;
					return;
				}
				if (this.current_symbol == 65537)
				{
					this.ProcessNumber(this.lexer.StringValue);
					this.parser_return = true;
					return;
				}
				if (this.current_symbol == 65546)
				{
					this.token = JsonToken.PropertyName;
					return;
				}
				if (this.current_symbol == 65538)
				{
					this.token = JsonToken.Boolean;
					this.token_value = true;
					this.parser_return = true;
				}
				return;
			}
		}

		// Token: 0x06004158 RID: 16728 RVA: 0x0012E65E File Offset: 0x0012C85E
		private bool ReadToken()
		{
			if (this.end_of_input)
			{
				return false;
			}
			this.lexer.NextToken();
			if (this.lexer.EndOfInput)
			{
				this.Close();
				return false;
			}
			this.current_input = this.lexer.Token;
			return true;
		}

		// Token: 0x06004159 RID: 16729 RVA: 0x0012E69D File Offset: 0x0012C89D
		public void Close()
		{
			if (this.end_of_input)
			{
				return;
			}
			this.end_of_input = true;
			this.end_of_json = true;
			if (this.reader_is_owned)
			{
				this.reader.Close();
			}
			this.reader = null;
		}

		// Token: 0x0600415A RID: 16730 RVA: 0x0012E6D0 File Offset: 0x0012C8D0
		public bool Read()
		{
			if (this.end_of_input)
			{
				return false;
			}
			if (this.end_of_json)
			{
				this.end_of_json = false;
				this.automaton_stack.Clear();
				this.automaton_stack.Push(65553);
				this.automaton_stack.Push(65543);
			}
			this.parser_in_string = false;
			this.parser_return = false;
			this.token = JsonToken.None;
			this.token_value = null;
			if (!this.read_started)
			{
				this.read_started = true;
				if (!this.ReadToken())
				{
					return false;
				}
			}
			while (!this.parser_return)
			{
				this.current_symbol = this.automaton_stack.Pop();
				this.ProcessSymbol();
				if (this.current_symbol == this.current_input)
				{
					if (!this.ReadToken())
					{
						if (this.automaton_stack.Peek() != 65553)
						{
							throw new JsonException("Input doesn't evaluate to proper JSON text");
						}
						return this.parser_return;
					}
				}
				else
				{
					int[] array;
					try
					{
						array = JsonReader.parse_table[this.current_symbol][this.current_input];
					}
					catch (KeyNotFoundException ex)
					{
						throw new JsonException((ParserToken)this.current_input, ex);
					}
					if (array[0] != 65554)
					{
						for (int i = array.Length - 1; i >= 0; i--)
						{
							this.automaton_stack.Push(array[i]);
						}
					}
				}
			}
			if (this.automaton_stack.Peek() == 65553)
			{
				this.end_of_json = true;
			}
			return true;
		}

		// Token: 0x0400441E RID: 17438
		private static IDictionary<int, IDictionary<int, int[]>> parse_table;

		// Token: 0x0400441F RID: 17439
		private Stack<int> automaton_stack;

		// Token: 0x04004420 RID: 17440
		private int current_input;

		// Token: 0x04004421 RID: 17441
		private int current_symbol;

		// Token: 0x04004422 RID: 17442
		private bool end_of_json;

		// Token: 0x04004423 RID: 17443
		private bool end_of_input;

		// Token: 0x04004424 RID: 17444
		private Lexer lexer;

		// Token: 0x04004425 RID: 17445
		private bool parser_in_string;

		// Token: 0x04004426 RID: 17446
		private bool parser_return;

		// Token: 0x04004427 RID: 17447
		private bool read_started;

		// Token: 0x04004428 RID: 17448
		private TextReader reader;

		// Token: 0x04004429 RID: 17449
		private bool reader_is_owned;

		// Token: 0x0400442A RID: 17450
		private object token_value;

		// Token: 0x0400442B RID: 17451
		private JsonToken token;
	}
}
