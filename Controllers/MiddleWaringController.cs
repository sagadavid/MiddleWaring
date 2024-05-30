using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.RateLimiting;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class MiddleWareSampleController(ILogger<MiddleWareSampleController> logger) : ControllerBase
    {
        private readonly Random _random = new();

        [HttpGet("delay-request")]
        //[RequestTimeout(5000)]
        //[RequestTimeout("ShortTimeoutPolicy")]
        [RequestTimeout("LongTimeoutPolicy")]

        public async Task<ActionResult> RequestTimeoutDemo()
        {
            var delay = _random.Next(1, 10);
            logger.LogInformation($"a wishful delay for {delay} seconds");//our logging is already functional and logs by log2file extension, still we want to log special message here !

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(delay), Request.HttpContext.RequestAborted);
            }
            catch
            {
                logger.LogWarning("request timed out !");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "The request timed out after 5 seconds");
            }
            return Ok($"Task is deliberately delayed for {delay} seconds");
        }

        [HttpGet("limit-rate")]
        [EnableRateLimiting(policyName: "limited")]
        public ActionResult RateLimitingDemo()
        {
            return Ok($"allowed request amount is declared by policy : {DateTime.Now.Ticks.ToString()}");
        }
    }
}
