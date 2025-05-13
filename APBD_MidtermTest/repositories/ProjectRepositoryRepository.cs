using System.Data.SqlClient;
using APBD_MidtermTest.dtos;
using APBD_MidtermTest.exceptions;
using APBD_MidtermTest.models;
using Microsoft.Data.SqlClient;

namespace APBD_MidtermTest.repositories;

public class ProjectRepositoryRepository(string connectionString) : IProjectRepository
{
    private readonly string _connectionString = connectionString;

    public dynamic GetTeamMemberById(int id)
    {
        var query = "SELECT * FROM TeamMember WHERE IdTeamMember = @Id";
        using (var command = new SqlCommand(query, new SqlConnection(_connectionString)))
        {
            command.Parameters.AddWithValue("@Id", id);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new
                    {
                        IdTeamMember = reader["IdTeamMember"],
                        FirstName = reader["FirstName"],
                        LastName = reader["LastName"],
                        Email = reader["Email"]
                        // Add other team member properties as needed
                    };
                }

                return null;
            }
        }
    }

    public List<dynamic> GetTasksOfTeamMemberById(int teamMemberId, bool isAssigned)
    {
        var columnToFilter = isAssigned ? "IdAssignedTo" : "IdCreator";
        var query = $@"
            SELECT 
                t.Name AS TaskName, 
                t.Description, 
                t.Deadline, 
                p.Name AS ProjectName, 
                tt.Name AS TaskTypeName
            FROM Task t
            JOIN Project p ON t.IdProject = p.IdProject
            JOIN TaskType tt ON t.IdTaskType = tt.IdTaskType
            WHERE t.{columnToFilter} = @TeamMemberId
            ORDER BY t.Deadline DESC";

        var tasks = new List<dynamic>();
        using (var command = new SqlCommand(query, new SqlConnection(_connectionString)))
        {
            command.Parameters.AddWithValue("@TeamMemberId", teamMemberId);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tasks.Add(new
                    {
                        TaskName = reader["TaskName"],
                        Description = reader["Description"],
                        DeadLine = reader["Deadline"],
                        ProjectName = reader["ProjectName"],
                        TaskTypeName = reader["TaskTypeName"]
                    });
                }
            }
        }

        return tasks;
    }

    public async System.Threading.Tasks.Task AddTask(TaskDto dto)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new DataBaseException("No such task name");
        }

        if (string.IsNullOrWhiteSpace(dto.Description))
        {
            throw new DataBaseException("No such task description");
        }

        if (dto.Deadline == default)
        {
            throw new DataBaseException("No deadline");
        }
        
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Validate foreign keys exist
        await ValidateForeignKeyExists(connection, "Project", dto.IdProject);
        await ValidateForeignKeyExists(connection, "TaskType", dto.IdTaskType);
        await ValidateForeignKeyExists(connection, "TeamMember", dto.IdAssignedTo);
        await ValidateForeignKeyExists(connection, "TeamMember", dto.IdCreator);

        // Insert new task
        const string sql = @"
            INSERT INTO Task (Name, Description, Deadline, IdProject, IdTaskType, IdAssignedTo, IdCreator)
            VALUES (@Name, @Description, @Deadline, @IdProject, @IdTaskType, @IdAssignedTo, @IdCreator);
            SELECT CAST(SCOPE_IDENTITY() as int);";

        var parameters = new
        {
            dto.Name,
            dto.Description,
            dto.Deadline,
            dto.IdProject,
            dto.IdTaskType,
            dto.IdAssignedTo,
            dto.IdCreator
        };

        try
        {
            var taskId = await connection.ExecuteScalarAsync<int>(sql, parameters);
            return taskId;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to create task", ex);
        }
    }
    
    private async Task ValidateForeignKeyExists(SqlConnection connection, string tableName, int id)
    {
        var sql = $"SELECT COUNT(1) FROM {tableName} WHERE Id{tableName} = @Id";
        var exists = await connection.ExecuteScalarAsync<bool>(sql, new { Id = id });
        
        if (!exists)
            throw new ArgumentException($"{tableName} with ID {id} does not exist");
    }
}