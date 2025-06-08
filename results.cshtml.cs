using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace MyCoreApp.Pages
{
    public class ResultsModel : PageModel
    {
        public TestResult TestResult { get; set; }

        public void OnGet(int testId, int score)
        {
            // ����� ������ ���� ������ ��������� ����������� �� ���� ������
            // ��� ������ � mock-�������
            TestResult = new TestResult
            {
                TestId = testId,
                TestName = "���� �� ����������",
                PercentageCorrect = score,
                Grade = CalculateGrade(score),
                CorrectAnswers = (int)Math.Round(20 * (score / 100.0)),
                TotalQuestions = 20,
                TimeTaken = 25,
                CompletionDate = DateTime.Now,
                ShowSolutions = true,
                SolutionsUrl = "#",
                QuestionResults = new List<QuestionResult>
                {
                    new QuestionResult
                    {
                        QuestionNumber = 1,
                        QuestionText = "��� ����� 2 + 2?",
                        UserAnswer = "4",
                        CorrectAnswer = "4",
                        IsCorrect = true
                    },
                    new QuestionResult
                    {
                        QuestionNumber = 2,
                        QuestionText = "������ ���������: x? - 4 = 0",
                        UserAnswer = "x = 2",
                        CorrectAnswer = "x = 2, x = -2",
                        IsCorrect = false
                    },
                    new QuestionResult
                    {
                        QuestionNumber = 3,
                        QuestionText = "���� ����� ���������� ������ �� 9?",
                        UserAnswer = "3",
                        CorrectAnswer = "3",
                        IsCorrect = true
                    }
                }
            };
        }

        private string CalculateGrade(int percentage)
        {
            return percentage switch
            {
                >= 90 => "5",
                >= 75 => "4",
                >= 60 => "3",
                >= 40 => "2",
                _ => "1"
            };
        }
    }

    public class TestResult
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public int PercentageCorrect { get; set; }
        public string Grade { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public int TimeTaken { get; set; } // � �������
        public DateTime CompletionDate { get; set; }
        public bool ShowSolutions { get; set; }
        public string SolutionsUrl { get; set; }
        public List<QuestionResult> QuestionResults { get; set; }
    }

    public class QuestionResult
    {
        public int QuestionNumber { get; set; }
        public string QuestionText { get; set; }
        public string UserAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}