using FreelanceManager.Interfaces;
using Microsoft.EntityFrameworkCore;
using FreelanceManager.Repositry;
using FreelanceManager.Models;

namespace FreelanceManager.Repositry
{
    public class ProjectRepo:IProjectRepo
    {
        ITIContext context;

        public ProjectRepo(ITIContext context) 
        {
            this.context = context;
        }

		public void Add(Project obj)
		{
	
			context.projects.Add(obj);
		}

		public IEnumerable<Project> GetAll()
		{
			
			return context.projects.Include(p=>p.Missions);
		}

		public Project GetById(int Id)
		{
            Project project = context.projects.Include(p => p.Missions).Include(p=>p.Client).FirstOrDefault(p=>p.Id==Id);
            
			return project;
		}

        public void Remove(int Id)
        {
            Project project = context.projects.FirstOrDefault(p => p.Id == Id);
			context.projects.Remove(project);
        }

        public void RemoveById(int id)
        {
			Project project = context.projects.FirstOrDefault(p => p.Id == id);
			context.projects.Remove(project);
		}

        public void RemoveByObj(Project obj)
        {
            throw new NotImplementedException();
        }

        public void Save()
		{
			context.SaveChanges();
		}

		public void Update(int Id, Project obj)
		{
			Project project  = context.projects.FirstOrDefault(p=>p.Id==Id);
           

            if (project is not null)
			{
                context.projects.Update(obj);
            }
        }
	}
}
