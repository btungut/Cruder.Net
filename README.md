CRUDer Framework
===================
Cruder is the usefull, generic and authorization supported Asp.Net MVC framework which is allows you to create CRUD based applications quickly.

Dependent Another Frameworks
-------------

 - ~~Asp.Net MVC < 6~~ (Now supports any .Net Application)
 - Castle Windsor (I'm not planning to change :smile:)
 - Entity Framework (I'm planning to add more ORM supports, Really!)

>I promise you will learn and enjoy to use Cruder Framework in following 3 minutes. Just keep going to read.

I prepared 2 different examples to use Cruder Framework.

Purpose of the first example is **how you implement and use Cruder quickly**. I believe, that shows Cruder features to you directly. I named it **The Quick Implementation**.

The second example purpose is how I implement and **how Cruder could be used in complex, N-Tier architecture soultions**. I named it **The Perfect Implementation** (Arguable to how perfect is :smile:)

*I prepared both of 2 examples for newly created solutions.*


##The Quick Implementation

Create a class library and name it **"Demo.Data"**.This layer should be include our data-access classes. Like repositories and entities.
###Demo.Data Layer
 - Install Entity Framework latest version from NuGet.
 - Install **Cruder.Core** and **Cruder.Data** from NuGet (or add reference directly from source for debugging)
 - Create **CategoryEntity**, **CategoryRepository** and **DatabaseContext** classes.

![enter image description here](https://lh3.googleusercontent.com/-2R-w5VlohdA/VfV4y3Gwi6I/AAAAAAAABlo/IGiGdjxPWHw/s0/Demo.Data.PNG "Demo.Data.PNG")
####Category Entity
    using Cruder.Core.Contract;
    
    namespace Demo.Data.Entities
    {
        public class CategoryEntity : IEntity<int>
        {
            public int Id { get; set; }
    
            public string Name { get; set; }
        }
    }

**CategoryEntity** is the our first and single Entity class (in this example) which is mapping **Categories** table.
In Cruder Framework we want to that every entity class must be implemented from **IEntity** or **IEntity<>** classes.
You can find detailed description about IEntity implementation at The Perfect Implementation example.
####Category Repository
    using Demo.Data.Entities;
    
    namespace Demo.Data.Repositories
    {
        public class CategoryRepository : Cruder.Data.EntityRepository<CategoryEntity,int>
        {
        }
    }

Here is the hearth of the Cruder Framework **EntityRepository<>** !
It is responsible every data accessing actions. It resolves Context classes, finds DbSet at realtime and manages creating, reading, updating and removing actions.

You can look over the virtual methods in EntityRepostory<> in this way you can **interrupt** and **customize** CRUD actions.
####Database Context (EF Context)

    using Demo.Data.Entities;
    using System.Data.Entity;
    
    namespace Demo.Data.Model
    {
        public class DatabaseContext : DbContext
        {
            public DbSet<CategoryEntity> Categories { get; set; }
    
            public DatabaseContext() : base("Cruder.Demo")
            {
            }
    
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Entity<CategoryEntity>()
                    .HasKey(x => x.Id)
                    .ToTable("Categories");
    
                base.OnModelCreating(modelBuilder);
            }
        }
    }

This is traditional Entity Framework context class implementation. This implementation does not depend any Cruder object, you can design how you want.

###Demo.UI Layer
Create a empty Asp.Net MVC project and;

 - Add Demo.Data as reference
 - Install **Cruder.Core** and **Cruder.Web** from NuGet.

####Configure **Web.Config** ;
![enter image description here](https://lh3.googleusercontent.com/-t5yyHvJ92Ic/Vfch16Jvg6I/AAAAAAAABm4/g3biR0rZWq0/s0/Web.Config.png "Web.Config.png")

####Configure **Views/Web.config** ; 
![enter image description here](https://lh3.googleusercontent.com/-5OufWPbc9Ng/VfWEFRUd_LI/AAAAAAAABl8/hKmoo5g9lss/s0/Demo.UI.Views.Web.Config.png "Demo.UI.Views.Web.Config.png")

####Configure **Global.asax** ;
![enter image description here](https://lh3.googleusercontent.com/-7F_li4jgvZU/VfWFBeRk4YI/AAAAAAAABmI/M8ONMgGIhvA/s0/Demo.UI.Global.Asax.png "Demo.UI.Global.Asax.png")

####Add **Category Controller** and **Views**;
![enter image description here](https://lh3.googleusercontent.com/-Ni2Dkl3wC-I/VfWFgvC4a_I/AAAAAAAABmc/CpcBN9AL-TA/s0/Demo.UI.Classes.png "Demo.UI.Classes.png")

####CategoryController

    using Demo.Data.Entities;
    
    namespace Demo.UI.Controllers
    {
        public class CategoryController : Cruder.Web.Mvc.Controllers.CruderWebController<CategoryEntity>
        {
        }
    }

####Views/Category/Index.cshtml

    @model Cruder.Web.ViewModel.ListViewModel<Demo.Data.Entities.CategoryEntity>
    
    <table width="600px">
        <thead>
            <tr>
                <td>Id</td>
                <td>Name</td>
                <td>Actions</td>
            </tr>
        </thead>
        <tbody>
            @foreach (var item in Model.Data)
            {
                <tr>
                    <td>@item.Id</td>
                    <td>@item.Name</td>
                    <td>
                        <a href="@Url.Action("AddEdit", new { id = item.Id })">Edit</a>
                        <a href="@Url.Action("Delete", new { id = item.Id })">Delete</a>
                    </td>
                </tr>
            }
        </tbody>
    </table>

####Views/Category/AddEdit.cshtml

    @model Cruder.Web.ViewModel.DetailViewModel<Demo.Data.Entities.CategoryEntity>
    
    @if (CruderHtml.PageMessage != null)
    {
        <div>@CruderHtml.PageMessage.Type.ToString() : <br />@Html.Raw(CruderHtml.PageMessage.Content)</div>
    }
    
    <h2>@Model.Data.Name</h2>
    
    <form method="post" action="@Request.Url">
        @Html.TextBoxFor(x => x.Data.Name)
    
        <br />
        
        <input type="submit" value="Save"/>
    </form>

##The Perfect Implementation
Will be added as soon as.

