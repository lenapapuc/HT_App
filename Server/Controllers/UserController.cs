using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Net;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager;
        }

       

        [HttpGet]
        [Route("{id}")]
        public async Task<UserDto> GetUserById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

           
            var roles = await _userManager.GetRolesAsync(user);


            var userDto = new UserDto
            {
                Name = user.Name,
                Strikes = user.Strikes,
                Role = roles[0],
                UserId = id.ToString()
            };

            return userDto;
        }

        [HttpPut]
        [Route("strike/{id}")]
        public async Task<IActionResult> StrikeUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            user.Strikes += 1; // Increment the strikes
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
            var message = await _context.Messages.Where(m=>m.CreatedBy.Id == id).ToListAsync();

            if (message == null) throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound); 
            foreach(var item in message) { _context.Messages.Remove(item); }
            
           

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return NoContent(); // HTTP 204 No Content
            }

            return BadRequest(result.Errors);
        }

    }
}
