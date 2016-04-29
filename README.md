CSharp-Minifier
===============

Library for C# code minification based on NRefactory. That is lib for spaces, line breaks, comments removing, reduction of identifiers name length and so on in C# code.

This lib primarily developed for code minification in quines (for my [Freaky-Sources](https://github.com/KvanTTT/Freaky-Sources) project).

Implemented features:

* Identifiers compression (ignored names are supported too):
  * Local vars
  * Type members
  * Types
* Misc minifications:
  * private words removing
  * namespaces words removing
  * vars initialization expression compressing:
    * List<byte> a = new List<byte>() => var a = new List<byte>()
    * var a = new b() => b a = new b()
  * Curly spaces with one expression inside removing ```if (a) { b; } => if (a) b;```
  * boolean expressions optimization: ```if (a == true) => if (a); if (a == false) => if (!a)```
  * Comments and regions removing
* Whitespaces chars removing with maximum string length limit.

Tests and GUI application are included. 
 
Modified for use with Space Engineers in-game scripts. 
* Console program includes following features:
  * Ability to modify behavior using comments in code
    * Define ignored symbols ```// NO MINIFY SYMBOLS: symbol1 symbol2 symbol2```
    * Insert classes from file in place of comment ```// MINIFY INSERT: filename.cs```
    * Disable minify for area of code:
       ```// NO MINIFY {```
      ```code();```
      ```// NO MINIFY }```
  * Unicode input
  * Output to script.cs file in unicode

