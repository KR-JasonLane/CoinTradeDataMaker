using CoinTradeDataMaker.Utils.Helper;

using CoinTradeDataMaker.Core.API;
using CoinTradeDataMaker.Core.Model;

namespace CoinTradeDataMaker;

public class Program
{
	/// <summary>
	/// 프로그램의 메인 함수입니다.
	/// </summary>
	public static void Main()
	{
		//작업 종료를 위한 [ctrl + c] 키이벤트
		Console.CancelKeyPress += new ConsoleCancelEventHandler((sender, args) =>
		{
			//작업 반복 플래그 내려줌
			IsContinueWork = false;

			/*
			 * 여기에 마무리 작업 로직 추가
			 */

			//콘솔종료
			args.Cancel = true;
		});

		//키 입력 받아서 API 객체 생성하기
		InPutKeyAndCreateAPIObject();

		//예측데이터 딜레이 입력받기
		InputMilliSecond();

		//작업 시작
		DoWork();
	}

	#region Properties

	/// <summary>
	/// Upbit 서버와 통신하기 위해 API를 핸들링 하는 객체 입니다.
	/// </summary>
	private static UpBitAPI? UpBit;

	/// <summary>
	/// 데이터 수집 작업의 반복플래그
	/// </summary>
	private static bool IsContinueWork;

	/// <summary>
	/// 이전정보 조회 후 몇초 뒤 정보를 예측하는 데이터를 만들건지 정함.
	/// </summary>
	private static int MilliSecondForSleep;

	#endregion

	#region Work Method

	/// <summary>
	/// 사용자에게 발급받은 API키를 입력받고, 유효한 UpBitAPI객체를 생성합니다.
	/// </summary>
	private static void InPutKeyAndCreateAPIObject()
	{
		while (true)
		{
			//Access Key 입력받음
			Console.Write("\nAccess Key : ");
			string accKey = Console.ReadLine();

			//Secret Key 입력받음
			Console.Write("Secret Key : ");
			string secKey = Console.ReadLine();

			//임시 API 핸들링 객체 생성
			UpBitAPI tempUpBit = new UpBitAPI(accKey, secKey);

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
	/// 사용자에게 딜레이를 입력받아, 시세 조회 후 몇 밀리세컨드 이후의 예측데이터를 만들것인지 결정합니다.
	/// </summary>
	private static void InputMilliSecond()
	{
		MilliSecondForSleep = 0;

		while (MilliSecondForSleep <= 33)
		{
			string message = string.Empty;
			message += "예측값 간격 설정 \n\n";
			message += "업비트 API 정책 상 초당 30회 이상 조회요청을 할 수 없습니다.\n";
			message += "34 밀리세컨드 이상의 값을 입력해 주세요. (1초 = 1000ms)";

			WriteHeaderLog(message);

			Console.Write("예측값 간격 (단위 : ms) : ");

			try		{ MilliSecondForSleep = int.Parse(Console.ReadLine()); }
			catch	{ continue; }
		}
	}

	/// <summary>
	/// UpBitAPI객체가 생성된 후 데이터수집을 시작합니다.
	/// </summary>
	private static void DoWork()
	{
		//작업 반복 플래그 켜줌
		IsContinueWork = true;

		//현재가 조회에 사용할 URL의 추가부분을 만든다.
		string additionalTickerUrl = MakeAdditionalTickerURL();

		//과거 데이터가 될 현재가를 조회한다.
		List<UpBitTicker> oldTickers = UpBit.GetUpBitTickers(additionalTickerUrl);

		//파일 관리객체 생성
		FileHelper fileHelper = new FileHelper("Output", "TrainingData.csv");

		//폴더 준비상태 확인
		if (fileHelper.IsFolderReady() == false)
		{
			WriteNormalLog("Error : 폴더 찾기 실패");
			return;
		}
		
		//결과 파일 준비상태 확인
		if(fileHelper.IsFileReady() == false)
		{
			WriteNormalLog("Error : 파일 찾기 실패");
			return;
		}

		//반복작업 시작
		while (IsContinueWork == true)
		{
			//설정한 시간만큼 딜레이
			Thread.Sleep(MilliSecondForSleep);

			//최신 현재가 조회
			List<UpBitTicker> curTickers = UpBit.GetUpBitTickers(additionalTickerUrl);


			/* 여기서 데이터를 저장 */
		}
	}

	/// <summary>
	/// 필요한 항목의 시세조회를 위해 종목코드를 삽입한 문자열을 만들어 냅니다.
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
		List<UpBitMarket> markets = UpBit.GetUpBitMarkets();

		if(markets != null )
		{
			foreach (var market in markets)
			{
				// BTC 형태의 종목은 포함하지 않음. (BTC = 비트코인거래 / KRW = 현금거래)
				if(market.market.Contains("KRW-") == true)
				{
					//종목코드와 쉼표 삽입
					result += $"{market.market},";
				}
			}

			if(result != string.Empty)
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
	/// 콘솔에 강조하는 파티션이 포함 된 로그를 작성합니다.
	/// 헤더는 무조건 콘솔을 정리하고 로그를 작성합니다.
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
	/// 콘솔에 일반 파티션이 포함 된 로그를 작성합니다.
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

	#endregion
}