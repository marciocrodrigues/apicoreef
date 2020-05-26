using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("users")]
    public class UserController : Controller
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<User>> Post([FromBody]User model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                _context.Users.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível cria usuário" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody]User model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _context.Users.AsNoTracking().Where(x => x.Username == model.Username && x.Password == model.Password).FirstOrDefaultAsync();

                if(user == null)
                    return NotFound(new { message = "Usuário ou senha inválidos" });
                
                var token = TokenService.GenerateToken(user);

                return Ok(new {
                    user = user,
                    token = token
                });
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível realizar o login" });
            }
        }
    }
}