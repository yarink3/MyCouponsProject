using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using Google.Cloud.Firestore;

namespace TodoApi.Controllers
{
    [Route("api/TodoItems")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;
        public FirestoreDb db = Database.Instance.db;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetTodoItems()
        {


            try
            {
                
                    //checked password same is db
                    Console.WriteLine("login");
                    DocumentReference docRef = db.Collection("CouponsLists").Document("yarink3");
                    DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                    if (snapshot.Exists)
                    {
                        Console.WriteLine("Document data for {0} document:", snapshot.Id);
                        Dictionary<string, object> city = snapshot.ToDictionary();
                        string x = (string)city["company"];
                    string company = (string)city["company"];
                           long sum = (long)city["sum"];

                    Timestamp expireDate = (Timestamp)city["expireDate"];
                    string serialNumber = (string)city["serialNumber"];
                        Coupon y = new Coupon
                        {
                            company = (string)city["company"],
                            sum = (long)city["sum"],
                            expireDate = (Timestamp) city["expireDate"],
                            serialNumber = (string)city["serialNumber"]


                        };
                        return Ok(y);

                    //return Enumerable.Range(1, 5).Select(index => new TodoItem
                    //{
                    //    Id=123+index,
                    //    IsComplete=true,
                    //    Name="yarin"


                    //})
                    //foreach (KeyValuePair<string, object> pair in city)
                    //    {
                    //        Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                    //    }
                }
                else
                    {
                        Console.WriteLine("Document {0} does not exist!", snapshot.Id);
                    }
               
                return Ok("ok");
            }
            catch
            {
                return Ok("nooo");
            }

            //return Enumerable.Range(1, 5).Select(index => new TodoItem
            //{
            //    Id=123+index,
            //    IsComplete=true,
            //    Name="yarin"


            //})
            //.ToArray();




            //  if (_context.TodoItems == null)
            //{
            //    return NotFound();
            //}
            //  return await _context.TodoItems.ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
          if (_context.TodoItems == null)
          {
              return NotFound();
          }
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
