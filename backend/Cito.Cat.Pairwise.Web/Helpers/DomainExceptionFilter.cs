using System;
using System.Collections.Generic;
using System.Linq;
using Cito.Cat.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Cito.Cat.Pairwise.Web.Helpers
{
    public class DomainExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public DomainExceptionFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DomainExceptionFilter>();
        }

        public override void OnException(ExceptionContext context)
        {
            var domainExceptions = new List<DomainException>();

            if (context.Exception is DomainException)
            {
                domainExceptions.Add((DomainException) context.Exception);
            }

            if (context.Exception is AggregateException exception)
            {
                domainExceptions.AddRange(exception.InnerExceptions
                    .OfType<DomainException>().ToList());
            }

            if (domainExceptions.Any())
            {
                var messages = domainExceptions.Select(e => new {e.Message});
                var badRequest = domainExceptions.Any(e => e.IsBadRequest);
                foreach (var domainException in domainExceptions)
                {
                    _logger.LogWarning(0,
                        $"A domain exception was thrown: {domainException.Message}. The request that caused it was {(domainException.IsBadRequest ? "malformed" : "not malformed")}",
                        domainException);
                }

                //context.ModelState.AddModelError("All", string.Join(", ", messages));
                // if (badRequest)
                // {
                //     context.Result = new BadRequestObjectResult(messages);
                // }
                // else
                // {
                //     context.Result = new NotFoundObjectResult(messages);
                // }
                //context.ExceptionHandled=true;
            }

            
            base.OnException(context);
        }
    }
}