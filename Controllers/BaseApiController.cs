using ChatApp.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers

{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {

    }
}