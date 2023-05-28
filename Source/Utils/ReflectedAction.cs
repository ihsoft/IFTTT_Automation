using System.Reflection;
using UnityDev.LogUtils;

namespace IFTTT_Automation.Utils {

public class ReflectedAction<T, TArg0> {
  readonly MethodInfo _methodInfo;

  public ReflectedAction(string methodName) {
    _methodInfo = typeof(T).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    if (_methodInfo == null) {
      DebugEx.Error("Cannot obtain method {0} from {1}", methodName, typeof(T));
    }
  }

  /// <summary>Indicates if the target method was found and ready to use.</summary>
  public bool IsValid() {
    return _methodInfo != null;
  }

  /// <summary>Invokes the method or NOOP if the method is not found.</summary>
  public void Invoke(T instance, TArg0 arg0) {
    if (_methodInfo == null) {
      DebugEx.Warning("Skipping invocation: instance={0}, arg0={1}", instance, arg0);
      return;
    }
    _methodInfo.Invoke(instance, new object[] { arg0 });
  }
}

}
