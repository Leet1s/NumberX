namespace NX;

public static class MathY{
	// *** Basic numbers:
	public static NX ZERO    = new NX(false, new sbyte[]{0}, 0, 0);
	public static NX ONE     = new NX(false, new sbyte[]{1}, 0, 0);
	public static NX TWO     = new NX(false, new sbyte[]{2}, 0, 0);
	public static NX BASE_ID = new NX(false, new sbyte[]{0, 1}, 0, 0);
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
	public COMP Compare(NX A, NX B){
		(int L, int H) = PowerBounds(A, B);
		
	}
	// § Numeric:
	// *** Helpers:
	private static (int, int) PowerBounds(NX Num) => (Num.Powr, Num.Powr + Num.Len());
	private static (int, int) PowerBounds(NX A, NX B){
		int LBound = Math.Min(A.Powr, B.Powr);
		int HBound = Math.Max(A.Powr + A.Len() -1, B.Powr + B.Len() -1);
		return (LBound, HBound);
	}
}