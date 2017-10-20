#region Using Directives
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System;
#endregion

namespace TodoApi.Controllers
{
    /// <summary>
    /// Todo Controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        #region Private Instance Fields
        //private readonly TodoContext _context;
        private readonly ILogger<TodoController> _logger;
        private readonly INotificationService _notificationService;
        private readonly IRepositoryService<TodoItem> _repositoryService;
        #endregion

        #region Public Constructor
        /// <summary>
        /// Initializes a new instance of the TodoController class. 
        /// </summary>
        /// <param name="context">TodoContext parameter used as an in-memroy database.</param>
        /// <param name="repositoryService">Repository service used to read and write data.</param>
        /// <param name="notificationService">NotificationService used to send notifications.</param>
        /// <param name="logger">Logger.</param>
        public TodoController(TodoContext context, 
                              IRepositoryService<TodoItem> repositoryService,
                              INotificationService notificationService,
                              ILogger<TodoController> logger)
        {
            
            _logger = logger;
            _repositoryService = repositoryService;
            _notificationService = notificationService;

            //_context = context;
            //if (!_context.TodoItems.Any())
            //{
            //    _context.TodoItems.Add(new TodoItem {
            //                                            Id = Guid.NewGuid().ToString(),
            //                                            Name = "Buy movie tickets",
            //                                            IsComplete = false });
            //    _context.TodoItems.Add(new TodoItem {
            //                                            Id = Guid.NewGuid().ToString(),
            //                                            Name = "Buy food",
            //                                            IsComplete = false });
            //    _context.TodoItems.Add(new TodoItem {
            //                                            Id = Guid.NewGuid().ToString(),
            //                                            Name = "Call my brother",
            //                                            IsComplete = false });
            //    _context.SaveChanges();
            //}
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets all the todo items.
        /// </summary>
        /// <returns>All the todo items.</returns>
        /// <response code="200">Get all the items, if any.</response>
        [HttpGet]
        [ProducesResponseType(typeof(TodoItem), 200)]
        public async Task<IActionResult> GetAll()
        {
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                _logger.LogInformation(LoggingEvents.ListItems, "Listing all items");
                var items = await _repositoryService.GetAllAsync();
                return new ObjectResult(items);
                //return _context.TodoItems.ToList();
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
        /// <response code="200">The item specified by the id.</response>
        /// <response code="404">If the item is not found.</response>     
        [HttpGet("{id}", Name = "GetTodo")]
        [ProducesResponseType(typeof(TodoItem), 200)]
        [ProducesResponseType(typeof(TodoItem), 404)]
        public async Task<IActionResult> GetById(string id)
        {
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();

                _logger.LogInformation(LoggingEvents.GetItem, "Getting item {ID}...", id);
                //var item = _context.TodoItems.FirstOrDefault(t => t.Id == id);
                var item = await _repositoryService.GetByIdAsync(id);
                if (item == null)
                {
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, "No item with id equal to {ID} was found.", id);
                    return NotFound();
                }

                _logger.LogInformation(LoggingEvents.GetItem, "Item {ID} has been successfully retrieved.", item.Id);

                await SendNotificationAsync("Read", item);

                return new ObjectResult(item);
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
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": 2,
        ///        "name": "walk the dog",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <param name="item">The todo item to create.</param>
        /// <returns>If the operation succeeds, it returns the newly created item.</returns>
        /// <response code="201">Returns the newly created item.</response>
        /// <response code="400">If the item is null.</response>     
        [HttpPost]
        [ProducesResponseType(typeof(TodoItem), 201)]
        [ProducesResponseType(typeof(TodoItem), 400)]
        public async Task<IActionResult> Create([FromBody]TodoItem item)
        {
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();

                if (item == null)
                {
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, "The item cannot be null.");
                    return BadRequest();
                }
                //if (_context.TodoItems.Any(t => t.Id == item.Id))
                //{
                //    _logger.LogWarning(LoggingEvents.GetItemNotFound, "An item with id equal to {ID} already exists.", item.Id);
                //    return BadRequest();
                //}
                //_context.TodoItems.Add(item);
                //_context.SaveChanges();

                await _repositoryService.CreateAsync(item);

                _logger.LogInformation(LoggingEvents.InsertItem, "Item {ID} has been successfully created.", item.Id);

                await SendNotificationAsync("Create", item);

                return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
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
        /// <returns>No content.</returns>
        /// <response code="204">No content if the item is successfully updated.</response>
        /// <response code="404">If the item is not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoItem), 204)]
        [ProducesResponseType(typeof(TodoItem), 404)]
        public async Task<IActionResult> Update(string id, [FromBody] TodoItem item)
        {
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();

                if (item == null || item.Id != id)
                {
                    _logger.LogWarning(LoggingEvents.GetItemNotFound, "The item is null or its id is different from the id in the payload.");
                    return BadRequest();
                }

                //var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
                //if (todo == null)
                //{
                //    _logger.LogWarning(LoggingEvents.GetItemNotFound, "No item with id equal to {ID} was found.", id);
                //    return NotFound();
                //}

                //todo.IsComplete = item.IsComplete;
                //todo.Name = item.Name;

                //_context.TodoItems.Update(todo);
                //_context.SaveChanges();

                await _repositoryService.UpdateAsync(item);

                _logger.LogInformation(LoggingEvents.UpdateItem, "Item {ID} has been successfully updated.", item.Id);

                await SendNotificationAsync("Update", item);

                return new NoContentResult();
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
        /// <returns>No content.</returns>
        /// <response code="202">No content if the item is successfully deleted.</response>
        /// <response code="404">If the item is not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(TodoItem), 204)]
        [ProducesResponseType(typeof(TodoItem), 404)]
        public async Task<IActionResult> Delete(string id)
        {
            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();

                //var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
                //if (todo == null)
                //{
                //    _logger.LogWarning(LoggingEvents.GetItemNotFound, "No item with id equal to {ID} was found.", id);
                //    return NotFound();
                //}

                //_context.TodoItems.Remove(todo);
                //_context.SaveChanges();

                await _repositoryService.DeleteByIdAsync(id);

                _logger.LogInformation(LoggingEvents.DeleteItem, "Item {ID} has been successfully deleted.", id);

                await SendNotificationAsync("Delete", new TodoItem { Id = id});

                return new NoContentResult();
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation($"Delete method completed in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }

        private async Task SendNotificationAsync(string operation, TodoItem todoItem)
        {
            if (_notificationService != null)
            {
                await _notificationService.SendNotificationAsync(new Notification
                {
                    Operation = operation,
                    Item = todoItem
                });
            }
        }
        #endregion
    }
}