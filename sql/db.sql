IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [Approvers] (
    [ApproverId] int NOT NULL IDENTITY,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Approvers] PRIMARY KEY ([ApproverId])
);

GO

CREATE TABLE [ParentWorkPackages] (
    [ParentWorkPckageId] int NOT NULL IDENTITY,
    [Status] int NOT NULL,
    CONSTRAINT [PK_ParentWorkPackages] PRIMARY KEY ([ParentWorkPckageId])
);

GO

CREATE TABLE [PayGrade] (
    [PayGradeId] int NOT NULL IDENTITY,
    [PayLevel] nvarchar(max) NULL,
    [Cost] float NOT NULL,
    [Year] int NOT NULL,
    CONSTRAINT [PK_PayGrade] PRIMARY KEY ([PayGradeId])
);

GO

CREATE TABLE [Projects] (
    [ProjectId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [CostingProposal] nvarchar(max) NULL,
    [OriginalBudget] nvarchar(max) NULL,
    [CostAtImplementation] nvarchar(max) NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Projects] PRIMARY KEY ([ProjectId])
);

GO

CREATE TABLE [Supervisors] (
    [SupervisorId] int NOT NULL IDENTITY,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Supervisors] PRIMARY KEY ([SupervisorId])
);

GO

CREATE TABLE [ProjectReports] (
    [ProjectReportId] int NOT NULL IDENTITY,
    [StartingPercentage] float NOT NULL,
    [CompletedPercentage] float NOT NULL,
    [CreatedTime] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [ProjectId] int NOT NULL,
    CONSTRAINT [PK_ProjectReports] PRIMARY KEY ([ProjectReportId]),
    CONSTRAINT [FK_ProjectReports_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([ProjectId]) ON DELETE CASCADE
);

GO

CREATE TABLE [WorkPackages] (
    [WorkPackageId] int NOT NULL IDENTITY,
    [WorkPackageCode] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [Contractor] nvarchar(max) NULL,
    [Purpose] nvarchar(max) NULL,
    [Input] nvarchar(max) NULL,
    [Output] nvarchar(max) NULL,
    [Activity] nvarchar(max) NULL,
    [IsParent] bit NOT NULL,
    [ProjectId] int NOT NULL,
    [ParentWorkPackageId] int NULL,
    CONSTRAINT [PK_WorkPackages] PRIMARY KEY ([WorkPackageId]),
    CONSTRAINT [FK_WorkPackages_ParentWorkPackages_ParentWorkPackageId] FOREIGN KEY ([ParentWorkPackageId]) REFERENCES [ParentWorkPackages] ([ParentWorkPckageId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_WorkPackages_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([ProjectId]) ON DELETE CASCADE
);

GO

CREATE TABLE [Employees] (
    [EmployeeId] int NOT NULL IDENTITY,
    [FirstName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NULL,
    [Title] int NOT NULL,
    [CreatedTime] datetime2 NOT NULL,
    [FlexTime] float NOT NULL,
    [VacationTime] float NOT NULL,
    [Status] int NOT NULL,
    [ApproverId] int NULL,
    [SupervisorId] int NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([EmployeeId]),
    CONSTRAINT [FK_Employees_Approvers_ApproverId] FOREIGN KEY ([ApproverId]) REFERENCES [Approvers] ([ApproverId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Employees_Supervisors_SupervisorId] FOREIGN KEY ([SupervisorId]) REFERENCES [Supervisors] ([SupervisorId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Budgets] (
    [BudgetId] int NOT NULL IDENTITY,
    [Hour] float NOT NULL,
    [Status] int NOT NULL,
    [Type] int NOT NULL,
    [WorkPacakgeId] int NOT NULL,
    [WorkPackageId] int NULL,
    [PayGradeId] int NOT NULL,
    CONSTRAINT [PK_Budgets] PRIMARY KEY ([BudgetId]),
    CONSTRAINT [FK_Budgets_PayGrade_PayGradeId] FOREIGN KEY ([PayGradeId]) REFERENCES [PayGrade] ([PayGradeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Budgets_WorkPackages_WorkPackageId] FOREIGN KEY ([WorkPackageId]) REFERENCES [WorkPackages] ([WorkPackageId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [WorkPackageReports] (
    [WorkPackageReportId] int NOT NULL IDENTITY,
    [WeekNumber] int NOT NULL,
    [Status] int NOT NULL,
    [Comments] nvarchar(max) NULL,
    [WorkAccomplished] nvarchar(max) NULL,
    [WorkAccomplishedNP] nvarchar(max) NULL,
    [Problem] nvarchar(max) NULL,
    [ProblemAnticipated] nvarchar(max) NULL,
    [WorkPackageId] int NOT NULL,
    CONSTRAINT [PK_WorkPackageReports] PRIMARY KEY ([WorkPackageReportId]),
    CONSTRAINT [FK_WorkPackageReports_WorkPackages_WorkPackageId] FOREIGN KEY ([WorkPackageId]) REFERENCES [WorkPackages] ([WorkPackageId]) ON DELETE CASCADE
);

GO

CREATE TABLE [Credentials] (
    [CredentialId] int NOT NULL IDENTITY,
    [Password] nvarchar(max) NULL,
    [Status] int NOT NULL,
    [EmployeeId] int NOT NULL,
    CONSTRAINT [PK_Credentials] PRIMARY KEY ([CredentialId]),
    CONSTRAINT [FK_Credentials_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE CASCADE
);

GO

CREATE TABLE [EmployeePays] (
    [EmployeePayId] int NOT NULL IDENTITY,
    [Year] int NOT NULL,
    [Status] int NOT NULL,
    [EmployeeId] int NOT NULL,
    [PayGradeId] int NOT NULL,
    CONSTRAINT [PK_EmployeePays] PRIMARY KEY ([EmployeePayId]),
    CONSTRAINT [FK_EmployeePays_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_EmployeePays_PayGrade_PayGradeId] FOREIGN KEY ([PayGradeId]) REFERENCES [PayGrade] ([PayGradeId]) ON DELETE CASCADE
);

GO

CREATE TABLE [ProjectEmployees] (
    [ProjectEmployeeId] int NOT NULL IDENTITY,
    [Status] int NOT NULL,
    [Role] int NOT NULL,
    [ProjectId] int NOT NULL,
    [EmployeeId] int NOT NULL,
    CONSTRAINT [PK_ProjectEmployees] PRIMARY KEY ([ProjectEmployeeId]),
    CONSTRAINT [FK_ProjectEmployees_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProjectEmployees_Projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [Projects] ([ProjectId]) ON DELETE CASCADE
);

GO

CREATE TABLE [Signatures] (
    [SignatureId] int NOT NULL IDENTITY,
    [PassPhrase] nvarchar(max) NULL,
    [HashedSignature] nvarchar(max) NULL,
    [CreatedTime] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [EmployeeId] int NOT NULL,
    CONSTRAINT [PK_Signatures] PRIMARY KEY ([SignatureId]),
    CONSTRAINT [FK_Signatures_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE CASCADE
);

GO

CREATE TABLE [Timesheets] (
    [TimesheetId] int NOT NULL IDENTITY,
    [WeekEnding] datetime2 NOT NULL,
    [WeekNumber] int NOT NULL,
    [FlexTime] float NOT NULL,
    [Status] int NOT NULL,
    [EmployeeId] int NOT NULL,
    CONSTRAINT [PK_Timesheets] PRIMARY KEY ([TimesheetId]),
    CONSTRAINT [FK_Timesheets_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE CASCADE
);

GO

CREATE TABLE [WorkPackageEmployees] (
    [WorkPackageEmployeeId] int NOT NULL IDENTITY,
    [Role] int NOT NULL,
    [Status] int NOT NULL,
    [WorkPackageId] int NOT NULL,
    [EmployeeId] int NOT NULL,
    CONSTRAINT [PK_WorkPackageEmployees] PRIMARY KEY ([WorkPackageEmployeeId]),
    CONSTRAINT [FK_WorkPackageEmployees_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([EmployeeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_WorkPackageEmployees_WorkPackages_WorkPackageId] FOREIGN KEY ([WorkPackageId]) REFERENCES [WorkPackages] ([WorkPackageId]) ON DELETE CASCADE
);

GO

CREATE TABLE [TimesheetRows] (
    [TimesheetRowId] int NOT NULL IDENTITY,
    [SatHour] float NOT NULL,
    [SunHour] float NOT NULL,
    [MonHour] float NOT NULL,
    [TueHour] float NOT NULL,
    [WedHour] float NOT NULL,
    [ThuHour] float NOT NULL,
    [FriHour] float NOT NULL,
    [Notes] nvarchar(max) NULL,
    [TimesheetId] int NOT NULL,
    [WorkPackageId] int NOT NULL,
    [PayGradeId] int NOT NULL,
    CONSTRAINT [PK_TimesheetRows] PRIMARY KEY ([TimesheetRowId]),
    CONSTRAINT [FK_TimesheetRows_PayGrade_PayGradeId] FOREIGN KEY ([PayGradeId]) REFERENCES [PayGrade] ([PayGradeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TimesheetRows_Timesheets_TimesheetId] FOREIGN KEY ([TimesheetId]) REFERENCES [Timesheets] ([TimesheetId]) ON DELETE CASCADE,
    CONSTRAINT [FK_TimesheetRows_WorkPackages_WorkPackageId] FOREIGN KEY ([WorkPackageId]) REFERENCES [WorkPackages] ([WorkPackageId]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_Budgets_PayGradeId] ON [Budgets] ([PayGradeId]);

GO

CREATE INDEX [IX_Budgets_WorkPackageId] ON [Budgets] ([WorkPackageId]);

GO

CREATE INDEX [IX_Credentials_EmployeeId] ON [Credentials] ([EmployeeId]);

GO

CREATE INDEX [IX_EmployeePays_EmployeeId] ON [EmployeePays] ([EmployeeId]);

GO

CREATE INDEX [IX_EmployeePays_PayGradeId] ON [EmployeePays] ([PayGradeId]);

GO

CREATE INDEX [IX_Employees_ApproverId] ON [Employees] ([ApproverId]);

GO

CREATE INDEX [IX_Employees_SupervisorId] ON [Employees] ([SupervisorId]);

GO

CREATE INDEX [IX_ProjectEmployees_EmployeeId] ON [ProjectEmployees] ([EmployeeId]);

GO

CREATE INDEX [IX_ProjectEmployees_ProjectId] ON [ProjectEmployees] ([ProjectId]);

GO

CREATE INDEX [IX_ProjectReports_ProjectId] ON [ProjectReports] ([ProjectId]);

GO

CREATE INDEX [IX_Signatures_EmployeeId] ON [Signatures] ([EmployeeId]);

GO

CREATE INDEX [IX_TimesheetRows_PayGradeId] ON [TimesheetRows] ([PayGradeId]);

GO

CREATE INDEX [IX_TimesheetRows_TimesheetId] ON [TimesheetRows] ([TimesheetId]);

GO

CREATE INDEX [IX_TimesheetRows_WorkPackageId] ON [TimesheetRows] ([WorkPackageId]);

GO

CREATE INDEX [IX_Timesheets_EmployeeId] ON [Timesheets] ([EmployeeId]);

GO

CREATE INDEX [IX_WorkPackageEmployees_EmployeeId] ON [WorkPackageEmployees] ([EmployeeId]);

GO

CREATE INDEX [IX_WorkPackageEmployees_WorkPackageId] ON [WorkPackageEmployees] ([WorkPackageId]);

GO

CREATE INDEX [IX_WorkPackageReports_WorkPackageId] ON [WorkPackageReports] ([WorkPackageId]);

GO

CREATE INDEX [IX_WorkPackages_ParentWorkPackageId] ON [WorkPackages] ([ParentWorkPackageId]);

GO

CREATE INDEX [IX_WorkPackages_ProjectId] ON [WorkPackages] ([ProjectId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190204230716_EruptMigration', N'2.2.1-servicing-10028');

GO