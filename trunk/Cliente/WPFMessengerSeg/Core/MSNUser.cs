using System;
using MessengerLib.Core;
using System.Collections.Generic;
using MessengerLib;
using System.Linq;
using System.Text.RegularExpressions;

namespace WPFMessengerSeg.Core
{
    public class MSNUser
    {

        public IList<MSNFeature> ListFeature { get; set; } 

        public int ID { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public bool Online { get; set; }

        public string Password { get; set; }

        public DateTime? Expiration { get; set; }
        public string ExpirationString
        {
            get
            {
                return FormatDateString(Expiration);
            }
        }

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
                return ((DateTime)date).ToString(MessengerLib.Config.DateFormat);
            }
            else
            {
                return String.Empty;
            }
        }

        public static bool IsValidPassword(string password)
        {
            /*REQUISITOS DA SENHA:
             * - de 5 a 7 caracteres
             * - No minimo:
             *   - 1 caracter em caixa alta
             *   - 1 caracter em caixa baixa
             *   - 1 digito
             *   - 1 caracter especial
             */
            string regExp = @"^.*(?=.{5,7})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$";

            if (Regex.IsMatch(password, regExp))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool HasFeature(MSNUser user, Operation operation)
        {

            IList<MSNOperation> query = null;

            //procura em todos os recursos pela operação
            foreach (MSNFeature feature in user.ListFeature)
            {
                query = (from op in feature.ListOperation 
                            where op.Name == operation
                            select op ).ToList();

                if (query.Count > 0)
                {
                    return true;
                }
            }

            return false;
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

                if (!IsValidPassword(password))
                {
                    result = "A senha deve conter de 5 a 7 caracteres, com letras, dígitos, sinais.";

                    //auditoria
                    UDPConnection.InvalidPassword(currentUser);
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
