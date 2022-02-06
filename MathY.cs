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

using System.Threading.Tasks;

namespace NumberX;

public static class MathY{
	// *** Basic numbers:
	public static readonly NX ZERO    = new NX(false, new short[]{0}, 0, 0);
	public static readonly NX ONE     = new NX(false, new short[]{1}, 0, 0);
	public static readonly NX TWO     = new NX(false, new short[]{2}, 0, 0);
	public static readonly NX BASE_ID = new NX(false, new short[]{0, 1}, 0, 0);
	public static readonly NX POS_INF = new NX(false, new short[]{short.MaxValue}, 0, int.MaxValue);
	public static readonly NX NEG_INF = new NX(true , new short[]{short.MaxValue}, 0, int.MaxValue);
	// *** Unary operations:
	public static NX Floor(in NX Num){
		if(Num.Powr >= 0){return Num;}
		try{return new NX(Num.Sign, Num[-Num.Powr, Num.Size], Num.Base, 0);}
		catch{return ZERO.Based(Num.Base);}
	}
	public static NX Ceil(in NX Num){
		NX Res = Floor(Num);
		if(Num > Res){Res++;}
		return Res;
	}
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
	public static NX RecSB(NX Num) => ONE.Based(Num.Base) / Num;
	public static NX RecNR(NX Num){
		// ¶ Safeguard:
		if(Abs(Num) == ZERO.Based(Num.Base)){
			Console.Error.WriteLine("\tWarning:\nA division by zero was made!");
			if(Num.Sign){return NEG_INF;}
			return POS_INF;
		}
		if(Num.Equals(POS_INF) || Num.Equals(NEG_INF)){return ZERO.Based(Num.Base);}
		// ¶ Init:
		NX  Rec = NX.New(1 / (double) Num, Num.Base);
		NX _Rec = new NX{Base = Num.Base};
		// ¶ Newton-Raphson Iterations:
		int i = 0;
		while(MatchingDigits(Rec, _Rec) <= NX.PRECISION && i <= NX.PRECISION){
			_Rec = Rec;
			RefineNR(Num, ref Rec);
			i++;
		}
		// Return
		return Rec;
		// ¶ Newton-Raphson Iteration:
		void RefineNR(in NX Target, ref NX Guess) => Guess += Guess * (ONE.Based(Guess.Base) - Guess * Target);
	}
	public static NX FacSB(NX Num){
		// ¶ Safeguard:
		if(!IsInteger(Num)){
			Console.WriteLine("\tWarning:\nThe factorial of a non-integer number is being attempted using the school-book method, which is meant only for integers!");
			Num = Floor(Num);
		}
		// ¶ Init:
		NX Fac = ONE.Based(Num.Base);
		NX One = ONE.Based(Num.Base);
		while(Num > One){
			Fac *= Num;
			Num.Simplify();
			Num--;
		}
		// Return:
		return Fac;
	}
	// § Trigometric:
	public static NX CosTS(NX Num){
		// Init:
		NX C = ZERO.Based(Num.Base);
		NX N = ZERO.Based(Num.Base);
		NX I = ONE.Based(Num.Base);
		// ¶ Taylor series:
		while(I > ONE.Based(I.Base) << NX.PRECISION){
			I  = (-ONE.Based(Num.Base) ^ N) / !(TWO.Based(Num.Base) * N) * (Num ^ (TWO.Based(Num.Base) * N));
			C += I;
			N++;
		}
		// Return
		return C;
	}
	// TODO SinTS
	// TODO TanTS
	public static NX TanKMApprox(NX Num){
		// Init:
		NX PI2  = PI(Num.Base) ^ TWO.Based(Num.Base);
		NX NS4  = (Num ^ TWO.Based(Num.Base)) * new NX(true, new short[]{4}, Num.Base, 0);
		NX Five = new NX(false, new short[]{5}, Num.Base, 0);
		// Return
		return (Num * (Five * PI2 + NS4)) / (Five * (PI2 + NS4));
	}
	// *** Binary operations:
	// § Comparisons:
	public enum COMP{SAME, LESS, MORE};
	public static COMP Compare(in NX A, in NX B){
		// ¶ Safeguard:
		if(A.Base != B.Base){Console.Error.WriteLine("\tWarning:\nA comparison of numbers with different bases was attempted!");}
		// ¶ Shortcut:
		if(A.Sign != B.Sign){
			if(A.Sign){return COMP.LESS;}
			else{return COMP.MORE;}
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
	// § Arithmetic:
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
		for(int i = 0; i < B.Size; i++){C += SingleMul(A, B[i])>>(i + B.Powr);}
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
			+ ((M - L - H)>>(A.Size / 2))
			+ (H >>(A.Size))
			)>>(A.Powr + B.Powr);
	}
	public static NX DivSB(NX A, in NX B){
		// ¶ Safeguard:
		if(A.Base != B.Base){
			Console.Error.WriteLine("\tError:\nA division of numbers with different bases was attempted!");
			return null!;
		}
		if(Abs(B) == ZERO.Based(B.Base)){
			Console.Error.WriteLine("\tWarning:\nA division by zero was made!");
			if(A.Sign){return NEG_INF;}
			return POS_INF;
		}
		if(B.Equals(POS_INF) || B.Equals(NEG_INF)){return ZERO.Based(B.Base);}
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
			C[i] = (short) BinSrc(TableB, A >>(Shift - j));
			A   -= TableB[C[i]]>>(Shift - j);
			j++;
		}
		// Return:
		C.Simplify();
		return C;
	}
	public static NX DivNR(NX A, NX B) => (A.Signed(A.Sign ^ B.Sign) << (B.LastPow +1)) * RecNR(Abs(B) << (B.LastPow +1));
	public static NX DivRG(NX A, NX B){
		// Safeguard:
		if(A.Base != B.Base){
			Console.Error.WriteLine("\tError:\nA division with numbers with different bases was attempted!");
			return null!;
		}
		// ¶ Init:
		A.Sign ^= B.Sign;
		B.Sign  = false;
		A     <<= B.LastPow +1;
		B     <<= B.LastPow +1;
		int i = 0;
		while(ONE.Based(A.Base) - B > ONE.Based(A.Base) << NX.PRECISION && i <= NX.PRECISION){
			NX F = TWO.Based(B.Base) - B;
			Parallel.Invoke(
				() => A *= F,
				() => B *= F
			);
			i++;
		}
		// Return
		return A;
	}
	// TODO Mod
	public static NX ExpSQ(in NX A, NX B){
		// ¶ Safeguard:
		if(A.Base != B.Base){
			Console.Error.WriteLine("\tError:\nAttempted to exponentiate numbers with different bases!");
			return null!;
		}
		if(!IsInteger(B)){
			Console.WriteLine("\tWarning:\nA exponentiation with a non integer power is being attempted with a method not designed for that!");
			B = Floor(B);
		}
		// ¶ Base cases:
		if(A == -ONE.Based(A.Base)){
			if(IsEven(B)){return ONE.Based(A.Base);}
			else{return -ONE.Based(A.Base);}
		}
		if(B == ONE.Based(B.Base)){return A;}
		if(A == ONE.Based(A.Base) || B == ZERO.Based(B.Base)){return ONE.Based(A.Base);}
		// ¶ Init:
		NX C = new NX();
		// ¶ Binary divide and conquer recursion:
		if(IsEven(B)){
			C  = ExpSQ(A, B / TWO.Based(B.Base));
			C *= C;
		} else{
			B--;
			C  = ExpSQ(A, B / TWO.Based(B.Base));
			C *= C;
			C *= A;
		}
		// Return
		return C;
	}
	// *** N-Ary operations:
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
	// *** Constants:
	public static NX E(byte Base){
		// ¶ Safeguard:
		if(Base < 2){
			Console.Error.WriteLine("\tError:\nAttempted to compute a constant with an invalid base!");
			return null!;
		}
		// ¶ Init:
		NX E = ONE.Based(Base);
		NX N = ONE.Based(Base);
		NX I = ONE.Based(Base);
		// ¶ Iterations:
		while(I > ONE.Based(Base) << NX.PRECISION){
			I  = ~!N;
			E += I;
			N++;
		}
		// Return
		return E;
	}
	public static NX PI(byte Base){
		// ¶ Safeguard:
		if(Base < 2){
			Console.Error.WriteLine("\tError:\nAttempted to compute a constant with an invalid base!");
			return null!;
		}
		// ¶ Init:
		NX P = ONE.Based(Base);
		NX N = ONE.Based(Base);
		NX I = ONE.Based(Base);
		// ¶ Iterations:
		while(I > ONE.Based(Base) << NX.PRECISION){
			I  = ((TWO.Based(Base) ^ N) * (!N ^ TWO.Based(Base)))/!(ONE.Based(Base) + TWO.Based(Base) * N);
			P += I;
			N++;
		}
		// Return
		return TWO.Based(Base) * P;
	}
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
	public static int MatchingDigits(in NX A, in NX B){
		int Len = Math.Min(A.Size, B.Size);
		int i   = 0;
		while(A[i] == B[i] && i < Len){i++;}
		return i;
	}
	public static bool IsInteger(in NX Num) => Num.Powr >= 0;
	public static bool IsEven(in NX Num){
		bool Even = Num.NumAtPow(0) % 2 == 0;
		if(Num.Base % 2 == 0 || Num.Powr + Num.Size <= 1){return Even;}
		for(int i = 1; i < Num.Powr + Num.Size; i++){Even ^= Num.NumAtPow(i) % 2 != 0;}
		return Even;
	}
}