# NX
## Global
- PRECISION:
  - Used for operations in MathY that may not have a predictable bound, as it is the case with the inverse operations.
  - Division is a example, as it may produce a infinitely repeating 'decimal'.
  - Not necessarily how many digits beyond the floating point will be calculated, but the total amount of digits starting from the most significant digit, which may be zero in some instances.
- DFLT_BASE:
  - Default base.
  - In case a base is not explicitly provided, it will use the default, which is 10, but may changed.
- B64:
  - Represents all the 64 characters used in some representations of numbers in different bases.
  - Starts with 0-9, then uses the latin alphabet (a-z), as well as the capitalized version (A-Z) to represent higher values, and, finally, @# for 63 and 64, respectively.
- B64Pat:
  - Regular expression text for the B64 syntax:
    - Endianness indicator: '<' or '>'(default)
    - Sign: '-' or '+'(default)
    - Digits: "xxx.xxx" (floating point is optional)
    - Base: '*' and a single B64 character (if omitted or invalid, DFLT_BASE will be used)
    - Power: '^' and a signed B64 style integer (example: -1234)
- B64RE:
  - Regular Expression instance for B64 strings
## Attributes:
- Sign:
  - Boolean, false := positive; true := negative
- Nums:
  - Short for Numbers
  - Signed 16 Bit Integer Array
  - Big-Endianness format (Digit at position 0 is the most significant)
- Base:
  - Unsigned Byte
  - Represents the base in which the number is stored at
- Powr:
  - Short for Power
  - Signed 32 Bit Integer
  - Much like scientific notation, it represents the power of the most significant digit
# MathY
