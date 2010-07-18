﻿using System;

namespace WPFMessengerSeg.Core
{
    public class MSNUser
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public bool Online { get; set; }

        //apenas o usuário logado terá esses 2 atributos com valor
        public string Password { get; set; }

        public DateTime? Expiration { get; set; }
        public string ExpirationString
        {
            get
            {
                return FormatDateString(Expiration);
            }
        }


        //apenas através da tela de administração esses atributos são preenchidos
        public Boolean Blocked { get; set; }
        public int TimeAlert { get; set; }

        public DateTime? UnblockDate { get; set; }
        public string UnblockDateString
        {
            get
            {
                return FormatDateString(UnblockDate);
            }
        }

        private string FormatDateString(DateTime? date)
        {
            if (date.HasValue)
            {
                return String.Format(MessengerLib.Config.DateFormat, date);
            }
            else
            {
                return String.Empty;
            }
        }

        public static string ValidateChanges(string currentUser, string name ,string user, bool validatePass, string password, string password2)
        {

            string result = string.Empty;

            if (validatePass)
            {
                if (String.IsNullOrEmpty(password))
                {
                    result = "Informe e confirme a senha.";
                    return result;
                }

                if (!password.Equals(password2))
                {
                    result = "As senhas não coincidem.";
                    return result;
                }
            }

            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(user))
            {
                result = "Informe todos os campos corretamente.";
                return result;
            }

            string testUser = String.Empty;

            //faz o teste se mudou o usuário
            if (!currentUser.Equals(user))
            {
                testUser = TCPConnection.GetUserAvailable(user);
            }

            //usuário já em utilização
            if (!(testUser.Equals(MessengerLib.Config.OKMessage) || String.IsNullOrEmpty(testUser)))
            {
                result = testUser;
                return result;
            }

            return result;

        }

    }
}
