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
		Parallel.For(0, C.Len(), i => {
			C[i] = (short)(ASign * A.NumAtPow(LB + i) + BSign * B.NumAtPow(LB + i));
		});
		// ¶ Checks:
		for(int i = C.Len() -1; i-- >= 0;){
			if(C[i] < 0){
				C.Sign = true;
				break;
			} else if(C[i] > 0){break;}
		}
		if(C.Sign){for(int i = 0; i < C.Len(); i++){C[i] *= -1;}}
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
			new short[A.Len() + B.Len()],
			A.Base,
			A.Powr + B.Powr
		);
		// ¶ Multiplication:
		for(int i = 0; i < B.Len(); i++){C += SingleMul(A, B[i]).ShiftPow(i + B.Powr);}
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
		if(A.Len() == 1){return SingleMul(A, B[0]);}
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
			+ (M - L - H).ShiftPow(A.Len() / 2) 
			+ H.ShiftPow(A.Len())
			).ShiftPow(A.Powr + B.Powr);
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
	// *** Helpers:
	private static (int LB, int HB) PowerBounds(in NX Num) => (Num.Powr, Num.Powr + Num.Len() -1);
	private static (int LB, int HB) PowerBounds(in NX A, in NX B){
		int LBound = Math.Min(A.Powr, B.Powr);
		int HBound = Math.Max(A.Powr + A.Len() -1, B.Powr + B.Len() -1);
		return (LBound, HBound);
	}
	private static NX SingleMul(NX Num, in short Fac){
		for(int i = 0; i < Num.Len(); i++){Num[i] *= Fac;}
		Num.CBCleanUp();
		return Num;
	}
	private static void MatchLength(ref NX A, ref NX B){
		if(A.Len() >= B.Len()){B.Nums = B[0 .. A.Len()];}
		else{A.Nums = A[0 .. B.Len()];}
	}
	private static (NX, NX) SplitHalf(NX Num){
		int Mid = Num.Len() / 2;
		NX  LH  = new NX(
			Num.Sign,
			Num[0 .. (Mid +1)],
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
}