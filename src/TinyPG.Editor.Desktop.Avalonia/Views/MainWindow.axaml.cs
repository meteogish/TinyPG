using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.TextMate;
using ReactiveUI;
using TextMateSharp.Grammars;
using TinyPG.Editor.Desktop.Avalonia.ViewModels;

namespace TinyPG.Editor.Desktop.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        private readonly TextEditor _textEditor;
        private readonly TextMate.Installation _textMateInstallation;
        private readonly RegistryOptions _registryOptions;
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
            
            // _textEditor.Document = new TextDocument("// AvaloniaEdit supports displaying control chars:");
            _registryOptions = new RegistryOptions(ThemeName.DarkPlus);

            _textMateInstallation = _textEditor.InstallTextMate(_registryOptions);
            _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(_registryOptions.GetLanguageByExtension(".cs").Id));
            
            // _textEditor.TextArea.TextView.Redraw();
        }
        
        private void textEditor_TextArea_TextEntering(object? sender, TextInputEventArgs e)
        {
        }
        
        private void textEditor_TextArea_TextEntered(object? sender, TextInputEventArgs e)
        {
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);
            var vm = DataContext as MainWindowViewModel;
            vm?.AskUserForAPathToNewFile.RegisterHandler(GetFileInteractionResult);
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            var vm = DataContext as MainWindowViewModel;
            vm?.AskUserForAPathToNewFile.RegisterHandler(GetFileInteractionResult);
        }
        
        private async Task GetFileInteractionResult(InteractionContext<Unit, string> interactionContext)
        {
            var dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Extensions = { "tpg" } });

            var result = await dialog.ShowAsync(this);

            if (result != null)
            {
                interactionContext.SetOutput(result[0]);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
	        base.OnClosed(e);
	        _textMateInstallation.Dispose();
        }
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private Task MenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}