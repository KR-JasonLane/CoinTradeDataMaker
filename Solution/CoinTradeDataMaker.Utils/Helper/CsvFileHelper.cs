namespace CoinTradeDataMaker.Utils.Helper;

public class CsvFileHelper
{
    public CsvFileHelper(string folderName, string fileName)
	{
		FolderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
		FilePath = Path.Combine(FolderPath, fileName);
	}

	#region Properties

	/// <summary>
	/// 폴더의 경로
	/// </summary>
	private string FolderPath;

	/// <summary>
	/// 파일의 경로
	/// </summary>
	public string FilePath { get; init; }

    #endregion

    #region Methods

    /// <summary>
    /// Csv파일을 작성합니다.
    /// </summary>
    /// <param name="data"> 데이터의 목록 </param>
	public void Write(params string[] datas)
    {
        // Use StreamWriter to write to the CSV file
        using (StreamWriter sw = new StreamWriter(FilePath, true))
        {
            // Write header (column names) if the file is empty
            sw.WriteLine(string.Join(",", datas));
        }
    }

    #endregion

}

