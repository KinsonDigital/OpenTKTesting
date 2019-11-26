using SDLWithOpenGL.SDL2;
using SDLWithOpenGL.Services;
using System.Windows;

namespace SDLWithOpenGL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RenderSurfaceWindow _renderWindow;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ISDLInvoker sdl = new SDLInvoker();
            ITaskManagerService taskService = new TaskManagerService();
            INativeMethods nativeMethods = new NativeMethods();

            _renderWindow = new RenderSurfaceWindow(sdl, taskService, nativeMethods);

            _renderWindow.Show();
        }
    }
}
