using System;

namespace MessengerLib.Handler
{
    public class FeatureHandler
    {
        public static Feature GetFeature(string feature)
        {
            return (Feature)Enum.Parse(typeof(Feature), feature, true);
        }
    }

    public enum Feature
    {
         Main
        ,Chat
        ,ContactList
    }

}
