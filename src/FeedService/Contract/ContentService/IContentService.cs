using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace FeedService
{
    [ServiceContract]
    public interface IContentService
    {
        [OperationContract]
        void Load();

        void Synchronize();

        void RemoveItem();

        void KeepItem();

        void Search();

        void Share();

        void Notify();
    }
}
