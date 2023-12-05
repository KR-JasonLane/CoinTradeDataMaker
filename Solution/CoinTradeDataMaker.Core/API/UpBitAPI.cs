using CoinTradeDataMaker.Core.Model;


namespace CoinTradeDataMaker.Core.API;

public class UpBitAPI
{
	public UpBitAPI(string accKey, string secKey)
	{
		AccKey = accKey;
		SecKey = secKey;
	}

	#region Properties

	/// <summary>
	/// Upbit Open API Access Key
	/// </summary>
	private string AccKey;

	/// <summary>
	/// Upbit Open API Secret Key
	/// </summary>
	private string SecKey;

	#endregion

	#region API Method

	/// <summary>
	/// Upbit 서버와의 연결이 유효한지 검사합니다.
	/// </summary>
	/// <returns> 연결시도 후 결과 반환 </returns>
	public bool ConnectionTest()
	{
		var response = ExecuteResponse(UpBitURL.ApiKeys);
		return response != null ? response.IsSuccessful : false;
	}

	/// <summary>
	/// URL을 사용하여 필요한 정보를 담은 객체를 가져옵니다.
	/// </summary>
	/// <param name="url"> API 접속 URL </param>
	/// <returns> 서버와 통신하여 얻은 RestResponse객체 반환 </returns>
	private RestResponse? ExecuteResponse(string url)
	{
		try
		{
			var payload = new JwtPayload
			{
				{ "access_key", AccKey },
				{ "nonce", Guid.NewGuid().ToString() },
				{ "query_hash_alg", "SHA512" }
			};

			byte[] keyBytes = Encoding.Default.GetBytes(SecKey);
			var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
			var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
			var header = new JwtHeader(credentials);
			var secToken = new JwtSecurityToken(header, payload);

			var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
			var authorizationToken = "Bearer " + jwtToken;

			var client = new RestClient(url);
			var request = new RestRequest();
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Authorization", authorizationToken);
			RestResponse response = client.Execute(request);

			return response;
		}
		catch
		{
			return null;
		}
	}

	/// <summary>
	/// 업비트에서 거래 가능한 마켓 목록 정보를 호출합니다.
	/// </summary>
	/// <returns> 업비트에서 거래 가능한 마켓 목록 정보 </returns>
	public List<UpBitMarket>? GetUpBitMarkets()
	{
		var response = ExecuteResponse(UpBitURL.Market);

		if(response!= null)
		{ 
			return JsonConvert.DeserializeObject<List<UpBitMarket>>(response.Content);
		}

		return null;
	}

	/// <summary>
	/// 업비트에서 현재가 정보를 호출합니다.
	/// </summary>
	/// <param name="additionalURL"> 조회할 마켓코드가 쉼표로 구분되어있는 문자열 입니다. ex)KRW-BTC,KRW-SOL </param>
	/// <returns> 현재가 정보 목록 </returns>
	public List<UpBitTicker>? GetUpBitTickers(string additionalURL)
	{
		var response = ExecuteResponse(UpBitURL.Ticker + additionalURL);

		if (response != null && response.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<List<UpBitTicker>>(response.Content);
		}

		return null;
	}

	/// <summary>
	/// 업비트에서 호가 정보를 호출합니다.
	/// </summary>
	/// <param name="additionalURL"> 조회할 마켓코드가 쉼표로 구분되어있는 문자열 입니다. ex)KRW-BTC,KRW-SOL </param>
	/// <returns> 호가 정보 목록 </returns>
	public List<UpBitOrderBook>? GetUpBitOrderBooks(string additionalURL)
	{
		var response = ExecuteResponse(UpBitURL.OrderBook  + additionalURL);

		if(response != null && response.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<List<UpBitOrderBook>>(response.Content);
		}

		return null;
	}

	#endregion
}
