using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RecordBackend.Models;
using RecordBackend.Services;
using System;
using System.Threading.Tasks;

namespace RecordBackend.Controllers
{
    /*--Фильтр ValidateModel
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }*/

    //[ValidateModel]
    //https azure [RequireHttps]
    [Route("api/[controller]")]
    public class RpcController : Controller
    {
        const string SessionKeyCookie = @"SessionKey";

        IRpcRepository rpcRepository;

        public RpcController(IRpcRepository rpcRepo)
        {
            rpcRepository = rpcRepo;
        }

        // POST api/rpc
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RpcRequest request)
        {
            /*https azure
            if (!HttpContext.Request.IsHttps)
            {
                logger.LogInformation(@"POST Bad request (only SSL)");
                return BadRequest("Only SSL");
            }*/

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //читаем ключ сессии из cookies
            string sessionKey = string.Empty;
            if (HttpContext.Request.Cookies.ContainsKey(SessionKeyCookie))
            {
                sessionKey = HttpContext.Request.Cookies[SessionKeyCookie];
            }

            string userAgent = Request.Headers["User-Agent"].ToString();
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            var requestContainer = new RpcRequestContainer()
            {
                RpcRequest = request,
                Identity = new Identity(sessionKey, userAgent, ipAddress)
            };

            try 
            {
                RpcResponseContainer responseContainer = await rpcRepository.ExecAsync(requestContainer);
            }
            catch (error)
            {
                RpcResponseContainer responseContainer = new RpcResponseContainer()
                {
                    RpcResponse = new RpcResponse()
                    {
                        Error = new RpcError()
                    }
                };
                
            }

            //изменяем агент в cookies
            if (string.IsNullOrEmpty(responseContainer.SessionKey))
            {
                //удаляем SessionKey из cookies
                HttpContext.Response.Cookies.Delete(SessionKeyCookie);
            }
            else if (!string.Equals(responseContainer.SessionKey, sessionKey))
            {
                //устанавливаем в cookies новый SessionKey если изменился
                HttpContext.Response.Cookies.Append(SessionKeyCookie, responseContainer.SessionKey, new CookieOptions() { HttpOnly = true, Expires = responseContainer.ExpirationDate ?? DateTime.MaxValue});
                //new CookieOptions { Domain = HttpContext.Request.Host.Host, /*SameSite = SameSiteMode.None,*/ HttpOnly = true, Expires = DateTime.MaxValue});
            }

            return Ok(responseContainer.RpcResponse);
        }
    }
}
