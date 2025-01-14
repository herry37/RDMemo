using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace DecryptApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 解碼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butDecode_Click(object sender, EventArgs e)
        {
            string encryptedText = txtInput.Text;

            // 防呆：檢查輸入是否為空
            if (string.IsNullOrWhiteSpace(encryptedText))
            {
                txtDecodeOut.Text = "請輸入要解碼的 Base64 字串。";
                return;
            }

            // 防呆：檢查是否是有效的 Base64 字符串
            if (!Encodeservice.IsBase64String(encryptedText))
            {
                txtDecodeOut.Text = "輸入的字串不是有效的 Base64 編碼。";
                return;
            }

            try
            {

                string decryptedText = Encodeservice.desDecryptBase64(encryptedText);
                txtDecodeOut.Text = "解碼結果: " + decryptedText;
            }
            catch (FormatException)
            {
                txtDecodeOut.Text = "解碼失敗：輸入的 Base64 字串格式錯誤。";
            }
            catch (CryptographicException)
            {
                txtDecodeOut.Text = "解碼失敗：解密過程中出現錯誤。";
            }
            catch (Exception ex)
            {
                txtDecodeOut.Text = "解碼失敗: " + ex.Message;
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                string encryptedText = txtEncrypt.Text;
                // 防呆：檢查輸入是否為空
                if (string.IsNullOrWhiteSpace(encryptedText))
                {
                    txtEncryptOut.Text = "請輸入要加密的 Base64 字串。";
                    return;
                }

                string encryptText = txtEncrypt.Text;
                string decryptedText = Encodeservice.SettingEncryptionBase64(encryptText);
                txtEncryptOut.Text = "加密結果: " + decryptedText;
            }
            catch (CryptographicException)
            {
                txtEncryptOut.Text = "加密失敗：加密過程中出現錯誤。";
            }
            catch (Exception ex)
            {
                txtEncryptOut.Text = "加密失敗: " + ex.Message;
            }
        }
        /// <summary>
        /// "SHA512 雜湊計算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSH512_Click(object sender, EventArgs e)
        {
            try
            {
                // 取得輸入的帳號與密碼
                string password = txtBoxInSHA512password.Text;
                string account = txtBoxInSHA512account.Text;

                // 驗證輸入是否為空
                if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(account))
                {
                    throw new ArgumentException("帳號或密碼不得為空");
                }

                // 使用 SHA512 進行雜湊計算

                var base64Hash = Encodeservice.ComputeHash(password, "SHA512", Encoding.UTF8.GetBytes(account));
                //var newpas = Encodeservice.ComputeHash(password, "SHA512", Encoding.UTF8.GetBytes(account));

                txtBoxShowSH512.Text = "雜湊結果: " + base64Hash;
            }
            catch (ArgumentException ex)
            {
                txtBoxShowSH512.Text = "輸入錯誤: " + ex.Message;
            }
            catch (Exception ex)
            {
                txtBoxShowSH512.Text = "處理失敗: " + ex.Message;
            }
        }

    }
}

