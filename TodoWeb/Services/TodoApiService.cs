#region Copyright
//=======================================================================================
// Microsoft 
//
// This sample is supplemental to the technical guidance published on my personal
// blog at https://github.com/paolosalvatori. 
// 
// Author: Paolo Salvatori
//=======================================================================================
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// LICENSED UNDER THE APACHE LICENSE, VERSION 2.0 (THE "LICENSE"); YOU MAY NOT USE THESE 
// FILES EXCEPT IN COMPLIANCE WITH THE LICENSE. YOU MAY OBTAIN A COPY OF THE LICENSE AT 
// http://www.apache.org/licenses/LICENSE-2.0
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING, SOFTWARE DISTRIBUTED UNDER THE 
// LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY 
// KIND, EITHER EXPRESS OR IMPLIED. SEE THE LICENSE FOR THE SPECIFIC LANGUAGE GOVERNING 
// PERMISSIONS AND LIMITATIONS UNDER THE LICENSE.
//=======================================================================================
#endregion

#region Using Directives
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TodoWeb.Models;
using System.Net.Http;
using System;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Options;
#endregion

namespace TodoWeb.Services
{
    /// <summary>
    /// TodoApiService class
    /// </summary>
    public class TodoApiService : ITodoApiService
    {
        #region Private Constants
        private const string DefaultBaseAddress = "todoapi";
        private const string GetAllTodoItemsUrl = "/api/todo";
        private const string GetTodoItemByIdUrl = "/api/todo/{0}";
        private const string CreateTodoItemUrl = "/api/todo";
        private const string UpdateTodoItemUrl = "/api/todo/{0}";
        private const string DeleteTodoItemUrl = "/api/todo/{0}";
        #endregion

        #region Private Instance Fields
        private readonly IOptions<TodoApiServiceOptions> _options;
        private readonly ILogger<TodoApiService> _logger;
        private readonly HttpClient _httpClient;
        #endregion

        #region Public Constructor
        /// <summary>
        /// Initializes a new instance of the TodoApiService class. 
        /// </summary>
        /// <param name="context">TodoContext parameter used as an in-memroy database.</param>
        /// <param name="logger">Logger.</param>
        public TodoApiService(IOptions<TodoApiServiceOptions> options, ILogger<TodoApiService> logger)
        {
            _options = options;
            _logger = logger;

            var endpoint = string.IsNullOrWhiteSpace(_options?.Value?.EndpointUri) ?
                           DefaultBaseAddress :
                           _options.Value.EndpointUri;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri($"http://{endpoint}")
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _logger.LogInformation(LoggingEvents.Configuration, $"HttpClient.BaseAddress = {_httpClient.BaseAddress}");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets all the todo items.
        /// </summary>
        /// <returns>All the todo items.</returns>
        public async Task<IEnumerable<TodoItem>> GetTodoItemsAsync()
        {
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                _logger.LogInformation(LoggingEvents.ListItems, "Retrieving all items...");
                var response = await _httpClient.GetAsync(GetAllTodoItemsUrl);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var todoItems = JsonConvert.DeserializeObject<List<TodoItem>>(json);
                _logger.LogInformation(LoggingEvents.ListItems, $"{todoItems.Count} items successfully retrieved.");
                return todoItems;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation(LoggingEvents.MethodCallDuration, $"GetAll method completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }

        /// <summary>
        /// Gets a specific todo item by id.
        /// </summary>
        /// <param name="id">The id of the todo item.</param>
        /// <returns>The todo item with the specified id.</returns> 
        public async Task<TodoItem> GetTodoItemAsync(string id)
        {
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                if (string.IsNullOrWhiteSpace(id))
                {
                    var message = "The id cannot be null or empty.";
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, message);
                    throw new ArgumentNullException(nameof(id), message);
                }
                _logger.LogInformation(LoggingEvents.GetItem, "Getting item {ID}...", id);
                var response = await _httpClient.GetAsync(string.Format(GetTodoItemByIdUrl, id));
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var todoItem = JsonConvert.DeserializeObject<TodoItem>(json);
                _logger.LogInformation(LoggingEvents.GetItem, "Item {ID} has been successfully retrieved.", todoItem.Id);
                return todoItem;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation($"GetById method completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }


        /// <summary>
        /// Creates a new todo item.
        /// </summary>
        /// <param name="item">The todo item to create.</param>
        /// <returns>If the operation succeeds, it returns the newly created item.</returns>   
        public async Task<TodoItem> CreateTodoItemAsync(TodoItem item)
        {
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                if (item == null)
                {
                    var message = "The item cannot be null.";
                    _logger.LogWarning(LoggingEvents.InsertItem, message);
                    throw new ArgumentNullException(nameof(item), message);
                }
                var json = JsonConvert.SerializeObject(item);
                var postContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(CreateTodoItemUrl, postContent);
                response.EnsureSuccessStatusCode();
                json = await response.Content.ReadAsStringAsync();
                var todoItem = JsonConvert.DeserializeObject<TodoItem>(json);
                _logger.LogInformation(LoggingEvents.InsertItem, "Item {ID} has been successfully created.", item.Id);
                return todoItem;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation($"Create method completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }

        /// <summary>
        /// Updates a given todo item. 
        /// </summary>
        /// <param name="id">The id of the todo item.</param>
        /// <param name="item">The todo item to update.</param>
        /// <returns>True if the item was successfully updated, false otherwise.</returns>
        public async Task<bool> UpdateTodoItemAsync(TodoItem item)
        {
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                if (item == null)
                {
                    var message = "The item cannot be null.";
                    _logger.LogWarning(LoggingEvents.UpdateItem, message);
                    throw new ArgumentNullException(nameof(item), message);
                }
                var json = JsonConvert.SerializeObject(item);
                var putContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(string.Format(UpdateTodoItemUrl, item.Id), putContent);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(LoggingEvents.UpdateItem, "Item {ID} has been successfully updated.", item.Id);
                    return true;
                }
                return false;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation($"Update method completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }

        /// <summary>
        /// Deletes a specific todo item.
        /// </summary>
        /// <param name="id">The id of the todo item.</param>      
        /// <returns>True if the item was successfully deleted, false otherwise.</returns>
        public async Task<bool> DeleteTodoItemAsync(string id)
        {
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                if (string.IsNullOrWhiteSpace(id))
                {
                    var message = "The id cannot be null or empty.";
                    _logger.LogWarning(LoggingEvents.DeleteItem, message);
                    throw new ArgumentNullException(nameof(id), message);
                }
                var response = await _httpClient.DeleteAsync(string.Format(DeleteTodoItemUrl, id));
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(LoggingEvents.DeleteItem, "Item {ID} has been successfully deleted.", id);
                    return true;
                }
                return false;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation($"Delete method completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }
        #endregion
    }
}