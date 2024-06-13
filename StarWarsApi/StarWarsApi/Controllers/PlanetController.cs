using Microsoft.AspNetCore.Mvc;
using StarWarsApi.Models;
using StarWarsApi.Service;

namespace StarWarsApi.Controllers
{

    public class PlanetController : Controller
    {
        private readonly IStarWarsService _starWarsService;

        public PlanetController(IStarWarsService starWarsService)
        {
            _starWarsService = starWarsService;
        }


        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 5)
        {
            IEnumerable<Planet> planets;

            //    Ако терминът за търсене не е празен и е число, извличаме планета по ID.
            // Ако намерим планета, създаваме списък с тази единствена планета.
            //Ако не намерим планета, създаваме празен списък.

            if (!string.IsNullOrEmpty(search) && int.TryParse(search, out int id))
            {
                var planet = await _starWarsService.GetPlanetByIdAsync(id);
                planets = planet != null ? new List<Planet> { planet } : new List<Planet>();
            }
            else
            {
                //Ако терминът за търсене е празен или не е число:
                //Извличаме всички планети.
                //Ако терминът за търсене не е празен, филтрираме планетите по име(без значение за главни/ малки букви).
                planets = await _starWarsService.GetAllPlanetsAsync();

                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    planets = planets.Where(p => p.Name.ToLower().Contains(search));
                }
            }


            ViewData["SearchTerm"] = search; //    Съхраняваме термина за търсене в ViewData, за да можем да го използваме в изгледа.

            var totalItems = planets.Count();
            var totalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize); // Изчисляваме общия брой планети(totalItems) и броя на страниците(totalPages).
            planets = planets.Skip((page - 1) * pageSize).Take(pageSize); // Разделяме планетите на страници, като пропускаме планетите до текущата страница и взимаме само определен брой планети(pageSize).

            //Съхраняваме текущата страница и общия брой страници в ViewData, за да ги използваме в изгледа.
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            return View(planets);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string search, int page = 1, int pageSize = 5)// приема три параметъра: search (текст за търсене), page (номер на страница, по подразбиране 1) и pageSize (размер на страница, по подразбиране 5).
        {
            IEnumerable<Planet> planets;

            //Проверка дали search е валиден ID:
            if (!string.IsNullOrEmpty(search) && int.TryParse(search, out int id))
            {
                //  Ако search не е null или празен и може да бъде конвертиран към int(int.TryParse), се опитва да намери планета по ID.
                var planet = await _starWarsService.GetPlanetByIdAsync(id);
                //Ако намери планета, я добавя в списък, ако не, създава празен списък.
                planets = planet != null ? new List<Planet> { planet } : new List<Planet>();
            }
            else
            {
                //Ако search не е валидно ID, зарежда всички планети.
                planets = await _starWarsService.GetAllPlanetsAsync();

                if (!string.IsNullOrEmpty(search))
                {
                    //Ако search не е null или празен, филтрира планетите по име, съдържащо текста за търсене(с малки букви).
                    search = search.ToLower();
                    planets = planets.Where(p => p.Name.ToLower().Contains(search));
                }
            }
            //    Изчислява общия брой планети.
            //Изчислява общия брой страници, като дели броя на планетите на размера на страницата и закръгля нагоре.
            //Извлича необходимата страница, като прескача предишните страници и взема само текущата.
            var totalItems = planets.Count();
            var totalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);
            planets = planets.Skip((page - 1) * pageSize).Take(pageSize);


            //Връща JSON обект, съдържащ планетите, общия брой страници и текущата страница.
            return Json(new
            {
                planets,
                totalPages,
                currentPage = page
            });
        }

        public async Task<IActionResult> Details(string name)
        {
            //Асинхронно извлича всички планети чрез _starWarsService.
            var planets = await _starWarsService.GetAllPlanetsAsync();
            //Използва FirstOrDefault за намиране на първата планета(или null, ако няма такава), чието име съвпада с параметъра name.
            var planet = planets.FirstOrDefault(p => p.Name == name);
            if (planet == null) //Ако не е намерена планета с даденото име (planet е null), методът връща NotFound() (HTTP статус код 404), което означава, че ресурсът не е намерен.
            {
                return NotFound();
            }
            return View(planet);//Ако планетата е намерена, методът връща изглед (View) с предадения като модел обект planet.
        }
    }
}
