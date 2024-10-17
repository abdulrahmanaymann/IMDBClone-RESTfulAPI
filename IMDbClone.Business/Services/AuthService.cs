using AutoMapper;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.AuthDTOs;
using IMDbClone.Core.DTOs.UserDTOs;
using IMDbClone.Core.Entities;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;

namespace IMDbClone.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ITokenService _tokenService;

        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ITokenService tokenService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginDTO)
        {
            var user = await _userManager.FindByNameAsync(loginDTO.UserName);
            if (user == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null,
                    Message = "Invalid username or password."
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
            if (!result.Succeeded)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null,
                    Message = "Invalid username or password."
                };
            }

            var token = _tokenService.CreateToken(user);

            return new LoginResponseDTO()
            {
                Token = token,
                User = _mapper.Map<UserDTO>(user),
                Message = "Login successful."
            };
        }

        public async Task<UserDTO> RegisterAsync(RegisterationRequestDTO registerDTO)
        {
            bool isUserUnique = await _unitOfWork.User.IsUniqueUser(registerDTO.UserName);
            if (!isUserUnique)
            {
                throw new Exception("User already exists! Please try again with a different username.");
            }

            // step 1: create a new user
            var user = _mapper.Map<ApplicationUser>(registerDTO);
            try
            {
                // step 2: create the user
                var result = await _userManager.CreateAsync(user, registerDTO.Password);
                if (!result.Succeeded)
                {
                    throw new Exception("User creation failed! Please check user details and try again.");
                }

                // step 3: add user to the default role
                await _userManager.AddToRoleAsync(user, "User");

                return _mapper.Map<UserDTO>(user);
            }

            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the user. Please try again later.", ex);
            }
        }
    }
}
