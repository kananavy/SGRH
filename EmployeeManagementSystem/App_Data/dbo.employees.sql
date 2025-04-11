﻿CREATE TABLE [dbo].[employees] (
    [id]             INT           IDENTITY (1, 1) NOT NULL,
    [employee_id]    VARCHAR (MAX) NULL,
    [full_name]      VARCHAR (MAX) NULL,
    [gender]         VARCHAR (MAX) NULL,
    [contact_number] VARCHAR (MAX) NULL,
	[adresse] VARCHAR (MAX) NULL,
    [position]       VARCHAR (MAX) NULL,
    [image]          VARCHAR (MAX) NULL,
    [salary]         INT           NULL,
    [insert_date]    DATE          NULL,
    [update_date]    DATE          NULL,
    [delete_date]    DATE          NULL,
    [status]         VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

