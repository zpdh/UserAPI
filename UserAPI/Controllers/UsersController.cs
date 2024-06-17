using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAPI.Data;
using UserAPI.Helpers;
using UserAPI.Models;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("v1")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetAsync([FromServices] DataContext context)
        {
            var userList = await
                context
                .Users
                .AsNoTracking()
                .ToListAsync();

            return Ok(userList);
        }

        [HttpGet]
        [Route("users/{id}")]
        public async Task<IActionResult> GetByIdAsync([FromServices] DataContext context, [FromRoute] int id)
        {
            var user = await
                context
                .Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost("users")]
        public async Task<IActionResult> PostAsync([FromServices] DataContext context, [FromBody] Users user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                user.Password = user.Password.GenerateHash();
                await
                context
                .Users
                .AddAsync(user);

                context
                .SaveChangesAsync();

            }
            catch
            {
                return BadRequest();
            }
            await
                context
                .SaveChangesAsync();

            return Created($"v1/users/{user.Id}", user);
        }

        [HttpPut]
        [Route("users/{id}")]
        public async Task<IActionResult> PutAsync([FromServices] DataContext context, [FromBody] Users user, [FromRoute] int id)
        {
            if (!ModelState.IsValid) return BadRequest();

            var target =
                await
                context
                .Users
                .FirstOrDefaultAsync(x => x.Id == id);

            if (target == null) return NotFound();

            try
            {
                target.Name = user.Name;
                target.Email = user.Email;

                if (user.Password.Length <= 16) target.Password = user.Password.GenerateHash();
                else target.Password = user.Password;

                context.Update(target);

                await
                context
                .SaveChangesAsync();

                return Ok(target);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("users/{id}")]
        public async Task<IActionResult> DeleteAsync([FromServices] DataContext context, [FromRoute] int id)
        {
            var user =
            await
            context
            .Users
            .FirstOrDefaultAsync(x => x.Id == id);

            if (user == null) return NotFound();

            try
            {
                context.Remove(user);

                await
                context
                .SaveChangesAsync();

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
