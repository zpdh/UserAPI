using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserAPI.Data;
using UserAPI.Models;
using UserAPI.Services;

namespace UserAPI.Controllers
{
    [ApiController]
    [Route("v1")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        [Route("users")]
        [Authorize]
        public async Task<IActionResult> GetAsync([FromServices] DataContext context)
        {
            var userList = 
            await
            context
            .Users
            .AsNoTracking()
            .ToListAsync();

            return Ok(userList);
        }

        [HttpGet]
        [Route("users/{id}")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync([FromServices] DataContext context, [FromRoute] int id)
        {
            var user = 
            await
            context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost("users")]
        [Authorize]
        public async Task<IActionResult> PostAsync([FromServices] DataContext context, [FromBody] Users user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await
                context
                .Users
                .AddAsync(user);

                await 
                context
                .SaveChangesAsync();

                return Created($"v1/users/{user.Id}", user);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("users/{id}")]
        [Authorize]
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
                target.Password = user.Password;

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
        [Authorize]
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

        //To generate key, go to v1/auth and enter a valid user in database, then use the bearer token to authenticate.
        [HttpPost]
        [Route("auth")]
        public async Task<ActionResult<dynamic>> AuthenticateAsync([FromServices] DataContext context, [FromBody] Users model)
        {
            var user = 
            await 
            context
            .Users
            .FirstOrDefaultAsync(x => x.Name == model.Name);

            if (user == null || user.Password != model.Password) return NotFound();

            var token = TokenService.GenerateToken(user);

            return new
            {
                user = user.Name,
                token = token
            };
        }
    }
}
