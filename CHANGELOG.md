# Change Log of NumberX:
## Info:
* Author: Karuljonnai G. M. V. (karuljonnai@gmail.com)
* Current Version: 0.2.1
* [GitHub Repository](https://github.com/Karuljonnai/NumberX/)
---
## 1.1.1
* *: Changed JSON read & write to comply with standards
## 1.1.0
* *: Relaxed B64 syntax; now only the number itself is obligatory in the string builder
* *: Changeable default base for cases were the base is invalid or not specified
* *: Non digit-by-digit calculation methods (such as with NR, RG, EH, and TS iterative algorithms) now only return the accurate digits
* *: Fixed Karatsuba, now fully compatible with the Big Endianness update
* *: BASE_ID's power
* *: CBCleanUp now compatible with Big Endianness
* *: Spelling mistakes & Typos
* *: New name for builders
* +: Fix method
* +: NX cast from short
* +: Root calculation by the exponentiation with the reciprocal of B
* +: Hyperbolic functions with two different algorithms for each one
* +: Fancier exception handling
* +: Min & Max to MathY
* +: Product
* +: N choose K
* +: JSON string representation of NX
## 1.0.0
* !: Changed Nums to Big Endianness representation, as well as everything that it impacts
* *: Fixed several errors in string conversion methods of NX
* *: Parallelized SingleMul
* +: Newton-Raphson algorithm for the nth root
## 0.3.2
* *: Added 'in' to several NX function parameters to make them more memory efficient
* *: ExpSQ -> PowSQ && EToTS -> ExpTS
* *: Iteration limit to E & PI computations
* +: Natural logarithm using the Edmond Halley's method
* +: Log N
* +: Non integer exponentiation
## 0.3.1
* *: Wrong power shift on division
* +: Sine and Tangent (Taylor series)
* +: Modulus
* +: Automatic base 0 reassignment; used for MathY constants
* +: Base checking operators
## 0.3.0
* *: Error in comparison method
* +: Shift operators (Little Endianness based)
* +: Newton-Raphson division and reciprocal
* +: Goldschmidt division algorithm
* +: Floor and ceil
* +: Exponentiation by squaring
* +: Computable E and PI
* +: Cosine (Taylor series)
* +: Tangent (Karuljonnai Màrthos' Approximation)
## 0.2.2
* *: Fixed errors in several MathY methods
* +: Increment and decrement
* +: Integer verification
## 0.2.1
* *: String method now supports bases from 2 to 64
* +: Positive and negative infinity to MathY as basic numbers
## 0.2.0
* *: .Len() now just .Size
* *: Basic numbers on MathY now read only
* +: Division method to MathY and operator to NX
* +: Binary search algorithm to help division on MathY
* +: IsEven method to MathY
## 0.1.3
* *: CBCleanUp fix (wrong indexes)
* *: Transferred Based from MathY to NX
## 0.1.2
* +: CBCleanUp functionality
* +: Indexer using index
## 0.1.1
* *: Parallelized Karatsuba multiplication
## 0.1.0
* +: Karatsuba multiplication algorithm
## 0.0.5
* 0: Added licensing
## 0.0.4
* *: Changed CBCleanUp from MathY to NX
* +: Indexer to NX for easier manipulation of Nums values
* -: Unnecessary redundancy from NumAtPowr
## 0.0.3
* *: Better Error syntax
* +: Simplify added to NX
* +: Addition added to MathY
* +: Negate operator
## 0.0.2
* *: Increased base range, no longer limited from 2 to 62, now from 2 to 255
* +: Comparisons to MathY
## 0.0.1
* +: B62 String format and syntax;
* +: NX constructors (string, double, long)
* +: Unary operations to MathY (Abs, Negate)
