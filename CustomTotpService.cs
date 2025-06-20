using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

public class CustomTotpService
{
    private readonly TimeSpan _timestep = TimeSpan.FromMinutes(3);
    private readonly Encoding _encoding = new UTF8Encoding(false, true);
    private readonly byte[] _secretKey;

    public CustomTotpService(byte[] secretKey)
    {
        _secretKey = secretKey;
    }

    private ulong GetCurrentTimeStepNumber()
    {
        var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var delta = DateTime.UtcNow - unixEpoch;
        return (ulong)(delta.Ticks / _timestep.Ticks);
    }

    private byte[] ApplyModifier(byte[] input, string modifier)
    {
        if (string.IsNullOrEmpty(modifier))
            return input;

        var modifierBytes = _encoding.GetBytes(modifier);
        var combined = new byte[input.Length + modifierBytes.Length];
        Buffer.BlockCopy(input, 0, combined, 0, input.Length);
        Buffer.BlockCopy(modifierBytes, 0, combined, input.Length, modifierBytes.Length);
        return combined;
    }

    private int ComputeTotp(HashAlgorithm hashAlgorithm, ulong timestepNumber, string modifier)
    {
        const int mod = 1000000;

        var timestepAsBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long)timestepNumber));
        var hash = hashAlgorithm.ComputeHash(ApplyModifier(timestepAsBytes, modifier));

        var offset = hash[hash.Length - 1] & 0xf;
        var binaryCode = (hash[offset] & 0x7f) << 24
                         | (hash[offset + 1] & 0xff) << 16
                         | (hash[offset + 2] & 0xff) << 8
                         | (hash[offset + 3] & 0xff);

        return binaryCode % mod;
    }

    public int GenerateCode(string modifier = null)
    {
        var timestep = GetCurrentTimeStepNumber();
        using var hmac = new HMACSHA1(_secretKey);
        return ComputeTotp(hmac, timestep, modifier);
    }

    public bool ValidateCode(int code, string modifier = null)
    {
        var timestep = GetCurrentTimeStepNumber();
        using var hmac = new HMACSHA1(_secretKey);

        for (var i = -2; i <= 2; i++)
        {
            var calculated = ComputeTotp(hmac, (ulong)((long)timestep + i), modifier);
            if (calculated == code)
                return true;
        }

        return false;
    }
}
