using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
    public class TracksController : Controller
    {
        private readonly MusicDbContext _context;

        private IWebHostEnvironment _env;

        private readonly UserManager<MusicUser> _userManager;

        public TracksController(MusicDbContext context, UserManager<MusicUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        // GET: Tracks
        public async Task<IActionResult> Index()
        {

            var user  = await _userManager.GetUserAsync(User);

            var musicDbContext = _context.Tracks
                 .Include(d => d.Detections)
            .Where(t => t.ArtistId == user.Id);
           
            return View(await musicDbContext.ToListAsync());
        }

        // GET: Tracks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _context.Tracks
                .Include(t => t.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        // GET: Tracks/Create
        public IActionResult Create()
        {

            ViewData["ArtistId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Tracks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Track track)
        {
            var user = await _userManager.GetUserAsync(User);
            track.ArtistId = user.Id;


            if (ModelState.IsValid)
            {
                track.TrackUrl = "coming";
                _context.Add(track);
                var saved = await _context.SaveChangesAsync();

                if (saved > 0)
                {
                    var file = track.AudioFile;

                    //save to wwwroot/audios/artistId/{track-details}
                    var folder = Path.Combine(_env.WebRootPath, "audios", user.Id);

                    //create if not exists
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    var path = Path.Combine(folder, track.Name);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    track.TrackUrl = $"/audios/{user.Id}/{track.Name}";

                    var result = await IdentifyTrackWithAudD(path);

                    if (result.Status.ToLower().Equals("success"))
                    {
                        track.Approved = false;
                        Detection detection = new Detection()
                        {
                            TrackId = track.Id,
                            ArtistName = result.Result?.Artist ?? "Not-Known",
                            SongName = result.Result?.Title ??"Not-Known",
                            Confidence = 100,
                            DateDetected = DateTime.Now

                        };

                        _context.Add(detection);


                    }
                    else
                    {
                        track.Approved = true;
                    }

                    _context.Update(track);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }




            ViewData["ArtistId"] = new SelectList(_context.Users, "Id", "Id", track.ArtistId);
            return View(track);
        }

        private async Task<AudDResponse> IdentifyTrackWithAudD(string filePath)
        {
            var client = new HttpClient();
            using var form = new MultipartFormDataContent();

            form.Add(new StringContent("a72a1baaa05b2ea9553ffb17bf9b5ba8"), "api_token");
            form.Add(new StringContent("apple_music,spotify,youtube"), "return");

            var fileStream = System.IO.File.OpenRead(filePath);
            form.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

            var response = await client.PostAsync("https://api.audd.io/", form);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AudDResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }


        // GET: Tracks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _context.Tracks.FindAsync(id);
            if (track == null)
            {
                return NotFound();
            }
            ViewData["ArtistId"] = new SelectList(_context.Users, "Id", "Id", track.ArtistId);
            return View(track);
        }

        // POST: Tracks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Track track)
        {
            if (id != track.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(track);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrackExists(track.Id))
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
            ViewData["ArtistId"] = new SelectList(_context.Users, "Id", "Id", track.ArtistId);
            return View(track);
        }

        // GET: Tracks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var track = await _context.Tracks
                .Include(t => t.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        // POST: Tracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var track = await _context.Tracks.FindAsync(id);
            if (track != null)
            {
                _context.Tracks.Remove(track);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrackExists(int id)
        {
            return _context.Tracks.Any(e => e.Id == id);
        }
    }
}
