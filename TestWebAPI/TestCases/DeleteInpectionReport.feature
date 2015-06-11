Feature: DeleteInspectionReport
	Target: DELETE api/Organization/Delete/{entityName}/{id}
	Test Scenarios:
		1. Delete a new Inspection Report;
		2. Delete a non-existing Inspection Report;

@p1
Scenario: Delete Inspection Report
	Given Create a new Inspection Report
	| AttributePhysicalName | Value             |
	| PImpProjAutoID        | 1111 - 1111       |
	| PContractAutoID       | cofee02 - cofee02 |
	| ContractPerson        | 12 - 12           |
	| InspectorID           | 1                 |
	| WorkPerformed         | WorkPerformed     |
	When Send a request to api:'Organization/Delete' to delete this Inspection Report
	Then Target Inspection Report should be deleted
