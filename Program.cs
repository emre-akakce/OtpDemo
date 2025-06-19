using System.Security.Cryptography;

class Program
{
    static void Main()
    {
        string otp = GenerateNumericOtp(6);
        Console.WriteLine($"Your OTP is: {otp}");
        // You can now return or send this OTP to the user
    }

    public static string GenerateNumericOtp(int length)
    {
        if (length <= 0)
            throw new ArgumentException("OTP length must be positive");

        var otp = new char[length];
        using var rng = RandomNumberGenerator.Create();
        byte[] buffer = new byte[1];

        for (int i = 0; i < length; i++)
        {
            rng.GetBytes(buffer);
            otp[i] = (char)('0' + (buffer[0] % 10)); // Generates digits 0–9
        }

        return new string(otp);
    }
}
