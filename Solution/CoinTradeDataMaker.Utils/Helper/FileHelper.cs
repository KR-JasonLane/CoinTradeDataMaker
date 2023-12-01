namespace CoinTradeDataMaker.Utils.Helper;

public class FileHelper
{
	public FileHelper(string folderName, string fileName)
	{
		FolderName = folderName;
		FileName = fileName;

		FolderPath = Path.Combine(Directory.GetCurrentDirectory(), FolderName);
		FilePath = Path.Combine(FolderPath, FileName);
	}

	#region Properties

	/// <summary>
	/// 어플리케이션 실행 지점에 존재하는 폴더 이름입니다.
	/// </summary>
	private string FolderName;

	/// <summary>
	/// 어플리케이션 실행지점에 존재하는 하위폴더 내 파일 이름입니다.
	/// </summary>
	private string FileName;

	/// <summary>
	/// 폴더의 주소값입니다.
	/// </summary>
	private string FolderPath;

	/// <summary>
	/// 파일의 주소값입니다.
	/// </summary>
	private string FilePath;

	#endregion

	#region Work Method

	/// <summary>
	/// 폴더가 생성되어 있는지 여부를 확인하고,
	/// 폴더가 존재하지 않으면 생성을 시도합니다.
	/// </summary>
	/// <returns> 폴더의 존재여부를 반환합니다. </returns>
	public bool IsFolderReady()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(FolderPath);

		if (directoryInfo.Exists == false)
		{
			try
			{
				directoryInfo.Create();
				return true;
			}
			catch
			{
				return false;
			}
		}
		else
		{
			return true;
		}
	}

	/// <summary>
	/// 파일이 생성되어 있는지 여부를 확인하고,
	/// 파일이 존재하지 않으면 생성을 시도합니다.
	/// </summary>
	/// <returns>파일의 존재 여부를 반환합니다. </returns>
	public bool IsFileReady()
	{
		if (File.Exists(FilePath) == false)
		{
			try
			{
				using (File.Create(FilePath))
				{
					return true;
				}
			}
			catch
			{
				return false;
			}
		}
		else
		{
			return true;
		}
	}

	#endregion
}
