using System;
using System.Collections.Generic;
using System.Linq;

namespace DommelExtended.Json;

public class JsonPropertyResolver : DefaultPropertyResolver
{
    private readonly HashSet<Type> _jsonPrimitiveTypes;

    public JsonPropertyResolver(IReadOnlyCollection<Type> jsonTypes)
    {
        // Append the given types to the base set of types Dommel considers
        // primitive so they will be used in insert and update queries.
        _jsonPrimitiveTypes = new HashSet<Type>(base.PrimitiveTypes.Concat(jsonTypes));
        JsonTypes = jsonTypes;
    }

    // Internal for testing
    public IReadOnlyCollection<Type> JsonTypes { get; }

    protected override HashSet<Type> PrimitiveTypes => _jsonPrimitiveTypes;
}
