using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyCoreApp.Services;
using System.ComponentModel.DataAnnotations;

namespace MyCoreApp.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "Введите логин")]
            [StringLength(20, MinimumLength = 4, ErrorMessage = "Логин должен быть от 4 до 20 символов")]
            [Display(Name = "Логин")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Введите email")]
            [EmailAddress(ErrorMessage = "Некорректный email")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Введите пароль")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Подтвердите пароль")]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Выберите тип пользователя")]
            [Display(Name = "Тип пользователя")]
            public string UserType { get; set; } = string.Empty;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Input.Password != Input.ConfirmPassword)
            {
                ModelState.AddModelError("Input.ConfirmPassword", "Пароли не совпадают");
                return Page();
            }

            try
            {
                bool isRegistered = await _userService.RegisterUserAsync(
                    Input.Username, Input.Password, Input.Email, Input.UserType
                );

                if (!isRegistered)
                {
                    ModelState.AddModelError("", "Пользователь с таким логином или email уже существует");
                    return Page();
                }

                TempData["SuccessMessage"] = "Регистрация успешна! Теперь вы можете войти в систему.";
                return RedirectToPage("/Login");
            }
            catch
            {
                ModelState.AddModelError("", "Произошла ошибка при регистрации. Пожалуйста, попробуйте позже.");
                return Page();
            }
        }
    }
}
