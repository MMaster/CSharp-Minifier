using System;
using System.IO;
using System.Collections.Generic;

namespace CSharpMinifier.Console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length < 1) {
				System.Console.Error.WriteLine ("Usage: CSharpMinifier [options] [filename]");
				return;
			}
				
			int lineLength = 80;

			if (Array.IndexOf(args, "--line-length") >= 0) {
				
				int index = Array.IndexOf(args, "--line-length") + 1;
				if (index < args.Length) {					
					lineLength = int.Parse (args [index]);
				} else {
					System.Console.Error.WriteLine ("Error: Invalid line length!");
					return;
				}
			}

			var minifierOptions = new CSharpMinifier.MinifierOptions
            {
                LocalVarsCompressing = true,
                MembersCompressing = true,
                TypesCompressing = true,
                SpacesRemoving = true,
                RegionsRemoving = true,
                CommentsRemoving = true,
                MiscCompressing = true,
                ConsoleApp = true,
                NamespacesRemoving = false,
                LineLength = 130,
                ToStringMethodsRemoving = false,
                PublicCompressing = true,
                EnumToIntConversion = false,
                UselessMembersCompressing = true
            };
			/*{
				LocalVarsCompressing = Array.IndexOf(args, "--locals") >= 0,
				MembersCompressing = Array.IndexOf(args, "--members") >= 0,
				TypesCompressing = Array.IndexOf(args, "--types") >= 0,
				SpacesRemoving = Array.IndexOf(args, "--spaces") >= 0,
				RegionsRemoving = Array.IndexOf(args, "--regions") >= 0,
				CommentsRemoving = Array.IndexOf(args, "--comments") >= 0,
				MiscCompressing = Array.IndexOf(args, "--misc") >= 0,
				ConsoleApp = Array.IndexOf(args, "--console") >= 0,
				NamespacesRemoving = Array.IndexOf(args, "--namespaces") >= 0,
				LineLength = lineLength,
				ToStringMethodsRemoving = Array.IndexOf(args, "--to-string-methods") >= 0,
				PublicCompressing = Array.IndexOf(args, "--public") >= 0,
				EnumToIntConversion = Array.IndexOf(args, "--enum-to-int") >= 0,
				UselessMembersCompressing = Array.IndexOf(args, "--useless-members") >= 0
			};*/


			string filename = args [args.Length - 1];

			if (!System.IO.File.Exists (filename)) {
				System.Console.Error.WriteLine ("Error: File does not exist!");
				return;
			}

			string origcode = File.ReadAllText (filename,System.Text.Encoding.Unicode);
            string[] lines = origcode.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            bool skipping = false;
            List<string> nonMinified = new List<string>();
            int nonMiniIdx = 0;
            string code = "";
            string[] noMiniSymbols = null;
            string preprocessedCode = "";

            foreach (string line in lines) {
                string l = line.Trim();

                if (noMiniSymbols == null && l.StartsWith("// NO MINIFY SYMBOLS: ")) {
                    noMiniSymbols = l.Substring(22).Split(' ');
                    continue;
                }

                if (l.StartsWith("// MINIFY INSERT: ")) {
                    string fname = l.Substring(18);
                    string insCode = File.ReadAllText(fname, System.Text.Encoding.Unicode);
                    int afterNsIdx = insCode.IndexOf("{");
                    if (afterNsIdx < 0) {
                        System.Console.Error.Write("ERROR: Cannot find { in inserted file: " + fname);
                        continue;
                    }
                    insCode = insCode.Substring(afterNsIdx + 1);

                    int lastBrackIdx = insCode.LastIndexOf("}");
                    if (lastBrackIdx < 0) {
                        System.Console.Error.Write("ERROR: Cannot find } in inserted file: " + fname);
                        continue;
                    }

                    insCode = insCode.Substring(0, lastBrackIdx);
                    preprocessedCode += insCode + "\n";
                    continue;
                }

                preprocessedCode += l + "\n";
            }

            lines = preprocessedCode.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (string line in lines) {
                string l = line;

                if (l == "// NO MINIFY }") {
                    nonMiniIdx++;
                    skipping = false;
                    continue;
                }

                if (skipping) {
                    nonMinified[nonMiniIdx] += l + "\n";
                    continue;
                }

                if (l == "// NO MINIFY {") {
                    code += "#define MinifierPlaceholder" + nonMiniIdx + "E\n";
                    nonMinified.Add("");
                    skipping = true;
                    continue;
                }
                code += l + "\n";
            }

            CSharpMinifier.Minifier minifier = new CSharpMinifier.Minifier(minifierOptions, noMiniSymbols);
			string output = minifier.MinifyFromString (code);

            output = output.Replace(">==", "> ==");

            nonMiniIdx = 0;
            foreach (string nonMini in nonMinified) {
                output = output.Replace("#define MinifierPlaceholder" + nonMiniIdx + "E", "\n" + nonMini);
                nonMiniIdx++;
            }

			if (Array.IndexOf (args, "--skip-compile") >= 0) {
			} else {
				var compileResult = CSharpMinifier.CompileUtils.Compile (output);

				if (!compileResult.Errors.HasErrors) {
					System.Console.Error.WriteLine ("Compiled successfully!");
				} else {
					for (int i = 0; i < compileResult.Errors.Count; i++) {
						var error = compileResult.Errors [i];
						System.Console.Error.WriteLine ("Line: " + error.Line + ", Column: " + error.Column + " :: " + error.ErrorText);
					}

					System.Console.Error.WriteLine ("Error: Compile failed!  Minified code could still be valid.");
				}
			}

            System.IO.File.WriteAllText("script.cs", output, System.Text.Encoding.UTF8);
			
		}
	}
}
