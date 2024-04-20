using System;
using JetBrains.Annotations;
using LiveMap.Common.Api.Layer.Options.Marker;
using Newtonsoft.Json;

namespace LiveMap.Common.Api.Layer.Marker;

/// <summary>
/// The circle marker is used to draw circle overlays on the map
/// </summary>
/// <remarks>
/// You can make distorted circles with the <see cref="Ellipse"/>
/// </remarks>
[PublicAPI]
public class Circle : Marker<Circle> {
    /// <summary>
    /// Absolute (not relative to spawn) world coordinates for the circle's center
    /// </summary>
    [JsonProperty(Order = 10)]
    public Point Point { get; set; }

    /// <inheritdoc cref="CircleOptions"/>
    [JsonProperty(Order = 11)]
    public new CircleOptions? Options { get; set; }

    /// <summary>
    /// Create a new circle at 0,0 with a random id
    /// </summary>
    public Circle() : this(Guid.NewGuid().ToString(), new Point(0, 0)) { }

    /// <summary>
    /// Create a new circle
    /// </summary>
    /// <param name="id">Unique identifying key</param>
    /// <param name="point">Absolute (not relative to spawn) world coordinates for the circle's center</param>
    /// <param name="options">Optional settings for the circle</param>
    public Circle(string id, Point point, CircleOptions? options = null) : base("circle", id) {
        Point = point;
        Options = options;
    }
}
