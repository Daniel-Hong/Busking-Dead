using System.IO;
using UnityEngine;
using UnityEditor;
using GameTable;

public class ImportTableBianry
{
	[MenuItem("Utils/Table/Create Binary Table")]
	static void Execute()
	{
		string directoryPath = Application.dataPath + "/Resources/Table";
		if (!Directory.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}

		string filePath = Application.dataPath + "/Table";
		string[] files = Directory.GetFiles(directoryPath);
		int cnt = 0;
			
		foreach (string fileName in files)
		{
			if (fileName.EndsWith(".bytes") || fileName.EndsWith(".bytes.meta"))
			{
				File.Delete(fileName);
			}
		}
		
		if (Directory.Exists(filePath) == false)
		{
			Debug.LogError(filePath + " no exist");

			return;
		}

		files = Directory.GetFiles(filePath);
			
		foreach (string fileName in files)
		{
			if (Path.GetExtension(fileName) == ".txt")
			{
				CSVLoader csvLoader = new CSVLoader();
					
				if (!csvLoader.LoadFromFile(filePath + "/" + Path.GetFileName(fileName)))
				{
					Debug.Log("Failed Load : " + fileName);
				}
					
				if (!csvLoader.SecuredSave(directoryPath + "/" + Path.GetFileName(fileName)))
				{
					Debug.Log("Failed Save : " + fileName);
				}
				else
				{
					cnt++;
				}
			}
		}
			
		Debug.Log("Create Table Binary File : " + cnt.ToString());
	}
}