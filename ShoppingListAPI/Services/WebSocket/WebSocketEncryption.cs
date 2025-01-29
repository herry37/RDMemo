using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ShoppingListAPI.Services.WebSocket;

/// <summary>
/// WebSocket 加密服務
/// 提供安全的訊息加密和解密功能，使用 AES 加密演算法
/// </summary>
public class WebSocketEncryption
{
    // 加密金鑰和初始化向量
    private readonly byte[] _key;
    private readonly byte[] _iv;
    
    /// <summary>
    /// 建構函式
    /// </summary>
    /// <param name="configuration">應用程式設定</param>
    /// <exception cref="ArgumentNullException">當設定中缺少必要的加密金鑰時拋出</exception>
    public WebSocketEncryption(IConfiguration configuration)
    {
        // 從設定檔讀取加密金鑰
        var keyString = configuration["WebSocket:EncryptionKey"] 
            ?? throw new ArgumentNullException(nameof(configuration), "加密金鑰未設定");
        var ivString = configuration["WebSocket:InitializationVector"] 
            ?? throw new ArgumentNullException(nameof(configuration), "初始化向量未設定");

        // 使用 SHA256 產生固定長度的金鑰
        using var sha256 = SHA256.Create();
        _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyString));
        
        // 使用 MD5 產生固定長度的初始化向量
        using var md5 = MD5.Create();
        _iv = md5.ComputeHash(Encoding.UTF8.GetBytes(ivString));
    }

    /// <summary>
    /// 加密訊息
    /// </summary>
    /// <param name="plainText">要加密的文字</param>
    /// <returns>加密後的 Base64 字串</returns>
    /// <exception cref="ArgumentException">當輸入為空或 null 時拋出</exception>
    /// <exception cref="CryptographicException">當加密過程發生錯誤時拋出</exception>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("加密文字不可為空", nameof(plainText));

        try
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            
            return Convert.ToBase64String(cipherBytes);
        }
        catch (Exception ex)
        {
            throw new CryptographicException("加密過程發生錯誤", ex);
        }
    }

    /// <summary>
    /// 解密訊息
    /// </summary>
    /// <param name="cipherText">要解密的 Base64 字串</param>
    /// <returns>解密後的原始文字</returns>
    /// <exception cref="ArgumentException">當輸入為空或 null 時拋出</exception>
    /// <exception cref="CryptographicException">當解密過程發生錯誤時拋出</exception>
    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentException("解密文字不可為空", nameof(cipherText));

        try
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            using var decryptor = aes.CreateDecryptor();
            var cipherBytes = Convert.FromBase64String(cipherText);
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (Exception ex)
        {
            throw new CryptographicException("解密過程發生錯誤", ex);
        }
    }
}
