using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Contexts;
using Chirp.Infrastructure.Models;
using Chirp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers
{
    public class CheepController : Controller
    {
        private readonly ICheepRepository _cheepRepository;
        // // GET: Cheep
        // public ActionResult Index()
        // {
        //     return View();
        // }
        //
        // // GET: Cheep/Details/5
        // public ActionResult Details(int id)
        // {
        //     return View();
        // }
        //
        // // GET: Cheep/Create
        // public ActionResult Create()
        // {
        //     return View();
        // }

        // POST: Cheep/Create
        [HttpPost]
        [Route("Cheep/Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // if (User.Identity != null && User.Identity.IsAuthenticated)
                // {
                //     _service.AddCheep(new AddCheepDto
                //     {
                //         AuthorEmail = User.GetUserEmail(),
                //         AuthorName = User.GetUserFullName(),
                //         Text = cheepText
                //     });
                // }
                
                // TODO: Add insert logic here
                
                return Accepted();
            }
            catch
            {
                return BadRequest();
            }
        }

        // // GET: Cheep/Edit/5
        // public ActionResult Edit(int id)
        // {
        //     return View();
        // }
        //
        // // POST: Cheep/Edit/5
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public ActionResult Edit(int id, IFormCollection collection)
        // {
        //     try
        //     {
        //         // TODO: Add update logic here
        //
        //         return RedirectToAction(nameof(Index));
        //     }
        //     catch
        //     {
        //         return View();
        //     }
        // }
        //
        // // GET: Cheep/Delete/5
        // public ActionResult Delete(int id)
        // {
        //     return View();
        // }
        //
        // POST: Cheep/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult DeleteCheep(Guid id)
        {
            _cheepRepository.DeleteCheep(id);
            return RedirectToAction(); // Replace "Index" with the name of your view that shows the list of cheeps.
        }
    }
}