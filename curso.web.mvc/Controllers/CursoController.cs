using curso.web.mvc.Models.Cursos;
using curso.web.mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System;
using System.Threading.Tasks;

namespace curso.web.mvc.Controllers
{
    [Authorize]
    public class CursoController : Controller
    {
        private readonly ICursoService _cursoService;

        public CursoController(ICursoService cursoService)
        {
            _cursoService = cursoService;
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(CadastrarCursoViewModelInput cadastrarCursoViewModelInput)
        {
            if (!ModelState.IsValid)
            {
                return View(cadastrarCursoViewModelInput);
            }

            try
            {
                var curso = await _cursoService.Registrar(cadastrarCursoViewModelInput);
                AddSuccessMessage($"O curso foi cadastrado com sucesso: {curso.Nome}");
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

        public async Task<IActionResult> Listar()
        {
            var cursos = await _cursoService.Obter();
            return View(cursos);
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
    }
}
