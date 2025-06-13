using DynamicTimeTable.Models;
using Microsoft.AspNetCore.Mvc;

namespace DynamicTimeTable.Controllers
{
    public class TimeTableController : Controller
    {
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult SubmitInput(TimetableInputModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            TempData["TotalSubjects"] = model.TotalSubjects;
            TempData["TotalHours"] = model.TotalHours;
            TempData["WorkingDays"] = model.WorkingDays;
            TempData["SubjectsPerDay"] = model.SubjectsPerDay;

            TempData.Keep();
            return RedirectToAction("SubjectHours");
        }

        public IActionResult SubjectHours()
        {
            if (TempData["TotalSubjects"] == null || TempData["TotalHours"] == null)
                return RedirectToAction("Index");

            ViewBag.TotalSubjects = TempData["TotalSubjects"];
            ViewBag.TotalHours = TempData["TotalHours"];

            TempData.Keep();
            return View();
        }

        [HttpPost]
        public IActionResult GenerateTimetable(List<SubjectHoursModel> subjects)
        {
            int totalHours = Convert.ToInt32(TempData["TotalHours"]);
            int subjectsPerDay = Convert.ToInt32(TempData["SubjectsPerDay"]);
            int workingDays = Convert.ToInt32(TempData["WorkingDays"]);
            int totalEntered = subjects.Sum(s => s.WeeklyHours);

            if (totalEntered != totalHours)
            {
                ViewBag.TotalSubjects = TempData["TotalSubjects"];
                ViewBag.TotalHours = totalHours;
                ViewBag.Error = "Total subject hours must equal total hours for the week.";
                TempData.Keep();
                return View("SubjectHours");
            }

            // Create and shuffle the subject pool
            var subjectPool = subjects
                .SelectMany(s => Enumerable.Repeat(s.SubjectName, s.WeeklyHours))
                .OrderBy(_ => Guid.NewGuid())
                .ToList();

            List<List<string>> timetable = new();
            int index = 0;
            for (int i = 0; i < subjectsPerDay; i++)
            {
                List<string> row = new();
                for (int j = 0; j < workingDays; j++)
                {
                    row.Add(subjectPool[index++]);
                }
                timetable.Add(row);
            }

            TempData["TimetableJson"] = System.Text.Json.JsonSerializer.Serialize(timetable);
            TempData["Rows"] = subjectsPerDay;
            TempData["Cols"] = workingDays;

            return RedirectToAction("Result");
        }


        public IActionResult Result()
        {
            if (TempData["TimetableJson"] is not string timetableJson ||
                TempData["Rows"] is not int rows ||
                TempData["Cols"] is not int cols)
            {
                return RedirectToAction("Index");
            }

            var timetable = System.Text.Json.JsonSerializer.Deserialize<List<List<string>>>(timetableJson);

            ViewBag.Table = timetable;
            ViewBag.Rows = rows;
            ViewBag.Cols = cols;

            return View();
        }

    }
}
