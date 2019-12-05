using RenderWindowTesting;
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

namespace WPFTestingUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RenderEngine _renderEngine;
        private Task _engineStartTask;
        private CancellationTokenSource _engineStartTaskTokenSrc;


        public MainWindow()
        {
            InitializeComponent();

            _renderEngine = new RenderEngine();
        }


        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _renderEngine.StartEngine();
            //_engineStartTaskTokenSrc = new CancellationTokenSource();

            //_engineStartTask = new Task(() =>
            //{
            //}, _engineStartTaskTokenSrc.Token);
        }


        private void StopButton_Click(object sender, RoutedEventArgs e)
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

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            _renderEngine.Restart();
        }
    }
}
