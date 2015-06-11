Feature: AddInspectionReport
	Target: POST api/Organization/Save
	Test Scenarios:
		1. Create a new Inspection Report;
		2. Create a new Inspection Report with line items.


@p1
Scenario: Create Inspection Report
	Given Inspection Report information as following
	| AttributePhysicalName | Value             |
	| PImpProjAutoID        | 1111 - 1111       |
	| PContractAutoID       | cofee02 - cofee02 |
	| ContractPerson        | 12 - 12           |
	| InspectorID           | 1                 |
	| WorkPerformed         | WorkPerformed     |
	When Post Inspection Report information to api 'Organization/Save'
	Then A new Inspection Report should be created and all fields saved correctly

@p1
Scenario: Create Inspection Report With Line Items
	Given Inspection Report information as following
	| AttributePhysicalName | Value             |
	| PImpProjAutoID        | 1111 - 1111       |
	| PContractAutoID       | cofee02 - cofee02 |
	| ContractPerson        | 12 - 12           |
	| InspectorID           | 1                 |
	| WorkPerformed         | WorkPerformed     |
	And Input line items at 'Labor' section
	| AttributePhysicalName | Value       |
	| SOVItemAutoID         | 1.6 - Bonds |
	When Post Inspection Report information to api 'Organization/Save'
	Then A new Inspection Report should be created and all fields saved correctly