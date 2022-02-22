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

using System.Runtime.CompilerServices;

namespace NumberX;

public static class MathY{
	// *** Basic numbers:
	public static readonly NX ZERO    = new NX(false, new short[]{0}, 0, 0);
	public static readonly NX ONE     = new NX(false, new short[]{1}, 0, 0);
	public static readonly NX TWO     = new NX(false, new short[]{2}, 0, 0);
	public static readonly NX BASE_ID = new NX(false, new short[]{1}, 0, 1);
	public static readonly NX POS_INF = new NX(false, new short[]{short.MaxValue}, 0, int.MaxValue);
	public static readonly NX NEG_INF = new NX(true , new short[]{short.MaxValue}, 0, int.MaxValue);
	// *** Unary operations:
	public static NX Floor(in NX Num){
		if(Num.LowPow >= 0){return Num;}
		if(Num.Powr < 0){return ZERO.InBase(Num);}
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
		if(Num.Equals(POS_INF) || Num.Equals(NEG_INF)){return ZERO.InBase(Num.Base);}
		// ¶ Init:
		NX  Rec = NX.FromDouble(1 / (double) Num, Num.Base);
		NX _Rec = new NX{Base = Num.Base};
		// ¶ Newton-Raphson Iterations:
		int i = 0;
		while(i <= NX.PRECISION && MatchingDigits(Rec, _Rec) <= NX.PRECISION){
			_Rec = Rec;
			Rec += Rec * (ONE - Rec * Num);
			i++;
		}
		// Return
		Rec.Fix();
		return Rec.Cut();
	}
	public static NX ExpTS(NX Num){
		// ¶ Init:
		NX R = ZERO.InBase(Num.Base);
		NX N = ZERO.InBase(Num.Base);
		NX I = ONE.InBase(Num.Base);
		// ¶ Taylor series:
		int i = 0;
		while(i <= NX.PRECISION && I > ONE >> NX.PRECISION - Num.LowPow){
			I  = (Num ^ N) / !N;
			R += I;
			N++;
			i++;
		}
		// Return
		R.Fix();
		return R.Cut();
	}
	public static NX LnEH(NX Num){
		// ¶ Base cases:
		if(Abs(Num) == ZERO){
			return NEG_INF;
		}
		if(Num == ONE){return ZERO.InBase(Num);}
		// ¶ Safeguard:
		if(Num < ZERO){
			Console.Error.WriteLine("\tError:\nThe natural logarithm of a negative number was attempted!");
			return null!;
		}
		if(Num == ZERO){return NEG_INF;}
		// ¶ Init:
		NX  Ln = NX.FromDouble(Math.Log((double)Num), Num.Base);
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
		Ln.Fix();
		return Ln.Cut();
	}
	public static NX FacSB(NX Num){
		// ¶ Safeguard:
		if(!IsInteger(Num)){
			Console.WriteLine("\tWarning:\nThe factorial of a non-integer number is being attempted using the school-book method, which is meant only for integers!");
			Num = Floor(Num);
		}
		// ¶ Init:
		NX Fac = ONE.InBase(Num.Base);
		while(Num > ONE){
			Fac *= Num;
			Num.Simplify();
			Num--;
		}
		// Return:
		Fac.Fix();
		return Fac;
	}
	// § Trigonometric:
	public static NX CosTS(in NX Num){
		// ¶ Init:
		NX C = ZERO.InBase(Num.Base);
		NX N = ZERO.InBase(Num.Base);
		NX I = ONE.InBase(Num.Base);
		// ¶ Taylor series:
		int i = 0;
		while(i <= NX.PRECISION && I > ONE >> NX.PRECISION - Num.LowPow){
			I  = ((-ONE) ^ N) / !(TWO * N) * (Num ^ (TWO * N));
			C += I;
			N++;
			i++;
		}
		// Return
		C.Fix();
		return C.Cut();
	}
	public static NX SinTS(in NX Num){
		// ¶ Init:
		NX S = ZERO.InBase(Num.Base);
		NX N = ZERO.InBase(Num.Base);
		NX I = ONE.InBase(Num.Base);
		// ¶ Taylor series:
		int i = 0;
		while(i <= NX.PRECISION && I > ONE >> NX.PRECISION - Num.LowPow){
			I  = ((-ONE) ^ N) / !(TWO * N + ONE) * (Num ^ (TWO * N + ONE));
			S += I;
			N++;
			i++;
		}
		// Return
		S.Fix();
		return S.Cut();
	}
	public static NX TanTS(NX Num){
		// ¶ Init:
		NX S = new NX();
		NX C = new NX();
		// ¶ Parallel computation:
		Parallel.Invoke(
			() => S = SinTS(Num),
			() => C = CosTS(Num)
		);
		// Return
		return S / C;
	}
	public static NX TanKMApprox(in NX Num){
		// ¶ Init:
		NX PI2  = PI(Num.Base) ^ TWO;
		NX NS4  = (Num ^ TWO) * new NX(true, new short[]{4}, Num.Base, 0);
		NX Five = new NX(false, new short[]{5}, Num.Base, 0);
		// Return
		return (Num * (Five * PI2 + NS4)) / (Five * (PI2 + NS4));
	}
	// § Hyperbolic:
	public static NX CoshTS(in NX Num){
		// ¶ Init:
		NX C = ZERO.InBase(Num.Base);
		NX N = ZERO.InBase(Num.Base);
		NX I = ONE.InBase(Num.Base);
		// ¶ Taylor series:
		int i = 0;
		while(i <= NX.PRECISION && I > ONE >> NX.PRECISION - Num.LowPow){
			I  = (Num ^ N) / !N;
			C += I;
			N += TWO;
			i++;
		}
		// Return
		C.Fix();
		return C.Cut();
	}
	public static NX CoshEX(in NX Num){
		// ¶ Init:
		NX EX = ExpTS(Num);
		// Return
		return (EX + ~EX) / TWO;
	}
	public static NX SinhTS(in NX Num){
		// ¶ Init:
		NX C = ZERO.InBase(Num.Base);
		NX N = ONE.InBase(Num.Base);
		NX I = ONE.InBase(Num.Base);
		// ¶ Taylor series:
		int i = 0;
		while(i <= NX.PRECISION && I > ONE >> NX.PRECISION - Num.LowPow){
			I  = (Num ^ N) / !N;
			C += I;
			N += TWO;
			i++;
		}
		// Return
		C.Fix();
		return C.Cut();
	}
	public static NX SinhEX(in NX Num){
		// ¶ Init:
		NX EX = ExpTS(Num);
		// Return
		return (EX - ~EX) / TWO;
	}
	public static NX TanhTS(NX Num){
		// ¶ Init:
		NX S = new NX();
		NX C = new NX();
		// ¶ Parallel computation:
		Parallel.Invoke(
			() => S = SinhTS(Num),
			() => C = CoshTS(Num)
		);
		// Return
		return S / C;
	}
	public static NX TanhEX(in NX Num){
		// ¶ Init:
		NX EX = ExpTS(TWO * Num);
		// Return
		return (EX + ONE) / (EX - ONE);
	}
	// *** Binary operations:
	// § Comparisons:
	public enum COMP{SAME, LESS, MORE};
	public static COMP Compare(in NX A, in NX B){
		// ¶ Safeguard:
		if(A | B){
			if(!(A & B)){
				Console.Error.WriteLine("\tWarning:\nA comparison with dual zero base is being attempted!");
				goto CompareContinue;
			}
			if(A.Base == 0){return Compare(A.InBase(B), B);}
			else{return Compare(A, B.InBase(A));}
		}
		if(A & B){Console.Error.WriteLine("\tWarning:\nA comparison of numbers with different bases is being attempted!");}
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
	public static NX Max(in NX A, in NX B) => A > B ? A : B;
	public static NX Min(in NX A, in NX B) => A < B ? A : B;
	// § Arithmetic:
	public static NX SumSB(NX A, NX B){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(A, B, (a, b) => SumSB(a, b));
			if(R){return R!;}
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
		// Return
		C.Fix();
		return C;
	}
	public static NX Mul(in NX A, in NX B){
		int Length = A.Size + B.Size;
		if(Length > 300){return MulAK(A, B);}
		return MulSB(A, B);
	}
	public static NX MulSB(in NX A, in NX B){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(A, B, (a, b) => MulSB(a, b));
			if(R){return R!;}
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
		C.Fix();
		return C;
	}
	public static NX MulAK(NX A, NX B){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(A, B, (a, b) => MulAK(a, b));
			if(R){return R!;}
		}
		// ¶ Init:
		if(A.LowPow < 0 | B.LowPow < 0){
			int ShiftA = MakeInt(ref A);
			int ShiftB = MakeInt(ref B);
			return MulAK(A, B)<<(ShiftA + ShiftB);
		}
		MatchLength(ref A, ref B);
		// ¶ Base Case:
		if(A.Size == 1){return SingleMul(A, B[0]);}
		// ¶ Init:
		int HalfLen = A.Size / 2;
		(NX A_H, NX A_L) = SplitHalf(A, HalfLen);
		(NX B_H, NX B_L) = SplitHalf(B, HalfLen);
		// ¶ Recursive calls in parallel:
		NX L = new NX();
		NX M = new NX();
		NX H = new NX();
		Parallel.Invoke(
			() => {L = Mul(A_L,  B_L);},
			() => {M = Mul(A_L + B_H, A_H + B_L);},
			() => {H = Mul(A_H,  B_H);}
		);
		// Return:
		return (L + ((M - L - H)<<(HalfLen)) + (H <<(HalfLen * 2)));
	}
	public static NX DivSB(NX A, in NX B){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(A, B, (a, b) => DivSB(a, b));
			if(R){return R!;}
		}
		if(Abs(B) == ZERO){
			Console.Error.WriteLine("\tWarning:\nA division by zero was made!");
			if(A.Sign){return NEG_INF;}
			return POS_INF;
		}
		if(B.Equals(POS_INF) | B.Equals(NEG_INF)){return ZERO.InBase(B.Base);}
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
			C[i] = (short)BinSrc(A >> C.Powr - i, TableB);
			A   -= TableB[C[i]] << C.Powr - i;
		}
		// Return:
		C.Fix();
		return C;
	}
	public static NX DivNR(in NX A, in NX B){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(A, B, (a, b) => DivNR(a, b));
			if(R){return R!;}
		}
		// Return
		return (A.Signed(A.Sign ^ B.Sign) >> (B.Powr +1)) * RecNR(Abs(B) >> (B.Powr +1));
	}
	public static NX DivRG(NX A, NX B){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(A, B, (a, b) => DivRG(a, b));
			if(R){return R!;}
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
		A.Fix();
		return A.Cut();
	}
	public static NX ModSB(NX A, in NX B){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(A, B, (a, b) => ModSB(a, b));
			if(R){return R!;}
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
			A -= TableB[BinSrc(A >> Shift - j, TableB)] << Shift - j;
			j++;
		}
		// Return
		A.Fix();
		return A.Signed(Sign);
	}
	public static NX PowSQ(in NX A, NX B){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(A, B, (a, b) => PowSQ(a, b));
			if(R){return R!;}
		}
		if(!IsInteger(B)){
			Console.WriteLine("\tWarning:\nA exponentiation with a non integer power is being attempted with a method not designed for that!");
			B = Floor(B);
		}
		// ¶ Base cases:
		if(A == -ONE){
			if(IsEven(B)){return ONE.InBase(A);}
			else{return -ONE.InBase(A);}
		}
		if(B == ONE){return A;}
		if(A == ONE || B == ZERO){return ONE.InBase(A);}
		if(A == BASE_ID){return ONE.InBase(A) << (int)B;}
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
		C.Fix();
		return C;
	}
	public static NX PowLn(in NX A, in NX B){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(A, B, (a, b) => PowLn(a, b));
			if(R){return R!;}
		}
		// ¶ Base cases:
		if(A == -ONE){
			if(IsEven(B)){return ONE.InBase(A.Base);}
			else{return -ONE.InBase(A.Base);}
		}
		if(B == ONE){return A;}
		if(A == ONE || B == ZERO){return ONE.InBase(A.Base);}
		// Return
		return ExpTS(B * LnEH(A));
	}
	public static NX LogLn(NX A, NX B){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(A, B, (a, b) => LogLn(a, b));
			if(R){return R!;}
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
		{
			NX? R = CheckBases(A, B, (a, b) => RootNR(a, b));
			if(R){return R!;}
		}
		// ¶ Init:
		NX  Root = NX.FromDouble(Math.Pow((double)A, 1 / (double)B), A.Base);
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
		Root.Fix();
		return Root.Cut();
	}
	public static NX RootLn(in NX A, in NX B){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(A, B, (a, b) => RootLn(a, b));
			if(R){return R!;}
		}
		// Return
		return ExpTS(~B * LnEH(A));
	}
	public static NX RootPow(in NX A, in NX B){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(A, B, (a, b) => RootPow(a, b));
			if(R){return R!;}
		}
		// Return
		return PowLn(A, ~B);
	}
	// TODO
	/*
	public static NX RootDD(in NX A, NX B){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(A, B, (a, b) => RootDD(a, b));
			if(R){return R!;}
		}
		if(!IsInteger(B)){
			Console.WriteLine("\tWarning:\nA root with a non integer radical is being attempted with a method not designed for that!");
			B = Floor(B);
		}
		if(IsEven(B) & A.Sign){
			Console.Error.WriteLine("\tError:\nThe root of a negative number with an even radical was attempted!");
			return null!;
		}
		if(B == ONE){return A;}
		if(B.Sign){return ~RootDD(A, B.Signed(false));}
		// ¶ Init:
		int BInt = (int)B;
		NX C = new NX(
			false,
			new short[NX.PRECISION],
			A.Base,
			0
		);
		NX[] ASplit    = SplitBy(A, BInt);
		NX   Partial   = ZERO.InBase(A);
		NX   Remainder = ASplit[0];
		// ¶ Digits computation:
		for(int i = 0; i < NX.PRECISION; i++){
			short Temp = RootBinSrc();
		}
		// Return
		C.Powr = A.Powr / BInt;
		return C;
		// Root Binary Search:
		short RootBinSrc(){
			
		}
	}
	*/
	public static NX Choose(NX N, NX K){
		// ¶ Safeguard:
		{
			NX? R = CheckBases(N, K, (a, b) => Choose(a, b));
			if(R){return R!;}
		}
		MakeInt(ref N);
		MakeInt(ref K);
		// ¶ Init:
		NX C = ONE;
		NX D = N - K;
		NX M = Max(K, D);
		// ¶ Computation:
		NX i = N;
		for(; i > M; i--){C *= i;}
		i = Min(K, D);
		for(; i > ONE; i--){C /= i;}
		// Return
		return C;
	}
	// *** N-Ary operations:
	public static NX Summation(in NX[] Numbers){
		// ¶ Safeguard:
		for(int i = 0; i < Numbers.Length; i++){
			if(Numbers[0] & Numbers[i]){
				Console.Error.WriteLine("\tError:\nThe summation of numbers with different bases was attempted!");
				throw new CollidingBases();
			}
		}
		// ¶ Init:
		NX Total = new NX{Base = Numbers[0].Base};
		// ¶ Summation:
		foreach(NX i in Numbers){Total += i;}
		// Return:
		Total.Simplify();
		return Total;
	}
	public static NX Product(in NX[] Numbers){
		// ¶ Safeguard:
		for(int i = 0; i < Numbers.Length; i++){
			if(Numbers[0] & Numbers[i]){
				Console.Error.WriteLine("\tError:\nThe product of numbers with different bases was attempted!");
				throw new CollidingBases();
			}
		}
		// ¶ Init:
		NX Total = new NX{Base = Numbers[0].Base};
		// ¶ Summation:
		foreach(NX i in Numbers){Total *= i;}
		// Return:
		Total.Simplify();
		return Total;
	}
	// *** Constants:
	public static NX  E(byte Base = 0){
		// ¶ Safeguard:
		if(Base < 2){Base = NX.DFLT_BASE;}
		// ¶ Init:
		NX E = ONE.InBase(Base);
		NX N = ONE.InBase(Base);
		NX I = ONE.InBase(Base);
		// ¶ Iterations:
		int i = 0;
		while(i <= NX.PRECISION & I > ONE >> NX.PRECISION){
			I  = ~!N;
			E += I;
			N++;
			i++;
		}
		// Return
		return E.Cut();
	}
	public static NX PI(byte Base = 0){
		// ¶ Safeguard:
		if(Base < 2){Base = NX.DFLT_BASE;}
		// ¶ Init:
		NX P = ONE.InBase(Base);
		NX N = ONE.InBase(Base);
		NX I = ONE.InBase(Base);
		// ¶ Iterations:
		int i = 0;
		while(i <= NX.PRECISION & I > ONE >> NX.PRECISION){
			I  = ((TWO ^ N) * (!N ^ TWO))/!(ONE + TWO * N);
			P += I;
			N++;
			i++;
		}
		// Return
		return (TWO * P).Cut();
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
	private static int BinSrc(in NX Target, in NX[] Table){
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
		int D = A.Size - B.Size;
		if(D > 0){
			B.Nums  = B[-D .. ^0];
			B.Powr += D;
		}
		if(D < 0){
			A.Nums  = A[ D .. ^0];
			A.Powr -= D;
		}
	}
	private static (NX, NX) SplitHalf(NX Num, in int Mid){
		NX  HH  = new NX(
			Num.Sign,
			Num[0 .. Mid],
			Num.Base,
			Mid -1
		);
		NX  LH  = new NX(
			Num.Sign,
			Num[Mid .. ^0],
			Num.Base,
			Mid - Num.Size % 2
		);
		return (HH, LH);
	}
	private static int MakeInt(ref NX Num){
		if(IsInteger(Num)){return 0;}
		int Temp  = Num.LowPow;
		Num.Powr -= Temp;
		return Temp;
	}
	private static NX[] SplitBy(in NX Num, in int Size){
		// ¶ Init:
		NX[] Arr    = new NX[NX.PRECISION];
		int  Offset = Num.Powr % Size - Size +1;
		for(int i = 0; i < Arr.Length; i++){
			Arr[i].Base = Num.Base;
			Arr[i].Powr = Size -1;
		}
		// ¶ Splitting:
		for(int i = 0, j = 0; i < Arr.Length; i++, j += Size){
			Arr[i].Nums = new short[Size];
			for(int k = 0; k < Size; k++){
				try{Arr[i][k] = Num[j + k + Offset];}
				catch{continue;}
			}
		}
		// Return
		return Arr;
	}
	// *** Miscellaneous:
	public static int  MatchingDigits(in NX A, in NX B){
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
	// *** Checkers:
	private static NX? CheckBases(in NX A, in NX B, Func<NX, NX, NX> Caller, [CallerMemberName] string CallerName = ""){
		if(A | B){
			if(A & B){
				Console.Error.WriteLine($"\tError:\nDual Base Zero Exception @ {CallerName}");
				throw new DualBaseZero();
			}
			if(A.Base == 0){return Caller(A.InBase(B), B);}
			else{return Caller(A, B.InBase(A));}
		}
		if(!(A & B)){
			Console.Error.WriteLine($"\tError:\nColliding Bases Exception @ {CallerName}");
			throw new CollidingBases();
		}
		return null;
	}
}