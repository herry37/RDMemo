using System.Security.Cryptography;
using System.Text;

namespace ShoppingListAPI.Services.WebSocket;

public class WebSocketEncryption
{
    private const string ServerKeyString = "your-server-key-here"; // 請替換為實際的伺服器金鑰
    
    public static async Task<byte[]> DecryptSharedSecret(string encryptedSecret)
    {
        try
        {
            // 將 base64 字串轉換為位元組陣列
            var encryptedBytes = Convert.FromBase64String(encryptedSecret);
            
            // 使用 RSA 解密
            using var rsa = RSA.Create();
            rsa.ImportFromPem(ServerKeyString);
            
            var decryptedBytes = await Task.Run(() =>
                rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256));
            
            return decryptedBytes;
        }
        catch (Exception)
        {
            // 如果解密失敗，返回空陣列
            return Array.Empty<byte>();
        }
    }

    public static string GenerateServerKey()
    {
        using var rsa = RSA.Create(2048);
        return rsa.ExportRSAPrivateKeyPem();
    }
}
