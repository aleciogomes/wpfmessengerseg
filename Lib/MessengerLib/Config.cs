
using System;
namespace MessengerLib
{
     public static class Config
    {
         public static string ErrorMessage = "ERRO";
         public static string OKMessage = "OK";

         public static string EndStackMessage = "0:";

         public static int TCPPort = 1012;
         public static int UDPPort = 1011;

         public static string ServerURL = "127.0.0.1";

         public static string DateFormat = "{0:dd/MM/yyyy}";
         public static string DateFormatMySQL = "yyyy-MM-dd";

         public static string FormatUserDisplay(string name, string login)
         {
             return String.Format("{0} (id: {1})", name, login);
         }

    }
}
