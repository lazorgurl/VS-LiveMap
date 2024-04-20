using System;
using JetBrains.Annotations;
using LiveMap.Common.Api.Layer.Options.Marker;
using Newtonsoft.Json;

namespace LiveMap.Common.Api.Layer.Marker;

/// <summary>
/// The rectangle marker is used to draw rectangle overlays on the map
/// </summary>
/// <remarks>
/// This is mostly used to show specific areas/sections of the world such as protected claims, cities, nations, etc.<br/>
/// You can make more detailed shapes with the Polygon
/// </remarks>
[PublicAPI]
public class Rectangle : Marker<Rectangle> {
    /// <summary>
    /// Absolute (not relative to spawn) world coordinates for the rectangle's points
    /// </summary>
    [JsonProperty(Order = 10)]
    public Point[] Point { get; set; }

    /// <inheritdoc cref="RectangleOptions"/>
    [JsonProperty(Order = 11)]
    public new RectangleOptions? Options { get; set; }

    /// <summary>
    /// Create a new rectangle at 0,0 with a random id
    /// </summary>
    public Rectangle() : this(Guid.NewGuid().ToString(), new[] { new Point(0, 0), new Point(0, 0) }) { }

    /// <summary>
    /// Create a new rectangle
    /// </summary>
    /// <param name="id">Unique identifying key</param>
    /// <param name="point">Absolute (not relative to spawn) world coordinates for the rectangle's points</param>
    /// <param name="options">Optional settings for the rectangle</param>
    public Rectangle(string id, Point[] point, RectangleOptions? options = null) : base("rectangle", id) {
        Point = point;
        Options = options;
    }
}
