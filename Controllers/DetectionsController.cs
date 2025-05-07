using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Music.Areas.Identity.Data;
using Music.Data;
using Music.Models;

namespace Music.Controllers
{
    [Authorize]
    public class DetectionsController : Controller
    {
        private readonly MusicDbContext _context;
        private readonly UserManager<MusicUser> _userManager;

        public DetectionsController(MusicDbContext context, UserManager<MusicUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Detections
        public async Task<IActionResult> Index()
        {
            var musicDbContext = _context.Detections.Include(d => d.Track);
            return View(await musicDbContext.ToListAsync());
        }

        // GET: Detections/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            var user = await _userManager.GetUserAsync(User);
            if (id == null)
            {
                return NotFound();
            }

            var detection = await _context.Detections
                .Include(d => d.Track)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detection == null)
            {
                return NotFound();
            }

            var detections = await _context.Detections
                .AsNoTracking()
                .AsSplitQuery()
                .Include(d=>d.Track)
                .FirstOrDefaultAsync(d=>d.ArtistName == detection.ArtistName && 
                d.SongName == detection.SongName && d.Track.ArtistId != detection.Track.ArtistId);

            if(detections != null)
            {
                ViewData["id"] = detections.Track.ArtistId;
            }
            else
            {
                var randomUser = await _context.Users
                                         .Where(d => d.Id != user.Id)
                                        // This is executed in SQL
                                         .Select(d => new { d.Id })    // Just get what you need
                                         .FirstOrDefaultAsync();


                var track = new Track()
                {
                    AlbumId = detection.Track.AlbumId,
                    Approved = true,
                    ArtistId = randomUser.Id,
                    Genre = detection.Track.Genre,
                    Length = detection.Track.Length,
                    Name = detection.Track.Name,
                    Plays = (int)new Random().Next(2134, 10000),
                    TrackUrl = detection.Track.TrackUrl,
                    UnitPrice = detection.Track.UnitPrice
                };

                _context.Add(track);

                await _context.SaveChangesAsync();

                var detectionNew = new Detection()
                {
                    Id = detection.Id,
                    ArtistName = detection.ArtistName,
                    SongName = detection.SongName,
                    DateDetected = detection.DateDetected,
                    Confidence = detection.Confidence,
                    TrackId = track.Id
                };

                _context.Add(detectionNew);

                await _context.SaveChangesAsync();

                ViewData["id"] = track.ArtistId;
            }

                return View(detection);
        }

        // GET: Detections/Create
        public IActionResult Create()
        {
            ViewData["TrackId"] = new SelectList(_context.Tracks, "Id", "Id");
            return View();
        }

        // POST: Detections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TrackId,DateDetected,ArtistName,SongName,Confidence")] Detection detection)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrackId"] = new SelectList(_context.Tracks, "Id", "Id", detection.TrackId);
            return View(detection);
        }

        // GET: Detections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detection = await _context.Detections.FindAsync(id);
            if (detection == null)
            {
                return NotFound();
            }
            ViewData["TrackId"] = new SelectList(_context.Tracks, "Id", "Id", detection.TrackId);
            return View(detection);
        }

        // POST: Detections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TrackId,DateDetected,ArtistName,SongName,Confidence")] Detection detection)
        {
            if (id != detection.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetectionExists(detection.Id))
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
            ViewData["TrackId"] = new SelectList(_context.Tracks, "Id", "Id", detection.TrackId);
            return View(detection);
        }

        // GET: Detections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detection = await _context.Detections
                .Include(d => d.Track)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detection == null)
            {
                return NotFound();
            }

            return View(detection);
        }

        // POST: Detections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detection = await _context.Detections.FindAsync(id);
            if (detection != null)
            {
                _context.Detections.Remove(detection);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetectionExists(int id)
        {
            return _context.Detections.Any(e => e.Id == id);
        }
    }
}
