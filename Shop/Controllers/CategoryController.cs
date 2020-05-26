using Microsoft.AspNetCore.Mvc;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet("list")]
        public string Get()
        {
            return "Olá Mundo";
        }
    }
}