using Microsoft.Win32;
using Renderer.Core;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Timers;
using System.Windows;

namespace MyH264Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int localPort = 8002;
        D3DImageSource d3dSource;
        //720P=1280*720
        int width = 1920;
        int height = 1080;
        ConcurrentQueue<byte> streamBuf = new ConcurrentQueue<byte>();
        MyUdpServer udpServer = null;
        H264Player player = null;
        //码流结束,0==false,1=true
        uint isStop = 0;


        public MainWindow()
        {
            InitializeComponent();
            //设置视频源，即将imageView的源设置为d3dSource
            this.d3dSource = new D3DImageSource();
            try
            {
                if (this.d3dSource.SetupSurface(width, height, FrameFormat.YV12))
                {
                    this.ImageD3D.Source = this.d3dSource.ImageSource;
                }
                else
                {
                    MessageBox.Show("本机显卡不支持该种帧格式：" + FrameFormat.YV12);
                }
            }
            catch
            {
                MessageBox.Show("设置显卡视频源错误。");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            udpServer = new MyUdpServer(localPort);
            udpServer.DataReceived += UdpServer_DataReceived;
            player = new H264Player(streamBuf, width, height, d3dSource);
            Start();
        }
        private void Start()
        {
            //启动udp服务器接收视频数据
            udpServer.StartServer();
            player.Start();
            isStop = 0;
        }
        private void Stop()
        {
            udpServer.StopServer();
            isStop = 1;
            if (player != null)
                player.Stop();

        }
        //收到视频流数据
        private void UdpServer_DataReceived(object arg1, MyCustomEventArgs arg2)
        {
            if (isStop == 0)
            {
                foreach (var item in arg2.Data)
                {
                    streamBuf.Enqueue(item);
                }
            }

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            isStop = 1;
            if (player != null)
                player.Stop();
            udpServer.StopServer();
        }

    
    }
}
