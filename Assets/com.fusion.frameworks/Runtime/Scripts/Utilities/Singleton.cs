using System;

namespace Fusion.Frameworks.Utilities
{
    public class Singleton<T>
    {
        private static T instance;
        private static readonly object lockObject = new object();
        public static T Instance {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = (T)Activator.CreateInstance(typeof(T), true);
                        }
                    }
                }
                return instance;
            }
        }

    }

}

