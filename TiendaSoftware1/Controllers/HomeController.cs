using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TiendaSoftware1.Models;

namespace TiendaSoftware1.Controllers
{
    public class HomeController : Controller
    {
        Bdg3Context db = new Bdg3Context();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var productos = db.Productos.ToList();
            return View(productos);
        }


        public IActionResult GestionUsuarios()
        {
            return View(db.Usuarios.ToList());
        }


        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(Usuario usuario)
        {
            // Verificar si el correo electr�nico ya existe
            if (db.Usuarios.Any(x => x.Email == usuario.Email))
            {
                ViewBag.Notification = "Esta cuenta ya existe";
                return View();
            }
            else
            {
                // Asignar el rol predeterminado (por ejemplo, RolId = 1 para 'Usuario')
                usuario.RolId = 1; // Aseg�rate de que este ID corresponde al rol de 'Usuario' en la tabla Roles

                db.Usuarios.Add(usuario);
                db.SaveChanges();

                // Guardar informaci�n de sesi�n
                HttpContext.Session.SetString("email", usuario.Email.ToString());
                HttpContext.Session.SetString("clave", usuario.Password.ToString());

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(Usuario usuario)
        {
            var checkLogin = db.Usuarios.Where(x => x.Email.Equals(usuario.Email)
            && x.Password.Equals(usuario.Password))
                .FirstOrDefault();

            if (checkLogin != null)
            {
                HttpContext.Session.SetString("email", usuario.Email.ToString());
                HttpContext.Session.SetString("clave", usuario.Password.ToString());
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Notification = "Email o clave incorrectas";
            }
            return View();
        }


        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}