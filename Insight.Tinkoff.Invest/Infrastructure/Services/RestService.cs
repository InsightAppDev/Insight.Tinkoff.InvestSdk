﻿using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Tinkoff.Invest.Infrastructure.Exceptions;
using Insight.Tinkoff.Invest.Infrastructure.Json;

namespace Insight.Tinkoff.Invest.Infrastructure.Services
{
    public abstract class RestService
    {
        private HttpClient _client;

        protected string BaseUrl { get; }

        protected RestService(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            BaseUrl = baseUrl;
        }

        protected async Task<TO> Post<TI, TO>(string path, TI payload, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, GetRequestUrl(path));
            if (payload != null)
                request.Content = new StringContent(JSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var client = EnsureHttpClientCreated();
            var response = await client.SendAsync(request, cancellationToken);
            return await GetResponseItem<TO>(path, response);
        }

        protected async Task<T> Get<T>(string path, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetRequestUrl(path));
            var response = await EnsureHttpClientCreated()
                .SendAsync(request, cancellationToken);
            return await GetResponseItem<T>(path, response);
        }

        protected async Task<T> Delete<T>(string path, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, GetRequestUrl(path));
            var response = await EnsureHttpClientCreated()
                .SendAsync(request, cancellationToken);
            return await GetResponseItem<T>(path, response);
        }

        protected async Task<TO> Put<TI, TO>(string path, TI payload, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, GetRequestUrl(path));
            if (payload != null)
                request.Content = new StringContent(JSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var client = EnsureHttpClientCreated();
            var response = await client
                .SendAsync(request, cancellationToken);
            return await GetResponseItem<TO>(path, response);
        }

        protected virtual async Task<T> GetResponseItem<T>(string path, HttpResponseMessage response)
        {
            var json = await GetResponseString(response);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new RestServiceException(GetRestServiceExceptionMessage(path, response.StatusCode));

            return JSerializer.Deserialize<T>(json);
        }

        protected Uri GetRequestUrl(string path)
        {
            return new UriBuilder($"{BaseUrl}{path}").Uri;
        }

        protected async Task<string> GetResponseString(HttpResponseMessage response)
            => Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());

        protected string GetRestServiceExceptionMessage(string path, HttpStatusCode code)
            => $"Ошибка в результате запроса к апи. Url: {path} Код: {(int) code}";

        private HttpClient EnsureHttpClientCreated()
        {
            if (_client == null)
                _client = CreateClient();

            return _client;
        }

        protected virtual HttpClient CreateClient()
        {
            return new HttpClient {BaseAddress = new Uri(BaseUrl)};
        }
    }
}