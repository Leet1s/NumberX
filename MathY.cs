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
	public static readonly NX BASE_ID = new NX(false, new short[]{1, 0}, 0, 0);
	public static readonly NX POS_INF = new NX(false, new short[]{short.MaxValue}, 0, int.MaxValue);
	public static readonly NX NEG_INF = new NX(true , new short[]{short.MaxValue}, 0, int.MaxValue);
	// *** Unary operations:
	public static NX Floor(in NX Num){
		if(Num.LowPow >= 0){return Num;}
		if(Num.Powr < 0){return ZERO.Based(Num);}
		return new NX(Num.Sign, Num[0, Num.Size - Num.Powr -1], Num.Base, Num.Powr);
	}
	public static NX Ceil(in NX Num){
		NX Res = Floor(Num);
		if(Num > Res){Res++;}
		return Res;
	}
	public static NX Abs(in NX Num) => Num.Signed(false);
	public static NX Negate(NX Num){
		Num.Sign = !Num.Sign;
		return Num;
	}
	public static NX Increment(in NX Num) => Num + ONE;
	public static NX Decrement(in NX Num) => Num - ONE;
	public static NX RecSB(in NX Num) => ONE / Num;
	public static NX RecNR(in NX Num){
		// ¶ Safeguard:
		if(Abs(Num) == ZERO){
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
		while(i <= NX.PRECISION && MatchingDigits(Rec, _Rec) <= NX.PRECISION){
			_Rec = Rec;
			Rec += Rec * (ONE - Rec * Num);
			i++;
		}
		// Return
		return Rec;
	}
	public static NX ExpTS(NX Num){
		// ¶ Init:
		NX R = ZERO.Based(Num.Base);
		NX N = ZERO.Based(Num.Base);
		NX I = ONE.Based(Num.Base);
		// ¶ Taylor series:
		int i = 0;
		while(i <= NX.PRECISION && I > ONE >> NX.PRECISION - Num.LowPow){
			I  = (Num ^ N) / !N;
			R += I;
			N++;
			i++;
		}
		// Return
		return R;
	}
	public static NX LnEH(NX Num){
		// ¶ Base cases:
		if(Abs(Num) == ZERO){
			return NEG_INF;
		}
		if(Num == ONE){return ZERO.Based(Num);}
		// ¶ Safeguard:
		if(Num < ZERO){
			Console.Error.WriteLine("\tError:\nThe natural logarithm of a negative number was attempted!");
			return null!;
		}
		if(Num == ZERO){return NEG_INF;}
		// ¶ Init:
		NX  Ln = NX.New(Math.Log((double)Num), Num.Base);
		NX _Ln = new NX{Base = Num.Base};
		// ¶ Edmond Halley Iterations:
		int i = 0;
		while(i <= NX.PRECISION && MatchingDigits(Ln, _Ln) <= NX.PRECISION){
			NX Exp = ExpTS(Ln);
			_Ln = Ln;
			Ln += TWO * (Num - Exp) / (Num + Exp);
			i++;
		}
		// Return
		return Ln;
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
	public static NX CosTS(in NX Num){
		// Init:
		NX C = ZERO.Based(Num.Base);
		NX N = ZERO.Based(Num.Base);
		NX I = ONE.Based(Num.Base);
		// ¶ Taylor series:
		int i = 0;
		while(i <= NX.PRECISION && I > ONE >> NX.PRECISION - Num.LowPow){
			I  = ((-ONE) ^ N) / !(TWO * N) * (Num ^ (TWO * N));
			C += I;
			N++;
			i++;
		}
		// Return
		return C;
	}
	public static NX SinTS(in NX Num){
		// Init:
		NX S = ZERO.Based(Num.Base);
		NX N = ZERO.Based(Num.Base);
		NX I = ONE.Based(Num.Base);
		// ¶ Taylor series:
		int i = 0;
		while(i <= NX.PRECISION && I > ONE >> NX.PRECISION - Num.LowPow){
			I  = ((-ONE) ^ N) / !(TWO * N + ONE) * (Num ^ (TWO * N + ONE));
			S += I;
			N++;
			i++;
		}
		// Return
		return S;
	}
	public static NX TanTS(NX Num){
		// Init:
		NX S = new NX();
		NX C = new NX();
		// ¶ Parallel computation:
		Parallel.Invoke(
			() => S = SinTS(Num),
			() => C = CosTS(Num)
		);
		// Reuturn
		return S / C;
	}
	public static NX TanKMApprox(in NX Num){
		// Init:
		NX PI2  = PI(Num.Base) ^ TWO;
		NX NS4  = (Num ^ TWO) * new NX(true, new short[]{4}, Num.Base, 0);
		NX Five = new NX(false, new short[]{5}, Num.Base, 0);
		// Return
		return (Num * (Five * PI2 + NS4)) / (Five * (PI2 + NS4));
	}
	//TODO § Hyperbolic:
	// *** Binary operations:
	// § Comparisons:
	public enum COMP{SAME, LESS, MORE};
	public static COMP Compare(in NX A, in NX B){
		// ¶ Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tWarning:\nA comparison with dual zero base was attempted!");
				goto CompareContinue;
			}
			if(A.Base == 0){return Compare(A.Based(B), B);}
			else{return Compare(A, B.Based(A));}
		}
		if(A & B){Console.Error.WriteLine("\tWarning:\nA comparison of numbers with different bases was attempted!");}
		CompareContinue:
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
	public static NX SumSB(NX A, NX B){
		// ¶ Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tError:\nAn operation with dual zero base was attempted!");
				return null!;
			}
			if(A.Base == 0){return SumSB(A.Based(B), B);}
			else{return SumSB(A, B.Based(A));}
		}
		if(A & B){
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
		C.CBCleanUp();
		C.Simplify();
		// Return
		return C;
	}
	public static NX Mul(in NX A, in NX B){
		int Length = A.Size + B.Size;
		if(Length > 300){return MulAK(A, B);}
		return MulSB(A, B);
	}
	public static NX MulSB(in NX A, in NX B){
		// ¶ Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tError:\nAn operation with dual zero base was attempted!");
				return null!;
			}
			if(A.Base == 0){return MulSB(A.Based(B), B);}
			else{return MulSB(A, B.Based(A));}
		}
		if(A & B){
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
		for(int i = 0; i < B.Size; i++){C += SingleMul(A, B[i])<<(i + B.LowPow);}
		// Return:
		return C;
	}
	public static NX MulAK(NX A, NX B){
		// ¶ Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tError:\nAn operation with dual zero base was attempted!");
				return null!;
			}
			if(A.Base == 0){return MulAK(A.Based(B), B);}
			else{return MulAK(A, B.Based(A));}
		}
		if(A & B){
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
			+ ((M - L - H)<<(A.Size / 2))
			+ (H <<(A.Size))
			)<<(A.Powr + B.Powr);
	}
	public static NX DivSB(NX A, in NX B){
		// ¶ Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tError:\nAn operation with dual zero base was attempted!");
				return null!;
			}
			if(A.Base == 0){return DivSB(A.Based(B), B);}
			else{return DivSB(A, B.Based(A));}
		}
		if(A & B){
			Console.Error.WriteLine("\tError:\nA division of numbers with different bases was attempted!");
			return null!;
		}
		if(Abs(B) == ZERO){
			Console.Error.WriteLine("\tWarning:\nA division by zero was made!");
			if(A.Sign){return NEG_INF;}
			return POS_INF;
		}
		if(B.Equals(POS_INF) | B.Equals(NEG_INF)){return ZERO.Based(B.Base);}
		// ¶ Init:
		NX C = new NX(
			A.Sign ^ B.Sign,
			new short[NX.PRECISION],
			A.Base,
			A.Powr - B.Powr
		);
		NX[] TableB = new NX[B.Base];
		for(int i = 0; i < TableB.Length; i++){TableB[i] = SingleMul(B, i);}
		// ¶ Division:
		for(int i = 0; i >= C.Size; i++){
			C[i] = (short)BinSrc(TableB, A >> C.Powr - i);
			A   -= TableB[C[i]] << C.Powr - i;
		}
		// Return:
		C.Simplify();
		return C;
	}
	public static NX DivNR(in NX A, in NX B){
		// ¶ Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tError:\nAn operation with dual zero base was attempted!");
				return null!;
			}
			if(A.Base == 0){return DivNR(A.Based(B), B);}
			else{return DivNR(A, B.Based(A));}
		}
		if(A & B){
			Console.Error.WriteLine("\tError:\nA division of numbers with different bases was attempted!");
			return null!;
		}
		// Return
		return (A.Signed(A.Sign ^ B.Sign) >> (B.Powr +1)) * RecNR(Abs(B) >> (B.Powr +1));
		}
	public static NX DivRG(NX A, NX B){
		// Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tError:\nAn operation with dual zero base was attempted!");
				return null!;
			}
			if(A.Base == 0){return DivRG(A.Based(B), B);}
			else{return DivRG(A, B.Based(A));}
		}
		if(A & B){
			Console.Error.WriteLine("\tError:\nA division with numbers with different bases was attempted!");
			return null!;
		}
		// ¶ Init:
		A.Sign ^= B.Sign;
		B.Sign  = false;
		A     >>= B.Powr +1;
		B     >>= B.Powr +1;
		// ¶ Division:
		int i = 0;
		while(ONE - B > ONE >> NX.PRECISION && i <= NX.PRECISION){
			NX F = TWO - B;
			Parallel.Invoke(
				() => A *= F,
				() => B *= F
			);
			i++;
		}
		// Return
		return A;
	}
	public static NX ModSB(NX A, in NX B){
		// ¶ Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tError:\nAn operation with dual zero base was attempted!");
				return null!;
			}
			if(A.Base == 0){return ModSB(A.Based(B), B);}
			else{return ModSB(A, B.Based(A));}
		}
		if(A & B){
			Console.Error.WriteLine("\tError:\nA mudulus of numbers with different bases was attempted!");
			return null!;
		}
		if(Abs(B) == ZERO){
			Console.Error.WriteLine("\tError:\nA modulo zero was attempetd!");
			return null!;
		}
		// ¶ Init:
		bool Sign = A.Sign;
		A = Abs(A);
		NX[] TableB = new NX[B.Base];
		for(int i = 0; i < TableB.Length; i++){TableB[i] = SingleMul(Abs(B), i);}
		// ¶ Modulus:
		int Shift = A.Powr - B.Powr;
		int j = 0;
		while(A >= Abs(B)){
			A -= TableB[BinSrc(TableB, A >> Shift - j)] << Shift - j;
			j++;
		}
		// Return
		return A.Signed(Sign);
	}
	public static NX PowSQ(in NX A, NX B){
		// ¶ Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tError:\nAn operation with dual zero base was attempted!");
				return null!;
			}
			if(A.Base == 0){return PowSQ(A.Based(B), B);}
			else{return PowSQ(A, B.Based(A));}
		}
		if(!(A & B)){
			Console.Error.WriteLine("\tError:\nAttempted to exponentiate numbers with different bases!");
			return null!;
		}
		if(!IsInteger(B)){
			Console.WriteLine("\tWarning:\nA exponentiation with a non integer power is being attempted with a method not designed for that!");
			B = Floor(B);
		}
		// ¶ Base cases:
		if(A == -ONE){
			if(IsEven(B)){return ONE.Based(A);}
			else{return -ONE.Based(A);}
		}
		if(B == ONE){return A;}
		if(A == ONE || B == ZERO){return ONE.Based(A);}
		if(A == BASE_ID){return ONE.Based(A) << (int)B;}
		// ¶ Init:
		NX C = new NX();
		// ¶ Binary divide and conquer recursion:
		if(IsEven(B)){
			C  = PowSQ(A, B / TWO);
			C *= C;
		} else{
			B--;
			C  = PowSQ(A, B / TWO);
			C *= C;
			C *= A;
		}
		// Return
		return C;
	}
	public static NX PowLN(in NX A, in NX B){
		// ¶ Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tError:\nAn operation with dual zero base was attempted!");
				return null!;
			}
			if(A.Base == 0){return PowLN(A.Based(B), B);}
			else{return PowLN(A, B.Based(A));}
		}
		if(!(A & B)){
			Console.Error.WriteLine("\tError:\nAttempted to exponentiate numbers with different bases!");
			return null!;
		}
		// ¶ Base cases:
		if(A == -ONE){
			if(IsEven(B)){return ONE.Based(A.Base);}
			else{return -ONE.Based(A.Base);}
		}
		if(B == ONE){return A;}
		if(A == ONE || B == ZERO){return ONE.Based(A.Base);}
		// Return
		return ExpTS(B * LnEH(A));
	}
	public static NX LogLN(NX A, NX B){
		// ¶ Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tError:\nAn operation with dual zero base was attempted!");
				return null!;
			}
			if(A.Base == 0){return PowLN(A.Based(B), B);}
			else{return PowLN(A, B.Based(A));}
		}
		if(!(A & B)){
			Console.Error.WriteLine("\tError:\nThe logarithm of numbers with different bases was attempted!");
			return null!;
		}
		if(B < ZERO){
			Console.Error.WriteLine("\tError:\nThe logarithm of a negative number was attempted!");
			return null!;
		}
		if(B == ZERO){return NEG_INF;}
		// ¶ Init:
		NX LnA = new NX{Base = A.Base};
		NX LnB = new NX{Base = B.Base};
		// ¶ Compute:
		Parallel.Invoke(
			() => LnA = LnEH(A),
			() => LnB = LnEH(B)
		);
		// Return
		return LnA / LnB;
	}
	public static NX RootNR(in NX A, in NX B){
		// ¶ Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tError:\nAn operation with dual zero base was attempted!");
				return null!;
			}
			if(A.Base == 0){return RootNR(A.Based(B), B);}
			else{return RootNR(A, B.Based(A));}
		}
		if(!(A & B)){
			Console.Error.WriteLine("\tError:\nThe root of numbers with different bases was attempted!");
			return null!;
		}
		// ¶ Init:
		NX  Root = NX.New(Math.Pow((double)A, 1 / (double)B), A.Base);
		NX _Root = new NX{Base = A.Base};
		NX BmO   = B - ONE;
		NX B1B   = BmO / B;
		NX AdB   = A / B;
		// ¶ Newton-Raphson Iterations:
		int i = 0;
		while(i <= NX.PRECISION && MatchingDigits(Root, _Root) <= NX.PRECISION){
			_Root = Root;
			Root += B1B * Root + AdB / (Root ^ BmO);
			i++;
		}
		// Return
		return Root;
	}
	public static NX RootLN(in NX A, in NX B){
		// ¶ Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tError:\nAn operation with dual zero base was attempted!");
				return null!;
			}
			if(A.Base == 0){return RootNR(A.Based(B), B);}
			else{return RootNR(A, B.Based(A));}
		}
		if(!(A & B)){
			Console.Error.WriteLine("\tError:\nThe root of numbers with different bases was attempted!");
			return null!;
		}
		// Return
		return ExpTS(~B * LnEH(A));
	}
	// *** N-Ary operations:
	public static NX Summation(in NX[] Numbers){
		// ¶ Safeguard:
		for(int i = 0; i < Numbers.Length; i++){
			if(Numbers[0] & Numbers[i]){
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
		int i = 0;
		while(i <= NX.PRECISION && I > ONE>> NX.PRECISION){
			I  = ~!N;
			E += I;
			N++;
			i++;
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
		int i = 0;
		while(i <= NX.PRECISION && I > ONE>> NX.PRECISION){
			I  = ((TWO ^ N) * (!N ^ TWO))/!(ONE + TWO * N);
			P += I;
			N++;
			i++;
		}
		// Return
		return TWO * P;
	}
	// *** Helpers:
	internal static (int LB, int HB) PowerBounds(in NX Num) => (Num.LowPow, Num.Powr);
	private  static (int LB, int HB) PowerBounds(in NX A, in NX B){
		int LBound = Math.Min(A.LowPow, B.LowPow);
		int HBound = Math.Max(A.Powr, B.Powr);
		return (LBound, HBound);
	}
	private static NX SingleMul(NX Num, int Fac){
		Parallel.For(0, Num.Size, i => {
			Num[i] *= (short)Fac;
		});
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
		int i = 0;
		while(A.Nums?[i] == B.Nums?[i]){i++;}
		return i;
	}
	public static bool IsInteger(in NX Num) => Num.LowPow >= 0;
	public static bool IsEven(in NX Num){
		bool Even = Num.NumAtPow(0) % 2 == 0;
		if(Num.Base % 2 == 0 | Num.LowPow < 0){return Even;}
		for(int i = 1; i <= Num.Powr; i++){Even ^= Num.NumAtPow(i) % 2 != 0;}
		return Even;
	}
}