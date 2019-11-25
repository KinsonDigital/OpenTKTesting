using SDLWithOpenGL.SDL2;
using SDLWithOpenGL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
