using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Music.Areas.Identity.Data;
using Music.Data;
using Music.Models;

namespace Music.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly MusicDbContext _context;

        private readonly UserManager<MusicUser> _userManager;

        public HomeController(ILogger<HomeController> logger, MusicDbContext context, UserManager<MusicUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {




            var user  = await _userManager.GetUserAsync(User);

          
            var tracks = await _context.Tracks.Where(d=>d.ArtistId == user.Id).ToListAsync();
            var artists = await _context.Users.ToListAsync();
            var detections = await _context.Detections
            
            .ToListAsync();

            var model = new HomeViewModel
            {
                Tracks = tracks,
                Artists = artists.Take(5).ToList(),
                Detections = detections
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class HomeViewModel
    {
        public List<Track> Tracks { get; set; }
        public List<User> Artists { get; set; }
        public List<Detection> Detections { get; set; }
    }
}
