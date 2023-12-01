namespace CoinTradeDataMaker.Core.Model;

public class UpBitTicker
{
	/* [change], [change_price], [change_rate], [signed_change_price], [signed_change_rate] 필드들은 전일종가 대비 값 */

	/// <summary>
	/// 종목 구분 코드
	/// </summary>
	public string market { get; set; }

	/// <summary>
	/// 최근 거래 일자 (UTC)
	/// 포맷 : yyyyMMdd
	/// </summary>
	public string trade_date { get; set; }

	/// <summary>
	/// 최근 거래 시각(UTC)
	/// 포맷 : HHmmss
	/// </summary>
	public string trade_time { get; set; }

	/// <summary>
	/// 최근 거래 일자(KST)
	/// 포맷 : yyyyMMdd
	/// </summary>
	public string trade_date_kst { get; set; }

	/// <summary>
	/// 최근 거래 시각(KST)
	/// 포맷 : HHmmss
	/// </summary>
	public string trade_time_kst { get; set; }

	/// <summary>
	/// 최근 거래 일시(UTC)
	/// 포맷 : Unix Timestamp
	/// </summary>
	public long trade_timestampe { get; set; }

	/// <summary>
	/// 시가
	/// </summary>
	public double opening_price { get; set; }

	/// <summary>
	/// 고가
	/// </summary>
	public double high_price { get; set; }

	/// <summary>
	/// 저가
	/// </summary>
	public double low_price { get; set; }

	/// <summary>
	/// 종가(현재가)
	/// </summary>
	public double trade_price { get; set; }

	/// <summary>
	/// 전일 종가(UTC 0시 기준)
	/// </summary>
	public double prev_closing_price { get; set; }

	/// <summary>
	/// EVEN : 보합 / RISE : 상승 / FALL : 하락
	/// </summary>
	public string change {  get; set; }

	/// <summary>
	/// 변화액의 절대값
	/// </summary>
	public double change_price { get; set; }

	/// <summary>
	/// 변화율의 절대값
	/// </summary>
	public double change_rate { get; set; }

	/// <summary>
	/// 부호가 있는 변화액
	/// </summary>
	public double signed_change_price { get; set; }

	/// <summary>
	/// 부호가 있는 변화율
	/// </summary>
	public double signed_change_rate { get; set; }

	/// <summary>
	/// 가장 최근 거래량
	/// </summary>
	public double trade_volume { get; set; }

	/// <summary>
	/// 누적 거래대금(UTC 0시 기준)
	/// </summary>
	public double acc_trade_price { get; set; }

	/// <summary>
	/// 24시간 누적 거래 대금
	/// </summary>
	public double acc_trade_price_24h { get; set; }

	/// <summary>
	/// 누적 거래량(UTC 0시 기준)
	/// </summary>
	public double acc_trade_volume { get; set; }

	/// <summary>
	/// 24시간 누적 거래량
	/// </summary>
	public double acc_trade_volume_24h { get; set; }

	/// <summary>
	/// 52주 신고가
	/// </summary>
	public double highst_52_week_price { get; set; }

	/// <summary>
	/// 52주 신고가 달성일
	/// 포맷 : yyyy-MM-dd
	/// </summary>
	public string highst_52_week_date { get; set; }

	/// <summary>
	/// 52주 신저가
	/// </summary>
	public double lowest_52_week_price { get; set; }

	/// <summary>
	/// 52주 신저가 달성일
	/// 포맷 : yyyy-MM-dd
	/// </summary>
	public string lowest_52_week_date { get; set; }

	/// <summary>
	/// 타임스태프
	/// </summary>
	public long timestamp { get; set; }
}
