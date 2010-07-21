using System;

namespace MessengerLib.Handler
{
    public class OperationHandler
    {
        public static Operation GetOperation(string operation)
        {
            return (Operation)Enum.Parse(typeof(Operation), operation, true);
        }
    }

    public enum Operation
    {
        RegUsers
       ,ChangeProp
       ,Auditor
       ,SendMsg
       ,RecMsg
       ,SendEmoticons
       ,RecEmoticons
       ,SendMsgOffUser
    }

}
