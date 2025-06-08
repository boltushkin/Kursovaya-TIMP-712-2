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
            [Required(ErrorMessage = "������� �����")]
            [StringLength(20, MinimumLength = 4, ErrorMessage = "����� ������ ���� �� 4 �� 20 ��������")]
            [Display(Name = "�����")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "������� email")]
            [EmailAddress(ErrorMessage = "������������ email")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "������� ������")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "������ ������ ���� �� ����� 6 ��������")]
            [DataType(DataType.Password)]
            [Display(Name = "������")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "����������� ������")]
            [Compare("Password", ErrorMessage = "������ �� ���������")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "�������� ��� ������������")]
            [Display(Name = "��� ������������")]
            public string UserType { get; set; } = string.Empty;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Input.Password != Input.ConfirmPassword)
            {
                ModelState.AddModelError("Input.ConfirmPassword", "������ �� ���������");
                return Page();
            }

            try
            {
                bool isRegistered = await _userService.RegisterUserAsync(
                    Input.Username, Input.Password, Input.Email, Input.UserType
                );

                if (!isRegistered)
                {
                    ModelState.AddModelError("", "������������ � ����� ������� ��� email ��� ����������");
                    return Page();
                }

                TempData["SuccessMessage"] = "����������� �������! ������ �� ������ ����� � �������.";
                return RedirectToPage("/Login");
            }
            catch
            {
                ModelState.AddModelError("", "��������� ������ ��� �����������. ����������, ���������� �����.");
                return Page();
            }
        }
    }
}
