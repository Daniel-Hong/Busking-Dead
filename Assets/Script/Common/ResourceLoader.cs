using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;


public class ResourceLoader : MonoGlobalSingleton<ResourceLoader>
{
    public class LoadedResource
    {
        public UnityEngine.Object m_Resource;
        public int m_ReferencedCount;

        public LoadedResource(UnityEngine.Object resource)
        {
            m_Resource = resource;
            m_ReferencedCount = 1;
        }
    }

    Dictionary<string, LoadedResource> loadedResources = new Dictionary<string, LoadedResource>();
    Dictionary<string, ResourceRequest> inProgressOperations = new Dictionary<string, ResourceRequest>();


    public IEnumerator Load<T>(IObserver<Object> obs, string resourceName)
    {
        while (inProgressOperations.ContainsKey(resourceName))
            yield return null;

        if (loadedResources.ContainsKey(resourceName))
        {
            var resource = loadedResources[resourceName];
            if (resource != null && resource.m_Resource != null)
            {
                resource.m_ReferencedCount++;

                //Debug.LogFormat("{0} is loaded successfully at frame {1} : [RefCount] {2}", resourceName, Time.frameCount, resource.m_ReferencedCount);
                obs.OnNext(resource.m_Resource);
                obs.OnCompleted();
            }
            else
            {
                obs.OnError(new System.Exception("Resource already loaded but actual data is null. Name: " + resourceName));
            }
        }
        else
        {
            ResourceRequest request = Resources.LoadAsync(resourceName, typeof(T));

            inProgressOperations.Add(resourceName, request);
            
            yield return request;
            
            inProgressOperations.Remove(resourceName);

            if (request.asset != null)
            {
                var resource = new LoadedResource(request.asset);
                loadedResources.Add(resourceName, resource);

                //Debug.LogFormat("{0} is loaded successfully at frame {1} : [RefCount] {2}", resourceName, Time.frameCount, resource.m_ReferencedCount);
                obs.OnNext(request.asset);
                obs.OnCompleted();
            }
            else
            {
                obs.OnError(new System.Exception("Asynchronously loaded resource is null. Name: " + resourceName));
            }
        }
    }

    public float GetProgress(string resourceName)
    {
        if (loadedResources.ContainsKey(resourceName))
        {            
            return 1.0f;
        }

        ResourceRequest request;
        inProgressOperations.TryGetValue(resourceName, out request);
        if(request != null)
        {
            return request.progress;
        }

        return 0.0f;
    }

    public void Unload(string resourceName)
    {
        //Debug.LogFormat("--> {0} resource(s) in memory before unloading \"{1}\"", loadedResources.Count, resourceName);
        int refCount = 0;

        LoadedResource res;
        loadedResources.TryGetValue(resourceName, out res);
        if (res != null && res.m_Resource != null)
        {
            refCount = --res.m_ReferencedCount;
            if (refCount == 0)
            {
                loadedResources.Remove(resourceName);
                Resources.UnloadUnusedAssets();

                //Debug.LogFormat("Resource {0} has been unloaded successfully.", resourceName);
            }
        }

        //Debug.LogFormat("<-- {0} resource(s) in memory after unloading \"{1}\" : [RefCount] {2}", loadedResources.Count, resourceName, refCount);

    }

    public void UnloadAll()
    {
        loadedResources.Clear();
        Resources.UnloadUnusedAssets();
    }
}
