# Change Log of NumberX:
## Info:
* Author: Karuljonnai G. M. V. (karuljonnai@gmail.com)
* Current Version: 0.2.1
* [GitHub Repository](https://github.com/Karuljonnai/NumberX/)
---
## 1.0.0
* !: Changed Nums to Big Endianness representation, as well as everything that it impacts
* *: Fixed several errors in string conversion methods of NX
* *: Parallelized SingleMul
* +: Newton-Raphson algorithm for the nth root
## 0.3.2
* *: Added 'in' to several NX function parameters to make them more memory efficient
* *: ExpSQ -> PowSQ && EToTS -> ExpTS
* *: Iteration limit to E & PI computations
* +: Narutal logarithm using the Edmond Halley's method
* +: Log N
* +: Non integer exponentiation
## 0.3.1
* *: Wrong power shift on division
* +: Sine and Tangent (Taylor series)
* +: Modulus
* +: Automatic base 0 reasignment; used for MathY constants
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
* *: Basic numbers on Mathy now read only
* +: Division method to MathY and operator to NX
* +: Binary search algorithm to help division on MathY
* +: IsEven method to MathY
## 0.1.3
* *: CBCleanUp fix (wrong indexes)
* *: Transfered Based from MathY to NX
## 0.1.2
* +: CBCleanUp functionality
* +: Indexer using index
## 0.1.1
* *: Parallelized Karatsuka multiplication
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
