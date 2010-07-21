using System.Collections.Generic;
using MessengerLib.Handler;

namespace MessengerLib.Core
{
    public class MSNFeature
    {
        public MSNFeature()
        {
            this.ListOperation = new List<MSNOperation>();
        }

        public int ID { get; set; }
        public Feature Name { get; set; }
        public IList<MSNOperation> ListOperation { get; set; } 
    }
}
