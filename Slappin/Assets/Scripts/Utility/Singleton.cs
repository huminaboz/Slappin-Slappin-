using UnityEngine;

public class InitSingleton<T> : Singleton<T> where T : MonoBehaviour {
  public virtual void Init() { }
}

/// <summary>
/// This implementation expects singletons to be already present on a gameObject
///   in the scene. It won't create one if one is not found.
///
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
  private static T _instance;

  private static object _lock = new object();

  public static T I {
    get {
      lock (_lock) {
        if (_instance == null) {
          _instance = (T)FindObjectOfType(typeof(T), true);

          if (FindObjectsOfType(typeof(T)).Length > 1) {
            Debug.LogError("[Singleton] Something went really wrong " +
                " - there should never be more than 1 singleton!");
            return _instance;
          }

          if (_instance == null) {
            // we actually expect and check for null in OnDisable
            //  if this error actually happens we'll just get the null reference error
            // Debug.LogError("[Singleton] Instance '" + typeof(T) +
            //     " could not be found.");
          } else if (_instance is InitSingleton<T> initSingleton) {
            initSingleton.Init();
          }
        }

        return _instance;
      }
    }
  }

  protected virtual void OnDestroy() {
    _instance = null;
  }
}