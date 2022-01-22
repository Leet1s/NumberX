namespace NX;

using System.Text.RegularExpressions;

public class NX{
	// *** Global:
	internal volatile static ushort     PRECISION  = 32;
	// § Regex:
	private const string B62     = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
	private const string Pattern = @"^(?<ord>[<>])(?<sign>[+-])?(?<nums>(?<nBC>[0-9a-zA-Z]*)[,\.]?(?<nAC>[0-9a-zA-Z]*))(?<base>\*[2-9a-zA-Z])(?<powr>\^[+-]?[0-9a-zA-Z]+)?$";
	private static Regex RE      = new Regex(Pattern);
	// *** Self attributes:
	// § Number properties:
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
		byte    Base = (byte) B62.IndexOf(Elements[6].ToString()[1]);
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
	public int Len() => this.Nums.Length;
	public static ushort GetPrecision() => PRECISION;
	// § Setters:
	public static void SetPrecision(ushort Precision){
		PRECISION = Precision;
		Console.WriteLine("\tWarning:\nThe Precision was altered; having the precision set too high will plummet the performance. Use it at your own risk. The recommended precision range is 15<->100.");
	}
	//TODO *** Operator methods:
	public static NX operator +(NX Num) => Num;
	public static NX operator -(NX Num) => MathY.Negate(Num);
	public static NX operator +(NX A, NX B) => MathY.Sum(A, B);
	//TODO *** Conversion casting:
	// *** Miscellaneous methods:
	// § Visualization:
	public override string ToString(){
		return this.ToStrB62();
	}
	public string ToStrB62(in bool BEndian = true){
		if(this.Base > 62){
			Console.Error.WriteLine("\tError:\nAttempted to write a NX with a base outside of the B62's range!");
			return "";
		}
		// ¶ Endianness indicator:
		string Str = BEndian ? ">" : "<";
		// ¶ Sign indicator:
		Str += this.Sign ? '-' : '+';
		// ¶ Digits sequence:
		if(BEndian)
			for(int i = this.Nums.Length; --i >= 0;)
				Str  += B62[this.Nums[i]];
		else
			for(int i = 0; i < this.Nums.Length; i++)
				Str  += B62[this.Nums[i]];
		// ¶ Base indicator:
		Str += '*' + B62[this.Base];
		// ¶ Power indicator:
		Str += '^' + this.Powr < 0 ? '-' : '+';
		short[] Pow = ToNums(this.Powr, this.Base);
		if(BEndian)
			for(int i = Pow.Length; --i >= 0;)
				Str += B62[Pow[i]];
		else
			for(int i = 0; i < Pow.Length; i++)
				Str += B62[Pow[i]];
		// Return:
		return Str;
	}
	// § Helper Functions:
	private static bool StrSign(in string Sign) => "-".Equals(Sign);
	private static short[] StrNums(string Digits, in bool BEndian){
		// ¶ Safeguard:
		if(Digits == null){return new short[1]{0};}
		// ¶ Init:
		short[] Nums = new short[Digits.Length];
		if(BEndian){Digits = (string) Digits.Reverse();}
		// ¶ Decoding:
		for(int i = 0; i < Nums.Length; i++){Nums[i] = (short) B62.IndexOf(Digits[i]);}
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
			else{Pow = - ID;}
		}
		for(int i = 0; i < Power.Length; i++){Pow += PowSign * B62.IndexOf(Power[i]) * (int) Math.Pow(Base, i);}
		// Return:
		return Pow;
	}
	private static int LongLog2(long Num){
		const long Bit = long.MinValue;
		int        Pow = 63;
		while(Pow >= 0){
			if((Num & Bit) == Bit){break;}
			Num <<= 1;
			Pow--;
		}
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
	internal short NumAtPow(in int Pow){
		if(Pow < this.Powr || Pow >= this.Powr + this.Len()){return 0;}
		return this.Nums[Pow - this.Powr];
	}
	internal short Index(in int Index){
		if(Index < 0 || Index >= this.Len()){return 0;}
		return this.Nums[Index];
	}
	internal bool IsOverLoaded(){
		foreach(short i in this.Nums){if(i < 0 || i > this.Base){return true;}}
		return false;
	}
	
}