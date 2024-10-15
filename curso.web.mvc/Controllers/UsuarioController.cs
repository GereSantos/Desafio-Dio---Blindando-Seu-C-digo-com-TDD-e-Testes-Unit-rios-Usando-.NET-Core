using curso.web.mvc.Models.Usuarios;
using curso.web.mvc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace curso.web.mvc.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(RegistrarUsuarioViewModelInput registrarUsuarioViewModelInput)
        {
            try
            {
                var usuario = await _usuarioService.Registrar(registrarUsuarioViewModelInput);
                AddSuccessMessage($"Os dados foram cadastrados com sucesso para o login {usuario.Login}");
            }
            catch (ApiException ex)
            {
                AddErrorMessage(ex.Message);
            }
            catch (Exception ex)
            {
                AddErrorMessage(ex.Message);
            }

            return View();
        }

        public IActionResult Logar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logar(LoginViewModelInput loginViewModelInput)
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                var usuario = await _usuarioService.Logar(loginViewModelInput);
                await SignInUserAsync(usuario);

                AddSuccessMessage($"O usuário está autenticado {usuario.Token}");
            }
            catch (ApiException ex)
            {
                AddErrorMessage(ex.Message);
            }
            catch (Exception ex)
            {
                AddErrorMessage(ex.Message);
            }

            return View();
        }

        public IActionResult EfetuarLogoff()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logoff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Logar));
        }

        // Método para adicionar mensagens de erro ao ModelState
        private void AddErrorMessage(string message)
        {
            ModelState.AddModelError(string.Empty, message);
        }

        // Método para adicionar mensagens de sucesso ao ModelState
        private void AddSuccessMessage(string message)
        {
            ModelState.AddModelError(string.Empty, message);
        }

        // Método auxiliar para efetuar o login do usuário
        private async Task SignInUserAsync(UsuarioViewModelOutput usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Usuario.Codigo.ToString()),
                new Claim(ClaimTypes.Name, usuario.Usuario.Login),
                new Claim(ClaimTypes.Email, usuario.Usuario.Email),
                new Claim("token", usuario.Token),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }
}
