﻿using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using EntityLayer.Dto;
using MvcProjeKampi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MvcProjeKampi.Controllers
{
    public class LoginController : Controller
    {
        private IAuthService _authService = new AuthManager(new AdminManager(new EfAdminDal()), new WriterManager(new EfWriterDal()));

        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(LoginDto loginDto)
        {
            if (_authService.Login(loginDto))
            {
                FormsAuthentication.SetAuthCookie(loginDto.AdminUserName, false);
                Session["AdminUserName"] = loginDto.AdminUserName;
                return RedirectToAction("Index", "AdminCategory");
            }
            else
            {
                ViewData["ErrorMessage"] = "Kullanıcı adı veya Parola yanlış";
                return View();
            }
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }

        public ActionResult WriterLogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("AllHeading", "WriterPanel");
        }

        [HttpGet]
        public ActionResult WriterLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult WriterLogin(WriterLoginDto writerLoginDto)
        {
            var response = Request["g-recaptcha-response"];
            const string secret = "6LfbKk8bAAAAANkMjzLC_iAGX45a_J8RUWe1XYeQ";
            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResult>(reply);

            if(_authService.WriterLogin(writerLoginDto) && captchaResponse.Success)
            {
                FormsAuthentication.SetAuthCookie(writerLoginDto.WriterMail, false);
                Session["WriterMail"] = writerLoginDto.WriterMail;
                return RedirectToAction("MyContent", "WriterPanelContent");
            }
            else
            {
                ViewData["ErrorMessage"] = "Kullanıcı adı veya Parola yanlış";
                return RedirectToAction("WriterLogin");
            }

        }
    }
}