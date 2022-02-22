# NumberX

NumberX is a C# library for storing and manipulating numbers with arbitrary precision and in different bases.
---
## String Syntax

To use the NX builder that uses a string to correctly build a number at the desired base, the string needs to be formatted correctly:

This is an example of a formatted string using all available syntax, which will be explained shortly: >+3.141592*a^+0

* (Optional)The first element is the endianness indicator (< or >); in the example '>' is used, indicating big-endianness, which is how we usually write numbers.
* (Optional)The second is the sign of the number(+ or -); if it is omitted,
the number will be considered positive.
* (Obligatory)The third is the number itself, a full stop or a comma may be used to indicate the floating point separator; the symbols allowed (B64) to represent the numbers are, in sequence: 0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ@#.
* (Optional)The forth part is the base with '*' as an indicator, followed by a single B62 character, if not specified, or an invalid base is used, it will use the default base (which is 10, but may be changed by the user)
* (Optional)The final part represents the power with '^' as an indicator, it works just like the number, with the exeption of the floating point, which is forbidden.
