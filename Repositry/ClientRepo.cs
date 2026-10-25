using FreelanceManager.Interfaces;
using FreelanceManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FreelanceManager.Repositry
{
    public class ClientRepo : IClientRepo
    {
        ITIContext context;
        public ClientRepo(ITIContext context)
        {
            this.context = context;
        }

        public IEnumerable<Client> GetAll()
        {
            IEnumerable<Client> clientsList = context.clients.ToList();
            return clientsList;
        }

        public IEnumerable<Client> GetAllWithProjects()
        {
            //return context.clients.Include(c => c.projects);
            return context.clients.Include(c => c.projects).ThenInclude(p => p.Missions).ToList();
        }
        public Client GetById(int id)
        {
            return context.clients.FirstOrDefault(c => c.Id == id);
        }
        public void Add(Client clientFromReq)
        {
            context.Add(clientFromReq);
        }
        public void RemoveById(int id)
        {
            context.clients.Remove(GetById(id));
        }
        public void Update(int id, Client clientFromReq)
        {
            Client clientFromDb = GetById(clientFromReq.Id);

            if (clientFromDb != null)
            {
                context.clients.Update(clientFromReq);
            }

            //clientFromDb.Name = clientFromReq.Name;
            //clientFromDb.Phone = clientFromReq.Phone;
            //clientFromDb.Email = clientFromReq.Email;
            //clientFromDb.ContactPerson = clientFromReq.ContactPerson;
            //clientFromDb.Address = clientFromReq.Address;
            //clientFromDb.Status = clientFromReq.Status;
        }
        public void Save()
        {
            context.SaveChanges();
        }


        public void RemoveByObj(Client obj)
        {
            throw new NotImplementedException();
        }

       


    }
}
