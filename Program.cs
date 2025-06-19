using System;
using OtpNet;

class Program
{
    static void Main()
    {

        byte[] secretKey = Base32Encoding.ToBytes("JBSWY3DPEHPK3PXP");


        var totp = new Totp(secretKey, mode: OtpHashMode.Sha256);

        string otp = totp.ComputeTotp(); // e.g., "123456"

        Console.WriteLine($"Current TOTP: {otp}");
    }
}
