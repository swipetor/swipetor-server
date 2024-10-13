using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwipetorApp.Areas.Api.Models;
using SwipetorApp.Models.DbEntities;
using SwipetorApp.Models.Enums;
using SwipetorApp.Services.Auth;
using SwipetorApp.Services.Contexts;
using SwipetorApp.Services.Users;
using WebLibServer.Contexts;
using WebLibServer.Exceptions;
using WebLibServer.Utils;

namespace SwipetorApp.Areas.Api;

[ApiController]
[Area(AreaNames.Api)]
[Route("api/auth")]
public class AuthApi(
    IDbProvider dbProvider,
    UserCx userCx,
    UsernameCheckerSvc usernameCheckerSvc,
    AuthSvc authSvc,
    IConnectionCx connectionCx,
    AuthProtectionSvc authProtectionSvc,
    LoginCodeSvc loginCodeSvc)
    : Controller
{
    [HttpPost("email-login-code")]
    public async Task<IActionResult> EmailLoginCode(AuthEmailLoginCodeModel model)
    {
        if (!ModelState.IsValid) throw new HttpJsonError("Please check and provide all input.");

        await loginCodeSvc.ThrowIfNotAllowedToProceed(model.Email);

        await using var db = dbProvider.Create();

        var user = db.Users.SingleOrDefault(u => u.Email == model.Email);

        var loginRequest = new LoginRequest
        {
            Id = Guid.NewGuid(),
            UserId = user?.Id,
            Email = model.Email,
            EmailCode = StringUtils.RandomString(new Random().Next(6, 8)).ToUpperInvariant()
        };

        db.LoginRequests.Add(loginRequest);
        await db.SaveChangesAsync();

        await loginCodeSvc.EmailLoginCode(loginRequest);

        return Json(new
        {
            loginRequestId = loginRequest.Id
        });
    }

    [HttpPost("submit-login-code")]
    public async Task<IActionResult> SubmitLoginCode(AuthEnterLoginCodeReqModel model)
    {
        if (!ModelState.IsValid)
            throw new HttpJsonError("Please check and provide all input.");

        authProtectionSvc.ThrowIfManyLoginAttemptsFromThisIpAddress();
        authProtectionSvc.ThrowIfLoginRequestIdAttemptedBefore(model.LoginRequestId);
        
        model.LoginCode = model.LoginCode.Trim().ToUpperInvariant();

        // Add Login Try into DB
        await using var db = dbProvider.Create();
        var loginAttempt = new LoginAttempt
        {
            Id = Guid.NewGuid(),
            LoginRequestId = model.LoginRequestId,
            TriedEmailCode = model.LoginCode
        };
        db.LoginAttempts.Add(loginAttempt);
        await db.SaveChangesAsync();
        // End Login Try

        var loginRequest = db.LoginRequests.FirstOrDefault(l =>
            l.Id == model.LoginRequestId && l.CreatedAt > DateTime.UtcNow.AddMinutes(-30));

        if (loginRequest == null) throw new HttpJsonError("Invalid login request. [INEXT]");

        authProtectionSvc.ThrowIfManyLoginAttemptsToUserId(loginRequest.UserId);

        if (loginRequest.IsUsed) throw new HttpJsonError("Already used login code.");

        if (loginRequest.BrowserAgent != connectionCx.BrowserAgent)
            throw new HttpJsonError("Invalid login request. [BRWAG]");
        if (loginRequest.EmailCode != model.LoginCode) throw new HttpJsonError("Invalid login code. [INCD]");

        loginRequest.IsUsed = true;
        await db.SaveChangesAsync();

        var user = authSvc.GetOrCreateUser(loginRequest.Email);
        await authSvc.LoginWith(user);

        return Json(new
        {
            hasUsername = !string.IsNullOrWhiteSpace(user.Username)
        });
    }

    [HttpPost("set-username")]
    public async Task<OkResult> SetUsername([FromBody] AuthSetUsernameRequestModel model)
    {
        var currentUser = userCx.Value;
        var username = model.Username?.Trim();

        if (!string.IsNullOrEmpty(currentUser.Username)) return Ok();

        usernameCheckerSvc.CheckAndThrowIfInvalid(username);

        await using var db = dbProvider.Create();

        var user = db.Users.Single(u => u.Id == currentUser.Id);

        if (!string.IsNullOrEmpty(user.Username)) return Ok();

        user.Username = username;
        await db.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("check-username")]
    public IActionResult CheckUsername(AuthCheckUsernameReqModel model)
    {
        var username = model.Username.Trim();

        try
        {
            usernameCheckerSvc.CheckAndThrowIfInvalid(username);
        }
        catch (HttpJsonError e)
        {
            return Json(new { available = false, error = e.Title });
        }

        return Json(new { available = true });
    }

    [HttpGet("suggest-usernames")]
    public IActionResult SuggestUsernames()
    {
        using var db = dbProvider.Create();

        // Fetch 10 random words
        var randomWords = db.EnglishWords
            .OrderBy(w => Guid.NewGuid())
            .Take(10)
            .Select(w => w.Word)
            .ToList();

        // Create the array with combined words
        var combinedWords = new string[randomWords.Count / 2];
        for (var i = 0; i < randomWords.Count / 2; i++)
        {
            var firstWord = TruncateWord(randomWords[i], 5);
            var secondWord = TruncateWord(randomWords[randomWords.Count - 1 - i], 5);
            var random = new Random();
            combinedWords[i] = firstWord + secondWord + random.Next(10, 100);
        }

        return Json(combinedWords);
    }

    private string TruncateWord(string word, int maxLength)
    {
        word = word.Length <= maxLength ? word : word.Substring(0, maxLength);
        return StringUtils.FirstLetterToUpper(word);
    }

    [Authorize]
    [Route("logout/{userId:int}")]
    public async Task<IActionResult> Logout(int userId)
    {
        if (userId != userCx.ValueOrNull?.Id) return Redirect("~/");

        await HttpContext.SignOutAsync();

        return Redirect("~/");
    }
}