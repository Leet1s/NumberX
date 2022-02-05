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

using System.Threading.Tasks;

public static class MathY{
	// *** Basic numbers:
	public static readonly NX ZERO    = new NX(false, new short[]{0}, 0, 0);
	public static readonly NX ONE     = new NX(false, new short[]{1}, 0, 0);
	public static readonly NX TWO     = new NX(false, new short[]{2}, 0, 0);
	public static readonly NX BASE_ID = new NX(false, new short[]{0, 1}, 0, 0);
	public static readonly NX POS_INF = new NX(false, new short[]{short.MaxValue}, 0, 0);
	public static readonly NX NEG_INF = new NX(true , new short[]{short.MaxValue}, 0, 0);
	// *** Unary operations:
	public static NX Abs(NX Num){
		Num.Sign = false;
		return Num;
	}
	public static NX Negate(NX Num){
		Num.Sign = !Num.Sign;
		return Num;
	}
	public static NX Increment(NX Num) => Num + ONE.Based(Num.Base);
	public static NX Decrement(NX Num) => Num - ONE.Based(Num.Base);
	// *** Binary operations:
	// § Comparisons:
	public enum COMP{SAME, LESS, MORE};
	public static COMP Compare(in NX A, in NX B){
		// ¶ Safeguard:
		if(A.Base != B.Base){Console.Error.WriteLine("\tWarning:\nA comparison of numbers with different bases was attempted!");}
		// ¶ Shortcut:
		if(A.Sign != B.Sign){
			if(A.Sign){return COMP.MORE;}
			else{return COMP.LESS;}
		}
		// ¶ Init:
		(int LB, int HB) = PowerBounds(A, B);
		// ¶ Comparison:
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
	public static NX Sum(NX A, NX B){
		// ¶ Safeguard:
		if(A.Base != B.Base){
			Console.Error.WriteLine("\tError:\nAn addition of numbers with different bases was attempted!");
			return null!;
		}
		// ¶ Init:
		(int LB, int HB) = PowerBounds(A, B);
		NX C = new NX(
			false,
			new short[HB - LB +1],
			A.Base,
			LB
		);
		int ASign = A.Sign ? -1 : 1;
		int BSign = B.Sign ? -1 : 1;
		// ¶ Summation:
		Parallel.For(0, C.Size, i => {
			C[i] = (short)(ASign * A.NumAtPow(LB + i) + BSign * B.NumAtPow(LB + i));
		});
		// ¶ Checks:
		for(int i = C.Size -1; i-- >= 0;){
			if(C[i] < 0){
				C.Sign = true;
				break;
			} else if(C[i] > 0){break;}
		}
		if(C.Sign){for(int i = 0; i < C.Size; i++){C[i] *= -1;}}
		C.CBCleanUp();
		C.Simplify();
		// Return
		return C;
	}
	public static NX MulSB(in NX A, in NX B){
		// ¶ Safeguard:
		if(A.Base != B.Base){
			Console.Error.WriteLine("\tError:\nA multiplication of numbers with different bases was attempted!");
			return null!;
		}
		// ¶ Init:
		NX C = new NX(
			A.Sign ^ B.Sign,
			new short[A.Size + B.Size],
			A.Base,
			A.Powr + B.Powr
		);
		// ¶ Multiplication:
		for(int i = 0; i < B.Size; i++){C += SingleMul(A, B[i]).ShiftPow(i + B.Powr);}
		// Return:
		return C;
	}
	public static NX MulAK(NX A, NX B){
		// ¶ Safeguard:
		if(A.Base != B.Base){
			Console.Error.WriteLine("\tError:\nA multiplication of numbers with different bases was attempted!");
			return null!;
		}
		// ¶ Init:
		MatchLength(ref A, ref B);
		// Base Case:
		if(A.Size == 1){return SingleMul(A, B[0]);}
		// ¶ Init:
		(NX A_L, NX A_H) = SplitHalf(A);
		(NX B_L, NX B_H) = SplitHalf(B);
		// ¶ Recursive calls in parallel:
		NX L = new NX();
		NX M = new NX();
		NX H = new NX();
		Parallel.Invoke(
			() => {L = MulAK(A_L,  B_L);},
			() => {M = MulAK(A_L + B_H, A_H + B_L);},
			() => {H = MulAK(A_H,  B_H);}
		);
		// Return:
		return 
			(L 
			+ (M - L - H).ShiftPow(A.Size / 2) 
			+ H.ShiftPow(A.Size)
			).ShiftPow(A.Powr + B.Powr);
	}
	public static NX DivSB(NX A, in NX B){
		// ¶ Safeguard:
		if(A.Base != B.Base){
			Console.Error.WriteLine("\tError:\nA division of numbers with different bases was attempted!");
			return null!;
		}
		if(A.Size < B.Size){return DivSB(B, A);}
		// ¶ Init:
		NX C = new NX(
			A.Sign ^ B.Sign,
			new short[NX.PRECISION],
			A.Base,
			A.LastPow - B.LastPow - NX.PRECISION +1
		);
		NX[] TableB = new NX[B.Base];
		for(int i = 0; i < TableB.Length; i++){TableB[i] = SingleMul(B, i);}
		// ¶ Division:
		int j = 0;
		int Shift = A.LastPow - B.LastPow;
		for(int i = C.Size -1; i >= 0; i--){
			C[i] = (short) BinSrc(TableB, A.ShiftPow(Shift - j));
			A   -= TableB[C[i]].ShiftPow(Shift - j);
			j++;
		}
		// Return:
		C.Simplify();
		return C;
	}
	public static NX Summation(in NX[] Numbers){
		// ¶ Safeguard:
		for(int i = 0; i < Numbers.Length; i++){
			if(Numbers[0].Base != Numbers[i].Base){
				Console.Error.WriteLine("\tError:\nThe summation of numbers with different bases was attempted!");
				return null!;
			}
		}
		// ¶ Init:
		NX Total = new NX{Base = Numbers[0].Base};
		// ¶ Summation:
		foreach(NX i in Numbers){Total += i;}
		// Return:
		return Total;
	}
	// TODO Constants:
	// *** Helpers:
	internal static (int LB, int HB) PowerBounds(in NX Num) => (Num.Powr, Num.Powr + Num.Size -1);
	private  static (int LB, int HB) PowerBounds(in NX A, in NX B){
		int LBound = Math.Min(A.Powr, B.Powr);
		int HBound = Math.Max(A.Powr + A.Size -1, B.Powr + B.Size -1);
		return (LBound, HBound);
	}
	private static NX SingleMul(NX Num, in int Fac){
		for(int i = 0; i < Num.Size; i++){Num[i] *= (short) Fac;}
		Num.CBCleanUp();
		return Num;
	}
	private static int BinSrc(in NX[] Table, in NX Target){
		int L = 0;
		int R = Table.Length -1;
		while(L <= R){
			if(R < 0){return 0;}
			int M = (L + R) / 2;
			switch(Compare(Target, Table[M])){
				case COMP.SAME: return M;
				case COMP.LESS: R = M -1; break;
				case COMP.MORE: L = M +1; break;
			}
		}
		return R;
	}
	private static void MatchLength(ref NX A, ref NX B){
		if(A.Size >= B.Size){B.Nums = B[0 .. A.Size];}
		else{A.Nums = A[0 .. B.Size];}
	}
	private static (NX, NX) SplitHalf(NX Num){
		int Mid = Num.Size / 2;
		NX  LH  = new NX(
			Num.Sign,
			Num[0 .. Mid],
			Num.Base,
			0
		);
		NX  HH  = new NX(
			Num.Sign,
			Num[Mid .. ^0],
			Num.Base,
			0
		);
		return (LH, HH);
	}
	// *** Miscellaneous:
	public static bool IsInteger(in NX Num) => Num.Powr >= 0;
	public static bool IsEven(in NX Num){
		bool Even = Num.NumAtPow(0) % 2 == 0;
		if(Num.Base % 2 == 0 || Num.Powr + Num.Size <= 1){return Even;}
		for(int i = 1; i < Num.Powr + Num.Size; i++){Even ^= Num.NumAtPow(i) % 2 != 0;}
		return Even;
	}
}