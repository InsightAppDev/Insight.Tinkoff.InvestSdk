﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Tinkoff.InvestSdk.Infrastructure.Exceptions;
using Insight.Tinkoff.InvestSdk.Infrastructure.Json;

namespace Insight.Tinkoff.InvestSdk.Infrastructure.Services
{
    internal abstract class RestService
    {
        private readonly string _baseUrl;

        protected HttpClient Client;

        protected RestService(string baseUrl, HttpClient client = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            _baseUrl = baseUrl;
            Client = client ?? new HttpClient();
        }

        internal virtual async Task<TO> Post<TI, TO>(string path,
            TI payload,
            AuthenticationHeaderValue authorization = null,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, BuildUri(path));
            if (payload != null)
                request.Content = new StringContent(JSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            if (authorization != null)
                request.Headers.Authorization = authorization;
            var response = await Client.SendAsync(request, cancellationToken);
            return await GetResponseItem<TO>(path, response);
        }

        internal virtual async Task<T> Get<T>(string path,
            AuthenticationHeaderValue authorization = null,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, BuildUri(path));
            if (authorization != null)
                request.Headers.Authorization = authorization;
            var response = await Client.SendAsync(request, cancellationToken);
            return await GetResponseItem<T>(path, response);
        }

        internal virtual async Task<T> Delete<T>(string path,
            AuthenticationHeaderValue authorization = null,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, BuildUri(path));
            if (authorization != null)
                request.Headers.Authorization = authorization;
            var response = await Client
                .SendAsync(request, cancellationToken);
            return await GetResponseItem<T>(path, response);
        }

        internal virtual async Task<TO> Put<TI, TO>(string path, TI payload,
            AuthenticationHeaderValue authorization = null,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, BuildUri(path));
            if (authorization != null)
                request.Headers.Authorization = authorization;
            if (payload != null)
                request.Content = new StringContent(JSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await Client
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

        protected async Task<string> GetResponseString(HttpResponseMessage response)
            => Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());

        protected string GetRestServiceExceptionMessage(string path, HttpStatusCode code)
            => $"Ошибка в результате запроса к апи. Url: {path} Код: {(int) code}";

        private string BuildUri(string path)
        {
            return new UriBuilder($"{_baseUrl.TrimEnd('/')}/{path.TrimStart('/')}").ToString();
        }
    }
}