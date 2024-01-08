using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CCXT;
using Newtonsoft.Json;
using System.Reflection;
using CCXT.NET.Binance;

namespace app0.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    private readonly IHubContext<CounterHub> _hubContext;

    public IndexModel(ILogger<IndexModel> logger, IHubContext<CounterHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public void OnGet()
    {
        string baze = "tbtc"; //eth //tbtc
        string pair = "TBTCUSD"; //ETHUSDT //TBTCUSD
        // Create the task for getting Ether price every 10 seconds
        Task.Run(async () =>
        {
            try 
            {
                var _public_api = new CCXT.NET.Kraken.Public.PublicApi();
            
                while (true)
                {
                    var _tickers = await _public_api.FetchTickersAsync();
                    if (_tickers.success)
                    {
                        var _btcusd_tickers = _tickers.result.Where(t => t.symbol.ToUpper().Equals(pair));

                        foreach (var _t in _btcusd_tickers)
                        {
                            Console.Out.WriteLine($"symbol: {_t.symbol}, closePrice: {_t.closePrice}");
                            await _hubContext.Clients.All.SendAsync("ReceivePrice", _t.closePrice).ConfigureAwait(false);
                            
                        }
                            
                    }
                    else
                    {
                        Console.Out.WriteLine($"error: {_tickers.message}");
                    }
                    await Task.Delay(1000 * 10);
                    
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"We ran into an Exception on the current price: {ex}");
            }
            
        });

        // Compute average price for BTC in the last 10 minutes
        Task.Run(async () =>
        {
            try
            {
                var _public_api = new CCXT.NET.Kraken.Public.PublicApi();
                do
                {
                    int number_of_candles = 10;
                    decimal sumClosePrices = 0;
                    var _ohlcvs = await _public_api.FetchOHLCVsAsync(baze,"usd", "1m", 0, number_of_candles);
                    
                    foreach (var _candle in _ohlcvs.result)
                    {
                        sumClosePrices += _candle.closePrice;
                    }
                    decimal averageClosePrice = _ohlcvs.result.Count > 0 ? sumClosePrices / _ohlcvs.result.Count : 0;
                    await _hubContext.Clients.All.SendAsync("ReceiveCounter", averageClosePrice).ConfigureAwait(false);
                    Console.Out.WriteLine($"Average Close Price in the last {number_of_candles} minutes = {averageClosePrice}");
                    Console.Out.WriteLine($"The sum was {sumClosePrices}");
                    
                    
                    await Task.Delay(1000 * 10);
                }
                while (true);
                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"We ran into an Exception on the counter/average price: {ex}");
            }
            

        });

        
    }
}
