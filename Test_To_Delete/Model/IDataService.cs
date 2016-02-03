using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test_To_Delete.Model
{
    public interface IDataService
    {
        void GetData(Action<DataItem, Exception> callback);
    }
}
