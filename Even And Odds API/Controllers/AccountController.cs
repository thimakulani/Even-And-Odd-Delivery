using Even_And_Odds_API.Data;
using Even_And_Odds_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Even_And_Odds_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private UserManager<ApplicationUser> manager;
        private SignInManager<ApplicationUser> signInManager;
        private readonly AppDbContext _context;


        public AccountController(UserManager<ApplicationUser> manager, SignInManager<ApplicationUser> signInManager, AppDbContext context)
        {
            this.manager = manager;
            this.signInManager = signInManager;
            _context = context;
        }
        [HttpPost("login")]
        public async Task<ActionResult<ApplicationUser>> Login(UserSignUp login)
        {
            var results = await signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);

            if (results.Succeeded)
            {
                var user = await manager.FindByEmailAsync(login.Email);
                if (user != null)
                {
                    return Ok(user);
                }
                else
                {
                    return NotFound("User not found");
                }
            }
            else
            {
                return NotFound("Incorrect usrname or password");
            }
        }
        [HttpPost("resetPassword")]
        public async Task<ActionResult> ResetPassword(UserLogin passwordReset)
        {
            var results = await manager.FindByEmailAsync(passwordReset.Email);

            if (results != null)
            {
                string token = await manager.GeneratePasswordResetTokenAsync(results);

                if (token != null)
                {
                    var token_results = await manager.ResetPasswordAsync(results, token, passwordReset.Password);

                    if (token_results.Succeeded)
                    {
                        return Ok("Password reset was succesful");
                    }
                    else
                    {
                        string error = "";

                        foreach (var err in token_results.Errors)
                        {
                            error = $"{error}{err.Description}\n";
                        }

                        return BadRequest();
                    }
                }
                else
                {
                    return NotFound("User not found");
                }
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost("signup")]
        public async Task<ActionResult> SignUp(UserSignUp signUp)
        {
            ApplicationUser users = new ApplicationUser()
            {
                Email = signUp.Email,
                Name = signUp.Name,
                Surname = signUp.Surname,
                PhoneNumber = signUp.Phone,
                Type = signUp.Type,
                UserName = signUp.Email
            };

            var results = await manager.CreateAsync(users, signUp.Password);

            if (results.Succeeded)
            {
                return Ok(users.Id);
            }
            else
            {
                string error = "";

                foreach (var res in results.Errors)
                {
                    error = $"{error} {res.Description}\n";
                }

                return BadRequest(error);
            }
        }
    }
}
public class UserLogin
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UpdateUser
{
    public string Id { get; set; }
    public string Url { get; set; }
}

public class UserSignUp
{

    public string Name { get; set; }
    public string Email { get; set; }
    public string Surname { get; set; }
    public string Phone { get; set; }
    public string Role { get; set; }
    public string RegNo { get; set; }
    public string Type { get; set; }
    public string Color { get; set; }
    public string Make { get; set; }
    public string Status { get; set; }
    public string Password { get; set; }
}