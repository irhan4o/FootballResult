using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FootballResult.Models;
using FootballResult.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.Runtime.ConstrainedExecution;

namespace FootballResult.Controllers
{
    public class TeamsController : Controller
    {
        private readonly FootballResultDB _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeamsController(FootballResultDB context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        // GET: Teams
        public async Task<IActionResult> Index(string searchString, string sortOrder, string filterTeam)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameFirstSortParm"] = String.IsNullOrEmpty(sortOrder) ? "namefirst_desc" : "";
            ViewData["NameSecondSortParm"] = sortOrder == "NameSecond" ? "namesecond_desc" : "NameSecond";

            // Вземи всички уникални имена на отбори за филтър
            var teamsQuery = _context.Team.AsQueryable();
            var teamNames = await teamsQuery
                .Select(t => t.NameFirstTeam)
                .Union(teamsQuery.Select(t => t.NameSecoundTeam))
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();
            ViewBag.TeamNames = teamNames;

            // Филтриране по търсене
            if (!String.IsNullOrEmpty(searchString))
            {
                teamsQuery = teamsQuery.Where(t =>
                    t.NameFirstTeam.Contains(searchString) ||
                    t.NameSecoundTeam.Contains(searchString) ||
                    (t.Description != null && t.Description.Contains(searchString))
                );
            }

            // Филтриране по избран отбор
            if (!String.IsNullOrEmpty(filterTeam))
            {
                teamsQuery = teamsQuery.Where(t =>
                    t.NameFirstTeam == filterTeam || t.NameSecoundTeam == filterTeam
                );
            }

            // Сортиране
            switch (sortOrder)
            {
                case "namefirst_desc":
                    teamsQuery = teamsQuery.OrderByDescending(t => t.NameFirstTeam);
                    break;
                case "NameSecond":
                    teamsQuery = teamsQuery.OrderBy(t => t.NameSecoundTeam);
                    break;
                case "namesecond_desc":
                    teamsQuery = teamsQuery.OrderByDescending(t => t.NameSecoundTeam);
                    break;
                default:
                    teamsQuery = teamsQuery.OrderBy(t => t.NameFirstTeam);
                    break;
            }

            return View(await teamsQuery.ToListAsync());
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Team
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create(TeamsViewModel models)
        {
            if (ModelState.IsValid)
            {
                var teamEntity = new Team
                {
                    NameFirstTeam = models.NameFirstTeam,
                    NameSecoundTeam = models.NameSecoundTeam,
                    Description = models.Description
                };

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Качване на първата снимка
                if (models.PictureFirst != null && models.PictureFirst.Length > 0)
                {
                    string uniqueFileNameFirst = Guid.NewGuid().ToString() + "_" + Path.GetFileName(models.PictureFirst.FileName);
                    string filePathFirst = Path.Combine(uploadsFolder, uniqueFileNameFirst);

                    using (var fileStream = new FileStream(filePathFirst, FileMode.Create))
                    {
                        await models.PictureFirst.CopyToAsync(fileStream);
                    }

                    teamEntity.PictureFirst = $"/uploads/{uniqueFileNameFirst}";
                }

                // Качване на втората снимка
                if (models.PictureSecound != null && models.PictureSecound.Length > 0)
                {
                    string uniqueFileNameSecound = Guid.NewGuid().ToString() + "_" + Path.GetFileName(models.PictureSecound.FileName);
                    string filePathSecound = Path.Combine(uploadsFolder, uniqueFileNameSecound);

                    using (var fileStream = new FileStream(filePathSecound, FileMode.Create))
                    {
                        await models.PictureSecound.CopyToAsync(fileStream);
                    }

                    teamEntity.PictureSecound = $"/uploads/{uniqueFileNameSecound}";
                }

                _context.Team.Add(teamEntity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(models);
        }


        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Team.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameFirstTeam,NameSecoundTeam,Description")] Team team,
    IFormFile PictureFirst, IFormFile PictureSecound)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            // Вземаме съществуващия запис от базата
            var existingTeam = await _context.Team.FindAsync(id);
            if (existingTeam == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Актуализираме само текстовите полета
                    existingTeam.NameFirstTeam = team.NameFirstTeam;
                    existingTeam.NameSecoundTeam = team.NameSecoundTeam;
                    existingTeam.Description = team.Description;

                    // Обработка на първата снимка
                    if (PictureFirst != null && PictureFirst.Length > 0)
                    {
                        existingTeam.PictureFirst = await UploadImage(PictureFirst, existingTeam.PictureFirst);
                    }

                    // Обработка на втората снимка
                    if (PictureSecound != null && PictureSecound.Length > 0)
                    {
                        existingTeam.PictureSecound = await UploadImage(PictureSecound, existingTeam.PictureSecound);
                    }

                    _context.Update(existingTeam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        private async Task<string> UploadImage(IFormFile newImage, string existingImagePath)
        {
            // Изтриване на старата снимка (ако има)
            if (!string.IsNullOrEmpty(existingImagePath))
            {
                string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingImagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Качване на новата снимка
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(newImage.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await newImage.CopyToAsync(fileStream);
            }

            return $"/uploads/{uniqueFileName}";
        }


        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Team
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Team.FindAsync(id);
            if (team != null)
            {
                _context.Team.Remove(team);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _context.Team.Any(e => e.Id == id);
        }
    }
}
