namespace CoinTradeDataMaker.DataBase;

public class DataBaseManager
{

	public DataBaseManager(string dbFilePath)
	{
		DBFilePath = dbFilePath;

		ConnectString = $"Data Source={DBFilePath}";
	}

	#region Properties

	/// <summary>
	/// .db파일의 경로
	/// </summary>
	private string DBFilePath;

	/// <summary>
	/// .db파일을 연결할 수 있는 문자열
	/// </summary>
	private string ConnectString;

	/// <summary>
	/// 데이터베이스에 접근할 수 있는 객체
	/// </summary>
	private SQLiteConnection Connection;

	#endregion
	#region Methods

	/// <summary>
	/// .db파일을 만듭니다.
	/// </summary>
	/// <param name="command"> .db파일을 만들면서 기본으로 사용할 커맨드 </param>
	/// <exception cref="Exception"> .db파일을 만들면서 발생한 예외 </exception>
	public void CreateDataBase(string? command = null)
	{
		try
		{
			Connection = new SQLiteConnection(ConnectString);
			Connection.Open();

			if (string.IsNullOrEmpty(command) == false)
			{
				var cmd = new SQLiteCommand(command, Connection);
				cmd.ExecuteNonQuery();
			}
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
	}

	/// <summary>
	/// 데이터베이스 파일에 데이터를 입력
	/// </summary>
	/// <param name="table"> 테이블 이름 </param>
	/// <param name="columns"> 컬럼 </param>
	/// <param name="values"> 입력할 값 </param>
	/// <exception cref="Exception"></exception>
	public void Insert(string table, List<string> columns, List<string> values)
	{
		try
		{
			if(columns.Count != values.Count) throw new Exception("컬럼수와 값의 갯수가 일치하지 않습니다.");


			string query = $"INSERT INTO {table} (";

			for(int i = 0; i < columns.Count; i++)
			{
				query += columns[i];
				if(i != columns.Count - 1) query += ",";
				else query += ")";
			}

			query += " VALUES(";

			for(int i = 0; i < values.Count; i++)
			{
				query += $"'{values[i]}'";
				if(i != values.Count - 1) query += ",";
				else query += ")";
			}

			var command = new SQLiteCommand(query, Connection);
			command.ExecuteNonQuery();
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
	}

	#endregion
}
