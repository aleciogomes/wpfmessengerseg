﻿
using System;
namespace MessengerLib.Util
{
    public static class Config
    {
         public static string ErrorMessage = "ERRO NO SERVIDOR";
         public static string OKMessage = "OK";

         public static string EndStackMessage = "#:";

         internal static int DefaultTCPPort = 1012;
         internal static int DefaultUDPPort = 1011;
         internal const string DefaultServer = "127.0.0.1";

         public static string DateFormat = "dd/MM/yyyy";
         public static string DateFormatMySQL = "yyyy-MM-dd";
         public static string DateFormatMySQLFull = "yyyy-MM-dd HH:mm:ss";

         public static string FormatUserDisplay(string name, string login)
         {
             return String.Format("{0} (id: {1})", name, login);
         }

    }
}
