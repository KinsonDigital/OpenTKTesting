using WPFTestingUI.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using OpenTK.Platform;
using OpenTK.Graphics.OpenGL4;
using System.Windows.Interop;
using OpenTK.Graphics;
using OpenTK.Platform;

namespace WPFTestingUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindowInfo
    {
        private RenderEngine _renderEngine;
        private IWindowInfo _windowInfo;
        private IGLInvoker _glInvoker;
        private Renderer _renderer;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _glInvoker = new GLInvoker();
            _renderer = new Renderer(RenderHost, _glInvoker);
            _renderEngine = new RenderEngine(_renderer);
            _renderEngine.StartEngine();
        }

        public IntPtr Handle { get; private set; }

        public static int ViewPortWidth { get; set; }

        public static int ViewPortHeight { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            _renderEngine.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _renderEngine.Pause();
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            _renderEngine.Reload();
        }
    }
}
