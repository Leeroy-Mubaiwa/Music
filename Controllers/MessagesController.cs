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
    public class MessagesController : Controller
    {
        private readonly MusicDbContext _context;

        private readonly UserManager<MusicUser> _userManager;

        public MessagesController(MusicDbContext context, UserManager<MusicUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Messages
        public async Task<IActionResult> Index(string artistId)
        {
            var user = await _userManager.GetUserAsync(User);


            if (!string.IsNullOrEmpty(artistId))
            {
                ViewData["artistId"] = artistId;

                if(await _context.Messages.AsNoTracking().FirstOrDefaultAsync(d=>d.SourceUser == user.Id && d.TargetUser == artistId) == null)
                {
                    Message message = new Message()
                    {
                        Message1 = "Hello, I want to collaborate on this song",
                        DateSent = DateTime.Now,
                        SourceUser = user.Id,
                        TargetUser = artistId,

                    };

                    _context.Add(message);
                    await _context.SaveChangesAsync();
                }

             
            }

        var messageBody = await messageBodies(user);
            return View(messageBody);

        }

        private async Task<List<MessageBody>> messageBodies(MusicUser user)
        {

            var messages = await _context.Messages
                .AsNoTracking()
                .Include(d => d.TargetUserNavigation)
                .AsNoTracking()
                .AsSplitQuery()
                .Where(d => d.SourceUser == user.Id)
                .ToListAsync();


            var messageBody = messages.GroupBy(d => d.TargetUser).Select(m => new MessageBody()
            {
                Artist = m.FirstOrDefault().TargetUserNavigation,
                Messages = m.ToList()

            }).ToList();

            return messageBody;
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.SourceUserNavigation)
                .Include(m => m.TargetUserNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["SourceUser"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["TargetUser"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string message , string destination)
        {

            var user = await _userManager.GetUserAsync(User);
            var text = new Message()
            {
                Message1 = message,
                TargetUser = destination,
                SourceUser = user.Id,
                DateSent = DateTime.Now,


            };

            _context.Add(text);
            await _context.SaveChangesAsync();
            ViewData["artistId"] = destination;
            //var messageBody = await messageBodies(user);
            return RedirectToAction(nameof(Index), new { artistId = destination });
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["SourceUser"] = new SelectList(_context.Users, "Id", "Id", message.SourceUser);
            ViewData["TargetUser"] = new SelectList(_context.Users, "Id", "Id", message.TargetUser);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SourceUser,TargetUser,DateSent,Message1")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
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
            ViewData["SourceUser"] = new SelectList(_context.Users, "Id", "Id", message.SourceUser);
            ViewData["TargetUser"] = new SelectList(_context.Users, "Id", "Id", message.TargetUser);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.SourceUserNavigation)
                .Include(m => m.TargetUserNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }

    }

    public class MessageBody
    {
        public User Artist { get; set; } 
        public List<Message> Messages { get; set; }
    }
}
