using CircuitBreakerSample;
using Newtonsoft.Json;
using System.Net;

namespace OurSampleForCircuitBreaker
{
    public class CircuitBreakerForStockMarketEngine : IGetOrdersFromEngine
    {
        private CircuitBreakerState _state;
        public CircuitBreakerForStockMarketEngine()
        {
            _state = new CircuitBreakerClosed(this);
        }
        public Task<List<object>> GetOrders()
        {
            return _state.GetOrders();
        }
        private abstract class CircuitBreakerState
        {
            protected CircuitBreakerForStockMarketEngine _owner;
            public CircuitBreakerState(CircuitBreakerForStockMarketEngine owner)
            {
                _owner = owner;
            }

            public abstract Task<List<object>> GetOrders();
        }
        private class CircuitBreakerClosed : CircuitBreakerState
        {

            private int _errorCount = 0;
            public CircuitBreakerClosed(CircuitBreakerForStockMarketEngine owner)
                : base(owner) { }

            public override async Task<List<object>> GetOrders()
            {
                try
                {
                    using var httpClient = new HttpClient();
                    var result = httpClient.GetAsync("https://localhost:7092/api/Trades/GetAllTrades").GetAwaiter().GetResult();
                    var trades = await result.Content.ReadAsStringAsync();
                    if (result.StatusCode != HttpStatusCode.OK)
                    {
                        _trackErrors(new Exception("Trade Service Is not Availbe"));
                    }
                    return JsonConvert.DeserializeObject<List<object>>(trades);
                }
                catch (Exception e)
                {
                    _trackErrors(e);
                    throw e;
                }
            }

            private void _trackErrors(Exception e)
            {
                _errorCount += 1;
                if (_errorCount > Config.CircuitClosedErrorLimit)
                {
                    _owner._state = new CircuitBreakerOpen(_owner);
                }
            }
        }

        private class CircuitBreakerOpen : CircuitBreakerState
        {

            public CircuitBreakerOpen(CircuitBreakerForStockMarketEngine owner)
                : base(owner)
            {
                new Timer(_ =>
                {
                    owner._state = new CircuitBreakerHalfOpen(owner);
                }, null, Config.CircuitOpenTimeout, Timeout.Infinite);
            }

            public override Task<List<object>> GetOrders()
            {
                throw new NotImplementedException();
            }


        }

        private class CircuitBreakerHalfOpen : CircuitBreakerState
        {
            private static readonly string Message = "Call failed when circuit half open";
            public CircuitBreakerHalfOpen(CircuitBreakerForStockMarketEngine owner)
                : base(owner) { }

            public override async Task<List<object>> GetOrders()
            {
                try
                {
                    using var httpClient = new HttpClient();
                    var result = httpClient.GetAsync("https://localhost:7092/api/Trades/GetAllTrades").GetAwaiter().GetResult();
                    var trades = await result.Content.ReadAsStringAsync();

                    if (result.StatusCode != HttpStatusCode.OK)
                    {
                        _owner._state = new CircuitBreakerOpen(_owner);
                    }

                    _owner._state = new CircuitBreakerClosed(_owner);
                    return JsonConvert.DeserializeObject<List<object>>(trades);
                }
                catch (Exception e)
                {
                    _owner._state = new CircuitBreakerOpen(_owner);
                    throw new Exception(Message, e);
                }
            }
        }

    }
}

