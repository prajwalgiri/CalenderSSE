using Microsoft.AspNetCore.Mvc;
using SSEApi.CalenderService;

namespace SSEApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalenderController : ControllerBase
    {
        private readonly CalenderService.CalenderAdapter _calenderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CalenderController(CalenderService.CalenderAdapter calenderService, IHttpContextAccessor httpContext)
        {
            _calenderService = calenderService;
            _httpContextAccessor = httpContext;
        }
        [HttpGet]
        [Route("events")]
        public Task Get(CancellationToken cancellation, string name)
        {

            return _calenderService.ConnectCalander(_httpContextAccessor, cancellation, name);
        }
        [HttpPost]
        [Route("add-event")]
        public async Task Add([FromBody] Event @event,CancellationToken cancellation) { 
            await _calenderService.SendEvent(@event); 
        }
    }
}
