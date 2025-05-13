using APBD_MidtermTest.models;
using Task = APBD_MidtermTest.models.Task;

namespace APBD_MidtermTest.dtos;

public class TasksOfTeamMemberDto
{
    public TeamMember TeamMember { get; set; }
    public List<dynamic> Task { get; set; }
    public List<dynamic> TaskOfTest { get; set; }
}