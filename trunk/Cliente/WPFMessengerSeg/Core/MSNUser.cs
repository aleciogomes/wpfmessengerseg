using System;

namespace WPFMessengerSeg.Core
{
    public class MSNUser
    {
        public string UserName { get; set; }
        public string UserLogin { get; set; }
        public bool Online { get; set; }

        //apenas o usuário logado terá esses 2 atributos com valor
        public string UserPassword { get; set; }
        public DateTime? Expiration { get; set; }


        public static string ValidateChanges(string name ,string user, bool validatePass, string password, string password2)
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
            if (!MSNSession.User.UserLogin.Equals(user))
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
