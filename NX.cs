using System.Security.Cryptography.X509Certificates;
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

namespace NumberX;

using System.Text.RegularExpressions;
using System.Linq;

public class NX{
	// *** Global:
	internal volatile static ushort PRECISION = 32;
	// § Regex:
	private const string B64     = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ@#";
	private const string Pattern = @"^([<>])([+-])?(([0-9a-zA-Z@#]*)[,\.]?([0-9a-zA-Z@#]*))(\*[2-9a-zA-Z])(\^[+-]?[0-9a-zA-Z@#]+)?$";
	private static Regex RE      = new Regex(Pattern);
	// *** Self attributes:
	internal bool    Sign = false;
	internal short[] Nums = {0};
	internal byte    Base = 2;
	internal int     Powr = 0;
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
	// *** Builder:
	// * String B62 Builder
	public static NX New(in string Num){
		// ¶ Safeguard (Checks for correct usage of syntax):
		if(!RE.IsMatch(Num)){
			Console.Error.WriteLine("\tError:\nThe creation of a NX was attempted and failed: Syntax error.");
			return null!;
		}
		// ¶ Init:
		var  Elements = RE.Match(Num).Groups;
		bool isBE     = ">".Equals(Elements[1].ToString());
		// ¶ Creates the raw values for the new NX:
		bool    Sign = StrSign(Elements[2].ToString());
		short[] Nums = StrNums(Elements[4].ToString() + Elements[5].ToString(), isBE);
		byte    Base = (byte) B64.IndexOf(Elements[6].ToString()[1]);
		int     Powr = StrPowr(Elements[7].ToString(),  Elements[3].ToString(), Base, isBE);
		// Return:
		return new NX(Sign, Nums, Base, Powr);
	}
	// * Floating point builder
	public static NX New(double Num, in byte Base = 2){
		// ¶ Safeguard:
		if(Base < 2){
			Console.Error.WriteLine("\tError:\nAttempted to create a NX with an invalid base!");
		}
		// ¶ Init:
		bool    Sign = Num < 0;
		Num          = Math.Abs(Num);
		short[] Nums = new short[PRECISION];
		// ¶ Conversion:
		int j   = Nums.Length;
		int Pow = (int)(Math.Log2(Num) / Math.Log2(Base));
		for(int i = Pow; i > Pow - PRECISION; i--){
			short Temp = (short)(Num / Math.Pow(Base, i));
			Num       -= Temp * Math.Pow(Base, i);
			Nums[--j]  = Temp;
		}
		int Powr = Pow - Nums.Length;
		// Return:
		return new NX(Sign, Nums, Base, Powr);
	}
	// * Integer builder
	public static NX New(long Num, in byte Base = 2){
		// ¶ Safeguard:
		if(Base < 2){
			Console.Error.WriteLine("\tError:\nAttempted to create a NX with an invalid base!");
		}
		// ¶ Init:
		bool    Sign = Num < 0;
		short[] Nums = ToNums(Num, Base);
		// Return:
		return new NX(Sign, Nums, Base, 0);
	}
	// *** Getters & Setters:
	// § Getters:
	public int Size => this.Nums.Length;
	public static ushort GetPrecision() => PRECISION;
	public int LastPow => this.Powr + this.Size -1;
	// * Indexers
	public short this[int Index]{
		get{
			if(Index < 0 || Index >= this.Size){return 0;}
			return this.Nums[Index];
		}
		set => this.Nums[Index] = value;
	}
	public short[] this[System.Range Range]{
		get{
			short[] Result = new short[Range.End.Value - Range.Start.Value];
			int j = 0;
			for(int i = Range.Start.Value; i < Range.End.Value; i++){Result[j++] = this[i];}
			return Result;
		}
	}
	public short this[Index Index]{
		get => this[Index.Value];
		set => this[Index.Value] = value;
	}
	// § Setters:
	public static void SetPrecision(ushort Precision){
		PRECISION = Precision;
		Console.WriteLine("\tWarning:\nThe Precision was altered; having the precision set too high will plummet the performance. Use it at your own risk. The recommended precision range is 15<->100.");
	}
	// *** Operator methods:
	public static NX operator ++(NX Num) => Num = MathY.Increment(Num);
	public static NX operator --(NX Num) => Num = MathY.Decrement(Num);
	public static NX operator +(NX Num) => Num;
	public static NX operator -(NX Num) => MathY.Negate(Num);
	public static NX operator +(NX A, NX B) => MathY.Sum(A, B);
	public static NX operator -(NX A, NX B) => MathY.Sum(A, -B);
	public static NX operator *(NX A, NX B){
		int Length = A.Size + B.Size;
		if(Length > 300){return MathY.MulAK(A, B);}
		return MathY.MulSB(A, B);
	}
	public static NX operator /(NX A, NX B) => MathY.DivSB(A, B);
	// § Comparators:
	public static bool operator ==(NX A, NX B) => MathY.Compare(A, B) == MathY.COMP.SAME;
	public static bool operator !=(NX A, NX B) => MathY.Compare(A, B) != MathY.COMP.SAME;
	public static bool operator  >(NX A, NX B) => MathY.Compare(A, B) == MathY.COMP.MORE;
	public static bool operator  <(NX A, NX B) => MathY.Compare(A, B) == MathY.COMP.LESS;
	public static bool operator >=(NX A, NX B) => MathY.Compare(A, B) != MathY.COMP.LESS;
	public static bool operator <=(NX A, NX B) => MathY.Compare(A, B) != MathY.COMP.MORE;
	public override bool Equals(object? Obj) => ReferenceEquals(this, Obj);
	public override int GetHashCode() => base.GetHashCode();
	// *** Conversion:
	public override string ToString() => this.ToStrB64();
	public static explicit operator double(NX Num) => Num.ToDouble();
	public static explicit operator long(NX Num) => Num.ToLong();
	public string ToStrB64(in bool BEndian = true){
		if(this.Base > 64){
			Console.Error.WriteLine("\tError:\nAttempted to write a NX with a base outside of the B64's range!");
			return "";
		}
		// ¶ Endianness indicator:
		string Str = BEndian ? ">" : "<";
		// ¶ Sign indicator:
		Str += this.Sign ? '-' : '+';
		// ¶ Digits sequence:
		if(BEndian)
			for(int i = this.Size; --i >= 0;)
				Str  += B64[this[i]];
		else
			for(int i = 0; i < this.Size; i++)
				Str  += B64[this[i]];
		// ¶ Base indicator:
		Str += '*' + B64[this.Base];
		// ¶ Power indicator:
		Str += '^' + this.Powr < 0 ? '-' : '+';
		short[] Pow = ToNums(this.Powr, this.Base);
		if(BEndian)
			for(int i = Pow.Length; --i >= 0;)
				Str += B64[Pow[i]];
		else
			for(int i = 0; i < Pow.Length; i++)
				Str += B64[Pow[i]];
		// Return:
		return Str;
	}
	public double ToDouble(){
		// ¶ Init:
		double Value = 0;
		double Sign  = this.Sign ? -1 : 1;
		// ¶ Sum of Nums:
		for(int i = 0; i < this.Size; i++){Value += Sign * this[i] * Math.Pow(this.Base, i + this.Powr);}
		// Return:
		return Value;
	}
	public long ToLong(){
		// ¶ Init:
		long Value = 0;
		long Sign  = this.Sign ? -1 : 1;
		(_, int HB) = MathY.PowerBounds(this);
		if(HB < 0){return Value;}
		// ¶ Sum of Nums:
		for(int i = 0; i <= HB; i++){Value += Sign * this.NumAtPow(i) * (long)Math.Pow(this.Base, i);}
		// Return:
		return Value;
	}
	// *** Miscellaneous methods:
	// § Helper Functions:
	private static bool StrSign(in string Sign) => "-".Equals(Sign);
	private static short[] StrNums(string Digits, in bool BEndian){
		// ¶ Safeguard:
		if(Digits == null){return new short[1]{0};}
		// ¶ Init:
		short[] Nums = new short[Digits.Length];
		if(BEndian){Digits = (string) Digits.Reverse();}
		// ¶ Decoding:
		for(int i = 0; i < Nums.Length; i++){Nums[i] = (short) B64.IndexOf(Digits[i]);}
		// Return:
		return Nums;
	}
	private static int StrPowr(string Power, string Num, in byte Base, in bool BEndian){
		// ¶ Safeguard:
		if(Power == null || Power.Length == 0){return 0;}
		// ¶ Init:
		int PowSign;
		Power   = Power[1 .. ^0];
		if(Power[0] == '-'){PowSign = -1;}
			else{PowSign = 1;}
		if(Power[0] is '+' or '-'){Power = Power[1..^0];}
		if(BEndian){Power = (string) Power.Reverse();}
		// ¶ Sums the power:
		int Pow;
		int ID = Num.IndexOf('.');
		if(ID == -1){Pow = 0;}
		else{
			if(BEndian){Pow = ID - (Num.Length -1);}
			else{Pow = -ID;}
		}
		for(int i = 0; i < Power.Length; i++){Pow += PowSign * B64.IndexOf(Power[i]) * (int) Math.Pow(Base, i);}
		// Return:
		return Pow;
	}
	private static short[] ToNums(long Value, in byte Base = 2){
		// ¶ Safeguard:
		if(Base < 2){
			Console.Error.WriteLine("\tError:\nAtempted to convert a number at an invalid base.");
			return new short[]{0};
		}
		// ¶ Init:
		Value        = Math.Abs(Value);
		int     Pow  = (int)(Math.Log2(Value) / Math.Log2(Base));
		short[] Nums = new short[Pow +1];
		// ¶ Convertion:
		for(int i = Pow; i >= 0; i--){
			short Temp = (short)(Value / Math.Pow(Base, i));
			Value     -= (long)  (Temp * Math.Pow(Base, i));
			Nums[i]    = Temp;
		}
		// Return:
		return Nums;
	}
	internal short NumAtPow(in int Pow) => this[Pow - this.Powr];
	internal NX ShiftPow(in int Shift){
		NX Temp    = new NX(this);
		Temp.Powr += Shift;
		return Temp;
	}
	internal NX Based(in byte NewBase) => new NX(this){Base = NewBase};
	internal bool IsOverLoaded(){
		foreach(short i in this.Nums){if(i < 0 || i > this.Base){return true;}}
		return false;
	}
	// *** Cleaners:
	public void CBCleanUp(){
		while(this.IsOverLoaded()){
			if(this[^1] >= this.Base){this.Nums = this[0 .. (this.Size +1)];}
			else if(this[^1] < 0){
				for(int i = 0; i < this.Size; i++){this[i] *= -1;}
				this.CBCleanUp();
				return;
			}
			for(int i = 0; i < this.Size; i++){
				if(this[i] >= this.Base){
					this[i +1] += (short)(this[i] / this.Base);
					this[i]     = (short)(this[i] % this.Base);
				} else if(this[i] < 0){
					this[i +1] -= (short)(this[i] / this.Base +1);
					this[i]     = (short)(this[i] % this.Base);
				}
			}
		}
	}
	public void Simplify(){
		int L = 0;
		int R = this.Size -1;
		while(this.Nums[L] == 0 && L <= R){L++;}
		while(this.Nums[R] == 0 && R > L){R--;}
		this.Nums  = this.Nums[L .. (R +1)];
		this.Powr += L;
	}
}