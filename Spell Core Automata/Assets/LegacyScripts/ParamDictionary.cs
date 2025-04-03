using System.Collections.Generic;

public class ParamDictionary
{
    private Dictionary<string, object> parameters = new Dictionary<string, object>();

    // 添加参数
    public void Add(string key, object value)
    {
        parameters[key] = value;
    }

    // 获取参数
    public T Get<T>(string key, T defaultValue = default)
    {
        if (parameters.ContainsKey(key) && parameters[key] is T)
        {
            return (T)parameters[key];
        }
        return defaultValue;
    }

    // 检查是否包含某个键
    public bool ContainsKey(string key)
    {
        return parameters.ContainsKey(key);
    }

    // 转换为字典（方便调试或序列化）
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>(parameters);
    }

    public int Count{ get { return parameters.Count; } }
}
