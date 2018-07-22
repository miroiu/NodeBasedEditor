using Ninject.Modules;
using VEdit;
using VEdit.Common;
using VEdit.Dialogs;
using VEdit.Editor;
using VEdit.UI;

namespace Ninject.Extensions.VEdit
{
    public class UIModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IServiceProvider>().To<ServiceProvider>().InSingletonScope();
            Bind<EditorViewModel>().ToSelf().InSingletonScope();

            // Mixed
            Bind<ISerializer<string>>().To<JsonSerializer>().InSingletonScope();
            Bind<IEditorSettings>().To<EditorSettings>().InSingletonScope();
            Bind<IOutputManager>().To<OutputConsole>().InSingletonScope();
            Bind<IColorProvider>().To<ColorProvider>().InSingletonScope();
            Bind<IFileIO>().To<FileIO>().InSingletonScope();
            Bind<IClipboardManager>().To<ClipboardManager>().InSingletonScope();
            Bind<ILogger>().To<Logger>().InSingletonScope().WithConstructorArgument("log.txt");
            Bind<IBreakpointManager>().To<BreakpointManager>().InSingletonScope();

            // No singletons
            Bind<ISelectionService<IElement>>().To<ElementSelectionService>();
            Bind<IArchive>().To<Archive>();
            Bind<IImage>().To<ImageAdapter>();

            // Databases
            Bind<IActionsDatabase>().To<ActionsDatabase>().InSingletonScope();
            Bind<IPluginProvider>().To<PluginProvider>().InSingletonScope();
            Bind<INodeDatabase>().To<NodeDatabase>().InSingletonScope();
            Bind<IGraphProvider>().To<GraphProvider>().InSingletonScope();

            // Factories
            Bind<IPluginFactory>().To<PluginBuilder>().InSingletonScope();
            Bind<IPinFactory>().To<PinFactory>().InSingletonScope();
            Bind<IGraphFactory>().To<GraphFactory>().InSingletonScope();
            Bind<INodeFactory>().To<NodeFactory>().InSingletonScope();
            Bind<ICommandProvider>().To<CommandProvider>().InSingletonScope();

            // Dialogs
            Bind<IOpenFileDialog>().To<OpenFileDialogAdapter>().InSingletonScope();
            Bind<IOpenFolderDialog>().To<OpenFolderDialogAdapter>().InSingletonScope();

            Bind<IDialogManager>().To<DialogManager>().InSingletonScope();
        }
    }
}
