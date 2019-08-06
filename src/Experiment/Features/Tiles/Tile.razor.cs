﻿#nullable enable
using Microsoft.AspNetCore.Components;
using OpenGraphTilemaker.OpenGraph;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Experiment.Features.Tiles
{
    public class TileModel : ComponentBase
    {
        [Parameter] protected OpenGraphMetadata? OpenGraphMetadata { get; set; }
    }
}