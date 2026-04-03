using System;
using BCrypt.Net;

class Program {
    static void Main() {
        // Tạo mã băm chuẩn cho mật khẩu admin@123
        string hash = BCrypt.Net.BCrypt.HashPassword("admin@123");
        Console.WriteLine("VALID_HASH:" + hash);
    }
}
