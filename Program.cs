using System;
using System.Text;

class Program
{
    static void Main()
    {
        // Simulate a shared secret (you’d store this per user)
        var secretKey = Encoding.UTF8.GetBytes("super_secret_shared_key");

        // Create service
        var totpService = new CustomTotpService(secretKey);

        // Generate code
        var otp = totpService.GenerateCode();
        Console.WriteLine($"Generated OTP: {otp}");

        // Simulate user entering the OTP
        Console.Write("Enter OTP to verify: ");
        var input = Console.ReadLine();

        if (int.TryParse(input, out var inputCode))
        {
            bool valid = totpService.ValidateCode(inputCode);
            Console.WriteLine(valid ? "✅ OTP is valid!" : "❌ OTP is invalid.");
        }
        else
        {
            Console.WriteLine("❌ Invalid input format.");
        }
    }
}
