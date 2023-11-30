using CoinTradeDataMaker.Core.Model;

using System.Linq.Expressions;

namespace CoinTradeDataMaker.Core.API;
public class UpBitAPI
{
	/// <summary>
	/// Upbit Open API Access Key
	/// </summary>
	private string AccKey;

	/// <summary>
	/// Upbit Open API Secret Key
	/// </summary>
	private string SecKey;

	public UpBitAPI(string accKey, string secKey)
	{
		AccKey = accKey;
		SecKey = secKey;
	}

	public bool ConnectionTest()
	{
		var response = ExecuteResponse(UpBitURL.ApiKeys);

		if (response != null)
		{
			return response.IsSuccessStatusCode;
		}
		else
		{
			return false;
		}
	}

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
}
