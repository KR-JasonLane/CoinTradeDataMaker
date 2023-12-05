namespace CoinTradeDataMaker.Core.Model;

public class UpBitURL
{
	/// <summary>
	/// 사용가능한 API 키 조회
	/// </summary>
	public static readonly string ApiKeys = "https://api.upbit.com/v1/api_keys";

	/// <summary>
	/// 마켓 코드 조회
	/// </summary>
	public static readonly string Market = "https://api.upbit.com/v1/market/all";

	/// <summary>
	/// 현재가 정보
	/// 'markets=' 뒤에 조회하고자 하는 종목코드들을 반점으로 구분하여 작성
	/// ex) ...?markets=KRW-BTC,BTC-ETH
	/// </summary>
	public static readonly string Ticker = "https://api.upbit.com/v1/ticker?markets=";

	/// <summary>
	/// 호가 정보
	/// 'markets=' 뒤에 조회하고자 하는 종목코드들을 반점으로 구분하여 작성
	/// ex) ...?markets=KRW-BTC,BTC-ETH
	/// </summary>
	public static readonly string OrderBook = "https://api.upbit.com/v1/orderbook?markets=";
}
