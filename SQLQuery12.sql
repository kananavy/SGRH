CREATE TABLE [dbo].[payrool] (
    [id]             INT           IDENTITY (1, 1) NOT NULL,
    [employee_id]    VARCHAR (MAX) NULL,
    [full_name]      VARCHAR (MAX) NULL,
    [departement]    VARCHAR (MAX) NULL,
    [position]       VARCHAR (MAX) NULL,
    [salary_base]    INT           NULL,
    [overtime_hours] INT           NULL,
    [overtime_rate]  INT           NULL,
    [bonuses]        INT           NULL,
    [salary]         INT           NULL,
    [insert_date]    DATE          NULL,
    [date_recrute]   DATE          NULL,
    [update_date]    DATE          NULL,
    [delete_date]    DATE          NULL,
    [status]         VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

