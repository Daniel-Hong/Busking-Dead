using System.Collections.Generic;

namespace GameTable
{
	public class TableSkill : TableBase
	{
		public enum SkillType
		{
			Single	= 0,
		}

		public class Data
		{
			public int id;
			public SkillType type;
			public int ap;
			public float cooltime;
		}

		public Dictionary<int, Data> dictionaryData;

		void Awake()
		{
			dictionaryData = new Dictionary<int, Data>();
			var csv = TableManager.Instance.GetCSVLoader(this.GetType());
			Read(csv);
		}
		public override void Read(CSVLoader csvLoader)
		{
			dictionaryData.Clear();

			int colIdx, stringId;
			string enumString;

			for (int i = 0; i < csvLoader.Rows; ++i)
			{
				colIdx = 0;
				Data newData = new Data();

				csvLoader.ReadValue(colIdx++, i, 0, out newData.id);
				csvLoader.ReadValue(colIdx++, i, "", out enumString);
				if (Common.TryParseEnum(enumString, out newData.type) != true)
				{
					throw new System.Exception(string.Format("Table Skill unkown enum string:{0}, column:{1}", enumString, i + 1));
				}
				csvLoader.ReadValue(colIdx++, i, 0, out newData.ap);
				csvLoader.ReadValue(colIdx++, i, 0, out newData.cooltime);

				dictionaryData.Add(newData.id, newData);
			}
		}

		public override void ReLoadTableData()
		{
			var csv = TableManager.Instance.GetCSVLoader(this.GetType());
			Read(csv);
		}

		public bool IsContainsKey(int id)
		{
			return dictionaryData.ContainsKey(id);
		}

		public Data GetData(int id)
		{
			if (IsContainsKey(id) == false)
			{
				return null;
			}

			return dictionaryData[id];
		}
	}
}