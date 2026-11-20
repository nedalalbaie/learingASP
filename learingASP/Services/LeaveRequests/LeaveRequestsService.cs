using Azure.Core;
using learingASP.Calculators;
using learingASP.Common;
using learingASP.Dtos.LeaveRequests;
using learingASP.Enums;
using learingASP.Models;
using learingASP.Repositories;

namespace learingASP.Services.LeaveRequests
{
    public class LeaveRequestsService(IUnitOfWork unitOfWork) : ILeaveRequestsService
    {
        public async Task<Result<List<LeaveRequestResponseDto>>> GetAllLeaveRequestsAsync()
        {
            var leaveRequests = await unitOfWork.LeaveRequestsRepository
                .GetAllLeaveRequestsAsync();

            var response = leaveRequests.Select(lr => new LeaveRequestResponseDto
            {
                Id = lr.Id,
                EmployeeId = lr.EmployeeId,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Days = lr.Days,
                Status = lr.Status,
                IsAdminCreated = lr.IsAdminCreated,
                CreatedById = lr.CreatedById,
                ReviewedById = lr.ReviewedById,
                ReviewedAt = lr.ReviewedAt,
                CreatedAt = lr.CreatedAt
            }).ToList();

            return Result<List<LeaveRequestResponseDto>>.Success(response);
        }

        public async Task<Result<LeaveRequestResponseDto>>
            CreateLeaveRequestAsync(int employeeId, CreateLeaveRequestDto leaveRequestDto)
        {
            var employee = await unitOfWork.EmployeesRepository
                     .GetEmployeeByIdAsync(employeeId);

            if (employee is null || !employee.IsActive)
            {
                return Error.NotFound("Employee not found or inactive.");
            }

            if (leaveRequestDto.EndDate < leaveRequestDto.StartDate)
            {
                return Error.Validation("End date cannot be before start date.");
            }

            var currentYear = DateTime.UtcNow.Year;

            if (leaveRequestDto.StartDate.Year != currentYear ||
                leaveRequestDto.EndDate.Year != currentYear)
            {
                return Error.Validation(
                    "Leave requests must be within the current calendar year.");
            }

            if (leaveRequestDto.StartDate < employee.StartDate)
            {
                return Error.Validation("Leave cannot start before the employee's start date.");
            }

            var hasOverlap = await unitOfWork.LeaveRequestsRepository
                .HasOverlappingLeaveRequestAsync(
                    employeeId,
                    leaveRequestDto.StartDate,
                    leaveRequestDto.EndDate);

            if (hasOverlap)
            {
                return Error.Conflict(
                    "This leave request overlaps with an existing leave request.");
            }

            var requestedDays = WorkingDaysCalculator.Count(leaveRequestDto.StartDate, leaveRequestDto.EndDate);

            if (requestedDays <= 0)
            {
                return Error.Validation(
                 "Leave request must contain at least one working day.");
            }

            var entitlement = EntitlementCalculator.ForYear(employee.StartDate, currentYear);

            var approvedDays = await unitOfWork.LeaveRequestsRepository
                 .GetApprovedLeaveRequestDaysAsync(employeeId, currentYear);

            var pendingDays = await unitOfWork.LeaveRequestsRepository
                 .GetPendingLeaveRequestDaysAsync(employeeId, currentYear);

            var availableDays = entitlement - approvedDays - pendingDays;

            if (requestedDays > availableDays)
            {
                return Error.Validation(
                     $"Insufficient leave balance. Available days: {availableDays}.");
            }

            var newLeaveRequest = new LeaveRequest
            {
                EmployeeId = employeeId,
                StartDate = leaveRequestDto.StartDate,
                EndDate = leaveRequestDto.EndDate,
                Days = requestedDays,
                CreatedById = employeeId,
                Status = LeaveStatus.Pending,
                IsAdminCreated = false
            };

            unitOfWork.LeaveRequestsRepository.AddLeaveRequestAsync(newLeaveRequest);
            await unitOfWork.SaveChangesAsync();

            return new LeaveRequestResponseDto
            {
                Id = newLeaveRequest.Id,
                EmployeeId = newLeaveRequest.EmployeeId,
                StartDate = newLeaveRequest.StartDate,
                EndDate = newLeaveRequest.EndDate,
                Days = newLeaveRequest.Days,
                Status = newLeaveRequest.Status,
                IsAdminCreated = newLeaveRequest.IsAdminCreated,
                CreatedById = newLeaveRequest.CreatedById,
                ReviewedById = newLeaveRequest.ReviewedById,
                ReviewedAt = newLeaveRequest.ReviewedAt,
                CreatedAt = newLeaveRequest.CreatedAt
            };
        }

        public async Task<Result<LeaveRequestResponseDto>>
             AdminCreateLeaveRequestAsync(int adminId, AdminCreateLeaveRequestDto leaveRequestDto)
        {
            var admin = await unitOfWork.EmployeesRepository
                 .GetEmployeeByIdAsync(adminId);

            var employee = await unitOfWork.EmployeesRepository
                .GetEmployeeByIdAsync(leaveRequestDto.EmployeeId);

            if (employee is null || !employee.IsActive)
            {
                return Error.NotFound("Employee not found or inactive.");
            }

            if (leaveRequestDto.EndDate < leaveRequestDto.StartDate)
            {
                return Error.Validation("End date cannot be before start date.");
            }

            var currentYear = DateTime.UtcNow.Year;

            if (leaveRequestDto.StartDate.Year != currentYear ||
                leaveRequestDto.EndDate.Year != currentYear)
            {
                return Error.Validation(
                    "Leave must be within the current calendar year.");
            }

            if (leaveRequestDto.StartDate < employee.StartDate)
            {
                return Error.Validation("Leave cannot start before the employee's start date.");
            }

            var hasOverlap = await unitOfWork.LeaveRequestsRepository
                .HasOverlappingLeaveRequestAsync(
                    leaveRequestDto.EmployeeId,
                    leaveRequestDto.StartDate,
                    leaveRequestDto.EndDate);

            if (hasOverlap)
            {
                return Error.Conflict(
                    "This leave request overlaps with an existing leave request.");
            }

            var requestedDays = WorkingDaysCalculator.Count(
                leaveRequestDto.StartDate,
                leaveRequestDto.EndDate);

            if (requestedDays <= 0)
            {
                return Error.Validation(
                    "Leave request must contain at least one working day.");
            }

            var entitlement = EntitlementCalculator.ForYear(employee.StartDate, currentYear);

            var approvedDays = await unitOfWork.LeaveRequestsRepository
                .GetApprovedLeaveRequestDaysAsync(employee.Id, currentYear);

            var pendingDays = await unitOfWork.LeaveRequestsRepository
                .GetPendingLeaveRequestDaysAsync(employee.Id, currentYear);

            var availableDays = entitlement - approvedDays - pendingDays;

            if (requestedDays > availableDays)
            {
                return Error.Validation(
                    $"Insufficient leave balance. Available days: {availableDays}.");
            }

            var now = DateTime.UtcNow;

            var newLeaveRequest = new LeaveRequest
            {
                EmployeeId = employee.Id,
                StartDate = leaveRequestDto.StartDate,
                EndDate = leaveRequestDto.EndDate,
                Days = requestedDays,
                Status = LeaveStatus.Approved,
                IsAdminCreated = true,
                CreatedById = adminId,
                ReviewedById = adminId,
                CreatedAt = now,
                ReviewedAt = now
            };

            unitOfWork.LeaveRequestsRepository.AddLeaveRequestAsync(newLeaveRequest);

            await unitOfWork.SaveChangesAsync();

            return new LeaveRequestResponseDto
            {
                Id = newLeaveRequest.Id,
                EmployeeId = newLeaveRequest.EmployeeId,
                StartDate = newLeaveRequest.StartDate,
                EndDate = newLeaveRequest.EndDate,
                Days = newLeaveRequest.Days,
                Status = newLeaveRequest.Status,
                IsAdminCreated = newLeaveRequest.IsAdminCreated,
                CreatedById = newLeaveRequest.CreatedById,
                ReviewedById = newLeaveRequest.ReviewedById,
                ReviewedAt = newLeaveRequest.ReviewedAt,
                CreatedAt = newLeaveRequest.CreatedAt
            };
        }

        public async Task<Result<LeaveBalanceDto>> GetEmployeeLeaveBalanceAsync(int employeeId, int? yearQuery)
        {
            var employee = await unitOfWork.EmployeesRepository.GetEmployeeByIdAsync(employeeId);

            if (employee is null)
            {
                return Error.NotFound("Employee not found.");
            }

            var requestedYear = yearQuery ?? DateTime.UtcNow.Year;

            var entitlement = EntitlementCalculator.ForYear(employee.StartDate, requestedYear);

            var approvedDays = await unitOfWork.LeaveRequestsRepository
                .GetApprovedLeaveRequestDaysAsync(employeeId, requestedYear);

            var pendingDays = await unitOfWork.LeaveRequestsRepository
                .GetPendingLeaveRequestDaysAsync(employeeId, requestedYear);

            var availableDays = entitlement - approvedDays - pendingDays;

            return new LeaveBalanceDto
            {
                EmployeeId = employeeId,
                Year = requestedYear,
                Entitlement = entitlement,
                ApprovedDays = approvedDays,
                PendingDays = pendingDays,
                AvailableDays = availableDays
            };
        }

        public async Task<Result<List<LeaveRequestResponseDto>>> GetEmployeeLeaveRequestsAsync(int employeeId)
        {
            var employee = await unitOfWork.EmployeesRepository
                .GetEmployeeByIdAsync(employeeId);

            if (employee is null || !employee.IsActive)
            {
                return Error.NotFound("Employee not found or inactive.");
            }

            var employeeLeaveRequests = await unitOfWork.LeaveRequestsRepository
                .GetEmployeeLeaveRequestsAsync(employeeId);

            return employeeLeaveRequests.Select(l => new LeaveRequestResponseDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Days = l.Days,
                Status = l.Status,
                IsAdminCreated = l.IsAdminCreated,
                CreatedById = l.CreatedById,
                ReviewedById = l.ReviewedById,
                ReviewedAt = l.ReviewedAt,
                CreatedAt = l.CreatedAt
            }).ToList();
        }

        public async Task<Result> ApproveLeaveRequestAsync(int leaveRequestId, int adminId)
        {
            var admin = await unitOfWork.EmployeesRepository
                .GetEmployeeByIdAsync(adminId);

            if (admin is null || !admin.IsActive)
            {
                return Error.Forbidden("Only active administrators can approve leave requests.");
            }

            var leaveRequest = await unitOfWork.LeaveRequestsRepository
                .GetLeaveRequestByIdAsync(leaveRequestId);

            if (leaveRequest is null)
            {
                return Error.NotFound("Leave request not found.");
            }

            if (leaveRequest.Status != LeaveStatus.Pending)
            {
                return Error.Conflict("Only pending leave requests can be approved.");
            }

            var currentYear = DateTime.UtcNow.Year;

            var entitlement = EntitlementCalculator.ForYear(
                leaveRequest.Employee.StartDate, currentYear);

            var approvedDays = await unitOfWork.LeaveRequestsRepository
                .GetApprovedLeaveRequestDaysAsync(leaveRequest.EmployeeId, currentYear);

            var pendingDays = await unitOfWork.LeaveRequestsRepository
                .GetPendingLeaveRequestDaysAsync(leaveRequest.EmployeeId, currentYear);

            if (approvedDays + pendingDays > entitlement)
            {
                return Error.Conflict("The employee's leave balance has changed. This request can no longer be approved.");
            }

            var now = DateTime.UtcNow;

            leaveRequest.Status = LeaveStatus.Approved;
            leaveRequest.ReviewedById = adminId;
            leaveRequest.ReviewedAt = now;

            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> RejectLeaveRequestAsync(int leaveRequestId, int adminId)
        {
            var admin = await unitOfWork.EmployeesRepository
                .GetEmployeeByIdAsync(adminId);

            if (admin is null || !admin.IsActive)
                return Error.Forbidden("Only active administrators can reject leave requests.");

            var leaveRequest = await unitOfWork.LeaveRequestsRepository
                .GetLeaveRequestByIdAsync(leaveRequestId);

            if (leaveRequest is null)
            {
                return Error.NotFound("Leave request not found.");
            }

            if (leaveRequest.Status != LeaveStatus.Pending)
            {
                return Error.Conflict("Only pending leave requests can be rejected.");
            }

            var now = DateTime.UtcNow;

            leaveRequest.Status = LeaveStatus.Rejected;
            leaveRequest.ReviewedById = adminId;
            leaveRequest.ReviewedAt = now;

            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}
