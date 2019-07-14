﻿using System.Collections.Generic;
using MediatR;
using OpenGraphTilemaker.OpenGraph;

namespace Experiment.Features.Tiles
{
    public class SearchTilesRequest : IRequest<TilesState>
    {
        public List<OpenGraphMetadata> OriginalTiles { get; set; }

        public string SearchText { get; set; }
    }
}
