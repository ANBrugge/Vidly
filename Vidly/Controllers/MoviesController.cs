using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vidly.Models;
using Vidly.ViewModels;


namespace Vidly.Controllers
{
    public class MoviesController : Controller
    {
        private ApplicationDbContext _context;

        public MoviesController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
       
        // hardcoded movie list
        //public IEnumerable<Movie> GetMovies()
        //{
        //    return new List<Movie>
        //    {
        //        new Movie { Name = "IT", Id = 0},
        //        new Movie { Name = "Gran Torino", Id = 1},
        //        new Movie { Name = "Batman Begins", Id = 2},
        //        new Movie { Name = "Monthy Python: The Life of Bryan", Id = 3},
        //        new Movie { Name = "Bladerunner", Id = 4},
        //    };
        //}

        // GET: Movies
        public ActionResult Index()
        {
            var movie = _context.Movies.Include(m => m.Genre).ToList();

            return View(movie);

            //if (!pageIndex.HasValue)
            //    pageIndex = 1;

            //if (string.IsNullOrWhiteSpace(sortBy))
            //    sortBy = "Name";

            //return Content(string.Format("pageIndex={0}&sortBy={1}", pageIndex, sortBy));
            // nu kan het alleen via index?pageIndex=1&sortBy=Name, dus je moet een custom route maken om deze parameters in de URL te kunnen bakken
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Movie movie) 
        {
            if(!ModelState.IsValid)
            {
                var viewModel = new MovieFormViewModel(movie)
                {
                    Genres = _context.Genres.ToList()
                };
                                
                return View("MovieForm", viewModel);
            }

            if (movie.Id == 0) 
            {
                movie.DateAdded = DateTime.Now;
                _context.Movies.Add(movie);
            }
            else
            {
                var movieInDb = _context.Movies.Single(m => m.Id == movie.Id);

                movieInDb.Name = movie.Name;
                movieInDb.ReleaseDate = movie.ReleaseDate;
                movieInDb.GenreId = movie.GenreId;
                movieInDb.NumberInStock = movie.NumberInStock;
            }

            _context.Genres.Single(m => m.Id == movie.GenreId);
                _context.SaveChanges();


            return RedirectToAction("Index", "Movies");
        }

        public ViewResult New() 
        {
            var genres = _context.Genres.ToList();

            var viewModel = new MovieFormViewModel() // Als je hier een movie doorgeeft, worden default values van Release Date en number in stock zichtbaar.
                                                     // Id moet wel een waarde 0 krijgen, dus die waarde geef je door aan MovieFormViewModel default ctor.
                                                                
            {
                Genres = genres
            };
            
            return View("MovieForm", viewModel); // action hoeft geen gelijknamige naam te hebben als view: meerdere actions kunnen wijzen naar een view
        }


        public ActionResult Edit(int id)
        {
            var movie = _context.Movies.SingleOrDefault(m => m.Id == id);
            
            if (movie == null)
                    return HttpNotFound();

            var viewModel = new MovieFormViewModel(movie) // huidige data van movie kunnen zien. movie props worden gemapt naar viewModel gelijknamige props
            {
                //Movie = movie,
                Genres = _context.Genres.ToList()
            };
                
            return View("MovieForm", viewModel);
        }






        public ActionResult Details(int id)
        {
            var movie = _context.Movies.Include(m => m.Genre).SingleOrDefault(m => m.Id == id);
            //var movie = GetMovies().SingleOrDefault(m => m.Id == id);

            return View(movie);
        }

        public ActionResult Random()
        {
            var movie = new Movie() { Name = "Shrek" };

            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Sander"},
                new Customer { Id = 2, Name = "Ruth"},
                new Customer { Id = 3, Name = "Judith"},
                new Customer { Id = 4, Name = "Alex"},
                new Customer { Id = 5, Name = "Ellen"},
            };

            var viewModel = new RandomMovieViewModel
            {
                Movie = movie,
                Customers = customers
            };

            return View(viewModel);
        }


        [Route("movies/released/{year:regex(\\d{4})}/{month:range(1,12)}")]
        public ActionResult ByReleaseYear(int year, byte month)
        {
            return Content(year + "/" + month);
        }

    }
}