using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        } 

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Product>>> Get()
        {
            var products = await _context.Products.Include(x => x.Category).AsNoTracking().OrderBy(x => x.Title).ToListAsync();
            return Ok(products);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var product = await _context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Ok(product);
        }
        
        [HttpGet]
        [Route("categories/{id:int}")]
        public async Task<ActionResult<List<Product>>> GetByCategory(int id)
        {
            var products = await _context.Products.Include(x => x.Category).AsNoTracking().Where(x => x.CategoryId == id).OrderBy(x => x.Title).ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Product>> Post([FromBody]Product model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var category = await  _context.Categories.FirstOrDefaultAsync(x => x.Id == model.CategoryId);

            if(category == null)
                return BadRequest(new { message = "Categoria não encontrada para o produto" });

            try
            {
                _context.Products.Add(model);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar produto" });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> Put(int id, [FromBody]Product model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if(product == null)
                return NotFound(new { message = "Produto não encontrado" });
            
            if(product.CategoryId !=  model.CategoryId)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);

                if(category == null)
                    return BadRequest(new { message = "Categoria não encontrada para o produto" });
            }

            try
            {
                _context.Entry<Product>(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch(DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro já foi atualizado" });
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível altetar o produto" });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);

            if(product == null)
                return NotFound(new { message = "Produto não encontrado" });

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Produto removido com sucesso" });
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível remover o produto" });
            }
        }
        
    }
}