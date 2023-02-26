using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurSampleForCircuitBreaker
{
    public interface IGetOrdersFromEngine
    {
        Task<List<object>> GetOrders();   
    }
}
