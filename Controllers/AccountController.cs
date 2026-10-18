using AutoMapper;
using ChatApp.DTOs;
using ChatApp.Entities;
using ChatApp.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _usermanager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(ITokenService tokenService,UserManager<AppUser>usermanager,IMapper mapper)
        {
            _tokenService = tokenService;
            _usermanager = usermanager;
            _mapper = mapper;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("Username is taken.");
            }

            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();

            var result = await _usermanager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var roleResult = await _usermanager.AddToRoleAsync(user, "MEMBER");

            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _usermanager.Users.Include(p => p.Photos)
            .FirstOrDefaultAsync(u => u.UserName == loginDto.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            var result=await _usermanager.CheckPasswordAsync(user, loginDto.Password);
            if(!result)return Unauthorized("Invalid password.");

              

            return new UserDto
            {
                Username = user.UserName,
                Token =await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                KnownAs=user.KnownAs,
                Gender=user.Gender
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _usermanager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }

}