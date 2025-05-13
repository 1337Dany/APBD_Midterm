using APBD_MidtermTest.dtos;
using Task = APBD_MidtermTest.models.Task;

namespace APBD_MidtermTest.Services;

public interface IDbService
{
    Task<TasksOfTeamMemberDto> GetTasksOfTeamMemberAsync(int id);
    System.Threading.Tasks.Task AddTask(TaskDto dto);
}