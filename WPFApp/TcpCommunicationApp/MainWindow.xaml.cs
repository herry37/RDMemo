using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;


namespace TcpCommunicationApp
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 使用 TcpListener 接收連線
        /// 伺服器以背景執行緒處理連線，避免阻塞 UI 執行緒
        /// </summary>
        private TcpListener _server;
        private Thread _serverThread;
        /// <summary>
        /// 使用 TcpClient 連接伺服器並發送訊息
        /// 從伺服器接收回應，並更新介面日誌
        /// </summary>
        private TcpClient _client;

        public MainWindow()
        {
            InitializeComponent();
            // 將轉換器加入資源中
            this.Resources["StringToVisibilityConverter"] = new StringToVisibilityConverter();
        }

        /// <summary>
        /// 實作 TCP 伺服器
        /// 責監聽連線並處理來自客戶端的訊息
        /// 伺服器的處理邏輯應該使用非同步方法，這樣可以防止阻塞主線程
        /// 伺服器啟動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _serverThread = new Thread(() =>
                {
                    /* IPAddress.Any：允許伺服器監聽所有本機網卡的 IP 地址 */
                    /* 5000：TCP 連接埠，應與客戶端使用的連接埠一致 */
                    _server = new TcpListener(IPAddress.Any, 5000);
                    _server.Start();
                    Dispatcher.Invoke(() => ServerLog.Text += "伺服器已啟動...\n");

                    while (true)
                    {
                        try
                        {
                            /* 接受連線 */
                            var client = _server.AcceptTcpClient();
                            Dispatcher.Invoke(() => ServerLog.Text += "客戶端已連線。\n");

                            /* 讀取訊息 */
                            var stream = client.GetStream();
                            byte[] buffer = new byte[1024];
                            /* 從客戶端接收訊息 */
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);

                            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                            /* WPF 的 UI 執行緒和背景執行緒無法直接交互，必須使用 Dispatcher.Invoke 來更新 UI */
                            Dispatcher.Invoke(() => ServerLog.Text += $"收到訊息: {message}\n");

                            /* 回覆訊息 */
                            byte[] response = Encoding.UTF8.GetBytes("伺服器已接收訊息");
                            stream.Write(response, 0, response.Length);
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.Invoke(() => ServerLog.Text += $"錯誤: {ex.Message}\n");
                        }
                    }
                });
                _serverThread.IsBackground = true;
                _serverThread.Start();
            }
            catch (Exception ex)
            {
                ServerLog.Text += $"伺服器錯誤: {ex.Message}\n";
            }
        }

        /// <summary>
        /// 實作 TCP 客戶端
        /// 客戶端負責向伺服器發送訊息並接收回應
        /// 客戶端在發送訊息後應該等待伺服器回應。你需要確保客戶端使用非同步方式處理資料流，以避免主線程被阻塞
        /// 客戶端傳送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var client = new TcpClient("localhost", 5000)) // 連接到本機伺服器
                {
                    var stream = client.GetStream();
                    string message = ClientMessage.Text;

                    // 發送訊息
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    await stream.WriteAsync(data, 0, data.Length); // 非同步寫入
                    ClientLog.Text += $"發送訊息: {message}\n";

                    // 接收回應
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); // 非同步讀取
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    ClientLog.Text += $"伺服器回應: {response}\n";
                }
            }
            catch (Exception ex)
            {
                ClientLog.Text += $"客戶端錯誤: {ex.Message}\n";
            }
        }

    }
}
