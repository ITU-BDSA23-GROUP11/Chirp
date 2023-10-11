﻿using Chirp.DBService.Models;
using Chirp.DBService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.WebService.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _service;
    public List<Cheep> Cheeps { get; set; }

    public int AmountOfPages { get; set; }

    public PublicModel(ICheepRepository service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {
        Cheeps = _service.GetCheeps();
        
        //Calculate the amount of pages needed
        AmountOfPages = (int)Math.Ceiling((double)Cheeps.Count() / 32);
        return Page();
    }
}