﻿// Copyright 2016-2018 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Browser.Http;
using Mono.WebAssembly.Interop;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using Serilog.Sinks.PeriodicBatching;

namespace OpenGraphTilemaker.Web.Client.ClientApp.Diagnostics
{
    internal class BrowserHttpSink : PeriodicBatchingSink
    {
        public const int DefaultBatchPostingLimit = 1000;
        public const int DefaultQueueSizeLimit = 100000;
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);
        private static readonly JsonValueFormatter JsonValueFormatter = new JsonValueFormatter();

        private static readonly TimeSpan RequiredLevelCheckInterval = TimeSpan.FromMinutes(2);
        private readonly ControlledLevelSwitch _controlledSwitch;

        private readonly string _endpointUrl;
        private readonly long? _eventBodyLimitBytes;
        private readonly HttpClient _httpClient;

        private DateTime _nextRequiredLevelCheckUtc = DateTime.UtcNow.Add(RequiredLevelCheckInterval);

        public BrowserHttpSink(
            string endpointUrl,
            int batchPostingLimit,
            TimeSpan period,
            long? eventBodyLimitBytes,
            LoggingLevelSwitch levelControlSwitch,
            int queueSizeLimit)
            : base(batchPostingLimit, period, queueSizeLimit) {
            _endpointUrl = endpointUrl ?? throw new ArgumentNullException(nameof(endpointUrl));
            _eventBodyLimitBytes = eventBodyLimitBytes;
            _controlledSwitch = new ControlledLevelSwitch(levelControlSwitch);
            _httpClient = new HttpClient(new BrowserHttpMessageHandler());
            _httpClient.BaseAddress = new Uri(GetBaseUri());
        }

        internal static string FormatCompactPayload(IEnumerable<LogEvent> events, long? eventBodyLimitBytes) {
            var payload = new StringWriter();

            foreach (var logEvent in events) {
                var buffer = new StringWriter();

                try {
                    CompactJsonFormatter.FormatEvent(logEvent, buffer, JsonValueFormatter);
                }
                catch (Exception ex) {
                    LogNonFormattableEvent(logEvent, ex);
                    continue;
                }

                var json = buffer.ToString();
                if (CheckEventBodySize(json, eventBodyLimitBytes)) payload.WriteLine(json);
            }

            return payload.ToString();
        }

        private static bool CheckEventBodySize(string json, long? eventBodyLimitBytes) {
            if (eventBodyLimitBytes.HasValue &&
                Encoding.UTF8.GetByteCount(json) > eventBodyLimitBytes.Value) {
                SelfLog.WriteLine(
                    "Event JSON representation exceeds the byte size limit of {0} set for this sink and will be dropped; data: {1}",
                    eventBodyLimitBytes, json);
                return false;
            }

            return true;
        }

        private static string GetBaseUri() => new MonoWebAssemblyJSRuntime().Invoke<string>("Blazor._internal.uriHelper.getBaseURI", Array.Empty<object>());

        private static void LogNonFormattableEvent(LogEvent logEvent, Exception ex) =>
            SelfLog.WriteLine(
                "Event at {0} with message template {1} could not be formatted into JSON for the server and will be dropped: {2}",
                logEvent.Timestamp.ToString("o"), logEvent.MessageTemplate.Text, ex);

        protected override bool CanInclude(LogEvent evt) => _controlledSwitch.IsIncluded(evt);

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if (disposing)
                _httpClient.Dispose();
        }

        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events) {
            _nextRequiredLevelCheckUtc = DateTime.UtcNow.Add(RequiredLevelCheckInterval);

            var payload = FormatCompactPayload(events, _eventBodyLimitBytes);

            var content = new StringContent(payload, Encoding.UTF8, SerilogServerApi.CompactLogEventFormatMimeType);

            var result = await _httpClient.PostAsync(_endpointUrl, content).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode)
                throw new LoggingFailedException(
                    $"Received failed result {result.StatusCode} when posting events to the server.");

            var returned = await result.Content.ReadAsStringAsync();
            _controlledSwitch.Update(SerilogServerApi.ReadEventInputResult(returned));
        }

        // The sink must emit at least one event on startup, and the server be
        // configured to set a specific level, before background level checks will be performed.
        protected override async Task OnEmptyBatchAsync() {
            if (_controlledSwitch.IsActive &&
                _nextRequiredLevelCheckUtc < DateTime.UtcNow)
                await EmitBatchAsync(Enumerable.Empty<LogEvent>());
        }
    }
}
