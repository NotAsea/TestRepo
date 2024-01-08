namespace TestRepo.Service.Services.Interfaces;

public interface IPersonService
{
    Task<ListReturn> GetPeople(
        int index = 1,
        int size = 1000,
        string sortBy = "Id",
        string sortType = "asc",
        string nameSearch = ""
    );

    Task<PersonModel> GetPerson(int id);
    Task<int> SavePerson(PersonModel model);
    Task DeletePeople(int[] peopleId, bool isForce);
    Task DeletePerson(int id, bool isForce);
    Task ActivatePerson(int id);
    Task ActivatePeople(int[] id);
}
