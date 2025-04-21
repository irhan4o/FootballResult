using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FootballResult.Models;
using FootballResult.Models.Data;

namespace FootballResult.Controllers
{
    public class Stats_TeamsController : Controller
    {
        private readonly FootballResultDB _context;

        public Stats_TeamsController(FootballResultDB context)
        {
            _context = context;
        }

        // GET: Stats_Teams
        public async Task<IActionResult> Index()
        {
            var footballResultDB = _context.Stats_Teams.Include(s => s.Stats).Include(s => s.Team);
            return View(await footballResultDB.ToListAsync());
        }

        // GET: Stats_Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stats_Teams = await _context.Stats_Teams
                .Include(s => s.Stats)
                .Include(s => s.Team)
                .FirstOrDefaultAsync(m => m.TeamFId == id);
            if (stats_Teams == null)
            {
                return NotFound();
            }

            return View(stats_Teams);
        }

        // GET: Stats_Teams/Create
        public IActionResult Create()
        {
            ViewData["StatsFId"] = new SelectList(_context.Stats, "Id", "Id");
            ViewData["TeamFId"] = new SelectList(_context.Team, "Id", "NameFirstTeam");
            return View();
        }

        // POST: Stats_Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TeamFId,StatsFId")] Stats_Teams stats_Teams)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stats_Teams);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StatsFId"] = new SelectList(_context.Stats, "Id", "Id", stats_Teams.StatsFId);
            ViewData["TeamFId"] = new SelectList(_context.Team, "Id", "NameFirstTeam", stats_Teams.TeamFId);
            return View(stats_Teams);
        }

        // GET: Stats_Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stats_Teams = await _context.Stats_Teams.FindAsync(id);
            if (stats_Teams == null)
            {
                return NotFound();
            }
            ViewData["StatsFId"] = new SelectList(_context.Stats, "Id", "Id", stats_Teams.StatsFId);
            ViewData["TeamFId"] = new SelectList(_context.Team, "Id", "NameFirstTeam", stats_Teams.TeamFId);
            return View(stats_Teams);
        }

        // POST: Stats_Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeamFId,StatsFId")] Stats_Teams stats_Teams)
        {
            if (id != stats_Teams.TeamFId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stats_Teams);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Stats_TeamsExists(stats_Teams.TeamFId))
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
            ViewData["StatsFId"] = new SelectList(_context.Stats, "Id", "Id", stats_Teams.StatsFId);
            ViewData["TeamFId"] = new SelectList(_context.Team, "Id", "NameFirstTeam", stats_Teams.TeamFId);
            return View(stats_Teams);
        }

        // GET: Stats_Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stats_Teams = await _context.Stats_Teams
                .Include(s => s.Stats)
                .Include(s => s.Team)
                .FirstOrDefaultAsync(m => m.TeamFId == id);
            if (stats_Teams == null)
            {
                return NotFound();
            }

            return View(stats_Teams);
        }

        // POST: Stats_Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stats_Teams = await _context.Stats_Teams.FindAsync(id);
            if (stats_Teams != null)
            {
                _context.Stats_Teams.Remove(stats_Teams);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Stats_TeamsExists(int id)
        {
            return _context.Stats_Teams.Any(e => e.TeamFId == id);
        }
    }
}
