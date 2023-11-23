using BookLoansPorject.Models;
using BookLoansPorject.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace BookLoansPorject.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly BookDbContext db = new BookDbContext();
        // GET: Books
        [AllowAnonymous]
        public async Task<ActionResult> Index(int pg = 1)
        {
            var data = await db.Books.OrderBy(a => a.BookId).ToPagedListAsync(pg, 5);
            return View(data);
        }
        public ActionResult Create()
        {

            BookViewModel b = new BookViewModel();
            b.BookLoans.Add(new BookLoan { });
            return View(b);
        }
        [HttpPost]
        public ActionResult Create(BookViewModel data, string act = "")
        {
            if (act == "add")
            {
                data.BookLoans.Add(new BookLoan { });

                foreach (var item in ModelState.Values)
                {
                    item.Errors.Clear();
                }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                data.BookLoans.RemoveAt(index);
                foreach (var item in ModelState.Values)
                {
                    item.Errors.Clear();
                }
            }
            if (act == "insert")
            {
                if (ModelState.IsValid)
                {
                    var b = new Book
                    {
                        Title = data.Title,
                        Author = data.Author,
                        Published= data.Published,
                        Description= data.Description,
                        Genre = data.Genre,
                        IsAvailable = data.IsAvailable,
                    };
                    string ext = Path.GetExtension(data.BookCover.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                    string savePath = Server.MapPath("~/Pictures/") + fileName;
                    data.BookCover.SaveAs(savePath);
                    b.BookCover = fileName;
                    foreach (var l in data.BookLoans)
                    {

                        b.BookLoans.Add(l);
                    }
                    db.Books.Add(b);
                    db.SaveChanges();
                }
            }
            ViewBag.Act = act;
            return PartialView("_CreatePartial", data);
        }
        public ActionResult Edit(int id)
        {
            var c = db.Books
                .Select(x => new BookEditModel
                {
                    BookId = x.BookId,
                    Title = x.Title,
                    Author = x.Author,
                    Published = x.Published,
                    Description = x.Description,
                    Genre = x.Genre,
                    IsAvailable = x.IsAvailable,
                    BookLoans = x.BookLoans.ToList()

                })
                  .FirstOrDefault(x => x.BookId == id);
            ViewData["CurrentPic"] = db.Books.First(x => x.BookId == id).BookCover;
            return View(c);

        }
        [HttpPost]
        public ActionResult Edit(BookEditModel data, string act = "")
        {
            if (act == "add")
            {
                data.BookLoans.Add(new BookLoan { });
            }

            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                data.BookLoans.RemoveAt(index);
            }

            if (act == "update")
            {
                if (ModelState.IsValid)
                {
                    var b = db.Books.First(x => x.BookId == data.BookId);
                    b.Title = data.Title;
                    b.Author = data.Author;
                    b.Published = data.Published;
                    b.Description = data.Description;
                    b.Genre = data.Genre;
                    b.IsAvailable = data.IsAvailable;

                    if (data.BookCover != null)
                    {
                        string ext = Path.GetExtension(data.BookCover.FileName);
                        string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = Server.MapPath("~/Pictures/") + fileName;
                        data.BookCover.SaveAs(savePath);
                        b.BookCover = fileName;
                    }
                    db.BookLoans.RemoveRange(db.BookLoans.Where(x => x.BookId == data.BookId).ToList());
                    foreach (var item in data.BookLoans)
                    {
                        b.BookLoans.Add(new BookLoan
                        {
                            BorrowerName = item.BorrowerName,
                            Address = item.Address,
                            Phone = item.Phone,
                            LoanDate = item.LoanDate,
                            ReturnDate = item.ReturnDate,

                        });
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            ViewData["CurrentPic"] = db.Books.First(x => x.BookId == data.BookId).BookCover;
            return View(data);

        }
        public ActionResult Delete(int id)
        {
            var book = new Book { BookId = id };
            db.Entry(book).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            return Json(new { success = true, deleted = id });
        }
    }
}