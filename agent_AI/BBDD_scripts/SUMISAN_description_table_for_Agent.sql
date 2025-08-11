USE SUMISAN;
GO

-- Step 1: Collect row counts per table
IF OBJECT_ID('tempdb..#TableRowCounts') IS NOT NULL DROP TABLE #TableRowCounts;

SELECT 
    t.name AS TableName,
    SUM(p.rows) AS TotalRows
INTO #TableRowCounts
FROM sys.tables t
JOIN sys.partitions p ON t.object_id = p.object_id
WHERE p.index_id IN (0, 1)
GROUP BY t.name;

-- Step 2: Join with PKs and FKs
SELECT 
    r.TableName,
    r.TotalRows,
    pk.name AS PrimaryKeyName,
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS FK_FromTable,
    c1.name AS FK_FromColumn,
    OBJECT_NAME(fk.referenced_object_id) AS FK_ToTable,
    c2.name AS FK_ToColumn
FROM #TableRowCounts r
LEFT JOIN sys.tables t ON t.name = r.TableName
LEFT JOIN sys.indexes pk ON pk.object_id = t.object_id AND pk.is_primary_key = 1
LEFT JOIN sys.foreign_keys fk ON fk.parent_object_id = t.object_id
LEFT JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
LEFT JOIN sys.columns c1 ON fkc.parent_object_id = c1.object_id AND fkc.parent_column_id = c1.column_id
LEFT JOIN sys.columns c2 ON fkc.referenced_object_id = c2.object_id AND fkc.referenced_column_id = c2.column_id
ORDER BY r.TableName;

-- Clean up
DROP TABLE #TableRowCounts;