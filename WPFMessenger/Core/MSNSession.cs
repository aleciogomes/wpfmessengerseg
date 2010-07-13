using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFMessenger.Core
{
    /*
     LUIZ:
        User: 7237
        PW: ht7mxh

     ALÉCIO:
        User: 6254
        Pw: vv2tab
    
     OUTROS:
        User: 3299 
        Pw: 12qnmn
    */

    public static class MSNSession
    {

        public static MSNUser User { get; set; }

        public static void CreateUser()
        {
            User = new MSNUser();
        }

    }
}
