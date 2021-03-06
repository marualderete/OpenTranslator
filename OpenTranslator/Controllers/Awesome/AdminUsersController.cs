﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenTranslator.Data;
using OpenTranslator.Repostitory;
using OpenTranslator.Models.Input;
using Omu.AwesomeMvc;

namespace OpenTranslator.Controllers.Awesome
{
    public class AdminUsersController : Controller
    {
        	//public StringTranslationDBEntities entities =new StringTranslationDBEntities();
		private IUsers IUsers;
		public AdminUsersController()
		{
			this.IUsers = new UsersRepository(new StringTranslationEntities());
		}
		// GET: User
	public ActionResult Index()
		{
			if (Request.Cookies["UserId"] == null)
			{
				return RedirectToAction("Index", "User");
			}
			else
			{
				return View("Index","_AdminLayout");
			}

		}
		public ActionResult embeded()
		{
			return View("Index","_LayoutEmbedAdmin");
		}

		public ActionResult UserDataItems(GridParams g)
		{
			var items = IUsers.GetUsers().AsQueryable();
			var key = Convert.ToInt32(g.Key);
			var model = new GridModelBuilder<UserMaster>(items, g)
			{
				Key = "Id",
				GetItem = () => items.Single(x => x.Id == key)
			}.Build();
			return Json(model);

		}
		public ActionResult Create()
		{
			return PartialView();
		}

		[HttpPost]
		public ActionResult Create(UserInput input)
		{
			if (!ModelState.IsValid)
			{
				return PartialView(input);
			}
			if (this.doesUserEmailExist(input) == true)
			{
				return PartialView(input);

			}
			else
			{

				try
				{
					UserMaster user = new UserMaster();
					user.EmailId = input.EmailId;
					user.Password = input.Password;
					IUsers.InsertUser(user);
					return Json(user);
				}
				catch (Exception ex)
				{
					var a = ex;
					return Json("");
				}

			}
			// returning the key to call grid.api.update
			// return Json(entities.Open_Translator_SP().FirstOrDefault(x => x.TextId == input.TextId));


		}
		[HttpPost]
		public bool doesUserEmailExist(UserInput input)
		{
			var id = Convert.ToDecimal(input.Id);
			if (input.Id == null)
			{
				var user = IUsers.GetUsers().Where(x => x.EmailId == input.EmailId).FirstOrDefault();
				if (user != null)
				{
					ViewBag.errormsg = "Email already exist.";
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				var user = IUsers.GetUsers().Where(x => x.EmailId == input.EmailId && x.Id != id).FirstOrDefault();
				if (user != null)
				{
					ViewBag.errormsg = "Email already exist.";
					return true;
				}
				else
				{
					return false;
				}
			}



		}

		public ActionResult Edit(int id)
		{

			var user = IUsers.GetUserID(id);
			return PartialView(
				"Create",
				new UserInput
				{

					Id = user.Id.ToString(),
					EmailId = user.EmailId,
					Password = user.Password
				});
		}

		[HttpPost]
		public ActionResult Edit(UserInput input)
		{
			if (!ModelState.IsValid)
			{
				return PartialView("Create", input);
			}
			if (this.doesUserEmailExist(input) == true)
			{
				return PartialView("Create", input);
			}
			else
			{
				//var user = new UserMaster();
				var id = Convert.ToInt32(input.Id);
				var user = IUsers.GetUserID(id);
				user.EmailId = input.EmailId;
				user.Password = input.Password;
				IUsers.UpdateUser(user);
				return Json(new { input.Id });
			}

		}
		public ActionResult Delete(string id, string gridId)
		{

			return PartialView(new DeleteConfirmInput
			{
				TextId = id,
				Message = string.Format("Are you sure you want to delete")
			});
		}

		[HttpPost]
		public ActionResult Delete(DeleteConfirmInput input)
		{
			try
			{
				IUsers.DeleteUser(input.Id);
			}
			catch (Exception ex)
			{

			}
			return Json(new { Id = input.TextId });
		}
    }
}
