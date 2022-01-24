namespace NX;

public static class MathY{
	// *** Basic numbers:
	public static NX ZERO    = new NX(false, new short[]{0}, 0, 0);
	public static NX ONE     = new NX(false, new short[]{1}, 0, 0);
	public static NX TWO     = new NX(false, new short[]{2}, 0, 0);
	public static NX BASE_ID = new NX(false, new short[]{0, 1}, 0, 0);
	// *** Unary operations:
	private static NX Based(NX Num, in byte Base){
		Num.Base = Base;
		return Num;
	}
	public static NX Abs(NX Num){
		Num.Sign = false;
		return Num;
	}
	public static NX Negate(NX Num){
		Num.Sign = !Num.Sign;
		return Num;
	}
	// *** Binary operations:
	// § Comparisons:
	public enum COMP{SAME, LESS, MORE};
	public static COMP Compare(in NX A, in NX B){
		(int LB, int HB) = PowerBounds(A, B);
		for(int i = HB; i >= LB; i--){
			int DigitA = A.NumAtPow(i);
			int DigitB = B.NumAtPow(i);
			if(DigitA != DigitB){
				if(DigitA > DigitB){return COMP.MORE;}
				else{return COMP.LESS;}
			}
		}
		return COMP.SAME;
	}
	// § Numeric:
	public static NX Sum(in NX A, in NX B){
		// ¶ Safeguard:
		if(A.Base != B.Base){
			Console.Error.WriteLine("Error:\n\tAn addition of numbers with different bases was attempted!");
			return null!;
		}
		// ¶ Init:
		(int LB, int HB) = PowerBounds(A, B);
		NX C = new NX(
			false,
			new short[HB - LB +2],
			A.Base,
			LB
		);
		int ASign = A.Sign ? -1 : 1;
		int BSign = B.Sign ? -1 : 1;
		// ¶ Summation:
		for(int i = 0; i < C.Len() -1; i++){C[i] = (short)(ASign * A.NumAtPow(LB + i) + BSign * B.NumAtPow(LB + i));}
		// ¶ Checks:
		for(int i = C.Len() -1; i-- >= 0;){
			if(C[i] < 0){
				C.Sign = true;
				break;
			} else if(C[i] > 0){break;}
		}
		if(C.Sign){for(int i = 0; i < C.Len(); i++){C[i] *= -1;}}
		C.CBCleanUp();
		// Return
		return C;
	}
	// *** Helpers:
	private static (int, int) PowerBounds(in NX Num) => (Num.Powr, Num.Powr + Num.Len());
	private static (int, int) PowerBounds(in NX A, in NX B){
		int LBound = Math.Min(A.Powr, B.Powr);
		int HBound = Math.Max(A.Powr + A.Len() -1, B.Powr + B.Len() -1);
		return (LBound, HBound);
	}
}