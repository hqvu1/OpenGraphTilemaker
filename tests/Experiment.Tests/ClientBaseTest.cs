﻿using System;
using System.Net;
using System.Net.Http;
using BaseTestCode;
using Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OpenGraphTilemaker.GetPocket;
using OpenGraphTilemaker.OpenGraph;
using Xunit.Abstractions;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace

namespace OpenGraphTilemaker.Tests
{
    public class ClientBaseTest<T> : BaseTest<T>, IDisposable
    {
        private const string CachingFolder = @"C:\WINDOWS\Temp\";
        
        private readonly HttpClient _realHttpClient;
        private readonly Uri _uri = new Uri("https://getpocket.com/users/Flynn0r/feed/all");

        private readonly TimeSpan _cachingTimeSpan = TimeSpan.FromSeconds(120);
        private readonly TimeSpan _timeoutTimeSpan = TimeSpan.FromSeconds(15);

        protected ClientBaseTest(ITestOutputHelper testConsole) : base(testConsole) => _realHttpClient = new HttpClient();

        public void Dispose() => _realHttpClient?.Dispose();

        protected TileMakerClient TileMakerClient(string fakeResponse) =>
            new TileMakerClient(HttpClient(fakeResponse, HttpStatusCode.OK), TileMaker(), HttpLoader());

        protected OpenGraphTileMaker TileMaker() => new OpenGraphTileMaker();

        protected HttpLoader HttpLoader() => new HttpLoader(DiscCache());

        protected DiscCache DiscCache() => new DiscCache(DiscCacheIOptions());

        protected IOptions<DiscCacheOptions> DiscCacheIOptions() =>
            Options.Create(new DiscCacheOptions {CacheFolder = CachingFolder, CacheState = CacheState.Disabled});

        protected IOptions<PocketOptions> GetPocketIOptions() => Options.Create(new PocketOptions(_uri, _cachingTimeSpan, _timeoutTimeSpan));

        protected Pocket Pocket() => new Pocket(MemoryCache(), FeedService());

        protected Feed<PocketEntry> FeedService() => new Feed<PocketEntry>();

        protected MemoryCache MemoryCache() => new MemoryCache(new MemoryCacheOptions());

        protected TileMakerClient RealTileMakerClient() => new TileMakerClient(_realHttpClient, TileMaker(), HttpLoader());
    }
}