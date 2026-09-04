using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Linq;
using System.Text.Json;

namespace SchoolScheduler.IntegrationTests;

public class ArgumentOrderTests : IClassFixture<SchoolSchedulerFactory>
{
    private readonly HttpClient _client;

    public ArgumentOrderTests(SchoolSchedulerFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task FullFlow_VerifyArgumentOrder()
    {
        // 1. Setup School
        var schoolSignup = new { Email = "admin@test.com", Password = "Password123!", SchoolName = "Test School", LecturesPerDay = 8 };
        var schoolResp = await _client.PostAsJsonAsync("/auth/school/signup", schoolSignup);
        if (schoolResp.StatusCode != HttpStatusCode.OK)
        {
            var errorContent = await schoolResp.Content.ReadAsStringAsync();
            throw new Exception($"School signup failed with {schoolResp.StatusCode}: {errorContent}");
        }
        var schoolData = await schoolResp.Content.ReadFromJsonAsync<JsonElement>();
        string schoolToken = schoolData.GetProperty("token").GetString()!;
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", schoolToken);

        // 2. Setup Grade
        var gradeResp = await _client.PostAsJsonAsync("/school/management/grades", new { Category = 1, Level = 1 });
        var gradeId = (await gradeResp.Content.ReadAsStringAsync()).Trim('"');

        // 3. Setup Classroom
        var roomResp = await _client.PostAsJsonAsync("/school/management/class-rooms", new { GradeId = gradeId, Name = "Room A" });
        var roomId = (await roomResp.Content.ReadAsStringAsync()).Trim('"');

        // 4. Setup Teachers
        var t1Id = await CreateTeacherApi("Teacher A", "t1@test.com");
        var t2Id = await CreateTeacherApi("Teacher B", "t2@test.com");

        // 5. Setup Subjects
        var s1Id = await CreateSubjectApi("Math");
        var s2Id = await CreateSubjectApi("Physics");

        // --- TEST 1: CreateClassSubjectAssignment ---
        // Route: POST /school/assignments
        // Command: CreateClassSubjectAssignmentCommand(Guid ClassRoomId, Guid SubjectId, Guid TeacherId, int WeeklyQuota)
        var assignmentReq = new { ClassRoomId = roomId, SubjectId = s1Id, TeacherId = t1Id, WeeklyQuota = 5 };
        var assignResp = await _client.PostAsJsonAsync("/school/assignments", assignmentReq);
        Assert.Equal(HttpStatusCode.OK, assignResp.StatusCode);

        var getAssignResp = await _client.GetAsync($"/school/assignments/class-room/{roomId}");
        var assignments = await getAssignResp.Content.ReadFromJsonAsync<List<JsonElement>>();
        var assignment = assignments.FirstOrDefault(a => a.GetProperty("subjectId").GetString() == s1Id);
        Assert.NotNull(assignment);
        Assert.Equal(t1Id, assignment.GetProperty("teacherId").GetString());

        // --- TEST 2: UnassignSubjectFromTeacher ---
        // Route: DELETE /school/teachers/{id}/subjects/{subjectId}
        // Command: UnassignSubjectFromTeacherCommand(Guid TeacherId, Guid SubjectId)
        var unassignResp = await _client.DeleteAsync($"/school/teachers/{t1Id}/subjects/{s1Id}");
        Assert.Equal(HttpStatusCode.OK, unassignResp.StatusCode);

        // Verify it's gone
        var getAssignResp2 = await _client.GetAsync($"/school/assignments/class-room/{roomId}");
        var assignments2 = await getAssignResp2.Content.ReadFromJsonAsync<List<JsonElement>>();
        Assert.Empty(assignments2.Where(a => a.GetProperty("subjectId").GetString() == s1Id));

        // --- TEST 3: UpdateClassSubjectAssignment ---
        // First recreate the assignment
        await _client.PostAsJsonAsync("/school/assignments", new { ClassRoomId = roomId, SubjectId = s2Id, TeacherId = t2Id, WeeklyQuota = 3 });

        // Get the assignment ID
        var currentAssigns = await _client.GetAsync($"/school/assignments/class-room/{roomId}");
        var currentList = await currentAssigns.Content.ReadFromJsonAsync<List<JsonElement>>();
        var assignId = currentList.First().GetProperty("id").GetString()!;

        // Update it to Teacher A
        var updateReq = new { TeacherId = t1Id, SubjectId = s2Id, WeeklyQuota = 10 };
        var updateResp = await _client.PutAsJsonAsync($"/school/assignments/{assignId}", updateReq);
        Assert.Equal(HttpStatusCode.OK, updateResp.StatusCode);

        // Verify update
        var finalAssigns = await _client.GetAsync($"/school/assignments/class-room/{roomId}");
        var finalList = await finalAssigns.Content.ReadFromJsonAsync<List<JsonElement>>();
        var finalAssign = finalList.First();
        Assert.Equal(t1Id, finalAssign.GetProperty("teacherId").GetString());
        Assert.Equal(s2Id, finalAssign.GetProperty("subjectId").GetString());
    }

    private async Task<string> CreateTeacherApi(string name, string email)
    {
        var resp = await _client.PostAsJsonAsync("/school/teachers", new { Name = name, Email = email });
        return (await resp.Content.ReadAsStringAsync()).Trim('"');
    }

    private async Task<string> CreateSubjectApi(string name)
    {
        var resp = await _client.PostAsJsonAsync("/school/management/subjects", new { Name = name });
        return (await resp.Content.ReadAsStringAsync()).Trim('"');
    }
}
