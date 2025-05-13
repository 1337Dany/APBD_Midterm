using APBD_MidtermTest.dtos;
using APBD_MidtermTest.models;

namespace APBD_MidtermTest.repositories;

public interface IProjectRepository
{
    dynamic GetTeamMemberById(int id);
    List<dynamic> GetTasksOfTeamMemberById(int teamMemberId, bool isAssigned);
    System.Threading.Tasks.Task AddTask(TaskDto dto);
}