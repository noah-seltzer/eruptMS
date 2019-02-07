using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace COMP4911Timesheets.Migrations
{
    public partial class ExtendedUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Approvers",
                columns: table => new
                {
                    ApproverId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approvers", x => x.ApproverId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParentWorkPackages",
                columns: table => new
                {
                    ParentWorkPckageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentWorkPackages", x => x.ParentWorkPckageId);
                });

            migrationBuilder.CreateTable(
                name: "PayGrades",
                columns: table => new
                {
                    PayGradeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PayLevel = table.Column<string>(nullable: true),
                    Cost = table.Column<double>(nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayGrades", x => x.PayGradeId);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CostingProposal = table.Column<string>(nullable: true),
                    OriginalBudget = table.Column<string>(nullable: true),
                    CostAtImplementation = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                });

            migrationBuilder.CreateTable(
                name: "Supervisors",
                columns: table => new
                {
                    SupervisorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervisors", x => x.SupervisorId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectReports",
                columns: table => new
                {
                    ProjectReportId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StartingPercentage = table.Column<double>(nullable: false),
                    CompletedPercentage = table.Column<double>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectReports", x => x.ProjectReportId);
                    table.ForeignKey(
                        name: "FK_ProjectReports_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkPackages",
                columns: table => new
                {
                    WorkPackageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WorkPackageCode = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Contractor = table.Column<string>(nullable: true),
                    Purpose = table.Column<string>(nullable: true),
                    Input = table.Column<string>(nullable: true),
                    Output = table.Column<string>(nullable: true),
                    Activity = table.Column<string>(nullable: true),
                    IsParent = table.Column<bool>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    ParentWorkPackageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPackages", x => x.WorkPackageId);
                    table.ForeignKey(
                        name: "FK_WorkPackages_ParentWorkPackages_ParentWorkPackageId",
                        column: x => x.ParentWorkPackageId,
                        principalTable: "ParentWorkPackages",
                        principalColumn: "ParentWorkPckageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkPackages_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Title = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    FlexTime = table.Column<double>(nullable: false),
                    VacationTime = table.Column<double>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ApproverId = table.Column<int>(nullable: true),
                    SupervisorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.UniqueConstraint("AK_AspNetUsers_EmployeeId", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Approvers_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Approvers",
                        principalColumn: "ApproverId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Supervisors_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisors",
                        principalColumn: "SupervisorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    BudgetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Hour = table.Column<double>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    WorkPacakgeId = table.Column<int>(nullable: false),
                    WorkPackageId = table.Column<int>(nullable: true),
                    PayGradeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.BudgetId);
                    table.ForeignKey(
                        name: "FK_Budgets_PayGrades_PayGradeId",
                        column: x => x.PayGradeId,
                        principalTable: "PayGrades",
                        principalColumn: "PayGradeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Budgets_WorkPackages_WorkPackageId",
                        column: x => x.WorkPackageId,
                        principalTable: "WorkPackages",
                        principalColumn: "WorkPackageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkPackageReports",
                columns: table => new
                {
                    WorkPackageReportId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WeekNumber = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    WorkAccomplished = table.Column<string>(nullable: true),
                    WorkAccomplishedNP = table.Column<string>(nullable: true),
                    Problem = table.Column<string>(nullable: true),
                    ProblemAnticipated = table.Column<string>(nullable: true),
                    WorkPackageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPackageReports", x => x.WorkPackageReportId);
                    table.ForeignKey(
                        name: "FK_WorkPackageReports_WorkPackages_WorkPackageId",
                        column: x => x.WorkPackageId,
                        principalTable: "WorkPackages",
                        principalColumn: "WorkPackageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Credentials",
                columns: table => new
                {
                    CredentialId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Password = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    EmployeeId1 = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credentials", x => x.CredentialId);
                    table.ForeignKey(
                        name: "FK_Credentials_AspNetUsers_EmployeeId1",
                        column: x => x.EmployeeId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeePays",
                columns: table => new
                {
                    EmployeePayId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssignedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    EmployeeId1 = table.Column<string>(nullable: false),
                    PayGradeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePays", x => x.EmployeePayId);
                    table.ForeignKey(
                        name: "FK_EmployeePays_AspNetUsers_EmployeeId1",
                        column: x => x.EmployeeId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeePays_PayGrades_PayGradeId",
                        column: x => x.PayGradeId,
                        principalTable: "PayGrades",
                        principalColumn: "PayGradeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectEmployees",
                columns: table => new
                {
                    ProjectEmployeeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<int>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    EmployeeId1 = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectEmployees", x => x.ProjectEmployeeId);
                    table.ForeignKey(
                        name: "FK_ProjectEmployees_AspNetUsers_EmployeeId1",
                        column: x => x.EmployeeId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectEmployees_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Signatures",
                columns: table => new
                {
                    SignatureId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PassPhrase = table.Column<string>(nullable: true),
                    HashedSignature = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    EmployeeId1 = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signatures", x => x.SignatureId);
                    table.ForeignKey(
                        name: "FK_Signatures_AspNetUsers_EmployeeId1",
                        column: x => x.EmployeeId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Timesheets",
                columns: table => new
                {
                    TimesheetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WeekEnding = table.Column<DateTime>(nullable: false),
                    WeekNumber = table.Column<int>(nullable: false),
                    FlexTime = table.Column<double>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    EmployeeId1 = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timesheets", x => x.TimesheetId);
                    table.ForeignKey(
                        name: "FK_Timesheets_AspNetUsers_EmployeeId1",
                        column: x => x.EmployeeId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkPackageEmployees",
                columns: table => new
                {
                    WorkPackageEmployeeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Role = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    WorkPackageId = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    EmployeeId1 = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPackageEmployees", x => x.WorkPackageEmployeeId);
                    table.ForeignKey(
                        name: "FK_WorkPackageEmployees_AspNetUsers_EmployeeId1",
                        column: x => x.EmployeeId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkPackageEmployees_WorkPackages_WorkPackageId",
                        column: x => x.WorkPackageId,
                        principalTable: "WorkPackages",
                        principalColumn: "WorkPackageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimesheetRows",
                columns: table => new
                {
                    TimesheetRowId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SatHour = table.Column<double>(nullable: false),
                    SunHour = table.Column<double>(nullable: false),
                    MonHour = table.Column<double>(nullable: false),
                    TueHour = table.Column<double>(nullable: false),
                    WedHour = table.Column<double>(nullable: false),
                    ThuHour = table.Column<double>(nullable: false),
                    FriHour = table.Column<double>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    TimesheetId = table.Column<int>(nullable: false),
                    WorkPackageId = table.Column<int>(nullable: false),
                    PayGradeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimesheetRows", x => x.TimesheetRowId);
                    table.ForeignKey(
                        name: "FK_TimesheetRows_PayGrades_PayGradeId",
                        column: x => x.PayGradeId,
                        principalTable: "PayGrades",
                        principalColumn: "PayGradeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimesheetRows_Timesheets_TimesheetId",
                        column: x => x.TimesheetId,
                        principalTable: "Timesheets",
                        principalColumn: "TimesheetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimesheetRows_WorkPackages_WorkPackageId",
                        column: x => x.WorkPackageId,
                        principalTable: "WorkPackages",
                        principalColumn: "WorkPackageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ApproverId",
                table: "AspNetUsers",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SupervisorId",
                table: "AspNetUsers",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_PayGradeId",
                table: "Budgets",
                column: "PayGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_WorkPackageId",
                table: "Budgets",
                column: "WorkPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Credentials_EmployeeId1",
                table: "Credentials",
                column: "EmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePays_EmployeeId1",
                table: "EmployeePays",
                column: "EmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePays_PayGradeId",
                table: "EmployeePays",
                column: "PayGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectEmployees_EmployeeId1",
                table: "ProjectEmployees",
                column: "EmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectEmployees_ProjectId",
                table: "ProjectEmployees",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectReports_ProjectId",
                table: "ProjectReports",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Signatures_EmployeeId1",
                table: "Signatures",
                column: "EmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetRows_PayGradeId",
                table: "TimesheetRows",
                column: "PayGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetRows_TimesheetId",
                table: "TimesheetRows",
                column: "TimesheetId");

            migrationBuilder.CreateIndex(
                name: "IX_TimesheetRows_WorkPackageId",
                table: "TimesheetRows",
                column: "WorkPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_EmployeeId1",
                table: "Timesheets",
                column: "EmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageEmployees_EmployeeId1",
                table: "WorkPackageEmployees",
                column: "EmployeeId1");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageEmployees_WorkPackageId",
                table: "WorkPackageEmployees",
                column: "WorkPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageReports_WorkPackageId",
                table: "WorkPackageReports",
                column: "WorkPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackages_ParentWorkPackageId",
                table: "WorkPackages",
                column: "ParentWorkPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackages_ProjectId",
                table: "WorkPackages",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropTable(
                name: "Credentials");

            migrationBuilder.DropTable(
                name: "EmployeePays");

            migrationBuilder.DropTable(
                name: "ProjectEmployees");

            migrationBuilder.DropTable(
                name: "ProjectReports");

            migrationBuilder.DropTable(
                name: "Signatures");

            migrationBuilder.DropTable(
                name: "TimesheetRows");

            migrationBuilder.DropTable(
                name: "WorkPackageEmployees");

            migrationBuilder.DropTable(
                name: "WorkPackageReports");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "PayGrades");

            migrationBuilder.DropTable(
                name: "Timesheets");

            migrationBuilder.DropTable(
                name: "WorkPackages");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ParentWorkPackages");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Approvers");

            migrationBuilder.DropTable(
                name: "Supervisors");
        }
    }
}
