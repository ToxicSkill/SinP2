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
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;

namespace SinP2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SystemInfo systemInfo = new SystemInfo();
        DispatcherTimer Timer99 = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            WindowBlur.SetIsEnabled(this, true);
            MouseDown += Window_MouseDown;

            try
            {
                InitializeSinP();
            }
            catch (Exception e)
            {
                MessageBox.Show("System info data error: " + e.Message);
            }

            Timer99.Tick += Timer99_Tick; // don't freeze the ui
            Timer99.Interval = new TimeSpan(0, 0, 0, 0, 1024);
            Timer99.IsEnabled = true;
        }
        private bool InitializeSinP()
        {
            setComputerName();    
            setOSName();
            setCPUName();
            setGPUName();
            setRAMName();
            setDriveName();
            return true;
        }
        public int getCPUCounter()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            // will always start at 0
            dynamic firstValue = cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            // now matches task manager reading
            dynamic secondValue = cpuCounter.NextValue();

            return (int)secondValue;
        }


        private void Timer99_Tick(Object sender, EventArgs e)
        {
            var percent = (int)getCPUCounter();
            CpuUsage.Content = percent + " % CPU";
            progress.Value = percent;
            if (percent > 80)
                progress.Foreground = new SolidColorBrush(Colors.Red);
            else
                progress.Foreground = new SolidColorBrush(Colors.Green);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void setComputerName()
        {
            CName.Content = systemInfo.MachineName;
        }
        private void setOSName()
        {
            OSName.Content = systemInfo.OSName;
        }
        private void setCPUName()
        {
            ProcessorName.Content = systemInfo.CPUName;
        }
        private void setGPUName()
        {
            GraphicName.Content = systemInfo.GPUName;
        }
        private void setRAMName()
        {
            RAMName.Content = systemInfo.RAMName;
        }
        private void setDriveName()
        {
            DriveName.Content = systemInfo.DriveName;
        }

    }
}

