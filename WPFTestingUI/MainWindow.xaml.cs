using WPFTestingUI.OpenGL;
using System;
using System.Windows;
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
        private IRenderer _renderer;

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

            var link = new Texture("Link.png");
            link.X = 50;
            link.Y = 50;
            link.Size = 2f;

            var sword = new Texture("Sword.png");
            sword.X = 250;
            sword.Y = 50;
            sword.Size = 0.5f;


            _renderEngine.AddTexture(link);
            _renderEngine.AddTexture(sword);

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
