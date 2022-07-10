using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaEdit.Document;
using ReactiveUI;
namespace TinyPG.Editor.Desktop.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            OpenNewDocumentCommand = ReactiveCommand.CreateFromTask(StartOpenNewDocument);
            AskUserForAPathToNewFile = new Interaction<Unit, string>();

            Document = new TextDocument("// AvaloniaEdit supports displaying control chars:");
        }
        
        public readonly Interaction<Unit, string> AskUserForAPathToNewFile;
        
        private async Task StartOpenNewDocument()
        {
            string newPath = await AskUserForAPathToNewFile.Handle(Unit.Default);
            if (!string.IsNullOrWhiteSpace(newPath))
            {
                await OpenNewDocument(newPath);
            }
        }

        private TextDocument document;

        public TextDocument Document 
        {
            get => document;
            set => this.RaiseAndSetIfChanged(ref document, value);
        }
        
        public ReactiveCommand<Unit, Unit> OpenNewDocumentCommand { get; }

        Task OpenNewDocument(string path)
        {
            Document = new TextDocument(File.ReadAllText(path));
            return Task.CompletedTask;
        }
    }
}
