using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using TinyPG.CodeGenerators;
using TinyPG.Compiler;
namespace TinyPG.SourceGenerators
{
	[Generator]
	public class GrammarSourceGenerator : ISourceGenerator
	{
		public void Initialize(GeneratorInitializationContext context)
		{
		}

		public void Execute(GeneratorExecutionContext context)
		{
#if DEBUG
			if (!Debugger.IsAttached)
			{
				Debugger.Launch();
			}
#endif
			// if (context.AdditionalFiles.Length == 0)
			// {
			// 	// context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("008", "No files to be get", "", "", DiagnosticSeverity.Error, false), Location.None, "No files path"));
			// 	// context.AddSource($"NoSource", SourceText.From("Heelo not addtional fiels"));
			// }
			var mainSyntaxTree = context.Compilation.SyntaxTrees
				.First(x => x.HasCompilationUnitRoot);

			var directory = Path.GetDirectoryName(mainSyntaxTree.FilePath);
			Log.Print($"AppContext.BaseDir: {System.AppContext.BaseDirectory}");
			Log.Print($"Dir.CurrDir: {Directory.GetCurrentDirectory()}");
			Log.Print($"Path: {directory}");

			// string bins = Path.Combine(Directory.GetCurrentDirectory(), "bin", "Debug", "net6.0", "Templates", "C#");
			string bins = "C:/WorkVistex/Repositories/github/TinyPG/src/TinyPG.GrammarCompiler/Templates/C#";
			
			Log.Print($"bins: {bins}, exists: {Directory.Exists(bins)}");

			foreach (AdditionalText additionalFile in context.AdditionalFiles)
			{
				try
				{
					Log.Print($"Addtional file: {additionalFile.Path}");
					GrammarTree gt = LoadGrammar(additionalFile.GetText().ToString());
					Grammar grammar = (Grammar)gt.Eval();
					grammar.TemplatePathCustom = bins;
					grammar.Log = Log.Print;
					grammar.Preprocess();
					
					List<string> sources = new List<string>();
					string language = grammar.Directives["TinyPG"]["Language"];
					ICodeGenerator generator;
					Log.Print($"Got grammar: " + grammar.Directives.Count);
					foreach (Directive d in grammar.Directives)
					{
						Log.Print($"{d.Name} : {string.Join(", ",d.Values)}");
						generator = CodeGeneratorFactory.CreateGenerator(d.Name, language);
						if (generator == null)
						{
							Log.Print($"Generator is null");
						}
						
						if (generator != null && d.ContainsKey("FileName"))
						{
							generator.FileName = d["FileName"];
						}

						if (generator != null && d["Generate"].ToLower() == "true")
						{
							Log.Print("Before generating for : " + d.Name);
							string sourceText = generator.Generate(grammar, false);
							Log.Print($"Got source text for '{d.Name}': ");
							context.AddSource(d.Name, sourceText);
						}
					}
				}
				catch (Exception e)
				{
					Log.Print(e.Message);
				}
				finally
				{
					Log.FlushLogs(context);
				}
			}
		}

		private GrammarTree LoadGrammar(string content)
		{
			string grammarfile = content;
			Scanner scanner = new Scanner();
			Parser parser = new Parser(scanner);
			GrammarTree tree = (GrammarTree)parser.Parse(grammarfile, "SourceGenerator", new GrammarTree());
			return tree;
		}
	}
#if DEBUG
	internal static class Log
	{
		public static List<string> Logs { get; } = new();

		public static void Print(string msg) => Logs.Add("//\t" + msg);

// More print methods ...

		public static void FlushLogs(GeneratorExecutionContext context)
		{
			context.AddSource($"logs.g.cs", SourceText.From(string.Join("\n", Logs), Encoding.UTF8));
		}
	}
#endif
}
