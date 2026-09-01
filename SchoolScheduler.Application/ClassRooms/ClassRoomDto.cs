using System;

namespace SchoolScheduler.Application.ClassRooms;

public record ClassRoomDto(Guid Id, Guid SchoolId, Guid GradeId, string Name);
