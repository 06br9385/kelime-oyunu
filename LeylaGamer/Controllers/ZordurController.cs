using LeylaGamer.Models;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;


namespace LeylaGamer.Controllers
{
    public class ZordurController : Controller
    {
        private readonly ILogger<ZordurController> _logger;

        public ZordurController(ILogger<ZordurController> logger)
        {
            _logger = logger;
        }


        public IActionResult trTR()
        {
            AddVisitor();/*siteye giriş yapanı ekle*/
            GetPageHitCount();/*siteye giriş yapanların toplam sayısı*/
            GetGameOverCount();/*oyun kaybedenlerin toplam sayısı*/
            ViewBag.Lang = true;
            return View(GetScoreList());
        }


        public IActionResult enEN()
        {
            ViewBag.Lang = false;
            AddVisitor();/*siteye giriş yapanı ekle*/
            GetPageHitCount();/*siteye giriş yapanların toplam sayısı*/
            GetGameOverCount();/*oyun kaybedenlerin toplam sayısı*/
            return View(GetScoreList());
        }

        public void AddGameOver()
        {
            using (var db = new LiteDatabase(@"NoSqlDB.db"))
            {
                var gameovers = db.GetCollection<GameOverCount>("gameovers");
                GameOverCount gameover = new GameOverCount();
                gameover.overCount = 1;
                gameover.overDate = DateTime.Now.ToString();
                gameovers.Insert(gameover);
            }
        }

        public int GetGameOverCount()
        {
            List<GameOverCount> list = new List<GameOverCount>();
            using (var db = new LiteDatabase(@"NoSqlDB.db"))
            {
                var gameovers = db.GetCollection<GameOverCount>("gameovers");
                foreach (var item in gameovers.FindAll())
                {
                    list.Add(item);
                }
            }
            ViewBag.GameOverCount = list.Count;
            return ViewBag.GameOverCount;
        }

        public void AddScore(double second,string gameKeyId)
        {
            using (var db = new LiteDatabase(@"NoSqlDB.db"))
            {
                var scores = db.GetCollection<Score>("scores");
                Score scr = new Score();
                scr.second = second;
                scr.scoreKey = gameKeyId;
                scr.scoreDate = DateTime.Now.ToString();
                scr.scoreMonth = DateTime.Now.Month;
                scr.scoreYear = DateTime.Now.Year;
                scores.Insert(scr);
            }
        }

        public List<Score> GetScoreList()
        {
           
            List<Score> list = new List<Score>();
            using (var db = new LiteDatabase(@"NoSqlDB.db"))
            {
                var scores = db.GetCollection<Score>("scores");
                foreach (var item in scores.FindAll())
                {
                    list.Add(item);
                }
            }
            ViewBag.Count = list.Count;
            var hitList = list.OrderBy(c => c.second).Take(5).ToList();
            return hitList;
        }

        public void AddVisitor()
        {
            using (var db = new LiteDatabase(@"NoSqlDB.db"))
            {
                var visitors = db.GetCollection<PageHitCount>("visitors");
                PageHitCount visitor = new PageHitCount();
                visitor.Visitor = 1;
                visitor.VisitorDate = DateTime.Now.ToString();
                visitors.Insert(visitor);
            }

        }

        public int GetPageHitCount()
        {
            List<PageHitCount> list = new List<PageHitCount>();
            using (var db = new LiteDatabase(@"NoSqlDB.db"))
            {
                var visitors = db.GetCollection<PageHitCount>("visitors");
                foreach (var item in visitors.FindAll())
                {
                    list.Add(item);
                }
            }
            ViewBag.PageHitCount = list.Count;
            return ViewBag.PageHitCount;
        }

        private void DeleteAllDb()
        {
            using (var db = new LiteDatabase(@"NoSqlDB.db"))
            {
                var visitors = db.GetCollection("visitors").DeleteAll();
                var scores = db.GetCollection("scores").DeleteAll();
                var gameovers = db.GetCollection("gameovers").DeleteAll();

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}