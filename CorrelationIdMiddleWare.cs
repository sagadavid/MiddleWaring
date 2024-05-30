using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiddleWaring
{
    public class CorrelationIdMiddleWare(RequestDelegate next, ILogger<CorrelationIdMiddleWare> logger)
    {
        private const string CorrelationIdHeaderName = "X-Correlation-ID";

        public async Task InvokeAsync(HttpContext context)
        {
            //generate or retreave correlation Id
            var corID = context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var existingCorID)
            ? existingCorID.ToString()
            : Guid.NewGuid().ToString();

            //log request path and corId
            logger.LogInformation("see path: {path} and see corId {corId}", context.Request.Path, corID);

            //envoj context, call next middleware in the pipeline
            await next(context);
        }

    }
}