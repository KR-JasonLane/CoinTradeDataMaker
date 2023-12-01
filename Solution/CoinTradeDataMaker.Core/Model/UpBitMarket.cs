namespace CoinTradeDataMaker.Core.Model;

public class UpBitMarket
{
	/// <summary>
	/// 업비트에서 제공중인 시장 정보
	/// </summary>
	public string market { get; set; }

	/// <summary>
	/// 거래 대상 디지털 자산 한글명
	/// </summary>
	public string korean_name { get; set; }

	/// <summary>
	/// 거래 대상 디지털 자산 영문명
	/// </summary>
	public string english_name { get; set; }

	/// <summary>
	/// 유의종목 여부
	/// NONE(해당사항 없음), CAUTION(투자유의)
	/// </summary>
	public string market_warning { get; set; }
}
