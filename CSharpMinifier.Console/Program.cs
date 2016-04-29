using System;
using System.IO;

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
			};


			string filename = args [args.Length - 1];

			if (!System.IO.File.Exists (filename)) {
				System.Console.Error.WriteLine ("Error: File does not exist!");
				return;
			}

			string code = File.ReadAllText (filename);

			CSharpMinifier.Minifier minifier = new CSharpMinifier.Minifier (minifierOptions);
			string output = minifier.MinifyFromString (code);

			if (Array.IndexOf (args, "--skip-compile") >= 0) {
			} else {
				var compileResult = CSharpMinifier.CompileUtils.Compile (code);

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

			System.Console.WriteLine (output);
		}
	}
}
