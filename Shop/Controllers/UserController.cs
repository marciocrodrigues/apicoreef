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
    [Route("v1/users")]
    public class UserController : Controller
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            return Ok(users);
        }


        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post([FromBody]User model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                model.Role = "employee";

                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                model.Password = "";
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível cria usuário" });
            }
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
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

                // Esconde a senha
                user.Password = "";
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

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(int id, [FromBody]User model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var user = await _context.Users.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();

            if(user == null)
                return NotFound(new { message = "Usuário não encontrado" });
            
            try
            {
                _context.Entry<User>(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch(DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Registro já foi atualizado" });
            }
            catch
            {
                return BadRequest(new { message = "Erro ao tentar alterar usuário" });
            }
        }
    }
}