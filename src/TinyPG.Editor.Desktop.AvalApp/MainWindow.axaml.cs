using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;

namespace TinyPG.Editor.Desktop.AvalApp
{
    public partial class MainWindow : Window
    {
        private readonly TextEditor _textEditor;
        private readonly RegistryOptions _registryOptions;
        private readonly TextMate.Installation _textMateInstallation;
        public MainWindow()
        {
            InitializeComponent();
            _textEditor = this.FindControl<TextEditor>("Editor");
            _textEditor.Background = Brushes.Transparent;
            _textEditor.ShowLineNumbers = true;
            // _textEditor.TextArea.Background = this.Background;
            _textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            _textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            _textEditor.Options.ShowBoxForControlCharacters = true;
            _textEditor.TextArea.IndentationStrategy = new AvaloniaEdit.Indentation.CSharp.CSharpIndentationStrategy(_textEditor.Options);
            // _textEditor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            _textEditor.TextArea.RightClickMovesCaret = true;
            
            _textEditor.Document = new TextDocument("// AvaloniaEdit supports displaying control chars:");
            _registryOptions = new RegistryOptions(ThemeName.DarkPlus);

            _textMateInstallation = _textEditor.InstallTextMate(_registryOptions);
            _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_registryOptions.GetLanguageByExtension(".cs").Id));
        }
        
        private void textEditor_TextArea_TextEntering(object? sender, TextInputEventArgs e)
        {
        }
        
        private void textEditor_TextArea_TextEntered(object? sender, TextInputEventArgs e)
        {
        }
    }
}