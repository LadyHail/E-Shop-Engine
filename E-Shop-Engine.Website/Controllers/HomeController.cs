﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using E_Shop_Engine.Domain.DomainModel;
using E_Shop_Engine.Domain.Interfaces;
using E_Shop_Engine.Website.Models;
using NLog;
using X.PagedList;

namespace E_Shop_Engine.Website.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMailingRepository _mailingRepository;

        public HomeController(IRepository<Category> categoryRepository, IProductRepository productRepository, IMailingRepository mailingRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _mailingRepository = mailingRepository;
            logger = LogManager.GetCurrentClassLogger();
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
            _mailingRepository.CustomMail(model.Email, model.Name, model.Message);
            }
            catch
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return PartialView(model);
            }

            NotifySetup("notification-success", "Success!", "Message sent!");
            return Json(new { url = Url.Action("Index") });
        }

        // GET: Categories - for navbar
        public PartialViewResult NavList()
        {
            IQueryable<Category> model = _categoryRepository.GetAll();
            IEnumerable<CategoryViewModel> viewModel = Mapper.Map<IQueryable<Category>, IEnumerable<CategoryViewModel>>(model);
            return PartialView("_Categories", viewModel);
        }

        [HttpGet]
        public PartialViewResult GetSpecialOffers()
        {
            IQueryable<Product> model = _productRepository.GetAllSpecialOffers();
            IEnumerable<ProductViewModel> viewModel = Mapper.Map<IQueryable<Product>, IEnumerable<ProductViewModel>>(model);
            return PartialView("SpecialOffers", viewModel);
        }

        [HttpGet]
        public PartialViewResult GetSpecialOffersInDeck(int? page)
        {
            int pageNumber = page ?? 1;
            IQueryable<Product> model = _productRepository.GetAllShowingInDeck();
            IPagedList<Product> pagedModel = model.OrderBy(x => x.Edited).ToPagedList(pageNumber, 25);
            IEnumerable<ProductViewModel> mappedModel = Mapper.Map<IEnumerable<ProductViewModel>>(pagedModel);
            IPagedList<ProductViewModel> viewModel = new StaticPagedList<ProductViewModel>(mappedModel, pagedModel.GetMetaData());
            return PartialView("_ProductsDeck", viewModel);
        }

        private void NotifySetup(string type, string title, string text)
        {
            TempData["notifyType"] = type;
            TempData["notifyTitle"] = title;
            TempData["notifyText"] = text;
        }
    }
}