using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;


namespace MyCoreApp.Pages
{
    public class TestModel : PageModel
    {
        [BindProperty, Required(ErrorMessage = "�������� �����")]
        public string Question1 { get; set; }

        [BindProperty]
        public string[] Question2 { get; set; } = Array.Empty<string>();

        [BindProperty, Required(ErrorMessage = "������� �����")]
        public string Question3 { get; set; }

        public void OnGet()
        {
            // ������������� ������ ����� ��� �������������
        }

        public IActionResult OnPostSubmitTest()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ������ �������� ������� � �������� ������
            int score = CalculateScore();

            // ��������������� �� �������� �����������
            return RedirectToPage("/Results", new { score = score });
        }

        private int CalculateScore()
        {
            int score = 0;

            // �������� �������
            if (Question1 == "4") score += 1;

            if (Question2.Contains("2") && Question2.Contains("-2") && Question2.Length == 2)
                score += 1;

            if (Question3?.ToLower() == "3") score += 1;

            return score;
        }
    }
}