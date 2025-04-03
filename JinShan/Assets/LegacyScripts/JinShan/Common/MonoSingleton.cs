using UnityEngine;

namespace JinShan
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        //T表示子类
        //public static T Instance { get; private set; }
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        instance = new GameObject("Singleton of " + typeof(T)).AddComponent<T>();
                    }
                    else
                    {
                        instance.Init();
                    }
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                instance.Init();
            }
        }

        public virtual void Init()
        { }

        //场景中唯一的对象 即可继承此父类
    }
}