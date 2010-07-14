using MessengerLib;

namespace WPFMessengerServer.Authentication
{
    public class UserIdentity
    {

        private Encoder encoder;

        public UserIdentity()
        {
            this.encoder = new Encoder();
        }

        public bool IsValid(string user, string password)
        {
            string validPassword = encoder.GenerateMD5("senhaDoZeca");

            if (user.Equals("zeca") && password.Equals(validPassword))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

    }
}
