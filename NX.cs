//		NumberX, a C# library for storing and manipulating numbers with 
//	arbitrary base and precision.
//	Copyright (C) 2022  Karuljonnai Gustav Màrthos Vünnsha
//
//		This program is free software: you can redistribute it and/or modify
//	it under the terms of the GNU Affero General Public License as published
//	by the Free Software Foundation, either version 3 of the License, or
//	any later version.
//
//		This program is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//	GNU Affero General Public License for more details.
//
//		You should have received a copy of the GNU Affero General Public License
//	along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
//		In case of any questions, you may contact the creator at 
//	<karuljonnai@gmail.com>.
//
//		This program was developed using GitHub; you can find the original
//	repository at <https://github.com/Karuljonnai/NumberX>.

using System.Text.RegularExpressions;

namespace NumberX;

[Serializable]
public class NX{
	// *** Global:
	internal static ushort PRECISION = 32;
	internal static byte   DFLT_BASE = 10;
	// § Regex:
	private const string B64    = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ@#";
	private const string B64Pat = @$"^([<>])([+-])?(([{B64}]*)[,\.]?([{B64}]*))(\*[{B64}])?(\^[+-]?[{B64}]+)?$";
	private static Regex B64RE  = new(B64Pat);
	private const string NXON   = @"^{\n?\t?Base: ?([\d]{1,3}),\n?\t?Sign: ?(True|False),\n?\t?Powr: ?(-?\+?\d{1,8}),\n?\t?Nums: ?\[\n?((?:\t?\t?\d{1,3},\n?)*)(\t?\t?\d{1,3})\n?\t?\]\n?}";
	private static Regex NXONRE = new(NXON);
	// *** Self attributes:
	internal byte    Base = DFLT_BASE;
	internal bool    Sign = false;
	internal int     Powr = 0;
	internal short[] Nums = {0};
	// *** Raw constructors:
	internal NX(in bool Sign, in short[] Digits, in byte Base, in int Power){
		this.Sign = Sign;
		this.Nums = Digits;
		this.Base = Base;
		this.Powr = Power;
	}
	internal NX(in NX Num){
		this.Sign = Num.Sign;
		this.Nums = Num.Nums;
		this.Base = Num.Base;
		this.Powr = Num.Powr;
	}
	internal NX(){}
	// *** Builders:
	// § String Builders:
	// * String B64 Builder
	public static NX FromB64(in string Num){
		// ¶ Safeguard:
		if(!B64RE.IsMatch(Num)){
			Console.Error.WriteLine("\tError:\nThe creation of a NX was attempted and failed: Syntax error.");
			throw new B64SyntaxError();
		}
		// ¶ Init:
		var Elements = B64RE.Match(Num).Groups;
		bool    isBE = "<" !=  Elements[1].ToString();
		bool    Sign = B64Sign(Elements[2].ToString());
		short[] Nums = B64Nums(Elements[4].ToString() + Elements[5].ToString(), isBE);
		byte    Base = B64Base(Elements[6].ToString());
		int     Powr = B64Powr(Elements[7].ToString(),  Elements[3].ToString(), Base, isBE);
		// Return
		return new NX(Sign, Nums, Base, Powr);
	}
	public static NX FromJSON(in string Num){
		// ¶ Safeguard:
		if(!NXONRE.IsMatch(Num)){
			Console.Error.WriteLine("\tError:\nThe creation of a NX was attempted and failed: Syntax error.");
			throw new NXONSyntaxError();
		}
		// ¶ Init:
		var Elements = NXONRE.Match(Num).Groups;
		byte    Base = NXONBase(Elements[1].ToString());
		bool    Sign = NXONSign(Elements[2].ToString());
		int     Powr = NXONPowr(Elements[3].ToString());
		short[] Nums = NXONNums(Elements[4].ToString() + Elements[5].ToString());
		// Return
		return new NX(Sign, Nums, Base, Powr);
	}
	// § Number Builders:
	// * Floating point builder
	public static NX FromDouble(double Num, byte Base = 0){
		// ¶ Safeguard:
		if(Base < 2){Base = DFLT_BASE;}
		// ¶ Init:
		bool    Sign = Num < 0;
		short[] Nums = new short[PRECISION];
		Num          = Math.Abs(Num);
		int     Powr = Num != 0 ? (int)(Math.Log2(Num) / Math.Log2(Base)) : 0;
		// ¶ Conversion:
		int j = 0;
		for(int i = Powr; i > Powr - PRECISION; i--){
			short Temp = (short)(Num / Math.Pow(Base, i));
			Num       -= Temp * Math.Pow(Base, i);
			Nums[j++]  = Temp;
		}
		// Return:
		return new NX(Sign, Nums, Base, Powr);
	}
	// * Integer builder
	public static NX FromLong(in long Num, byte Base = 0){
		// ¶ Safeguard:
		if(Base < 2){Base = DFLT_BASE;}
		// ¶ Init:
		bool    Sign = Num < 0;
		uint    Powr = (uint)Math.Abs((Math.Log2(Num) / Math.Log2(Base)));
		short[] Nums = ToNums(Num, Base, Powr);
		// Return:
		return new NX(Sign, Nums, Base, (int)Powr);
	}
	// *** Getters & Setters:
	// § Getters:
	public int Size => this.Nums.Length;
	public int LowPow => this.Powr - this.Size +1;
	public static ushort GetPrecision() => PRECISION;
	public static byte GetDefaultBase() => DFLT_BASE;
	// § Setters:
	public static void SetPrecision(ushort Precision){
		PRECISION = Precision;
		Console.WriteLine("\tWarning:\nThe Precision was altered; having the precision set too high will plummet the performance. Use it at your own risk. The recommended precision range is 15<->100.");
	}
	public static void SetDefaultBase(byte Base){
		if(Base < 2){
			Console.WriteLine("\tWarning:\nInvalid base; changes not applied!");
			return;
		}
		DFLT_BASE = Base;
	}
	// *** Indexers
	public   short this[in int Index]{
		get{
			if(Index < 0 || Index >= this.Size){return 0;}
			return this.Nums[Index];
		}
		set => this.Nums[Index] = value;
	}
	public   short this[in Index Index]{
		get{
			int Val = Index.IsFromEnd ? this.Size - Index.Value : Index.Value;
			return this[Val];
		}
		set{
			int  Val  = Index.IsFromEnd ? this.Size - Index.Value : Index.Value;
			this[Val] = value;
		}
	}
	internal short[] this[in int Start, in int End]{
		get => this.Nums[Start .. End];
	}
	public   short[] this[in System.Range Range]{
		get{
			int     Start  = Range.Start.Value;
			int     End    = Range.End.IsFromEnd ? this.Size - Range.End.Value : Range.End.Value;
			short[] Result = new short[Range.End.Value - Range.Start.Value];
			for(int i = Start , j = 0; i < End; i++, j++){Result[j] = this[i];}
			return Result;
		}
	}
	// *** Operator methods:
	public static NX operator <<(in NX Num, in int Shift) => Num.ShiftPow(Shift);
	public static NX operator >>(in NX Num, in int Shift) => Num.ShiftPow(-Shift);
	public static NX operator ++(in NX Num) => MathY.Increment(Num);
	public static NX operator --(in NX Num) => MathY.Decrement(Num);
	public static NX operator  ~(in NX Num) => MathY.RecSB(Num);
	public static NX operator  !(in NX Num) => MathY.FacSB(Num);
	public static NX operator  +(in NX Num) => Num;
	public static NX operator  -(in NX Num) => MathY.Negate(Num);
	public static NX operator  +(in NX A, in NX B) => MathY.SumSB(A, B);
	public static NX operator  -(in NX A, in NX B) => MathY.SumSB(A, -B);
	public static NX operator  *(in NX A, in NX B) => MathY.Mul(A, B);
	public static NX operator  /(in NX A, in NX B) => MathY.DivSB(A, B);
	public static NX operator  %(in NX A, in NX B) => MathY.ModSB(A, B);
	public static NX operator  ^(in NX A, in NX B) => MathY.PowSQ(A, B);
	// § Comparators:
	public static bool operator ==(in NX A, in NX B) => MathY.Compare(A, B) == MathY.COMP.SAME;
	public static bool operator !=(in NX A, in NX B) => MathY.Compare(A, B) != MathY.COMP.SAME;
	public static bool operator  >(in NX A, in NX B) => MathY.Compare(A, B) == MathY.COMP.MORE;
	public static bool operator  <(in NX A, in NX B) => MathY.Compare(A, B) == MathY.COMP.LESS;
	public static bool operator >=(in NX A, in NX B) => MathY.Compare(A, B) != MathY.COMP.LESS;
	public static bool operator <=(in NX A, in NX B) => MathY.Compare(A, B) != MathY.COMP.MORE;
	public static bool operator  &(in NX A, in NX B) => A.Base == B.Base;
	public static bool operator  |(in NX A, in NX B) => A.Base == 0 | B.Base == 0;
	public override bool Equals(object? Obj) => ReferenceEquals(this, Obj);
	public override int GetHashCode() => base.GetHashCode();
	// *** Conversion:
	public override string ToString() => this.ToStrJSON();
	public static explicit operator double(in NX Num) => Num.ToDouble();
	public static explicit operator long(in NX Num) => Num.ToLong();
	public static implicit operator bool(in NX? Num) => !ReferenceEquals(Num, null);
	public static explicit operator NX(in short Num){
		return new NX{
			Sign = Num < 0,
			Nums = new short[]{Math.Abs(Num)},
			Base = 0,
			Powr = 0
		};
	}
	public string ToStrB64(in int Digits = int.MaxValue){
		// ¶ Safeguard:
		if(this.Base > 64){
			Console.Error.WriteLine("\tError:\nAttempted to write a NX with a base outside of the B64's range!");
			return "";
		}
		// ¶ Endianness indicator:
		string Str = ">";
		// ¶ Sign indicator:
		Str += this.Sign ? "-" : "+";
		// ¶ Digits sequence:
		Str += B64[this[0]] + ".";
		int Len = Math.Min(this.Size, Digits);
		for(int i = 1; i < Len; i++){Str += B64[this[i]];}
		// ¶ Base indicator:
		Str += "*" + B64[this.Base];
		// ¶ Power indicator:
		Str += "^" + (this.Powr < 0 ? "-" : "+");
		uint PowrPow = (uint)Math.Abs((Math.Log2(this.Powr) / Math.Log2(this.Base)));
		short[] Pow = ToNums(this.Powr, this.Base, PowrPow);
		for(int i = 0; i < Pow.Length; i++){Str += B64[Pow[i]];}
		// Return
		return Str;
	}
	public string ToStrJSON(bool Pretty = true){
		if(!Pretty){return this.ToStrJSONCompact();}
		// ¶ Init:
		this.Fix();
		string Digits = string.Join(",\n\t\t", this.Nums);
		// Return
		return $"{{\n\tBase: {this.Base},\n\tSign: {this.Sign},\n\tPowr: {this.Powr},\n\tNums: [\n\t\t{Digits}\n\t]\n}}";
	}
	private string ToStrJSONCompact(){
		// ¶ Init:
		this.Fix();
		string Digits = string.Join(",", this.Nums);
		// Return
		return $"{{Base:{this.Base},Sign:{this.Sign},Powr:{this.Powr},Nums:[{Digits}]}}";
	}
	public double ToDouble(){
		// ¶ Init:
		double Value = 0;
		double Sign  = this.Sign ? -1 : 1;
		// ¶ Sum of Nums:
		for(int i = 0; i < this.Size; i++){Value += Sign * this[i] * Math.Pow(this.Base, this.Powr - i);}
		// Return:
		return Value;
	}
	public long ToLong(){
		// ¶ Init:
		long Value = 0;
		long Sign  = this.Sign ? -1 : 1;
		// ¶ Sum of Nums:
		for(int i = 0; i <= this.Powr; i++){Value += Sign * this.NumAtPow(i) * (long)Math.Pow(this.Base, i);}
		// Return:
		return Value;
	}
	// *** Miscellaneous methods:
	internal NX InBase(in byte NewBase) => new NX(this){Base = NewBase};
	internal NX InBase(in NX Num) => new NX(this){Base = Num.Base};
	internal NX Signed(in bool NewSign) => new NX(this){Sign = NewSign};
	internal NX ShiftPow(in int Shift){
		NX Temp    = new NX(this);
		Temp.Powr += Shift;
		return Temp;
	}
	internal NX Segment(in int Start, in int End) => new NX(this.Sign, this[Start .. End], this.Base, this.Powr - Start);
	internal NX Segment(in Range range) => new NX(this.Sign, this[range], this.Base, this.Powr - range.Start.Value);
	internal NX Cut() => this.Segment(0, PRECISION);
	internal short NumAtPow(in int Pow) => this[this.Powr - Pow];
	internal int IndexAtPow(in int Pow) => this.Powr - Pow;
	internal int PowAtIndex(in int Ind) => this.Powr - Ind;
	// § Helper Functions:
	private static bool    B64Sign(in string Sign) => "-".Equals(Sign);
	private static short[] B64Nums(string Digits, in bool BEndian){
		// ¶ Safeguard:
		if(Digits == null){return new short[1]{0};}
		// ¶ Init:
		short[] Nums = new short[Digits.Length];
		if(!BEndian){Digits = Reverse(Digits);}
		// ¶ Decoding:
		for(int i = 0; i < Nums.Length; i++){Nums[i] = (short)B64.IndexOf(Digits[i]);}
		// Return:
		return Nums;
	}
	private static byte    B64Base(in string Base){
		if(Base.Length == 0){return DFLT_BASE;}
		int Index = B64.IndexOf(Base[1]);
		if(Index < 2){return DFLT_BASE;}
		return (byte)Index;
	}
	private static int     B64Powr(string Power, in string Num, in byte Base, in bool BEndian){
		// ¶ Init:
		int FP = Num.IndexOf('.');
		int Pow;
		if(FP == -1){Pow = Num.Length -1;}
		else{
			if(BEndian){Pow = FP -1;}
			else{Pow = Num.Length - FP -2;}
		}
		if(Power.Length == 0){return Pow;}
		Power = Power[1 .. ^0];
		int PowSign = Power[0] == '-' ? -1 : 1;
		if(Power[0] is '+' or '-'){Power = Power[1 .. ^0];}
		// ¶ Sums the power:
		if(BEndian){Power = Reverse(Power);}
		for(int i = 0; i < Power.Length; i++){Pow += PowSign * B64.IndexOf(Power[i]) * (int)Math.Pow(Base, i);}
		// Return:
		return Pow;
	}
	private static byte    NXONBase(in string Base) => byte.Parse(Base);
	private static bool    NXONSign(in string Sign) => "False".Equals(Sign);
	private static int     NXONPowr(in string Powr) => int.Parse(Powr);
	private static short[] NXONNums(in string Nums){
		// ¶ Init:
		string[] Digits  = Nums.Split(',', StringSplitOptions.TrimEntries);
		short[]  Numbers = new short[Digits.Length];
		// ¶ Assignment:
		for(int i = 0; i < Numbers.Length; i ++){Numbers[i] = short.Parse(Digits[i]);}
		// Return
		return Numbers;
	}
	private static short[] ToNums(long Value, in byte Base, in uint Powr){
		// ¶ Safeguard:
		if(Base < 2){
			Console.Error.WriteLine("\tError:\nAtempted to convert a number at an invalid base.");
			return new short[]{0};
		}
		// ¶ Init:
		Value        = Math.Abs(Value);
		short[] Nums = new short[Powr +1];
		// ¶ Convertion:
		for(uint i = Powr, j = 0; i >= 0; i--, j++){
			short Temp = (short)(Value / Math.Pow(Base, i));
			Value     -= (long)(Temp * Math.Pow(Base, i));
			Nums[j]    = Temp;
		}
		// Return:
		return Nums;
	}
	internal bool IsOverLoaded(){
		foreach(short i in this.Nums){if(i < 0 | i > this.Base){return true;}}
		return false;
	}
	private static string Reverse(string Str){
		char[] Arr = Str.ToCharArray();
		Array.Reverse(Arr);
		return new string(Arr);
	}
	// *** Cleaners:
	public void CBCleanUp(){
		if(this.Base == 0){return;}
		while(this.IsOverLoaded()){
			Restart:
			if(this[0] >= this.Base){
				this.Nums = this[-1 .. this.Size];
				this.Powr++;
			}
			else if(this[0] < 0){
				for(int i = 0; i < this.Size; i++){this[i] *= -1;}
				goto Restart;
			}
			for(int i = this.Size -1; i >= 0; i++){
				if(this[i] >= this.Base){
					this[i -1] += (short)(this[i] / this.Base);
					this[i]     = (short)(this[i] % this.Base);
				} else if(this[i] < 0){
					this[i -1] -= (short)(this[i] / this.Base +1);
					this[i]     = (short)(this[i] % this.Base + this.Base);
				}
			}
		}
	}
	public void Simplify(){
		int L = 0;
		int R = this.Size -1;
		while(this.Nums[L] == 0 & L <= R){L++;}
		while(this.Nums[R] == 0 & R > L){R--;}
		this.Nums  = this.Nums[L .. (R +1)];
		this.Powr -= L;
	}
	public void Fix(){
		this.Simplify();
		this.CBCleanUp();
	}
}