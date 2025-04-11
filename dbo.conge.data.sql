SET IDENTITY_INSERT [dbo].[conge] ON
INSERT INTO [dbo].[conge] ([id], [employee_id], [full_name], [departement], [position], [adresse], [motif], [num_conge], [reliquat], [nature_conge], [insert_date], [periode_debut], [periode_fin], [update_date], [delete_date]) VALUES (5134, N'q', N'q', N'INFORMATIQUE', N'Administrateur resau', N'q', N'q', 1, N'19', N'EXCEPTIONNEL', N'2024-09-19', N'2024-09-19 00:00:00', N'2024-09-29 00:00:00', NULL, NULL)
INSERT INTO [dbo].[conge] ([id], [employee_id], [full_name], [departement], [position], [adresse], [motif], [num_conge], [reliquat], [nature_conge], [insert_date], [periode_debut], [periode_fin], [update_date], [delete_date]) VALUES (5135, N'q', N'q', N'INFORMATIQUE', N'Administrateur resau', N'q', N'q', 2, N'19', N'EXCEPTIONNEL', N'2024-09-19', N'2024-09-19 00:00:00', N'2024-09-29 00:00:00', NULL, NULL)
INSERT INTO [dbo].[conge] ([id], [employee_id], [full_name], [departement], [position], [adresse], [motif], [num_conge], [reliquat], [nature_conge], [insert_date], [periode_debut], [periode_fin], [update_date], [delete_date]) VALUES (5136, N'q', N'q', N'INFORMATIQUE', N'Administrateur resau', N'q', N'q', 3, N'21', N'EXCEPTIONNEL', N'2024-09-19', N'2024-09-19 00:00:00', N'2024-09-27 00:00:00', NULL, NULL)
INSERT INTO [dbo].[conge] ([id], [employee_id], [full_name], [departement], [position], [adresse], [motif], [num_conge], [reliquat], [nature_conge], [insert_date], [periode_debut], [periode_fin], [update_date], [delete_date]) VALUES (5137, N'q', N'q', N'INFORMATIQUE', N'Administrateur resau', N'q', N'q', 4, N'21', N'EXCEPTIONNEL', N'2024-09-19', N'2024-09-19 00:00:00', N'2024-09-27 00:00:00', NULL, NULL)
INSERT INTO [dbo].[conge] ([id], [employee_id], [full_name], [departement], [position], [adresse], [motif], [num_conge], [reliquat], [nature_conge], [insert_date], [periode_debut], [periode_fin], [update_date], [delete_date]) VALUES (5138, N'q', N'q', N'INFORMATIQUE', N'Administrateur resau', N'q', N'q', 5, N'21', N'EXCEPTIONNEL', N'2024-09-19', N'2024-09-19 00:00:00', N'2024-09-27 00:00:00', NULL, NULL)
SET IDENTITY_INSERT [dbo].[conge] OFF
WITH CTE AS (
    SELECT 
        id,
        ROW_NUMBER() OVER (PARTITION BY employee_id, periode_debut, periode_fin ORDER BY id) AS row_num
    FROM conge
)
DELETE FROM CTE WHERE row_num > 1;
WITH LatestConge AS (
    SELECT
        employee_id,
        MAX(insert_date) AS latest_date
    FROM conge
    WHERE delete_date IS NULL
    GROUP BY employee_id
)
SELECT
    c.employee_id,
    c.full_name,
    c.departement,
    c.position,
    c.nature_conge,
    c.motif,
    c.adresse,
    c.reliquat,
    c.periode_debut,
    c.periode_fin,
    (SELECT COUNT(*) FROM conge WHERE employee_id = c.employee_id AND delete_date IS NULL) AS total_conge,
    c.num_conge
FROM conge c
INNER JOIN LatestConge lc ON c.employee_id = lc.employee_id AND c.insert_date = lc.latest_date
WHERE c.delete_date IS NULL

WITH LatestConge AS (
    SELECT
        employee_id,
        MAX(num_conge) AS latest_num_conge
    FROM conge
    WHERE delete_date IS NULL
    GROUP BY employee_id
)
SELECT
    c.employee_id,
    c.full_name,
    c.departement,
    c.position,
    c.nature_conge,
    c.motif,
    c.adresse,
    c.reliquat,
    c.periode_debut,
    c.periode_fin,
    c.num_conge
FROM conge c
INNER JOIN LatestConge lc ON c.employee_id = lc.employee_id AND c.num_conge = lc.latest_num_conge
WHERE c.delete_date IS NULL


