﻿using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insight.Tinkoff.InvestSdk.Infrastructure.Exceptions;
using Insight.Tinkoff.InvestSdk.Infrastructure.Json;

namespace Insight.Tinkoff.InvestSdk.Infrastructure.Services
{
    internal abstract class RestService
    {
        protected HttpClient Client;

        protected string BaseUrl { get; }

        private bool _clientInitialized;

        protected RestService(string baseUrl, HttpClient client = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            BaseUrl = baseUrl;
            Client = client;
        }

        internal virtual async Task<TO> Post<TI, TO>(string path, TI payload,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, path);
            if (payload != null)
                request.Content = new StringContent(JSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var client = EnsureHttpClientCreated();
            var response = await client.SendAsync(request, cancellationToken);
            return await GetResponseItem<TO>(path, response);
        }

        internal virtual async Task<T> Get<T>(string path, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            var client = EnsureHttpClientCreated();
            var response = await client.SendAsync(request, cancellationToken);
            return await GetResponseItem<T>(path, response);
        }

        internal virtual async Task<T> Delete<T>(string path, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, path);
            var client = EnsureHttpClientCreated();
            var response = await client
                .SendAsync(request, cancellationToken);
            return await GetResponseItem<T>(path, response);
        }

        internal virtual async Task<TO> Put<TI, TO>(string path, TI payload,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, path);
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

        protected async Task<string> GetResponseString(HttpResponseMessage response)
            => Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());

        protected string GetRestServiceExceptionMessage(string path, HttpStatusCode code)
            => $"Ошибка в результате запроса к апи. Url: {path} Код: {(int) code}";

        private HttpClient EnsureHttpClientCreated()
        {
	        if (!_clientInitialized)
	        {
		        if (Client == null)
			        Client = new HttpClient();

		        Client.BaseAddress = new Uri(BaseUrl);
		        SetHeaders();
		        _clientInitialized = true;
	        }

	        return Client;
        }

        protected abstract void SetHeaders();
    }
}