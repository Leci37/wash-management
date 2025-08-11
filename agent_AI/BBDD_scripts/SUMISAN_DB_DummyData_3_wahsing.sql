USE SUMISAN;
GO

-- Insert 30 dummy WASHINGS
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080101, 2, 11, 11, '2025-08-01 08:00:00', NULL, 'P', 'Ciclo estándar', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080102, 3, 10, NULL, '2025-08-01 09:00:00', '2025-08-01 11:00:00', 'F', 'Revisión visual previa', 'Alta carga en bandeja');
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080103, 1, 8, 8, '2025-08-01 10:00:00', NULL, 'P', 'Proceso urgente', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080104, 2, 9, NULL, '2025-08-01 11:00:00', NULL, 'P', 'Validación de detergente', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080105, 1, 1, 1, '2025-08-01 12:00:00', NULL, 'P', 'Secado prolongado', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080106, 2, 11, NULL, '2025-08-01 13:00:00', NULL, 'P', 'Validación de detergente', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080107, 3, 3, 3, '2025-08-01 14:00:00', NULL, 'P', 'Cambio de detergente', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080108, 2, 10, 10, '2025-08-01 15:00:00', '2025-08-01 17:00:00', 'F', 'Material etiquetado erróneamente', 'Secado prolongado');
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080109, 3, 9, 9, '2025-08-01 16:00:00', NULL, 'P', 'Proceso urgente', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080110, 2, 7, NULL, '2025-08-01 17:00:00', NULL, 'P', 'Material etiquetado erróneamente', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080111, 1, 6, 6, '2025-08-01 18:00:00', '2025-08-01 20:00:00', 'F', 'Inicio programado', 'Alta carga en bandeja');
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080112, 1, 2, NULL, '2025-08-01 19:00:00', NULL, 'P', 'Inicio programado', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080113, 2, 9, NULL, '2025-08-01 20:00:00', '2025-08-01 22:00:00', 'F', 'Muestra de control incluida', 'Instrumental completo');
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080114, 2, 8, NULL, '2025-08-01 21:00:00', NULL, 'P', 'Ciclo estándar', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080115, 2, 6, 6, '2025-08-01 22:00:00', NULL, 'P', 'Revisión visual previa', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080116, 4, 4, 4, '2025-08-01 23:00:00', '2025-08-02 01:00:00', 'F', 'Secado prolongado', 'Proceso urgente');
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080117, 3, 12, NULL, '2025-08-02 00:00:00', NULL, 'P', 'Instrumental completo', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080118, 2, 7, 7, '2025-08-02 01:00:00', '2025-08-02 03:00:00', 'F', 'Revisión visual previa', 'Revisión visual previa');
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080119, 3, 2, 2, '2025-08-02 02:00:00', NULL, 'P', 'Revisión visual previa', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080120, 2, 4, NULL, '2025-08-02 03:00:00', NULL, 'P', 'Material etiquetado erróneamente', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080121, 2, 3, NULL, '2025-08-02 04:00:00', '2025-08-02 06:00:00', 'F', 'Instrucciones especiales', 'Material sensible presente');
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080122, 3, 1, NULL, '2025-08-02 05:00:00', '2025-08-02 07:00:00', 'F', 'Alta carga en bandeja', 'Inicio programado');
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080123, 4, 10, 10, '2025-08-02 06:00:00', NULL, 'P', 'Sello de seguridad intacto', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080124, 3, 12, NULL, '2025-08-02 07:00:00', '2025-08-02 09:00:00', 'F', 'Muestra de control incluida', 'Material sensible presente');
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080125, 4, 10, 10, '2025-08-02 08:00:00', '2025-08-02 10:00:00', 'F', 'Material etiquetado erróneamente', 'Cambio de detergente');
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080126, 1, 11, NULL, '2025-08-02 09:00:00', NULL, 'P', 'Muestra de control incluida', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080127, 4, 5, NULL, '2025-08-02 10:00:00', '2025-08-02 12:00:00', 'F', 'Lavado manual previo', 'Material etiquetado erróneamente');
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080128, 2, 8, NULL, '2025-08-02 11:00:00', NULL, 'P', 'Material sensible presente', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080129, 1, 2, NULL, '2025-08-02 12:00:00', NULL, 'P', 'Retraso por mantenimiento', NULL);
INSERT INTO WASHINGS (WashingId, MachineId, StartUserId, EndUserId, StartDate, EndDate, Status, StartObservation, FinishObservation)
VALUES (25080130, 1, 12, NULL, '2025-08-02 13:00:00', '2025-08-02 15:00:00', 'F', 'Sello de seguridad intacto', 'Instrucciones especiales');