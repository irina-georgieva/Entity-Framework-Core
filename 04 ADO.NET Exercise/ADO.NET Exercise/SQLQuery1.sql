USE [MinionsDB]

-- P02 Villain Names

SELECT [v].[Name],
	COUNT ([mv].[MinionId]) AS [MinionsCount]
	FROM [Villains] AS [v]
	LEFT JOIN [MinionsVillains] AS [mv]
	ON [v].[Id] = [mv].[VillainId]
	GROUP BY [v].[Name]
	HAVING COUNT([mv].[MinionId]) > 3
	ORDER BY [MinionsCount]

-- P03 Minion Names
SELECT [Name]
	FROM [Villains]
	WHERE [Id] = 1


SELECT [m].[Name], [m].[Age]
	FROM [MinionsVillains] AS [mv]
	LEFT JOIN [Minions] AS [m]
	ON [m].[Id] = [mv].[MinionId]
	WHERE [mv].[VillainId] = 1
	ORDER BY [m].[Name]

-- P04 Add Minion
SELECT [Id]
	FROM [Towns]
	WHERE [Name] = 'London'

INSERT INTO [Towns]([Name])
	VALUES
	('Sofia')

SELECT [Id]
	FROM [Villains]
	WHERE [Name] = 'Gru'

SELECT [Id]
	FROM [EvilnessFactors]
	WHERE [Name] = 'Evil'

INSERT INTO [Villains]([Name], [EvilnessFactorId])
	 VALUES
		('New Name', 4)

INSERT INTO [Minions]([Name], [Age], [TownId])
	VALUES
	('Name', 'Age', 'TownId')

SELECT [Id]
	FROM [Minions]	
	WHERE [Name] = 'Name' AND [Age] = 16 AND [TownId] = 1

INSERT INTO [MinionsVillains]([MinionId], [VillainId])
	VALUES
	(1,1)

--P06 Remove Villain

SELECT [Name]
	FROM [Villains]
	WHERE [Id] = 1

DELETE FROM [MinionsVillains]
	WHERE [VillainId] = 1

DELETE FROM [Villains]
	WHERE [Id] = 1

-- P09 Increase Age Stored Procedure
EXEC [dbo].[usp_GetOlder] 1

SELECT [Name], [Age]
	FROM [Minions]
	WHERE [Id] = 1