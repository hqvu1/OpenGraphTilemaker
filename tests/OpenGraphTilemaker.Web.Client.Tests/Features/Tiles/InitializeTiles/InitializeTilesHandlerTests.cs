﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaseTestCode;
using FluentAssertions;
using OpenGraphTilemaker.Tests;
using OpenGraphTilemaker.Web.Client.Features.Tiles;
using Xunit;
using Xunit.Abstractions;

namespace OpenGraphTilemaker.Web.Client.Tests.Features.Tiles
{
    public class InitializeTilesHandlerTests : ClientBaseTest
    {
        public InitializeTilesHandlerTests(ITestOutputHelper testConsole) : base(testConsole) { }

        //[Fact]
        //public async Task InitializeTilesRequest() {
        //    // Arrange
        //    var request = new InitializeTilesRequest();
        //    var response = "<head><meta property=\"og:title\" content=\"Microsoft launches Spend iOS app that automatically tracks and matches expenses\" />";
        //    response += "<meta property=\"og:image\" content=\"image\" />";
        //    response += "<meta property=\"og:description\" content=\"description\" /></head>";
        //    var handler = new TilesState.InitializeTilesHandler(Pocket(), TileMakerClient(response), GetPocketIOptions(), MockStore(new TilesState()));

        //    // Act
        //    var result = await handler.Handle(request, CancellationToken.None);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.OriginalTiles.Should().NotBeNullOrEmpty();
        //    var first = result.OriginalTiles.First();
        //    first.Title.Should().NotBeNullOrWhiteSpace();
        //    first.Title.Should().Be("Microsoft launches Spend iOS app that automatically tracks and matches expenses");

        //    TestConsole.WriteLine(first.Title);
        //}

        //[Fact]
        //[XunitCategory("Integration")]
        //public async Task InitializeTilesRequest_WithRealHttpClient() {
        //    // Arrange
        //    var request = new InitializeTilesRequest();
        //    var handler = new TilesState.InitializeTilesHandler(Pocket(), RealTileMakerClient(), GetPocketIOptions(), MockStore(new TilesState()));

        //    // Act
        //    var result = await handler.Handle(request, CancellationToken.None);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.OriginalTiles.Should().NotBeNullOrEmpty();

        //    foreach (var tile in result.OriginalTiles) {
        //        TestConsole.WriteLine(tile?.Title ?? "---");
        //    }
        //}
    }
}
