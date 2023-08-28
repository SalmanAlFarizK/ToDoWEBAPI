using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ToDoList.Models;
using ToDoList.Models.Domain;

namespace ToDoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly MVCDemoDbContext _context;

        public TodoController(MVCDemoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllTasks()
        {
            var todos = _context.Tasks.ToList();
            return Ok(todos);
        }

        [HttpPost]
        public IActionResult CreateTask(Tasks todo)
        {
            if (ModelState.IsValid)
            {
                if (Characters(todo.Task))
                {
                    return BadRequest("Task cannot contain special characters.");
                }
                else
                {
                    todo.Priority = todo.Category == "work" ? 0 : 1;
                    _context.Tasks.Add(todo);
                    _context.SaveChanges();
                    return CreatedAtAction(nameof(GetAllTasks), null);
                }
            }
            else
            {
                return BadRequest("Task cannot be null.");
            }
        }

        private bool Characters(string input)
        {
            // Define your list of special characters or pattern here
            string specialCharacters = "!@#$%^&*()";

            foreach (char character in input)
            {
                if (specialCharacters.Contains(character))
                {
                    return true;
                }
            }

            return false;
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, Tasks todo)
        {
            if (ModelState.IsValid)
            {
                var existingTask = _context.Tasks.Find(id);
                if (existingTask == null)
                {
                    return NotFound();
                }
                if (Characters(todo.Task))
                {
                    return BadRequest("Task cannot contain special characters.");
                }
                else
                {
                    existingTask.Task = todo.Task;
                    _context.SaveChanges();
                    return NoContent();
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPatch("{id}/toggle")]
        public IActionResult ToggleTaskStatus(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }

            task.Status = !task.Status;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("work")]
        public IActionResult GetWorkTasks()
        {
            var workTasks = _context.Tasks.Where(task => task.Priority == 0).ToList();
            return Ok(workTasks);
        }

        [HttpGet("personal")]
        public IActionResult GetPersonalTasks()
        {
            var personalTasks = _context.Tasks.Where(task => task.Priority == 1).ToList();
            return Ok(personalTasks);
        }

        [HttpDelete("clear")]
        public IActionResult ClearAllTasks()
        {
            var allTasks = _context.Tasks.ToList();

            foreach (var todo in allTasks)
            {
                _context.Tasks.Remove(todo);
            }

            _context.SaveChanges();

            return NoContent();
        }
    }
}
