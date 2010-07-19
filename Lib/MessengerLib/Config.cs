
using System;
namespace MessengerLib
{
     public static class Config
    {
         public static string ErrorMessage = "ERRO NO SERVIDOR";
         public static string OKMessage = "OK";

         public static string EndStackMessage = "#:";

         public static int TCPPort = 1012;
         public static int UDPPort = 1011;

         public static string ServerURL = "127.0.0.1";

         public static string DateFormat = "dd/MM/yyyy";
         public static string DateFormatMySQL = "yyyy-MM-dd";
         public static string DateFormatMySQLFull = "yyyy-MM-dd HH:mm:ss";

         public static string FormatUserDisplay(string name, string login)
         {
             return String.Format("{0} (id: {1})", name, login);
         }

    }
}
