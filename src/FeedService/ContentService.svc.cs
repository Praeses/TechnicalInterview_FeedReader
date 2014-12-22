using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FeedService
{
    public class Content : IContentService
    {
        [OperationContract]
        public void Load(){}

        public void Synchronize() { }

        public void RemoveItem() { }

        public void KeepItem() { }

        public void Search() { }

        public void Share() { }

        public void Notify() { }
    }
}
