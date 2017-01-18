using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NetCoreWebPoc.Controllers
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? Birthday { get; set; }
        public string JsonVal { get; set; }

        //public Person()
        //{
        //    this.Name = "Mehmet Erdal";
        //    this.Surname = "Özkınacı";
        //    this.Birthday = DateTime.Now;
        //}
    }

    public class PostgreSqlContext : DbContext
    {
        public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options) : base(options)
        {
        }

        public DbSet<Person> Person { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    builder.Entity<Person>().HasKey(m => m.Id);

        //    base.OnModelCreating(builder);
        //}

        //public override int SaveChanges()
        //{
        //    ChangeTracker.DetectChanges();

        //    updateUpdatedProperty<Person>();

        //    return base.SaveChanges();
        //}

        //private void updateUpdatedProperty<T>() where T : class
        //{
        //    var modifiedSourceInfo =
        //        ChangeTracker.Entries<T>()
        //            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
        //}
    }

    [Route("api/[controller]")]
    public class PocController : Controller
    {
        private readonly PostgreSqlContext dbContext;

        public PocController(PostgreSqlContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Route("People")]
        public IEnumerable<Person> GetPeople()
        {
            //List<Person> people = new List<Person>();
            //people.Add(new Person());
            //people.Add(new Person());

            //return people;
            return dbContext.Person;
        }

        [HttpGet]
        [Route("People/{id:int}")]
        public IActionResult GetPerson(int id)
        {
            //return new Person();
            Person person = dbContext.Person.FirstOrDefault(p => p.Id == id);
            if (person != null)
                return Ok(person);

            return NotFound();
        }

        [HttpGet]
        [Route("People/JsonIncluded/{name}")]
        public IEnumerable<Person> JsonIncluded(string name)
        {
            return dbContext.Person.FromSql("SELECT * FROM public.\"Person\" WHERE \"JsonVal\" @> '{{\"name\":\"" + name + "\"}}'").ToList();
        }

        [HttpGet]
        [Route("People/BirthdayExist")]
        public IEnumerable<Person> BirthdayExist()
        {
            return dbContext.Person.FromSql("SELECT * FROM public.\"Person\" WHERE \"JsonVal\" ? 'birthday'").ToList();
        }

        [HttpGet]
        [Route("People/NameOrBirthdayExist")]
        public IEnumerable<Person> NameOrBirthdayExist()
        {
            return dbContext.Person.FromSql("SELECT * FROM public.\"Person\" WHERE \"JsonVal\" ?| array['name','birthday']").ToList();
        }

        [HttpGet]
        [Route("People/NameAndBirthdayExist")]
        public IEnumerable<Person> NameAndBirthdayExist()
        {
            return dbContext.Person.FromSql("SELECT * FROM public.\"Person\" WHERE \"JsonVal\" ?& array['name','birthday']").ToList();
        }

        [HttpGet]
        [Route("People/IdGreater/{id:int}")]
        public IEnumerable<Person> IdGreater(int id)
        {
            return dbContext.Person.FromSql("SELECT * FROM public.\"Person\" WHERE \"JsonVal\" ->> 'id' > '" + id.ToString() + "'").ToList();
        }

        [HttpGet]
        [Route("People/NameEqual/{name}")]
        public IEnumerable<Person> NameEqual(string name)
        {
            return dbContext.Person.FromSql("SELECT * FROM public.\"Person\" WHERE \"JsonVal\" ->> 'name' = '" + name + "'").ToList();
        }
    }
}