// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using Timberborn.Persistence;

namespace Automation.Utils {

/// <summary>Simple generic serializer for a class that implements <see cref="IGameSerializable"/>.</summary>
/// <remarks>
/// This serializer simple passes control to the serializable object. On load, an instance of type
/// <typeparamref name="T"/> is created and loaded, so in order to load descendant classes, a specialized serializer is
/// needed for each type. 
/// </remarks>
/// <typeparam name="T">the type of the class</typeparam>
/// <seealso cref="DynamicClassSerializer{T}"/>
public sealed class StaticClassSerializer<T> : IObjectSerializer<T> where T : IGameSerializable, new() {
  /// <inheritdoc/>
  public void Serialize(T value, IObjectSaver objectSaver) {
    value.SaveTo(objectSaver);
  }

  /// <inheritdoc/>
  public Obsoletable<T> Deserialize(IObjectLoader objectLoader) {
    var instance = new T();
    instance.LoadFrom(objectLoader);
    return instance;
  }
}

}
