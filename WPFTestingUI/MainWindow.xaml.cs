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

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _renderEngine = new RenderEngine(RenderHost);
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
    }
}
