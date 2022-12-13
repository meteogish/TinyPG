using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyPG.CodeGenerators.Roslyn;
using TinyPG.Compiler;

namespace TinyPG.UnitTests
{
    [TestClass]
    public class RoslynParserCodeGeneratorTests
    {
        private const string TESTFILESPATH = @"..\..\..\Testfiles\";
        
        [TestMethod]
        public void FirstTest()
        {
            var generator = new RoslynParserGenerator();
            GrammarTree grammarTree = LoadGrammar(TESTFILESPATH + @"simple expression1.tpg");
            Grammar grammar = (Grammar) grammarTree.Eval();

            string parserCsharpCode = generator.Generate(grammar, true);
            
            Assert.IsTrue(true);
        }
        
        private GrammarTree LoadGrammar(string filename)
        {
            string grammarfile = System.IO.File.ReadAllText(filename);
            Scanner scanner = new Scanner();
            Parser parser = new Parser(scanner);
            GrammarTree tree = (GrammarTree)parser.Parse(grammarfile, filename, new GrammarTree());
            return tree;
        }
    }
}
