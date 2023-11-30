using CoinTradeDataMaker.Core.API;

namespace CoinTradeDataMaker;

public class Program
{
	private static UpBitAPI UpBit;

	public static void Main()
	{
		Console.Write("Access Key : ");
		string accKey = Console.ReadLine();

		Console.Write("Secret Key : ");
		string secKey = Console.ReadLine();

		UpBit = new UpBitAPI(accKey, secKey);

		if(UpBit.ConnectionTest() == true )
		{			
			while(true)
			{

			}
		}
	}
}