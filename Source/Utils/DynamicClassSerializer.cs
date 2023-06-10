// Timberborn Utils
// Author: igor.zavoychinskiy@gmail.com
// License: Public Domain

using System;
using System.Linq;
using Timberborn.Persistence;

namespace Automation.Utils {

/// <summary>Serializer that can handle the descendant classes.</summary>
/// <remarks>
/// <p>
/// This serializer stores the actual type info into the state and uses it during the load. Serializer for type
/// <typeparamref name="T"/> can load any type that is descendant of <typeparamref name="T"/>. In order to get the final
/// type, make the upcast from the loaded instance.
/// </p>
/// <p>
/// Even though the <typeparamref name="T"/> type can be abstract, the actual, type that was serialized, must have a
/// public default constructor in order to be loaded. Or else an exception will be thrown on the state loading.
/// </p>
/// </remarks>
/// <typeparam name="T">the type of the base class. It can be abstract.</typeparam>
/// <seealso cref="DynamicClassSerializer{T}"/>
public sealed class DynamicClassSerializer<T> : IObjectSerializer<T> where T : IGameSerializable {
  /// <summary>Property name that identifies the actual tape in the saved state.</summary>
  public static readonly PropertyKey<string> TypeIdPropertyKey = new("TypeId");

  /// <inheritdoc/>
  public void Serialize(T value, IObjectSaver objectSaver) {
    objectSaver.Set(TypeIdPropertyKey, typeof(T).FullName);
    value.SaveTo(objectSaver);
  }

  /// <inheritdoc/>
  public Obsoletable<T> Deserialize(IObjectLoader objectLoader) {
    var savedTypeId = objectLoader.Get(TypeIdPropertyKey);
    var objectType = AppDomain.CurrentDomain.GetAssemblies()
        .Select(assembly => assembly.GetType(savedTypeId))
        .FirstOrDefault(t => t != null);
    if (objectType == null) {
      throw new InvalidOperationException($"Cannot find type for typeId: {savedTypeId}");
    }
    if (objectType.GetConstructor(Type.EmptyTypes) == null) {
      throw new InvalidOperationException($"No default constructor in : {objectType}");
    }
    if (!typeof(T).IsAssignableFrom(objectType)) {
      throw new InvalidOperationException($"Incompatible types: saved={objectType}, serializer={typeof(T)}");
    }
    var instance = (T) Activator.CreateInstance(objectType);
    instance.LoadFrom(objectLoader);
    return instance;
  }
}

}
