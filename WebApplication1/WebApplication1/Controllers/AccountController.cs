﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication1.Models.Entity;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {  
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_v"></param>
        /// <param name="returnUrl">Url désiré avant la redirection sur page de log</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(ConnexionViewModel _v, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(_v);
            }
            else
            {
                User _u = UserData.GetLesUtilisateurs().FirstOrDefault(util => util.pseudo == _v.Identifiant && util.password == _v.Password || util.mail == _v.Identifiant);
                if (_u != null)
                {
                    FormsAuthentication.SetAuthCookie(_u.pseudo, false);
                    if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                        return Redirect(ReturnUrl);
                    else
                        return RedirectToAction("Index", controllerName: "Home");
                }
                else
                {
                    ViewBag.Error = "Il semble que votre compte soit inexistant";
                    return View(_v);
                }
            }    
        }
            
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", controllerName: "Home");
        }

       
        [Authorize]
        public ActionResult Profil()
        {
            string _pseudo = HttpContext.User.Identity.Name;
            User _u = UserData.GetLesUtilisateurs().FirstOrDefault(u => u.pseudo == _pseudo);
            if (_u == null)
                 return View("~/Views/Shared/Error.cshtml");
            else
            {
                UserViewModel _uViewModel = new UserViewModel()
                {
                    Nom = _u.nom,
                    Ville = _u.ville,
                    Adresse = _u.adresse,
                    CodePostale = _u.codePostale,
                    Mail = _u.mail,
                    Pseudo = _u.pseudo,
                    Prenom = _u.prenom

                };
                return View(_uViewModel);
            }
        }

        /// <summary>
        /// ne change pas les valeurs dans la liste
        /// </summary>
        /// <returns></returns>
        [Authorize]
 //       [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Profil(UserViewModel _userViewModel)
        {
            if(ModelState.IsValid)
            {
                User _u = UserData.GetLesUtilisateurs().Single(u => u.pseudo == _userViewModel.Pseudo);
                _u.prenom = _userViewModel.Prenom;
                _u.nom = _userViewModel.Nom;
                _u.ville = _userViewModel.Ville;
                _u.mail = _userViewModel.Mail;
                _u.password = _userViewModel.Password;
                _u.codePostale = _userViewModel.CodePostale;
                _u.adresse = _userViewModel.Adresse;
                ViewBag.Sucess = "Profil modifié avec succès !";

                return View(_userViewModel);
            }
            return View(_userViewModel); 
        }


        public ActionResult CreateAccount()
        {
            return View((new CreateViewModel()));
        }

        [HttpPost]
        public ActionResult CreateAccount(CreateViewModel _createViewModel)
        {
           
            var  _u =  (from util in UserData.GetLesUtilisateurs() where util.mail == _createViewModel.Email || util.pseudo == _createViewModel.Pseudo select util).ToList();
            if (_u.Count() > 0)
            {
                // voir affichage dans la vue
                ModelState.AddModelError("mailExist", "ce mail ou ce pseudo, ou il est déjà utilisé");
                return View(_createViewModel);
            }    
            else
            {
                UserData.GetLesUtilisateurs().Add(
                    new Models.Entity.User { mail = _createViewModel.Email, pseudo = _createViewModel.Pseudo, password = _createViewModel.Password });
                return RedirectToAction("index", "home");
            }
                
        }
    }
}