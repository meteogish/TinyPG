using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Grammars.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace TinyPG.Editor.Desktop.Avalonia.Grammars
{
    public class TPGGrammarRegistryOptions : IRegistryOptions
    {
        private IRawGrammar RawGrammar { get; }
        private GrammarDefinition GrammarDefinition { get; set; }
        public RegistryOptions Inner { get; }

        const string GrammarPrefix = "TinyPG.Editor.Desktop.Avalonia.Grammars.tpg.";
        const string ThemesPrefix = "TextMateSharp.Grammars.Resources.Themes.";

        public TPGGrammarRegistryOptions(RegistryOptions inner)
        {
            LoadGrammarDefinition();
            RawGrammar = LoadRawTPGGrammar();
            Inner = inner;
        }

        private void LoadGrammarDefinition()
        {
            var serializer = new JsonSerializer();

            using (Stream stream = TryOpenEmbeddedResourceStream("package.json"))
            using (StreamReader reader = new StreamReader(stream))
            using (JsonTextReader jsonTextReader = new JsonTextReader(reader))
            {
                GrammarDefinition = serializer.Deserialize<GrammarDefinition>(jsonTextReader);
            }
        }

        public IRawTheme GetDefaultTheme()
        {
            return Inner.GetDefaultTheme();
        }

        public IRawGrammar GetGrammar(string scopeName)
        {
            return RawGrammar;
        }


        public ICollection<string> GetInjections(string scopeName)
        {
            return null;
        }

        public IRawTheme GetTheme(string scopeName)
        {
            return Inner.GetTheme(scopeName);
        }

        internal static Stream TryOpenEmbeddedResourceStream(string path)
        {
            return typeof(TPGGrammarRegistryOptions).GetTypeInfo().Assembly.GetManifestResourceStream(GrammarPrefix + path);
        }

        private IRawGrammar LoadRawTPGGrammar()
        {
            Stream grammarStream = TryOpenEmbeddedResourceStream(GetGrammarFile("source.tpg"));

            if (grammarStream == null)
                throw new NullReferenceException("Invalid grammar reading process.");

            using (grammarStream)
            using (StreamReader reader = new StreamReader(grammarStream))
            {
                return GrammarReader.ReadGrammarSync(reader);
            }
        }

        string GetGrammarFile(string scopeName)
        {
            foreach (Grammar grammar in GrammarDefinition.Contributes.Grammars)
            {
                if (scopeName.Equals(grammar.ScopeName))
                {
                    string grammarPath = grammar.Path;

                    if (grammarPath.StartsWith("./"))
                        grammarPath = grammarPath.Substring(2);

                    grammarPath = grammarPath.Replace("/", ".");

                    return grammarPath;
                }
            }

            return null;
        }


    }
}