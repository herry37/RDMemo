using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TableToModel;
using static TableToModel.DatabaseConnectionFactory;

namespace TableToModel
{
    public partial class TableToModel : Form
    {
        private IDatabaseConnectionFactory _currentFactory;
        private string _lastConnectionString;
        private bool _lastConnectionResult;
        private DateTime _lastTestTime = DateTime.MinValue;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);

        public TableToModel()
        {
            InitializeComponent();
            InitializeDatabaseTypeComboBox();
            this.FormClosing += TableToModel_FormClosing;
        }

        private void TableToModel_FormClosing(object sender, FormClosingEventArgs e)
        {
            _connectionLock?.Dispose();
        }

        /// <summary>
        /// 初始化資料庫類型下拉選單
        /// </summary>
        private void InitializeDatabaseTypeComboBox()
        {
            comDBType.Items.Add(DatabaseConnectionFactory.DatabaseType.MSSQL.ToString());
            comDBType.Items.Add(DatabaseConnectionFactory.DatabaseType.MySQL.ToString());
            comDBType.Items.Add(DatabaseConnectionFactory.DatabaseType.MongoDB.ToString());
            comDBType.SelectedIndex = 0; // 默認選擇第一項
        }

        /// <summary>
        /// 檢查連線
        /// </summary>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            string strMessage = "";
            bool isInputValid = true;

            if (string.IsNullOrEmpty(txtIP.Text)) { isInputValid = false; strMessage = "IP未填寫!\r"; }
            if (string.IsNullOrEmpty(txtID.Text)) { isInputValid = false; strMessage += "ID未填寫!\r"; }
            if (string.IsNullOrEmpty(txtPass.Text)) { isInputValid = false; strMessage += "密碼未填寫!\r"; }
            if (string.IsNullOrEmpty(txtDbName.Text)) { isInputValid = false; strMessage += "DbName未填寫!\r"; }

            if (isInputValid)
            {
                var dbType = (DatabaseConnectionFactory.DatabaseType)comDBType.SelectedIndex;
                var factory = DatabaseConnectionFactory.GetFactory(dbType);

                try 
                {
                    var connectionString = factory.GetConnectionString(txtIP.Text, txtID.Text, txtPass.Text, txtDbName.Text);

                    if (CheckConnection(factory, connectionString))
                    {
                        MessageBox.Show("連線成功!");
                    }
                    else
                    {
                        MessageBox.Show("連線失敗!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"連線錯誤: {ex.Message}");
                }
            }
            else { MessageBox.Show(strMessage); };
        }

        /// <summary>
        /// 產 Model
        /// </summary>
        private void btnModel_Click(object sender, EventArgs e)
        {
            string strMessage = "";

            if (string.IsNullOrEmpty(txtIP.Text)) { strMessage = "IP未填寫!\r"; }
            if (string.IsNullOrEmpty(txtID.Text)) { strMessage += "ID未填寫!\r"; }
            if (string.IsNullOrEmpty(txtPass.Text)) { strMessage += "密碼未填寫!\r"; }
            if (string.IsNullOrEmpty(txtDbName.Text)) { strMessage += "DbName未填寫!\r"; }
            if (string.IsNullOrEmpty(txtTableName.Text)) { strMessage += "Table/Collection 名稱未填寫!\r"; }

            if (string.IsNullOrEmpty(strMessage))
            {
                try
                {
                    var dbType = (DatabaseConnectionFactory.DatabaseType)comDBType.SelectedIndex;
                    string modelData;

                    if (dbType == DatabaseConnectionFactory.DatabaseType.MongoDB)
                    {
                        var generator = new MongoDbModelGenerator();
                        modelData = generator.GenerateModelAsync(txtIP.Text, txtID.Text, txtPass.Text, txtDbName.Text, txtTableName.Text)
                            .GetAwaiter()
                            .GetResult();
                    }
                    else
                    {
                        var converter = new ModelConverter();
                        modelData = converter.GetModel(dbType, txtIP.Text, txtID.Text, txtPass.Text, txtDbName.Text, txtTableName.Text);
                    }

                    txtModel.Text = modelData;
                    txtModel.ScrollBars = ScrollBars.Vertical;
                    txtModel.SelectionStart = txtModel.Text.Length;
                    txtModel.ScrollToCaret();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"生成 Model 時發生錯誤: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show(strMessage);
            }
        }

        /// <summary>
        /// 檢查資料庫連線
        /// </summary>
        private bool CheckConnection(IDatabaseConnectionFactory factory, string connectionString)
        {
            try
            {
                // 使用 SemaphoreSlim 確保同一時間只有一個連線測試在執行
                if (!_connectionLock.Wait(50)) // 減少等待時間到 50ms
                {
                    return false;
                }

                // 檢查快取
                if (factory == _currentFactory && 
                    connectionString == _lastConnectionString && 
                    (DateTime.Now - _lastTestTime).TotalSeconds < 10) // 增加快取時間到 10 秒
                {
                    return _lastConnectionResult;
                }

                // 設定取消令牌，確保不會等待太久
                using var cts = new CancellationTokenSource(3000); // 增加到 3 秒

                try
                {
                    if (factory is MongoDbConnectionFactory)
                    {
                        // MongoDB 連線測試
                        var settings = MongoClientSettings.FromConnectionString(connectionString);
                        settings.ServerSelectionTimeout = TimeSpan.FromMilliseconds(300);
                        settings.ConnectTimeout = TimeSpan.FromMilliseconds(300);
                        var client = new MongoClient(settings);
                        
                        _lastConnectionResult = Task.Run(async () =>
                        {
                            try
                            {
                                var db = client.GetDatabase("admin");
                                await db.RunCommandAsync((Command<BsonDocument>)"{ping:1}", cancellationToken: cts.Token);
                                return true;
                            }
                            catch
                            {
                                return false;
                            }
                        }, cts.Token).GetAwaiter().GetResult();
                    }
                    else
                    {
                        // SQL Server 和 MySQL 連線測試
                        using var connection = factory.CreateConnection(connectionString);
                        _lastConnectionResult = Task.Run(async () =>
                        {
                            try
                            {
                                await Task.WhenAny(
                                    Task.Run(() => connection.Open()),
                                    Task.Delay(2000, cts.Token) // 增加到 2 秒
                                );
                                return connection.State == ConnectionState.Open;
                            }
                            catch
                            {
                                return false;
                            }
                            finally
                            {
                                if (connection.State == ConnectionState.Open)
                                {
                                    connection.Close();
                                }
                            }
                        }, cts.Token).GetAwaiter().GetResult();
                    }

                    // 更新快取
                    _currentFactory = factory;
                    _lastConnectionString = connectionString;
                    _lastTestTime = DateTime.Now;

                    return _lastConnectionResult;
                }
                catch (OperationCanceledException)
                {
                    // 如果操作被取消，視為連線失敗
                    _lastConnectionResult = false;
                    return false;
                }
            }
            finally
            {
                // 確保一定會釋放鎖
                if (_connectionLock.CurrentCount == 0)
                {
                    _connectionLock.Release();
                }
            }
        }
    }
}
