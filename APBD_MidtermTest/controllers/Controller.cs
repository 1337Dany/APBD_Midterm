using APBD_MidtermTest.models;
using APBD_MidtermTest.dtos;
using APBD_MidtermTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_MidtermTest.controllers;

[ApiController]
[Route ("api/[controller]")]
public class Controller(IDbService dbService)
{
    private readonly IDbService _dbService = dbService;

    [HttpGet("api/tasks/{id}")]
    public async Task<IResult> GetById(int id)
    {
        var result = await _dbService.GetTasksOfTeamMemberAsync(id);
        return result == null ? Results.NotFound() : Results.Ok(result);
    }

    [HttpGet("api/tasks")]
    public async Task<IResult> CreateTask(TaskDto dto)
    {
        if (dto == null)
        {
            return Results.BadRequest("Invalid data.");
        }
        
        try
        {
            await _dbService.AddTask(dto);
            return Results.Ok("Task have been successfully created.");
        }
        catch (Exception ex)
        {
            return Results.StatusCode(500);
        }
    }

    // [HttpPost("/{id}")]
    // public async Task<IResult> Save(TestDto dto)
    // {
    //     if (dto == null)
    //     {
    //         return Results.BadRequest("Invalid data.");
    //     }
    //
    //     try
    //     {
    //         await _dbService.CreatePotatoTeacherAsync(dto);
    //         return Results.Ok("Quiz and Potato Teacher have been successfully created.");
    //     }
    //     catch (Exception ex)
    //     {
    //         return Results.StatusCode(500);
    //     }
    // }
}