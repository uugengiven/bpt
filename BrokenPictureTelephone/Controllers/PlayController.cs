using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using BrokenPictureTelephone.Data;
using BrokenPictureTelephone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BrokenPictureTelephone.Controllers
{
    public class PlayController : Controller
    {
        private ApplicationDbContext db;
        private IConfiguration config;

        public PlayController(ApplicationDbContext theDatabase, IConfiguration configuration)
        {
            db = theDatabase;
            config = configuration;
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

        public IActionResult SavePicture(string picturetext, int gameId)
        {
            // Get the picture in as text
            // it is officially Base64 at the moment
            // how do I get it into something we can save to azure?
            // how do I save to azure?

            // I need to save the picture in here somewhere
            // I should probably get the picture in as a thing
            string base64 = picturetext.Substring(picturetext.IndexOf(',') + 1);
            byte[] pictureArray = Convert.FromBase64String(base64);

            // Azure needs a stream and a filename, and then we can save it
            var pictureStream = new MemoryStream(pictureArray);
            string fileName = (Guid.NewGuid().ToString()) + ".jpg";

            // Azure needs your connection string like a db
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            if(string.IsNullOrEmpty(connectionString))
            {
                connectionString = config.GetValue<string>("AzureConnectionString");
            }
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            // Azure needs to know what folder you want to save in
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("bpt");
            // Then, you can get a blob writer thing from azure
            containerClient.UploadBlob(fileName, pictureStream);


            // Now it is time to save in the database
            Entry newEntry = new Entry();
            newEntry.PictureUrl = fileName;
            newEntry.DateAdded = DateTime.Now;
            newEntry.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            newEntry.GameId = gameId;

            // save entry into the database
            db.Entries.Add(newEntry);
            db.SaveChanges();

            return Redirect("/");
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

            return Redirect("/");
        }

        public IActionResult SaveDescription(string description, int gameId)
        {
            // Now it is time to save in the database
            Entry newEntry = new Entry();
            newEntry.Description = description;
            newEntry.DateAdded = DateTime.Now;
            newEntry.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            newEntry.GameId = gameId;

            // save entry into the database
            db.Entries.Add(newEntry);
            db.SaveChanges();

            return Redirect("/");
        }

        public IActionResult Show(int Id)
        {
            // grab all of the entries from a game
            // send them to the view so we can see them
            var allEntries = db.Entries.Where(e => e.GameId == Id).ToList();

            return View(allEntries);
        }
    }
}
