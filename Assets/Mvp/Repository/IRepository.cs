using UnityEngine;

namespace Fps.MVP.Repository
{
    public interface IRepository
    {
        TSource Load<TSource>(string key) where TSource : Object;
        TComponent LoadComponent<TComponent>(string key) where TComponent : Component;
        GameObject LoadPrefab(string key);
    }
}