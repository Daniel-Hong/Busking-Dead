using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UniRx;
using GameTable;

public class TableManager : MonoGlobalSingleton<TableManager>
{
    private const string ASSET_PATH = "Table/";
    private Dictionary<System.Type, CSVLoader> _tables;

    /// <summary>
    /// 로딩 체크를 위한 리스트
    /// </summary>
    private List<string> _tablePathList = new List<string>();
    private List<TableBase> _tableComponentList = new List<TableBase>();

    private int _loadingCount;

    public bool LoadComplete { get; private set; }

    public IEnumerator LoadPatch()
    {
        yield break;
    }

    public void UnloadPatch()
    {
    }

    protected override void Init()
    {
        _tables = new Dictionary<System.Type, CSVLoader>();
        LoadComplete = false;
        _loadingCount = 0;
    }

    T LoadTableImmediate<T>() where T : TableBase
    {
        var fName = typeof(T).Name.Replace("Table", "");
        string path = string.Format("{0}{1}", ASSET_PATH, fName);
        var textAsset = Resources.Load<TextAsset>(path);

        if(textAsset == null)
        {
            Debug.LogErrorFormat("Not Found Table : {0}", path);            
        }

        var csvLoader = new CSVLoader();
        csvLoader.SecuredLoad(textAsset.bytes);
        if (!_tables.ContainsKey(typeof(T)))
        {
            _tables.Add(typeof(T), csvLoader);
            _tableComponentList.Add(this.gameObject.AddComponent<T>());
            return this.gameObject.GetComponent<T>();                    
        }
        else
        {
            Debug.LogWarningFormat("Alreay Load Table : {0}", typeof(T).Name);
            return this.gameObject.GetComponent<T>();
        }

    }

    public void ReLoadTable()
    {
        for (int i = 0; i < _tableComponentList.Count; i++)
        {
            _tableComponentList[i].ReLoadTableData();
        }
    }

    public void Load(System.Action callback = null)
    {
        if (LoadComplete)
        {
            if (callback != null)
                callback.Invoke();

            return;
        }

        if (_loadingCount > 0)
        {
            Debug.LogWarningFormat("Now Table Loading");
            return;
        }

        const string findNamespace = "GameTable";
        var tables = (from t in Assembly.GetExecutingAssembly().GetTypes()
                      where t.IsClass && t.Namespace == findNamespace && t.IsSubclassOf(typeof(TableBase))
                      select t).ToList();
        
        _tablePathList = (from table in tables
                          select string.Format("{0}{1}", ASSET_PATH, table.Name)).ToList();


        _loadingCount = tables.Count;
        if (tables.Count > 0)
        {
            foreach (var tableType in tables)
            {
                LoadPrefab(tableType.Name).Do(o => OnLoadComplete(tableType, o))
                .Do(_ => --_loadingCount).Where(_ => _loadingCount == 0).Do(_ => OnPreLoadTable(tables))
                .TakeWhile(_=> LoadComplete == false)
                .Subscribe(_ =>
                {
                    LoadComplete = true;
                    _tablePathList = null;
                    if (callback != null)
                    {
                        callback.Invoke();
                }
                });
            }
        }
        else
        {
            LoadComplete = true;
            _tablePathList = null;
            if (callback != null)
            {
                callback.Invoke();
            }

        }
    }

    IObservable<Object> LoadPrefab(string fileName)
    {
        var fName = fileName.Replace("Table", "");
        string path = string.Format("{0}{1}", ASSET_PATH, fName);
        return Observable.FromCoroutine<Object>(Observer => ResourceLoader.Instance.Load<TextAsset>(Observer, path));
    }

    void OnLoadComplete(System.Type t, Object o)
    {
        var textAsset = o as TextAsset;
        var csvLoader = new CSVLoader();
        csvLoader.SecuredLoad(textAsset.bytes);
        if(!_tables.ContainsKey(t))
        {
            _tables.Add(t, csvLoader);
        }
        
    }

    void OnPreLoadTable(List<System.Type> list)
    {
        PriorityTableSort(list);
        
        for (int i = 0; i < list.Count; ++i)
        {            
            _tableComponentList.Add(this.gameObject.AddComponent(list[i]) as TableBase);
        }
    }

    void PriorityTableSort(List<System.Type> list)
    {
        // var temp1 = list[0];
        // var index = list.FindIndex(x => x.Name == "TableString");
        // list[0] = typeof(TableString);
        // list[index] = temp1;

        // var temp2 = list[1];
        // index = list.FindIndex(x => x.Name == "TableCommonString");
        // list[1] = typeof(TableCommonString);
        // list[index] = temp2;

        // var temp3 = list[2];
        // index = list.FindIndex(x => x.Name == "TableDefine");
        // list[2] = typeof(TableDefine);
        // list[index] = temp3;
    }

    public CSVLoader GetCSVLoader(System.Type t)
    {
        CSVLoader csv = new CSVLoader();
        if (_tables.TryGetValue(t, out csv))
        {
            return csv;
        }

        return null;
    }

    bool Unload<T>() where T : TableBase
    {
        T tableBase = this.GetComponent<T>();
        if (tableBase == null)
            return false;

        GameObject.Destroy(tableBase);

        return true;
    }

    public bool IsLoaded<T>() where T : TableBase
    {
        T tableBase = this.GetComponent<T>();
        if (tableBase == null)
            return false;

        return tableBase.IsLoaded;
    }

    public T Get<T>() where T : TableBase
    {
        T tableBase = this.GetComponent<T>();
        if (tableBase == null)
        {
            Debug.LogWarningFormat("No PreLoad Table Name : {0}", typeof(T).Name);
            return LoadTableImmediate<T>();
        }

        return tableBase;
    }

    float value;
    public float GetProgress()
    {
        if (LoadComplete)
        {
            return 1;
        }
        else
        {
            value = 0.0f;
            if (_tablePathList.Count > 0)
            {
                for (int i = 0; i < _tablePathList.Count; ++i)
                {
                    value += ResourceLoader.Instance.GetProgress(_tablePathList[i]);
                }

                if (value > 0.0f)
                {
                    value /= (float)_tablePathList.Count;
                }
            }

            return value;
        }
    }
}
