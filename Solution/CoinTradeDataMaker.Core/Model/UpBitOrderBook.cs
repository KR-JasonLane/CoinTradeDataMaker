namespace CoinTradeDataMaker.Core.Model;

public class UpBitOrderBook
{
	/// <summary>
	/// 마켓 코드
	/// </summary>
	public string market { get; set; }

	/// <summary>
	/// 호가 생성 시각
	/// </summary>
	public long timestamp { get; set; }

	/// <summary>
	/// 호가 매도 총 잔량
	/// </summary>
	public double total_ask_size { get; set; }

	/// <summary>
	/// 호가 매수 총 잔량
	/// </summary>
	public double total_bid_size { get; set; }

	/// <summary>
	/// 호가
	/// 리스트에는 15호가 정보가 들어가며 차례대로 1호가, 2호가 ... 15호가의 정보를 담고있습니다.
	/// </summary>
	public List<object> orderbook_units { get; set; }

	/// <summary>
	/// 매도호가
	/// </summary>
	public double ask_price { get; set; }

	/// <summary>
	/// 매수호가
	/// </summary>
	public double bid_price { get; set;}

	/// <summary>
	/// 매도 잔량
	/// </summary>
	public double ask_size { get; set; }

	/// <summary>
	/// 매수 잔량
	/// </summary>
	public double bid_size { get; set;}
}
