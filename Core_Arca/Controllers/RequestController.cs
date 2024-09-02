using Azure.Core;
using Core_Arca.Data;
using Core_Arca.Helpers;
using Core_Arca.Helpers.Enums;
using Core_Arca.Models;
using Core_Arca.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json;

namespace Core_Arca.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ServiceNowHelper _serviceNowHelper;

        public RequestController(IUserService userService, ServiceNowHelper serviceNowHelper)
        {
            _userService = userService;
            _serviceNowHelper = serviceNowHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllArcaRequest()
        {
            try
            {
                string? uniqueCode = HttpContext.Items["UniqueCode"] as string;
                string url = UrlUtil.GetRequestUrl();
                //_serviceNowHelper.RemoveAuthorizationToken();
                dynamic responseData = await _serviceNowHelper.GetCustomerById(url);

                if (responseData != null)
                {
                    var result = JsonConvert.SerializeObject(responseData);
                    return Ok(result);
                }
                else
                    return BadRequest("Error: Record not found.");
            }
            catch (Exception ex)
            {
                LogErrorsToTable(HttpContext, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArcaRequest(int id)
        {
            try
            {   
                string? uniqueCode = HttpContext.Items["UniqueCode"] as string;
                string url = UrlUtil.GetRequestUrl(id);

                _serviceNowHelper.RemoveAuthorizationToken();
                TaskResult responseData = await _serviceNowHelper.GetCustomerById(url);

                if (responseData.IsSuccess)
                    return Ok(responseData.Content);
                else
                    return BadRequest(responseData.ErrorMessage);
            }
            catch (Exception ex)
            {
                LogErrorsToTable(HttpContext, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("/Arca")]
        public async Task<IActionResult> PostArcaRequest(ArcaRequest arcaRequest)
        {
            try
            {
                
                if (!Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues authValue))
                    return BadRequest(new ApiResponse<object>(HttpStatusCode.BadRequest, "Missing Authorization Header"));

                var (username, password) = _serviceNowHelper.GetRequestedUsernamePassword(authValue);
                bool IsAuthenticated = _userService.Authenticate(username, password);
                if (!IsAuthenticated)
                    return Unauthorized(new ApiResponse<object>(HttpStatusCode.Unauthorized, "Invalid username or password. Please contact Scantron support"));

                string? uniqueCode = HttpContext.Items["UniqueCode"] as string;
                if (arcaRequest == null)
                {
                    return BadRequest("The given Arca request is not a valid!");
                }

                arcaRequest.transmissionTimestamp = DateTime.Now.ToString("O");

                Device device = new();

                if (!string.IsNullOrEmpty(arcaRequest.serial))
                {
                    // Get all device info from serial
                    device = await _serviceNowHelper.GetCustomerBySerialAsync(arcaRequest.serial);

                    if(device == null)
                    {
                        return BadRequest("The serial number does not match with the existing records");
                    }
                }

                string[] result = DataConverter.Validate_ArcaRequest(arcaRequest, device);

                string sValidationIssues = result[0];
                string sValidationDecision = result[1];  // Continue or Stop

                // ----------------------------------------------------------------
                // Try to make an Omaha ticket with the request data 
                // ----------------------------------------------------------------
                var omahaTicketId = "";
                int iRequest = 0;
                dynamic responce = null;
                // sValidationDecision -- 0=No Issues, 1=Auto Ticket With Warning, 2=Auto Ticket With Review, 3=Manual Ticket, 4=Rejection
                RequestData data = DataConverter.GetSNArcaRequest(arcaRequest, device);
                if (
                    sValidationDecision == "No Issues"
                    || sValidationDecision == "Auto Ticket With Warning"
                    || sValidationDecision == "Auto Ticket With Review"
                    )
                {
                    // I'm really NOT using the passed address in creation of the ticket (maybe in updating SVRTICKD?) or better add a note with the mismatch
                    // Either use the Key TicketsController's "createHtsTicket" and UpdateHtsTicket
                    //or 
                    responce = await _serviceNowHelper.sendRequestToSNow(data);
                    omahaTicketId = responce[0].ticketId;
                }

                if (sValidationDecision != "Rejection" && omahaTicketId == "")
                {
                    // Email call center to make the ticket using the request data
                }

                // ----------------------------------------------------------------
                // Return Result of Submission Attempt
                // ----------------------------------------------------------------

                arcaRequest.processingIssues = sValidationIssues;

                if (sValidationDecision == "Rejection")
                {
                    arcaRequest.transmissionResult = "Rejection";
                    arcaRequest.omahaTicketId = "";
                }
                else
                {
                    if (omahaTicketId != "")
                    {
                        arcaRequest.transmissionResult = "Success";
                        arcaRequest.omahaTicketId = omahaTicketId;
                    }
                    else
                    {
                        arcaRequest.transmissionResult = "Manual Placement";
                        arcaRequest.omahaTicketId = "";
                    }
                }

                string url = UrlUtil.GetRequestUrl();
               // _serviceNowHelper.RemoveAuthorizationToken();

                if (responce != null && responce[0].message == "Success")
                {
                    return Ok(arcaRequest);
                }
                else
                {
                    return BadRequest(responce.message);
                }
            }
            catch (Exception ex)
            {
                LogErrorsToTable(HttpContext, ex);
                return BadRequest(ex.Message);
            }
        }

        private void LogErrorsToTable(HttpContext httpContext, Exception ex)
        {
            //try
            //{
            //    APIErrorLogs errorLogs = new()
            //    {
            //        BuildType = "Arca",
            //        Method = httpContext.Request.Method.ToUpper(),
            //        UniqueCode = httpContext.Items["UniqueCode"] as string,
            //        RequestedData = httpContext.Response.Body.ToString(),
            //        Summary = ex.Message,
            //        Description = ex.ToString(),
            //        Stacktrace = ex.StackTrace,
            //        CreatedOn = DateTime.Now,
            //        IpAddress = httpContext.Connection.RemoteIpAddress?.ToString()
            //    };
            //    _context.APIErrorLogs.Add(errorLogs);
            //    _context.SaveChanges();
            //}
            //catch
            //{
            //    //Left Intentionally
            //}
        }
    }
}
