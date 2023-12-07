using CoinTradeDataMaker.Utils.Helper;

using CoinTradeDataMaker.Core.API;
using CoinTradeDataMaker.Core.Model;
using CoinTradeDataMaker.DataBase;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace CoinTradeDataMaker;

public class Program
{
	/// <summary>
	/// 프로그램의 메인 함수
	/// </summary>
	public static void Main()
	{
		DBTableName = "TrainingDataTable";
		LossDataCount = 0;

		//키 입력 받아서 API 객체 생성하기
		InPutKeyAndCreateAPIObject();

		//예측데이터 딜레이 입력받기
		InputMilliSecond();

		//작업 시작
		DoWork();
	}

	#region Properties

	/// <summary>
	/// Upbit 서버와 통신하기 위해 API를 핸들링 하는 객체
	/// </summary>
	private static UpBitAPI? UpBit;

	/// <summary>
	/// 이전정보 조회 후 몇초 뒤 정보를 예측하는 데이터를 만들건지 정함
	/// </summary>
	private static int MilliSecondForSleep;

	/// <summary>
	/// 결과데이터를 저장할 데이터베이스 매니저
	/// </summary>
	private static DataBaseManager? DB;

	/// <summary>
	/// 데이터베이스에서 참조하는 테이블 이름을 기억
	/// </summary>
	private static string DBTableName;

	/// <summary>
	/// 손실데이터 갯수를 나타냄
	/// </summary>
	private static int LossDataCount;

	#endregion

	#region Work Method

	/// <summary>
	/// 사용자에게 발급받은 API키를 입력받고, 유효한 UpBitAPI객체를 생성
	/// </summary>
	private static void InPutKeyAndCreateAPIObject()
	{
		while (true)
		{
			//Access Key 입력받음
			Console.Write("\nAccess Key : ");
			string accKey = Console.ReadLine()!;

			//Secret Key 입력받음
			Console.Write("Secret Key : ");
			string secKey = Console.ReadLine()!;

			//임시 API 핸들링 객체 생성
			UpBitAPI tempUpBit = new UpBitAPI(accKey!, secKey!);

			// 연결 테스트 실패 시 키 입력 재시도
			if (tempUpBit.ConnectionTest() == false)
			{
				string message = string.Empty;
				message += "연결에 실패하였습니다.\n\n";
				message += "1. 발급받은 Access key값과 Seceret Key값을 확인해 주세요.\n";
				message += "2. 발급받은 API에 올바른 IP가 입력되어있는지 확인해 주세요.";

				WriteHeaderLog(message);
				continue;
			}
			else
			{
				//연결에 성공한 객체를 기억
				UpBit = tempUpBit;
				break;
			}
		}
	}

	/// <summary>
	/// 사용자에게 딜레이를 입력받아, 시세 조회 후 몇 밀리세컨드 이후의 예측데이터를 만들것인지 결정
	/// </summary>
	private static void InputMilliSecond()
	{
		//사용자에게 입력받은 지연시간 저장
		MilliSecondForSleep = 0;

		//유효한 데이터가 들어올 때 까지 반복
		//환경마다 다르겠지만, 쓰레드의 안정성을 위해 지연시간을 1초이상으로 제한
		while (MilliSecondForSleep < 1000)
		{
			string message = string.Empty;
			message += "지연시간 설정 \n";
			message += "1초 이상의 값 입력.\n\n";
			message += $"(1초 = 1000ms)";

			WriteHeaderLog(message);

			Console.Write("예측값 지연시간 (단위 : ms) : ");

			// 정수형으로 파싱하지 못하면 처음으로
			try { MilliSecondForSleep = int.Parse(Console.ReadLine()!); }
			catch { continue; }
		}
	}

	/// <summary>
	/// UpBitAPI객체가 생성된 후 데이터수집을 시작
	/// </summary>
	private static void DoWork()
	{
		//현재가 조회에 사용할 URL의 추가부분을 만든다.
		string additionalTickerUrl = MakeAdditionalTickerURL();

		//파일 관리객체 생성
		FileHelper fileHelper = new FileHelper("Output", "TrainingData.db");

		//폴더 준비상태 확인
		if (fileHelper.IsFolderReady() == false)
		{
			WriteErrorLog("Error : 폴더 찾기 실패");
			return;
		}

		//데이터베이스 매니저 준비
		DB = new DataBaseManager(fileHelper.FilePath);

		//결과 파일 준비상태 확인
		if (fileHelper.IsFileReady() == false)
		{
			string createQuery = string.Empty;
			createQuery += $"CREATE TABLE {DBTableName}(";
			createQuery += "current_time TEXT,";
			createQuery += "market TEXT,";
			createQuery += "btc_trade_price TEXT,";
			createQuery += "eth_trade_price TEXT,";
			createQuery += "trade_date_kst TEXT,";
			createQuery += "trade_time_kst TEXT,";
			createQuery += "opening_price TEXT,";
			createQuery += "high_price TEXT,";
			createQuery += "low_price TEXT,";
			createQuery += "trade_price TEXT,";
			createQuery += "prev_closing_price TEXT,";
			createQuery += "change TEXT,";
			createQuery += "change_price TEXT,";
			createQuery += "change_rate TEXT,";
			createQuery += "signed_change_price TEXT,";
			createQuery += "signed_change_rate TEXT,";
			createQuery += "trade_volume TEXT,";
			createQuery += "acc_trade_price TEXT,";
			createQuery += "acc_trade_price_24h TEXT,";
			createQuery += "acc_trade_volume TEXT,";
			createQuery += "acc_trade_volume_24h TEXT,";
			createQuery += "highst_52_week_price TEXT,";
			createQuery += "highst_52_week_date TEXT,";
			createQuery += "lowest_52_week_price TEXT,";
			createQuery += "lowest_52_week_date TEXT,";
			createQuery += "result TEXT)";

			//DB파일 생성
			try { DB.CreateDataBase(createQuery); }
			catch
			{
				WriteErrorLog("Error : 데이터베이스 파일 생성 실패");
				return;
			}
		}

		//과거 데이터
		List<UpBitTicker> oldTickers = null;

		//반복작업 시작
		while (true)
		{
			//설정한 시간만큼 딜레이
			Thread.Sleep(MilliSecondForSleep);

			//과거데이터가 없으면 과거데이터만 생성 후 다시 작업진행
			if (oldTickers == null)
			{
				try
				{
					oldTickers = UpBit.GetUpBitTickers(additionalTickerUrl)!;
				}
				catch (Exception e)
				{
					WriteErrorLog($"Error : {e.Message}");
				}
				continue;
			}

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			List<UpBitTicker>? curTickers = null;

			try
			{
				//최신 현재가, 호가 조회
				curTickers = UpBit.GetUpBitTickers(additionalTickerUrl)!;
			}
			catch (Exception e)
			{
				WriteErrorLog($"Error : {e.Message}");
				continue;
			}

			//쓰레드풀에 데이터 쓰기 작업 등록
			ThreadPool.QueueUserWorkItem((callback) => AddTrainingData(curTickers, oldTickers));

			//시스템 지연시간 측정
			stopwatch.Stop();

			//현재가 과거데이터 업데이트
			oldTickers = curTickers!;

			string message = string.Empty;
			message += $"작업결과\n";
			message += $" - 설정 지연시간 : {MilliSecondForSleep}ms\n";
			message += $" - 시스템 지연시간 : {stopwatch.ElapsedMilliseconds}ms\n\n";
			message += $" * 총 지연시간 : {MilliSecondForSleep + stopwatch.ElapsedMilliseconds}ms\n";
			message += $" * 총 손실데이터 : {LossDataCount}개";

			WriteNormalLog(message);
		}
	}

	/// <summary>
	/// 데이터베이스에 데이터 입력
	/// </summary>
	/// <param name="trainingData"> 값을 추출해 내기 위한 TrainingData객체 </param>
	private static void InputDataInDB(TrainingData trainingData)
	{
		List<string> columns = new List<string>()
		{
			"market",
			"btc_trade_price",
			"eth_trade_price",
			"trade_date_kst",
			"trade_time_kst",
			"opening_price",
			"high_price",
			"low_price",
			"trade_price",
			"prev_closing_price",
			"change",
			"change_price",
			"change_rate",
			"signed_change_price",
			"signed_change_rate",
			"trade_volume",
			"acc_trade_price",
			"acc_trade_price_24h",
			"acc_trade_volume",
			"acc_trade_volume_24h",
			"highst_52_week_price",
			"highst_52_week_date",
			"lowest_52_week_price",
			"lowest_52_week_date",
			"result",
		};

		List<string> values = new List<string>()
		{ 		
			$"{trainingData.market}",
			$"{trainingData.btc_trade_price}",
			$"{trainingData.eth_trade_price}",
			$"{trainingData.trade_date_kst}",
			$"{trainingData.trade_time_kst}",
			$"{trainingData.opening_price}",
			$"{trainingData.high_price}",
			$"{trainingData.low_price}",
			$"{trainingData.trade_price}",
			$"{trainingData.prev_closing_price}",
			$"{trainingData.change}",
			$"{trainingData.change_price}",
			$"{trainingData.change_rate}",
			$"{trainingData.signed_change_price}",
			$"{trainingData.signed_change_rate}",
			$"{trainingData.trade_volume}",
			$"{trainingData.acc_trade_price}",
			$"{trainingData.acc_trade_price_24h}",
			$"{trainingData.acc_trade_volume}",
			$"{trainingData.acc_trade_volume_24h}",
			$"{trainingData.highest_52_week_price}",
			$"{trainingData.highest_52_week_date}",
			$"{trainingData.lowest_52_week_price}",
			$"{trainingData.lowest_52_week_date}",
			$"{trainingData.result}"
		};

		try
		{
			DB!.Insert(DBTableName, columns, values);
		}
		catch (Exception e)
		{
			WriteErrorLog($"{trainingData.market}데이터 손실!\n에러로그 : {e.Message}");
			LossDataCount++;
		}
		
	}

	/// <summary>
	/// 참고 데이터를 받아 훈련데이터를 생성
	/// </summary>
	/// <param name="curTickers"> 최신 현재가 정보 </param>
	/// <param name="oldTickers"> 이전 현재가 정보 </param>
	private static void AddTrainingData(List<UpBitTicker> curTickers, List<UpBitTicker> oldTickers)
	{
		//비트코인, 이더리움 가격을 추출
		double btc_trade_price = oldTickers.Find(x => x.market == "KRW-BTC")!.trade_price;
		double eth_trade_price = oldTickers.Find(x => x.market == "KRW-ETH")!.trade_price;

		for (int i = 0; i < curTickers.Count; i++)
		{
			TrainingData curData = new TrainingData()
			{
				market					=	oldTickers[i].market,
				btc_trade_price			=	btc_trade_price,
				eth_trade_price			=	eth_trade_price,
				trade_date_kst			=	oldTickers[i].trade_date_kst,
				trade_time_kst			=	oldTickers[i].trade_time_kst,
				opening_price			=	oldTickers[i].opening_price,
				high_price				=	oldTickers[i].high_price,
				low_price				=	oldTickers[i].low_price,
				trade_price				=	oldTickers[i].trade_price,
				prev_closing_price		=	oldTickers[i].prev_closing_price,
				change					=	oldTickers[i].change,
				change_price			=	oldTickers[i].change_price,
				change_rate				=	oldTickers[i].change_rate,
				signed_change_price		=	oldTickers[i].signed_change_price,
				signed_change_rate		=	oldTickers[i].signed_change_rate,
				trade_volume			=	oldTickers[i].trade_volume,
				acc_trade_price			=	oldTickers[i].acc_trade_price,
				acc_trade_price_24h		=	oldTickers[i].acc_trade_price_24h,
				acc_trade_volume		=	oldTickers[i].acc_trade_volume,
				acc_trade_volume_24h	=	oldTickers[i].acc_trade_volume_24h,
				highest_52_week_price	=	oldTickers[i].highest_52_week_price,
				highest_52_week_date	=	oldTickers[i].highest_52_week_date,
				lowest_52_week_price	=	oldTickers[i].lowest_52_week_price,
				lowest_52_week_date		=	oldTickers[i].lowest_52_week_date,
				result					=	curTickers[i].trade_price
			};

			InputDataInDB(curData);
		}

	}

	/// <summary>
	/// 필요한 항목의 시세조회를 위해 종목코드를 삽입한 문자열을 만듬
	/// </summary>
	/// <returns> KRW 종목코드만을 검색하여 만들어낸 문자열. </returns>
	private static string MakeAdditionalTickerURL()
	{
		//반환할 문자열
		string result = string.Empty;

		//로그작성
		DateTime now = DateTime.Now;
		WriteHeaderLog($"[{now.ToString("yyyy년 MM월 dd일(ddd)")}]  마켓정보 조회");

		//마켓데이터를 불러온다.
		List<UpBitMarket> markets = UpBit!.GetUpBitMarkets()!;

		if (markets != null)
		{
			foreach (var market in markets)
			{
				// BTC 형태의 종목은 포함하지 않음. (BTC = 비트코인거래 / KRW = 현금거래)
				if (market.market.Contains("KRW-") == true)
				{
					//종목코드와 쉼표 삽입
					result += $"{market.market},";
				}
			}

			if (result != string.Empty)
			{
				//맨 마지막 쉼표는 삭제
				result = result.TrimEnd(',');
			}
		}

		return result;
	}

	#endregion

	#region Write Log

	/// <summary>
	/// 콘솔에 강조하는 파티션이 포함 된 로그를 작성
	/// 헤더는 무조건 콘솔을 정리하고 로그를 작성
	/// </summary>
	/// <param name="text"> 파티션 사이에 들어갈 메시지 </param>
	private static void WriteHeaderLog(string text)
	{
		//콘솔창 정리
		Console.Clear();

		Console.WriteLine();
		Console.WriteLine("==========================================================================");
		Console.WriteLine($"\n{text}\n");
		Console.WriteLine("==========================================================================");
		Console.WriteLine();
	}

	/// <summary>
	/// 콘솔에 일반 파티션이 포함 된 로그를 작성
	/// </summary>
	/// <param name="text"> 파티션 사이에 들어갈 메시지 </param>
	private static void WriteNormalLog(string text)
	{
		DateTime now = DateTime.Now;

		Console.WriteLine();
		Console.WriteLine("-----------------------------------------");
		Console.WriteLine($"[{now.ToString("yyyy/MM/dd hh:mm:ss")}]");
		Console.WriteLine($"\n{text}\n");
		Console.WriteLine("-----------------------------------------");
		Console.WriteLine();
	}

	/// <summary>
	/// 콘솔에 파티션이 포함 된 에러로그를 작성
	/// </summary>
	/// <param name="text"> 파티션 사이에 들어갈 메시지 </param>
	private static void WriteErrorLog(string text)
	{
		DateTime now = DateTime.Now;

		Console.WriteLine();
		Console.WriteLine("*****************************************");
		Console.WriteLine($"[{now.ToString("yyyy/MM/dd hh:mm:ss")}]");
		Console.WriteLine($"\n{text}\n");
		Console.WriteLine("*****************************************");
		Console.WriteLine();
	}

	#endregion
}