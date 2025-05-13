using APBD_MidtermTest.models;
using APBD_MidtermTest.dtos;
using APBD_MidtermTest.repositories;
using Task = APBD_MidtermTest.models.Task;

namespace APBD_MidtermTest.Services;

public class DbService(IProjectRepository repo) : IDbService
{
    public async Task<TasksOfTeamMemberDto> GetTasksOfTeamMemberAsync(int id)
    {
        var teamMember = repo.GetTeamMemberById(id);
        var assignedTasks = repo.GetTasksOfTeamMemberById(id, isAssigned: true);
        var createdTasks = repo.GetTasksOfTeamMemberById(id, isAssigned: false);

        var result = new TasksOfTeamMemberDto
        {
            TeamMember = teamMember,
            Task = assignedTasks,
            TaskOfTest = createdTasks
        };

        return await System.Threading.Tasks.Task.FromResult(result);
    }

    public async System.Threading.Tasks.Task AddTask(TaskDto dto)
    {
        await repo.AddTask(dto);
    }
}