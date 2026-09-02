using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskForge.Api.Data;
using TaskForge.Api.DTOs;
using TaskForge.Api.Models;

namespace TaskForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var existingUser = await _context.Users
                .AnyAsync(user => user.Email == request.Email);

            if (existingUser)
            {
                return BadRequest(new
                {
                    message = "Email is already registered."
                });
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User registered successfully.",
                user = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.CreatedAt
                }
            });
        }
    }
}