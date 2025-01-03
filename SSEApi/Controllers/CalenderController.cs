using Microsoft.AspNetCore.Mvc;
using SSEApi.CalenderService;

namespace SSEApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalenderController : ControllerBase
    {
        private readonly CalenderService.CalenderAdapter _calenderAdapter;
        private readonly CalenderService.CalenderService _calenderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CalenderController(CalenderService.CalenderAdapter calenderAdapter,CalenderService.CalenderService calenderService, IHttpContextAccessor httpContext)
        {
            _calenderService = calenderService;
            _httpContextAccessor = httpContext;
            _calenderAdapter = calenderAdapter;
        }
        [HttpGet]
        [Route("events")]
        public Task Get(CancellationToken cancellation, string name)
        {

            return _calenderAdapter.ConnectCalander(_httpContextAccessor, cancellation, name);
        }
        [HttpPost]
        [Route("add-event")]
        public async Task Add([FromBody] Event @event,CancellationToken cancellation) { 
            await _calenderService.AddEvent(_calenderAdapter,@event); 
        }
    }
}
