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

    }
}
