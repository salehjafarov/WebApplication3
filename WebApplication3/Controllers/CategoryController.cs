﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication3.EntityModels;
using WebApplication3.Models;
using WebApplication3.ViewModels;
using WebApplication3.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace WebApplication3.Controllers
{
    public class CategoryController : BaseController
    {
        public CategoryController(UserManager<User> userManager) : base(userManager)
        {

        }
        [HttpGet]
        public IActionResult Index()
        {
            CategoryViewModel vm = new CategoryViewModel();
            vm.CurrentUser = CurrentUser;
            using (CateringContext cm = new CateringContext())
            {
                var categoriesdb = cm.CookCategories.Include(x => x.Creator);
                vm.Categories = categoriesdb.Where(x => x.IsDeleted == false)
                                                 .Select(x => Mapper<CookCategory,CategoryModel>.Map(x)).ToList();
            }
            
            
            return View(vm);
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            CategoryModel Categorymodel;
            using (CateringContext c= new CateringContext())
            {
                var category = c.CookCategories.Include(x => x.Creator).FirstOrDefault(x => x.Id == id);
                if (category == null)
                {
                    Categorymodel = new CategoryModel();
                }
                else
                {
                    Categorymodel = Mapper<CookCategory, CategoryModel>.Map(category);
                    Categorymodel.ButtonText = "Update";
                }
            }
            
            return View(Categorymodel);
        }

        [HttpPost]
        public IActionResult Save(CategoryModel Categorymodel)
        {
            if (!ModelState.IsValid)
            {
                return View(Categorymodel);
            }
            using (CateringContext c = new CateringContext())
            {
                CookCategory category = Mapper<CookCategory, CategoryModel>.Map(Categorymodel);
                category.IsDeleted = false;
                category.LastModifiedDate = DateTime.UtcNow;
                category.CreatorId = CurrentUser.Id;
                if (Categorymodel.Id == 0)
                {
                    try
                    {
                        c.Add(category);
                        c.SaveChanges();
                    }
                    catch (Exception)
                    {
                        return Content("Error");
                        
                    }
                    
                }
                else
                {
                    try
                    {
                        c.Update(category);
                        c.SaveChanges();
                    }
                    catch (Exception)
                    {

                        return Content("Error");
                    }
                    
                }
                
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (CateringContext c = new CateringContext())
            {
                var category =  c.CookCategories.FirstOrDefault(x => x.Id == id);
                category.IsDeleted = true;
                category.LastModifiedDate = DateTime.UtcNow;
                category.CreatorId = CurrentUser.Id;
                try
                {
                    c.SaveChanges();
                }
                catch (Exception)
                {

                    return Content("Error");
                }
                
            }
            return RedirectToAction("Index");
        }
    }
}
