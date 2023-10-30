using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MonsterCardGame.Uitilities
{
    class PJWTHeader
    {
        public DateTime TimeToLive { get; set; }
    }

    public static class PJWToken
    {
        public static string CreateToken(object content, DateTime ttl, string secret)
        {
            string header64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new PJWTHeader() { TimeToLive = ttl })));
            string content64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(content)));
            string checksum64;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(header64 + content64 + secret));
                checksum64 = Convert.ToBase64String(hashBytes);
            }

            return $"{header64}:{content64}:{checksum64}";
        }

        public static bool IsValid(string token, string secret)
        {
            string[] fragments = token.Split(':');
            if (fragments.Count() < 3)
                throw new ArgumentException($"[{nameof(PJWToken)}] GetContent token in wrong format");

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(fragments[0] + fragments[1] + secret));
                if (fragments[2] != Convert.ToBase64String(hashBytes))
                    return false;
            }

            string headerJson = Encoding.UTF8.GetString(Convert.FromBase64String(fragments[0]));
            PJWTHeader? header = JsonSerializer.Deserialize<PJWTHeader>(headerJson);
            if (header == null)
                return false;

            return DateTime.Now < header.TimeToLive;
        }

        public static T? GetContent<T>(string token)
        {
            string[] fragments = token.Split(':');
            if (fragments.Count() < 3)
                throw new ArgumentException($"[{nameof(PJWToken)}] GetContent token in wrong format");

            string contentJson = Encoding.UTF8.GetString(Convert.FromBase64String(fragments[1]));
            return JsonSerializer.Deserialize<T>(contentJson);
        }
    }
}