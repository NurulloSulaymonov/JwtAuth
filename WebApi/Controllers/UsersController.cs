using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Data.Entities;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController:ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<User> userManager,ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }
    
    
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("users")]
    public IActionResult Get()
    {
        return Ok("Hello World");
    }

    [HttpGet("logInfo")]
    public IActionResult LogInfo(string text)
    {
        _logger.LogInformation("Weather Forecast executing...");
        return Ok();
    }
    
    
    
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("Divide")]
    public IActionResult LogInfo(int  a, int b)
    {
        try
        {
            return Ok(a / b);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
        return Ok();
    }
}