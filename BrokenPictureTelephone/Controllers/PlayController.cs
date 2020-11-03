using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BrokenPictureTelephone.Data;
using BrokenPictureTelephone.Models;
using Microsoft.AspNetCore.Mvc;

namespace BrokenPictureTelephone.Controllers
{
    public class PlayController : Controller
    {
        public ApplicationDbContext db;

        public PlayController(ApplicationDbContext theDatabase)
        {
            db = theDatabase;
        }

        public IActionResult Index()
        {
            // This is the /play thing
            // and we should grab a random game that we haven't played in yet (but don't do that part yet, cause I only have one user so far)
            // and then show the description from t h at game
            // and let the user draw that description
            // eventually, we'll deal with what happens if we have a drawing instead

            var games = db.Games.ToList();
            // grab a random number from 0 to games.Length/games.Count
            Random rnd = new Random();
            int myRandomNumber = rnd.Next(0, games.Count);
            Game myRandomGame = games[myRandomNumber]; // I now have a random game

            var lastEntryInGame = 
                db.Entries
                    .Where(e => e.GameId == myRandomGame.Id)
                    .OrderByDescending(e => e.DateAdded)
                    .FirstOrDefault();
            return View(lastEntryInGame);
        }

        public IActionResult NewGame()
        {
            return View();
        }

        public IActionResult Save(string description)
        {
            // save description in the database under a new game
            Game newgame = new Game(); // Create a new game
            newgame.Created = DateTime.Now; // Set when it was created

            Entry firstEntry = new Entry(); // Create a new entry (they gave us a description)
            firstEntry.Description = description;
            firstEntry.DateAdded = DateTime.Now;
            firstEntry.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            newgame.Entries = new List<Entry>(); // In the game, make sure entries is set up to be a new list
            newgame.Entries.Add(firstEntry); // Add the entry into the new game

            db.Games.Add(newgame); // Add the new game into the database (create the sql insert statement)
            db.SaveChanges(); // Save everything (run the sql statement that was just created)

            return View();
        }
    }
}
